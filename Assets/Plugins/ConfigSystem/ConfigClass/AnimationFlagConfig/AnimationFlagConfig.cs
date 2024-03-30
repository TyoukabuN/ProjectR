using System.Collections;
using System.Collections.Generic;
using LS.Game;
using UnityEngine;
using static UnityEditor.Progress;

public static class AnimationFlagConfig
{
    private static bool hadInitialize = false;
    private static string configAssetName = "AnimationFlagConfigAsset";
    private static AnimationFlagConfigAsset asset = null;

    public static void OnAssetDirty()
    {
        allWeapons = null;
        id2strValue = null;
        allStates = null;
    }

    public static void OnInit()
    {
        Debug.Log($"[Config][Init] {typeof(AnimationFlagConfig)}");
        asset = ConfigSystem.LoadConfig<AnimationFlagConfigAsset>(configAssetName);
        hadInitialize = (asset != null);
    }

    static AnimationFlagConfig()
    {
        OnInit();
    }

    private static bool IsValid()
    {
        return asset != null;
    }

    public static Dictionary<int, AnimationFlagConfigItem> id2strValue;
    public static AnimationFlagConfigItem GetConfig(int flagId)
    {
        if (!IsValid())
            return AnimationFlagConfigItem.Empty;
        if (asset.items == null || asset.items.Count <= 0)
            return AnimationFlagConfigItem.Empty;
        if (flagId <= 0)
            return AnimationFlagConfigItem.Empty;

        id2strValue ??= new Dictionary<int, AnimationFlagConfigItem>();
        if (id2strValue.TryGetValue(flagId, out var item))
            return item;

        //TODO:[T]要优化配置的方法
        for (int i = 0; i < asset.items.Count; i++)
        {
            item = asset.items[i];
            if (item.id == flagId)
            {
                id2strValue[item.id] = item;
                return item;
            }
        }
        int index = flagId - 1;
        if (index < 0 || index >= asset.items.Count)
            return AnimationFlagConfigItem.Empty;
        return asset.items[flagId - 1];
    }

    private static List<AnimationFlagConfigItem> allWeapons = null;
    public static List<AnimationFlagConfigItem> GetAllWeaponConfig()
    {
        id2strValue ??= new Dictionary<int, AnimationFlagConfigItem>();
        if (allWeapons != null)
            return allWeapons;
        allWeapons = new List<AnimationFlagConfigItem>();
        for (int i = 0; i < asset.items.Count; i++)
        {
            var item = asset.items[i];
            if (item.id >= 10000)
            {
                id2strValue[item.id] = item;
                allWeapons.Add(item);
            }
        }
        return allWeapons;
    }


    private static List<AnimationFlagConfigItem> allStates = null;
    public static List<AnimationFlagConfigItem> GetAllStateConfig()
    {
        id2strValue ??= new Dictionary<int, AnimationFlagConfigItem>();
        if (allStates != null)
            return allStates;
        allStates = new List<AnimationFlagConfigItem>();
        for (int i = 0; i < asset.items.Count; i++)
        {
            var item = asset.items[i];
            if (item.id < 10000)
            {
                id2strValue[item.id] = item;
                allStates.Add(item);
            }
        }
        return allStates;
    }
}