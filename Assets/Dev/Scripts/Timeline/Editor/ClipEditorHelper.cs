using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PJR.Timeline.Editor.TimelineWindow;

namespace PJR.Timeline.Editor
{
    public static class ClipEditorHelper
    {
        static WindowState windowState => TimelineWindow.instance.state;
        public static void SetClipRangeSafe(this Clip clip, double start, double end)
        {
            if (start < 0)
                start = 0;
            if ((end - start) < windowState.CurrentSecondPerFrame)
                end = windowState.CurrentSecondPerFrame;
            clip.start = start;
            clip.end = end;
        }
        public static void SetClipStartSafe(this Clip clip, double start)
        {
            SetClipRangeSafe(clip, start, clip.end);
        }
        public static void SetClipEndSafe(this Clip clip, double end)
        {
            SetClipRangeSafe(clip, clip.start, end);
        }
        public static bool ValidRangeChangeable(this Clip clip, double second)
        {
            return (clip.start + second) >= 0;
        }
        public static bool ValidRangeChangeableByFrame(this Clip clip, int frames)
        {
            double durationOffset = frames / windowState.CurrentFrameRate;
            return ValidRangeChangeable(clip, durationOffset);
        }
        public static bool ValidRangeChangeableByPixel(this Clip clip, float pixel)
        {
            return ValidRangeChangeableByFrame(clip, windowState.PixelToFrame(pixel));
        }


        #region Clip Start/End Clamp 得出一个限制后的对应单位的Start/End

        public static double ClampClipEndChange(this Clip clip, double end)
        {
            var start = clip.start;
            if ((end - start) < windowState.CurrentSecondPerFrame)
                end = start + windowState.CurrentSecondPerFrame;
            return end;
        }
        public static float ClampClipEndChange_Pixel(this Clip clip, float pixel) => windowState.FrameToPixel(ClampClipEndChange_Frame(clip, windowState.PixelToFrame(pixel)));
        public static int ClampClipEndChange_Frame(this Clip clip, int frames) => TimeUtil.ToFrames(ClampClipEndChange(clip, frames / windowState.CurrentFrameRate), windowState.CurrentFrameRate);


        public static double ClampClipStartChange(this Clip clip, double start)
        {
            var end = clip.end;
            if (start < 0)
                start = 0;
            if ((end - start) < windowState.CurrentSecondPerFrame)
                start = end - windowState.CurrentSecondPerFrame;
            return start;
        }
        public static float ClampClipStartChange_Pixel(this Clip clip, float pixel) => ClampClipStartChange_Frame(clip, windowState.PixelToFrame(pixel));
        public static int ClampClipStartChange_Frame(this Clip clip, int frames) => TimeUtil.ToFrames(ClampClipStartChange(clip, frames / windowState.CurrentFrameRate), windowState.CurrentFrameRate);
        #endregion
    }
}
