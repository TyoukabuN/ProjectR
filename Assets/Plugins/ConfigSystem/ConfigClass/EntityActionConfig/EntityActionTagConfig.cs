using LS.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EntityActionTagConfig
{
    private static bool hadInitialize = false;
    private static string configAssetName = "EntityActionTagConfigAsset";
    private static EntityActionTagConfigAsset asset = null;
    public static EntityActionTagConfigAsset ConfigAsset
    {
        get
        {
            if (!hadInitialize) OnInit();
            return asset;
        }
    }
    /// <summary>
    /// 大类之间的间隔
    /// </summary>
    public static int CategoryInterval = 1000000;
    public static int KindInterval = 1000;

    public static int GetId(int category, int kind, int index)
    {
        return CategoryInterval * category + KindInterval * kind + index;
    }

    public static void OnAssetDirty()
    {
        Enum2ConfigItem = null;
        strValue2ConfigItem = null;
        id2ConfigItem = null;
    }
    private static bool IsValid()
    {
        return asset != null;
    }

    public static void OnInit()
    {
        Debug.Log($"[Config][Init] {typeof(AnimationFlagConfig)}");
        asset = ConfigSystem.LoadConfig<EntityActionTagConfigAsset>(configAssetName);
        hadInitialize = (asset != null);
        //
        //Enum2ConfigItem = new Dictionary<Enum, EntityActionTagConfigItem>();
        //strValue2ConfigItem = new Dictionary<string, EntityActionTagConfigItem>();
        //id2ConfigItem = new Dictionary<int, EntityActionTagConfigItem>();
    }

    static EntityActionTagConfig()
    { 
        OnInit();
    }

    public static void ForeachConfigItem(EntityActionTagConfigAsset asset,Func<EntityActionTagConfigItem,bool> prediate) {
        if (asset.meta != null)
            ForeachConfigItem((EntityActionTagConfigAsset)asset.meta, prediate);

        if (asset.items == null || asset.items.Count <= 0)
            return;
        for (int i = 0; i < asset.items.Count; i++)
        {
            var item = asset.items[i];
            if (prediate(item))
                return;
        }
    }

    public static Dictionary<int, EntityActionTagConfigItem> id2ConfigItem;

    public static EntityActionTagConfigItem GetConfigById(int id)
    {
        if (asset == null)
            return null;

        if (id2ConfigItem == null)
        {
            id2ConfigItem = new Dictionary<int, EntityActionTagConfigItem>();
            //cache
            ForeachConfigItem(asset, item =>
            {
                id2ConfigItem[item.id] = item;
                return false;
            });
        }

        if (id2ConfigItem.TryGetValue(id, out var res))
            return res;
        return null;
    }
    public static bool GetConfigById(int id, out EntityActionTagConfigItem configItem)
    {
        configItem = GetConfigById(id);
        return configItem != null;
    }

    public static Dictionary<string, EntityActionTagConfigItem> strValue2ConfigItem;

    public static EntityActionTagConfigItem GetConfigByStrValue(string strValue)
    {
        if (asset == null || string.IsNullOrEmpty(strValue))
            return null;
        if (strValue2ConfigItem == null)
        { 
            strValue2ConfigItem = new Dictionary<string, EntityActionTagConfigItem>();
            //cache
            ForeachConfigItem(asset, item =>
            {
                strValue2ConfigItem[item.strValue] = item;
                return false;
            });
        }
        if (strValue2ConfigItem.TryGetValue(strValue, out var res))
            return res;
        return null;
    }
    public static bool GetConfigByStrValue(string strValue, out EntityActionTagConfigItem configItem)
    {
        configItem = GetConfigByStrValue(strValue);
        return configItem != null;
    }


    public static Dictionary<Enum, EntityActionTagConfigItem> Enum2ConfigItem;
    public static EntityActionTagConfigItem GetConfigByEnum(Enum enumValue)
    {
        Enum2ConfigItem ??= new Dictionary<Enum, EntityActionTagConfigItem> ();
        if (Enum2ConfigItem.TryGetValue(enumValue, out var res))
            return res;

        EnumInfo info = new EnumInfo(enumValue);

        if (GetConfigByStrValue(info.fullName, out res))
        {
            Enum2ConfigItem[enumValue] = res;
            return res;
        }

        return null;
    }
    public static bool GetConfigByEnum(Enum enumValue, out EntityActionTagConfigItem configItem)
    {
        configItem = GetConfigByEnum(enumValue);
        return configItem != null;
    }

    public struct EnumInfo
    {
        public string typeName;
        public string fieldName;
        public int intValue;
        /// <summary>
        /// {typeName}.{fieldName}
        /// </summary>
        public string fullName;
        public EnumInfo(Enum enumValue)
        {
            typeName = enumValue.GetType().Name;
            fieldName = enumValue.ToString();
            intValue = Convert.ToInt32(enumValue);
            fullName = $"{typeName}.{fieldName}";
        }
    }

    public static EntityActionTagConfigItem GetConfigByStringMember(Type classType, string strValue)
    {
        string fullName = $"{classType.Name}.{strValue}";
        return GetConfigByStrValue(fullName);
    }

    public static List<EntityActionTagConfigItem> GetConfigs(Func<EntityActionTagConfigItem, bool> prediate)
    {
        List<EntityActionTagConfigItem> res = new List<EntityActionTagConfigItem>();
        if(prediate == null)
            return res;
        ForeachConfigItem(asset, item =>
        {
            if(prediate(item))
                res.Add(item);
            return false;
        });
        return res;
    }
}
