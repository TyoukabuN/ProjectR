using System;

namespace LS.Game.DataContext
{
    public interface IValueTypeChunk
    {
        public Type ValueType { get; }
        public int ValueCount { get; }
    }

    [Serializable]
    public struct ValueTypeChunk1<T> : IValueTypeChunk where T : struct
    {
        public static ValueTypeChunk1<T> Empty => new();
        public Type ValueType => typeof(T);
        public int ValueCount => 1;

        private T _value0;
        
        public T Get(int index)
        {
            return index switch
            {
                0 => _value0,
                _ => throw new IndexOutOfRangeException()
            };
        }
        
        public void Set(int index, T value)
        {
            switch (index)
            {
                case 0: _value0 = value; break;
                default: throw new IndexOutOfRangeException();
            }
        }
    }
    [Serializable]
    public struct ValueTypeChunk2<T> : IValueTypeChunk where T : struct
    {
        public static ValueTypeChunk2<T> Empty => new();
        public Type ValueType => typeof(T);
        public int ValueCount => 2;
        private T _value0;
        private T _value1;
        
        public T Get(int index)
        {
            return index switch
            {
                0 => _value0,
                1 => _value1,
                _ => throw new IndexOutOfRangeException()
            };
        }
        
        public void Set(int index, T value)
        {
            switch (index)
            {
                case 0: _value0 = value; break;
                case 1: _value1 = value; break;
                default: throw new IndexOutOfRangeException();
            }
        }
    }
    [Serializable]
    public struct ValueTypeChunk3<T> : IValueTypeChunk where T : struct
    {
        public static ValueTypeChunk3<T> Empty => new();
        public Type ValueType => typeof(T);
        public int ValueCount => 3;
        private T _value0;
        private T _value1;
        private T _value2;
        
        public T Get(int index)
        {
            return index switch
            {
                0 => _value0,
                1 => _value1,
                2 => _value2,
                _ => throw new IndexOutOfRangeException()
            };
        }
        
        public void Set(int index, T value)
        {
            switch (index)
            {
                case 0: _value0 = value; break;
                case 1: _value1 = value; break;
                case 2: _value2 = value; break;
                default: throw new IndexOutOfRangeException();
            }
        }
    }
    [Serializable]
    public struct ValueTypeChunk4<T> : IValueTypeChunk where T : struct
    {
        public static ValueTypeChunk4<T> Empty => new();
        public Type ValueType => typeof(T);
        public int ValueCount => 4;
        private T _value0;
        private T _value1;
        private T _value2;
        private T _value3;
        
        public T Get(int index)
        {
            return index switch
            {
                0 => _value0,
                1 => _value1,
                2 => _value2,
                3 => _value3,
                _ => throw new IndexOutOfRangeException()
            };
        }
        
        public void Set(int index, T value)
        {
            switch (index)
            {
                case 0: _value0 = value; break;
                case 1: _value1 = value; break;
                case 2: _value2 = value; break;
                case 3: _value3 = value; break;
                default: throw new IndexOutOfRangeException();
            }
        }
    }
    [Serializable]
    public struct ValueTypeChunk5<T> : IValueTypeChunk where T : struct
    {
        public static ValueTypeChunk5<T> Empty => new();
        public Type ValueType => typeof(T);
        public int ValueCount => 5;
        private T _value0;
        private T _value1;
        private T _value2;
        private T _value3;
        private T _value4;
        
        public T Get(int index)
        {
            return index switch
            {
                0 => _value0,
                1 => _value1,
                2 => _value2,
                3 => _value3,
                4 => _value4,
                _ => throw new IndexOutOfRangeException()
            };
        }
        
        public void Set(int index, T value)
        {
            switch (index)
            {
                case 0: _value0 = value; break;
                case 1: _value1 = value; break;
                case 2: _value2 = value; break;
                case 3: _value3 = value; break;
                case 4: _value4 = value; break;
                default: throw new IndexOutOfRangeException();
            }
        }
    }
    [Serializable]
    public struct ValueTypeChunk6<T> : IValueTypeChunk where T : struct
    {
        public static ValueTypeChunk6<T> Empty => new();
        public Type ValueType => typeof(T);
        public int ValueCount => 6;
        private T _value0;
        private T _value1;
        private T _value2;
        private T _value3;
        private T _value4;
        private T _value5;
        
        public T Get(int index)
        {
            return index switch
            {
                0 => _value0,
                1 => _value1,
                2 => _value2,
                3 => _value3,
                4 => _value4,
                5 => _value5,
                _ => throw new IndexOutOfRangeException()
            };
        }
        
        public void Set(int index, T value)
        {
            switch (index)
            {
                case 0: _value0 = value; break;
                case 1: _value1 = value; break;
                case 2: _value2 = value; break;
                case 3: _value3 = value; break;
                case 4: _value4 = value; break;
                case 5: _value5 = value; break;
                default: throw new IndexOutOfRangeException();
            }
        }
    }
    [Serializable]
    public struct ValueTypeChunk7<T> : IValueTypeChunk where T : struct
    {
        public static ValueTypeChunk7<T> Empty => new();
        public Type ValueType => typeof(T);
        public int ValueCount => 7;
        private T _value0;
        private T _value1;
        private T _value2;
        private T _value3;
        private T _value4;
        private T _value5;
        private T _value6;
        
        public T Get(int index)
        {
            return index switch
            {
                0 => _value0,
                1 => _value1,
                2 => _value2,
                3 => _value3,
                4 => _value4,
                5 => _value5,
                6 => _value6,
                _ => throw new IndexOutOfRangeException()
            };
        }
        
        public void Set(int index, T value)
        {
            switch (index)
            {
                case 0: _value0 = value; break;
                case 1: _value1 = value; break;
                case 2: _value2 = value; break;
                case 3: _value3 = value; break;
                case 4: _value4 = value; break;
                case 5: _value5 = value; break;
                case 6: _value6 = value; break;
                default: throw new IndexOutOfRangeException();
            }
        }
    }
    [Serializable]
    public struct ValueTypeChunk8<T> : IValueTypeChunk where T : struct
    {
        public static ValueTypeChunk8<T> Empty => new();
        public Type ValueType => typeof(T);
        public int ValueCount => 8;
        private T _value0;
        private T _value1;
        private T _value2;
        private T _value3;
        private T _value4;
        private T _value5;
        private T _value6;
        private T _value7;
        
        public T Get(int index)
        {
            return index switch
            {
                0 => _value0,
                1 => _value1,
                2 => _value2,
                3 => _value3,
                4 => _value4,
                5 => _value5,
                6 => _value6,
                7 => _value7,
                _ => throw new IndexOutOfRangeException()
            };
        }
        public void Set(int index, T value, bool force = false)
        {
            switch (index)
            {
                case 0: _value0 = value; break;
                case 1: _value1 = value; break;
                case 2: _value2 = value; break;
                case 3: _value3 = value; break;
                case 4: _value4 = value; break;
                case 5: _value5 = value; break;
                case 6: _value6 = value; break;
                case 7: _value7 = value; break;
                default: throw new IndexOutOfRangeException();
            }
        }
    }
}