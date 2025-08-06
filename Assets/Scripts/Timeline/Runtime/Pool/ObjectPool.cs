namespace PJR.Timeline.Pool
{
    /// <summary>
    /// Timeline内部的ObjectPool，会在Release的时候调用IDisposable.Dispose
    /// 可以将Dispose当作Clear方法来实现,作用都是清理
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class ObjectPool<T> where T : PoolableObject, new()
    {
        static UnityEngine.Pool.ObjectPool<T> _pool;

        private static UnityEngine.Pool.ObjectPool<T> pool
            => _pool ??= new UnityEngine.Pool.ObjectPool<T>
            (
                () => new T(), 
                null,
                obj => obj.Clear(),
                null,
                false
            );

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
        public abstract void Clear();
        public abstract void Release();
        public static bool operator ==(PoolableObject lhs, object rhs)
        {
            if(ReferenceEquals(lhs, null) || lhs.IsReleased)
                return ReferenceEquals(rhs, null);
            return Equals(lhs, rhs);
        }
        public static bool operator !=(PoolableObject obj, object nullCheck) => !(obj == nullCheck);
    }
}
