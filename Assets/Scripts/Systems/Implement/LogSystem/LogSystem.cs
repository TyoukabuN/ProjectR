using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace PJR.Systems
{
    public partial class LogSystem
    {
        public static string PATH_logFile = Application.persistentDataPath;

        private static string currentLogFileName;
        public static string DefaultLogFileName => currentLogFileName;

        public const string NAME_logFile = "log-editor.log";
        public const string NAME_logFile_releaase = "log-release.log";
        public const string NAME_logFilePrev = "log-editor-prev.log";
        public const string NAME_cmdLogFile = "cmd.log";

        public const string FORMAT_COMMON_LOG = "";

        public const string TAG_PROJECT = "PJR";
        public const string TAG_COMMON_LOG = "Log";
        public const string TAG_WARNING_LOG = "Warning";
        public const string TAG_ERROR_LOG = "Error";
        public const string TAG_CRASH_LOG = "Crash";
        public const string TAG_ASSERT_LOG = "Assert";
        public const string TAG_EXCEPTION_LOG = "Exception";

        public static string STR_BLANK = " ";

        static StringBuilder s_sb;
        static StringBuilder sb => s_sb ??= new StringBuilder();

        public static void Log(object content, bool invokeUnityLog = true) => Log(content.ToString(), invokeUnityLog);
        public static void LogWarning(object content, bool invokeUnityLog = true) => LogWarning(content.ToString(), invokeUnityLog);
        public static void LogError(object content, bool invokeUnityLog = true) => LogError(content.ToString(), invokeUnityLog);

        public static void Log(string content, bool invokeUnityLog = true) { var str = LogWithTag(content, TAG_PROJECT,TAG_COMMON_LOG); if (invokeUnityLog) Debug.Log(str); }
        public static void LogWarning(string content, bool invokeUnityLog = true) { var str = LogWithTag(content, TAG_PROJECT,TAG_WARNING_LOG); if (invokeUnityLog) Debug.LogWarning(str); }
        public static void LogError(string content, bool invokeUnityLog = true) { var str = LogWithTag(content, TAG_PROJECT,TAG_ERROR_LOG); if (invokeUnityLog) Debug.LogError(str); }

        public static void Log(string tag, string content, bool invokeUnityLog = true) { var str = LogWithTag(content, TAG_PROJECT,TAG_COMMON_LOG, tag); if (invokeUnityLog) Debug.Log(str); }
        public static void LogWarning(string tag, string content, bool invokeUnityLog = true) { var str = LogWithTag(content, TAG_PROJECT,TAG_WARNING_LOG, tag); if (invokeUnityLog) Debug.LogWarning(str); }
        public static void LogError(string tag, string content, bool invokeUnityLog = true) { var str = LogWithTag(content, TAG_PROJECT,TAG_ERROR_LOG, tag); if (invokeUnityLog) Debug.LogError(str); }

        public static string LogWithTag(string content, params string[] tag)
        {
            BeginEdit();
            sb.Append(GetTimeStamp());
            //sb.Append(GetFrameCount());
            if (tag != null){ 
                for (int i = 0; i < tag.Length; i++)
                {
                    //sb.Append($"[{tag[i]}]{STR_BLANK}");
                    sb.Append($"[{tag[i]}]");
                }
            }
            if (tag == null || tag.Length <= 0)
                sb.Append(STR_BLANK);
            sb.Append(content);

            return EndEdit();
        }

        static void BeginEdit()
        {
            sb.Clear();
        }
        static string EndEdit(string logFileName = null)
        {
            string str = sb.ToString();
            AppendLog(str, logFileName);
            sb.Clear();
            return str;
        }
        public static string GetTimeStamp()
        {
            return DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff]");
            //return DateTime.Now.ToString("[HH:mm:ss.fff]");
        }
        public static string GetFrameCount()
        {
            return $"[f:{Time.frameCount}]";
        }


        /// <summary>
        /// 获取log文件路径
        /// </summary>
        /// <returns></returns>
        public static string GetLogFileOutputPath() 
            => GetOutputSpaceFile(NAME_logFile);

        /// <summary>
        /// 获取log文件路径
        /// </summary>
        /// <returns></returns>
        public static string GetPreviousLogFileOutputPath() 
            => GetOutputSpaceFile(NAME_logFilePrev);

        /// <summary>
        /// 获取log输出目录下在某个文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetOutputSpaceFile(string fileName)
        {
            return Path.Combine(PATH_logFile, fileName);
        }

        /// <summary>
        /// 给对应的log加一行
        /// </summary>
        /// <param name="content"></param>
        static void AppendLog(string content,string logFileName = null)
        {
            logFileName = string.IsNullOrEmpty(logFileName) ? currentLogFileName : logFileName;

            if (!Directory.Exists(PATH_logFile))
                return;
            using (var stream = File.AppendText(GetOutputSpaceFile(logFileName)))
            {
                stream.WriteLine(content);
            }
        }


        /// <summary>
        /// 设置默认的log文件名
        /// </summary>
        /// <param name="logFileName">不填的话默认NAME_logFile</param>
        public static void SetCurrentLogFile(string logFileName = null)
        {
            if (string.IsNullOrEmpty(logFileName))
            {
#if UNITY_EDITOR
                logFileName = NAME_logFile;
#else
                logFileName = NAME_logFile_releaase;
#endif
            }
            currentLogFileName = logFileName;
        }


        public static void ClearLog(string logFileName = null)
        {
            logFileName = string.IsNullOrEmpty(logFileName) ? currentLogFileName : logFileName;
            if (!Directory.Exists(PATH_logFile))
                return;
            File.WriteAllText(GetOutputSpaceFile(logFileName), string.Empty);
        }
    }
}
