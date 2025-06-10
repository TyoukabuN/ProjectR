using System;
using System.Collections.Generic;

namespace PJR.BlackBoard.CachedValueBoard
{
    public struct IndexMap
    {
        public static IndexMap Empty => new() { _length = 0 };
        public Pair index0; 
        public Pair index1; 
        public Pair index2; 
        public Pair index3; 
        public Pair index4; 
        public Pair index5; 
        public Pair index6; 
        public Pair index7;
             
        public struct Pair
        {
            public string key;
            public ICachedValue.IToBufferToken token;
            public uint guid;
            public int index;
        }
             
        public int MaxLength => 8;
        private bool _invalid;
        public bool Invalid => _length <= 0 || _invalid;
        private int _length;
        public int Length => _length;
             
        public Pair this[int i]
        {
            get
            {
                return i switch
                {
                    0 => index0,
                    1 => index1,
                    2 => index2,
                    3 => index3,
                    4 => index4,
                    5 => index5,
                    6 => index6,
                    7 => index7,
                    _ => throw new IndexOutOfRangeException("BufferIndexes index out of range")
                };
            }
            private set
            {
                switch (i)
                {
                    case 0: index0 = value; break;
                    case 1: index1 = value; break;
                    case 2: index2 = value; break;
                    case 3: index3 = value; break;
                    case 4: index4 = value; break;
                    case 5: index5 = value; break;
                    case 6: index6 = value; break;
                    case 7: index7 = value; break;
                    default: throw new IndexOutOfRangeException("BufferIndexes index out of range");
                }
            }
        }
        public bool Add(string key, int index, uint guid)
        {
            if (_length >= MaxLength)
                return false;
            
            this[_length++] = new()
            {
                key = key,
                token = null,
                index = index,
                guid = guid,
            };
            return true;
        }

        public bool Add(string key, ICachedValue.IToBufferToken token)
        {
            if (_length >= MaxLength)
                return false;
            
            this[_length++] = new()
            {
                key = key,
                token = token
            };
            return true;
        }

        public void Release()
        {
            _invalid = true;
            for (int i = 0; i < Length; i++)
            {
                var index = this[i];
                if(index.token != null)
                    index.token.Release();
            }
        }
    }
}