namespace PJR.Timeline
{
    public interface ISequenceHandle
    {
        public Sequence Sequence { get; }
    }
    
    public class SequenceHandle : ISequenceHandle
    {
        private Sequence _sequence;
        public Sequence Sequence => _sequence;
        public SequenceHandle(Sequence sequence) => _sequence = sequence;

        public static implicit operator Sequence(SequenceHandle handle)
        {
            return handle.Sequence;
        }
    }
}