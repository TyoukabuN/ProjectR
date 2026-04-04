#if UNITY_EDITOR
using System;
using Animancer;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace PJR.Timeline
{
    public partial class AnimancerClip
    {
        public class PreviewRunner : ClipRunner<AnimancerClip>
        {
            public override Type ClipType => typeof(AnimancerClip);
            private AnimancerComponent _animancer;
            private Animator _animator;
            private AnimancerState _animancerState;
        
            public PreviewRunner() : base(){}
            public PreviewRunner(AnimancerClip clip) : base(clip) { }
        
            public static ClipRunner Get(AnimancerClip clip)=> Pool.ObjectPool<PreviewRunner>.Get()?.Reset(clip);
        
            public override void OnStart(UpdateContext context)
            {
                base.OnStart(context);
                _animancer = context.gameObject?.GetComponent<AnimancerComponent>();
                if (_animancer == null)
                    AsFailure("animancer == null");
                else if (clip.animationClip == null)
                    AsFailure("clip.animationClip == null");
                _animator = context.gameObject?.GetComponent<Animator>();
                if (_animator == null)
                    AsFailure("animator == null");
                
                if(IsFailure)
                    return;

                if (!AnimationMode.InAnimationMode())
                    AnimationMode.StartAnimationMode();

                EditModeSampleAnimation(clip.animationClip, _animator);
            }
            protected override void OnFrameUpdate(UpdateContext context)
            {
            }
            protected override void OnDeltaUpdate(UpdateContext context)
            {
                var time = GetLocalSecond();
                AnimationMode.BeginSampling();
                EditModeSampleAnimation(clip.animationClip, _animator, time);
                AnimationMode.EndSampling();
            }
            private static bool ShouldEditModeSample(AnimationClip clip, Component component)
            {
                if(EditorApplication.isPlayingOrWillChangePlaymode)
                    return false;
                if(clip == null || component == null)
                    return false;
                if(EditorUtility.IsPersistent(component))
                    return false;
                return true;
            }

            private static void EditModeSampleAnimation(AnimationClip clip, Animator component, float time = 0f)
            {
                if (!ShouldEditModeSample(clip, component))
                    return;
                AnimationMode.SampleAnimationClip(component.gameObject, clip, time);
            }
            protected override void OnClear()
            {
                base.OnClear();
                if (AnimationMode.InAnimationMode())
                    AnimationMode.StopAnimationMode();
                _animancer = null;
                _animator = null;
                _animancerState = null;
            }
            public override void Release()
            {
                Clear();
                Pool.ObjectPool<PreviewRunner>.Release(this);
            }
        }
    }
}
#endif
