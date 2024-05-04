using UnityEngine;
using System;
using UnityEngine.Events;
using System.Collections.Generic;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PJR
{
    public enum TEventType : int
    {
        None = 0,
        Talk = 1,
        GetHurt = 2,
        GetBonus = 3,
        GetHostage = 4,
        CheckGameCanEnd = 5,
    }


    [Serializable]
    public enum TActionType : int
    {
        [LabelText("其他实体施力")] AddForce = 0,
        [LabelText("绊倒")] Stumble = 1,
        [LabelText("实体加速")] SpeedUp = 2,
        [LabelText("无敌")] Invincible = 3,
        [LabelText("广播游戏事件")] DispatchEvent = 24,
    }

    [Serializable]
    public enum TEntityPhase
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
    public class TActionEvent
    {
        [ShowInInspector]
        [SerializeReference]
        [LabelText("实现方法")]
        public ActionApproach trapMethod;
    }

    [Serializable]
    public abstract class ActionApproach
    {
        [ShowInInspector]
        [DisableIf("@true")]
        public abstract TActionType ActionType { get; }

        public abstract bool HasDirection { get; }
        public abstract Vector3 Direction { get; }

        public abstract void ExecuteActionEvent(TActionEvent evt, LogicEntity trapEntity, LogicEntity targetEntity);
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
        public TEntityPhase entityPhase = TEntityPhase.None;
        [LabelText("事件列表")]
        public List<TActionEvent> events = new List<TActionEvent>();
    }
}
