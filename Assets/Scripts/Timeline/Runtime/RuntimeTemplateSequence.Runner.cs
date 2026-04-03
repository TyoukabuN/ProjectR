using PJR.Timeline.Pool;
using UnityEngine;

namespace PJR.Timeline
{
    public partial class RuntimeTemplateSequence
    {
        public class Runner : SequenceRunner
        {
            private TrackRunner _trackRunner;
            private RuntimeTemplateSequence _templateSequence;
            private SecondTimeDriver _secondDriver;
            protected override ISequence sequence => _templateSequence;

            protected override void OnClear()
            {
                base.OnClear();
                _trackRunner?.Release();
                _trackRunner = null;
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
                if (_trackRunner != null)
                    action?.Invoke(_trackRunner);
            }

            protected override void OnDriveUpdate(float deltaTime)
            {
                float scaledDeltaTime = deltaTime * (float)GetTimeScale();
                TotalTime += scaledDeltaTime;

                var shared = new TimeDriverContext
                {
                    timeScale = GetTimeScale(),
                    totalTime = TotalTime,
                    gameObject = _gameObject,
                };
                _secondDriver.Drive(deltaTime, shared, OnUpdateInternal);
            }
            
            void OnUpdateInternal(UpdateContext context)
            {
                if (IsDone)
                    return;
                
                if (_trackRunner == null)
                {
                    runnerState = ERunnerState.Done;
                    return;
                }

                _trackRunner.OnUpdate(context);
                if (_trackRunner.runnerState == ERunnerState.Done)
                    runnerState = ERunnerState.Done;
            }

            public void Reset(GameObject gameObject, RuntimeTemplateSequence templateSequence)
            {
                _gameObject = gameObject;
                _templateSequence = templateSequence;
                var trackRunner = TrackRunner.Get();
                trackRunner.Reset(templateSequence, templateSequence);
                if (trackRunner.Invalid)
                {
                    trackRunner.Release();
                    return;
                }
                
                _trackRunner = trackRunner;
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