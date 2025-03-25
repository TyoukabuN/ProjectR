using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PJR
{
    [FlagConfigMark]
    public class FlagConfig
    {
        public static int categoryInterval = 1000000;
        public static int kindInterval = 1000;

        public static class Category
        {
            public const int Input_CharacterAction = 1000000;
            public const int Input_EnemyAction = 2000000;
            public const int Input_SailAction = 3000000;
            public const int Input_FishingAction = 4000000;
            public const int State_CharacterState = 5000000;
            public const int State_EnemyState = 6000000;
            public const int Behaviour_CharacterBehaviour = 7000000;
            public const int Behaviour_EnemyBehaviour = 8000000;
        }
        public string FileName => string.Empty;

        public const string FileFileTag = "config_flag";
        public string FileTag => FileFileTag;

        private static FlagConfig instance;

        private bool _initialized = false;
        private string _error = string.Empty;

        private Dictionary<int, FlagDefine> id2FlagDefine = new Dictionary<int, FlagDefine>();
        private Dictionary<string, FlagDefine> key2FlagDefine = new Dictionary<string, FlagDefine>();
        private Dictionary<int, FlagDefineSet> id2FlagDefineSet = new Dictionary<int, FlagDefineSet>();
        private Dictionary<int, List<FlagDefine>> category2FlagDefineList = new Dictionary<int, List<FlagDefine>>();
        private Dictionary<int, List<FlagDefineSet>> category2FlagDefineSetList = new Dictionary<int, List<FlagDefineSet>>();

        private List<FlagDefineSet> sortedDefineSet = new List<FlagDefineSet>();
        private List<FlagDefine> sortedCategoryDefines = new List<FlagDefine>();

        static FlagConfig()
        {
            instance = instance ?? new FlagConfig();
        }
        public bool CheckValid()
        {
            if (!string.IsNullOrEmpty(_error))
                return false;
            if (_initialized)
                return true;
            Initialize();
            return _initialized;
        }
        public void Reset()
        {
            id2FlagDefine = new Dictionary<int, FlagDefine>();
            key2FlagDefine = new Dictionary<string, FlagDefine>();
            id2FlagDefineSet = new Dictionary<int, FlagDefineSet>();
            category2FlagDefineList = new Dictionary<int, List<FlagDefine>>();
            category2FlagDefineSetList = new Dictionary<int, List<FlagDefineSet>>();

            sortedDefineSet = new List<FlagDefineSet>();
            sortedCategoryDefines = new List<FlagDefine>();
        }
        public void Initialize()
        {
            var defineSetList = LoadAllFlagDefineSet();
            for (int i = 0; i < defineSetList.Count; i++)
            {
                var defineSet = defineSetList[i];
                if (defineSet == null)
                    continue;
                var info = FlagIDInfo.GetInfo(defineSet.id);

                sortedDefineSet.Add(defineSet);
                id2FlagDefineSet[defineSet.ID] = defineSet;
                if (!category2FlagDefineSetList.TryGetValue(info.categoryID, out var flagDefineList))
                {
                    flagDefineList = new List<FlagDefineSet>();
                    category2FlagDefineSetList[info.categoryID] = flagDefineList;
                }
                flagDefineList.Add(defineSet);

                for (int j = 0; j < defineSet.FlagDefines.Count; j++)
                {
                    AddFlagDefine(defineSet.FlagDefines[j]);
                }
            }
            if (sortedDefineSet.Count > 0)
                sortedDefineSet.Sort((a, b) => a.ID.CompareTo(b.ID));
            if (sortedCategoryDefines.Count > 0)
                sortedCategoryDefines.Sort((a, b) => a.ID.CompareTo(b.ID));

            _initialized = true;
        }


        public const string FlagConfigFileRoot = "Assets/Dev/Prefabs/ConfigAssets/Flag";
        public const string FlagDefineSetFilter = "t:FlagDefineSet";
        /// <summary>
        /// 加载配置
        /// </summary>
        /// <returns></returns>
        public List<FlagDefineSet> LoadAllFlagDefineSet()
        {
            List<FlagDefineSet> list = new List<FlagDefineSet>();
#if UNITY_EDITOR
            if (Application.isEditor)
            {
                var guids = AssetDatabase.FindAssets(FlagDefineSetFilter, new string[] { FlagConfigFileRoot });
                for (int i = 0; i < guids.Length; i++)
                {
                    var guid = guids[i];
                    var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    if (string.IsNullOrEmpty(assetPath))
                        continue;
                    var defineSet = AssetDatabase.LoadAssetAtPath<FlagDefineSet>(assetPath) as FlagDefineSet;
                    if (defineSet == null)
                        continue;
                    list.Add(defineSet);
                }
            }
            else
#endif
            {
                //var assetInfos = ResourceSystem.GetAssetInfos(FileTag);
                //for (int i = 0; i < assetInfos.Length; i++)
                //{
                //    var info = assetInfos[i];
                //    var handle = ResourceSystem.LoadAssetSync(this, info);
                //    if (handle == null || !handle.IsValid)
                //    {
                //        Debug.Log($"failure to loaded FlagDefineSet {info.Address}");
                //        continue;
                //    }
                //    var defineSet = handle.AssetObject as FlagDefineSet;
                //    if (defineSet == null)
                //        continue;

                //    list.Add(defineSet);
                //}
            }
            return list;
        }

        /// <summary>
        /// 记录
        /// </summary>
        /// <param name="flagDefine"></param>
        /// <param name="returnIfExist"></param>
        private void AddFlagDefine(FlagDefine flagDefine, bool returnIfExist = false)
        {
            if (flagDefine == null)
                return;
            //记录id Map
            if (id2FlagDefine.ContainsKey(flagDefine.id))
            {
                Debug.LogWarning($"存在相同id的flag定义 ①：{id2FlagDefine[flagDefine.id].id}  ②：{flagDefine.id}");
                if (!returnIfExist)
                    id2FlagDefine[flagDefine.id] = flagDefine;
            }
            else
                id2FlagDefine[flagDefine.id] = flagDefine;

            //记录key Map
            if (!string.IsNullOrEmpty(flagDefine.Key))
            {
                if (key2FlagDefine.ContainsKey(flagDefine.Key))
                {
                    Debug.LogWarning($"存在相同key的flag定义 ①：[{key2FlagDefine[flagDefine.Key].id}]{key2FlagDefine[flagDefine.Key].Key}  ②：[{flagDefine.ID}]{flagDefine.Key}");
                    if (!returnIfExist)
                        key2FlagDefine[flagDefine.Key] = flagDefine;
                }
                else
                    key2FlagDefine[flagDefine.Key] = flagDefine;
            }
            else
                Debug.LogWarning($"存在key为空的flag定义 [id]: [{flagDefine.id}]");

            //记录category Map
            var category = flagDefine.CategoryID;
            if (flagDefine.Category > 0)
            {
                if (!category2FlagDefineList.TryGetValue(category, out var list))
                {
                    list = new List<FlagDefine>();
                    category2FlagDefineList.Add(category, list);
                }
                list.Add(flagDefine);
            }
            //记录大类flagDefine
            if (IsCategoryID(flagDefine.id))
            {
                sortedCategoryDefines.Add(flagDefine);
            }
        }

        /// <summary>
        /// 用keys获取flag组合
        /// </summary>
        /// <param name="category"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static Flag512 GetFlagByKeys(int category, params string[] keys)
        {
            var res = Flag512.Empty;
            if (!instance.CheckValid())
                return res;
            if (!TryGetFlagDefineByCategory(category, out var list))
                return res;

            for (int i = 0; i < keys.Length; i++)
            {
                var key = keys[i];
                for (int j = 0; j < list.Count; j++)
                {
                    var flagDefine = list[j];
                    if (flagDefine.Key == key)
                        res.FlagOr(FlagManager512.GetFlag(flagDefine));
                }
            }
            return res;
        }

        /// <summary>
        /// 用id找flag定义
        /// ！！修改这个函数的【名字】或【参数列表】的时候要同时修改FlagRuntimeEditorUtil里的对应函数
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static FlagDefine GetFlagDefine(int id)
        {
            if (!instance.CheckValid())
                return null;
            if (!instance.id2FlagDefine.TryGetValue(id, out var flagDefine))
                return null;
            return flagDefine;
        }
        public static List<FlagDefine> GetFlagDefinesByCategory(int category)
        {
            if (!instance.CheckValid())
                return null;
            if (!instance.category2FlagDefineList.TryGetValue(category, out var list))
                return null;
            return list;
        }
        public static bool TryGetFlagDefineByCategory(int category, out List<FlagDefine> list)
        {
            list = null;
            if (!instance.CheckValid())
                return false;
            return instance.category2FlagDefineList.TryGetValue(category, out list);
        }

        public static FlagDefine GetFlagDefineByKey(string key, int category = -1)
        {
            if (!instance.CheckValid())
                return null;
            FlagDefine flagDefine = null;
            if (category < 0)
            {
                if (!instance.key2FlagDefine.TryGetValue(key, out flagDefine))
                    return null;
            }
            else
            {
                if (TryGetFlagDefineByCategory(category, out var list))
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].Key == key)
                            return list[i];
                    }
                }
            }
            return flagDefine;
        }
        public static bool TryGetFlagDefine(int id, out FlagDefine flagDefine)
        {
            flagDefine = GetFlagDefine(id);
            return flagDefine != null;
        }
        public static bool TryGetFlagDefineByKey(string key, out FlagDefine flagDefine, int category = -1)
        {
            flagDefine = GetFlagDefineByKey(key, category);
            return flagDefine != null;
        }

        /// <summary>
        /// 用id找flag定义集
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static FlagDefineSet GetFlagDefineSet(int id)
        {
            if (!instance.CheckValid())
                return null;
            if (!instance.id2FlagDefineSet.TryGetValue(id, out var flagDefineSet))
                return null;
            return flagDefineSet;
        }

        public static bool IsBelongCategory(int category, int id)
        {
            if (id < category)
                return false;
            if (id >= category + categoryInterval)
                return false;
            return true;
        }
        public static int GetCategory(int id)
        {
            int category = (id / categoryInterval) * categoryInterval;
            return category;
        }
        public static bool IsValidCategory(int category)
        {
            if (category < categoryInterval)
                return false;
            return category % categoryInterval == 0;
        }
        public static bool IsCategoryID(int category)
        {
            return IsValidCategory(category);
        }

        #region Editor

#if UNITY_EDITOR
        public static Dictionary<int, FlagDefine> Editor_Id2FlagDefine => instance.CheckValid() ? instance.id2FlagDefine : null;
        public static Dictionary<string, FlagDefine> Editor_Key2FlagDefine => instance.CheckValid() ? instance.key2FlagDefine : null;
        public static Dictionary<int, FlagDefineSet> Editor_Id2FlagDefineSet => instance.CheckValid() ? instance.id2FlagDefineSet : null;
        public static Dictionary<int, List<FlagDefine>> Editor_Category2FlagDefineList => instance.CheckValid() ? instance.category2FlagDefineList : null;
        public static Dictionary<int, List<FlagDefineSet>> Editor_Category2FlagDefineSetList => instance.CheckValid() ? instance.category2FlagDefineSetList : null;
        public static List<FlagDefineSet> Editor_SortedDefineSet => instance.CheckValid() ? instance.sortedDefineSet : null;
        public static List<FlagDefine> Editor_SortedCategoryDefines => instance.CheckValid() ? instance.sortedCategoryDefines : null;

        public static void Editor_RefreshConfig()
        {
            instance.Reset();
            instance.Initialize();
        }
        /// <summary>
        /// ！！修改这个函数的【名字】或【参数列表】的时候要同时修改FlagRuntimeEditorUtil里的对应函数
        /// </summary>
        /// <param name="flagID"></param>
        /// <returns></returns>
        public static string Editor_GetMenuName(int flagID)
        {
            string menu = string.Empty;
            var flagDefine = FlagConfig.GetFlagDefine(flagID);
            if (flagDefine == null)
                return string.Empty;

            return Editor_GetFlagDefineMenuName(flagDefine);
        }
        /// <summary>
        /// ！！修改这个函数的【名字】或【参数列表】的时候要同时修改FlagRuntimeEditorUtil里的对应函数
        /// </summary>
        /// <param name="flagDefine"></param>
        /// <returns></returns>
        public static string Editor_GetFlagDefineMenuName(FlagDefine flagDefine)
        {
            string menu = string.Empty;
            if (flagDefine == null)
                return string.Empty;

            menu = flagDefine.Name;

            var flagIDInfo = FlagIDInfo.GetInfo(flagDefine.ID);
            if (flagIDInfo == null || !flagIDInfo.IsValid())
                return menu;

            var categorySet = FlagConfig.GetFlagDefineSet(flagIDInfo.categoryID);
            if (categorySet != null)
            {
                menu = menu.Replace(categorySet.Name, string.Empty);
                if (IsCategoryID(flagDefine.ID))
                    menu = $"{categorySet.Name}/[大类Flag]";
                else
                    menu = $"{categorySet.Name}/{menu.TrimStart('/')}";
            }

            return menu;
        }


        /// <summary>
        /// 显示flag选项菜单
        /// ！！修改这个函数的【名字】或【参数列表】的时候要同时修改FlagRuntimeEditorUtil里的对应函数
        /// </summary>
        /// <param name="callback">选项回调</param>
        /// <param name="category">显示对应大类选项</param>
        public static void Editor_ShowFlagGenericMenu(Action<FlagDefine> callback = null, int category = -1)
        {
            if (!instance.CheckValid())
                return;

            GenericMenu menu = new GenericMenu();

            Editor_FillGenericMenu(menu, callback, false, category);

            menu.ShowAsContext();
        }
        /// <summary>
        /// ！！修改这个函数的【名字】或【参数列表】的时候要同时修改FlagRuntimeEditorUtil里的对应函数
        /// </summary>
        /// <param name="callback">选择回调</param>
        /// <param name="categoryOnly">只显示大类</param>
        /// <param name="categorys">要显示的大类</param>
        public static void Editor_ShowFilteredFlagGenericMenu(Action<FlagDefine> callback = null, bool categoryOnly = false, params int[] categorys)
        {
            if (!instance.CheckValid())
                return;

            if (categorys == null || categorys.Length <= 0)
            {
                Editor_ShowFlagGenericMenu();
                return;
            }

            GenericMenu menu = new GenericMenu();

            for (int i = 0; i < categorys.Length; i++)
            {
                Editor_FillGenericMenu(menu, callback, categoryOnly, categorys[i]);
            }

            menu.ShowAsContext();
        }

        /// <summary>
        /// 填充Flag选项列表
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="callback"></param>
        /// <param name="categoryOnly">只显示大类Flag选项</param>
        /// <param name="category"></param>
        public static void Editor_FillGenericMenu(GenericMenu menu, Action<FlagDefine> callback = null, bool categoryOnly = false, int category = -1)
        {
            if (!instance.CheckValid())
                return;

            var isValidCategory = IsValidCategory(category);
            if (!isValidCategory && category > categoryInterval)
                Debug.LogWarning($"无效Flag大类 [category]: {category}");
            //除了-1之外的无效大类都返回
            if (!isValidCategory && category > categoryInterval)
                return;


            for (int i = 0; i < instance.sortedDefineSet.Count; i++)
            {
                var defineSet = instance.sortedDefineSet[i];
                var isBelongCategory = IsBelongCategory(category, defineSet.ID);

                if (categoryOnly)
                {
                    if (
                        (!isValidCategory) ||
                        (isValidCategory && isBelongCategory)
                        )
                    {
                        var flagDefine = instance.id2FlagDefine[category];
                        string menuName = Editor_GetFlagDefineMenuName(flagDefine);
                        menu.AddItem(new GUIContent(menuName), false, () =>
                        {
                            callback?.Invoke(flagDefine);
                        });
                    }
                    continue;
                }

                if (isValidCategory && !isBelongCategory)
                    continue;

                for (int j = 0; j < defineSet.FlagDefines.Count; j++)
                {
                    var flagDefine = defineSet.FlagDefines[j];
                    if (IsCategoryID(flagDefine.ID))
                        continue;
                    string menuName = Editor_GetFlagDefineMenuName(flagDefine);
                    menu.AddItem(new GUIContent(menuName), false, () =>
                    {
                        callback?.Invoke(flagDefine);
                    });
                }
            }
        }

        /// <summary>
        /// 保存所有配置
        /// </summary>
        public static void Editor_SetDirtyAndSaveAllConfig()
        {
            foreach (var set in Editor_SortedDefineSet)
            {
                EditorUtility.SetDirty(set);
            }
            AssetDatabase.SaveAssets();
        }
        /// <summary>
        /// 获取大类下最大的小类ID
        /// </summary>
        /// <param name="categoryID"大类ID，比如1000000</param>
        /// <returns></returns>
        public static int Editor_GetMaximumKindID(int categoryID)
        {
            if (Editor_Category2FlagDefineSetList.Count <= 0)
                return 0;
            return Editor_Category2FlagDefineSetList[categoryID].Max(set =>
            {
                return set == null ? 0 : set.ID;
            });
        }
        /// <summary>
        /// 获取当前最大的大类ID
        /// </summary>
        /// <returns></returns>
        public static int Editor_GetMaximumCategoryID()
        {
            if (Editor_Category2FlagDefineList.Count <= 0)
                return 0;
            return Editor_Category2FlagDefineList.Max(pair =>
            {
                return pair.Value == null ? 0 : pair.Key;
            });
        }
#endif

        #endregion
    }

    public static class FlagDefineExtension
    {
        public static FlagIDInfo GetIDInfo(this FlagDefine define)
        {
            return FlagIDInfo.GetInfo(define);
        }
        public static int GetMaximumID(this FlagDefineSet flagDefineSet)
        {
            if (flagDefineSet == null || flagDefineSet.FlagDefines == null)
                return -1;
            if (flagDefineSet.FlagDefines.Count <= 0)
                return flagDefineSet.id;
            return flagDefineSet.FlagDefines.Max(flagDefine => flagDefine.ID);
        }
    }
}