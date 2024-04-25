using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using PJR.Input;
using System;

namespace PJR.ScriptStates.Player
{
    public class Trans_OnGrounded : ScriptTransition<Trans_OnGrounded>
    {
        protected Trans_OnGrounded() { }
        public override bool Check(EntityScriptState state)
        {
            if (state.normalizeTime < canExitNormalizeTime)
                return false;
            var grounded = state.entity.physEntity.motor.GroundingStatus.IsStableOnGround;
            return !inverse ? grounded : !grounded;
        }
    }
    public class Trans_OnStateFinish : ScriptTransition<Trans_OnStateFinish>
    {
        protected Trans_OnStateFinish() { }
        public override bool Check(EntityScriptState state)
        {
            if (state.normalizeTime < canExitNormalizeTime)
                return false;
            return state.phase == EntityScriptState.Phase.End;
        }
    }
    public class Trans_OnWalking : ScriptTransition<Trans_OnWalking>
    {
        protected Trans_OnWalking() { }
        public override bool Check(EntityScriptState state)
        {
            var inputAxi = state.inputHandle.ReadValue<Vector2>(RegisterKeys.Move);
            return !inverse ? inputAxi.magnitude > 0.003f : inputAxi.magnitude <= 0.003f;
        }
    }
    public class Trans_OnRunning : ScriptTransition<Trans_OnRunning>
    {
        protected Trans_OnRunning() { }
        public override bool Check(EntityScriptState state)
        {
            var inputAxi = state.inputHandle.ReadValue<Vector2>(RegisterKeys.Move);
            var isRun = state.inputHandle.GetKey(RegisterKeys.Run);
            if (inputAxi.magnitude > 0)
            {
                return !inverse ? isRun : !isRun;
            }
            return false;
        }
    }
    public class Trans_JumpInputed : ScriptTransition<Trans_JumpInputed>
    {
        protected Trans_JumpInputed() { }
        public override bool Check(EntityScriptState state)
        {
            var canJump = true;
            canJump &= state.inputHandle.GetKeyDown(RegisterKeys.Jump);
            canJump &= state.entity.entityContext.jumpCount < state.entity.physicsConfig.JumpableTime;

            return !inverse ? canJump : !canJump;
        }
    }

    public struct DirectionNameSet { public string F, FL, FR, B, BL, BR; }

    public static class AnimationNameSet 
    {
        public const string IDLE = "Idle";

        public const string JUMP = "Jump";
        public const string JUMP_START = "Jump_Start";
        public const string JUMP_KEEP = "Jump_Keep";
        public const string JUMP_END = "Jump_End";
        public const string JUMP_LAND_W = "Jump_Land_Wait";
        public const string JUMP_LAND_M = "Jump_Land_Move";

        public const string WALK = "Walk";
        public const string WALK_F = "Walk_Front";
        public const string WALK_FL = "Walk_Front_L";
        public const string WALK_FR = "Walk_Front_R";
        public const string WALK_B = "Walk_Back";
        public const string WALK_BL = "Walk_Back_L";
        public const string WALK_BR = "Walk_Back_R";

        public const string RUN = "Run";
        public const string DASH_F = "Dash_Front";
        public const string DASH_FL = "Dash_Front_L";
        public const string DASH_FR = "Dash_Front_R";
        public const string DASH_B = "Dash_Back";
        public const string DASH_BL = "Dash_Back_L";
        public const string DASH_BR = "Dash_Back_R";


        public static DirectionNameSet Walk = new DirectionNameSet()
        {
            F = WALK_F,
            FL = WALK_FL,
            FR = WALK_FR,
            B = WALK_B,
            BL = WALK_BL,
            BR = WALK_BR,
        };
        public static DirectionNameSet Dash = new DirectionNameSet()
        {
            F = DASH_F,
            FL = DASH_FL,
            FR = DASH_FR,
            B = DASH_B,
            BL = DASH_BL,
            BR = DASH_BR,
        };
    }
}