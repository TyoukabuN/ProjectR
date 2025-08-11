using System.Collections;
using UnityEngine;

namespace PJR.Systems
{
    public abstract class MonoSingletonSystem<T> : MonoSingleton<T> where T : MonoSingleton
    {
        /// <summary>
        /// 实例位置偏移
        /// </summary>
        public virtual Vector3 Position { get; } = Vector3.zero;
        public override string Name { 
            get { return typeof(T).Name; }
        }
        /// <summary>
        /// 实例化系统实例后调用，在Awake之前
        /// </summary>
        public override void OnInstantiated()
        {
            base.OnInstantiated();

            gameObject.transform.position = Position;

            SystemHandler.instance.RegisterSystem(_instance);

            LogSystem.Log($"[System][OnInstantiated] {Name}");
        }

        /// <summary>
        /// 初始化系统,在<see cref="GameEntry.InitGame"/>调用
        /// </summary>
        public override IEnumerator Initialize()
        {
            LogSystem.Log($"[System][Init]{typeof(T).Name}");
            yield return null;
        }
    }
}
