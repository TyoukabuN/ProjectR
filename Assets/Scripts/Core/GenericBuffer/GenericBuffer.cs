using System;
using System.Collections.Generic;
using PJR.Core.BlackBoard.CachedValueBoard;
using UnityEngine;

namespace PJR.Core
{
    /// <summary>
    /// 一块缓存
    /// 通过使用缓存内的index来存取值,避免类型转换GC.Alloc
    /// 即避免Value:object => Value:Type的unboxing,
    /// 而是用index => A:Type,即通过过A内部的泛型和buffer中index,从buffer中直接获取值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericBuffer<T> : IVariableBuffer
    {
        private static GenericBuffer<T> _inst;
        public static GenericBuffer<T> instance => CheckInstance();
        static GenericBuffer()
        {
            CheckInstance();
        }

        private static GenericBuffer<T> CheckInstance()
        {
            _inst ??= new GenericBuffer<T>();
            GenericBufferCenter.Register(_inst);
            return _inst;
        }

        private GenericBuffer()
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
        private readonly object _bufferLock = new object(); // buffer锁对象
        private readonly object _indexLock = new object();  // index栈锁对象

        public uint NewGUID => ++s_guid;
        public uint GetGUID => s_guid;
        
        public Stack<int> freeBufferIndex
        {
            get
            {
                lock (_indexLock)
                    return new Stack<int>(_freeBufferIndex); // 返回副本以保证线程安全
            }
        }

        public BufferUnit<T>[] buffer
        {
            get
            {
                lock (_bufferLock)
                    return (BufferUnit<T>[])_buffer.Clone(); // 返回克隆数组避免外部修改
            }
        }
     
        public int GetEmptyBufferIndex()
        {
            lock (_indexLock)
            {
                if (_freeBufferIndex.Count > 0)
                    return _freeBufferIndex.Pop();
                return -1;
            }
        }
     
        public bool ExtractBuffer(int index, out BufferUnit<T> unit)
        {
            lock (_bufferLock)
            {
                unit = default;
                if(index < 0 || index >= _buffer.Length)
                    return false;
                
                unit = _buffer[index];
                return !unit.IsFree;
            }
        }

        public bool TryGetValue(int index, uint guid, out T value)
        {
            BufferUnit<T> unit = default;
            value = default;
            if(index <  0 || index >= buffer.Length)
                return false;
            
            lock (_bufferLock)
            {
                unit = _buffer[index];
            }

            if (unit.IsFree || unit.guid != guid)
                return false;
            
            value = unit.Value;
            return true;
        }

        public bool TryGetValue(ICacheableValue.IToBufferToken token, out T value)
        {
            value = default;
            if (token.ValueType != VariableType)
                return false;
            return TryGetValue(token.index, token.guid, out value);
        }

        public bool TryCacheValue(T value ,out Type type, out int index,out uint guid)
        {
            type = typeof(T);
            index = -1;
            guid = 0;

            lock (_indexLock)
            {
                if (_freeBufferIndex.Count > 0)
                    index = _freeBufferIndex.Pop();
                else
                    return false;
            }

            if (index < 0 || index >= buffer.Length)
                return false;

            lock (_bufferLock)
            {
                _buffer[index] = new(value)
                {
                    guid = NewGUID,
                };
            }

            guid = GetGUID;
            return true;
        }

        public bool ClearBuffer(int index, uint guid)
        {
            if (index < 0 || index >= buffer.Length)
                throw new ArgumentOutOfRangeException($"{_logHead}错误缓存索引! [index:{index}] [guid:{guid}]");
            
            lock (_bufferLock)
            {
                if (_buffer[index].guid != guid)
                {
                    Debug.LogWarning($"{_logHead}尝试释放的guid不匹配的缓存! [index:{index}] [guid:{guid}]");
                    return false;
                }

                _buffer[index] = BufferUnit<T>.Free;
            }

            lock (_indexLock)
            {
                if (_freeBufferIndex.Contains(index))
                {
                    Debug.LogWarning($"{_logHead}尝试释放已释放的缓存! [index:{index}] [guid:{guid}]");
                    return false;
                }
                _freeBufferIndex.Push(index);
            }

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