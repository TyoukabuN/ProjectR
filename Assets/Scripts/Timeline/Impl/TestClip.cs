using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PJR.Timeline;
using Animancer;
using static PJR.Timeline.Define;

namespace PJR
{
    [Serializable]
    public class TestClip : Clip
    {
        public int intValue;
        public float floatValue;
        public Transform transValue;
        public AnimationClip animationClip;
    }

    public class TestClipRunner : ClipRunner<TestClip>
    {
        public override Type ClipType => typeof(TestClip);
        int _counter = 0;
        private AnimancerComponent animancer;
        private AnimancerState animancerState;

        public TestClipRunner():base(null){}
        public TestClipRunner(TestClip clip) : base(clip) { }

        public override void OnStart(UpdateContext context)
        { 
            base.OnStart(context);
            animancer = clip.transValue.GetComponent<AnimancerComponent>();
        }
        public override void OnUpdate(Define.UpdateContext context)
        {
            if (clip.transValue != null)
            {
                if (clip.animationClip != null)
                {

                    animancerState ??= animancer.Layers[0].GetOrCreateState(clip.animationClip);
                    animancer.Layers[0].Play(animancerState);
                    animancerState.Time = (float)context.totalTime;
                    Debug.Log(animancerState.Time);
                }
                else
                { 
                    clip.transValue.position += Vector3.forward * clip.floatValue * (float)context.deltaTime;
                }
            }
            //Debug.Log(_counter += clip.intValue);
            //Debug.Log($"[time] {context.totalTime}");
            //if (context.frameChanged)
            //    Debug.Log($"[Frame] {context.totalFrame}  {context.totalTime}");
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


        #region Pool
        public static TestClipRunner Get(TestClip clip)
        {
            var runner = Timeline.Pool.ObjectPool<TestClipRunner>.Get();
            runner?.Reset(clip);
            return runner;
        }
        public override void Release()
        {
            Timeline.Pool.ObjectPool<TestClipRunner>.Release(this);
        }
        #endregion
    }
}
