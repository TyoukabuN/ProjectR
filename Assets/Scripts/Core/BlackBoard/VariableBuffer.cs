using System;
using System.Collections.Generic;
using System.Linq;
using PJR.BlackBoard.CachedValueBoard;
using UnityEngine;

namespace PJR.BlackBoard.CachedValueBoard
{
    public interface IVariableBuffer
    {
        public Type VariableType { get; }
        public bool ClearBuffer(int index, uint guid);
    }

    /// <summary>
    /// 一块缓存
    /// 通过使用缓存内的index来存取值,避免类型转换GC.Alloc
    /// 即避免Value:object => Value:Type的unboxing,
    /// 而是用index => A:Type,即通过过A内部的泛型和buffer中index,从buffer中直接获取值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class VariableBuffer<T> : IVariableBuffer
    {
        private static VariableBuffer<T> _inst;
        public static VariableBuffer<T> instance => CheckInstance();
        static VariableBuffer()
        {
            CheckInstance();
        }

        private static VariableBuffer<T> CheckInstance()
        {
            _inst ??= new VariableBuffer<T>();
            VariableBufferCenter.Register(_inst);
            return _inst;
        }

        private VariableBuffer()
        {
            _buffer = new BufferUnit<T>[BufferLength];
            Array.Fill(_buffer, BufferUnit<T>.Free);
                 
            _freeBufferIndex = new Stack<int>(BufferLength);
            for (int i = 0; i < BufferLength; i++)
                _freeBufferIndex.Push(i);
        }
        public Type VariableType => typeof(T);
        private uint s_guid = 0;
        public const int DefaultBufferLength = 128; 
        private readonly int _bufferLength = DefaultBufferLength;
        /// <summary>
        /// 就目前的即用即弃的需求而言,buffer的长度不需要很长
        /// 不知道后面有没有长期存储的需求,有的话可能要改成buffer长度可拓展
        /// </summary>
        public int BufferLength => _bufferLength;
        
        private BufferUnit<T>[] _buffer;
        private Stack<int> _freeBufferIndex;
        public uint NewGUID => ++s_guid;
        public uint GetGUID => s_guid;
        public Stack<int> freeBufferIndex => _freeBufferIndex;
        public BufferUnit<T>[] buffer => _buffer;
     
        public int GetEmptyBufferIndex()
        {
            if (_freeBufferIndex.Count > 0)
                return _freeBufferIndex.Pop();
            return -1;
        }
     
        public bool ExtractBuffer(int index, out BufferUnit<T> unit)
        {
            unit = default;
            if (index >= buffer.Length)
                return false;
            unit = buffer[index];
            return !unit.IsFree;
        }
        public bool TryGetValue(int index, uint guid, out T value)
        {
            BufferUnit<T> unit = default;
            value = default;
            if(index <  0 || index >= buffer.Length)
                return false;
            unit = buffer[index];
            if (unit.IsFree || unit.guid != guid)
                return false;
            value = unit.Value;
            return true;
        }
        public bool TryGetValue(ICachedValue.IToBufferToken token, out T value)
        {
            value = default;
            if (token.ValueType != VariableType)
                return false;
            return TryGetValue(token.index, token.guid, out value);
        }

        public bool TryCacheValue(T value ,out Type type, out int index,out uint guid)
        {
            type = typeof(T);
            index = GetEmptyBufferIndex();
            guid = 0;
            if (index < 0 || index >= buffer.Length)
                return false;
            buffer[index] = new(value){
                guid = NewGUID,
            };
            guid = GetGUID;
            return true;
        }
        public bool ClearBuffer(int index, uint guid)
        {
            if (index < 0 || index >= buffer.Length)
                throw new ArgumentOutOfRangeException($"{_logHead}错误缓存索引! [index:{index}] [guid:{guid}]");
            if (buffer[index].guid != guid)
            {
                Debug.LogWarning($"{_logHead}尝试释放的guid不匹配的缓存! [index:{index}] [guid:{guid}]");
                return false;
            }
            buffer[index] = BufferUnit<T>.Free;
            if (_freeBufferIndex.Contains(index))
            {
                Debug.LogWarning($"{_logHead}尝试释放已释放的缓存! [index:{index}] [guid:{guid}]");
                return false;
            }
            _freeBufferIndex.Push(index);
            return true;
        }

        private static string _logHead;
        public string LogHead {
            get {
                if (string.IsNullOrEmpty(_logHead))
                    _logHead = $"[VariableBuffer<{typeof(T)}>] ";
                return _logHead;
            }
        }
    }
}