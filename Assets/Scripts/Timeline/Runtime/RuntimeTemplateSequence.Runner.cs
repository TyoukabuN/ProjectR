using PJR.Timeline.Pool;
using System.Collections.Generic;
using UnityEngine;

namespace PJR.Timeline
{
    public partial class RuntimeTemplateSequence
    {
        public class Runner : SequenceRunner
        {
            private RuntimeTemplateSequence _templateSequence;
            private SecondTimeDriver _secondDriver;

            private TrackRunner TrackRunner => (_subRunners != null && _subRunners.Count > 0) ? _subRunners[0] : null;

            protected override void OnClear()
            {
                base.OnClear();
                if (_subRunners != null)
                {
                    UnityEngine.Pool.CollectionPool<List<TrackRunner>, TrackRunner>.Release(_subRunners);
                    _subRunners = null;
                }
                _templateSequence = null;
                _secondDriver = null;
            }

            public override void OnStart()
            {
                if (runnerState >= ERunnerState.Done)
                    return;
                Debug.Log("RuntimeTemplateSequence.Runner.OnStart");
                runnerState = ERunnerState.Running;
            }

            protected override void OnPlay()
            {
                runnerState = ERunnerState.Running;
            }

            protected override void OnPause()
            {
                runnerState = ERunnerState.Paused;
            }

            protected override void ForeachSubRunner(System.Action<UnitRunner> action)
            {
                var trackRunner = TrackRunner;
                if (trackRunner != null)
                    action?.Invoke(trackRunner);
            }

            protected override void OnDriveUpdate(float deltaTime)
            {
                float scaledDeltaTime = deltaTime * (float)GetTimeScale();
                CurrentTime += scaledDeltaTime;

                double duration = Sequence?.CalculateDuration() ?? 0;
                if (duration > 0 && CurrentTime >= duration)
                {
                    CurrentTime = (float)duration;
                    runnerState = ERunnerState.Done;
                    return;
                }

                var shared = new TimeDriverContext
                {
                    timeScale = GetTimeScale(),
                    currentTime = CurrentTime,
                    gameObject = _gameObject,
                };
                _secondDriver.Drive(deltaTime, shared, OnUpdateInternal);
            }
            
            void OnUpdateInternal(UpdateContext context)
            {
                if (IsDone)
                    return;

                var trackRunner = TrackRunner;
                if (trackRunner == null)
                {
                    runnerState = ERunnerState.Done;
                    return;
                }

                trackRunner.OnUpdate(context);
                if (trackRunner.runnerState == ERunnerState.Done)
                    runnerState = ERunnerState.Done;
            }

            public void Reset(GameObject gameObject, RuntimeTemplateSequence templateSequence)
            {
                _gameObject = gameObject;
                _templateSequence = templateSequence;
                Sequence = templateSequence;
                var trackRunner = Pool.ObjectPool<TrackRunner>.Get();
                trackRunner.Reset(templateSequence, templateSequence);
                if (trackRunner.Invalid)
                {
                    trackRunner.Release();
                    return;
                }

                _subRunners = UnityEngine.Pool.CollectionPool<List<TrackRunner>, TrackRunner>.Get();
                _subRunners.Add(trackRunner);
                runnerState = ERunnerState.None;
                _secondDriver = new SecondTimeDriver();
            }
            #region Pool

            public static Runner Get() => ObjectPool<Runner>.Get();
            public override void Release() => ObjectPool<Runner>.Release(this);

            #endregion
        }
    }
}