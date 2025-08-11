using PJR.Systems.Input;
using UnityEngine;

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
            return state.phase == ScriptState.Phase.End;
        }
    }
    public class Trans_OnWalking : ScriptTransition<Trans_OnWalking>
    {
        protected Trans_OnWalking() { }
        public override bool Check(EntityScriptState state)
        {
            var inputAxi = state.inputHandle.ReadValue<Vector2>(RegisterKeys.Move);
            float magnitude = inputAxi.magnitude;
            if (state.entity.ContainsExtraValue(EntityDefine.ExtraValueKey.SpeedModify))
                return !inverse ? true : false;

            return !inverse ? inputAxi.magnitude > 0.003f : inputAxi.magnitude <= 0.003f;
        }
    }
    public class Trans_OnRunning : ScriptTransition<Trans_OnRunning>
    {
        protected Trans_OnRunning() { }
        public override bool Check(EntityScriptState state)
        {
            if (state.entity.ContainsExtraValue(EntityDefine.ExtraValueKey.SpeedModify))
                return !inverse ? true : false;

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

    public class Trans_Falling : ScriptTransition<Trans_Falling> 
    {
        protected Trans_Falling() { }
        public override bool Check(EntityScriptState state)
        {
            bool falling = state.entity.physEntity.motor.BaseVelocity.y < 0;
            return !inverse ? falling : !falling;
        }
    }
}