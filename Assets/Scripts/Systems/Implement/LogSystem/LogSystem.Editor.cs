using System;
using System.Diagnostics;
using System.IO;
using PJR.Editor;
using UnityEngine;
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
            Application.logMessageReceived -= OnLogMessageReceived;
            Application.logMessageReceived += OnLogMessageReceived;
        }
         
        static void OnLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            //防止循环调用
            //后面还是改成标志位吧
            if (condition.IndexOf(TAG_PROJECT, StringComparison.Ordinal) >= 0)
                return;
            LogWithTag(condition, LogType2Tag(type));
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

        static void BackupLogToPrev(bool clearLog = false)
        {
            string logPath = GetLogFileOutputPath(); 
            if (!File.Exists(logPath))
                return;
            string prevLogPath = GetPreviousLogFileOutputPath();
            File.Copy(logPath, prevLogPath, true);
            
            if(clearLog)
                ClearLog();
        }

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        static void RegisterEditorEvent()
        {
            EditorEventObserver.PlayModeStateChanged -= OnPlayModeStateChanged;
            EditorEventObserver.PlayModeStateChanged += OnPlayModeStateChanged;
        }
        static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                BackupLogToPrev(true);
                
                Log(String.Empty);
                Log("////////////////////[EnterPlayMode]////////////////////");
                Log(String.Empty);
            }     
            else if (state == PlayModeStateChange.ExitingPlayMode)
            {
                Log(String.Empty);
                Log("////////////////////[ExitingPlayMode]////////////////////");
                Log(String.Empty);
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
