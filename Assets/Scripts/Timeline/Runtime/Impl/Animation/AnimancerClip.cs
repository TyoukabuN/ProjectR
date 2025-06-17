using Animancer;
using System;
using UnityEngine;
using static PJR.Timeline.Define;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
        public override ClipRunner GetRunner()=> AnimancerClipRunner.Get(this);

#if UNITY_EDITOR
        public override void GetContextMenu(GenericMenu menu)
        {
            menu.AddDisabledItem(new GUIContent("播放动画"));
        }
#endif
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
            Debug.Log("Clip Start!");
            animancer = context.gameObject?.GetComponent<AnimancerComponent>();
            if (animancer == null)
                AsFailure();
            else if (clip.animationClip == null)
                AsFailure("clip.animationClip == null");
            animancerState = animancer.Layers[0].GetOrCreateState(clip.animationClip);
            if(animancerState != null)
                animancerState.IsPlaying = true;
        }
        public override void OnUpdate(UpdateContext context)
        {
            if (animancerState == null)
                return;
            animancer.Layers[0].Play(animancerState);
            //Debug.Log(GetLocalSecond());
            animancerState.Time = GetLocalSecond();
        }
        public override void OnEnd()
        {
            base.OnEnd();
            Clear();
        }
        public override void Dispose()
        {
            base.Dispose();
            Clear();
        }
        void Clear()
        {
            if (animancerState != null)
                animancerState.IsPlaying = false;
        }

        public override void Release()
        {
            Pool.ObjectPool<AnimancerClipRunner>.Release(this);
        }
        
        public static ClipRunner Get(AnimancerClip clip)=> Pool.ObjectPool<AnimancerClipRunner>.Get()?.Reset(clip);
    }
}
