using Animancer;
using System;
using UnityEngine;
using static PJR.Timeline.Define;

namespace PJR.Timeline
{
    [Serializable]
    [TrackCreateMenuItem(nameof(AnimancerClip))]
    public class AnimancerClip : Clip
    {
        public AnimationClip animationClip;
        public override string GetClipName() => "播放动画";
        public override string GetClipInfo()
        {
            if (animationClip == null)
                return "[播放动画] null";
            return $"[播放动画] {animationClip?.name ?? string.Empty}";
        }
        public override ClipRunner GetRunner() => Pool.ObjectPool<AnimancerClipRunner>.Get();
    }

    public class AnimancerClipRunner : ClipRunner<AnimancerClip>
    {
        public override Type ClipType => typeof(AnimancerClip);
        private AnimancerComponent animancer;
        private AnimancerState animancerState;

        public AnimancerClipRunner() : base(null){}
        public AnimancerClipRunner(AnimancerClip clip) : base(clip) { }

        public override void OnStart(UpdateContext context)
        {
            base.OnStart(context);
            animancer = context.gameObject?.GetComponent<AnimancerComponent>();
            if (animancer == null)
                AsFailure();
            else if (clip.animationClip == null)
                AsFailure("clip.animationClip == null");
        }
        public override void OnUpdate(UpdateContext context)
        {
            animancerState ??= animancer.Layers[0].GetOrCreateState(clip.animationClip);
            animancer.Layers[0].Play(animancerState);
            animancerState.Time = (float)context.totalTime;
        }
        public override void OnEnd()
        {
            base.OnEnd();
            animancerState.IsPlaying = false;
        }
        public override void Dispose()
        {
            base.Dispose();
            animancerState.IsPlaying = false;
        }
        public override void Release()
        {
            Pool.ObjectPool<AnimancerClipRunner>.Release(this);
        }
    }
}
