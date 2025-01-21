using System;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace PJR.Timeline
{
    public interface IClip
    {
        bool Mute { get; set; }
        string ClipName { get; set; }
        double start { get; set; }
        double end { get; set; }
        double length { get; }
        int startFrame { get; }
        int endFrame { get; }
        string GetDisplayName();
    }

    [Serializable]
    public abstract class Clip : ScriptableObject, IClip
    {
        public bool Mute { get => mute; set => mute = value; }
        public string ClipName { get => clipName; set => clipName = value; }


        bool mute = false;

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

        public int startFrame => TimeUtil.ToFrames(start, Define.DefaultFrameRate);
        public int endFrame => TimeUtil.ToFrames(end, Define.DefaultFrameRate);


        public string GetDisplayName() => clipName;

        public Clip() { this.clipName = DefaultName; }
        public Clip(string clipName) { this.clipName = clipName; }

        public bool OutOfRange(double runTotalTime)
        {
            if (runTotalTime < start || runTotalTime > end)
                return true;
            return false;
        }
    }

    public class TrackCreateMenuItemAttribute : Attribute
    {
        public string menuItemName;
        public TrackCreateMenuItemAttribute(string menuItemName) 
        { 
            this.menuItemName = menuItemName;
        }
    }
}
