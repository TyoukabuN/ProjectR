
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

//[CreateAssetMenu(fileName = "AnimationFlagConfigAsset", menuName = "Configs/AnimationFlagConfigAsset", order = 8)]
public class AnimationFlagConfigAsset : ConfigAsset<int, AnimationFlagConfigItem>
{
#if UNITY_EDITOR
    [OnInspectorGUI]
    [PropertyOrder(-1)]
    [MenuItem("Assets/Create/ConfigsAsset/AnimationFlagConfigAsset")]
    public static void CreateAnimationFlagConfigAsset()
    {
        string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (AssetDatabase.IsValidFolder(assetPath))
        {
            GameObject gameObject = new GameObject(assetPath);
            AnimationFlagConfigAsset asset = ScriptableObject.CreateInstance<AnimationFlagConfigAsset>();
            asset.items = new List<AnimationFlagConfigItem>();
            var uniqueFileName = AssetDatabase.GenerateUniqueAssetPath(assetPath + "/AnimationFlagConfigAsset.asset");
            UnityEditor.AssetDatabase.CreateAsset(asset, uniqueFileName);
            return;
        }
    }

    public override void AddElement()
    {
        string assetPath = AssetDatabase.GetAssetPath(this);

        if (string.IsNullOrEmpty(assetPath))
            return;

        AnimationFlagConfigItem itemObj = ScriptableObject.CreateInstance<AnimationFlagConfigItem>();
        itemObj.name = "AnimationFlagConfigItem_" + items.Count;

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