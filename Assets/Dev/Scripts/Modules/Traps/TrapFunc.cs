using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJR
{
    public static class TrapFunc
    {
        public static void ExecuteActionEvent(TActionEvent evt, LogicEntity trapEntity, LogicEntity targetEntity)
        {
            if (evt.trapMethod.ActionType == TActionType.AddForce)
            {
                var method = evt.trapMethod as TrapMethod_AddForce;
                if (method == null)
                    return;

                Vector3 dir = trapEntity.transform.position - targetEntity.transform.position;
                dir = Vector3.ProjectOnPlane(dir, trapEntity.transform.up);
                dir.Normalize();
                Quaternion rot = Quaternion.LookRotation(dir,Vector3.up);

                var controller = targetEntity.AddExtraVelocity(
                    targetEntity.entityContext.LogicEntityIDStr,
                    rot * method.force,
                    method.duration,
                    method.damp);
                controller.easeType = method.easing;
            }
        }
    }
}
