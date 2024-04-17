using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJR
{
    public static class TrapFunc
    {
        public static void ExecuteActionEvent(TActionEvent evt, LogicEntity trapEntity)
        {
            if (evt.actionType == TActionType.AddForce)
            {
                var param = evt.trapActionEventParam as TrapActionParamAsset_AddForce;
                if (param == null)
                    return;
                var controller = trapEntity.AddExtraVelocity(
                    trapEntity.entityContext.LogicEntityIDStr,
                    param.force,
                    param.duration,
                    param.damp);
                controller.easeType = param.easing;
            }
        }
    }
}
