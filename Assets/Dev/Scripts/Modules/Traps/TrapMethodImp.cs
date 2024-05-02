using PJR.ScriptStates.Player;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

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

    /// <summary>
    /// 障碍（栏杆）
    /// </summary>
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

    /// <summary>
    /// 障碍（栏杆）
    /// </summary>
    public class TrapMethod_SpeedUp : TrapMethod
    {
        public override TActionType ActionType => TActionType.SpeedUp;
        public override bool HasDirection => true;
        public override Vector3 Direction => Vector3.one;

        [LabelText("速度")] public Vector3 speed = new Vector3(0,0,2f);
        [LabelText("持续时间")] public float duration = 3f;
        [LabelText("衰减系数")][PropertyTooltip(ExtraVelocity.tooltip)] public float damp = -1f;
        [InlineButton("ShowEasingHelpUrl", "曲线示例")]
        [LabelText("衰减曲线")] public Easing.Ease easing = Easing.Ease.Linear;
        [LabelText("强制移动阻断输入")] public bool blockInput = false;

        public override void ExecuteActionEvent(TActionEvent evt, LogicEntity trapEntity, LogicEntity targetEntity)
        {
            //不知哪改移速 A:加速的例子👇👇👇
            Vector3 dir = trapEntity.transform.position - targetEntity.transform.position;
            dir = Vector3.ProjectOnPlane(dir, trapEntity.transform.up);
            dir.Normalize();
            Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);

            var controller = targetEntity.AddExtraVelocity(
                targetEntity.entityContext.LogicEntityIDStr,
                rot * speed,
                duration,
                damp);
            controller.easeType = easing;

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
    /// <summary>
    /// 滚石
    /// </summary>
    public class TrapMethod_RollingStone : TrapMethod
    {
        public override TActionType ActionType => TActionType.Stumble;
        public override bool HasDirection => false;
        public override Vector3 Direction => Vector3.one;
        //需要物理效果
        public bool isPhysics = true;
        public override void ExecuteActionEvent(TActionEvent evt, LogicEntity trapEntity, LogicEntity targetEntity)
        {
            //trapEntity.physEntity.Animancer_Play(EntityAnimationDefine.AnimationName.Action_1);
            trapEntity.physEntity.avatar.transform.GetComponentInChildren<MeshRenderer>().material.color = Color.red;
            targetEntity.EnterState((int)EPlayerState.Stumble);
        }
    }
    /// <summary>
    /// 滚石触发
    /// </summary>
    public class TrapMethod_RollingStoneTrigger : TrapMethod
    {
        public override TActionType ActionType => TActionType.Stumble;
        public override bool HasDirection => false;
        public override Vector3 Direction => Vector3.one;
        [SerializeField]
        public List<GameObject> components = new List<GameObject>();
        public override void ExecuteActionEvent(TActionEvent evt, LogicEntity trapEntity, LogicEntity targetEntity)
        {
            if (trapEntity is TrapEntity)
            {
                if (components.Count >0)
                {
                    for (int i = 0; i < components.Count; i++)
                    {
                        TrapConfigHost tc = components[i].gameObject.TryGetComponent<TrapConfigHost>();
                        EntitySystem.CreateTrapEntity(tc);
                    }
                }
            }
            //TrapManualPool.instance.EntitiesGroup[group];
        }
    }
}