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
        string Description { get; set; }
        double start { get; set; }
        double end { get; set; }
        double duration { get; }
        int TotalFrame { get; }
        int StartFrame { get; set;}
        int EndFrame { get; set;}
        string GetClipName();
        string GetClipInfo();
        public ClipRunner GetRunner();
        public Color GetClipColor();
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
        [LabelText("禁用"), SerializeField]
        private bool _mute = false;
        public bool Mute { get => _mute; set => _mute = value; }

        [LabelText("描述"), SerializeField]
        private string _description;
        public string Description { get => _description; set => _description = value; }
        public virtual string GetClipName() => "[未命名Clip类型]";
        public virtual string GetClipInfo() => string.Empty;
        
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

        public double duration => end - start;
        public int TotalFrame => EndFrame - StartFrame;

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
        public Clip() { }
        public Clip(string description) { this._description = description; }

        
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
        public abstract ClipRunner GetRunner();
        public virtual Color GetClipColor() => Color.green;
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
    /// 用来存这种IClip用哪个TrackDrawer派生类
    /// </summary>
    public class BindingTrackDrawerAttribute : Attribute
    {
        private Type _type;
        public Type Type => _type;
        public BindingTrackDrawerAttribute(Type type) => _type = type;
    }
    
    
    public class BindingClipDrawerAttrubute : Attribute
    {
        private Type _type;
        public Type Type => _type;
        public BindingClipDrawerAttrubute(Type type) => _type = type;
    }
}
