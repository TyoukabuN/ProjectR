using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace PJR
{
    public class SystemHandler : MonoSingleton<SystemHandler>
    {
        public override string Name
        {
            get { return "Systems"; }
        }
        public static List<MonoSingleton> systems = new List<MonoSingleton>();
        public void RegisterSystem(MonoSingleton systemInstance)
        {
            systems ??= new List<MonoSingleton>();
            systems.Add(systemInstance);
            systemInstance.transform.SetParent(this.transform);
        }
        public override void Clear()
        {
            systems?.ForEach(instance => instance.Clear());
        }

        public override void Update()
        {
            systems?.ForEach(instance => instance.OnUpdate());
        }
    }
}
