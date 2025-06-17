using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJR.Timeline
{
    public static class Debug
    {
        public static bool enable = true;
        public static void Log(object message, Object context = null)
        {
            if(!enable)
                return;
            UnityEngine.Debug.Log(message,context);
        }
        public static void LogError(object message, Object context = null)
        {
            if(!enable)
                return;
            UnityEngine.Debug.LogError(message,context);
        }
        public static void LogWarning(object message, Object context = null)
        {
            if(!enable)
                return;
            UnityEngine.Debug.LogWarning(message,context);
        }
    }
}
