using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PJR.Timeline;

namespace PJR
{
    public class TestClip : Clip
    {
        public int intValue;
    }

    public class TestClipHandle : ClipHandle<TestClip>
    {
        public override Type ClipType => typeof(TestClip);
        int _counter = 0;

        public TestClipHandle(TestClip clip) : base(clip) { }
        public override void OnUpdate(Define.UpdateContext context)
        {
            Debug.Log(_counter += clip.intValue);
        }
    }
}
