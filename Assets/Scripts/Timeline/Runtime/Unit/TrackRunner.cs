using System;
using System.Collections.Generic;
using UnityEngine;
using static PJR.Timeline.Define;
using PJR.Timeline.Pool;


namespace PJR.Timeline
{
    public class TrackRunner : PoolableObject, IDisposable, IErrorRecorder
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
            private set
            {
                if (_state == value)
                    return;
                Internal_OnStateChanged(_state, value);
                _state = value;
            }
        }
        
        public List<ClipRunner> clipRunners => _clipRunners;

        List<ClipRunner> _clipRunners;
        private Track _track;
        private Sequence _sequence;
        Clip2ClipHandleFunc _clip2ClipHandle;

        public double totalTime = 0f;
        double _timeCounter = 0f;
        public bool Invalid => _state >= EState.Diposed || !string.IsNullOrEmpty(Error);

        public TrackRunner() 
        {
            Dispose();
            State = EState.None;
        }
        public TrackRunner(Sequence sequence, Track track):this(sequence, track, Global.Clip2ClipHandleFunc) { }
        public TrackRunner(Sequence sequence, Track track, Clip2ClipHandleFunc clip2ClipHandle) 
        {
            Reset(sequence, track, clip2ClipHandle);
        }
        public virtual void Dispose()
        {
            ForEachClipRunner(DisposeClipRunner);

            if (_clipRunners != null)
            {
                UnityEngine.Pool.CollectionPool<List<ClipRunner>, ClipRunner>.Release(_clipRunners);
                _clipRunners = null;
            }
            _track = null;
            _sequence = null;
            _clip2ClipHandle = null;

            totalTime = 0f;
            _timeCounter = 0f;
            State = EState.Diposed;
        }

        public virtual bool Reset(Sequence sequence, Track track) => Reset(sequence, track, Global.Clip2ClipHandleFunc);
        public virtual bool Reset(Sequence sequence, Track track, Clip2ClipHandleFunc clip2ClipHandle)
        {
            _sequence = sequence;
            _track = track;
            _clip2ClipHandle = clip2ClipHandle;

            if (_track == null)
            {
                State = EState.Failure;
                _error = Define.ErrCode_TrackRuner_TrackIsNull;
                return false;
            }
            if (_clip2ClipHandle == null)
            {
                State = EState.Failure;
                _error = Define.ErrCode_TrackRuner_Clip2ClipHandle;
                return false;
            }

            _clipRunners = UnityEngine.Pool.CollectionPool<List<ClipRunner>, ClipRunner>.Get();
            for (int i = 0; i < _track.clips.Count; i++)
            {
                var clip = _track.clips[i];
                if (clip == null)
                    continue;
                var clipRunner = clip.GetRunner();
                if (clipRunner == null)
                    continue;
                _clipRunners.Add(clipRunner);
            }

            ForEachClipRunner(InitClipRunner);

            State = EState.None;
            return true;
        }
        public virtual void OnInit()
        {
            State = EState.None;
        }
        public virtual void OnStart()
        {
            State = EState.Running;
        }

        public virtual void OnUpdate(UpdateContext context)
        {
            if (_state >= EState.Done)
                return;
            if (clipRunners == null)
            {
                State = EState.Done;
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

                if(clipRunner.State < ClipRunner.EState.Done)
                    allDone = false;
            }

            State = allDone ? EState.Done : _state;
        }
        
        public Action<EState, EState> OnStateChanged;
        private void Internal_OnStateChanged(EState oldState, EState newState)
        {
            OnStateChanged?.Invoke(oldState, newState);
            if (newState == EState.Done)
                Internal_OnDone();
        }
        private void Internal_OnDone()
        {
            Debug.Log("Sequence Done");
        }

        void DisposeClipRunner(ClipRunner clipHandle) => clipHandle?.Dispose();
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
            if (clipHandle.State >= ClipRunner.EState.Done)
                return false;
            return true;
        }
        double GetSecondPerFrame() => Utility.GetSecondPerFrame(_sequence?.FrameRateType ?? EFrameRate.Game);


        #region IErrorRecorder Impl
        string _error;
        public string Error => _error;
        public bool AnyError => !string.IsNullOrEmpty(Error);
        #endregion

        #region Pool
        public static TrackRunner Get() => ObjectPool<TrackRunner>.Get();
        public override void Release() => ObjectPool<TrackRunner>.Release(this);
        #endregion
    }
}
