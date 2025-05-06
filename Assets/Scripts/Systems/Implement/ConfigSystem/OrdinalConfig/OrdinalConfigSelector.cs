#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using System;
using UnityEditor;
using UnityEngine;

namespace PJR.Config
{
    public abstract class OrdinalConfigSelector<TConfig, TAsset, TItemAsset, TItem> : OdinMenuEditorWindow
        where TConfig       : OrdinalConfig<TAsset, TItemAsset, TItem>
        where TAsset        : OrdinalConfigAsset<TItemAsset, TItem>
        where TItemAsset    : OrdinalConfigItemAsset<TItem>
        where TItem         : OrdinalConfigItem
    {
        public delegate void OnConfigIdChange(TItem item);

        protected int _configId = 0;
        protected OnConfigIdChange _onChanged;
        protected bool _doClose = false;
        protected OdinMenuTree _menuTree = null;

        protected TConfig _configInstance;
        public virtual void Init(TConfig configInstance, OnConfigIdChange onChanged)
        { 
            _configInstance = configInstance;
            this._onChanged = onChanged;
        }

        protected virtual bool Valid
        {
            get
            {
                if (_configInstance == null || _onChanged == null)
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

            _menuTree = new OdinMenuTree();
            //tree.Config.SelectFirstWhenBuildTree = false;
            _menuTree.Selection.SelectionChanged += OnItemSelectionChange;
            _menuTree.Selection.SupportsMultiSelect = false;
            _menuTree.Config.DrawSearchToolbar = true;
            _menuTree.Config.SearchFunction = SearchItem;

            var configItems = _configInstance.Items;
            if (configItems == null)
                return _menuTree;
            _menuTree.DefaultMenuStyle = OdinMenuStyle.TreeViewStyle;

            for (int i = 0; i < configItems.Count; i++)
            {
                var item = configItems[i];
                if (item == null)
                {
                    Debug.LogError($"[{_configInstance.ConfigName}]配置文件的[index:{i}]为空!");
                    continue;
                }
                string idStr = $"[{item.ID}]".PadRight(12);
                _menuTree.Add($"{item.Editor_LabelName}", item);
            }

            _menuTree.EnumerateTree().AddThumbnailIcons();

            return _menuTree;
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
            return configId == this._configId;
        }
        public void OnSelectionConfirmed(OdinMenuTreeSelection odinMenuTreeSelection)
        {
            Debug.Log("OnSelectionConfirmed");
            TItem item = (TItem)odinMenuTreeSelection.SelectedValue;
            if (item == null)
                return;
            _configId = item.ID;
            _onChanged?.Invoke(item);
            _doClose = true;
        }

        public void OnItemSelectionChange(SelectionChangedType selectionChangedType)
        {
            if (selectionChangedType == SelectionChangedType.ItemAdded)
            {
                if (_doClose)
                    return;
                if (_menuTree == null)
                    return;
                if (_menuTree.Selection.SelectedValue == null)
                    return;
                TItem item = (TItem)_menuTree.Selection.SelectedValue;
                if (item == null)
                    return;
                if (_configId == item.ID)
                {
                    OnSelectionConfirmed(_menuTree.Selection);
                    return;
                }

                _configId = item.ID;
            }
            Debug.Log(selectionChangedType);
        }
        
        protected virtual void ClearCallBacks()
        {
            _configId = 0;
            _onChanged = null;
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
        protected int? _configId = 0;
        protected Action<TItemAsset> _onChanged;
        protected bool _doClose = false;
        protected OdinMenuTree _menuTree = null;

        protected TConfig _configInstance;
        public virtual void Init(TConfig configInstance, Action<TItemAsset> onChanged)
        {
            _configInstance = configInstance;
            _onChanged = onChanged;
        }

        protected virtual bool Valid
        {
            get
            {
                if (_configInstance == null || _onChanged == null)
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

            _menuTree = new OdinMenuTree();
            _menuTree.Config.DrawSearchToolbar = true;
            _menuTree.Config.ConfirmSelectionOnDoubleClick = true;
            _menuTree.Config.SearchFunction += SearchItem;
            
            _menuTree.Selection.SupportsMultiSelect = false;
            _menuTree.Selection.SelectionChanged += OnItemSelectionChange;
            _menuTree.Selection.SelectionConfirmed += OnSelectionConfirmed;

            var configItems = _configInstance.ItemAssets;
            if (configItems == null)
                return _menuTree;
            _menuTree.DefaultMenuStyle = OdinMenuStyle.TreeViewStyle; 

            for (int i = 0; i < configItems.Count; i++)
            {
                var item = configItems[i];
                if (item == null)
                {
                    Debug.LogError($"[{_configInstance.ConfigName}]配置文件的[index:{i}]为空!");
                    continue;
                }
                string idStr = $"[{item.ID}]".PadRight(12);
                _menuTree.Add($"{item.Editor_LabelName}", item);
            }

            _menuTree.EnumerateTree().AddThumbnailIcons();

            return _menuTree;
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
            return configId == _configId;
        }

        public void OnSelectionConfirmed(OdinMenuTreeSelection odinMenuTreeSelection)
        {
            Debug.Log("OnSelectionConfirmed");
            TItemAsset item = (TItemAsset)odinMenuTreeSelection.SelectedValue;
            if (item == null)
                return;
            _configId = item.ID;
            _onChanged?.Invoke(item);
            _doClose = true;
        }

        public void OnItemSelectionChange(SelectionChangedType selectionChangedType)
        {
            if (selectionChangedType == SelectionChangedType.ItemAdded)
            {
                if (_doClose)
                    return;
                if (_menuTree == null)
                    return;
                if (_menuTree.Selection.SelectedValue == null)
                    return;
                TItemAsset item = (TItemAsset)_menuTree.Selection.SelectedValue;
                if (item == null)
                    return;
                if (_configId == item.ID)
                {
                    OnSelectionConfirmed(_menuTree.Selection);
                    return;
                }

                _configId = item.ID;
            }
            Debug.Log(selectionChangedType);
        }
        protected virtual void ClearCallBacks()
        {
            _onChanged = null;
            _configId = null;
            _doClose = false;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            ClearCallBacks();
        }
    }
}

#endif