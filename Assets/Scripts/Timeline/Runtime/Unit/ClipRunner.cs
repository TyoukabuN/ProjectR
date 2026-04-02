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
        public abstract void OnFrameUpdate(UpdateContext context);
        /// <summary>
        /// 以deltaTime间隔更新
        /// </summary>
        /// <param name="context"></param>
        public abstract void OnDeltaUpdate(UpdateContext context);
        public virtual void OnEnd() { runnerState = ERunnerState.Done; }
        public virtual void OnDispose() { runnerState = ERunnerState.Diposed; }

        public override void Clear()
        {
            error = string.Empty;
            updateContext = null;
            base.Clear();
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
        public ClipRunner(TClip clip) => Reset(clip);
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

        public override void Clear()
        {
            base.Clear();
            _clip = null;
        }
    }
}
