using System;
using PJR.Core;

namespace PJR.BlackBoard.CachedValueBoard
{
    /// <summary>
    /// 记录在GenericBuffer中CacheValue时返回的引用的信息
    /// 用于后面的取值
    /// </summary>
    public struct IndexMap
    {
        public static IndexMap Empty => new() { _length = 0 };
        public KeyIndexPair index0; 
        public KeyIndexPair index1; 
        public KeyIndexPair index2; 
        public KeyIndexPair index3; 
        public KeyIndexPair index4; 
        public KeyIndexPair index5; 
        public KeyIndexPair index6; 
        public KeyIndexPair index7;
             
        public struct KeyIndexPair
        {
            public string key;
            public ICacheableValue.IToBufferToken token;
            private int _index;
            private uint _guid;
            private Type _valueType;
            public uint GUID => token?.guid ?? _guid;
            public int Index=> token?.index ?? _index;
            public Type ValueType=> token?.ValueType ?? _valueType;
            public KeyIndexPair(string key, ICacheableValue.IToBufferToken token)
            {
                this.key = key;
                this.token = token;
                _guid = 0;
                _index = 0;
                _valueType = null;
            }
            public KeyIndexPair(string key,Type valueType,int index , uint guid)
            {
                this.key = key;
                this.token = null;
                _index = index;
                _guid = guid;
                _valueType = valueType;
            }
        }
             
        public int MaxLength => 8; 
        private bool _invalid;
        public bool Invalid => _length <= 0 || _invalid;
        private int _length;
        public int Length => _length;
             
        public KeyIndexPair this[int i]
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
        public bool Add(string key,Type valueType, int index, uint guid)
        {
            if (_length >= MaxLength)
                return false;

            this[_length++] = new(key,valueType, index, guid);
            return true;
        }

        public bool Add(string key, ICacheableValue.IToBufferToken token)
        {
            if (_length >= MaxLength)
                return false;

            this[_length++] = new(key, token);
            return true;
        }

        public void Release()
        {
            _invalid = true;
            for (int i = 0; i < Length; i++)
            {
                //有token时
                if(this[i].token != null)
                    this[i].token.Release();
                //无token时
                else
                    GenericBufferCenter.TryClearBuffer(this[i].ValueType, this[i].Index, this[i].GUID);
            }
        }
    }
}