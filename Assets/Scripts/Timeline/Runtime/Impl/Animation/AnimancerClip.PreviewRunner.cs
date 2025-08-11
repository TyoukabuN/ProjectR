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
        
            public PreviewRunner() : base(null){}
            public PreviewRunner(AnimancerClip clip) : base(clip) { }
        
            public static ClipRunner Get(AnimancerClip clip)=> Pool.ObjectPool<PreviewRunner>.Get()?.Reset(clip);
        
            public override void OnStart(Define.UpdateContext context)
            {
                base.OnStart(context);
                _animancer = context.gameObject?.GetComponent<AnimancerComponent>();
                if (_animancer == null)
                    AsFailure("animancer == null");
                else if (clip.animationClip == null)
                    AsFailure("clip.animationClip == null");
                _animator = context.gameObject?.GetComponent<Animator>();
                if (_animancer == null)
                    AsFailure("animator == null");
                
                EditModeSampleAnimation(clip.animationClip, _animator);
                //clip.animationClip.EditModePlay(_animator);

                if(IsFailure)
                    return;
                //_animancer.runInEditMode = true;

                //animancerState = animancer.Layers[0].GetOrCreateState(clip.animationClip);
                // if (animancerState != null)
                // {
                //     animancer.Layers[0].Play(animancerState);
                // }
                // animancer.Playable.UpdateMode = DirectorUpdateMode.Manual;
            }
            public override void OnUpdate(Define.UpdateContext context)
            {
                //Debug.Log($"[AnimancerClip.PreviewRunner.OnUpdate] GetLocalSecond:{GetLocalSecond()}");
                var time = GetLocalSecond();
                //clip.animationClip.EditModeSampleAnimation(_animator,time);
                //animancerState.Time = GetLocalSecond();
                // animancer.Playable.UpdateMode = DirectorUpdateMode.Manual;
                // animancer.Evaluate((float)context.deltaTime);

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

            public override void OnEnd()
            {
                base.OnEnd();
                Clear();
            }
            public override void Clear()
            {
                base.Clear();
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
