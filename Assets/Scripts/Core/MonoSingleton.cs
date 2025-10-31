using System.Collections;
using PJR.Core;
using UnityEngine;

namespace PJR
{
    public class MonoSingleton<T> : MonoSingleton where T : MonoSingleton
    {
        protected static T _instance = null;
        public static T inst => instance;
        public static T Instance => instance;
        public static T instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    { 
                        var gobj = new GameObject(typeof(T).Name);
                        if (Application.isPlaying)
                            DontDestroyOnLoad(gobj);
                        _instance = gobj.AddComponent<T>();
                        gobj.name = $"[{_instance.Name}]";
                    }
                    _instance.Instantiated();
                }
                return _instance;
            }
        }

        /// <summary>
        /// 系统的初始化操作都可以放这里
        /// 这里是提供异步等待操作
        /// </summary>
        /// <returns></returns>
        public override IEnumerator Initialize() { yield break; }

    }

    
    /// <summary>
    /// 没带On是外部条用接口，带On的内部实现接口
    /// </summary>
    public abstract class MonoSingleton : MonoBehaviour
    {
        public virtual string Name { get; }

        //for external invoke
        public void Instantiated() => OnInstantiated();
        public void Clear() => OnClear();
        public void Update() => Update(Time.deltaTime);
        public void Update(float deltaTime) => OnUpdate(deltaTime);
        public abstract IEnumerator Initialize();
        
        //for internal override
        protected virtual void OnInstantiated() { }
        protected virtual void OnClear() { }
        protected virtual void OnUpdate(float deltaTime) { }
        
        //unity messages
        public virtual void LateUpdate() { }
        public virtual void OnDisable() { }
        public virtual void OnDestroy() { }
    }


}
