using PJR.Timeline.Pool;

namespace PJR.Timeline
{
    /// <summary>
    /// 运行时 Handle，实现播放控制，委托给 Director
    /// </summary>
    public class RuntimeSequenceHandle : SequenceDirector.SequenceHandle, ISequencePlayableHandle
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
       
        public static RuntimeSequenceHandle Get(SequenceDirector director)
        {
            var temp = ObjectPool<RuntimeSequenceHandle>.Get();
            temp._director = director;
            return temp;
        }
        public override void Release() => ObjectPool<RuntimeSequenceHandle>.Release(this);
    }
}