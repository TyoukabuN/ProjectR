using System;
using PJR.Timeline.Pool;
using UnityEngine;
using static PJR.Timeline.Define;

namespace PJR.Timeline
{
    public abstract class ClipRunner : PoolableObject , IDisposable
    {
        public enum EState
        {
            None = 0,
            Running,
            Done,
            Failure,
            Diposed,
        }
        EState _state = 0;
        public EState State
        {
            get => _state;
            protected set
            {
                if (_state == value)
                    return;
                Internal_OnStateChanged(_state, value);
                _state = value;
            }
        }
        
        public string error;
        public bool WaitingForStart => State == EState.None;
        public bool Running => State == EState.Running;
        public bool Done => State == EState.Done;

        public abstract Type ClipType { get; }
        public abstract Clip Clip {get;}

        public void AsFailure(string error = null)
        {
            State = EState.Failure; this.error = error;
            Debug.LogError($"ClipRunner Failure: {error}");
        }
        public virtual void OnInit() { State = EState.None; }
        public virtual void OnStart(UpdateContext context) { State = EState.Running; }
        public abstract void OnUpdate(UpdateContext context);
        public virtual void OnEnd() { State = EState.Done; }
        public virtual void OnDispose() { State = EState.Diposed; }

        public virtual void Dispose()
        {
            State = EState.Diposed;
            error = string.Empty;
            updateContext = null;
        }

        public TrackRunner trackRunner;

        public UpdateContext? updateContext;
        public void SetUpdateContext(UpdateContext context)
        {
            updateContext = context;
        }

        public float GetLocalSecond()
        {
            if (updateContext == null)
                return 0;
            return (float)(updateContext.Value.totalTime - Clip.start);
        }
        
        public Action<EState, EState> OnStateChanged;
        protected void Internal_OnStateChanged(EState oldState, EState newState)
        {
            OnStateChanged?.Invoke(oldState, newState);
            if (newState == EState.Done)
                Internal_OnDone();
        }
        protected void Internal_OnDone()
        {
            Debug.Log("Sequence Done");
        }
    }

    public abstract class ClipRunner<TClip> : ClipRunner where TClip : Clip
    {
        public override Clip Clip => clip;
        protected TClip _clip;
        public TClip clip => _clip;
        public ClipRunner(TClip clip)=>Reset(clip);
        public ClipRunner Reset(TClip clip)
        {
            if (clip == null || clip.GetType() != ClipType)
            {
                State = EState.Failure;
                error = clip == null ? Define.ErrCode_ClipRunner_ClipIsNull : Define.ErrCode_ClipRunner_ClipTypeNotMatched;
                return this;
            }
            _clip = clip;
            return this;
        }

        public override void Dispose()
        {
            base.Dispose();
            _clip = null;
        }
    }
}
