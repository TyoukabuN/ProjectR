using PJR.Timeline.Pool;
using UnityEngine;

namespace PJR.Timeline
{
    public partial class SequenceDirector
    {
        /// <summary>
        /// 运行时只读访问 Handle，不实现播放控制
        /// 运行时的 Play/Pause/Stop 请直接调用 SequenceDirector 的方法
        /// </summary>
        public class SequenceHandle : PoolableObject, ISequenceHandle
        {
            public virtual float time
            {
                get
                {
                    if (!Valid || _director._runner == null)
                        return 0;
                    return _director._runner.TotalTime;
                }
            }
            public bool Valid => Director != null;
            public ISequence Sequence => Director?.SequenceAsset;
            public SequenceAsset SequenceAsset => (SequenceAsset)Director?.SequenceAsset;
            public SequenceDirector Director => _director;
            protected SequenceDirector _director;
            public Object Object => _director?.gameObject;
            public SequenceRunner Runner => _director?.GetRunner();
            public SequenceHandle(){}
            public SequenceHandle(SequenceDirector director)=>_director = director;
            public static SequenceHandle Get(SequenceDirector director)
            {
                var temp = ObjectPool<SequenceHandle>.Get();
                temp._director = director;
                return temp;
            }
            public override void Clear()
            {
                _director = null;
            }
            public override void Release() => ObjectPool<SequenceHandle>.Release(this);
            public virtual double ToGlobalTime(double t) => t;
            public virtual double ToLocalTime(double t) => t;
        }
    }
}