using System;
using System.Collections.Generic;
using UnityEngine;
using static PJR.Timeline.Define;
using PJR.Timeline.Pool;

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


        public double elapseTime = 0f;
        public double unscaleElapseTime = 0f;
        double _secondPerFrame = 0;
        double _timeCounter = 0f;

        UpdateContext _updateContext;


        public SequenceRunner(Sequence sequence)
        {
            Init(sequence);
        }

        public void Init(Sequence sequence)
        {
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

            OnUpdate(0);
        }

        public void OnUpdate(float deltaTime)
        {
            if (_state != EState.Running)
                return;
            if (_trackRunners == null)
            {
                _state = EState.Done;
                return;
            }

            unscaleElapseTime += deltaTime;


            var scaledDeltaTime = GetTimeScale() * deltaTime;
            elapseTime += scaledDeltaTime;


            var context = UpdateContext(scaledDeltaTime, unscaleElapseTime);

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



        public void Dispose()
        {
            if (_trackRunners != null)
            {
                UnityEngine.Pool.CollectionPool<List<TrackRunner>, TrackRunner>.Release(_trackRunners);
                _trackRunners = null;
            }

            _sequence = null;
            _state = EState.Diposed;
        }

        UpdateContext UpdateContext(double unscaledDeltaTime) => UpdateContext(unscaledDeltaTime * GetTimeScale(), unscaledDeltaTime);
        UpdateContext UpdateContext(double scaledDeltaTime, double unscaledDeltaTime)
        {
            _updateContext.frameCount = Time.frameCount;
            _updateContext.timeScale = GetTimeScale();
            _updateContext.elapseTime = elapseTime;

            _updateContext.unscaledDeltaTime = unscaledDeltaTime;
            _updateContext.deltaTime = scaledDeltaTime;


            _updateContext.frameChanged = false;
            _timeCounter += scaledDeltaTime;
            if (_timeCounter < FrameUpdateFrequency)
            {
                _timeCounter -= FrameUpdateFrequency;
                _updateContext.frameCount++;
                _updateContext.frameChanged = true;
            }

            return _updateContext;
        }

        double GetTimeScale() => Time.timeScale;
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
    }

}