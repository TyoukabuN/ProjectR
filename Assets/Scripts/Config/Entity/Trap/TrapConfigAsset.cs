using System.Collections.Generic;
using PJR.Editor;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace PJR
{
    public class TrapConfigAsset : TriggerConfigAsset
    {
        [LabelText("可触发次数")]
        [Tooltip("< 0 即不限次数")]
        public int CanTriggerTimes = -1;

#if UNITY_EDITOR
        [MenuItem("Assets/PJR/创建配置/机关陷阱/机关陷阱配置")]
        public static void CreateAsset()
        {
            CSConfigHelper.CreateScriptableObject<TrapConfigAsset>();
        }
#endif
        /// <summary>
        /// 各种陷阱方法触发的地方
        /// </summary>
        /// <param name="events"></param>
        /// <param name="targetEntity">触发陷阱的目标</param>
        protected override void ExecuteActionEvents(List<EActionEvent> events, LogicEntity trapEntity, LogicEntity targetEntity)
        {
            if (CanTriggerTimes >= 0 && trapEntity.entityContext.TriggeredCount >= CanTriggerTimes)
                return;
            trapEntity.entityContext.AddTriggeredCount();

            base.ExecuteActionEvents(events, trapEntity, targetEntity);
        }
    }
}
