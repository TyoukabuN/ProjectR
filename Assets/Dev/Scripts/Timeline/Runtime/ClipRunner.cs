using System;
using UnityEngine;
using static PJR.Timeline.Define;

namespace PJR.Timeline
{
    public abstract class ClipRunner : IDisposable
    {
        public enum EState
        {
            None = 0,
            Running,
            Done,
            Failure,
            Diposed,
        }
        public EState state;
        public string error;
        public bool WaitingForStart => state == EState.None;
        public bool Running => state == EState.Running;
        public bool Done => state == EState.Done;

        public abstract Type ClipType { get; }
        public abstract Clip Clip {get;}

        public void AsFailure(string error = null) { state = EState.Failure; this.error = error; }

        public virtual void OnInit() { state = EState.None; }
        public virtual void OnStart(UpdateContext context) { state = EState.Running; }
        public abstract void OnUpdate(UpdateContext context);
        public virtual void OnEnd() { state = EState.Done; }
        public virtual void OnDispose() { state = EState.Diposed; }

        public virtual void Dispose()
        {
            state = EState.Diposed;
            error = string.Empty;
        }

        public TrackRunner sequenceHandle;
    }

    public abstract class ClipRunner<TClip> : ClipRunner where TClip : Clip
    {
        public override Clip Clip => clip;
        public TClip clip => _clip;
        public TClip _clip;
        public ClipRunner(TClip clip)
        {
            if (clip == null || clip.GetType() != ClipType)
            {
                state = EState.Failure;
                error = clip == null ? Define.ErrCode_ClipRunner_ClipIsNull : Define.ErrCode_ClipRunner_ClipTypeNotMatched;
                return;
            }
            _clip = clip;
        }
        public override void Dispose()
        {
            base.Dispose();
            _clip = null;
        }
    }
}
