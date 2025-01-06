using System;
using static PJR.Timeline.Define;

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
            Diposed,
        }
        public EState state;
        public bool WaitingForStart => state == EState.None;
        public bool Running => state == EState.Running;
        public abstract Type ClipType { get; }
        public abstract Clip Clip {get;}

        public string error;
        public virtual void OnInit() { state = EState.None; }
        public virtual void OnStart() { state = EState.Running; }
        public abstract void OnUpdate(UpdateContext context);
        public virtual void OnDispose() { state = EState.Diposed; }

        public SequenceHandle sequenceHandle;
    }

    public abstract class ClipHandle<TClip> : ClipHandle where TClip : Clip
    {
        public override Clip Clip => clip;
        public TClip clip => _clip;
        public TClip _clip;
        public ClipHandle(TClip clip)
        {
            if (clip == null || clip.GetType() != ClipType)
            {
                state = EState.Failure;
                error = clip == null ? Define.ErrCode_ClipHandle_ClipIsNull : Define.ErrCode_ClipHandle_ClipTypeNotMatched;
                return;
            }
            _clip = clip;
        }
    }
}
