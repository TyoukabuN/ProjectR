using Sirenix.OdinInspector;
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
}
