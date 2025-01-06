using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor.PackageManager;
using UnityEngine;

namespace PJR.Timeline
{
    public abstract class ClipHandle
    {
        public enum EState
        {
            None = 0,
            Running,
            Done,
            Failure,
        }
        public EState state;
        public bool WaitingForStart => state == EState.None;
        public bool Running => state == EState.Running;
        public abstract int ClipType { get; }
        public abstract Clip Clip {get;}

        public string error;
        public virtual bool CanStart() => true;
        public virtual void OnInit() { }
        public virtual void OnStart() { }
        public abstract void OnUpdate(UpdateContext context);
        public virtual void OnDispose() { }

        public SequenceHandle sequenceHandle;
    }

    public abstract class ClipHandle<TClip> : ClipHandle where TClip : Clip
    {
        public override Clip Clip => clip;
        public TClip clip => _clip;
        public TClip _clip;
        public ClipHandle(SequenceHandle seqHandle, TClip clip)
        {
            sequenceHandle = seqHandle;

            if (clip == null || clip.ClipType != ClipType)
            {
                state = EState.Failure;
                error = clip == null ? Define.ErrCode_ClipHandle_ClipIsNull : Define.ErrCode_ClipHandle_ClipTypeNotMatched;
                return;
            }
        }
    }
}
