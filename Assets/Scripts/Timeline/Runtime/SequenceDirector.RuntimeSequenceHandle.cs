using PJR.Timeline.Pool;

namespace PJR.Timeline
{
    public partial class SequenceDirector
    {
        /// <summary>
        /// 运行时 Handle，实现播放控制，委托给 Director
        /// </summary>
        public class RuntimeSequenceHandle : SequenceHandle, ISequencePlayableHandle
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
            public bool IsPlaying()
            {
                if (!Valid) return false;
                return _director.Runner?.IsRunning ?? false;
            }
            public void Play()
            {
                if (!Valid) return;
                _director.EnsureRunnerReady();
                _director.Play();
            }
            public void Pause()
            {
                if (!Valid) return;
                _director.EnsureRunnerReady();
                _director.Pause();
            }
            public void Stop()
            {
                if (!Valid) return;
                _director.Stop();
            }
            public void SeekTo(float seekTime)
            {
                if (!Valid) return;
                _director.SeekTo(seekTime);
            }
            public static RuntimeSequenceHandle Get(SequenceDirector director)
            {
                var temp = ObjectPool<RuntimeSequenceHandle>.Get();
                temp._director = director;
                return temp;
            }
            public override void Release() => ObjectPool<RuntimeSequenceHandle>.Release(this);
        }
    }
}