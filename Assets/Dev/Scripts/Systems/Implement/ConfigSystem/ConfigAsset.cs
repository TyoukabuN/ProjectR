using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Text;
using System;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class ConfigAsset<IdType,ItemType> : ConfigAsset
{
    public ConfigAsset<IdType, ItemType> meta;

    public int id;

    [TableList]
    [ListDrawerSettings(CustomAddFunction = "AddElement", CustomRemoveIndexFunction = "RemoveAt")]
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
#endif 
}

public abstract class ConfigAsset : SerializedScriptableObject
{
    public virtual bool FromJson(string assetPath)
    {
        return true;
    }
}

[OdinSerializeType]
public class IdConfigPair<TKey, TValue>
{
    [OdinSerialize] private TKey key;
    [OdinSerialize] private TValue value;

    public IdConfigPair(TKey key, TValue value)
    {
        this.key = key;
        this.value = value;
    }

    public TKey Key
    {
        get => this.key;
    }

    public TValue Value
    {
        get => this.value;
    }
    
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append('[');
        if ((object) this.Key != null)
            sb.Append(this.Key.ToString());
        sb.Append(", ");
        if ((object) this.Value != null)
            sb.Append(this.Value.ToString());
        sb.Append(']');
        return sb.ToString();
    }
}


