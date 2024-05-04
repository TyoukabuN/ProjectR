using PJR;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJR
{
    [Serializable]
    public abstract class ItemMethod
    {
        [ShowInInspector]
        [DisableIf("@true")]
        public abstract TActionType ActionType { get; }

        public abstract bool HasDirection { get; }
        public abstract Vector3 Direction { get; }

        public abstract void ExecuteActionEvent(TActionEvent evt, LogicEntity trapEntity, LogicEntity targetEntity);
    }
}
