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

        protected double m_Start;
        protected double m_End;
        /// <summary>
        /// clip start seconds
        /// </summary>
        public double start 
        {
            get => m_Start;
            set => m_Start = value;
        }
        /// <summary>
        /// clip end seconds
        /// </summary>
        public double end
        {
            get => m_End;
            set => m_End = value;
        }

        public double length => end - start;

        public int startFrame => Utility.Time.ToFrames(start, Define.DefaultFrameRate);
        public int endFrame => Utility.Time.ToFrames(end, Define.DefaultFrameRate);

        public int[] dependencyIDs;

        public virtual string GetDisplayName() => clipName;

        public Clip() { this.clipName = DefaultName; }
        public Clip(string clipName) { this.clipName = clipName; }
    }
}
