using System;
using System.Collections.Generic;
using PJR.Timeline.Pool;
using UnityEngine;

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
            Define.UpdateContext _updateContext;
            
            protected GameObject _gameObject;
            protected SequenceAsset _sequence;

            public void Reset(GameObject gameObject, SequenceAsset sequenceAsset)
            {
                _gameObject = gameObject;
                _sequence = sequenceAsset;

                if (_sequence == null)
                {
                    State = EState.Failure;
                    return;
                }

                if (!_sequence.Valid)
                {
                    State = EState.Done;
                    return;
                }

                _trackRunners = UnityEngine.Pool.CollectionPool<List<TrackRunner>, TrackRunner>.Get();
                for (int i = 0; i < _sequence.Tracks.Count; i++)
                {
                    var track = _sequence.Tracks[i];
                    if (!track.IsValid())
                        continue;
                    var trackRunner = TrackRunner.Get();
                    trackRunner.Reset(_sequence, track);
                    if (trackRunner.Invalid)
                    {
                        trackRunner.Release();
                        continue;
                    }

                    _trackRunners.Add(trackRunner);
                }

                _secondPerFrame = GetSecondPerFrame_Float();
                State = EState.None;

                _updateContext = new Define.UpdateContext();
                _updateContext.totalTime = 0;
                _updateContext.frameChanged = true;
                _updateContext.gameObject = _gameObject;
            }

            public override void OnStart()
            {
                if (State >= EState.Done)
                    return;
                Debug.Log("SequenceRunner.OnStart");
                State = EState.Running;

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

            public override void OnUpdate(float deltaTime, bool force = false)
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

            Define.UpdateContext UpdateContext(double scaledDeltaTime, double unscaledDeltaTime)
            {
                _updateContext.timeScale = GetTimeScale();
                _updateContext.totalTime = _totalTime;

                _updateContext.unscaledDeltaTime = unscaledDeltaTime;
                _updateContext.deltaTime = scaledDeltaTime;

                _updateContext.updateIntervalType = Define.IntervalType.Second;

                return _updateContext;
            }

            Define.UpdateContext UpdateContext(int frame)
            {
                if (frame > 0)
                {
                    _updateContext.frameChanged = true;
                    _updateContext.totalFrame += frame;
                }

                _updateContext.updateIntervalType = Define.IntervalType.Frame;

                return _updateContext;
            }



            public double GetTimeScale() => Time.timeScale;
            double GetSecondPerFrame() => Utility.GetSecondPerFrame(_sequence?.FrameRateType ?? Define.EFrameRate.Game);
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
                
                if (_state == EState.Diposed)
                    return;
                ForEachTrackRunner(ClearTrackRunner);

                if (_trackRunners != null)
                {
                    UnityEngine.Pool.CollectionPool<List<TrackRunner>, TrackRunner>.Release(_trackRunners);
                    _trackRunners = null;
                }

                _sequence = null;
                _secondPerFrame = 0;
                _frameUpdateCounter = 0f;
                _remainDeltaTime = 0f;
                _updateContext = default;
            }


            #region Pool

            public static Runner Get() => ObjectPool<Runner>.Get();
            public override void Release() => ObjectPool<Runner>.Release(this);

            #endregion
        }
    }
}