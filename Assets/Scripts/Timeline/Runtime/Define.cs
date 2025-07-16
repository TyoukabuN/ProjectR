using System;
using System.Globalization;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace PJR.Timeline
{
    public static class Define
    {
        public enum EFrameRate
        {
            [LabelText("游戏(60fps)")]
            Game = 0, //60 fps 
            [LabelText("高清(30fps)")]
            HD, //30 fps
            [LabelText("电影(24fps)")]
            Film, //24 fps
        }
        public static double SPF(this EFrameRate frameRateType)
        {
            switch (frameRateType)
            {
                case EFrameRate.Film:
                    return SPF_Film;
                case EFrameRate.HD:
                    return SPF_HD;
                case EFrameRate.Game:
                    return SPF_Gane;
                default:
                    return SPF_Default;
            }        
        }
        
        public static double FPS(this EFrameRate frameRateType)
        {
            switch (frameRateType)
            {
                case EFrameRate.Film:
                    return FPS_Film;
                case EFrameRate.HD:
                    return FPS_HD;
                case EFrameRate.Game:
                    return FPS_Gane;
                default:
                    return FPS_Default;
            }        
        }


        public const double SPF_Film = 0.04166667f;
        public const double SPF_HD = 0.03333334f;
        public const double SPF_Gane = 0.01666667f;
        public const double SPF_Default = SPF_Gane;

        public const double FPS_Film = 24;
        public const double FPS_HD = 30;
        public const double FPS_Gane = 60;
        public const double FPS_Default = FPS_Gane;

        public const double MinFrameRate = 1e-6;
        public const double MaxFrameRate = 1000.0;
        public const double DefaultFrameRate = 60.0;

        public enum IntervalType
        {
            Second = 0,
            Frame,
        }
        public struct UpdateContext
        {
            public double timeScale;
            public double totalTime;
            public int totalFrame;

            public double unscaledDeltaTime;
            public double deltaTime;

            public bool frameChanged;

            /// <summary>
            /// 不用更新间隔类型，所更新字段不一样
            /// </summary>
            public IntervalType updateIntervalType;

            public GameObject gameObject;
        }

        public const string Label_NonEditingSequenceTip = "没有选中任何Sequence asset";

        public const string ErrCode_TrackRuner_TrackIsNull = "[TrackRuner] Track is null";
        public const string ErrCode_TrackRuner_Clip2ClipHandle = "[TrackRuner] clip2ClipHandle is null";
        //ClipHandle ErrorCode
        public const string ErrCode_ClipRunner_ClipIsNull = "[ClipRunner] Clip is null";
        public const string ErrCode_ClipRunner_ClipTypeNotMatched = "[ClipRunner] clip.ClipType not matched with clipHandle.ClipType";

        public delegate ClipRunner Clip2ClipHandleFunc(IClip clip);
        
       
    }
    
    public enum TimeReferenceMode
    {
        Local = 0,
        Global = 1
    }
    
    public enum TimeFormat
    {
        /// <summary>Displays time values as frames.</summary>
        Frames,

        /// <summary>Displays time values as timecode (SS:FF) format.</summary>
        Timecode,

        /// <summary>Displays time values as seconds.</summary>
        Seconds
    }
    
    public static class TimeDisplayUnitExtensions
    {
        // public static TimeArea.TimeFormat ToTimeAreaFormat(this TimeFormat timeDisplayUnit)
        // {
        //     switch (timeDisplayUnit)
        //     {
        //         case TimeFormat.Frames: return TimeArea.TimeFormat.Frame;
        //         case TimeFormat.Timecode: return TimeArea.TimeFormat.TimeFrame;
        //         case TimeFormat.Seconds: return TimeArea.TimeFormat.None;
        //     }
        //
        //     return TimeArea.TimeFormat.Frame;
        // }

        public static string ToTimeString(this TimeFormat timeFormat, double time, double frameRate, string format = "f2")
        {
            switch (timeFormat)
            {
                case TimeFormat.Frames: return TimeUtil.TimeAsFrames(time, frameRate, format);
                case TimeFormat.Timecode: return TimeUtil.TimeAsTimeCode(time, frameRate, format);
                case TimeFormat.Seconds: return time.ToString(format, (IFormatProvider)CultureInfo.InvariantCulture.NumberFormat);
            }

            return time.ToString(format);
        }

        public static string ToTimeStringWithDelta(this TimeFormat timeFormat, double time, double frameRate, double delta, string format = "f2")
        {
            const double epsilon = 1e-7;
            var result = ToTimeString(timeFormat, time, frameRate, format);
            if (delta > epsilon || delta < -epsilon)
            {
                var sign = ((delta >= 0) ? "+" : "-");
                var deltaStr = ToTimeString(timeFormat, Math.Abs(delta), frameRate, format);
                return $"{result} ({sign}{deltaStr})";
            }
            return result;
        }

        public static double FromTimeString(this TimeFormat timeFormat, string timeString, double frameRate, double defaultValue)
        {
            double time = defaultValue;
            switch (timeFormat)
            {
                case TimeFormat.Frames:
                    if (!double.TryParse(timeString, NumberStyles.Any, CultureInfo.InvariantCulture, out time))
                        return defaultValue;
                    time = TimeUtil.FromFrames(time, frameRate);
                    break;
                case TimeFormat.Seconds:
                    time = TimeUtil.ParseTimeSeconds(timeString, frameRate, defaultValue);
                    break;
                case TimeFormat.Timecode:
                    time = TimeUtil.ParseTimeCode(timeString, frameRate, defaultValue);
                    break;
                default:
                    time = defaultValue;
                    break;
            }

            return time;
        }
    }
    
    public static class EditorGUIReflection
    {
        private static Type _editorGuiType;
        private static MethodInfo _delayedTextFieldMethod;

        static EditorGUIReflection()
        {
            // 获取EditorGUI类型（来自UnityEditor.CoreModule.dll）
            _editorGuiType = Type.GetType("UnityEditor.EditorGUI, UnityEditor.CoreModule");
        
            // 获取方法信息
            _delayedTextFieldMethod = _editorGuiType.GetMethod(
                "DelayedTextFieldInternal",
                BindingFlags.Static | BindingFlags.NonPublic,
                null,
                new Type[] {
                    typeof(Rect),
                    typeof(int),
                    typeof(GUIContent),
                    typeof(string),
                    typeof(string),
                    typeof(GUIStyle)
                },
                null);
        }

        public static string DelayedTextField(
            Rect position,
            int id,
            GUIContent label,
            string value,
            string allowedLetters = null,
            GUIStyle style = null)
        {
            // 设置默认参数
            allowedLetters = allowedLetters ?? string.Empty;
            style = style ?? EditorStyles.textField;

            // 通过反射调用方法
            return (string)_delayedTextFieldMethod.Invoke(
                null,
                new object[] {
                    position,
                    id,
                    label,
                    value,
                    allowedLetters,
                    style
                });
        }
    }
}
