using System;
using System.Collections.Generic;
using UnityEngine;
using static PJR.Timeline.Define;
using PJR.Timeline.Pool;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using System.Security.Cryptography;

namespace PJR.Timeline
{
    public class SequenceRunner : IDisposable
    {
        public enum EState
        {
            None = 0,
            Running,
            Done,
            Failure,
            Diposed,
        }
        EState _state;
        public EState state => _state;
        public List<TrackRunner> trackRunner => _trackRunners;
        public double FrameUpdateFrequency => _secondPerFrame;


        List<TrackRunner> _trackRunners;
        private Sequence _sequence;

        double _totalTime = 0f;
        double _unscaleTotalTime = 0f;
        double _secondPerFrame = 0f;
        double _frameUpdateCounter = 0f;
        GameObject _gameObject;

        UpdateContext _updateContext;

        public SequenceRunner()
        { 
        }
        public SequenceRunner(GameObject gameObject, Sequence sequence)
        {
            Init(gameObject, sequence);
        }

        public void Init(GameObject gameObject, Sequence sequence)
        {
            _gameObject = gameObject;
            _sequence = sequence;

            if (_sequence == null)
            { 
                _state = EState.Failure;
                return;
            }
            if (!_sequence.IsValid())
            { 
                _state = EState.Done;
                return;
            }

            _trackRunners = UnityEngine.Pool.CollectionPool<List<TrackRunner>, TrackRunner>.Get();
            for (int i = 0; i < _sequence.tracks.Length; i++)
            { 
                var track = _sequence.tracks[i];
                if (!track.IsValid())
                    continue;
                var trackRunner = TrackRunner.Get();
                trackRunner.Init(_sequence, track);
                if (trackRunner.Invalid)
                {
                    trackRunner.Pool();
                    continue;
                }

                _trackRunners.Add(trackRunner);
            }

            _secondPerFrame = GetSecondPerFrame();
            _state = EState.None;
        }
        public virtual void OnStart()
        {
            if (_state >= EState.Done)
                return;
            _state = EState.Running;

            ForEachTrackRunner(StartTrackRunner);

            _updateContext = new UpdateContext();
            _updateContext.totalTime = 0;
            _updateContext.frameChanged = true;
            _updateContext.gameObject = _gameObject;
            OnUpdate(_updateContext);
        }

        void OnUpdate(UpdateContext context)
        {
            if (_state != EState.Running)
                return;
            if (_trackRunners == null)
            {
                _state = EState.Done;
                return;
            }

            bool allDone = true;
            for (int i = 0; i < _trackRunners.Count; i++)
            {
                var trackRunner = _trackRunners[i];
                trackRunner.OnUpdate(context);

                if (trackRunner.state == TrackRunner.EState.Running)
                    allDone = false;
            }

            _state = allDone ? EState.Done : _state;
        }

        public float _remainDeltaTime;
        public void OnUpdate(float deltaTime)
        {
            if (_state != EState.Running)
                return;
            if (_trackRunners == null)
            {
                _state = EState.Done;
                return;
            }

            float scaledDeltaTime = deltaTime * (float)GetTimeScale();
            _remainDeltaTime += scaledDeltaTime;

            //以deltaTime间隔更新
            var context = UpdateContext(scaledDeltaTime, deltaTime);
            context.updateIntervalType = IntervalType.Second;
            OnUpdate(context);

            int frameUpdateRound = 0;
            //以帧间隔更新
            while (_remainDeltaTime >= FrameUpdateFrequency)
            {
                _remainDeltaTime -= (float)FrameUpdateFrequency;
                context = UpdateContext(1);
                OnUpdate(context);
                frameUpdateRound++;
            }
        }
        UpdateContext UpdateContext(double scaledDeltaTime, double unscaledDeltaTime)
        {
            _updateContext.timeScale = GetTimeScale();
            _updateContext.totalTime = _totalTime += scaledDeltaTime;

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

        public double GetTimeScale() => Time.timeScale;
        double GetSecondPerFrame() => Utility.GetSecondPerFrame(_sequence?.frameRateType ?? EFrameRate.Game);

        void StartTrackRunner(TrackRunner clipHandle) => clipHandle?.OnStart();
        void DisposeTrackRunner(TrackRunner clipHandle) => clipHandle?.Dispose();

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
        public void Dispose()
        {
            ForEachTrackRunner(DisposeTrackRunner);

            if (_trackRunners != null)
            {
                UnityEngine.Pool.CollectionPool<List<TrackRunner>, TrackRunner>.Release(_trackRunners);
                _trackRunners = null;
            }

            _state = EState.Diposed;
            _sequence = null;

            _totalTime = 0f;
            _unscaleTotalTime = 0f;
            _secondPerFrame = 0;
            _frameUpdateCounter = 0f;
            _remainDeltaTime = 0f;

            _updateContext = default;
        }


        #region Pool
        public static SequenceRunner Get() => ObjectPool<SequenceRunner>.Get();
        public void Pool() => ObjectPool<SequenceRunner>.Release(this);
        #endregion
    }

}