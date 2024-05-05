using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PJR
{
    [Serializable]
    public abstract class TriggerConfigAsset : EntityConfigAsset
    {
        [PropertyOrder(100)]
        [ShowInInspector]
        [LabelText("实体周期事件列表")]
        public List<EntityPhaseEvent> entityPhaseEvents = new List<EntityPhaseEvent>();

        [NonSerialized]
        bool isCache = false;

        [NonSerialized]
        Dictionary<PhyEntityPhase, EntityPhaseEvent> phase2Event = null;

        private void Init()
        {
            if (!isCache)
            {
                phase2Event = new Dictionary<PhyEntityPhase, EntityPhaseEvent>();
                entityPhaseEvents.ForEach(evt => phase2Event[evt.entityPhase] = evt);
                isCache = true;
            }
        }
        public bool AnyPhaseEvent(PhyEntityPhase entityPhase)
        {
            Init();
            return phase2Event.ContainsKey(entityPhase);
        }
        public bool TryGetPhaseEvent(PhyEntityPhase entityPhase, out EntityPhaseEvent phaseEvent)
        {
            Init();
            return phase2Event.TryGetValue(entityPhase, out phaseEvent);
        }
        public bool TryExecutePhaseEvent(PhyEntityPhase entityPhase, LogicEntity trapEntity, LogicEntity targetEntity)
        {
            Init();
            if (!TryGetPhaseEvent(entityPhase, out var entityPhaseEvent))
                return false;

            ExecuteActionEvents(entityPhaseEvent.events, trapEntity, targetEntity);
            return true;
        }
        protected virtual void ExecuteActionEvents(List<EActionEvent> events, LogicEntity trapEntity, LogicEntity targetEntity)
        {
            for (int i = 0; i < events.Count; i++)
            {
                var evt = events[i];
                if (evt == null)
                {
                    LogSystem.LogError($"evt == null");
                    continue;
                }
                if (evt.actionApproach == null)
                {
                    LogSystem.LogError($"evt.trapMethod == null");
                    continue;
                }
                evt.actionApproach.ExecuteActionEvent(evt, trapEntity, targetEntity);
            }
        }
    }
}
