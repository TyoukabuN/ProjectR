using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PJR.BlackBoard.CachedValueBoard
{
    [Serializable][InlineProperty][HideReferenceObjectPicker]
    public class CachedValue<T> : ICachedValue
    {
        #region Static
     
        static CachedValue()
        {
            _buffer = new BufferUnit<T>[BufferLength];
            Array.Fill(_buffer, BufferUnit<T>.Empty);
                 
            _freeBufferIndex = new Stack<int>(BufferLength);
        }
     
        public static uint _guid = 0;
     
        public static int BufferLength = 1024;
        private static BufferUnit<T>[] _buffer;
        private static Stack<int> _freeBufferIndex;
             
        public static Stack<int> freeBufferIndex => _freeBufferIndex;
        public static BufferUnit<T>[] buffer => _buffer;
     
        public static int GetEmptyBufferIndex()
        {
            int index = -1;
            //这里可以弄个生死池来管理可用的id，可能会比遍历快点
            for (var i = 0; i < buffer.Length; i++)
            {
                if (!buffer[i].IsEmpty)
                    continue;
                index = i;
                break;
            }
            return index;
        }
     
        public static bool ExtractBuffer(int index, out BufferUnit<T> unit)
        {
            unit = default;
            if (index >= buffer.Length)
                return false;
            unit = buffer[index];
            return !unit.IsEmpty;
        }
     
        public static void ClearBuffer(int index)
        {
            buffer[index] = BufferUnit<T>.Empty;
        }
     
        #endregion
     
        public CachedValue()
        {
        }
     
        public Type ValueType => typeof(T);
        
        [VerticalGroup("值")][SerializeField][HideLabel]
        private T _value;
        public T Value => _value;
     
        public bool FromBuffer(int index)
        {
            if (index >= buffer.Length)
                return false;
            if (buffer[index].IsEmpty)
                return false;
            _value = buffer[index].Value;
            return true;
        }
     
        public bool ToBuffer(out int index)
        {
            index = GetEmptyBufferIndex();
            if (index < 0)
                return false;
            buffer[index] = new(_value)
            {
                Stamp = ++_guid,
            };
            return true;
        }
    }
}