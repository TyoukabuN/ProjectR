using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public static class BuildCommand
{
    [MenuItem("PJR/打包/BuildPlayerTest")]
    public static void BuildPlayerTest()
    {
        BuildPlayer();
    }
    public static void BuildPlayer()
    {
        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, GetBuildPlayerLocation(), EditorUserBuildSettings.activeBuildTarget, BuildOptions.CleanBuildCache | BuildOptions.StrictMode);
    }

    private static string GetBuildPlayerLocation()
    {
        string buildTarget = EditorUserBuildSettings.activeBuildTarget.ToString();
        return $"{Application.dataPath}/../__builds/{buildTarget}/build_{GetDefaultBuildVersion()}/main.exe";
    }
    private static string GetDefaultBuildVersion()
    {
        int totalSecond = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
        return DateTime.Now.ToString("yyyy-MM-dd") + "-" + totalSecond;
    }
}
