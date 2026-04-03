using System;
using PJR.Timeline.Pool;
using static PJR.Timeline.Define;

namespace PJR.Timeline
{
    public abstract class ClipRunner : UnitRunner
    {
        public bool Running => IsRunning;
        public bool Done => IsDone;

        public abstract Type ClipType { get; }
        public abstract Clip Clip { get; }

        public virtual void OnInit() { runnerState = ERunnerState.None; }
        public virtual void OnStart(UpdateContext context) { runnerState = ERunnerState.Running; }
        /// <summary>
        /// 以帧间隔更新
        /// </summary>
        /// <param name="context"></param>
        protected abstract void OnFrameUpdate(UpdateContext context);
        /// <summary>
        /// 以deltaTime间隔更新
        /// </summary>
        /// <param name="context"></param>
        protected abstract void OnDeltaUpdate(UpdateContext context);
        public override void OnUpdate(UpdateContext context)
        {
            if (context.updateIntervalType == IntervalType.Frame)
                OnFrameUpdate(context);
            else
                OnDeltaUpdate(context);
        }

        public virtual void End() 
        { 
            runnerState = ERunnerState.Done; 
            OnEnd();
        }
        public virtual void Dispose() 
        { 
            runnerState = ERunnerState.Diposed; 
            Dispose();
        }
        protected override void OnPlay()
        {
            runnerState = ERunnerState.Running;
        }

        protected override void OnPause()
        {
            runnerState = ERunnerState.Paused;
        }

        protected virtual void OnEnd(){}
        protected virtual void OnDispose(){}

        protected override void OnClear()
        {
            error = string.Empty;
            updateContext = null;
        }

        public string error;
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
            return (float)updateContext.Value.totalTime - (float)Clip.start;
        }

        protected override void Internal_OnDone()
        {
            UnityEngine.Debug.Log("Sequence Done");
        }
    }

    public abstract class ClipRunner<TClip> : ClipRunner where TClip : Clip
    {
        public override Clip Clip => clip;
        protected TClip _clip;
        public TClip clip => _clip;
        public ClipRunner() => Reset();
        public ClipRunner(TClip clip) => Reset(clip);
        public ClipRunner Reset()
        {
            _clip = null;
            return this;
        }
        public ClipRunner Reset(TClip clip)
        {
            if (clip == null || clip.GetType() != ClipType)
            {
                AsFailure(clip == null ? ErrCode_ClipRunner_ClipIsNull : ErrCode_ClipRunner_ClipTypeNotMatched);
                return this;
            }
            _clip = clip;
            return this;
        }

        protected override void OnClear()
        {
            base.OnClear();
            _clip = null;
        }
    }
}
