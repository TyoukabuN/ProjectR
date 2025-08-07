using System;
using System.Collections.Generic;

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
        public bool Valid => _valid;
        public List<T> List => _list;
        
        private bool _valid;
        private List<T> _list;

        public static PooledList<T> Get()
        {
            var temp = new PooledList<T>();
            temp._valid = true;
            temp._list = UnityEngine.Pool.ListPool<T>.Get();
            return temp;
        }

        public void Dispose()
        {
            if(_list != null)
                UnityEngine.Pool.ListPool<T>.Release(_list);
        }
    }
}
