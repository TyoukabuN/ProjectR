using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            if (evt.trapMethod.ActionType == TActionType.SpeedUp)
            {
                var method = evt.trapMethod as TrapMethod_SpeedUp;
                if (method == null)
                    return;
                //不知哪改移速
            }
            #region 反射例
            var interfaceType = typeof(ITrapEvent);
            var assembly = Assembly.GetExecutingAssembly();

            var types = assembly.GetTypes()
                .Where(t => interfaceType.IsAssignableFrom(t) && t.IsClass)
                .ToList();

            var instances = types.Select(t => (ITrapEvent)Activator.CreateInstance(t)).ToList();

            foreach (var instance in instances)
            {
                instance.Recive(targetEntity);
            }
            #endregion
        }
    }
}
