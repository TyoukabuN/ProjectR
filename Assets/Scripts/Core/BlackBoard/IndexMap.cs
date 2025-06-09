using System;
using System.Collections.Generic;

namespace PJR.BlackBoard.CachedValueBoard
{
    public struct IndexMap
    {
        public static IndexMap Empty => new() { length = 0 };
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
            public int index;
        }
             
        public int MaxLength => 8;
        private int length;
        public int Length => length;
             
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
     
        public IEnumerable<Pair> Indexes()
        {
            if (length <= 0)
                yield break;
            int i = 0;
            if(i++ < Length) yield return index0;
            if(i++ < Length) yield return index1;
            if(i++ < Length) yield return index2;
            if(i++ < Length) yield return index3;
            if(i++ < Length) yield return index4;
            if(i++ < Length) yield return index5;
            if(i++ < Length) yield return index6;
            if(i++ < Length) yield return index7;
        }
     
        public bool Add(string key, int index)
        {
            if (length >= MaxLength)
                return false;
            this[length++] = new()
            {
                key = key,
                index = index
            };
            return true;
        }
    }
}