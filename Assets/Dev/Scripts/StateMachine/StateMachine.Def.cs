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
        public override bool Check(ScriptState state)
        {
            if (state.normalizeTime < canExitNormalizeTime)
                return false;
            return state.entity.entityContext.grounded > 0;
        }
    }
    public class Trans_OnStateFinish : ScriptTransition<Trans_OnStateFinish>
    {
        protected Trans_OnStateFinish() { }
        public override bool Check(ScriptState state)
        {
            if (state.normalizeTime < canExitNormalizeTime)
                return false;
            return state.phase == ScriptState.Phase.End;
        }
    }
    public class Trans_OnWalking : ScriptTransition<Trans_OnWalking>
    {
        protected Trans_OnWalking() { }
        public override bool Check(ScriptState state)
        {
            return state.inputHandle.ReadValue<Vector2>(RegisterKeys.Move).magnitude > 0;
        }
    }
    public class Trans_OnRunning : ScriptTransition<Trans_OnRunning>
    {
        protected Trans_OnRunning() { }
        public override bool Check(ScriptState state)
        {
            var inputAxi = state.inputHandle.ReadValue<Vector2>(RegisterKeys.Move);
            var isRun = state.inputHandle.GetKeyDown(RegisterKeys.Run);
            if (inputAxi.magnitude > 0)
            {
                return inverse ? isRun : !isRun;
            }
            return inverse;
        }
    }

    public struct DirectionNameSet { public string F, FL, FR, B, BL, BR; }

    public static class AnimationNameSet 
    {
        public const string IDLE = "Idle";

        public const string JUMP_START = "Jump_Start";
        public const string JUMP_KEEP = "Jump_Keep";
        public const string JUMP_LAND_W = "Jump_Land_Wait";
        public const string JUMP_LAND_M = "Jump_Land_Move";

        public const string WALK_F = "Walk_Front";
        public const string WALK_FL = "Walk_Front_L";
        public const string WALK_FR = "Walk_Front_R";
        public const string WALK_B = "Walk_Back";
        public const string WALK_BL = "Walk_Back_L";
        public const string WALK_BR = "Walk_Back_R";

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