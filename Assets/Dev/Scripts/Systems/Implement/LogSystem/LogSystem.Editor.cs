using UnityEngine;
using System.Diagnostics;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PJR
{
    public partial class LogSystem : StaticSystem
    {
#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        static void RegisterEditorEvent()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }
        static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                Log("/////////////////////////////////////////////////////////");
                Log("////////////////////[EnteredPlayMode]////////////////////");
                Log("/////////////////////////////////////////////////////////");
            }
            else if (state == PlayModeStateChange.ExitingPlayMode)
            {
                Log("/////////////////////////////////////////////////////////");
                Log("////////////////////[ExitingPlayMode]////////////////////");
                Log("/////////////////////////////////////////////////////////");
            }
        }

        [MenuItem("Debug/打开LogSystem日志",priority = 200)]
        public static void OpenLogFile()
        {
            Process.Start(GetLogFileOutputPath());
        }
        [MenuItem("Debug/打开LogSystem日志文件夹", priority = 201)]
        public static void OpenLogFileLocation()
        {
            Process.Start(Application.persistentDataPath);
        }
#endif
    }
}
