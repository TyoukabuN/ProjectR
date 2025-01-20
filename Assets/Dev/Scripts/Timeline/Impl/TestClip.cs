using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PJR.Timeline;
using Animancer;

namespace PJR
{
    public class TestClip : Clip
    {
        public int intValue;
        public float floatValue;
        public Transform transValue;
        public AnimationClip animationClip;
    }

    public class TestClipHandle : ClipRunner<TestClip>
    {
        public override Type ClipType => typeof(TestClip);
        int _counter = 0;
        private AnimancerComponent animancer;
        private AnimancerState state;

        public TestClipHandle(TestClip clip) : base(clip) { }

        public override void OnStart()
        {
            base.OnStart();
            animancer = clip.transValue.GetComponent<AnimancerComponent>();
        }
        public override void OnUpdate(Define.UpdateContext context)
        {
            if (clip.transValue != null)
            {
                if (clip.animationClip != null)
                {

                    state ??= animancer.Layers[0].GetOrCreateState(clip.animationClip);
                    animancer.Layers[0].Play(state);
                    state.Time = (float)context.totalTime;
                    Debug.Log(state.Time);
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
            state.IsPlaying = false;
        }
        public override void Dispose()
        {
            base.Dispose();
            state.IsPlaying = false;
        }
    }
}
