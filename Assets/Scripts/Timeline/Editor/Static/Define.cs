using UnityEngine;

namespace PJR.Timeline
{
    /// <summary>
    /// Sequence，Track，Clip有没有下到上的依赖信息<p/>
    /// 所以用这个结构来存一下
    /// </summary>
    public class SequenceUnitReference
    {
        protected Sequence _sequence;
        protected Track _track;
        protected Clip _clip ;
        public virtual Sequence Sequence => _sequence;
        public virtual Track Track => _track;
        public virtual Clip Clip  => _clip ;
        public SequenceUnitReference(Sequence sequence)
        {
            _sequence = sequence;
        }
    }

    public class TrackReference : SequenceUnitReference
    {
        public TrackReference(Sequence sequenceAsset, Track track):base(sequenceAsset)
        {
            _track = track;
        }
    }
    
    public class ClipReference : SequenceUnitReference
    {
        public ClipReference(Sequence sequenceAsset, Track track, Clip clip):base(sequenceAsset)
        {
            _track = track;
            _clip = clip;
        }
    }
}