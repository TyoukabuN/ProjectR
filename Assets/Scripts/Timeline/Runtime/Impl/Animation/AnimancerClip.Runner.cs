using System;
using Animancer;

namespace PJR.Timeline
{
    public partial class AnimancerClip
    {
        public class Runner : ClipRunner<AnimancerClip>
        {
            public override Type ClipType => typeof(AnimancerClip);
            private AnimancerComponent animancer;
            private AnimancerState animancerState;
        
            public Runner() : base(null){}
            public Runner(AnimancerClip clip) : base(clip) { }
        
            public static ClipRunner Get(AnimancerClip clip)=> Pool.ObjectPool<Runner>.Get()?.Reset(clip);
        
            public override void OnStart(Define.UpdateContext context)
            {
                base.OnStart(context);
                Debug.Log("Clip Start!");
                animancer = context.gameObject?.GetComponent<AnimancerComponent>();
                if (animancer == null)
                    AsFailure();
                else if (clip.animationClip == null)
                    AsFailure("clip.animationClip == null");
                
                if(IsFailure)
                    return;
                animancerState = animancer.Layers[0].GetOrCreateState(clip.animationClip);
                if (animancerState != null)
                {
                    animancer.Layers[0].Play(animancerState);
                }
            }
            public override void OnUpdate(Define.UpdateContext context)
            {
                if (animancerState == null)
                    return;
                Debug.Log($"[AnimancerClip.Runner.OnUpdate] GetLocalSecond:{GetLocalSecond()}");
                animancerState.Time = GetLocalSecond();
            }
            public override void OnEnd()
            {
                base.OnEnd();
                Clear();
            }
            public override void Clear()
            {
                base.Clear();
                if (animancerState != null)
                    animancerState.IsPlaying = false;
            }
        
            public override void Release()
            {
                Clear();
                Pool.ObjectPool<Runner>.Release(this);
            }
        }
    }
}