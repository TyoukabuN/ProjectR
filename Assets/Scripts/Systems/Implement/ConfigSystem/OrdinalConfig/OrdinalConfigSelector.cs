#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using System;
using UnityEditor;

namespace PJR.Config
{
    public abstract class OrdinalConfigSelector<TConfig, TAsset, TItemAsset, TItem> : OdinMenuEditorWindow
        where TConfig       : OrdinalConfig<TAsset, TItemAsset, TItem>
        where TAsset        : OrdinalConfigAsset<TItemAsset, TItem>
        where TItemAsset    : OrdinalConfigItemAsset<TItem>
        where TItem         : OrdinalConfigItem
    {
        public delegate void OnConfigIdChange(TItem item);

        protected int configId = 0;
        protected OnConfigIdChange onChanged;
        protected bool _doClose = false;
        protected OdinMenuTree tree = null;

        protected TConfig _configInstance;
        public virtual void Init(TConfig configInstance, OnConfigIdChange onChanged)
        { 
            _configInstance = configInstance;
            this.onChanged = onChanged;
        }

        protected virtual bool Valid
        {
            get
            {
                if (_configInstance == null || onChanged == null)
                    return false;
                return true;
            }
        }
        const string default_error_msg = "非法参数";
        protected virtual void DrawInvalidMsg()
        {
            EditorGUILayout.HelpBox(default_error_msg, MessageType.Error);
        }
        protected virtual string select_Tips => "选择配置";
        protected override void OnImGUI()
        {
            if (!Valid)
            {
                DrawInvalidMsg();
                return;
            }

            this.MenuWidth = position.width;

            base.OnImGUI();

            if (_doClose)
            {
                _doClose = false;
                Close();
            }
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            if (!Valid)
                return null;

            tree = new OdinMenuTree();
            //tree.Config.SelectFirstWhenBuildTree = false;
            tree.Selection.SelectionChanged += OnItemSelectionChange;
            tree.Selection.SupportsMultiSelect = true;
            tree.Config.DrawSearchToolbar = true;
            tree.Config.SearchFunction = SearchItem;

            var configItems = _configInstance.Items;
            if (configItems == null)
                return tree;
            tree.DefaultMenuStyle = OdinMenuStyle.TreeViewStyle;

            for (int i = 0; i < configItems.Count; i++)
            {
                var item = configItems[i];
                string idStr = $"[{item.ID}]".PadRight(12);
                tree.Add($"{item.Editor_LabelName}", item);
            }

            tree.EnumerateTree().AddThumbnailIcons();

            return tree;
        }
        private bool SearchItem(OdinMenuItem odinMenuItem)
        {
            if (odinMenuItem.Value != null)
            {
                var search = MenuTree.Config.SearchTerm;
                return odinMenuItem.GetFullPath().Contains(search, StringComparison.InvariantCultureIgnoreCase);
            }

            return false;
        }
        public bool IsSelected(int configId)
        {
            return configId == this.configId;
        }
        public void OnItemSelectionChange(SelectionChangedType selectionChangedType)
        {
            if (selectionChangedType == SelectionChangedType.ItemAdded)
            {
                if (tree == null)
                    return;
                if (tree.Selection.SelectedValue == null)
                    return;
                TItem item = (TItem)tree.Selection.SelectedValue;
                bool selected = false;
                if (item == null)
                    return;
                if (item.ID == configId)
                    configId = 0;
                else
                    configId = item.ID;
                selected = configId == item.ID;

                onChanged?.Invoke(item);
                _doClose = true;
            }
        }
        protected virtual void ClearCallBacks()
        {
            configId = 0;
            onChanged = null;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            ClearCallBacks();
        }
    }

    public abstract class OrdinalConfigSelector<TConfig, TAsset, TItemAsset> : OdinMenuEditorWindow
        where TConfig : OrdinalConfig<TAsset, TItemAsset>
        where TAsset : OrdinalConfigAsset<TItemAsset>
        where TItemAsset : OrdinalConfigItemAsset
    {
        protected int configId = 0;
        protected Action<TItemAsset> onChanged;
        protected bool _doClose = false;
        protected OdinMenuTree tree = null;

        protected TConfig _configInstance;
        public virtual void Init(TConfig configInstance, Action<TItemAsset> onChanged)
        {
            _configInstance = configInstance;
            this.onChanged = onChanged;
        }

        protected virtual bool Valid
        {
            get
            {
                if (_configInstance == null || onChanged == null)
                    return false;
                return true;
            }
        }
        const string default_error_msg = "非法参数";
        protected virtual void DrawInvalidMsg()
        {
            EditorGUILayout.HelpBox(default_error_msg, MessageType.Error);
        }
        protected virtual string select_Tips => "选择配置";
        protected override void OnImGUI()
        {
            if (!Valid)
            {
                DrawInvalidMsg();
                return;
            }

            this.MenuWidth = position.width;

            base.OnImGUI();

            if (_doClose)
            {
                _doClose = false;
                Close();
            }
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            if (!Valid)
                return null;

            tree = new OdinMenuTree();
            //tree.Config.SelectFirstWhenBuildTree = false;
            tree.Selection.SelectionChanged += OnItemSelectionChange;
            tree.Selection.SupportsMultiSelect = true;
            tree.Config.DrawSearchToolbar = true;
            tree.Config.SearchFunction = SearchItem;

            var configItems = _configInstance.ItemAssets;
            if (configItems == null)
                return tree;
            tree.DefaultMenuStyle = OdinMenuStyle.TreeViewStyle;

            for (int i = 0; i < configItems.Count; i++)
            {
                var item = configItems[i];
                string idStr = $"[{item.ID}]".PadRight(12);
                tree.Add($"{item.Editor_LabelName}", item);
            }

            tree.EnumerateTree().AddThumbnailIcons();

            return tree;
        }
        private bool SearchItem(OdinMenuItem odinMenuItem)
        {
            if (odinMenuItem.Value != null)
            {
                var search = MenuTree.Config.SearchTerm;
                return odinMenuItem.GetFullPath().Contains(search, StringComparison.InvariantCultureIgnoreCase);
            }

            return false;
        }
        public bool IsSelected(int configId)
        {
            return configId == this.configId;
        }
        public void OnItemSelectionChange(SelectionChangedType selectionChangedType)
        {
            if (selectionChangedType == SelectionChangedType.ItemAdded)
            {
                if (tree == null)
                    return;
                if (tree.Selection.SelectedValue == null)
                    return;
                TItemAsset item = (TItemAsset)tree.Selection.SelectedValue;
                bool selected = false;
                if (item == null)
                    return;
                if (item.ID == configId)
                    configId = 0;
                else
                    configId = item.ID;
                selected = configId == item.ID;

                onChanged?.Invoke(item);
                _doClose = true;
            }
        }
        protected virtual void ClearCallBacks()
        {
            configId = 0;
            onChanged = null;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            ClearCallBacks();
        }
    }
}

#endif