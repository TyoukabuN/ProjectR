using UnityEngine;

namespace PJR.Timeline.Editor
{
    static class TimeReferenceUtility
    {
        static TimelineWindow.WindowState state { get { return TimelineWindow.instance.State; } }

        public static float PixelToTime(Vector2 mousePos)
        {
            return PixelToTime(mousePos.x);
        }

        public static float PixelToTime(float pixelX)
        {
            return (float)state.PixelToTime(pixelX);
        }
        //
        // public static double GetSnappedTimeAtMousePosition(Vector2 mousePos)
        // {
        //     return state.GetSnappedTimeAtMousePosition(mousePos);
        // }
        //
        // public static double SnapToFrameIfRequired(double currentTime)
        // {
        //     return TimelinePreferences.instance.snapToFrame ? SnapToFrame(currentTime) : currentTime;
        // }
        //
        // public static double SnapToFrame(double time)
        // {
        //     if (state.timeReferenceMode == TimeReferenceMode.Global)
        //     {
        //         time = state.editSequence.ToGlobalTime(time);
        //         time = TimeUtility.RoundToFrame(time, state.referenceSequence.frameRate);
        //         return state.editSequence.ToLocalTime(time);
        //     }
        //
        //     return TimeUtility.RoundToFrame(time, state.referenceSequence.frameRate);
        // }
        //
        
        public static string ToTimeString(double time, string format = "F2")
        {
            if (EditorPreferences.TimeReferenceMode.Value == TimeReferenceMode.Global)
                time = state.SequenceHandle.ToGlobalTime(time);
        
            //return state.timeFormat.ToTimeString(time, state.referenceSequence.frameRate, format);
            return EditorPreferences.TimeFormat.Value.ToTimeString(time, state.SequenceHandle.Sequence.FrameRateType.FPS(), format);
        }
        
        public static double FromTimeString(string timeString)
        {
            //double newTime = state.timeFormat.FromTimeString(timeString, state.referenceSequence.frameRate, -1);
            double newTime = EditorPreferences.TimeFormat.Value.FromTimeString(timeString, state.SequenceHandle.Sequence.FrameRateType.FPS(), -1);
            if (newTime >= 0.0)
            {
                // return state.timeReferenceMode == TimeReferenceMode.Global ?
                //     state.editingSequence.ToLocalTime(newTime) : newTime;
                return EditorPreferences.TimeReferenceMode.Value == TimeReferenceMode.Global ?
                    state.SequenceHandle.ToLocalTime(newTime) : newTime;
            }
        
            return state.SequenceHandle.time;
        }
    }
}
