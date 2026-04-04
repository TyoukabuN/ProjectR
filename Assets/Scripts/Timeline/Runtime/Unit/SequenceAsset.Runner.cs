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
            public double FrameUpdateFrequency => _secondPerFrame;

            float _secondPerFrame = 0f;

            SecondTimeDriver _secondDriver;
            FrameTimeDriver _frameDriver;
            int _totalFrame;

            protected SequenceAsset _sequenceAsset;

            public void Reset(GameObject gameObject, SequenceAsset sequenceAsset)
            {
                _gameObject = gameObject;
                _sequenceAsset = sequenceAsset;
                Sequence = sequenceAsset;

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

                var tracks = ((ISequence)_sequenceAsset).Tracks;
                _subRunners = CollectionPool<List<TrackRunner>, TrackRunner>.Get();
                for (int i = 0; i < tracks.Count; i++)
                {
                    var track = tracks[i];
                    if (!track.IsValid())
                        continue;
                    var trackRunner = TrackRunner.Get();
                    trackRunner.Reset(_sequenceAsset, track);
                    if (trackRunner.Invalid)
                    {
                        trackRunner.Release();
                        continue;
                    }

                    _subRunners.Add(trackRunner);
                }

                _secondPerFrame = GetSecondPerFrame_Float();
                runnerState = ERunnerState.None;

                _totalFrame = 0;
                _secondDriver = new SecondTimeDriver();
                _frameDriver = new FrameTimeDriver(_secondPerFrame);
            }

            public override void OnStart()
            {
                if (runnerState >= ERunnerState.Done)
                    return;
                Debug.Log("SequenceRunner.OnStart");
                runnerState = ERunnerState.Running;

                ForEachTrackRunner(StartTrackRunner);
                var initContext = new UpdateContext
                {
                    timeScale = GetTimeScale(),
                    totalTime = 0,
                    totalFrame = 0,
                    frameChanged = true,
                    updateIntervalType = IntervalType.Second,
                    gameObject = _gameObject,
                };
                OnUpdateInternal(initContext);
            }
            
            protected override void OnDriveUpdate(float deltaTime)
            {
                if (!IsRunning)
                    return;
                if (_subRunners == null)
                {
                    runnerState = ERunnerState.Done;
                    return;
                }

                float scaledDeltaTime = deltaTime * (float)GetTimeScale();
                TotalTime += scaledDeltaTime;

                var shared = new TimeDriverContext
                {
                    timeScale = GetTimeScale(),
                    totalTime = TotalTime,
                    totalFrame = _totalFrame,
                    gameObject = _gameObject,
                };

                // 以 deltaTime 间隔更新
                _secondDriver.Drive(deltaTime, shared, OnUpdateInternal);

                if (!IsRunning)
                    return;

                // 以帧间隔更新
                _frameDriver.Drive(deltaTime, shared, OnFrameUpdateInternal);
            }
            
            void OnFrameUpdateInternal(UpdateContext context)
            {
                _totalFrame++;
                context.totalFrame = _totalFrame;
                OnUpdateInternal(context);
            }

            /// <summary>
            /// 更新一帧
            /// </summary>
            /// <param name="context"></param>
            protected void OnUpdateInternal(UpdateContext context)
            {
                if (runnerState != ERunnerState.Running)
                    return;

                if (_subRunners == null)
                {
                    runnerState = ERunnerState.Done;
                    return;
                }

                bool allDone = true;
                for (int i = 0; i < _subRunners.Count; i++)
                {
                    var trackRunner = _subRunners[i];
                    trackRunner.OnUpdate(context);

                    if (trackRunner.runnerState < ERunnerState.Done)
                        allDone = false;
                }

                runnerState = allDone ? ERunnerState.Done : runnerState;
            }

            
            void StartTrackRunner(TrackRunner trackHandle) => trackHandle?.OnStart();

            protected override void OnPlay()
            {
                runnerState = ERunnerState.Running;
            }

            protected override void OnPause()
            {
                runnerState = ERunnerState.Paused;
            }

            void ForEachTrackRunner(Action<TrackRunner> func) => ForeachSubRunner(func);

            protected override void OnClear()
            {
                base.OnClear();

                if (_subRunners != null)
                {
                    CollectionPool<List<TrackRunner>, TrackRunner>.Release(_subRunners);
                    _subRunners = null;
                }

                _sequenceAsset = null;
                _secondPerFrame = 0;
                _totalFrame = 0;
                _secondDriver = null;
                _frameDriver = null;
            }


            #region Pool

            public static Runner Get() => Pool.ObjectPool<Runner>.Get();
            public override void Release() => Pool.ObjectPool<Runner>.Release(this);

            #endregion
        }
    }
}