using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJR.Timeline
{
    public static class Define
    {
        //ClipHandle ErrorCode
        public const string ErrCode_ClipHandle_ClipIsNull = "Clip is null";
        public const string ErrCode_ClipHandle_ClipTypeNotMatched = "clip.ClipType not matched with clipHandle.ClipType";

        public delegate ClipHandle Clip2ClipHandleFunc(Clip clipHandle);
    }
}
