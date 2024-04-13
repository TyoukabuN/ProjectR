#if UNITY_EDITOR

using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PJR
{
    public static class CSConfigHelper
    {
        public static void CreateTableListConfigAsset<ConfigAssetType, ItemType>() where ConfigAssetType : TableListConfigAsset<ItemType>
        {
            string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (AssetDatabase.IsValidFolder(assetPath))
            {
                ConfigAssetType asset = ScriptableObject.CreateInstance<ConfigAssetType>();
                asset.items = new List<ItemType>();
                var uniqueFileName = AssetDatabase.GenerateUniqueAssetPath($"{assetPath}/{typeof(ConfigAssetType).Name}.asset");
                UnityEditor.AssetDatabase.CreateAsset(asset, uniqueFileName);
            }
        }
        public static void CreateListConfigAsset<ConfigAssetType, ItemType>() where ConfigAssetType : ListConfigAsset<ItemType>
        {
            string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (AssetDatabase.IsValidFolder(assetPath))
            {
                ConfigAssetType asset = ScriptableObject.CreateInstance<ConfigAssetType>();
                asset.items = new List<ItemType>();
                var uniqueFileName = AssetDatabase.GenerateUniqueAssetPath($"{assetPath}/{typeof(ConfigAssetType).Name}.asset");
                UnityEditor.AssetDatabase.CreateAsset(asset, uniqueFileName);
            }
        }

        public static void CreateScriptableObject<ConfigAssetType>() where ConfigAssetType : SerializedScriptableObject
        {
            string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (AssetDatabase.IsValidFolder(assetPath))
            {
                ConfigAssetType asset = ScriptableObject.CreateInstance<ConfigAssetType>();
                var uniqueFileName = AssetDatabase.GenerateUniqueAssetPath($"{assetPath}/{typeof(ConfigAssetType).Name}.asset");
                UnityEditor.AssetDatabase.CreateAsset(asset, uniqueFileName);
            }
        }
    }
}
#endif
