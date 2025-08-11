namespace PJR.Timeline
{
    /// <summary>
    /// Sequence，Track，Clip有没有下到上的依赖信息<p/>
    /// 所以用这个结构来存一下
    /// </summary>
    public class SequenceUnitReference
    {
        protected SequenceAsset SequenceAsset;
        protected Track _track;
        protected Clip _clip ;
        public virtual SequenceAsset sequenceAsset => SequenceAsset;
        public virtual Track Track => _track;
        public virtual Clip Clip  => _clip ;
        public SequenceUnitReference(SequenceAsset sequenceAsset)
        {
            SequenceAsset = sequenceAsset;
        }
    }

    public class TrackReference : SequenceUnitReference
    {
        public TrackReference(SequenceAsset sequenceAssetAsset, Track track):base(sequenceAssetAsset)
        {
            _track = track;
        }
    }
    
    public class ClipReference : SequenceUnitReference
    {
        public ClipReference(SequenceAsset sequenceAssetAsset, Track track, Clip clip):base(sequenceAssetAsset)
        {
            _track = track;
            _clip = clip;
        }
    }
}