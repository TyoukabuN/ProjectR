using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace PJR
{
    public class DebugMenu
    {
        public const string PJR_DebugMenuKey_Launch = "PJR_DebugMenuKey_Launch";
        public const string PJR_DebugMenuKey_LaunchInAssetBundleMode = "PJR_DebugMenuKey_LaunchInAssetBundleMode";

        [InitializeOnLoadMethod]
        static void RegisterEvent()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                LogSystem.LogError("OnPlayModeStateChanged!");
                EditorPrefs.SetBool(PJR_DebugMenuKey_Launch, false);
                EditorPrefs.SetBool(PJR_DebugMenuKey_LaunchInAssetBundleMode, false);
            }
            else if (state == PlayModeStateChange.EnteredPlayMode)
            {
                CreateLanuchSceneAndHierarchy();
            }
        }

        [MenuItem("Debug/Launch  #F5", false)]
        public static void Lanuch()
        {
            if (Application.isPlaying)
            {
                EditorApplication.isPlaying = false;
                return;
            }
            LogSystem.LogError("Lanuch Excute!");
            EditorApplication.isPlaying = true;
            EditorPrefs.SetBool(PJR_DebugMenuKey_Launch, true);
            RegisterEvent();
        }

        [MenuItem("Debug/Launch包模式 #%F5", false)]
        public static void LanuchInABMode()
        {
            EditorUtility.DisplayDialog("Tips", "还没有", "ok");
        }

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
                LogSystem.Log(gobj.ToString());
                GameObject.DestroyImmediate(gobj);
            }
            //Create Entry
            var gEntry = new GameObject("GameEntry");
            GameObject.DontDestroyOnLoad(gEntry);
            var entry = gEntry.AddComponent<GameEntry>();
            //Create InputSystem
        }
    }
}
