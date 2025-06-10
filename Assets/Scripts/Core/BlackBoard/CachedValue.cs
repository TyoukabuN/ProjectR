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
     
        private static uint s_guid = 0;
        public static uint NewGUID => ++s_guid;
        public static uint GetGUID => s_guid;

        public const int BufferLength = 1024;
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
     
        public bool FromBuffer(ICachedValue.IToBufferToken token, bool clearBuffer)
        {
            if (!token.Valid())
                return false;
            _value = buffer[token.index].Value;
            if(clearBuffer)
                ClearBuffer(token.index);
            return true;
        }

        public bool FromBuffer(int index, uint guid, bool clearBuffer)
        {
            _value = buffer[index].Value; 
            if(clearBuffer)
                ClearBuffer(index);
            return true;
        }

        public bool ToBuffer(out Type type, out int index, out uint guid)
        {
            type = typeof(T);
            index = GetEmptyBufferIndex();
            guid = 0;
            if (index < 0)
                return false;
            buffer[index] = new(_value){
                guid = NewGUID,
            };
            guid = GetGUID;
            return true;
        }
        public bool ToBuffer(out ICachedValue.IToBufferToken token)
        {
            if (!ToBuffer(out Type type,out var index, out var guid))
            {
                token = ToBufferToken.Invalid;
                return false;
            }
            //
            using(new ProfileScope("3"))
                token = new ToBufferToken(index, guid);
            return true;
        }
        
        public struct ToBufferToken : ICachedValue.IToBufferToken
        {
            public static ToBufferToken Invalid => new() { _invalid = true };

            private bool _invalid;
            private int _index;
            private uint _guid;
            public int index => _index;
            public Type ValueType => typeof(T); 
            public uint guid => _guid;
            
            public ToBufferToken(int index, uint guid)
            {
                _invalid = false;
                _index = index;
                _guid = guid;
            }

            public bool Valid()
            {
                if (_invalid)
                    return false; 
                if (_guid <= 0)
                    return false;
                if (_index <0 || _index >= buffer.Length)
                    return false;
                if (buffer[_index].IsEmpty)
                    return false;
                return true;
            }

            public void Release()
            {
                if (!Valid())
                    return;
                ClearBuffer(_index);
            }
        }
    }
}