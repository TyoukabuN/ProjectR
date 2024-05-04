using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PJR
{
    [Serializable]
    public abstract class EntityConfigAsset : ScriptableObject
    {
        [ShowInInspector]
        [LabelText("实体周期事件列表")]
        public List<EntityPhaseEvent> entityPhaseEvents = new List<EntityPhaseEvent>();
    }
}
