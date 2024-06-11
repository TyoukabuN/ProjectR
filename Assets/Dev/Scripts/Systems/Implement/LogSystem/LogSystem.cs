using System;
using UnityEngine;
using System.IO;
using System.Text;

namespace PJR
{
    public partial class LogSystem : StaticSystem
    {
        public static string PATH_logFile = Application.persistentDataPath;

        private static string defaultLogFileName = NAME_logFile;
        public static string DefaultLogFileName => defaultLogFileName;

        public const string NAME_logFile = "log.log";
        public const string NAME_cmdLogFile = "cmd.log";

        public const string FORMAT_COMMON_LOG = "";

        public const string TAG_COMMON_LOG = "Log";
        public const string TAG_WARNING_LOG = "Warning";
        public const string TAG_ERROR_LOG = "Error";
        public const string TAG_CRASH_LOG = "Crash";

        public static string STR_BLANK = " ";

        static StringBuilder s_sb;
        static StringBuilder sb => s_sb ??= new StringBuilder();

        public static void Log(object content, bool orginalLog = true) => Log(content.ToString(), orginalLog);
        public static void LogWarning(object content,bool orginalLog = true) => LogWarning(content.ToString(), orginalLog);
        public static void LogError(object content, bool orginalLog = true) => LogError(content.ToString(), orginalLog);

        public static void Log(string content, bool orginalLog = true) => Debug.Log(LogWithTag(content, TAG_COMMON_LOG, null));
        public static void LogWarning(string content, bool orginalLog = true) => Debug.LogWarning(LogWithTag(content, TAG_WARNING_LOG, null));
        public static void LogError(string content, bool orginalLog = true) => Debug.LogError(LogWithTag(content, TAG_ERROR_LOG, null));

        public static void Log(string tag, string content, bool orginalLog = true) { if (orginalLog) Debug.Log(LogWithTag(content, TAG_COMMON_LOG, tag)); else LogWithTag(content, TAG_COMMON_LOG, tag); }
        public static void LogWarning(string tag, string content, bool orginalLog = true) { if (orginalLog) Debug.LogWarning(LogWithTag(content, TAG_WARNING_LOG, tag)); else LogWithTag(content, TAG_WARNING_LOG, tag); }
        public static void LogError(string tag, string content, bool orginalLog = true) { if (orginalLog) Debug.LogError(LogWithTag(content, TAG_ERROR_LOG, tag)); else LogWithTag(content, TAG_ERROR_LOG, tag); }

        public static string LogWithTag(string content, params string[] tag)
        {
            BeginEdit();
            sb.Append(GetTimeStamp());
            if (tag != null){ 
                for (int i = 0; i < tag.Length; i++)
                {
                    sb.Append($"[{tag[i]}]{STR_BLANK}");
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

        /// <summary>
        /// 获取log文件路径
        /// </summary>
        /// <returns></returns>
        public static string GetLogFileOutputPath()
        {
            return GetOutputSpaceFile(NAME_logFile);
        }

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
            logFileName = string.IsNullOrEmpty(logFileName) ? defaultLogFileName : logFileName;

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
        public static void SetDefaultLogFile(string logFileName = null)
        {
            logFileName = string.IsNullOrEmpty(logFileName) ? NAME_logFile : logFileName;
            if (string.IsNullOrEmpty(logFileName))
                return;
            defaultLogFileName = logFileName;
        }


        public static void ClearLog(string logFileName = null)
        {
            logFileName = string.IsNullOrEmpty(logFileName) ? defaultLogFileName : logFileName;
            if (!Directory.Exists(PATH_logFile))
                return;
            using (var stream = File.AppendText(GetOutputSpaceFile(logFileName)))
            {
                stream.Write(string.Empty);
            }
        }
    }
}
