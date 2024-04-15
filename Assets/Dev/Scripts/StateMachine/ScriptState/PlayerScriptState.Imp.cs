using Animancer;
using UnityEngine;
using PJR.Input;

namespace PJR.ScriptStates.Player
{
    public enum EPlayerState
    {
        None = 0,
        Stand = 1,
        Walk = 2,
        Running = 3,
        Jump_Begin = 4,
        Jump_Falling = 5,
        Jump_Land = 6,
        Dushing = 7,
        End = 8,
    }
    public class AnimationState : EntityScriptState
    {
        protected AnimancerState animancerState;
        public override float normalizeTime
        {
            get
            {
                if (animancerState == null)
                    return 0f;
                return animancerState.NormalizedTime;
            }
        }
    }
    public class StandState : EntityScriptState
    {
        public override void Update(EntityContext stateContext)
        {
            entity.physEntity.Animancer_Play(AnimationNameSet.IDLE);
        }
        public override bool CanChange(int from)
        {
            return true;
        }
        public override void OnUpdateVelocity(KCContext context)
        {
            base.OnUpdateVelocity(context);

            PlayerControlFunc.GroundedMove(context);
        }
    }

    public class WalkState : EntityScriptState
    {
        public override void Update(EntityContext stateContext)
        {
            var inputAxi = inputHandle.ReadValueVec2(RegisterKeys.Move, true);
            var nameSet = AnimationNameSet.Walk;
            if (inputAxi.magnitude > 0)
            {
                if (Mathf.Abs(inputAxi.x) <= 0.001f)
                {
                    entity.physEntity.Animancer_Play(inputAxi.y > 0 ? nameSet.F : nameSet.B);
                }
                else
                {
                    if (inputAxi.y > 0)
                        entity.physEntity.Animancer_Play(inputAxi.x > 0 ? nameSet.FR : nameSet.FL);
                    else
                        entity.physEntity.Animancer_Play(inputAxi.x > 0 ? nameSet.BR : nameSet.BL);
                }
            }
            else
            {
                entity.physEntity.Animancer_Play(AnimationNameSet.IDLE);
            }
        }
        public override void OnUpdateVelocity(KCContext context)
        {
            base.OnUpdateVelocity(context);

            PlayerControlFunc.GroundedMove(context);
        }
        public override bool CanChange(int from)
        {
            return true;
        }
    }
    public class RunningState : EntityScriptState
    {
        public override void Update(EntityContext stateContext)
        {
            var inputAxi = inputHandle.ReadValueVec2(RegisterKeys.Move, true);
            var nameSet = AnimationNameSet.Dash;
            if (inputAxi.magnitude > 0)
            {
                if (Mathf.Abs(inputAxi.x) <= 0.001f)
                {
                    entity.physEntity.Animancer_Play(inputAxi.y > 0 ? nameSet.F : nameSet.B);
                }
                else
                {
                    if (inputAxi.y > 0)
                        entity.physEntity.Animancer_Play(inputAxi.x > 0 ? nameSet.FR : nameSet.FL);
                    else
                        entity.physEntity.Animancer_Play(inputAxi.x > 0 ? nameSet.BR : nameSet.BL);
                }
            }
            else
            {
                entity.physEntity.Animancer_Play(AnimationNameSet.IDLE);
            }
        }
        public override bool CanChange(int from)
        {
            return true;
        }
        public override void OnUpdateVelocity(KCContext context)
        {
            base.OnUpdateVelocity(context);

            PlayerControlFunc.GroundedMove(context);
        }
    }

    public class JumpBeginState : AnimationState
    {
        public bool requireJump = false;
        protected void Animation_OnJumpStartEnd()
        {
            //if (!entity.physEntity.Grounded) entity.physEntity.Animancer_Play(AnimationNameSet.JUMP_KEEP, 0, FadeMode.FromStart);
        }

        public override void OnEnter(LogicEntity entity)
        {
            base.OnEnter(entity);
            requireJump = true;
            animancerState = entity.physEntity.Animancer_Play(AnimationNameSet.JUMP_START, 0, FadeMode.FromStart);
            //animancerState.Events.OnEnd = Animation_OnJumpStartEnd;
        }
        public override void OnChange(int from)
        {
            if (animancerState != null)
                animancerState.Events.OnEnd = null;
        }
        public override bool CanChange(int from)
        {
            if (from == (int)EPlayerState.Stand || from == (int)EPlayerState.Walk || from == (int)EPlayerState.Running)
                return entity.physEntity.Grounded;

            return true;
        }
        public override void OnUpdateVelocity(KCContext context)
        {
            base.OnUpdateVelocity(context);

            if (requireJump)
            { 
                requireJump = !requireJump;
                entity.entityContext.AddJumpCount();
                PlayerControlFunc.EnterToAir(context);
                phase = Phase.End;
            }
        }
    }
    public class JumpFallingState : AnimationState
    {
        public override void OnEnter(LogicEntity entity)
        {
            base.OnEnter(entity);

            animancerState = entity.physEntity.Animancer_Play(AnimationNameSet.JUMP_KEEP, 0, FadeMode.FromStart);
        }
        public override bool CanChange(int from)
        {
            if (from == (int)EPlayerState.Stand || from == (int)EPlayerState.Walk || from == (int)EPlayerState.Running)
            {
                return entity.physEntity.motor.GroundingStatus.IsStableOnGround;
            }

            return true;
        }
        public override void OnUpdateVelocity(KCContext context)
        {
            base.OnUpdateVelocity(context);

            PlayerControlFunc.OnAir(context);
        }
    }

    public class JumpLandState : AnimationState
    {
        protected void Animation_OnJumpStartEnd()
        {
            if (!entity.physEntity.Grounded)
                entity.physEntity.Animancer_Play(AnimationNameSet.JUMP_KEEP, 0, FadeMode.FromStart);
        }
        public override void OnEnter(LogicEntity entity)
        {
            base.OnEnter(entity);
            var moving = entity.inputHandle.ReadValueVec2(RegisterKeys.Move).magnitude > 0;
            animancerState = entity.physEntity.Animancer_Play(moving ? AnimationNameSet.JUMP_LAND_M : AnimationNameSet.JUMP_LAND_W);
            animancerState.Events.OnEnd = ToPhaseEnd;
            entity.entityContext.RevertJumpCount();
        }
        public override void OnChange(int from)
        {
            if (animancerState != null)
                animancerState.Events.OnEnd = null;
        }
        public override bool CanChange(int from)
        {
            if (from == (int)EPlayerState.Stand || from == (int)EPlayerState.Walk || from == (int)EPlayerState.Running)
            {
                return entity.physEntity.Grounded;
            }

            return true;
        }
    }
}