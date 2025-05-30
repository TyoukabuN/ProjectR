using System;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace PJR.Timeline
{
    public static class ClipCategory
    {
        public const string Animation = "Animation";

        public static string Combine(string prefix, string itemName)
        {
            return $"{prefix}/{itemName}";
        }
    }
}
