using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJR
{
    public partial class ResourceSystem
    {
        public static int DebugLevel = 0;
        protected static class Debug
        {
            public static void Log(object message, Object context = null)
            {
                if (DebugLevel >= 1)
                    LogSystem.Log(message, context);
            }
            public static void LogError(object message, Object context = null)
            {
                if (DebugLevel >= 0)
                    LogSystem.LogError(message, context);
            }
            public static void LogWarning(object message, Object context = null)
            {
                if (DebugLevel >= 0)
                    LogSystem.LogWarning(message, context);
            }
            public static void AppendToFile(string filePath)
            {
            }
        }
    }
}