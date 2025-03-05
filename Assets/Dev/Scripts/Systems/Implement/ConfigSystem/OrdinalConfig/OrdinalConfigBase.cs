using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using System.IO;
using System;
using PJR.Systems;


#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEditor;
#endif

namespace LS.Game
{
    public abstract class OrdinalConfig<TAsset, TItemAsset> : SerializedScriptableObject
        where TAsset : OrdinalConfigAsset<TItemAsset>
        where TItemAsset : OrdinalConfigItemAsset
    {
        protected bool _initialized = false;

        protected TAsset _asset;
        public List<TItemAsset> ItemAssets => _asset.itemAssets;

        protected Dictionary<int, TItemAsset> _id2ItemAsset;
        public virtual string AssetPath => ConfigUtil.GetConfigPath(RelativeFilePath);
        public virtual string ItemAssetRoot => ConfigUtil.GetConfigPath(RelativeFileRoot);
        public virtual string ConfigName => GetType().Name;
        public virtual string RelativeFilePath => $"{ConfigName}/{ConfigName}Asset.asset";
        public virtual string RelativeFileRoot => $"{ConfigName}";
        public virtual string FileName => $"{ConfigName}Asset.asset";
        public virtual string ItemFileNamePrefix => $"{ConfigName}ItemAsset";
        public virtual bool Initialized => _initialized;

        protected string _error = null;

        protected virtual TAsset LoadConfigAsset()
        {
            TAsset asset = null;
#if UNITY_EDITOR
            if (Application.isEditor)
            {
                asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(AssetPath) as TAsset;
                if (asset == null)
                {
                    asset = Editor_CreateConfigAsset();
                    Debug.LogWarning($"自动创建{typeof(TAsset).Name}配置文件, 在{AssetPath}", asset);
                }
                asset.itemAssets ??= new List<TItemAsset>();
            }
            else
#endif
            {
                //todo:ResourceSystem没有同步加载方法
                //var handle = AssetSystem.Global.LoadAssetSync(FileName);
                //if (handle == null || !handle.IsValid)
                //{
                //    Debug.LogWarning($"failure to loaded {FileName} {handle.GetAssetInfo().Address}");
                //    return null;
                //}
                //asset = handle.AssetObject as TAsset;
                //asset.itemAssets = asset.itemAssets ?? new List<TItemAsset>();
            }
            return asset;
        }

        protected void Initialize() => Initialize(LoadConfigAsset());
        protected void Initialize(TAsset asset)
        {
            _asset = asset;
            if (_asset == null)
                return;

            _id2ItemAsset ??= new Dictionary<int, TItemAsset>();
            _id2ItemAsset.Clear();

            OnPreInitialize();

            //for (int i = 0; i < _asset.itemAssets.Count; i++)
            foreach (var itemAsset in _asset.itemAssets)
            {
                if (itemAsset == null)
                {
                    //Debug.LogError($"{FileName}中itemAssets索引为：{i}的对象为空");
                    continue;
                }
                CacheItemAsset(itemAsset);
            }

            _initialized = true;
        }

        /// <summary>
        /// 在TAsset加载后初始化缓存之前调用
        /// </summary>
        protected virtual void OnPreInitialize() { }
        protected void CacheItemAsset(TItemAsset itemAsset)
        {
            if (!itemAsset.Valid)
                Debug.LogError($"存在无效配置 [Name:{itemAsset.Name}] [ID:{itemAsset.ID}]");

            if (!_id2ItemAsset.TryGetValue(itemAsset.ID, out var temp))
                _id2ItemAsset[itemAsset.ID] = itemAsset;
            else
                Debug.LogError($"[{this.GetType().Name}._id2ItemAsset] 有重复ID = {itemAsset.ID}  [{itemAsset.Name}]  [{temp.Name}]");

            OnCacheItemAsset(itemAsset);
        }

        /// <summary>
        /// 缓存每个TItemAsset时调用
        /// </summary>
        /// <param name="itemAsset"></param>
        protected virtual void OnCacheItemAsset(TItemAsset itemAsset) { }

        public virtual bool CheckValid()
        {
            if (!string.IsNullOrEmpty(_error))
                return false;
            if (_id2ItemAsset == null || !_initialized)
                Initialize();
            return _initialized;
        }

        /// <summary>
        /// 需要重置缓存的时候调用
        /// </summary>
        protected virtual void OnReset() { }

        public virtual void Reset()
        {
            _id2ItemAsset = null;
            OnReset();
        }
        public virtual TItemAsset GetConfig(int id)
        {
            if (!CheckValid())
                return null;
            return _id2ItemAsset.GetValueOrDefault(id);
        }
        public virtual bool ContainsId(int id)
        {
            if (!CheckValid())
                return false;
            return _id2ItemAsset.ContainsKey(id);
        }

        #region About Editor
#if UNITY_EDITOR
        public virtual void Editor_ShowConfigAssetCreateDialog()
        {
            if (EditorUtility.DisplayDialog("Tips", $"没有在{AssetPath} 在找到{typeof(TAsset).Name},需要新建配置文件吗？", "创建", "取消"))
            {
                Editor_CreateConfigAsset();
            }
        }
        public virtual TAsset Editor_CreateConfigAsset()
        {
            if (!AssetDatabase.IsValidFolder(ItemAssetRoot))
            {
                AssetDatabase.CreateFolder(ConfigConstant.ConfigRoot, RelativeFileRoot);
            }
            var asset = ScriptableObject.CreateInstance<TAsset>();
            AssetDatabase.CreateAsset(asset, AssetPath);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            return asset;
        }

        public virtual TAsset Editor_Asset
        {
            get
            {
                if (!CheckValid())
                    return null;
                return _asset;
            }
        }
        public virtual List<TItemAsset> Editor_ItemAssets
        {
            get
            {
                if (!CheckValid())
                    return null;
                return _asset?.itemAssets;
            }
        }

        public virtual string Editor_NewConfig(TItemAsset itemAsset) => Editor_NewConfig(ItemAssetRoot, itemAsset, false);
        public virtual string Editor_NewConfig(TItemAsset itemAsset, bool throwError) => Editor_NewConfig(ItemAssetRoot, itemAsset, throwError);
        public virtual string Editor_NewConfig(string directory, TItemAsset itemAsset)=> Editor_NewConfig(directory, itemAsset, false);
        public virtual string Editor_NewConfig(string directory, TItemAsset itemAsset, bool throwError)
        {
            if (!Editor_IsItemCreatable(itemAsset, out string errorCode))
            {
                if (throwError)
                    throw new Exception(errorCode);
                return errorCode;
            }

            string assetPath = Path.Combine(directory, $"{ItemFileNamePrefix}_{itemAsset.ID}.asset");

            AssetDatabase.CreateAsset(itemAsset, assetPath);

            Editor_AddItemAsset(itemAsset);
            Editor_RefreshConfig(false);
            return null;
        }

        /// <summary>
        /// 讲一个ItemAsset添加到配置中
        /// </summary>
        /// <param name="itemAsset"></param>
        protected virtual void Editor_AddItemAsset(TItemAsset itemAsset)
        {
            Editor_Asset.itemAssets.Add(itemAsset);
            Editor_MaskAssetDirty(true);
        }

        /// <summary>
        /// 复制一份对应id的配置
        /// </summary>
        /// <param name="id">要复制的配置id</param>
        /// <param name="copy">复制后的配置</param>
        /// <returns>返回错误信息,返回值为空的时候表示成功</returns>
        public virtual string Editor_CopyItemAsset(int id, out TItemAsset copy)
        {
            var itemAsset = GetConfig(id);
            return Editor_CopyItemAsset(itemAsset, $"{itemAsset.Editor_LabelName}(CopyFrom:[ID:{itemAsset.ID}])", out copy);
        }

        public virtual string Editor_CopyItemAsset(TItemAsset itemAsset, out TItemAsset copy)=> Editor_CopyItemAsset(itemAsset, itemAsset.name, out copy);

        /// <summary>
        /// 复制一份配置
        /// </summary>
        /// <param name="itemAsset"></param>
        /// <param name="copy">复制后的配置</param>
        /// <returns>返回错误信息,返回值为空的时候表示成功</returns>
        public virtual string Editor_CopyItemAsset(TItemAsset itemAsset, string name, out TItemAsset copy)
        {
            copy = null;
            if (itemAsset == null)
                return "目标Asset == null";
            string sourcePath = AssetDatabase.GetAssetPath(itemAsset);
            if (string.IsNullOrEmpty(sourcePath))
                return $"找不到原{typeof(TItemAsset).Name}的路径";

            int newId = Editor_GetNewID();
            string destPath = Path.Combine(ItemAssetRoot, $"{ItemFileNamePrefix}_{newId}.asset");
            if (!AssetDatabase.CopyAsset(sourcePath, destPath))
                return "通过AssetDatabase.CopyAsset复制失败";

            copy = (TItemAsset)AssetDatabase.LoadAssetAtPath(destPath,typeof(TItemAsset));
            if (copy == null)
                return "copy == null";

            copy.ID = Editor_GetNewID();
            copy.Name = name;

            Editor_AddItemAsset(copy);
            Editor_RefreshConfig(false);
            return null;
        }

        public virtual string Editor_AddExistItemAsset(TItemAsset itemAsset) => Editor_AddExistItemAsset(itemAsset, itemAsset.name);

        /// <summary>
        /// 添加一个已经存在的配置
        /// </summary>
        /// <param name="itemAsset"></param>
        /// <param name="copy">复制后的配置</param>
        /// <returns>返回错误信息,返回值为空的时候表示成功</returns>
        public virtual string Editor_AddExistItemAsset(TItemAsset itemAsset, string name)
        {
            if (Editor_ContainItemAsset(itemAsset))
                return "已存在相同配置";
            itemAsset.ID = Editor_GetNewID();
            itemAsset.Name = name;
            
            EditorUtility.SetDirty(itemAsset);
            Editor_AddItemAsset(itemAsset);
            Editor_RefreshConfig(false);
            return null;
        }

        public void Editor_RefreshConfig() => Editor_RefreshConfig(true);
        public virtual void Editor_RefreshConfig(bool reload)
        {
            Reset();
            if(reload)
                Initialize();
            else
                Initialize(Editor_Asset);
        }
        public virtual int Editor_CurrentMaxID()
        {
            if (CheckValid() && ItemAssets.Count > 0)
            {
                if (ItemAssets.Count > 0)
                    return ItemAssets.Max(itemAsset => itemAsset.ID);
                else
                    return 0;
            }
            return 0;
        }
        public virtual int Editor_GetNewID()
        {
            if (!CheckValid())
                return -1;
            var maxIndex = Editor_CurrentMaxID();
            if (maxIndex <= 0)
                return 1;
            for (int i = 1; i <= maxIndex; i++)
            {
                if (!_id2ItemAsset.ContainsKey(i))
                    return i;
            }
            return maxIndex + 1;
        }

        protected bool Editor_IsItemCreatable(IOrdinalConfigItem item, out string errorCode)
        {
            errorCode = null;
            if (!CheckValid())
                errorCode = $"{ConfigName}配置初始化失败";
            if (!item.Valid)
                errorCode = $"想要创建的 {item.GetType().Name} 无效";
            if (_id2ItemAsset.ContainsKey(item.ID))
                errorCode = $"已存在对应ID:{item.ID} 配置";
            return string.IsNullOrEmpty(errorCode);
        }

        public TItemAsset Editor_GetItemAsset(int id)
        {
            if (!CheckValid())
                return default;
            return _id2ItemAsset.GetValueOrDefault(id);
        }

        public bool Editor_ContainItemAsset(TItemAsset itemAsset)
        {
            if (!CheckValid())
                return false;
            return Editor_ItemAssets.Contains(itemAsset);
        }
        public bool Editor_ContainID(int id)
        {
            if (!CheckValid())
                return false;
            return _id2ItemAsset.ContainsKey(id);
        }

        /// <summary>
        /// 删除对应ID的配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns>返回错误信息 ErrorCode</returns>
        public virtual string Editor_RemoveItem(int id)
        {
            if (!CheckValid())
                return $"{ConfigName}配置初始化失败";
            var itemAsset = Editor_GetItemAsset(id);
            if (itemAsset == null)
                return $"没有对应ID:{id}配置";
            return Editor_RemoveItem(itemAsset);
        }
        public virtual string Editor_RemoveItem(TItemAsset itemAsset)
        {
            if (itemAsset == null)
                throw new Exception("尝试移除一个空的ItemAsset");
            if (Editor_ItemAssets.Contains(itemAsset))
                Editor_Asset.itemAssets.Remove(itemAsset);
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(itemAsset));
            Editor_MaskAssetDirty(true);
            Editor_RefreshConfig();
            return null;
        }

        public void Editor_EditItemAsset(int id)
        {
            if (!CheckValid())
                return;
            var itemAsset = Editor_GetItemAsset(id);
            if (itemAsset == null)
                return;
            OdinEditorWindow.InspectObject(itemAsset);
        }

        public void Editor_MaskAssetDirty(bool save = false)
        {
            EditorUtility.SetDirty(Editor_Asset);
            if (save)
                AssetDatabase.SaveAssets();
        }
        public void Editor_MaskItemAssetDirty(int id, bool save = false)
        {
            if (!CheckValid())
                return;
            if (_id2ItemAsset.TryGetValue(id, out var itemAsset))
            {
                EditorUtility.SetDirty(itemAsset);
                if (save)
                    AssetDatabase.SaveAssets();
            }
        }

        protected virtual string docUrl => null;
        public virtual void Editor_Menu_Doc()
        {
            if (string.IsNullOrEmpty(docUrl))
            {
                Debug.LogError($"docUrl为空,你可能需要重写下{this.GetType().Name}.{nameof(docUrl)}");
                return;
            }
            Application.OpenURL(docUrl);
        }

        protected static Dictionary<int, PropertyTree> editor_id2PropertyTree = new Dictionary<int, PropertyTree>();
        /// <summary>
        /// 根据配置获取用来在界面上画Item的PropertyTree，直接联系到ItemAsset
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PropertyTree Editor_GetPropertyTreeByID(int id)
        {
            if (!CheckValid())
                return null;
            if (!_id2ItemAsset.TryGetValue(id, out var itemAsset))
                return null;
            editor_id2PropertyTree = editor_id2PropertyTree ?? new Dictionary<int, PropertyTree>();
            if (!editor_id2PropertyTree.TryGetValue(id, out var propertyTree))
            {
                propertyTree = PropertyTree.Create(itemAsset);
                editor_id2PropertyTree[id] = propertyTree;
            }
            return propertyTree;
        }

        protected virtual void Editor_AddDefautlMenuItem(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("刷新"), false, Editor_RefreshConfig);
            menu.AddItem(new GUIContent("收集+纠错"), false, Editor_CorrectConfig);
            menu.AddItem(new GUIContent("Ping ConfigAsset"), false, () => { EditorGUIUtility.PingObject(Editor_Asset); });
            menu.AddItem(new GUIContent("文档"), false, Editor_Menu_Doc);
        }
        public virtual void Editor_ShowMoreMenu(Action<GenericMenu> onBeginAddMenuItem)
        {
            var menu = new GenericMenu();
            onBeginAddMenuItem?.Invoke(menu);
            Editor_AddDefautlMenuItem(menu);
            menu.ShowAsContext();
        }

        protected List<TItemAsset> Editor_GetAllConfigItemAsset_FromItemAssetRoot()
        {
            List<TItemAsset> _tempItemAsset = null;
            var guids = AssetDatabase.FindAssets($"t:{typeof(TItemAsset).Name}", new string[] { ItemAssetRoot });
            for (int i = 0; i < guids.Length; i++)
            {
                var guid = guids[i];
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (string.IsNullOrEmpty(assetPath))
                    continue;

                _tempItemAsset ??= new List<TItemAsset>();
                TItemAsset asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(TItemAsset)) as TItemAsset;
                _tempItemAsset.Add(asset);
            }
            return _tempItemAsset;
        }

        /// <summary>
        /// 配置纠错
        /// 1) ItemAsset有可能是在Project中创建的,这样的ItemAsset不会被收录在TAsset.itemAssets里,所以需要重新收录它们
        /// </summary>
        /// <exception cref="System.Exception"></exception>
        public virtual void Editor_CorrectConfig()
        {
            if (!CheckValid())
                return;
            var configAsset = Editor_Asset;
            if (configAsset == null)
                throw new System.Exception("Find not Config asset!");
            var itemAssets = Editor_GetAllConfigItemAsset_FromItemAssetRoot();
            if (itemAssets == null)
            {
                Debug.LogWarning($"在[ItemAssetRoot：{ItemAssetRoot}]下, 没有找到任何{typeof(TItemAsset).Name}");
                return;
            }

            for (int i = 0; i < itemAssets.Count; i++)
            {
                var itemAsset = itemAssets[i];

                //已收录ItemAsset
                if (configAsset.itemAssets.Contains(itemAsset))
                    continue;

                //多态合法性判断
                if (!Editor_IsItemAssetCorrect(itemAsset))
                {
                    Debug.Log($"{itemAsset.GetType().Name}.item 非法！", itemAsset);
                    continue;
                }

                //ID不正确,更换id
                if (Editor_ContainID(itemAsset.ID))
                {
                    itemAsset.ID = Editor_GetNewID();
                    EditorUtility.SetDirty(itemAsset);
                }

                Editor_AddItemAsset(itemAsset);
                Editor_RefreshConfig(false);
            }

            //去重
            var distinct = configAsset.itemAssets.Distinct<TItemAsset>().ToList();
            configAsset.itemAssets = distinct;

            EditorUtility.SetDirty(configAsset);
            AssetDatabase.SaveAssets();

            EditorApplication.delayCall += Editor_RefreshConfig;
        }

        /// <summary>
        /// TItemAsset是不是合法的
        /// </summary>
        /// <param name="itemAsset"></param>
        /// <returns></returns>
        public virtual bool Editor_IsItemAssetCorrect(TItemAsset itemAsset) => itemAsset != null;

        public abstract void Editor_OpenItemCreateWindow(Action<TItemAsset> onFinish, string directory = null);
#endif
        #endregion
    }
}