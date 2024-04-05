using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJR
{
    public class MonoSingleton<T> : MonoSingleton where T : MonoSingleton
    {
        protected static T _instance = null;
        public static T inst => instance;
        public static T current => instance;
        public static T cur => instance;

        public static T instance
        {
            get
            {
                if (_instance == null)
                {
                    var gobj = new GameObject(typeof(T).Name);
                    DontDestroyOnLoad(gobj);
                    _instance = gobj.AddComponent<T>();
                    gobj.name = $"[{_instance.Name}]";
                    _instance.OnInstantiated();
                }
                return _instance;
            }
        }
    }

    public abstract class MonoSingleton : MonoBehaviour
    {
        public virtual string Name { get; }
        /// <summary>
        /// invoked before awake
        /// </summary>
        public virtual void OnInstantiated() { }
        public virtual void Init() { }
        public virtual void OnUpdate() { }
        public virtual void OnDispose() { }
    }
}
