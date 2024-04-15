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
        [LabelText("类型")]
        public TActionType actionType = TActionType.AddForce;
        [ShowInInspector]
        [LabelText("事件参数文件")]
        public TrapActionParamAsset trapActionEventParam = null;
    }

    [Serializable]
    public class AddForce : TrapActionParamAsset
    {
        [LabelText("力")] public Vector3 force = Vector3.zero;
        [LabelText("持续时间")] public float duration = 0.333f;
        [LabelText("衰减系数")][PropertyTooltip(ExtraVelocity.tooltip)] public float damp = -1f;
        [LabelText("衰减曲线")] public Easing.Ease easing = Easing.Ease.Linear;
    }

    public enum DampType {
        
        NormalizeTime,
        NoAttenuation,
        Factor,
        ///[=-1 ] 时 使用counterNormalize作为衰减系数 linear
        ///[= 0 ] 时;不衰减
        ///[> 0 ] 时;为线性衰减forceDamped -= forceDamped * damp * deltaTime
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
