#if UNITY_EDITOR
using System;
using Animancer;
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

                EditModeSampleAnimation(clip.animationClip, _animator);
            }
            protected override void OnFrameUpdate(UpdateContext context)
            {
            }
            protected override void OnDeltaUpdate(UpdateContext context)
            {
                var time = GetLocalSecond();
                EditModeSampleAnimation(clip.animationClip, _animator, time);
            }
            private static bool ShouldEditModeSample(AnimationClip clip, Component component)
            {
                return
                    !EditorApplication.isPlayingOrWillChangePlaymode &&
                    clip != null &&
                    component != null &&
                    !EditorUtility.IsPersistent(component);
            }

            private static void EditModeSampleAnimation(AnimationClip clip, Animator component, float time = 0f)
            {
                if (!ShouldEditModeSample(clip, component))
                    return;

                clip.SampleAnimation(component.gameObject, time);
            }
            protected override void OnClear()
            {
                base.OnClear();
                if (_animancerState != null)
                    _animancerState.IsPlaying = false;
                _animator?.Rebind();
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
