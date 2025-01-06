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

        public const float SPF_Film = 0.04166667f;
        public const float SPF_HD = 0.03333334f;
        public const float SPF_Gane = 0.01666667f;

        public const float SPF_Default = SPF_Gane;
        public struct UpdateContext
        {
            public int frameCount;
            public float timeScale;
            public float totalTime;

            public float deltaTime;
        }

        //ClipHandle ErrorCode
        public const string ErrCode_ClipHandle_ClipIsNull = "Clip is null";
        public const string ErrCode_ClipHandle_ClipTypeNotMatched = "clip.ClipType not matched with clipHandle.ClipType";

        public delegate ClipHandle Clip2ClipHandleFunc(Clip clip);
    }
}
