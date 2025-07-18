using System;
using System.Collections.Generic;
using UnityEngine;
using static PJR.Timeline.Define;
using PJR.Timeline.Pool;
using UnityEditor;

namespace PJR.Timeline
{
    public class SequenceRunner :PoolableObject
    {
        public enum EState
        {
            None = 0,       
            Running,
            Paused,
            Done,
            Failure,
            Diposed,
        }
        EState _state;

        public EState State
        {
            get => _state;
            set
            {
                if (_state == value)
                    return;
                Internal_OnStateChanged(_state, value);
                _state = value;
                if(_state == EState.Done)
                    Internal_OnDone();
            }
        }
        public bool IsRunning => State == EState.Running;
        public bool IsPaused => State == EState.Paused;
        public bool IsDone => State == EState.Done;
        public bool IsDisposed => State == EState.Diposed;
        public List<TrackRunner> trackRunner => _trackRunners;
        public double FrameUpdateFrequency => _secondPerFrame;

        public float TotalTime
        {
            get => _totalTime; 
            set => _totalTime = value;
        }

        List<TrackRunner> _trackRunners;
        private SequenceAsset _sequenceAsset;

        float _totalTime = 0f;
        float _unscaleTotalTime = 0f;
        float _secondPerFrame = 0f;
        float _frameUpdateCounter = 0f;
        GameObject _gameObject;
        UpdateContext _updateContext;

        public SequenceRunner()
        { 
        }
        public SequenceRunner(GameObject gameObject, SequenceAsset sequenceAsset)
        {
            Reset(gameObject, sequenceAsset);
        }

        public void Reset(GameObject gameObject, SequenceAsset sequenceAsset)
        {
            _gameObject = gameObject;
            _sequenceAsset = sequenceAsset;

            if (_sequenceAsset == null)
            { 
                State = EState.Failure;
                return;
            }
            if (!_sequenceAsset.IsValid())
            { 
                State = EState.Done;
                return;
            }

            _trackRunners = UnityEngine.Pool.CollectionPool<List<TrackRunner>, TrackRunner>.Get();
            for (int i = 0; i < _sequenceAsset.Tracks.Count; i++)
            { 
                var track = _sequenceAsset.Tracks[i];
                if (!track.IsValid())
                    continue;
                var trackRunner = TrackRunner.Get();
                trackRunner.Reset(_sequenceAsset, track);
                if (trackRunner.Invalid)
                {
                    trackRunner.Release();
                    continue;
                }

                _trackRunners.Add(trackRunner);
            }

            _secondPerFrame = GetSecondPerFrame_Float();
            State = EState.None;
            
            _updateContext = new UpdateContext();
            _updateContext.totalTime = 0;
            _updateContext.frameChanged = true;
            _updateContext.gameObject = _gameObject;
        }
        public virtual void OnStart()
        {
            if (State >= EState.Done)
                return;
            Debug.Log("SequenceRunner.OnStart");
            State = EState.Running;

            ForEachTrackRunner(StartTrackRunner);
            OnUpdate(_updateContext);
        }

        public virtual void Pause()
        {
            if (IsPaused)
                return;
            
        }

        void OnUpdate(UpdateContext context, bool force = false)
        {
            if (State == EState.Running || force)
            {
                if (_trackRunners == null)
                {
                    State = EState.Done;
                    return;
                }

                bool allDone = true;
                for (int i = 0; i < _trackRunners.Count; i++)
                {
                    var trackRunner = _trackRunners[i];
                    trackRunner.OnUpdate(context);

                    if (trackRunner.State < TrackRunner.EState.Done)
                        allDone = false;
                }

                State = allDone ? EState.Done : _state;
            }
            else if (State == EState.Paused)
            {
            }
            else if (State == EState.Done)
            {
            }
        }

        public float _remainDeltaTime;
        public void OnUpdate(float deltaTime, bool force = false)
        {
            if (!IsRunning && !force)
                return;
            if (_trackRunners == null)
            {
                State = EState.Done;
                return;
            }

            float scaledDeltaTime = deltaTime * (float)GetTimeScale();
            _remainDeltaTime += scaledDeltaTime;
            TotalTime += scaledDeltaTime;

            //以deltaTime间隔更新
            var context = UpdateContext(scaledDeltaTime, deltaTime);
            context.updateIntervalType = IntervalType.Second;
            OnUpdate(context,force);
            if (!IsRunning && !force)
                return;
            
            int frameUpdateRound = 0;
            //以帧间隔更新
            while (_remainDeltaTime >= FrameUpdateFrequency)
            {
                _remainDeltaTime -= (float)FrameUpdateFrequency;
                context = UpdateContext(1);
                OnUpdate(context,force);
                if(!IsRunning)
                    break;
                frameUpdateRound++;
            }
        }
        UpdateContext UpdateContext(double scaledDeltaTime, double unscaledDeltaTime)
        {
            _updateContext.timeScale = GetTimeScale();
            _updateContext.totalTime = _totalTime;

            _updateContext.unscaledDeltaTime = unscaledDeltaTime;
            _updateContext.deltaTime = scaledDeltaTime;

            _updateContext.updateIntervalType = IntervalType.Second;

            return _updateContext;
        }

        UpdateContext UpdateContext(int frame)
        {
            if (frame > 0)
            {
                _updateContext.frameChanged = true;
                _updateContext.totalFrame += frame;
            }
            _updateContext.updateIntervalType = IntervalType.Frame;

            return _updateContext;
        }

        public Action<EState, EState> OnStateChanged;
        private void Internal_OnStateChanged(EState oldState, EState newState)
        {
            OnStateChanged?.Invoke(oldState, newState);
        }

        private void Internal_OnDone()
        {
            Clear();
        }

        public double GetTimeScale() => Time.timeScale;
        double GetSecondPerFrame() => Utility.GetSecondPerFrame(_sequenceAsset?.FrameRateType ?? EFrameRate.Game);
        float GetSecondPerFrame_Float() => (float)GetSecondPerFrame();

        void StartTrackRunner(TrackRunner clipHandle) => clipHandle?.OnStart();
        void ClearTrackRunner(TrackRunner clipHandle) => clipHandle?.Clear();

        void ForEachTrackRunner(Action<TrackRunner> func)
        {
            if (_trackRunners == null || func == null)
                return;
            for (int i = 0; i < _trackRunners.Count; i++)
            {
                var clipHandle = _trackRunners[i];
                if (clipHandle == null)
                    continue;
                func.Invoke(clipHandle);
            }
        }
        public override void Clear()
        {
            if (_state == EState.Diposed)
                return;
            ForEachTrackRunner(ClearTrackRunner);

            if (_trackRunners != null)
            {
                UnityEngine.Pool.CollectionPool<List<TrackRunner>, TrackRunner>.Release(_trackRunners);
                _trackRunners = null;
            }

            State = EState.Diposed;
            _sequenceAsset = null;

            _totalTime = 0f;
            _unscaleTotalTime = 0f;
            _secondPerFrame = 0;
            _frameUpdateCounter = 0f;
            _remainDeltaTime = 0f;

            _updateContext = default;
        }


        #region Pool
        public static SequenceRunner Get() => ObjectPool<SequenceRunner>.Get();
        public override void Release() => ObjectPool<SequenceRunner>.Release(this);
        #endregion
    }

}