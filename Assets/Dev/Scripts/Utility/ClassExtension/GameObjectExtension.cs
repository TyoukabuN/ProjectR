using System.Collections;
using System.Collections.Generic;
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
            T component = obj.GetComponent<T>();
            if (component == null)
                component = obj.AddComponent<T>();
            return component;
        }
        public static Component TryGetComponent(this Component obj, System.Type type)
        {
            Component component = obj.GetComponent(type);
            if (component == null)
                component = obj.AddComponent(type);
            return component;
        }

        public static Component TryGetComponent(this Component obj, string type)
        {
            Component component = obj.GetComponent(type);
            if (component == null)
                component = obj.AddComponent(System.Type.GetType(type));
            return component;
        }
        public static T TryGetComponent<T>(this GameObject obj) where T : Component
        {
            T component = obj.GetComponent<T>();
            if (component == null)
                component = obj.AddComponent<T>();
            return component;
        }
        public static Component TryGetComponent(this GameObject obj, System.Type type)
        {
            Component component = obj.GetComponent(type);
            if (component == null)
                component = obj.AddComponent(type);
            return component;
        }

        public static Component TryGetComponent(this GameObject obj, string type)
        {
            Component component = obj.GetComponent(type);
            if (component == null)
                component = obj.AddComponent(System.Type.GetType(type));
            return component;
        }

        public static T TryGetComponent<T>(this Transform obj) where T : Component
        {
            T component = obj.GetComponent<T>();
            if (component == null)
                component = obj.AddComponent<T>();
            return component;
        }
        public static Component TryGetComponent(this Transform obj, System.Type type)
        {
            Component component = obj.GetComponent(type);
            if (component == null)
                component = obj.AddComponent(type);
            return component;
        }

        public static Component TryGetComponent(this Transform obj, string type)
        {
            Component component = obj.GetComponent(type);
            if (component == null)
                component = obj.AddComponent(System.Type.GetType(type));
            return component;
        }
    }
}
