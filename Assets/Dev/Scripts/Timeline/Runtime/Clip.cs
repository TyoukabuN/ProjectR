using System;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace PJR.Timeline
{
    [Serializable]
    public abstract class Clip
    {
    //#if UNITY_EDITOR
    //        public abstract ClipGUI 
    //#endif
        public const string DefaultName = "EmptyName";

        public string clipName;
        public int ClipType;

        /// <summary>
        /// clip start seconds
        /// </summary>
        public float start;
        /// <summary>
        /// clip end seconds
        /// </summary>
        public float end;

        public int startFrame => Mathf.RoundToInt(start / Define.SPF_Gane);
        public int endFrame => Mathf.RoundToInt(end / Define.SPF_Gane);

        public int[] dependencyIDs;

        public virtual string GetDisplayName() => clipName;

        public Clip() { this.clipName = DefaultName; }
        public Clip(string clipName) { this.clipName = clipName; }
    }
}
