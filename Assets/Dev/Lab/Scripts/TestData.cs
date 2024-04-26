using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TestData : ScriptableObject
{
    //[ListDrawerSettings(
    //  CustomAddFunction = "AddElement",
    //  CustomRemoveIndexFunction = "RemoveAt"
    //)]
    [ListDrawerSettings]
    public List<TestDataItem> items = new List<TestDataItem>();

    [OnInspectorGUI]
    [PropertyOrder(-1)]
    public void OnInspectorGUI()
    {
    }
    [MenuItem("Assets/Create/ConfigsAsset/TestData")]
    public static void CreateTestData()
    {
        string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (AssetDatabase.IsValidFolder(assetPath))
        {
            TestData asset = ScriptableObject.CreateInstance<TestData>();
            asset.items = new List<TestDataItem>();
            var uniqueFileName = AssetDatabase.GenerateUniqueAssetPath(assetPath + "/TestData.asset");
            UnityEditor.AssetDatabase.CreateAsset(asset, uniqueFileName);
            return;
        }
    }

    public void AddElement()
    {
        string assetPath = AssetDatabase.GetAssetPath(this);

        if (string.IsNullOrEmpty(assetPath))
            return;

        TestDataItem itemObj = ScriptableObject.CreateInstance<TestDataItem>();
        itemObj.name = "TestDataItem_" + items.Count;

        this.items.Add(itemObj);

        AssetDatabase.AddObjectToAsset(itemObj, assetPath);

        Save();
    }

    public void RemoveAt(int index)
    {
        if (index >= items.Count)
            return;
        var item = this.items[index];
        if (item != null)
            AssetDatabase.RemoveObjectFromAsset(item);

        items.RemoveAt(index);

        Save();
    }

    public virtual void Save()
    {
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AnimationFlagConfig.OnAssetDirty();
    }
}
