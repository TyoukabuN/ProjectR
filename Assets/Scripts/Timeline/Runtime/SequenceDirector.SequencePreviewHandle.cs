using PJR.Timeline.Pool;

namespace PJR.Timeline
{
    public partial class SequenceDirector
    {
        public class PreviewSequenceHandle : SequenceHandle, ISequencePlayableHandle
        {
            public new float time
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

            public bool IsPlaying()
            {
                if (!Valid || _director._runner == null)
                    return false;
                return _director._runner.IsRunning;
            }
            public void Play()
            {
                if (!Valid) 
                    return;
                _director.EnsureRunnerReady();
                _director.Play();
            }
            public void Pause()
            {
                if (!Valid) 
                    return;
                _director.EnsureRunnerReady();
                _director.Pause();
            }
            public void Stop()
            {
                if (!Valid)
                    return;
                _director.Stop();
            }
            /// <summary>
            /// 跳转到指定时间并强制刷新，不改变播放状态
            /// </summary>
            public void SeekTo(float seekTime)
            {
                if (!Valid)
                    return;
                _director.EnsureRunnerReady();
                time = seekTime;
                _director._runner.runnerState = ERunnerState.Running;
                _director.ManualUpdate(0, true);
            }
        }
    }
}