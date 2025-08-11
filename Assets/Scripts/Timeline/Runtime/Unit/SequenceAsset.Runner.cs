using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace PJR.Timeline
{
    public partial class SequenceAsset
    {
        public SequenceRunner GetRunner(GameObject gameObject)
        {
            var temp = Runner.Get();
            temp.Reset(gameObject, this);
            return temp;
        }
        public class Runner : SequenceRunner
        {
            public List<TrackRunner> trackRunner => _trackRunners;
            public double FrameUpdateFrequency => _secondPerFrame;

            public float TotalTime
            {
                get => _totalTime;
                set => _totalTime = value;
            }

            List<TrackRunner> _trackRunners;

            float _totalTime = 0f;
            float _unscaleTotalTime = 0f;
            float _secondPerFrame = 0f;
            float _frameUpdateCounter = 0f;
            
            protected GameObject _gameObject;
            protected SequenceAsset _sequenceAsset;

            protected override ISequence sequence => _sequenceAsset;

            public void Reset(GameObject gameObject, SequenceAsset sequenceAsset)
            {
                _gameObject = gameObject;
                _sequenceAsset = sequenceAsset;

                if (_sequenceAsset == null)
                {
                    runnerState = ERunnerState.Failure;
                    return;
                }

                if (!_sequenceAsset.Valid)
                {
                    runnerState = ERunnerState.Done;
                    return;
                }

                _trackRunners = CollectionPool<List<TrackRunner>, TrackRunner>.Get();
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
                runnerState = ERunnerState.None;

                _updateContext = new Define.UpdateContext();
                _updateContext.totalTime = 0;
                _updateContext.frameChanged = true;
                _updateContext.gameObject = _gameObject;
            }

            public override void OnStart()
            {
                if (runnerState >= ERunnerState.Done)
                    return;
                Debug.Log("SequenceRunner.OnStart");
                runnerState = ERunnerState.Running;

                ForEachTrackRunner(StartTrackRunner);
                OnUpdate(_updateContext);
            }

            public override void Pause()
            {
                if (IsPaused)
                    return;
            }
            
            protected void OnUpdate(Define.UpdateContext context, bool force = false)
            {
                if (runnerState == ERunnerState.Running || force)
                {
                    if (_trackRunners == null)
                    {
                        runnerState = ERunnerState.Done;
                        return;
                    }

                    bool allDone = true;
                    for (int i = 0; i < _trackRunners.Count; i++)
                    {
                        var trackRunner = _trackRunners[i];
                        trackRunner.OnUpdate(context);

                        if (trackRunner.runnerState < ERunnerState.Done)
                            allDone = false;
                    }

                    runnerState = allDone ? ERunnerState.Done : RunnerState;
                }
                else if (runnerState == ERunnerState.Paused)
                {
                }
                else if (runnerState == ERunnerState.Done)
                {
                }
            }

            public float _remainDeltaTime;

            public override void OnUpdate(float deltaTime, bool force = false)
            {
                if (!IsRunning && !force)
                    return;
                if (_trackRunners == null)
                {
                    runnerState = ERunnerState.Done;
                    return;
                }

                float scaledDeltaTime = deltaTime * (float)GetTimeScale();
                _remainDeltaTime += scaledDeltaTime;
                TotalTime += scaledDeltaTime;

                //以deltaTime间隔更新
                var context = UpdateContext(scaledDeltaTime, deltaTime);
                context.updateIntervalType = Define.IntervalType.Second;
                OnUpdate(context, force);
                if (!IsRunning && !force)
                    return;

                int frameUpdateRound = 0;
                //以帧间隔更新
                while (_remainDeltaTime >= FrameUpdateFrequency)
                {
                    _remainDeltaTime -= (float)FrameUpdateFrequency;
                    context = UpdateContext(1);
                    OnUpdate(context, force);
                    if (!IsRunning)
                        break;
                    frameUpdateRound++;
                }
            }
            double GetSecondPerFrame() => Utility.GetSecondPerFrame(_sequenceAsset?.FrameRateType ?? Define.EFrameRate.Game);
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
                base.Clear();
                
                if (RunnerState == ERunnerState.Diposed)
                    return;
                ForEachTrackRunner(ClearTrackRunner);

                if (_trackRunners != null)
                {
                    CollectionPool<List<TrackRunner>, TrackRunner>.Release(_trackRunners);
                    _trackRunners = null;
                }

                _sequenceAsset = null;
                _secondPerFrame = 0;
                _frameUpdateCounter = 0f;
                _remainDeltaTime = 0f;
                _updateContext = default;
            }


            #region Pool

            public static Runner Get() => Pool.ObjectPool<Runner>.Get();
            public override void Release() => Pool.ObjectPool<Runner>.Release(this);

            #endregion
        }
    }
}