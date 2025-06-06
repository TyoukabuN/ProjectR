namespace PJR.Timeline
{
    public interface ISequenceHandle
    {
        public SequenceAsset SequenceAsset { get; }
    }
    
    public class SequenceHandle : ISequenceHandle
    {
        private SequenceAsset _sequenceAsset;
        public SequenceAsset SequenceAsset => _sequenceAsset;
        public SequenceHandle(SequenceAsset sequenceAsset) => _sequenceAsset = sequenceAsset;

        public static implicit operator SequenceAsset(SequenceHandle handle)
        {
            return handle.SequenceAsset;
        }
    }
}