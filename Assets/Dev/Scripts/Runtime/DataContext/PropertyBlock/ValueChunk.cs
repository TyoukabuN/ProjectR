using System;
using UnityEngine;

namespace PJR.Dev.Game.DataContext
{
    public interface IValueChunk
    {
        public Type ValueType { get; }
        public int ValueCount { get; }
    }

    [Serializable]
    public struct ValueChunk1<T> : IValueChunk 
    {
        public static ValueChunk1<T> Empty => new();
        public Type ValueType => typeof(T);
        public int ValueCount => 1;

        [SerializeField] private T _value0;
        
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
    public struct ValueChunk2<T> : IValueChunk 
    {
        public static ValueChunk2<T> Empty => new();
        public Type ValueType => typeof(T);
        public int ValueCount => 2;
        [SerializeField] private T _value0;
        [SerializeField] private T _value1;
        
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
    public struct ValueChunk3<T> : IValueChunk 
    {
        public static ValueChunk3<T> Empty => new();
        public Type ValueType => typeof(T);
        public int ValueCount => 3;
        [SerializeField] private T _value0;
        [SerializeField] private T _value1;
        [SerializeField] private T _value2;
        
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
    public struct ValueChunk4<T> : IValueChunk 
    {
        public static ValueChunk4<T> Empty => new();
        public Type ValueType => typeof(T);
        public int ValueCount => 4;
        [SerializeField] private T _value0;
        [SerializeField] private T _value1;
        [SerializeField] private T _value2;
        [SerializeField] private T _value3;
        
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
    public struct ValueChunk5<T> : IValueChunk 
    {
        public static ValueChunk5<T> Empty => new();
        public Type ValueType => typeof(T);
        public int ValueCount => 5;
        [SerializeField] private T _value0;
        [SerializeField] private T _value1;
        [SerializeField] private T _value2;
        [SerializeField] private T _value3;
        [SerializeField] private T _value4;
        
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
    public struct ValueChunk6<T> : IValueChunk 
    {
        public static ValueChunk6<T> Empty => new();
        public Type ValueType => typeof(T);
        public int ValueCount => 6;
        [SerializeField] private T _value0;
        [SerializeField] private T _value1;
        [SerializeField] private T _value2;
        [SerializeField] private T _value3;
        [SerializeField] private T _value4;
        [SerializeField] private T _value5;
        
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
    public struct ValueChunk7<T> : IValueChunk 
    {
        public static ValueChunk7<T> Empty => new();
        public Type ValueType => typeof(T);
        public int ValueCount => 7;
        [SerializeField] private T _value0;
        [SerializeField] private T _value1;
        [SerializeField] private T _value2;
        [SerializeField] private T _value3;
        [SerializeField] private T _value4;
        [SerializeField] private T _value5;
        [SerializeField] private T _value6;
        
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
    public struct ValueChunk8<T> : IValueChunk 
    {
        public static ValueChunk8<T> Empty => new();
        public Type ValueType => typeof(T);
        public int ValueCount => 8;
        [SerializeField] private T _value0;
        [SerializeField] private T _value1;
        [SerializeField] private T _value2;
        [SerializeField] private T _value3;
        [SerializeField] private T _value4;
        [SerializeField] private T _value5;
        [SerializeField] private T _value6;
        [SerializeField] private T _value7;
        
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