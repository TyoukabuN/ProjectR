#if UNITY_EDITOR
using System;
using System.IO;
using PJR.Editor;
using PJR.Systems;
using UnityEditor;
using UnityEditor.Build.Player;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;

namespace PJR
{
    
    public static class DebugMenu
    {
        public static event Action OnCreateLanuchSceneAndHierarchy;
        public static bool InABMode
            => EditorPrefs.GetBool(EditorPrefKey.DebugMenu.LaunchInAssetBundleMode);

        [InitializeOnLoadMethod]
        static void RegisterEvent()
        {
            EditorEventObserver.PlayModeStateChanged -= OnPlayModeStateChanged;
            EditorEventObserver.PlayModeStateChanged += OnPlayModeStateChanged;
        }

        static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                EditorPrefs.SetBool(EditorPrefKey.DebugMenu.Launch, false);
                EditorPrefs.SetBool(EditorPrefKey.DebugMenu.LaunchInAssetBundleMode, false);
            }
            else if (state == PlayModeStateChange.EnteredPlayMode )
            {
                if (EditorPrefs.GetBool(EditorPrefKey.DebugMenu.Launch))
                { 
                    EditorPrefs.DeleteKey(EditorPrefKey.DebugMenu.Launch);
                    CreateLanuchSceneAndHierarchy();
                }
                else if(AnyGameObjectNamed("[Debug]"))
                {
                    EditorPrefs.DeleteKey(EditorPrefKey.DebugMenu.Launch);
                    CreateLanuchSceneAndHierarchy();
                }
            }
        }

        [MenuItem("Debug/Launch  #F5", false,priority = 100)]
        public static void Lanuch()
        {
            if (Application.isPlaying)
            {
                EditorApplication.isPlaying = false;
                return;
            }
            EditorApplication.isPlaying = true;
            EditorPrefs.SetBool(EditorPrefKey.DebugMenu.Launch, true);
            RegisterEvent();
        }

        [MenuItem("Debug/Launch包模式 #%F5", false, priority = 101)]
        public static void LanuchInABMode()
        {
            if (Application.isPlaying)
            {
                EditorApplication.isPlaying = false;
                return;
            }
            EditorApplication.isPlaying = true;
            EditorPrefs.SetBool(EditorPrefKey.DebugMenu.Launch, true);
            EditorPrefs.SetBool(EditorPrefKey.DebugMenu.LaunchInAssetBundleMode, true);
            RegisterEvent();
        }

        static void DontDestroyOnLoad(Object obj) => GameObject.DontDestroyOnLoad(obj);
        static void DestroyImmediate(Object obj) => GameObject.DestroyImmediate(obj);
        /// <summary>
        /// 创建游戏运行的场景和相应的Hierarchy
        /// </summary>
        static void CreateLanuchSceneAndHierarchy()
        {
            LogSystem.Log(nameof(CreateLanuchSceneAndHierarchy));
            var scene = EditorSceneManager.GetActiveScene();
            //Clear Scene
            foreach (var gobj in scene.GetRootGameObjects())
            {
                if (gobj.name.IndexOf("Test", StringComparison.Ordinal) == 0)
                    continue;
                DestroyImmediate(gobj);
            }

            OnCreateLanuchSceneAndHierarchy?.Invoke();
        }
        static bool AnyGameObjectNamed(string name)
        {
            var scene = EditorSceneManager.GetActiveScene();
            if (scene == null)
                return false;
            //Clear Scene
            foreach (var gobj in scene.GetRootGameObjects())
                if (gobj.name == name)
                    return true;
            return false;
        }
        static void GenTips()
        {
            var tips = new GameObject("[带Test开头的Gobj不会被 <Shift + F5> etc 删掉]");
            DontDestroyOnLoad(tips);
            tips.transform.SetAsFirstSibling();
        }

        private const string TempDir = "Temp/PlayerScriptCompilationTests";

        [MenuItem("Debug/当前平台代码编译测试 &F5", false, priority = 200)]
        public static void PlayerScriptCompileTest()
        {
            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            var settings = new ScriptCompilationSettings
            {
                group = BuildPipeline.GetBuildTargetGroup(buildTarget),
                target = buildTarget,
                options = ScriptCompilationOptions.None
            };

            string output = Path.Combine(TempDir, buildTarget.ToString());
            if(!Directory.Exists(output))
                Directory.CreateDirectory(output);

            EditorUtility.DisplayProgressBar("Compiling Player Scripts", "Build Target: " + settings.target + " (" + settings.group + ")", 0.1f);

            PlayerBuildInterface.CompilePlayerScripts(settings, output);

            EditorUtility.ClearProgressBar();
        }

        
        private const string TempScriptCompilationForDefaultBuild = "Temp/ScriptCompilationForDefaultBuild";

        public static void UpdateExistPlayerBuildDLL()
        {
            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            var settings = new ScriptCompilationSettings
            {
                group = BuildPipeline.GetBuildTargetGroup(buildTarget),
                target = buildTarget,
                options = ScriptCompilationOptions.None
            };
            
            string output = Path.Combine(TempScriptCompilationForDefaultBuild, buildTarget.ToString());
            if(!Directory.Exists(output))
                Directory.CreateDirectory(output);
            
            EditorUtility.DisplayProgressBar("Compiling Player Scripts", "Build Target: " + settings.target + " (" + settings.group + ")", 0.1f);

            var result = PlayerBuildInterface.CompilePlayerScripts(settings, output);

            EditorUtility.ClearProgressBar();
        }
    }
}
#endif
