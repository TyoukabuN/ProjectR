namespace PJR.BlackBoard.CachedValueBoard
{
    public struct BufferUnit<T>
    {
        public static BufferUnit<T> Empty => new() { _isEmpty = true };
     
        private bool _isEmpty;
        public uint guid;
        private T _value;
     
        public bool IsEmpty => _isEmpty;
        public bool Valid => !IsEmpty;
        public T Value => _value;
     
        public BufferUnit(T value)
        {
            guid = 0;
            _isEmpty = false;
            _value = value;
        }
    }
}