using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

namespace TinyGame
{
    public static class TEntityDefine
    {
        public static string MODEL_ROOT_NAME = "ModelRoot";
        //KD stand for key define
        public static readonly string KD_DASH_TRIGGER = "DashTrigger";
        public static readonly string KD_ANTIGRAVITY = "AntiGravity";
        public static readonly string KD_TINYGRAVITY = "TinyGravity";
        public static readonly string KD_JUMP = "Jump";

        public static readonly int ANIME_HASH_IDLE_HOLD_0 = Animator.StringToHash("Idle_hold_0");
        public static readonly int ANIME_HASH_IDLE_HOLD_1 = Animator.StringToHash("Idle_hold_1");

        public static readonly int ANIME_HASH_JUMP = Animator.StringToHash("Jump");
        public static readonly int ANIME_HASH_JUMP_INT_VALUE = Animator.StringToHash("JumpIntValue");
        public static readonly int ANIME_HASH_JUMP_COUNT = Animator.StringToHash("JumpCount");
        public static readonly int ANIME_HASH_JUMP_STATE = Animator.StringToHash("JumpState");
        public static readonly int ANIME_HASH_DASH_STATE = Animator.StringToHash("DashState");
        public static readonly int ANIME_HASH_MOVE = Animator.StringToHash("Move");

        public static readonly int ANIME_HASH_DEAD_STATE = Animator.StringToHash("DeadState");
        public static readonly int ANIME_HASH_HURT_STATE = Animator.StringToHash("HurtState");

        public static readonly int ANIME_HASH_ATTACK_TRIGGER = Animator.StringToHash("AttackTrigger");
    }
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
    public enum TPlayerActionType : int
    {
        AddForce = 0,
        CreateGhost = 1,
        AttractBonus = 2,
        Shield = 3,
        Invincible = 4,
        GetHurt = 5,
        MoveFactor = 6,
        PlayAnimeInt = 7,
        PlayAnimeString = 8,
        PlayerControllable = 9,
        GetBonus = 10,
        GetHostage = 11,
        PlayerSimulation = 12,//简单来说就是可不可以控制玩家能不能动
        CameraFollow = 13,
        CameraSwitchInt = 14,
        CameraSwitchString = 15,
        SetAnimatorTrigger = 16,
        SetAnimatorInteger = 17,
        PlayerEffect = 18,
        SelfPlayerEffect = 19,
        Dash = 20,
        Invoke = 21,
        SetTriggerMovePattern = 22,
        Destroy = 23,
        //特殊行为
        KeepOnFloor = 1000,
        TweeningPosY = 1001,
        KeepOnFloorPoint = 1002,
        TriggerPlayerAction = 1003,
    }

    [Serializable]
    public enum TEntityPhase
    {
        None = 0,
        OnCollisionEnter2D = 1,
        OnCollisionExit2D = 2,
        OnTriggerEnter2D = 3,
        OnUpdateDistanceFromPlayer = 4,
        OnFixedUpdateBegin = 5,
        OnCallbackFromLua = 6,
        OnAnimationClipEvent = 7,
    }

    [Serializable]
    public enum BlockEventType
    {
        ChangeBlockWrapMode,
        ChangeBlockGenerateMode,
        SendLuaEvent,
        AddPlayerAction,
        Camera,
        ChangeGameState,
    }
    [Serializable]
    public class TActionEvent
    {
        public BlockEventType blockRootEventType;
        //
        //public BlockWrapMode blockWrapMode;
        //public BlockGenerateMode blockGenerateMode;
        //
        public string luaEventName = "TINYGAME_CS_CALL_LUA";
        public int luaEventType = 1;
        public string luaEventParam = string.Empty;
        //
        public TPlayerActionType playerActionType = TPlayerActionType.CreateGhost;
        public float attractBonusDistance = 2f;
        public float attractBonusSmoothness = 5f;
        public float duration = 3f;
        //
        public Vector3 forceDirection = Vector3.right;
        public float forceFactor = 1f; 
        public float damp = 0f;
        //
        public int intValue = 0;
        public float floatValue = 0f;
        public bool boolValue = false;
        public string stringValue = string.Empty;
        public Vector3 vector2Value = Vector2.one;
        public Vector3 vector3Value = Vector3.one;
        public UnityEngine.Object objectValue = null;

        public UnityEvent onInvoke = new UnityEvent();


        public Vector3 force {
            get { return forceDirection * forceFactor; }
        }
        //
        public bool cameraFollow = true;
    }

    public enum TGManagerTrackChangeType
    {
        GameState,
        SwitchCamera,
        CheckPlayerOutOfSight,
    }
    public enum OutOfSightType:int
    { 
        None = 0,
        Left = 1,
        Right = 2,
        Bottom = 3,
        Top = 4,
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
}