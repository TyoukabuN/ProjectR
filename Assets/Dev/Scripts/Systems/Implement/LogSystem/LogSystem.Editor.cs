using UnityEngine;
using System.Diagnostics;
using System;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PJR
{
    public partial class LogSystem : StaticSystem
    {
#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#endif
        static void RegisterEvent()
        {
            SetDefaultLogFile();
            if (CMDUtility.IsBatchMode)
            {
                SetDefaultLogFile(NAME_cmdLogFile);
                Log("///////////////////////////////////////////////////");
                Log("////////////////////[BatchMode]////////////////////");
                Log("///////////////////////////////////////////////////");
                Application.logMessageReceived -= OnLogMessageReceived;
                Application.logMessageReceived += OnLogMessageReceived;
            }
        }

        /// <summary>
        /// Unity的Debug回调
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="stackTrace"></param>
        /// <param name="type"></param>
        static void OnLogMessageReceived(string condition, string stackTrace, LogType type)
        { 
            if(type == LogType.Log)
                Log($"{condition}\n{stackTrace}", false);
            else if(type == LogType.Warning)
                LogWarning($"{condition}\n{stackTrace}", false);
            else
                LogError($"{condition}\n{stackTrace}", false);
        }

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
