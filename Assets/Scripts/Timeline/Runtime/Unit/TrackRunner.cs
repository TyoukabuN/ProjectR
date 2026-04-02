using System;
using System.Collections.Generic;
using PJR.Timeline.Pool;
using UnityEditor;
using UnityEngine.Pool;
using static PJR.Timeline.Define;

namespace PJR.Timeline
{
    public class TrackRunner : BaseRunner, IErrorRecorder
    {
        public List<ClipRunner> clipRunners => _clipRunners;

        List<ClipRunner> _clipRunners;
        private ITrack _track;
        private ISequence _sequence;
        Clip2ClipHandleFunc _clip2ClipHandle;

        public double totalTime = 0f;
        double _timeCounter = 0f;
        public bool Invalid => runnerState >= ERunnerState.Diposed || AnyError;

        public TrackRunner()
        {
            Clear();
            runnerState = ERunnerState.None;
        }
        public override void Clear()
        {
            ForEachClipRunner(ClearClipRunner);

            if (_clipRunners != null)
            {
                CollectionPool<List<ClipRunner>, ClipRunner>.Release(_clipRunners);
                _clipRunners = null;
            }
            _track = null;
            _sequence = null;
            _clip2ClipHandle = null;

            totalTime = 0f;
            _timeCounter = 0f;
            base.Clear();
        }

        public virtual bool Reset(ISequence sequence, ITrack track) => Reset(sequence, track, Global.Clip2ClipHandleFunc);
        public virtual bool Reset(ISequence sequence, ITrack track, Clip2ClipHandleFunc clip2ClipHandle)
        {
            _sequence = sequence;
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

            _clipRunners = CollectionPool<List<ClipRunner>, ClipRunner>.Get();
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
                _clipRunners.Add(clipRunner);
            }

            ForEachClipRunner(InitClipRunner);

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

        public virtual void OnUpdate(UpdateContext context)
        {
            if (runnerState >= ERunnerState.Done)
                return;
            if (clipRunners == null)
            {
                runnerState = ERunnerState.Done;
                return;
            }

            bool allDone = true;
            for (int i = 0; i < clipRunners.Count; i++)
            {
                var clipRunner = clipRunners[i];
                if (!IsClipRunnerUpdatable(clipRunner))
                    continue;

                clipRunner.SetUpdateContext(context);
                
                if (clipRunner.Clip.OutOfRange(context.totalTime, _sequence.FrameRateType.SPF()))
                {
                    if (clipRunner.Running)
                        clipRunner.OnEnd();
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
        
        protected override void Internal_OnStateChanged(ERunnerState oldRunnerState, ERunnerState newRunnerState)
        {
            base.Internal_OnStateChanged(oldRunnerState, newRunnerState);
        }
        protected override void Internal_OnDone()
        {
        }

        void ClearClipRunner(ClipRunner clipHandle) => clipHandle?.Clear();
        void InitClipRunner(ClipRunner clipHandle) => clipHandle?.OnInit();
        void ForEachClipRunner(Action<ClipRunner> func)
        {
            if (clipRunners == null || func == null)
                return;
            for (int i = 0; i < clipRunners.Count; i++)
            {
                var clipHandle = clipRunners[i];
                if(clipHandle == null)
                    continue;
                func.Invoke(clipHandle);
            }
        }

        public bool IsClipRunnerUpdatable(ClipRunner clipHandle)
        {
            if (clipHandle == null)
                return false;
            if (clipHandle.runnerState >= ERunnerState.Done)
                return false;
            return true;
        }
        double GetSecondPerFrame() => Utility.GetSecondPerFrame(_sequence?.FrameRateType ?? EFrameRate.Game);

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
