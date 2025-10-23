#if UNITY_EDITOR

using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Player;
using UnityEditor.Build.Reporting;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace PJR.Build
{
    public static class BuildPlayerOptionsExtension
    {
        public static bool BuildPlayer(this BuildPlayerOptions buildPlayerOptions)
        {
            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            if (report.summary.result == BuildResult.Failed)
            {
                Debug.LogError($"Failure to build {Path.GetFileNameWithoutExtension(buildPlayerOptions.locationPathName)}");
                return false;
            }
            Debug.Log($"Success to build {Path.GetFileNameWithoutExtension(buildPlayerOptions.locationPathName)} player");
            return true;
        }
    }

    public struct PlayerBuildInfo
    {
        public const string BinaryFileExtension = ".exe";
        /// <summary>
        /// player的exe文件的名字，不包括后缀的.exe
        /// </summary>
        public string BinaryFileName;
        /// <summary>
        /// 这次build的名字，也就是build所在文件夹的位置
        /// </summary>
        public string BuildName;
        /// <summary>
        /// build所在文件夹在系统中的完整地址
        /// </summary>
        public string BuildPath;
        /// <summary>
        /// 给BuildPlayerOptions用的，player的执行文件在系统中的完整地址
        /// </summary>
        public string LocationPathName;

        /// <param name="binaryFileName">player的exe文件的名字，不包括后缀的.exe</param>
        /// <param name="buildName">这次build的名字，也就是build所在文件夹的位置</param>
        public PlayerBuildInfo(string binaryFileName, string buildName)
        {
            BinaryFileName = binaryFileName;
            BuildName = buildName;
            BuildPath = Path.Combine(PathDefine.PlayerBuildRoot, buildName);
            LocationPathName = Path.Combine(BuildPath, $"{BinaryFileName}{BinaryFileExtension}");
        }

        /// <summary>
        /// 使用平台: EditorUserBuildSettings.activeBuildTarget <br/>
        /// 使用场景: EditorBuildSettings.scenes <br/>
        /// options = BuildOptions.Development | BuildOptions.AllowDebugging
        /// </summary>
        public BuildPlayerOptions GetBuildPlayerOptions()
        {
            return GetBuildPlayerOptions(
                BuildUtil.GetCurrentBuildSettingsScenes(),
                BuildOptions.Development | BuildOptions.AllowDebugging
            );
        }
        
        public BuildPlayerOptions GetBuildPlayerOptions_Debug(string[] scenes)
        {
            return GetBuildPlayerOptions(
                scenes,
                BuildOptions.Development | BuildOptions.AllowDebugging
            );
        }
        
        public BuildPlayerOptions GetBuildPlayerOptions_Release(string[] scenes)
        {
            return GetBuildPlayerOptions(scenes, BuildOptions.AllowDebugging);
        }
        
        /// <summary>
        /// 使用平台: EditorUserBuildSettings.activeBuildTarget <br/>
        /// 使用场景: Assets/Dev/Scenes/ReleasedUnitTestScene.unity <br/>
        /// options = BuildOptions.Development | BuildOptions.AllowDebugging
        /// </summary>
        public BuildPlayerOptions GetBuildPlayerOptions_UnitTest()
        {
            return GetBuildPlayerOptions(
                new[] {
                    "Assets/Dev/Scenes/ReleasedUnitTestScene.unity"
                },
                BuildOptions.Development | BuildOptions.AllowDebugging
            );
        }
        
        public BuildPlayerOptions GetBuildPlayerOptions(string[] scenes, BuildOptions options)
        {
            if(scenes is not {Length: > 0})
                throw new System.ArgumentException("scenes is empty");
            
            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            var buildOptions = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = LocationPathName,
                target = buildTarget,
                targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget),
                options = options
            };
            return buildOptions;
        }

        public bool IsBuildPathExist()
        {
            if (string.IsNullOrEmpty(BuildPath))
                return false;
            return PathUtil.CreateDirectoryIfNoExist(BuildPath);
        }

        public bool TryUpdateDLL()
        {
            if (!IsBuildPathExist())
                return false;

            var tempCompliationPath = Path.Combine(BuildPath, "tempCompile");
            if (!PathUtil.CreateDirectoryIfNoExist(tempCompliationPath))
                return false;

            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            var settings = new ScriptCompilationSettings
            {
                target = buildTarget,
                group = BuildPipeline.GetBuildTargetGroup(buildTarget),
                options = ScriptCompilationOptions.DevelopmentBuild,
            };
            var results = PlayerBuildInterface.CompilePlayerScripts(settings, tempCompliationPath);

            var buildDataPath = $"{BuildPath}/{BinaryFileName}_Data/Managed";
            if (!Directory.Exists(buildDataPath))
            {
                Debug.Log($"没有找到Managed目录: {buildDataPath}");
                return false;
            }

            var skipped = new List<string>();
            var replaced = new List<string>();
            if (!string.IsNullOrEmpty(buildDataPath))
            {
                foreach (var filePath in Directory.GetFiles(tempCompliationPath, "*.dll"))
                {
                    var filename = Path.GetFileNameWithoutExtension(filePath);
                    if (filename.StartsWith("System.") || filename.StartsWith("UnityEngine.") ||
                        filename.StartsWith("Unity."))
                    {
                        skipped.Add(filename);
                        continue;
                    }

                    var fileNameExt = Path.GetFileName(filePath);
                    var existingFilePath = Path.Combine(buildDataPath, fileNameExt);
                    if (!File.Exists(existingFilePath))
                    {
                        skipped.Add(filename);
                        Debug.LogError(
                            $"could not update dll \"{filename}\" because it doesnt exsit in previous build \"{BuildName}\"");
                        Debug.Log(existingFilePath);
                        continue;
                    }

                    var corspndPDBFilePath = Path.Combine(buildDataPath, filename + ".pdb");
                    if (File.Exists(corspndPDBFilePath))
                    {
                        var compiledCorspndPDBFilePath = Path.Combine(tempCompliationPath, filename + ".pdb");
                        if (!File.Exists(compiledCorspndPDBFilePath))
                        {
                            Debug.LogError($"could not update replace pdb \"{filename}\"");
                        }
                        else
                        {
                            File.Delete(corspndPDBFilePath);
                            File.Move(compiledCorspndPDBFilePath, corspndPDBFilePath);
                        }
                    }

                    File.Delete(existingFilePath);
                    File.Move(filePath, existingFilePath);
                    replaced.Add(filename);
                }
            }

            Directory.Delete(tempCompliationPath, true);
            Debug.Log(
                $"Path:{BuildPath}.\nReplaced:\n{string.Join("\n", replaced)} \n\n Skipped:{string.Join("\n", skipped)}");

            return true;
        }
    }

    public static class BuildUtil
    {
        public const string ShortcutId = "BuildUtil/ShowContextMenu";
        [Shortcut(ShortcutId, KeyCode.F1, ShortcutModifiers.Alt)]
        public static void ShowContextMenu_ShortCut()
        {
            ShowContextMenu();
        }
        
        public static void ShowContextMenu()
        {
            GenericMenu menu = new GenericMenu();
            //
            menu.AddItem(new GUIContent("Open .playerBuilds"), false, OpenPlayerBuildsPath);
            menu.AddSeparator(string.Empty);
            //
            menu.AddItem(new GUIContent("UnitTest/Build"), false, BuildUnitTestPlayer);
            menu.AddItem(new GUIContent("UnitTest/Update DLL"), false, UpdateUnitTestPlayerDLL);
            menu.AddItem(new GUIContent("UnitTest/Clear And Build"), false, ClearAndBuildUnitTestPlayer);
            menu.AddItem(new GUIContent("UnitTest/Clear"), false, ClearUnitTestPlayer);
            //
            menu.ShowAsContext();
        }

        
        public static string[] GetCurrentBuildSettingsScenes()
        {
            var scenelist = new List<string>(EditorBuildSettings.scenes.Length);
            for (var i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                var editorBuildSettingsScene = EditorBuildSettings.scenes[i];
                if(string.IsNullOrEmpty(editorBuildSettingsScene.path))
                    continue;
                scenelist.Add(editorBuildSettingsScene.path);
            }
            return scenelist.ToArray();
        }

        public static void OpenPlayerBuildsPath()
        {
            OpenPath(PathDefine.PlayerBuildRoot);
        }
        public static void OpenPath(string path)
        {
            path = PathUtil.ReplaceToDirectorySeparatorChar(path);
            if (Directory.Exists(path))
                System.Diagnostics.Process.Start("explorer.exe", path);
            else
                Debug.LogWarning($"路径不存在: {path}");
        }
        
        public static string BuildUnitTestPlayerPath => Path.Combine(PathDefine.PlayerBuildRoot, "UnitTestBuild");
        public static void BuildUnitTestPlayer()
        {
            var bpInfo = new PlayerBuildInfo("UnitTest","UnitTestBuild");
            var bpOptions = bpInfo.GetBuildPlayerOptions_UnitTest();
            
            if(!bpOptions.BuildPlayer())
                return;
            OpenPath(bpInfo.BuildPath);
        }

        public static void UpdateUnitTestPlayerDLL()
        {
            var bpInfo = new PlayerBuildInfo("UnitTest","UnitTestBuild");
            bpInfo.TryUpdateDLL();
        }

        public static void ClearUnitTestPlayer()
        {
            if (!Directory.Exists(BuildUnitTestPlayerPath))
                return; 
            Directory.Delete(BuildUnitTestPlayerPath, true);
            Debug.Log("[ClearUnitTestPlayer] [Done]");
        }

        public static void ClearAndBuildUnitTestPlayer()
        {
            ClearUnitTestPlayer();
            BuildUnitTestPlayer();
        }

    }
}
#endif
