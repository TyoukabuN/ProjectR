#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Player;
using UnityEditor.Build.Reporting;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace PJR
{
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
            //
            menu.AddItem(new GUIContent("UnitTest/Build"), false, BuildUnitTestPlayer);
            menu.AddItem(new GUIContent("UnitTest/Update DLL"), false, UpdateUnitTestPlayerDLL);
            menu.AddItem(new GUIContent("UnitTest/Clear And Build"), false, ClearAndBuildUnitTestPlayer);
            menu.AddItem(new GUIContent("UnitTest/Clear"), false, ClearUnitTestPlayer);
            //
            menu.ShowAsContext();
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
            var binName = "UnitTest";
            var buildName = "UnitTestBuild";
            var buildPath = Path.Combine(PathDefine.PlayerBuildRoot, buildName);
            var locationPathName = Path.Combine(buildPath, $"{binName}.exe");

            if (!Directory.Exists(buildPath))
                Directory.CreateDirectory(buildPath);

            BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
            BuildPlayerOptions buildOptions = new BuildPlayerOptions();
            buildOptions.scenes = new[]
            {
                "Assets/Dev/Scenes/ReleasedUnitTestScene.unity"
            };
            buildOptions.locationPathName = locationPathName;
            buildOptions.target = buildTarget;
            buildOptions.targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
            buildOptions.options = BuildOptions.Development | BuildOptions.AllowDebugging;
    
            var report = BuildPipeline.BuildPlayer(buildOptions);
            if (report.summary.result == BuildResult.Failed)
            {
                Debug.LogError("Failure to build UnitTest player");
                return;
            }
            Debug.Log("Success to build UnitTest player");
            OpenPath(buildPath);
        }

        public static void UpdateUnitTestPlayerDLL()
        {
            var binName = "UnitTest";
            var buildName = "UnitTestBuild";
            var buildPath = Path.Combine(PathDefine.PlayerBuildRoot, buildName);
            var locationPathName = Path.Combine(buildPath, $"{binName}.exe");
            
            if (!Directory.Exists(buildPath))
                return;

            var tempCompliationPath = Path.Combine(buildPath, "tempCompile");
            var compiledDestinationDir = tempCompliationPath;
            if (!Directory.Exists(tempCompliationPath))
                Directory.CreateDirectory(tempCompliationPath);

            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            var settings = new ScriptCompilationSettings
            {
                target = buildTarget,
                group = BuildPipeline.GetBuildTargetGroup(buildTarget),
                options = ScriptCompilationOptions.DevelopmentBuild,
            };
            var results = PlayerBuildInterface.CompilePlayerScripts(settings, tempCompliationPath);

            var buildDataPath = $"{buildPath}/{binName}_Data/Managed";

            var skipped = new List<string>();
            var replaced = new List<string>();
            if (!string.IsNullOrEmpty(buildDataPath))
            {
                foreach (var filePath in System.IO.Directory.GetFiles(compiledDestinationDir, "*.dll"))
                {
                    var filename = System.IO.Path.GetFileNameWithoutExtension(filePath);
                    if (filename.StartsWith("System.") || filename.StartsWith("UnityEngine.") ||
                        filename.StartsWith("Unity."))
                        //|| (!replaceAll && !replaceDllNames.Contains(filename)))
                    {
                        skipped.Add(filename);
                        continue;
                    }

                    var fileNameExt = System.IO.Path.GetFileName(filePath);
                    var existingFilePath = System.IO.Path.Combine(buildDataPath, fileNameExt);
                    if (!File.Exists(existingFilePath))
                    {
                        skipped.Add(filename);
                        Debug.LogError(
                            $"could not update dll \"{filename}\" because it doesnt exsit in previous build \"{buildName}\"");
                        Debug.Log(existingFilePath);
                        continue;
                    }

                    var corspndPDBFilePath = Path.Combine(buildDataPath, filename + ".pdb");
                    if (File.Exists(corspndPDBFilePath))
                    {
                        var compiledCorspndPDBFilePath = Path.Combine(compiledDestinationDir, filename + ".pdb");
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

                    System.IO.File.Delete(existingFilePath);
                    System.IO.File.Move(filePath, existingFilePath);
                    replaced.Add(filename);
                }
            }

            Directory.Delete(compiledDestinationDir, true);
            Debug.Log(
                $"Path:{buildPath}.\nReplaced:\n{string.Join("\n", replaced)} \n\n Skipped:{string.Join("\n", skipped)}");
            EditorUtility.ClearProgressBar();
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
