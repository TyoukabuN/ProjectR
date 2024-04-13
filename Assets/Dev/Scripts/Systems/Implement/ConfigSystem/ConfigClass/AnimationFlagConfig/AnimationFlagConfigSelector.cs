#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEngine;
using UnityEditor;

public class AnimationFlagConfigSelector : OdinMenuEditorWindow
{
    public delegate void OnConfigIdChange(int weaponId, int stateId);

    public delegate void OnConfigIdChangeForHandler(AnimationFlagConfigItem item,bool selected);
    public static void Show(OnConfigIdChange onChanged)
    {
        var window = GetWindow<AnimationFlagConfigSelector>();
        window.ClearCallBacks();
        window.onChanged += onChanged;
    }    
    public static void Show(int weaponId,int stateId,OnConfigIdChangeForHandler onChanged)
    {
        var window = GetWindow<AnimationFlagConfigSelector>();
        window.ClearCallBacks();
        window.onChangedForHandler += onChanged;
        window.weaponId = weaponId;
        window.stateId = stateId;
    }

    private int weaponId = 0;
    private int stateId = 0;

    private OnConfigIdChange onChanged;
    private OnConfigIdChangeForHandler onChangedForHandler;

    protected virtual void ClearCallBacks()
    {
        weaponId = 0;
        stateId = 0;
        onChanged = null;
        onChangedForHandler = null;
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();

        ClearCallBacks();
    }

    private static string Select_Tips = "至少选择一个State Id";
    protected override void OnGUI()
    {
        this.MenuWidth = position.width;

        string configId = Select_Tips;
        int id = weaponId + stateId;
        if (stateId > 0)
            EditorGUILayout.TextField((weaponId + stateId).ToString());
        else
            EditorGUILayout.TextField(configId);

        UpdateSelection();

        base.OnImGUI();
         
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("刷新配置")) {
            AnimationFlagConfig.OnAssetDirty();
            ForceMenuTreeRebuild();
        }
    }

    private OdinMenuTree tree = null;
    protected override OdinMenuTree BuildMenuTree()
    {
        tree = new OdinMenuTree();
        tree.Selection.SelectionChanged += OnItemSelectionChange;
        tree.Selection.SupportsMultiSelect = true;

        var weapons = AnimationFlagConfig.GetAllWeaponConfig();
        tree.DefaultMenuStyle = OdinMenuStyle.TreeViewStyle;

        for (int i = 0; i < weapons.Count; i++)
        {
            var item = weapons[i];
            tree.Add("Weapon Id/" + item.strValue, item, GetSelectedIcon(false));
        }

        var states = AnimationFlagConfig.GetAllStateConfig();
        for (int i = 0; i < states.Count; i++)
        {
            var item = states[i];
            tree.Add("State Id/" + item.strValue, item, GetSelectedIcon(false));
        }

        tree.EnumerateTree().AddThumbnailIcons();

        return tree;
    }

    public bool IsSelected(AnimationFlagConfigItem item)
    {
        return IsSelected(item.id);
    }
    public bool IsSelected(int configId)
    {
        if(configId == 0)
            return configId == stateId;
        return configId == weaponId || configId == stateId;
    }

    public void OnItemSelectionChange(SelectionChangedType selectionChangedType)
    {
        if (selectionChangedType == SelectionChangedType.ItemAdded)
        {
            if (tree == null)
                return;
            if (tree.Selection.SelectedValue == null)
                return;
            AnimationFlagConfigItem item = (AnimationFlagConfigItem)tree.Selection.SelectedValue;
            bool selected = false;
            if (item == null)
                return;
            if (item.id >= 10000)
            {
                if (item.id == weaponId)
                    weaponId = 0;
                else
                    weaponId = item.id;
                selected = weaponId == item.id;
            }
            else if (item.id >= 0)
            {
                if (item.id == stateId)
                    stateId = 0;
                else
                    stateId = item.id;
                selected = stateId == item.id;
            }

            onChanged?.Invoke(weaponId, stateId);
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
                var item = (AnimationFlagConfigItem)stateMenuItem.Value;
                if (item == null)
                    continue;
                stateMenuItem.Icon = GetSelectedIcon(IsSelected(item));
                stateMenuItem.IconSelected = GetSelectedIcon(IsSelected(item));
            }
        }
    }
    private Texture GetSelectedIcon(bool selected)
    {
        return selected ? RuntionEditorUtil.LoadIcon("TestPassed") : RuntionEditorUtil.LoadIcon("TestNormal");
    }
}

#endif
