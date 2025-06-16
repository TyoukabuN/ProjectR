using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Pool;

#if UNITY_EDITOR
using Sirenix.Utilities;
using UnityEditor;
#endif

namespace PJR.BlackBoard.CachedValueBoard
{
    public class CacheableValueBoard
    {
        [OdinSerialize,LabelText("黑板值")]
        private Dictionary<string, ICacheableValue> _key2Value = new();
        public Dictionary<string, ICacheableValue> Key2Value => _key2Value;
     
        public bool TryGetIndexMap(out IndexMap indexMap)
        {
            indexMap = IndexMap.Empty;
            if (_key2Value == null)
                return false;            
            
            foreach (var pair in _key2Value)
            {
                if (pair.Value == null)
                {
                    Debug.LogWarning($"Exist Null Value! [key: {pair.Key}]");
                    continue;
                }

                if (!pair.Value.CacheToBuffer(out Type valueType,out var index, out var guid))
                {
                    Debug.LogError($"Failure to cache value to buffer! [key: {pair.Key}]");
                    return false;
                }
                indexMap.Add(pair.Key,valueType, index, guid);
            }
            return true;
        }
        
        /// <summary>
        /// 1）这个版本用了IToBufferToken:interface来wrap住所有需要的数据<br/>
        /// 2）用一个structure wrap住是为了减少<see cref="IndexMap.KeyIndexPair"/>的Construct方法的修改 <br/>
        /// 3）但是返回值是一个接口所以会触发boxing，发生Alloc <br/>
        /// 4）没什么特殊需求的话还是建议使用<see cref="CacheableValueBoard.TryGetIndexMap"/>
        /// </summary>
        /// <param name="indexMap"></param>
        /// <returns></returns>
        public bool TryGetIndexMap_Token(out IndexMap indexMap)
        {
            indexMap = IndexMap.Empty;
            if (_key2Value == null)
                return false;            
            
            foreach (var pair in _key2Value)
            {
                if (pair.Value == null)
                {
                    Debug.LogWarning($"Exist Null Value! [key: {pair.Key}]");
                    continue;
                }

                if (!pair.Value.CacheToBuffer(out var token))
                {
                    Debug.LogError($"Failure to cache value to buffer! [key: {pair.Key}]");
                    return false;
                }
                indexMap.Add(pair.Key, token);
            }
            return true;
        }

        public bool ApplyIndexMap(IndexMap indexMap, bool clearBuffer = false)
        {
            if (indexMap.Length <= 0)
                return false;
            if (_key2Value == null || _key2Value.Count <= 0)
                return false;
            for (int i = 0; i < indexMap.Length; i++)
            {
                var pair = indexMap[i];

                if (!_key2Value.TryGetValue(pair.key, out var cachedValue))
                    if(!TryAddValue(pair.ValueType, pair.key,out cachedValue))
                        continue;
                cachedValue.WriteFromBuffer(pair.Index, pair.GUID, clearBuffer);
            }
            return true;
        }
        public bool OverrideTo(ICachedValueBoardHolder holder)
        {
            var destBoard = holder?.GetCachedValueBoard();
            if (destBoard == null)
                return false;
            return OverrideTo(destBoard);
        }

        public bool OverrideTo(CacheableValueBoard targetBoard)
        {
            if (targetBoard == null)
                return false;
            if (!TryGetIndexMap(out var indexMap) || indexMap.Invalid)
                return false;
            return targetBoard.ApplyIndexMap(indexMap, true);
        }
        public bool TryAddValue(Type type, string key) => TryAddValue(type, key, out _);
        public bool TryAddValue(Type type, string key, out ICacheableValue cacheableValue)
        {
            cacheableValue = null;
            if (_key2Value.ContainsKey(key))
                return false;
            Type genericType = typeof(CacheableValue<>).MakeGenericType(type);
            cacheableValue = Activator.CreateInstance(genericType) as ICacheableValue;
            if (cacheableValue == null)
                return false;
            _key2Value.Add(key,cacheableValue);
            return true;
        }

        public ICacheableValue[] FindValuesOfType(Type type)
        {
            if (_key2Value == null || _key2Value.Count == 0)
                return Array.Empty<ICacheableValue>();

            var temp = ListPool<ICacheableValue>.Get();
            foreach (var pair in _key2Value)
            {
                if (pair.Value != null && pair.Value.ValueType == type)
                    temp.Add(pair.Value);
            }
            var res = temp.ToArray();
            ListPool<ICacheableValue>.Release(temp);
            return res;
        }
        
        public KeyValuePair<string,ICacheableValue>[] FindKeyValuesOfType(Type type)
        {
            if (_key2Value == null || _key2Value.Count == 0)
                return Array.Empty<KeyValuePair<string,ICacheableValue>>();

            var temp = ListPool<KeyValuePair<string,ICacheableValue>>.Get();
            foreach (var pair in _key2Value)
            {
                if (pair.Value != null && pair.Value.ValueType == type)
                    temp.Add(pair);
            }
            var res = temp.ToArray();
            ListPool<KeyValuePair<string,ICacheableValue>>.Release(temp);
            return res;
        }
        
        public bool TryGetValue<T>(string key, out T value)
        {
            value = default;
            if (string.IsNullOrEmpty(key))
                return false;
            if (!_key2Value.TryGetValue(key, out var cacheableValue))
                return false;
            value = (T)cacheableValue.GetValue();
            return true;
        }

#if UNITY_EDITOR

        private bool editor_showNewValueRect = false;

        [LabelText("Key"), BoxGroup("新黑板值",VisibleIf = "@editor_showNewValueRect"), ShowInInspector]
        [InfoBox("$editor_error", InfoMessageType.Error, "Editor_AnyError")]
        private string editor_newKeyAddBoardValue;
        [LabelText("ValueType"),BoxGroup("新黑板值"), ShowInInspector]
        [TypeFilter("Editor_GetFilteredTypeList", DrawValueNormally = true)]
        [InlineButton("Editor_ChangeTypeFilterMode","@Editor_GetTypeFilterLabel()")]
        private Type editor_newValueType;
        
        private string editor_error;
        private bool Editor_AnyError()=>  !string.IsNullOrEmpty(editor_error);

        [Button("添加"),HorizontalGroup("新黑板值/btns")]
        private void Editor_AddBoardValueButton()
        {
            string key = editor_newKeyAddBoardValue;
            if (string.IsNullOrEmpty(key))
            {
                editor_error = "key为空";
                return;
            }

            Type type = editor_newValueType;
            if (type == null)
            {
                editor_error = "类型为空";
                return;
            }
            _key2Value ??= new Dictionary<string, ICacheableValue>();
            if (_key2Value.ContainsKey(key))
            {
                editor_error = $"已包含{key}";
                return;
            }

            TryAddValue(editor_newValueType, key);
            editor_error = null;
        }
        [Button("取消"),HorizontalGroup("新黑板值/btns")]
        private void Editor_CancelButton()
        {
            editor_showNewValueRect = false;
            editor_error = null;
        }
        
        [Button("添加黑板值"),ShowIf("@!editor_showNewValueRect")]
        private void Editor_ShowAddBoardValueGroup()
        {
            editor_showNewValueRect = true;
        }

        private static TypeFilterTypeMode editor_typeFilterAttribute = 0;
        [Flags]
        public enum TypeFilterTypeMode
        {
            Preference = 0,
            Scripts = 1,
            ImportedAssemblies = 2,
            UnityEngine = 3,
            DotNetRuntime = 4,
            DynamicAssemblies = 5,
            Unknown = 6,
            ProjectSpecific = 7,
            All,
        }
        
        private static Dictionary<TypeFilterTypeMode, AssemblyCategory> TypeFilterTypeMode2AssemblyCategory = new()
        {
            {TypeFilterTypeMode.Scripts, AssemblyCategory.Scripts},
            {TypeFilterTypeMode.ImportedAssemblies, AssemblyCategory.ImportedAssemblies},
            {TypeFilterTypeMode.UnityEngine, AssemblyCategory.UnityEngine},
            {TypeFilterTypeMode.DotNetRuntime, AssemblyCategory.DotNetRuntime},
            {TypeFilterTypeMode.DynamicAssemblies, AssemblyCategory.DynamicAssemblies},
            {TypeFilterTypeMode.Unknown, AssemblyCategory.Unknown},
            {TypeFilterTypeMode.ProjectSpecific, AssemblyCategory.ProjectSpecific},
            {TypeFilterTypeMode.All, AssemblyCategory.All},
        };
            
        public static List<Type> PreferenceType = new List<Type>()
        {
            typeof(int),
            typeof(float),
            typeof(double),
            typeof(string),
            typeof(GameObject),
            typeof(Transform),
        };

        private static List<Type> _allTypeIEnumerableCache;
        public IEnumerable<Type> Editor_GetFilteredTypeList()
        {
            if (editor_typeFilterAttribute == TypeFilterTypeMode.Preference)
            {
                return PreferenceType;
            }

            if (!TypeFilterTypeMode2AssemblyCategory.TryGetValue(editor_typeFilterAttribute, out var assemblyCategory))
                return null;

            return AssemblyUtilities.GetTypes(assemblyCategory);
        }

        public void Editor_ChangeTypeFilterMode()
        {
            int temp = (int)editor_typeFilterAttribute;
            temp++;
            temp = temp % ((int)TypeFilterTypeMode.All + 1);
            editor_typeFilterAttribute = (TypeFilterTypeMode)temp;
        }

        public string Editor_GetTypeFilterLabel()
        {
            return editor_typeFilterAttribute switch {
                TypeFilterTypeMode.Preference => "Pref",
                TypeFilterTypeMode.All => "All",
                _ => editor_typeFilterAttribute.ToString(),
            };
        }
#endif
    }
}
