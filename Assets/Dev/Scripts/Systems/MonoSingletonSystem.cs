using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJR
{
    public abstract class MonoSingletonSystem<T> : MonoSingleton<T> where T : MonoSingleton
    {
        public override string Name { 
            get { return typeof(T).Name; }
        }

        public override void OnInstantiated()
        {
            base.OnInstantiated();
            LogSystem.Log($"[System][OnInstantiated] {Name}");
        }
    }
}
