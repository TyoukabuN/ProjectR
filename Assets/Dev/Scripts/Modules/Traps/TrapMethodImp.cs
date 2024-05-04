using PJR.ScriptStates.Player;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using static UnityEngine.EventSystems.EventTrigger;

namespace PJR
{
    /// <summary>
    /// 施力陷阱
    /// </summary>
    public class TrapMethod_AddForce : ActionApproach
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
    public class TrapMethod_Stumble : ActionApproach
    {
        public override TActionType ActionType => TActionType.Stumble;
        public override bool HasDirection => false;
        public override Vector3 Direction => Vector3.one;

        public override void ExecuteActionEvent(TActionEvent evt, LogicEntity trapEntity, LogicEntity targetEntity)
        {
            trapEntity.physEntity.Animancer_Play(EntityAnimationDefine.AnimationName.Action_1);
            targetEntity.EnterState((int)EPlayerState.Stumble);
            targetEntity.RemoveExtendValue(EntityDefine.ExtraValueKey.Dash);
        }
    }

    /// <summary>
    /// 加速
    /// </summary>
    [InlineProperty]
    public class TrapMethod_SpeedUp : ActionApproach
    {
        public override TActionType ActionType => TActionType.SpeedUp;
        public override bool HasDirection => true;
        public override Vector3 Direction => Vector3.one;

        [LabelText("速度")] public float speed = 10f;
        [LabelText("持续时间")] public float duration = 3f;
        [LabelText("转向系数")] public float orientationSharpness = 10f;

        public override void ExecuteActionEvent(TActionEvent evt, LogicEntity trapEntity, LogicEntity targetEntity)
        {
            targetEntity.AddExtraValue(EntityDefine.ExtraValueKey.Dash, this, duration);
            targetEntity.AddExtraValue(EntityDefine.ExtraValueKey.LastNonZeroInput, targetEntity.physEntity.motor.CharacterForward, duration);
        }
    }
    /// <summary>
    /// 滚石
    /// </summary>
    public class TrapMethod_RollingStone : ActionApproach
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
    [Serializable]
    public class TrapMethod_RollingStoneTrigger : ActionApproach
    {
        public override TActionType ActionType => TActionType.Stumble;
        public override bool HasDirection => false;
        public override Vector3 Direction => Vector3.one;
        [LabelText("对应场景ManualGroup的第几组")]
        [SerializeField]
        public string group = "1";
        //哎没办法储存
        //[LabelText("控制的陷阱hostGameObject")]
        //public List<GameObject> components = new List<GameObject>();
        public override void ExecuteActionEvent(TActionEvent evt, LogicEntity trapEntity, LogicEntity targetEntity)
        {
            if (trapEntity is TrapEntity)
            {
                Transform mTrans = SceneSystem.instance.SceneTrapRoot.Find("ManualGroup");
                if (mTrans == null){return;}
                Transform mNodeTrans = mTrans.Find(group);
                if (mNodeTrans != null)
                {
                    TrapConfigHost[] tcf = mNodeTrans.GetComponentsInChildren<TrapConfigHost>();
                    if (tcf!=null)
                    {
                        for (int i = 0; i < tcf.Length; i++)
                        {
                            EntitySystem.CreateTrapEntity(tcf[i]);
                        }
                    }
                        
                }
               
            }
            //TrapManualPool.instance.EntitiesGroup[group];
        }
    }
}