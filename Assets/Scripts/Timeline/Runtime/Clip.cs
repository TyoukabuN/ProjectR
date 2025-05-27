using System;
using Sirenix.OdinInspector;
using UnityEngine;
using static PJR.Timeline.Define;
#if UNITY_EDITOR
#endif

namespace PJR.Timeline
{
    public interface IClip
    {
        EFrameRate FrameRateType { get; set; }
        bool Mute { get; set; }
        string ClipName { get; set; }
        double start { get; set; }
        double end { get; set; }
        double length { get; }
        int StartFrame { get; set;}
        int EndFrame { get; set;}
        string GetDisplayName();
    }

    [Serializable]
    public abstract class Clip : IClip
    {
        [SerializeField,DisableIf("@true")]
        protected EFrameRate _frameRateType = 0;
        public EFrameRate FrameRateType
        {
            get => _frameRateType;
    #if UNITY_EDITOR
            set => _frameRateType = value;
    #endif
        }
        bool _mute = false;
        public bool Mute { get => _mute; set => _mute = value; }

        public const string DefaultName = "EmptyName";

        public string clipName;
        public string ClipName { get => clipName; set => clipName = value; }
        

        [SerializeField, DisableIf("@true")]
        protected double _start;
        [SerializeField, DisableIf("@true")]
        protected double _end;
        /// <summary>
        /// clip start seconds
        /// </summary>
        public double start 
        {
            get => _start;
            set
            {
                _start = value;
                _startFrame = TimeUtil.ToFrames(value, FrameRate);
            }
        }
        /// <summary>
        /// clip end seconds
        /// </summary>
        public double end
        {
            get => _end;
            set
            {
                _end = value;
                _endFrame = TimeUtil.ToFrames(value, FrameRate);
            }
        }

        public double length => end - start;

        [SerializeField, HideInInspector]
        protected int _startFrame;
        [SerializeField, HideInInspector]
        protected int _endFrame;

        [ShowInInspector]
        public int StartFrame
        {
            get => _startFrame;
            set
            {
                _startFrame = value;
                _start = FrameRateType.SPF() * _startFrame;
            }
        }

        [ShowInInspector]
        public int EndFrame
        {
            get => _endFrame;
            set
            {
                _endFrame = value;
                _end = FrameRateType.SPF() * _endFrame;
            }
        }
        
        public string GetDisplayName() => clipName;
        public Clip() { this.clipName = DefaultName; }
        public Clip(string clipName) { this.clipName = clipName; }

        
        public double SceondPerFrame => FrameRateType.SPF();
        public double FrameRate => FrameRateType.FPS();
        
        public bool OutOfRange(double runTotalTime)=> OutOfRange(runTotalTime, SceondPerFrame);
        public bool OutOfRange(double runTotalTime, double secondPerFrame)
        {
            var __start = _startFrame * secondPerFrame;
            var __end = _endFrame * secondPerFrame;
            if (runTotalTime < __start || runTotalTime > __end)
                return true;
            return false;
        }
    }

    /// <summary>
    /// 记录这个Track的类名,用于在TimelineWindow.Draw_AddTrackButton里画添加按钮
    /// 那为什么是Track不是Clip,留个坑给Track里有多个clip的需求
    /// </summary>
    public class TrackCreateMenuItemAttribute : Attribute
    {
        private string _menuItemName;
        public string MenuItemName => _menuItemName;
        public TrackCreateMenuItemAttribute(string menuItemName) 
        { 
            _menuItemName = menuItemName;
        }
    }

    /// <summary>
    /// 用来存这种IClip用哪个ClipGUI派生类
    /// </summary>
    public class BindingClipGUIAttribute : Attribute
    {
        private Type _clipType;
        public Type ClipType => _clipType;
        public BindingClipGUIAttribute(Type clipType)
        {
            _clipType = clipType;
        }
    }
}
