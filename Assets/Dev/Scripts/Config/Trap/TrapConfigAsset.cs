using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace PJR
{
    public class TrapConfigAsset : EntityConfigAsset
    {
        [LabelText("陷阱类型")]
        public TrapType type;

        [LabelText("可触发次数")]
        [Tooltip("< 0 即不限次数")]
        public int CanTriggerTimes = -1;

        [ShowInInspector]
        [LabelText("实体周期事件列表")]
        public List<EntityPhaseEvent> entityPhaseEvents = new List<EntityPhaseEvent>();

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/PJR/创建配置/机关陷阱/机关陷阱配置")]
        public static void CreateAsset()
        {
            CSConfigHelper.CreateScriptableObject<TrapConfigAsset>();
        }
#endif
    }

}
