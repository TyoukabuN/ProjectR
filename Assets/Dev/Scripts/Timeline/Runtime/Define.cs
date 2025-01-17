using NPOI.POIFS.FileSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJR.Timeline
{
    public static class Define
    {
        public enum EFrameRate
        {
            Film, //24 fps
            HD, //30 fps
            Game, //60 fps 
        }

        public const double SPF_Film = 0.04166667f;
        public const double SPF_HD = 0.03333334f;
        public const double SPF_Gane = 0.01666667f;
        public const double SPF_Default = SPF_Gane;

        public const double FPS_Film = 24;
        public const double FPS_HD = 30;
        public const double FPS_Gane = 60;
        public const double FPS_Default = FPS_Gane;

        public const double MinFrameRate = TimeUtil.kFrameRateEpsilon;
        public const double MaxFrameRate = 1000.0;
        public const double DefaultFrameRate = 60.0;

        
        public struct UpdateContext
        {
            public int frameCount;
            public double timeScale;
            public double elapseTime;
            public int elapseFrame;

            public double unscaledDeltaTime;
            public double deltaTime;

            public bool frameChanged;
        }

        public const string ErrCode_TrackRuner_TrackIsNull = "[TrackRuner] Track is null";
        public const string ErrCode_TrackRuner_Clip2ClipHandle = "[TrackRuner] clip2ClipHandle is null";
        //ClipHandle ErrorCode
        public const string ErrCode_ClipRunner_ClipIsNull = "[ClipRunner] Clip is null";
        public const string ErrCode_ClipRunner_ClipTypeNotMatched = "[ClipRunner] clip.ClipType not matched with clipHandle.ClipType";

        public delegate ClipRunner Clip2ClipHandleFunc(Clip clip);
    }
}
