namespace PJR.Timeline
{
    public interface ISequenceHolder
    {
        public SequenceAsset SequenceAsset { get; }
        public SequenceRunner GetRunner();
        public ISequenceHandle GetHandle();
    }
}