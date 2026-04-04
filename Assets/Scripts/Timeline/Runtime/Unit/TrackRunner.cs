using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Pool;
using static PJR.Timeline.Define;

namespace PJR.Timeline
{
    public class TrackRunner : UnitRunner<ClipRunner>, IErrorRecorder
    {
        public List<ClipRunner> clipRunners => _subRunners;
        private ITrack _track;
        Clip2ClipHandleFunc _clip2ClipHandle;

        public bool Invalid => runnerState >= ERunnerState.Diposed || AnyError;

        public TrackRunner()
        {
            Clear();
            runnerState = ERunnerState.None;
        }
        protected override void OnClear()
        {
            if (_subRunners != null)
            {
                CollectionPool<List<ClipRunner>, ClipRunner>.Release(_subRunners);
                _subRunners = null;
            }
            _track = null;
            _clip2ClipHandle = null;
        }

        public virtual bool Reset(ISequence sequence, ITrack track) => Reset(sequence, track, Global.Clip2ClipHandleFunc);
        public virtual bool Reset(ISequence sequence, ITrack track, Clip2ClipHandleFunc clip2ClipHandle)
        {
            Sequence = sequence;
            _track = track;
            _clip2ClipHandle = clip2ClipHandle;

            if (_track == null)
            {
                AsFailure(ErrCode_TrackRuner_TrackIsNull);
                return false;
            }
            if (_clip2ClipHandle == null)
            {
                AsFailure(ErrCode_TrackRuner_Clip2ClipHandle);
                return false;
            }
            if (_track.Clips == null)
            {
                AsFailure(ErrCode_TrackRuner_ClipsIsNull);
                return false;
            }
            if (_track.Clips.Count <= 0)
            {
                AsFailure(ErrCode_TrackRuner_NoneClip);
                return false;
            }

            _subRunners = CollectionPool<List<ClipRunner>, ClipRunner>.Get();
            for (int i = 0; i < _track.Clips.Count; i++)
            {
                var clip = _track.Clips[i];
                if (clip == null)
                    continue;
                ClipRunner clipRunner = null;

#if UNITY_EDITOR
                if(!EditorApplication.isPlaying)
                    clipRunner =clip.Editor_GetPreviewRunner();
                else
#endif
                    clipRunner =clip.GetRunner();
                
                if (clipRunner == null)
                    continue;
                _subRunners.Add(clipRunner);
            }

            ForeachSubRunner(InitClipRunner);

            runnerState = ERunnerState.None;
            return true;
        }
        public virtual void OnInit()
        {
            runnerState = ERunnerState.None;
        }
        public virtual void OnStart()
        {
            runnerState = ERunnerState.Running;
        }

        public override void OnUpdate(UpdateContext context)
        {
            if (runnerState >= ERunnerState.Done)
                return;
            if (_subRunners == null)
            {
                runnerState = ERunnerState.Done;
                return;
            }

            bool allDone = true;
            for (int i = 0; i < _subRunners.Count; i++)
            {
                var clipRunner = _subRunners[i];
                if (!IsClipRunnerUpdatable(clipRunner))
                    continue;

                clipRunner.SetUpdateContext(context);
                
                if (clipRunner.Clip.OutOfRange(context.currentTime, Sequence.FrameRateType.SPF()))
                {
                    if (clipRunner.Running)
                        clipRunner.End();
                }
                else
                { 
                    if (clipRunner.WaitingForStart)
                        clipRunner.OnStart(context);
                    if (clipRunner.Running)
                        clipRunner.OnUpdate(context);
                }

                if(clipRunner.runnerState < ERunnerState.Done)
                    allDone = false;
            }

            runnerState = allDone ? ERunnerState.Done : runnerState;
        }
        
        protected override void Internal_OnDone()
        {
        }

        protected override void OnPlay()
        {
            runnerState = ERunnerState.Running;
            ForeachSubRunner(sub => sub.Play());
        }

        protected override void OnPause()
        {
            runnerState = ERunnerState.Paused;
            ForeachSubRunner(sub => sub.Pause());
        }

        void InitClipRunner(ClipRunner clipHandle) => clipHandle?.OnInit();

        public bool IsClipRunnerUpdatable(ClipRunner clipHandle)
        {
            if (clipHandle == null)
                return false;
            if (clipHandle.runnerState >= ERunnerState.Done)
                return false;
            return true;
        }
        double GetSecondPerFrame() => Utility.GetSecondPerFrame(Sequence?.FrameRateType ?? EFrameRate.Game);

        #region IErrorRecorder Impl
        string IErrorRecorder.Error => base.Error;
        bool IErrorRecorder.AnyError => base.AnyError;
        #endregion

        #region Pool
        public static TrackRunner Get() => Pool.ObjectPool<TrackRunner>.Get();
        public override void Release() => Pool.ObjectPool<TrackRunner>.Release(this);
        #endregion
    }
}
