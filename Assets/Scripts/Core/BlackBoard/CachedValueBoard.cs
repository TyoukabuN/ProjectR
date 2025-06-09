using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PJR.BlackBoard.CachedValueBoard
{
    public class CachedValueBoard
    {
        [OdinSerialize,LabelText("黑板值")]
        private Dictionary<string, ICachedValue> _key2Value = new();
        public Dictionary<string, ICachedValue> Key2Value => _key2Value;
     
        public bool TryGetCacheValue(out IndexMap indexMap)
        {
            indexMap = IndexMap.Empty;
            if (_key2Value == null)
                return false;
            foreach (var pair in _key2Value)
            {
                if (!pair.Value.ToBuffer(out int index))
                {
                    Debug.LogError($"failure to cache value to buffer [key: {pair.Key}]");
                    return false;
                }
     
                indexMap.Add(pair.Key, index);
            }
            return true;
        }
     
        public bool ApplyIndexMap(IndexMap indexMap)
        {
            if (indexMap.Length <= 0)
                return false;
            if (_key2Value == null || _key2Value.Count <= 0)
                return false;
            foreach (var pair in indexMap.Indexes())
            {
                if(!_key2Value.TryGetValue(pair.key, out ICachedValue cachedValue))
                    continue;
                cachedValue.FromBuffer(pair.index);
            }
     
            return true;
        }
#if UNITY_EDITOR

        private bool editor_showNewValueRect = false;

        [LabelText("Key"), FoldoutGroup("新黑板值",Expanded = true, VisibleIf = "@editor_showNewValueRect"), ShowInInspector]
        private string editor_newKeyAddBoardValue;
        [LabelText("ValueType"),FoldoutGroup("新黑板值"), ShowInInspector]
        private Type editor_newValueType; 
        [Button("添加"),HorizontalGroup("新黑板值/btns")]
        private void Editor_AddBoardValueButton()
        {
            string key = editor_newKeyAddBoardValue;
            if (string.IsNullOrEmpty(key))
                return;
            _key2Value ??= new Dictionary<string, ICachedValue>();
            if (_key2Value.ContainsKey(key))
                return;
            Type genericType = typeof(CachedValue<>).MakeGenericType(editor_newValueType);
            ICachedValue obj = Activator.CreateInstance(genericType) as ICachedValue;
            _key2Value.Add(key,obj);
        }
        [Button("取消"),HorizontalGroup("新黑板值/btns")]
        private void Editor_CancelButton()
        {
            editor_showNewValueRect = false;
        }
        
        [Button("添加黑板值"),ShowIf("@!editor_showNewValueRect")]
        private void Editor_ShowAddBoardValueGroup()
        {
            editor_showNewValueRect = true;
        }
#endif
    }
}
