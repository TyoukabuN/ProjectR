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
        public abstract class SequenceHandle : PoolableObject, ISequencePlayableHandle
        {
            public float time
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

            float ISequenceHandle.Time { get; set; }
            public bool Valid => Director != null;
            public ISequence Sequence => Director?.SequenceAsset;
            public SequenceAsset SequenceAsset => (SequenceAsset)Director?.SequenceAsset;
            public SequenceDirector Director => _director;
            public Object Object => _director?.gameObject;
            protected SequenceDirector _director;
            public SequenceHandle()
            {
            }
            public SequenceHandle(SequenceDirector director)
            {
                _director = director;
            }
            public virtual bool IsPlaying()
            {
                if (!Valid) return false;
                return _director.Runner?.IsRunning ?? false;
            }
            public virtual void Play()
            {
                if (!Valid) 
                    return;
                if(_director.Runner?.IsDone ?? false)
                    _director.SeekTo(0);
                _director.Play();
            }
            public virtual void Pause()
            {
                if (!Valid) return;
                _director.Pause();
            }
            public virtual void Stop()
            {
                if (!Valid) return;
                _director.Stop();
            }
            /// <summary>
            /// 跳转到指定时间并强制刷新，不改变播放状态
            /// </summary>
            public virtual void SeekTo(float seekTime)
            {
                if (!Valid) return;
                _director.SeekTo(seekTime);
            }
            public virtual void ManualUpdate(float deltaTime)
            {
                Director?.ManualUpdate(deltaTime);
            }
            public override void Clear()
            {
                _director = null;
                ClearInternal();
            }
            protected virtual void ClearInternal()
            {
            }
            public virtual double ToGlobalTime(double t) => t;
            public virtual double ToLocalTime(double t) => t;
        }
    }
}