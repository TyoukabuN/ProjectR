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
        /// ʵ����ϵͳʵ������ã���Awake֮ǰ
        /// </summary>
        public override void OnInstantiated()
        {
            base.OnInstantiated();
            LogSystem.Log($"[System][OnInstantiated] {Name}");
        }

        /// <summary>
        /// ��ʼ��ϵͳ,��<see cref="GameEntry.InitGame"/>����
        /// </summary>
        public override void Init()
        {
            base.Init();
            LogSystem.Log($"[System][Init]{typeof(T).Name}");
        }
    }
}
 