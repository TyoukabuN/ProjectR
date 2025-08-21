using System;
using PJR.Dev.Game.DataContext;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

public class AffixTest : SerializedMonoBehaviour
{
    [FoldoutGroup("编辑器测试", expanded : true)]
    [NonSerialized,OdinSerialize]
    [HideLabel]
    public DataTypeValuePair TestTypeValuePair;
    
    [Space(20)]
    [FoldoutGroup("编辑器测试", expanded : true)]
    [NonSerialized,OdinSerialize]
    public DataValue TestValue;
    
    [Space(20)]
    [NonSerialized,OdinSerialize]
    public DataPackage Package1;
    [Space(20)]
    [NonSerialized,OdinSerialize]
    public DataPackage Package2;
    
    [Space(20)]
    [NonSerialized,OdinSerialize]
    public TempDataPackage TempPackage;

    [Button("Package合并测试")]
    public void PackageCombineTest()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = true;
            return;
        }

        using var temp = TempDataPackage.Get();
        temp.Add(Package1);
        temp.Add(Package2);
        Debug.Log(temp);
    }

    [Button("Package data添加测试")]
    public void PackageAddTest()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = true;
            return;
        }

        TempPackage ??= TempDataPackage.Get();
        TempPackage.Add(Package1);
        Debug.Log(TempPackage);
    }
}

namespace PJR.Dev.Game.DataContext
{
}
