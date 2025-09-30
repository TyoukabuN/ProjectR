using System.Collections;
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
                    _instance.OnInstantiated();
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

    public abstract class MonoSingleton : MonoBehaviour
    {
        public virtual string Name { get; }
        /// <summary>
        /// invoked before awake
        /// </summary>
        public virtual void OnInstantiated() { }
        public abstract IEnumerator Initialize();
        public virtual void Clear() { }

        public virtual void OnUpdate(float deltaTime) { }
        //unity messages
        public virtual void LateUpdate() { }
        public virtual void OnDisable() { }
        public virtual void OnDestroy() { }
    }
}
