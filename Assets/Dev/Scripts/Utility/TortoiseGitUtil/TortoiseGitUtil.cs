#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using JetBrains.Annotations;
using PJR;

public static class TortoiseGitUtil
{
    public static void SVNLog(string path)
    {
        RunTortoiseGitProc($"/command:log /path:\"{path}\"");
    }
    public static void SVNCommit(string path)
    {
        RunTortoiseGitProc($"/command:commit /path:\"{path}\"");
    }

    public static void SVNRevert(string path)
    {
        RunTortoiseGitProc($"/command:revert /path:{path}");
    }
    public static void SVNPull(string path)
    {
        RunTortoiseGitProc($"/command:pull /path:{path}");
    }
    public static void SVNPush(string path)
    {
        RunTortoiseGitProc($"/command:push /path:{path}");
    }

    public static bool RunTortoiseGitProc(string Arguments)
    {
        if (string.IsNullOrEmpty(Arguments))
        {
            Debug.LogWarning("[TortoiseGitUtil.RunTortoiseGitProc] Arguments is null or empty!");
            return false;
        }
        return CMDUtility.RunCommonLine(Arguments, "TortoiseGitProc.exe");
    }
}

public static class TortoiseGitMenu
{
    [MenuItem("Assets/TortoiseGit/Log")]
    public static void Log()
    {
        TortoiseGitUtil.SVNLog(GetSeletedAssetPaths());
    }
    [MenuItem("Assets/TortoiseGit/Commit")]
    public static void Commit()
    {
        TortoiseGitUtil.SVNCommit(GetSeletedAssetPaths());
    }
    [MenuItem("Assets/TortoiseGit/Revert")]
    public static void Revert()
    {
        TortoiseGitUtil.SVNRevert(GetSeletedAssetPaths());
    }
    [MenuItem("Assets/TortoiseGit/Pull")]
    public static void Pull()
    {
        TortoiseGitUtil.SVNPull(GetSeletedAssetPaths());
    }
    [MenuItem("Assets/TortoiseGit/Push")]
    public static void Push()
    {
        TortoiseGitUtil.SVNPush(GetSeletedAssetPaths());
    }
    [MenuItem("Assets/TortoiseGit/需要安装TortoiseGit")]
    public static void Tips()
    {
        //Debug.Log(GetSeletedAssetPaths());
        var assetPaths = AssetSelectionUtility.GetAllPrefabsUnderSelectedFolder();
        if(assetPaths != null)
            foreach (var assetPath in assetPaths)
            {
                Debug.Log(assetPath);
            }
    }
    public static string GetSeletedAssetPaths()
    {
        List<string> paths = new List<string>();
        var objs = Selection.GetFiltered<Object>(SelectionMode.Assets);
        if (objs == null || objs.Length <= 0)
        { 
            Debug.LogWarning("[TortoiseGitMenu.GetSeletedAssetPaths] 没有选中任何Assets!");
            return string.Empty;
        }
        foreach (var obj in objs)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(path))
                continue;
            paths.Add(path);
        }
        string res = string.Empty;
        for (int i = 0; i < paths.Count; i++)
        {
            string fullPath = PathUtility.GetFullPath(paths[i]);
            if (string.IsNullOrEmpty(res))
                res = fullPath;
            else
                res = $"{res};{fullPath}";
        }
        return res;
    }
}
#endif