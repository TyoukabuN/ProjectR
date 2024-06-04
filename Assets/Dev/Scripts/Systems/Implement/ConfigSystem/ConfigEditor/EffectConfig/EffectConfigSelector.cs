#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEngine;
using UnityEditor;
using LS.Game;
using System.Linq;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;

public class EffectConfigSelector : OdinMenuEditorWindow
{
    public delegate void OnConfigIdChange(EffectConfig.ConfigItem config);

    public delegate void OnConfigIdChangeForHandler(EffectConfig.ConfigItem item, bool selected);
    public static void Show(OnConfigIdChange onChanged)
    {
        var window = GetWindow<EffectConfigSelector>();
        window.ClearCallBacks();
        window.onChanged += onChanged;
    }

    private int configId = 0;

    private OnConfigIdChange onChanged;
    private OnConfigIdChangeForHandler onChangedForHandler;

    protected virtual void ClearCallBacks()
    {
        configId = 0;
        onChanged = null;
        onChangedForHandler = null;
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        ClearCallBacks();
    }

    private static string Select_Tips = "选择特效配置";
    protected override void OnGUI()
    {
        this.MenuWidth = position.width;

        //if (configId > 0)
        //    EditorGUILayout.TextField((configId).ToString());
        //else
        //    EditorGUILayout.TextField(Select_Tips);

        UpdateSelection();

        base.OnImGUI();

        //GUILayout.FlexibleSpace();
        //if (GUILayout.Button("刷新配置"))
        //{
        //    AnimationFlagConfig.OnAssetDirty();
        //    ForceMenuTreeRebuild();
        //}
    }

    private OdinMenuTree tree = null;
    protected override OdinMenuTree BuildMenuTree()
    {
        tree = new OdinMenuTree();
        //tree.Config.SelectFirstWhenBuildTree = false;
        tree.Selection.SelectionChanged += OnItemSelectionChange;
        tree.Selection.SupportsMultiSelect = true;
        tree.Config.DrawSearchToolbar = true;
        tree.Config.SearchFunction = SearchItem;

        //var selector = new GenericSelector<EffectConfig.ConfigItem>("搜索assetName", false, query.Select(x => new GenericSelectorItem<object>(x.Text, x.Value)));


        var configItems = EffectConfig.Config;
        if (configItems == null)
            return tree;
        tree.DefaultMenuStyle = OdinMenuStyle.TreeViewStyle;

        for (int i = 0; i < configItems.Count; i++)
        {
            var item = configItems.ElementAt(i).Value;
            string idStr = $"[{item.ID}]".PadRight(12);
            tree.Add($"{idStr}[{item.AssetName}]", item);
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

    //private GenericSelector<object> CreateSelector()
    //{
    //    var enableSearch = true;
    //    var selector = new GenericSelector<object>(this.Attribute.DropdownTitle, false, query.Select(x => new GenericSelectorItem<object>(x.Text, x.Value)));

    //    selector.CheckboxToggle = false;
    //    selector.EnableSingleClickToSelect();

    //    selector.SelectionTree.Config.DrawSearchToolbar = enableSearch;

    //    IEnumerable<object> selection = Enumerable.Empty<object>();

    //    if (!this.isList)
    //    {
    //        selection = this.getSelection();
    //    }

    //    selection = selection.Select(x => (x == null ? null : x.GetType()) as object);
    //    selector.SetSelection(selection);
    //    selector.SelectionTree.EnumerateTree().AddThumbnailIcons(true);

    //    return selector;
    //}

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
            EffectConfig.ConfigItem item = (EffectConfig.ConfigItem)tree.Selection.SelectedValue;
            bool selected = false;
            if (item == null)
                return;
            if (item.ID == configId)
                configId = 0;
            else
                configId = item.ID;
            selected = configId == item.ID;

            onChanged?.Invoke(item);
            onChangedForHandler?.Invoke(item, selected);
        }
        //Debug.Log(selectionChangedType.ToString());
    }

    public void UpdateSelection()
    {
        if (tree == null || tree.MenuItems == null)
            return;
        foreach (var weaponMenuItem in tree.MenuItems)
        {
            foreach (var stateMenuItem in weaponMenuItem.ChildMenuItems)
            {
                if (stateMenuItem == null || stateMenuItem.Value == null)
                    continue;
                var item = (EffectConfig.ConfigItem)stateMenuItem.Value;
                if (item == null)
                    continue;
                stateMenuItem.Icon = GetSelectedIcon(IsSelected(item.ID));
                stateMenuItem.IconSelected = GetSelectedIcon(IsSelected(item.ID));
            }
        }
    }
    private Texture GetSelectedIcon(bool selected)
    {
        return selected ? RuntionEditorUtil.LoadIcon("TestPassed") : RuntionEditorUtil.LoadIcon("TestNormal");
    }
}

#endif
