using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PJR.Core.TypeExtension;
using UnityEngine.Pool;

namespace PJR.Core.Pooling
{
    /// <summary>
    /// 配合ListPool使用,减少些返回值为List<T>的方法的GC
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct PooledList<T> :IDisposable , IEnumerable<T>
    {
        public static PooledList<T> Invalid = new()
        {
            _valid = false,
            _list = null,
        };
        public static PooledList<T> Get()
        {
            var temp = new PooledList<T>();
            temp._valid = true;
            temp._list = ListPool<T>.Get();
            return temp;
        }
        public bool Valid => _valid && _list != null;
        public bool AnyItem => _valid && _list.Count > 0;
        public int Count => _list?.Count ?? 0;
        public List<T> List => _list;
        
        private bool _valid;
        private List<T> _list;
        public T First => AnyItem ? _list[0] : default;
        public T Last => AnyItem ? _list[^1] : default;

        public T this[int index]
        {
            get
            {
                if (!Valid)
                    return default;
                if (!_list.WithinRange(index))
                    return default;
                return _list[index];
            }
        }

        public PooledList(List<T> list)
        {
            _list = list;
            _valid = true;
        }
        public void Add(T item)
        {
            if (!Valid)
                return;
            _list.Add(item);
        }
        public void Remove(T item)
        {
            if (!Valid)
                return;
            _list.Remove(item);
        }
        public void RemoveAt(int index)
        {
            if (!Valid)
                return;
            _list.RemoveAt(index);
        }

        public void Dispose()
        {
            if(_list != null)
                ListPool<T>.Release(_list);
        }
        public void Release()
        {
            if(_list != null)
                ListPool<T>.Release(_list);
        }
        
        public override bool Equals(object other)
        {
            if (!this.Valid)
                return other == null;
            return other != null && this._list.Equals(other);
        }
        
        public static implicit operator List<T>(PooledList<T> pooledList)
            => pooledList._list;
        public static implicit operator PooledList<T>(List<T> pooledList)
            => new(pooledList);

        public List<T>.Enumerator GetEnumerator() => _list.GetEnumerator();

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => _list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();

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
