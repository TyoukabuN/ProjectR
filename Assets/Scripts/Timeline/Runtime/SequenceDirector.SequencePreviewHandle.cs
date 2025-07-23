using PJR.Timeline.Pool;

namespace PJR.Timeline
{
    public partial class SequenceDirector
    {
        public class PreviewSequenceHandle : SequenceHandle
        {
            public override float time
            {
                get
                {
                    if (!Valid || _director._runner == null)
                        return 0;
                    return _director._runner.TotalTime;
                }
                set
                {
                    if (!Valid || _director._runner == null)
                        return;
                    Runner.TotalTime = value;   
                }
            }
            public static PreviewSequenceHandle Get(SequenceDirector director)
            {
                var temp = ObjectPool<PreviewSequenceHandle>.Get();
                temp._director = director;
                return temp;
            }
            public override void Clear()
            {
                _director = null;
            }
            public override void Release() => ObjectPool<PreviewSequenceHandle>.Release(this);
            public double ToGlobalTime(double t) => t;
            public double ToLocalTime(double t) => t;

            public override bool IsPlaying()
            {
                if (!Valid || _director._runner == null)
                    return false;
                return _director._runner.IsRunning;
            }
            public override void Play()
            {
                if (!Valid) 
                    return;
                if (_director._runner == null)
                    _director._runner = _director.GetRunner();
                
                if (_director._runner != null)
                {
                    if (_director._runner.runnerState == ERunnerState.None)
                    {
                        _director._runner.OnStart();
                    }
                    else if (_director._runner.runnerState == ERunnerState.Paused)
                    {
                        _director._runner.runnerState = ERunnerState.Running;
                    }
                    else if (_director._runner.runnerState == ERunnerState.Diposed)
                    {
                        _director._runner.Release();
                        _director._runner = null;
                    }
                }
            }
            public override void Pause()
            {
                if (!Valid || _director._runner == null) 
                    return;
                _director._runner.runnerState = ERunnerState.Paused;
            }
            public override void Stop()
            {
                if (!Valid)
                    return;
                _director.Stop();
            }
        }
    }
}