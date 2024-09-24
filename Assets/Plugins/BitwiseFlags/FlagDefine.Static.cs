using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public partial class FlagDefine
{
#if UNITY_EDITOR
    [MenuItem("Assets/Flag/Create/FlagDefineSet(包含Flag)")]
    public static void Editor_CreateFlagDefineSet()
    {
        string folderPath = GetSeletedFirstFolderPath();
        if (AssetDatabase.IsValidFolder(folderPath))
        {
            FlagDefineSet asset = ScriptableObject.CreateInstance<FlagDefineSet>();
            var uniqueFileName = AssetDatabase.GenerateUniqueAssetPath($"{folderPath}/{typeof(FlagDefineSet).Name}.asset");
            AssetDatabase.CreateAsset(asset, uniqueFileName);
        }
    }
    [MenuItem("Assets/Flag/Create/FlagDefineSetGroup(Set的组合)")]
    public static void Editor_CreateFlagDefineSetGroup()
    {
        string folderPath = GetSeletedFirstFolderPath();
        if (AssetDatabase.IsValidFolder(folderPath))
        {
            FlagDefineSetGroup asset = ScriptableObject.CreateInstance<FlagDefineSetGroup>();
            var uniqueFileName = AssetDatabase.GenerateUniqueAssetPath($"{folderPath}/{typeof(FlagDefineSetGroup).Name}.asset");
            AssetDatabase.CreateAsset(asset, uniqueFileName);
        }
    }
    static bool GetSeletedFirstFolder(out string assetPath, out Object asset)
    {
        assetPath = string.Empty;
        asset = null;
        var objs = Selection.GetFiltered<Object>(SelectionMode.Assets);
        if (objs == null || objs.Length <= 0)
        {
            return false;
        }
        foreach (var obj in objs)
        {
            asset = obj;
            assetPath = AssetDatabase.GetAssetPath(obj);
            if (!AssetDatabase.IsValidFolder(assetPath))
                continue;
            return true;
        }
        return false;
    }
    /// </summary>
    /// <param name="requireFullPath">需要完整路径</param>
    /// <returns>文件夹路径</returns>
    static string GetSeletedFirstFolderPath()
    {
        GetSeletedFirstFolder(out string assetPath, out Object asset);
        return assetPath;
    }
#endif
}
