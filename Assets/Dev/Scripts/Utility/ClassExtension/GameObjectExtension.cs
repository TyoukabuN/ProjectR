using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

namespace PJR
{
    public static class GameObjectExtension
    {
        public static T AddComponent<T>(this Component obj) where T : Component
        {
            return obj.gameObject.AddComponent<T>();
        }
        public static Component AddComponent(this Component obj, System.Type type)
        {
            return obj.gameObject.AddComponent(type);
        }
        public static T TryGetComponent<T>(this Component obj) where T : Component
        {
            return obj.TryGetComponent(typeof(T)) as T; ;
        }
        public static Component TryGetComponent(this Component obj, string type)
        {
            return obj.TryGetComponent(System.Type.GetType(type));
        }
        public static Component TryGetComponent(this Component obj, System.Type type)
        {
            Component component = obj.GetComponent(type);
            if (component == null)
                component = obj.AddComponent(type);
            return component;
        }

        public static T TryGetComponent<T>(this GameObject obj) where T : Component
        {
            return obj.TryGetComponent(typeof(T)) as T;
        }
        public static Component TryGetComponent(this GameObject obj, string type)
        {
            return obj.TryGetComponent(System.Type.GetType(type));
        }
        public static Component TryGetComponent(this GameObject obj, System.Type type)
        {
            Component component = obj.GetComponent(type);
            if (component == null)
                component = obj.AddComponent(type);
            return component;
        }



        public static T TryGetComponent<T>(this Transform obj) where T : Component
        {
            return obj.TryGetComponent(typeof(T)) as T;
        }
        public static Component TryGetComponent(this Transform obj, string type)
        {
            return obj.TryGetComponent(System.Type.GetType(type));
        }
        public static Component TryGetComponent(this Transform obj, System.Type type)
        {
            Component component = obj.GetComponent(type);
            if (component == null)
                component = obj.AddComponent(type);
            return component;
        }
        public static Component CopyComponent<TComponent>(this GameObject destGobj,TComponent originComponent) where TComponent : Component
        {
            var type = originComponent.GetType();
            TComponent copy = destGobj.AddComponent<TComponent>();

            FieldInfo[] fields = type.GetFields();
            foreach (FieldInfo field in fields)
            {
                field.SetValue(copy, field.GetValue(originComponent));
            }
            return copy;
        }
    }
}
