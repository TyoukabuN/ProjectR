namespace PJR.BlackBoard.CachedValueBoard
{
    public struct BufferUnit<T>
    {
        public static BufferUnit<T> Free => new() { _isFree = true };
     
        private bool _isFree;
        public uint guid;
        private T _value;
     
        public bool IsFree => _isFree;
        public bool Valid => !IsFree;
        public T Value => _value;
     
        public BufferUnit(T value)
        {
            guid = 0;
            _isFree = false;
            _value = value;
        }
    }
}