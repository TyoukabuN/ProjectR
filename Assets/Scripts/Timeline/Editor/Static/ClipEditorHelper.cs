using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static PJR.Timeline.Editor.TimelineWindow;

namespace PJR.Timeline.Editor
{
    public static class ClipEditorHelper
    {
        static WindowState windowState => TimelineWindow.instance.State;

        public static void TrySave(this IClip clip)
        {
            var sequenceAsset = windowState.editingSequence.SequenceAsset;
            if (sequenceAsset == null)
                return;
            EditorUtility.SetDirty(sequenceAsset);
        }

        public static void SetClipRangeSafe(this IClip clip, double start, double end)
        {
            if (start < 0)
                start = 0;
            if ((end - start) < windowState.CurrentSecondPerFrame)
                end = windowState.CurrentSecondPerFrame;
            clip.start = start;
            clip.end = end;
        }
        public static void SetClipStartSafe(this IClip clip, double start)
        {
            SetClipRangeSafe(clip, start, clip.end);
        }
        public static void SetClipEndSafe(this IClip clip, double end)
        {
            SetClipRangeSafe(clip, clip.start, end);
        }
        public static bool ValidRangeChangeable(this IClip clip, double second)
        {
            return (clip.start + second) >= 0;
        }
        public static bool ValidRangeChangeableByFrame(this IClip clip, int frames)
        {
            double durationOffset = frames / windowState.CurrentFrameRate;
            return ValidRangeChangeable(clip, durationOffset);
        }

        //将改动帧数控制到合理范围
        public static int ClampToValidFrameOffset(this IClip clip, int frameOffset)
        {
            var startFrame = clip.StartFrame + frameOffset;
            var endFrame = clip.EndFrame + frameOffset;
            if (startFrame < 0)
                return frameOffset - startFrame;
            return frameOffset;
        }

        public static bool ValidRangeChangeableByPixel(this IClip clip, float pixel)
        {
            return ValidRangeChangeableByFrame(clip, windowState.PixelToFrame(pixel));
        }


        #region IClip Start/End Clamp 得出一个限制后的对应单位的Start/End

        public static double ClampClipEndChange(this IClip clip, double end)
        {
            var start = clip.start;
            if ((end - start) < windowState.CurrentSecondPerFrame)
                end = start + windowState.CurrentSecondPerFrame;
            return end;
        }
        public static float ClampClipEndChange_Pixel(this IClip clip, float pixel) => windowState.FrameToPixel(ClampClipEndChange_Frame(clip, windowState.PixelToFrame(pixel)));
        public static int ClampClipEndChange_Frame(this IClip clip, int frames) => TimeUtil.ToFrames(ClampClipEndChange(clip, frames / windowState.CurrentFrameRate), windowState.CurrentFrameRate);


        public static double ClampClipStartChange(this IClip clip, double start)
        {
            var end = clip.end;
            if (start < 0)
                start = 0;
            if ((end - start) < windowState.CurrentSecondPerFrame)
                start = end - windowState.CurrentSecondPerFrame;
            return start;
        }
        public static float ClampClipStartChange_Pixel(this IClip clip, float pixel) => ClampClipStartChange_Frame(clip, windowState.PixelToFrame(pixel));
        public static int ClampClipStartChange_Frame(this IClip clip, int frames) => TimeUtil.ToFrames(ClampClipStartChange(clip, frames / windowState.CurrentFrameRate), windowState.CurrentFrameRate);
        #endregion
    }
}
