using PJR.Timeline.Pool;

namespace PJR.Timeline
{
    public class PreviewSequenceHandle : SequenceDirector.SequenceHandle, ISequencePlayableHandle
    {
        public static PreviewSequenceHandle Get(SequenceDirector director)
        {
            var temp = ObjectPool<PreviewSequenceHandle>.Get();
            temp._director = director;
            return temp;
        }
        public override void Release() => ObjectPool<PreviewSequenceHandle>.Release(this);
    }
}