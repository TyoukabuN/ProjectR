using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace PJR
{
    [ExecuteAlways]
    public class SystemHandler : MonoSingleton<SystemHandler>
    {
        private const string _name = "Systems";
        public override string Name => _name;

        public static List<MonoSingleton> systemInstances = new List<MonoSingleton>();
        public static Dictionary<Type,MonoSingleton> type2systemInstance = new Dictionary<Type, MonoSingleton>();
        public void RegisterSystem(MonoSingleton systemInstance)
        {
            RemoveSystem(systemInstance.GetType());
            systemInstances ??= new List<MonoSingleton>();
            systemInstances.Add(systemInstance);
            systemInstance.transform.SetParent(this.transform);
        }
        public void RemoveSystem<SystemType>() where SystemType : MonoSingleton => RemoveSystem(typeof(SystemType));
        public void RemoveSystem(Type systemType)
        {
            if (!type2systemInstance.TryGetValue(systemType, out var instance))
                return;
            instance.Clear();
            type2systemInstance.Remove(systemType);
            systemInstances.Remove(instance);
            DestroyImmediate(instance);
        }
        public bool TryGetSystem<SystemType>(out MonoSingleton instance) where SystemType : MonoSingleton
        {
            return type2systemInstance.TryGetValue(typeof(SystemType),out instance);
        }
        public override void Clear()
        {
            systemInstances?.ForEach(instance => instance.Clear());
        }
        void Update() 
        {
            systemInstances?.ForEach(instance => instance.OnUpdate(Time.deltaTime));
        }
    }
}
