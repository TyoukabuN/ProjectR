#if UNITY_EDITOR

using System;
using System.IO;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
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

    public struct PlayerBuildName
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
        public PlayerBuildName(string binaryFileName, string buildName)
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
        
        public BuildPlayerOptions GetBuildPlayerOptions_Debug()
        {
            return GetBuildPlayerOptions_Debug(BuildUtil.GetCurrentBuildSettingsScenes());
        }
        public BuildPlayerOptions GetBuildPlayerOptions_Debug(string[] scenes)
        {
            return GetBuildPlayerOptions(
                scenes,
                BuildOptions.Development | BuildOptions.AllowDebugging
            );
        }
        public BuildPlayerOptions GetBuildPlayerOptions_Release()
        {
            return GetBuildPlayerOptions_Release(BuildUtil.GetCurrentBuildSettingsScenes());
        }
        public BuildPlayerOptions GetBuildPlayerOptions_Release(string[] scenes)
        {
            return GetBuildPlayerOptions(
                scenes, 
                BuildOptions.AllowDebugging
            );
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
                throw new System.ArgumentException("要构建的Player的Scenes列表为空！");
            
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
            menu.AddItem(new GUIContent("Build Debug Player(Custom Name)"), false, BuildPlayerWithName);
            menu.AddSeparator(string.Empty);
            //
            menu.AddItem(new GUIContent("UnitTest/Build"), false, BuildUnitTestPlayer);
            menu.AddItem(new GUIContent("UnitTest/Update DLL"), false, UpdateUnitTestPlayerDLL);
            menu.AddItem(new GUIContent("UnitTest/Clear And Build"), false, ClearAndBuildUnitTestPlayer);
            menu.AddItem(new GUIContent("UnitTest/Clear"), false, ClearUnitTestPlayerFolder);
            //
            menu.ShowAsContext();
        }

        
        public static string[] GetCurrentBuildSettingsScenes()
        {
            var scenelist = new List<string>(EditorBuildSettings.scenes.Length);
            for (var i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                var editorBuildSettingsScene = EditorBuildSettings.scenes[i];
                if (editorBuildSettingsScene == null)
                    continue;
                if(!editorBuildSettingsScene.enabled)
                    continue;
                if(string.IsNullOrEmpty(editorBuildSettingsScene.path))
                    continue;
                string guid = AssetDatabase.AssetPathToGUID(editorBuildSettingsScene.path);
                if (string.IsNullOrEmpty(guid))
                {
                    Debug.LogWarning($"无效场景路径：{editorBuildSettingsScene.path}");
                    continue;
                }
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
        
        public static string BuildUnitTestPlayerPath => GetFullPlayerBuildPath("UnitTestBuild");
        public static string GetFullPlayerBuildPath(string playerBuildName) => Path.Combine(PathDefine.PlayerBuildRoot, playerBuildName);
        
        public static void BuildUnitTestPlayer()
        {
            var bpInfo = new PlayerBuildName("UnitTest","UnitTestBuild");
            var bpOptions = bpInfo.GetBuildPlayerOptions_UnitTest();
            
            if(!bpOptions.BuildPlayer())
                return;
            OpenPath(bpInfo.BuildPath);
        }
        
        public static void BuildPlayerWithName()
        {
            var dialog = new BuildPlayerInfoDialog(dialog =>
            {
                var playerName = dialog.GetPlayerBuildInfo();
                if(dialog.ClearPreBuild)
                    ClearPlayerBuildFolderByName(playerName.BuildName);
                playerName.GetBuildPlayerOptions_Debug().BuildPlayer();
            });
            dialog.Show();
        }


        public abstract class MiniDialog
        {
            private OdinEditorWindow _window;     
            protected OdinEditorWindow Window => _window; 
            public void Show()
            {
                var w = OdinEditorWindow.InspectObject(this);
                _window = w;
            }
            
            [OnInspectorGUI]
            protected void InspectorGUI()
            {
                OnInspectorGUI();
                DrawButtonGUI();
            }
            protected virtual void OnInspectorGUI()
            {
            }
            protected void DrawButtonGUI()
            {
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("确定"))
                    Confirm();
                if (GUILayout.Button("取消"))
                    Cancel();
                GUILayout.EndHorizontal();
            }
            protected void Confirm()
            {
                if (!OnConfirm())
                    return;
                CloseWindow();
            }
            protected void Cancel()
            {
                OnCancel();
                CloseWindow();
            }
            protected void CloseWindow()
            {
                _window?.Close();
                _window = null;
            }
            protected virtual bool OnConfirm() => true;
            protected virtual void OnCancel()
            {
            }
        }

        public class BuildPlayerInfoDialog : MiniDialog
        {
            public enum EBuildNameFormat
            {
                [LabelText("默认")]
                Default = 0,
                [LabelText("自定义构建名名字")]
                CustomBuildName,
                [LabelText("使用时间戳作为构建名")]
                UsingTimeStampAsBuildName
            }
            
            [LabelText("执行文件名")]
            [OnValueChanged(nameof(BinaryFileNameChanged))]
            public string BinaryFileName;
            [LabelText("构建名(文件夹名)"), DisableIf("@!CustomBuildName")]
            public string BuildName;

            [LabelText("构建名格式")]
            [OnValueChanged(nameof(BinaryFileNameChanged))]
            public EBuildNameFormat BuildNameFormat = 0;

            [LabelText("自定义构建名名字")]
            public bool CustomBuildName => BuildNameFormat == EBuildNameFormat.CustomBuildName;
            [LabelText("使用时间戳作为构建吗")]
            public bool UsingTimeStampAsBuildName => BuildNameFormat == EBuildNameFormat.UsingTimeStampAsBuildName;
            [LabelText("清理之前的构建")]
            public bool ClearPreBuild = false;

            private Action<BuildPlayerInfoDialog> _onConfirm;

            public BuildPlayerInfoDialog()
            {
                BinaryFileName = Application.productName;
            }
            public BuildPlayerInfoDialog(Action<BuildPlayerInfoDialog> onConfirm):this()
            {
                _onConfirm = onConfirm;
            }
            public string GetBuildName()
            {
                string buildName = string.Empty;
                if(BuildNameFormat == EBuildNameFormat.Default)
                    buildName = $"Build_{BinaryFileName}";
                else if(BuildNameFormat == EBuildNameFormat.CustomBuildName)
                    buildName = BuildName;
                else if (BuildNameFormat == EBuildNameFormat.UsingTimeStampAsBuildName)
                    buildName = $"Build_{DateTime.Now:yyyyMMdd_HHmmss_fff}";
                
                return buildName;
            }
            public PlayerBuildName GetPlayerBuildInfo()
            {
                return new PlayerBuildName(BinaryFileName,GetBuildName());
            }

            protected override bool OnConfirm()
            {
                if (string.IsNullOrEmpty(BinaryFileName))
                {
                    Window.ShowNotification(new GUIContent("请输入执行文件名"));
                    return false;
                }
                if (string.IsNullOrEmpty(GetBuildName()))
                {
                    Window.ShowNotification(new GUIContent("构建名无效"));
                    return false;
                }
                _onConfirm?.Invoke(this);
                return true;
            }
         
            private void BinaryFileNameChanged()
            {
                if(BuildNameFormat == EBuildNameFormat.Default)
                    BuildName = $"Build_{BinaryFileName}";
                else if(BuildNameFormat == EBuildNameFormat.CustomBuildName)
                    BuildName = BuildName;
                else if (BuildNameFormat == EBuildNameFormat.UsingTimeStampAsBuildName)
                {
                }
            }
        }

        public static void UpdateUnitTestPlayerDLL()
        {
            var bpInfo = new PlayerBuildName("UnitTest","UnitTestBuild");
            bpInfo.TryUpdateDLL();
        }

        public static void ClearUnitTestPlayerFolder() => ClearPlayerBuildFolderByPath(BuildUnitTestPlayerPath);
        public static void ClearPlayerBuildFolderByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;
            string path = GetFullPlayerBuildPath(name);
            ClearPlayerBuildFolderByPath(path);
        }
        public static void ClearPlayerBuildFolderByPath(string path)
        {
            if (!Directory.Exists(path))
                return; 
            Directory.Delete(path, true);
            Debug.Log($"[ClearPlayer][Done] {path}");
        }

        public static void ClearAndBuildUnitTestPlayer()
        {
            ClearUnitTestPlayerFolder();
            BuildUnitTestPlayer();
        }

    }
}
#endif
