using PJR.Timeline.Pool;
using UnityEngine;

namespace PJR.Timeline
{
    public partial class SequenceDirector
    {
        /// <summary>
        /// 只读访问 Handle 的抽象基类，不实现播放控制
        /// 运行时播放控制请直接调用 SequenceDirector 的方法
        /// </summary>
        public abstract class SequenceHandle : PoolableObject, ISequenceHandle
        {
            public virtual float time
            {
                get
                {
                    if (!Valid) return 0;
                    return _director.Runner?.TotalTime ?? 0;
                }
            }
            public bool Valid => Director != null;
            public ISequence Sequence => Director?.SequenceAsset;
            public SequenceAsset SequenceAsset => (SequenceAsset)Director?.SequenceAsset;
            public SequenceDirector Director => _director;
            protected SequenceDirector _director;
            public Object Object => _director?.gameObject;
            public SequenceRunner Runner => _director?.GetRunner();
            public SequenceHandle() {}
            public SequenceHandle(SequenceDirector director) => _director = director;
            public override void Clear()
            {
                _director = null;
            }
            public virtual double ToGlobalTime(double t) => t;
            public virtual double ToLocalTime(double t) => t;
        }

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