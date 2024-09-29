#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using JetBrains.Annotations;

namespace PJR
{
    public static class TortoiseGitUtility
    {
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
            return CMDUtility.RunCommonLine(Arguments, "TortoiseGitProc.exe");
        }
    }

    public static class GitMenu
    {
        [MenuItem("Assets/TortoiseGit/Commit")]
        public static void Commit()
        {
            TortoiseGitUtility.SVNCommit(GetSeletedAssetPaths());
        }
        [MenuItem("Assets/TortoiseGit/Revert")]
        public static void Revert()
        {
            TortoiseGitUtility.SVNRevert(GetSeletedAssetPaths());
        }
        [MenuItem("Assets/TortoiseGit/Pull")]
        public static void Pull()
        {
            TortoiseGitUtility.SVNPull(GetSeletedAssetPaths());
        }
        [MenuItem("Assets/TortoiseGit/Push")]
        public static void Push()
        {
            TortoiseGitUtility.SVNPush(GetSeletedAssetPaths());
        }
        [MenuItem("Assets/TortoiseGit/需要安装TortoiseGit")]
        public static void Tips()
        {
        }
        public static string GetSeletedAssetPaths()
        {
            List<string> paths = new List<string>();
            foreach (var obj in Selection.objects)
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
}
#endif