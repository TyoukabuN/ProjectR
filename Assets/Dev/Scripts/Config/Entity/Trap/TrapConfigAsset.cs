using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PJR
{
    public class TrapConfigAsset : EntityConfigAsset
    {
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

        [NonSerialized]
        bool isCache = false;

        [NonSerialized]
        Dictionary<TEntityPhase, EntityPhaseEvent> phase2Event = null;

        private void Init() {
            if (!isCache)
            {
                phase2Event = new Dictionary<TEntityPhase, EntityPhaseEvent>();
                entityPhaseEvents.ForEach(evt => phase2Event[evt.entityPhase] = evt);
                isCache = true;
            }
        }
        public bool AnyPhaseEvent(TEntityPhase entityPhase)
        {
            Init();
            return phase2Event.ContainsKey(entityPhase);
        }
        public bool TryGetPhaseEvent(TEntityPhase entityPhase, out EntityPhaseEvent phaseEvent)
        {
            Init();
            return phase2Event.TryGetValue(entityPhase, out phaseEvent);
        }
        public bool TryExecutePhaseEvent(TEntityPhase entityPhase, LogicEntity targetEntity)
        {
            Init();
            if (!TryGetPhaseEvent(entityPhase, out var entityPhaseEvent))
                return false;
            
            ExecuteActionEvents(entityPhaseEvent.events, targetEntity);
            return true;
        }

        /// <summary>
        /// 各种陷阱方法触发的地方
        /// </summary>
        /// <param name="events"></param>
        /// <param name="targetEntity">陷阱的目标</param>
        private void ExecuteActionEvents(List<TActionEvent> events, LogicEntity targetEntity)
        {
            for (int i = 0; i < events.Count; i++)
            {
                var evt = events[i];
                if (evt == null)
                    continue;
                TrapFunc.ExecuteActionEvent(evt, targetEntity);
                if (evt.actionType == TActionType.AddForce)
                {
                    var param = evt.trapActionEventParam as TrapActionParamAsset_AddForce;
                    if (param == null)
                        continue;
                    var controller = targetEntity.AddExtraVelocity(
                        targetEntity.entityContext.LogicEntityIDStr,
                        param.force,
                        param.duration,
                        param.damp);
                    controller.easeType = param.easing;
                }
            }
        }
    }
}
