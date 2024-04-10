
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.OdinInspector.Editor;

#if UNITY_EDITOR
using UnityEditor;
#endif

[HelpURL("https://docs.qq.com/doc/DY3NUQm5obkFodG5p")]
//[CreateAssetMenu(fileName = "AnimationFlagConfigAsset", menuName = "Configs/AnimationFlagConfigAsset", order = 8)]
public class AnimationFlagConfigAsset : ConfigAsset<int, AnimationFlagConfigItem>
{
#if UNITY_EDITOR
    [OnInspectorGUI]
    [PropertyOrder(-1)]
    public void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("不要在这个界面添加啊或者删除配置\n点击加号左边的按钮，进入List显示模式后，再进行添加和删除",MessageType.Warning);
    }
    [MenuItem("Assets/Create/ConfigsAsset/AnimationFlagConfigAsset")]
    public static void CreateAnimationFlagConfigAsset()
    {
        string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (AssetDatabase.IsValidFolder(assetPath))
        {
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