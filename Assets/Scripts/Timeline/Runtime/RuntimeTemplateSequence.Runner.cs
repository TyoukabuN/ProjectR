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
            protected override ISequence sequence => _templateSequence;

            public override void Clear()
            {
                base.Clear();
                _trackRunner?.Release();;
                _trackRunner = null;
                _templateSequence = null;
            }

            public override void OnStart()
            {
                if (runnerState >= ERunnerState.Done)
                    return;
                Debug.Log("RuntimeTemplateSequence.Runner.OnStart");
                runnerState = ERunnerState.Running;

            }

            public float _remainDeltaTime;

            public override void OnUpdate(float deltaTime, bool force = false)
            {
                float scaledDeltaTime = deltaTime * (float)GetTimeScale();
                _remainDeltaTime += scaledDeltaTime;
                TotalTime += scaledDeltaTime;
                
                var context = UpdateContext(scaledDeltaTime, deltaTime);
                OnUpdate(context);
            }
            
            void OnUpdate(Define.UpdateContext context)
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
            }
            #region Pool

            public static Runner Get() => ObjectPool<Runner>.Get();
            public override void Release() => ObjectPool<Runner>.Release(this);

            #endregion
        }
    }
}