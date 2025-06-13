using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace PJR.BlackBoard.CachedValueBoard
{
    [InlineProperty][HideReferenceObjectPicker]
    public class CacheableValue<T> : ICacheableValue
    {
        public static bool ExtractBuffer(int index, out BufferUnit<T> unit) => VariableBuffer<T>.instance.ExtractBuffer(index, out unit);
        public static void ClearBuffer(int index, uint guid) => VariableBuffer<T>.instance.ClearBuffer(index,guid);
        public Type ValueType => typeof(T);
        
        [VerticalGroup("值")][SerializeField][HideLabel]
        private T _localValue;
        public T localValue => _localValue;
        public object GetValue() => _localValue;

        public CacheableValue(){}
        public bool WriteFromBuffer(ICacheableValue.IToBufferToken token, bool clearBuffer)
        {
            if (!token.Valid())
                return false;
            if (!VariableBuffer<T>.instance.TryGetValue(token, out T bufferValue))
                return false;
            _localValue = bufferValue;
            if(clearBuffer)
                ClearBuffer(token.index, token.guid);
            return true;
        }

        public bool WriteFromBuffer(int index, uint guid, bool clearBuffer)
        {
            if (!VariableBuffer<T>.instance.TryGetValue(index, guid, out T bufferValue))
                return false;
            _localValue = bufferValue; 
            if(clearBuffer)
                ClearBuffer(index, guid);
            return true;
        }

        public bool CacheToBuffer(out Type type, out int index, out uint guid)
            => VariableBuffer<T>.instance.TryCacheValue(_localValue, out type, out index, out guid);

        public bool CacheToBuffer(out ICacheableValue.IToBufferToken token)
        {
            if (!CacheToBuffer(out Type type,out var index, out var guid))
            {
                token = ToBufferToken.Invalid;
                return false;
            }
            //exist GC.Alloc
            token = new ToBufferToken(index, guid);
            return true;
        }
        
        public struct ToBufferToken : ICacheableValue.IToBufferToken
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
                if (_index <0 || _index >= VariableBuffer<T>.instance.BufferLength)
                    return false;
                return true;
            }

            public void Release()
            {
                if (!Valid())
                    return;
                ClearBuffer(_index, _guid);
            }
        }
    }
}