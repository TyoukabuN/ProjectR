using UnityEngine;

namespace PJR.Timeline
{
    public partial class AnimancerClip : Clip
    {
        public static AnimancerClip GetRuntimeTemplate(AnimationClip clip, int endFrame)
            => GetRuntimeTemplate(clip, 0, endFrame);
        public static AnimancerClip GetRuntimeTemplate(AnimationClip clip, int startFrame, int endFrame)
        {
            var temp = CreateInstance<AnimancerClip>();
            temp.animationClip = clip;
            temp.SetFrameScope(startFrame, endFrame);
            return temp;
        }
        
        public static AnimancerClip GetRuntimeTemplate(AnimationClip clip, float endTime)
            => GetRuntimeTemplate(clip, 0f, endTime);
        public static AnimancerClip GetRuntimeTemplate(AnimationClip clip, float startTime, float endTime)
        {
            var temp = CreateInstance<AnimancerClip>();
            temp.animationClip = clip;
            temp.SetTimeScope(startTime, endTime);
            return temp;
        }
        
    }
}