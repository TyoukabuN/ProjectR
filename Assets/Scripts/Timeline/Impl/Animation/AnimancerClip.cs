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

#if UNITY_EDITOR
        public static void Create()
        {
        }
#endif
    }

    public class AnimancerClipRunner : ClipRunner<AnimancerClip>
    {
        public override Type ClipType => typeof(AnimancerClip);
        int _counter = 0;
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
