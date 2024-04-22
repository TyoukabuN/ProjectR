using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace PJR
{
    public abstract class MonoSingletonSystem<T> : MonoSingleton<T> where T : MonoSingleton
    {
        public override string Name { 
            get { return typeof(T).Name; }
        }
        /// <summary>
        /// 实例化系统实例后调用，在Awake之前
        /// </summary>
        public override void OnInstantiated()
        {
            base.OnInstantiated();

            SystemHandler.instance.RegisterSystem(_instance);

            LogSystem.Log($"[System][OnInstantiated] {Name}");
        }

        /// <summary>
        /// 初始化系统,在<see cref="GameEntry.InitGame"/>调用
        /// </summary>
        public override void Init()
        {
            base.Init();
            LogSystem.Log($"[System][Init]{typeof(T).Name}");
        }
    }
}
