using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace PJR.Timeline.Pool
{
    public static class ObjectPool<T> where T : class, IDisposable, new()
    {
        static UnityEngine.Pool.ObjectPool<T> _pool;
        static UnityEngine.Pool.ObjectPool<T> pool => _pool ??= new UnityEngine.Pool.ObjectPool<T>(() => new T(), null, obj=>obj.Dispose(), null, false);
        public static T Get() => pool.Get();
        public static PooledObject<T> Get(out T value) => pool.Get(out value);
        public static void Release(T toRelease) => pool.Release(toRelease);
    }
}
