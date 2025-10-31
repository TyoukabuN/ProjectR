using System;
using PJR.Core.Pooling;
using UnityEngine;

namespace PJR.Core
{


    public interface IBaiscObject :IReleasable
    {
        public int GetInstanceID();
    }
    
    /// <summary>
    /// 通用Runner，Runtime状态量持有者
    ///
    /// 约定:
    /// MethodA:用于外部调用。 OnMethodA:On前缀用于内部重写
    /// 
    /// 生命周期:
    /// OnReset => OnStart => OnUpdate => OnClear => OnRelease
    /// </summary>
    public abstract class BaiscObject : IBaiscObject
    {
        private const string ErrorText1 = "[BaiscObject] Attempt to release a null object!";

        /// <summary>
        /// 简单的内部状态
        /// 
        /// </summary>
        private enum EState
        {
            /// <summary>
            /// 刚Get出来时被赋予
            /// </summary>
            None,
            /// <summary>
            /// 在Start中被赋予
            /// </summary>
            Running,
            /// <summary>
            /// 在Release中被赋予
            /// </summary>
            Released
        }

        public virtual bool IsValid => true
        ;
        private bool IsWaitingToRun => _state == EState.None;
        private bool IsRunning => _state == EState.Running;
        private bool IsReleased => _state == EState.Released;
        
        
        private EState _state = EState.None;
        private float _lifeTime = 0;

        public void Reset()
        {
            _state = EState.None;
            _lifeTime = 0;
            OnReset();
        }
        protected abstract void OnReset();
        
        public void Start()
        {
            _state = EState.Running;
            OnStart();
        }
        protected abstract void OnStart();

        /// <summary>
        /// 用外部调用
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Update(float deltaTime)
        {
            OnUpdateTimeInfo(deltaTime);
            OnUpdate(deltaTime);
        }
        /// <summary>
        /// 用于内部重写
        /// </summary>
        /// <param name="deltaTime"></param>
        protected abstract void OnUpdate(float deltaTime);
        protected virtual void OnUpdateTimeInfo(float deltaTime)
        {
            if (!IsRunning)
                return;
            _lifeTime += deltaTime;
        }
        public void Clear()
        {
            _lifeTime = 0;
            OnClear();
        }
        
        protected abstract void OnClear();

        public virtual void Release()
        {
            if (IsReleased)
                return;
            _state = EState.Released;
            Clear();
        }

        public int GetInstanceID()
        {
            return 0;
        }

        protected abstract void OnRelease();

        /// <summary>
        /// 为内能直接在Runner内部访问
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public abstract class Pool<T> where T : BaiscObject, new()
        {

            static readonly ObjectPool<T> s_Pool = new ObjectPool<T>((Func<T>)(() => new T()));

            public static T Get()
            {
                var obj = s_Pool.Get();
                if (obj == null)
                    return null;
                return obj;
            }

            public static PooledObject<T> Get(out T value) => s_Pool.Get(out value);

            public static void Release(T toRelease)
            {
                if (toRelease == null)
                {
                    Debug.LogError(ErrorText1);
                    return;
                }
                toRelease.OnRelease();
                s_Pool.Release(toRelease);
            }
        }
    }

    /// <summary>
    /// 为了子类不用写GenericPool.Release(this) <br/>
    /// 用法：XXXRunner:GenericPool&lt;XXXRunner&gt;
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaiscObject<T> : BaiscObject where T : BaiscObject, new()
    {
        public override void Release()
        {
            base.Release();
            Pool<T>.Release(this as T);
        }

        public static T Get()
        {
            T obj = Pool<T>.Get();
            obj.Reset();
            return obj;
        }
    }
}