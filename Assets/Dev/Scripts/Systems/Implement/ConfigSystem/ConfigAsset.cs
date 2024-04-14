using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Text;
using System;
using UnityEngine.Serialization;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class ConfigAsset<IdType,ItemType> : ConfigAsset
{
    public ConfigAsset<IdType, ItemType> meta;

    public int id;

    [ListDrawerSettings(
        CustomAddFunction = "AddElement", 
        CustomRemoveIndexFunction = "RemoveAt",
        OnBeginListElementGUI = "OnBeginListElementGUI",
        OnEndListElementGUI = "OnEndListElementGUI"
    )]
    public List<ItemType> items;

#if UNITY_EDITOR
    public abstract void AddElement();
    public abstract void RemoveAt(int index);

    public virtual void Save()
    {
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AnimationFlagConfig.OnAssetDirty();
    }
    public virtual void OnBeginListElementGUI(int index) { }
    public virtual void OnEndListElementGUI(int index) { }
#endif 
}
public abstract class TableListConfigAsset<ItemType> : ConfigAsset
{
    [TableList]
    public List<ItemType> items;
}
public abstract class ListConfigAsset<ItemType> : ConfigAsset
{
    public List<ItemType> items;
}

public abstract class ConfigAsset : ScriptableObject
{
    public virtual bool FromJson(string assetPath)
    {
        return true;
    }
}
