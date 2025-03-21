#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LS.Game
{
    public abstract class OrdinalConfigMenuEditorWindow<TConfig, TAsset, TItemAsset, TItem> : OrdinalConfigMenuEditorWindow<TConfig, TAsset, TItemAsset>
        where TConfig : OrdinalConfig<TAsset, TItemAsset, TItem>
        where TAsset : OrdinalConfigAsset<TItemAsset, TItem>
        where TItemAsset : OrdinalConfigItemAsset<TItem>
        where TItem : OrdinalConfigItem
    {

       
    }

    public abstract class OrdinalConfigMenuEditorWindow<TConfig, TAsset, TItemAsset> : OdinMenuEditorWindow
    where TConfig : OrdinalConfig<TAsset, TItemAsset>
    where TAsset : OrdinalConfigAsset<TItemAsset>
    where TItemAsset : OrdinalConfigItemAsset
    {

        public enum HierarchyType
        {
            ID顺序,
            文件路径,
        }

        protected OdinMenuTree _tree = null;
        protected TConfig _config = null;
        protected string _error = null;
        protected HierarchyType _hierarchyType = HierarchyType.ID顺序;
        protected bool _inited = false;

        protected abstract string ItemAssetRoot { get; }
        public virtual string WindowTitle { get { return typeof(TConfig).Name; } }
        public virtual bool DrawSearchToolbar => true;

        public virtual void Init(TConfig config)
        {
            _config = config;
            _error = string.Empty;
            _inited = _config != null;
            titleContent = new GUIContent(WindowTitle);
        }

        public abstract void Init();

        protected bool _rebuildTreeNextFrame = false;
        protected override void OnBeginDrawEditors()
        {
            var selected = MenuTree.Selection.FirstOrDefault();
            var toolbarHeight = MenuTree.Config.SearchToolbarHeight;

            SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
            {
                GUILayout.Label("菜单显示方式:");
                var _hierarchyTypeTemp = (HierarchyType)EditorGUILayout.EnumPopup(_hierarchyType, GUILayout.Width(80));
                if (_hierarchyTypeTemp != _hierarchyType)
                {
                    _rebuildTreeNextFrame = true;
                    _hierarchyType = _hierarchyTypeTemp;
                }

                GUILayout.FlexibleSpace();

                Editor_OnDrawToolbarButton();

                //创建配置按钮
                if (_hierarchyType == HierarchyType.文件路径)
                {
                    if (selected != null)
                    {
                        if (selected.Value == null)
                        {
                            Editor_DrawCreateConfigBtn();
                        }
                    }
                }
                else if (_hierarchyType == HierarchyType.ID顺序)
                {
                    Editor_DrawCreateConfigBtn();
                }

                Editor_DrawCorrectConfigBtn();
                Editor_DrawRefreshConfigBtn();

                SirenixEditorGUI.EndHorizontalToolbar();
            }
        }

        protected override void OnImGUI()
        {
            base.OnImGUI();

            if (_rebuildTreeNextFrame)
            {
                _rebuildTreeNextFrame = false;
                ForceMenuTreeRebuild();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _error = null;
            _config = null;
            _inited = false;
            Debug.Log($"Destroy {this.GetType().Name}");
        }

        public virtual bool CheckValid()
        {
            _error = null;
            if (_config == null && !_inited)
                Init();
            if (_config == null)
                _error = "Config is Null!";
            return _config != null;
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            _tree = new OdinMenuTree();
            if (!CheckValid())
            {
                OnDebugError();
                return _tree;
            }

            //_tree.Config.SelectFirstWhenBuildTree = true;
            _tree.Config.DrawSearchToolbar = DrawSearchToolbar;
            _tree.Selection.SelectionChanged += OnMenuSelectionChanged;

            OnFillTree(_tree);

            _tree.EnumerateTree(OnSetupMenuItemRightClick);

            return _tree;
        }

        protected static string GetRelativePath(string assetPath)
        {
            var res = assetPath.Replace(ConfigConstant.ConfigRoot, string.Empty);
            res = res.TrimStart('/');
            return res;
        }
        protected static string ReplaceFileName(string assetPath, string newName)
        {
            var dir = Path.GetDirectoryName(assetPath);
            dir = RegularPath(dir);
            return $"{dir}/{newName}";
        }
        protected static string RegularPath(string path)
        {
            return path.Replace('\\', '/').Replace("\\", "/");
        }

        protected void OnFillTree(OdinMenuTree tree)
        {
            FillTreeDefault(tree);
        }
        protected void FillTreeDefault(OdinMenuTree tree)

        {
            if (!CheckValid())
            {
                OnDebugError();
                return;
            }

            if (_hierarchyType == HierarchyType.ID顺序)
            {
                var temp = new List<TItemAsset>(_config.Editor_Asset.itemAssets);
                temp.Sort(); 
                for (int i = 0; i < temp.Count; i++)
                {
                    var itemAsset = temp[i];
                    if (itemAsset == null)
                        continue;

                    string menuItemName = $"[{itemAsset.ID}] {itemAsset.Editor_LabelName}";
                    tree.Add(menuItemName, itemAsset);
                }
            }
            else if (_hierarchyType == HierarchyType.文件路径)
            {
                var guids = AssetDatabase.FindAssets($"t:Folder t:{typeof(TItemAsset).Name}", new string[] { _config.ItemAssetRoot });
                for (int i = 0; i < guids.Length; i++)
                {
                    var guid = guids[i];
                    var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    if (string.IsNullOrEmpty(assetPath))
                        continue;

                    if (AssetDatabase.IsValidFolder(assetPath))
                    {
                        tree.Add(GetRelativePath(assetPath), new FolderMenuItem(assetPath));
                    }
                    else
                    {
                        TItemAsset asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(TItemAsset)) as TItemAsset;
                        if (asset != null)
                        {
                            string menuItemName = $"[{asset.ID}] {asset.Editor_LabelName}";
                            if (!_config.Editor_ContainItemAsset(asset))
                                menuItemName = $"[未收录] {asset.Editor_LabelName}";
                            tree.Add(ReplaceFileName(GetRelativePath(assetPath), menuItemName), asset);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 文件夹MenuItem
        /// </summary>
        public class FolderMenuItem
        {
            protected string _assetPath;
            [ShowInInspector]
            public string AssetPath => _assetPath;

            protected Object _asset;
            public Object Asset => _asset;
            public FolderMenuItem(string assetPath)
            {
                _assetPath = assetPath;
                _asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));
            }
        }

        protected void OnSetupMenuItemRightClick(OdinMenuItem menuItem)
        {
            if (menuItem == null)
                return;
            menuItem.OnRightClick -= OnMenuItemRightClick;
            menuItem.OnRightClick += OnMenuItemRightClick;

            if (menuItem.Value is TItemAsset)
            {
                if (!_config.Editor_ContainItemAsset((TItemAsset)menuItem.Value))
                    menuItem.Icon = ConfigGUIIconRef.error;
                else
                    menuItem.Icon = null;
            }
        }

        protected void OnMenuItemRightClick(OdinMenuItem menuItem)
        {
            if (menuItem == null || menuItem.Value == null)
                return;
            var rightClickMenu = new GenericMenu();
            if (menuItem.Value is FolderMenuItem)
            {
                FolderMenuItem folderMenuItem = (FolderMenuItem)menuItem.Value;
                rightClickMenu.AddDisabledItem(new GUIContent("文件夹"));
                rightClickMenu.AddItem(new GUIContent("Ping"), false, () => { PingAsset(folderMenuItem.Asset); });
                rightClickMenu.AddItem(new GUIContent("创建配置"), false, () => { Editor_OnCreateConfig(menuItem.Value as FolderMenuItem); });
            }
            else if (menuItem.Value is TItemAsset)
            {
                var assetItem = (TItemAsset)menuItem.Value;
                rightClickMenu.AddDisabledItem(new GUIContent("ItemAsset"));
                rightClickMenu.AddItem(new GUIContent("Ping"), false, () => { PingAsset((Object)menuItem.Value); });
                rightClickMenu.AddItem(new GUIContent("复制一份"), false, () =>
                {
                    var errCode =_config.Editor_CopyItemAsset(assetItem, out var copy);
                    if(!string.IsNullOrEmpty(errCode))
                        Debug.LogError($"[复制失败] {errCode}");
                });
                rightClickMenu.AddItem(new GUIContent("删除"), false, () =>
                {
                    if (EditorUtility.DisplayDialog("提示", "您确定要删除这个ItemAsset? \n这是一个不可撤销的操作!\n你只能通过版本管理工具还原!", "确定", "取消"))
                    {
                        if (_config.Editor_ContainItemAsset(assetItem))
                            _config.Editor_RemoveItem(assetItem.ID);
                        else
                            _config.Editor_RemoveItem(assetItem);
                    }
                });
            }
            rightClickMenu.ShowAsContext();
        }

        protected void PingAsset(Object obj)
        {
            if (obj == null)
            {
                Debug.LogWarning("Attempt to Ping a null object!");
                return;
            }
            EditorApplication.delayCall += () =>
            {
                EditorGUIUtility.PingObject(obj);
            };
        }


        protected virtual void Editor_OnDrawToolbarButton()
        {
        }
        protected virtual void Editor_DrawCreateConfigBtn()
        {
            if (SirenixEditorGUI.ToolbarButton(new GUIContent("创建配置")))
            {
                Editor_OnCreateConfig();
            }
        }
        protected virtual void Editor_DrawRefreshConfigBtn()
        {
            if (SirenixEditorGUI.ToolbarButton(new GUIContent("刷新配置")))
            {
                Editor_RefreshConfig();
            }
        }
        protected virtual void Editor_DrawCorrectConfigBtn()
        {
            if (SirenixEditorGUI.ToolbarButton(new GUIContent("配置矫错")))
            {
                _config?.Editor_CorrectConfig();
            }
        }

        protected virtual void Editor_OnCreateConfig() => Editor_OnCreateConfig(null, null);
        protected virtual void Editor_OnCreateConfig(FolderMenuItem folderMenuItem)
        {
            if (folderMenuItem == null || folderMenuItem == null)
                throw new System.Exception("[FolderMenuItem is null]");
            Editor_OnCreateConfig(null, folderMenuItem.AssetPath);
        }
        protected virtual void Editor_OnCreateConfig(System.Action<TItemAsset> onFinish, string directory)
        {
            if (string.IsNullOrEmpty(directory))
            {
                _config?.Editor_OpenItemCreateWindow(onFinish, null);
                return;
            }

            if (!AssetDatabase.IsValidFolder(directory))
                throw new System.Exception($"[Invalid Folder directory]:{directory}");

            _config?.Editor_OpenItemCreateWindow(onFinish, directory);
        }

        protected void Editor_RefreshConfig()
        {
            AssertValid();
            _config?.Editor_RefreshConfig();
            RefreshWindow();
        }

        protected virtual void RefreshWindow()
        {
            _rebuildTreeNextFrame = true;
        }
        protected void OnDebugError()
        {
            if (!string.IsNullOrEmpty(_error))
                Debug.LogError(_error);
        }
        protected virtual void AssertValid()
        {
            if (!CheckValid())
                throw new System.Exception(_error);
        }
        protected virtual void OnMenuSelectionChanged(SelectionChangedType selectionChangedType)
        {

        }


    }
    static class ConfigGUIIconRef
    {
        private static Texture _warm;
        public static Texture warm => _warm ??= EditorGUIUtility.IconContent("d_console.warnicon.sml").image;

        private static Texture _error;
        public static Texture error => _error ??= EditorGUIUtility.IconContent("d_console.erroricon.sml").image;
    }
}
#endif

