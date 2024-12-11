using UnityEngine;
using System.Diagnostics;
using System;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PJR.Systems
{
    public partial class LogSystem : StaticSystem
    {
        static LogSystem()
        {
            RegisterEvent();
        }
        static void RegisterEvent()
        {
            SetDefaultLogFile();
            if (CMDUtility.IsBatchMode)
            {
                SetDefaultLogFile(NAME_cmdLogFile);
                Log("///////////////////////////////////////////////////");
                Log("////////////////////[BatchMode]////////////////////");
                Log("///////////////////////////////////////////////////");
            }
            Application.logMessageReceived -= OnLogMessageReceived2;
            Application.logMessageReceived += OnLogMessageReceived2;
        }
         
        static void OnLogMessageReceived2(string condition, string stackTrace, LogType type)
        {
            LogWithTag(stackTrace, LogType2Tag(type));
        }
        static string LogType2Tag(LogType logType)
        {
            switch (logType)
            {
                case LogType.Error:
                    return TAG_ERROR_LOG;
                case LogType.Assert:
                    return TAG_ASSERT_LOG;
                case LogType.Warning:
                    return TAG_WARNING_LOG;
                case LogType.Log:
                    return TAG_COMMON_LOG;
                case LogType.Exception:
                    return TAG_EXCEPTION_LOG;
                default:
                    return TAG_COMMON_LOG;
            }
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

        [MenuItem("Debug/打开LogSystem日志",priority = 300)]
        public static void OpenLogFile()
        {
            Process.Start(GetLogFileOutputPath());
        }
        [MenuItem("Debug/打开LogSystem日志文件夹", priority = 301)]
        public static void OpenLogFileLocation()
        {
            Process.Start(Application.persistentDataPath);
        }
#endif
    }
}
