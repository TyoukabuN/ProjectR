using System;
using UnityEngine;
using System.IO;
using System.Text;

namespace PJR
{
    public partial class LogSystem : StaticSystem
    {
        public static string PATH_logFile = Application.persistentDataPath;

        public static string NAME_logFile = "log.log";

        public static string FORMAT_COMMON_LOG = "";

        public static string TAG_COMMON_LOG = "Log";
        public static string TAG_WARNING_LOG = "Warning";
        public static string TAG_ERROR_LOG = "Error";
        public static string TAG_CRASH_LOG = "Crash";

        public static string STR_BLANK = " ";

        static StringBuilder s_sb;
        static StringBuilder sb => s_sb ??= new StringBuilder();

        public static void Log(object content) => Log(content.ToString());
        public static void LogWarning(object content) => LogWarning(content.ToString());
        public static void LogError(object content) => LogError(content.ToString());

        public static void Log(string tag,string content)=> Debug.Log(LogWithTag(content, TAG_COMMON_LOG, tag));
        public static void LogWarning(string tag,string content)=> Debug.LogWarning(LogWithTag(content, TAG_COMMON_LOG, tag));
        public static void LogError(string tag,string content)=> Debug.LogError(LogWithTag(content, TAG_COMMON_LOG, tag));

        public static void Log(string content) => Debug.Log(LogWithTag(content, TAG_COMMON_LOG));
        public static void LogWarning(string content) => Debug.LogWarning(LogWithTag(content, TAG_COMMON_LOG));
        public static void LogError(string content) => Debug.LogError(LogWithTag(content, TAG_COMMON_LOG));

        public static string LogWithTag(string content, params string[] tag)
        {
            BeginEdit();
            sb.Append(GetTimeStamp());
            for (int i = 0; i < tag.Length; i++)
            {
                sb.Append($"[{tag[i]}]{STR_BLANK}");
            }
            if (tag.Length <= 0)
                sb.Append(STR_BLANK);
            sb.Append(content);

            return EndEdit();
        }
        static void BeginEdit()
        {
            sb.Clear();
        }
        static string EndEdit()
        {
            string str = sb.ToString();
            AppendLog(str);
            sb.Clear();
            return str;
        }
        public static string GetTimeStamp()
        {
            return DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff]");
            //return DateTime.Now.ToString("[HH:mm:ss.fff]");
        }

        private static string s_logOutputPath = string.Empty;
        public static string GetLogFileOutputPath()
        {
            if (string.IsNullOrEmpty(s_logOutputPath))
                s_logOutputPath = Path.Combine(PATH_logFile, NAME_logFile);
            return s_logOutputPath;
        }

        static void AppendLog(string content)
        {
            if (!Directory.Exists(PATH_logFile))
                return;
            using (var stream = File.AppendText(GetLogFileOutputPath()))
            {
                stream.WriteLine(content);
            }
        }
    }
}