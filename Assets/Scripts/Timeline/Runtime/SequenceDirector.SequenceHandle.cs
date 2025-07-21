using System;
using PJR.Timeline.Pool;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

namespace PJR.Timeline
{
    public partial class SequenceDirector
    {
        public class SequenceHandle : PoolableObject, ISequencePlayableHandle
        {
            public virtual float time
            {
                get
                {
                    if (!Valid || _director._runner == null)
                        return 0;
                    return _director._runner.TotalTime;
                }
                set
                {
                    //runtime就不改了
                }
            }
            public bool Valid => Director != null;
            public ISequence Sequence => Director?.SequenceAsset;
            public SequenceAsset SequenceAsset => (SequenceAsset)Director?.SequenceAsset;
            public SequenceDirector Director => _director;
            protected SequenceDirector _director;
            public UnityEngine.Object Object => _director?.gameObject;
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
            public virtual  double ToGlobalTime(double t) => t;
            public virtual double ToLocalTime(double t) => t;

            public virtual bool IsPlaying()
            {
                if (!Valid || _director._runner == null)
                    return false;
                return _director._runner.IsRunning;
            }
            public virtual void Play()
            {
            }
            public virtual void Pause()
            {
            }
            public virtual void Stop()
            {
            }
        }
    }
}