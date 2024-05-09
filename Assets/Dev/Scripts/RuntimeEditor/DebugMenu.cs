#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
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
                EditorPrefs.SetBool(PJR_DebugMenuKey_Launch, false);
                EditorPrefs.SetBool(PJR_DebugMenuKey_LaunchInAssetBundleMode, false);
            }
            else if (state == PlayModeStateChange.EnteredPlayMode )
            {
                if (EditorPrefs.GetBool(PJR_DebugMenuKey_Launch))
                { 
                    EditorPrefs.DeleteKey(PJR_DebugMenuKey_Launch);
                    CreateLanuchSceneAndHierarchy();
                }
                else if(AnyGameObjectNamed("[Debug]"))
                {
                    EditorPrefs.DeleteKey(PJR_DebugMenuKey_Launch);
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
            EditorPrefs.SetBool(PJR_DebugMenuKey_Launch, true);
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
            EditorPrefs.SetBool(PJR_DebugMenuKey_Launch, true);
            EditorPrefs.SetBool(PJR_DebugMenuKey_LaunchInAssetBundleMode, true);
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
                if (gobj.name.IndexOf("Test") == 0)
                {
                    //DontDestroyOnLoad(gobj);
                    continue;
                }
                DestroyImmediate(gobj);
            }
            //Create Entry
            //GenTips();
            //
            var gEntry = new GameObject("GameEntry");
            var entry = gEntry.AddComponent<GameEntry>();
            //TODO:Any setting for entry;
            DontDestroyOnLoad(gEntry);
            //Create ControlSystem
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
    }
}
#endif
