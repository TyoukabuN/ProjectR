using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoSingleton where T : MonoSingleton
{
    private static T _instance = null;
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
                _instance.OnInit();
            }
            return _instance;
        }
    }
}

public abstract class MonoSingleton : MonoBehaviour
{ 
    public virtual string Name { get; }
    public virtual void OnInstantiated() { }
    public virtual void OnInit() { }
    public virtual void OnUpdate() { }
    public virtual void OnDispose() { }
}

