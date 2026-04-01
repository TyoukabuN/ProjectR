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
                    if (!Valid) return 0;
                    return _director.Runner?.TotalTime ?? 0;
                }
                set
                {
                    if (!Valid || _director.Runner == null) return;
                    _director.Runner.TotalTime = value;
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
                if (!Valid) return false;
                return _director.Runner?.IsRunning ?? false;
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
                if (!Valid) return;
                _director.SeekTo(seekTime);
            }
        }
    }
}