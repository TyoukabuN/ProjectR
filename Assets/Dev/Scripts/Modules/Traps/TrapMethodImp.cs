using PJR;
using PJR.ScriptStates.Player;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace PJR
{
    /// <summary>
    /// 施力陷阱
    /// </summary>
    public class TrapMethod_AddForce : TrapMethod
    {
        public override TActionType ActionType => TActionType.AddForce;
        public override bool HasDirection => true;
        public override Vector3 Direction => force;

        [LabelText("力")] public Vector3 force = Vector3.zero;
        [LabelText("持续时间")] public float duration = 0.333f;
        [LabelText("衰减系数")][PropertyTooltip(ExtraVelocity.tooltip)] public float damp = -1f;
        [LabelText("衰减曲线")] public Easing.Ease easing = Easing.Ease.Linear;

        public override void ExecuteActionEvent(TActionEvent evt, LogicEntity trapEntity, LogicEntity targetEntity)
        {
            if (evt.trapMethod.ActionType != ActionType)
                return;
            
            Vector3 dir = trapEntity.transform.position - targetEntity.transform.position;
            dir = Vector3.ProjectOnPlane(dir, trapEntity.transform.up);
            dir.Normalize();
            Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);

            var controller = targetEntity.AddExtraVelocity(
                targetEntity.entityContext.LogicEntityIDStr,
                rot * force,
                duration,
                damp);
            controller.easeType = easing;
        }
    }

    public class TrapMethod_Stumble : TrapMethod
    {
        public override TActionType ActionType => TActionType.Stumble;
        public override bool HasDirection => false;
        public override Vector3 Direction => Vector3.one;

        public override void ExecuteActionEvent(TActionEvent evt, LogicEntity trapEntity, LogicEntity targetEntity)
        {
            trapEntity.physEntity.Animancer_Play(EntityAnimationDefine.AnimationName.Action_1);
            targetEntity.EnterState((int)EPlayerState.Stumble);
        }
    }
}