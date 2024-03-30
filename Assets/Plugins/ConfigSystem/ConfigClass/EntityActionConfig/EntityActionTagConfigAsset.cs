using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class EntityActionTagConfigAsset : ConfigAsset<int, EntityActionTagConfigItem>
{
#if UNITY_EDITOR
  

    [PropertySpace(8)]
    [OnInspectorGUI]
    [PropertyOrder(-1)]
    public void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("不要在这个界面添加啊或者删除配置\n点击加号左边的按钮，进入List显示模式后，再进行添加和删除", MessageType.Warning);
    }
    [MenuItem("Assets/Create/ConfigsAsset/EntityActionConfigAsset/创建配置文件",priority = 1)]
    public static void CreateEntityActionConfigAsset()
    {
        string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (AssetDatabase.IsValidFolder(assetPath))
        {
            EntityActionTagConfigAsset asset = ScriptableObject.CreateInstance<EntityActionTagConfigAsset>();
            asset.items = new List<EntityActionTagConfigItem>();
            var uniqueFileName = AssetDatabase.GenerateUniqueAssetPath($"{assetPath}/{nameof(EntityActionTagConfigAsset)}.asset");
            UnityEditor.AssetDatabase.CreateAsset(asset, uniqueFileName);
            return;
        }
    }

    public void Add(int id, int category, int kind, string strValue = "", string desc = "", string icon = "", string className = "", string fieldName = "")
    {
        EntityActionTagConfigItem itemObj = ScriptableObject.CreateInstance<EntityActionTagConfigItem>();
        itemObj.id = id;
        itemObj.category = category;
        itemObj.kind = kind;
        itemObj.strValue = strValue;
        itemObj.desc = desc;
        itemObj.icon = icon;
        itemObj.className = className;
        itemObj.fieldName = fieldName;

        Add(itemObj);
    }
    public void Add(EntityActionTagConfigItem itemObj)
    {
        if (itemObj == null)
            return;

        string assetPath = AssetDatabase.GetAssetPath(this);

        if (string.IsNullOrEmpty(assetPath))
            return;

        this.items.Add(itemObj);
        AssetDatabase.AddObjectToAsset(itemObj, assetPath);
        Save();
    }

    public override void AddElement()
    {
        string assetPath = AssetDatabase.GetAssetPath(this);

        if (string.IsNullOrEmpty(assetPath))
            return;

        EntityActionTagConfigItem itemObj = ScriptableObject.CreateInstance<EntityActionTagConfigItem>();
        itemObj.name = "EntityActionConfigItem_" + items.Count;

        this.items.Add(itemObj);

        AssetDatabase.AddObjectToAsset(itemObj, assetPath);

        Save();
    }

    public override void RemoveAt(int index)
    {
        if (index >= items.Count)
            return;
        var item = this.items[index];
        if (item != null)
            AssetDatabase.RemoveObjectFromAsset(item);

        items.RemoveAt(index);

        Save();
    }
#endif
}
