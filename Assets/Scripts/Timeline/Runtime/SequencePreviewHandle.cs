using PJR.Timeline.Pool;

namespace PJR.Timeline
{
    public class PreviewSequenceHandle : SequenceDirector.SequenceHandle, ISequencePlayableHandle
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
    }
}