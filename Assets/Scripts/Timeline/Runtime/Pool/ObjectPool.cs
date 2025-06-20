using System;

namespace PJR.Timeline.Pool
{
    
    /// <summary>
    /// Timeline内部的ObjectPool，会在Release的时候调用IDisposable.Dispose
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class ObjectPool<T> where T : PoolableObject, IDisposable, new()
    {
        static UnityEngine.Pool.ObjectPool<T> _pool;
        static UnityEngine.Pool.ObjectPool<T> pool => _pool ??= new UnityEngine.Pool.ObjectPool<T>(() => new T(), null, obj=>obj.Dispose(), null, false);

        public static T Get()
        {
            var obj =pool.Get();
            obj.IsReleased = false;
            return obj;
        }
        
        public static void Release(T toRelease)
        {
            if (toRelease.IsReleased)
                return;
            toRelease.IsReleased = true;
            pool.Release(toRelease);
        }
    }
    public abstract class PoolableObject
    {
        public bool IsReleased = false;
        public abstract void Release();
    }
}
