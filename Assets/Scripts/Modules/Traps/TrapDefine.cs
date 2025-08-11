using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace PJR
{
    [Serializable]
    public enum EActionType : int
    {
        [LabelText("其他实体施力")] AddForce = 0,
        [LabelText("绊倒")] Stumble = 1,
        [LabelText("实体加速")] SpeedUp = 2,
        [LabelText("无敌星")] InvincibleStar = 3,
        //
        [LabelText("注册事件")] RegisteredEvent = 100,
        [LabelText("广播游戏事件")] DispatchEvent = 1011,
    }

    [Serializable]
    public enum PhyEntityPhase
    {
        None = 0,
        OnCollisionEnter = 1,
        OnCollisionExit = 2,
        OnTriggerEnter = 3,
        OnUpdateDistanceFromPlayer = 4,
        OnFixedUpdateBegin = 5,
        OnCallbackFromLua = 6,
        OnAnimationClipEvent = 7,
    }

    [Serializable]
    public class EActionEvent
    {
        [ShowInInspector]
        [SerializeReference]
        [LabelText("实现方法")]
        public ActionApproach actionApproach;
    }

    [Serializable]
    public abstract class ActionApproach
    {
        [ShowInInspector]
        [DisableIf("@true")]
        public abstract EActionType ActionType { get; }
        public abstract void ExecuteActionEvent(EActionEvent evt, LogicEntity trapEntity, LogicEntity targetEntity);
    }

    [Serializable]
    public abstract class TrapApproach : ActionApproach
    {
        public abstract bool HasDirection { get; }
        public abstract Vector3 Direction { get; }
    }
    [Serializable]
    public abstract class ItemApproach : ActionApproach
    {
    }

    public enum ValueChangeApproach
    {
        Tween,
        Immediately,
    }
    public enum AnimanerUpdateAproach
    {
        Auto,
        Manually,
    }

    [Serializable]
    public class EntityPhaseEvent
    {
        [LabelText("触发事件的Entity周期")]
        public PhyEntityPhase entityPhase = PhyEntityPhase.None;
        [LabelText("事件列表")]
        public List<EActionEvent> events = new List<EActionEvent>();
    }
}
