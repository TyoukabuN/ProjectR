using System;
using System.Collections.Generic;
using System.Text;
using NPOI.Util;

namespace PJR.Core.Pooling
{
    /// <summary>
    /// 配合ListPool使用,减少些返回值为List<T>的方法的GC
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct PooledList<T> :IDisposable
    {
        public static PooledList<T> Invalid = new()
        {
            _valid = false,
            _list = null,
        };
        public bool Valid => _valid && _list != null;
        public List<T> List => _list;
        
        private bool _valid;
        private List<T> _list;
        public T First => _valid ? _list[0] : default;
        public T Last => _valid ? _list[^1] : default;

        public static PooledList<T> Get()
        {
            var temp = new PooledList<T>();
            temp._valid = true;
            temp._list = UnityEngine.Pool.ListPool<T>.Get();
            return temp;
        }
        public static implicit operator List<T>(PooledList<T> pooledList)
            => pooledList._list;
        public void Dispose()
        {
            if(_list != null)
                UnityEngine.Pool.ListPool<T>.Release(_list);
        }
    }

    /// <summary>
    /// 相当于一个static的StringBuilder
    /// </summary>
    public struct PooledStringBuilder : IDisposable
    {
        public static PooledStringBuilder Invalid = new() { _valid = false };
        public bool Valid => _valid && _stringBuilder != null;
        public StringBuilder StringBuilder => _stringBuilder;
        
        private bool _valid;
        private StringBuilder _stringBuilder;
        
        public static PooledStringBuilder Get()
        {
            var temp = new PooledStringBuilder();
            temp._valid = true;
            temp._stringBuilder = new StringBuilder();
            return temp;
        }

        public void Dispose()
        {
            if(!Valid)
                return;
            _stringBuilder.Length = 0;
        }
    }
}
