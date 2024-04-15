using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

namespace PJR
{
    public static class EntityDefine
    {
        public enum LogicEntityType { 
            Empty = 0,
            Player = 1,
            Trap = 2,
        }
        /// <summary>
        /// 物理实体下的模型节点
        /// </summary>
        public static string MODEL_ROOT_NAME = "ModelRoot";

        /// <summary>
        /// 场景根节点下的陷阱节点
        /// </summary>
        public static string SCENE_ROOT_NAME_TRAP = "Traps";

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
}