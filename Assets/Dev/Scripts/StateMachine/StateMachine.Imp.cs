using Animancer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TG
{
    #region Transition

    #endregion

    #region State

    public class AnimationState : State { 
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
    public class StandState : State
    {
        public override void Update(StateContext stateContext)
        {
            entity.Animancer_Play(AnimationNameSet.IDLE);
        }
        public override bool CanChange(int from)
        {
            return true;
        }
    }

    public class WalkState : State
    {
        public override void Update(StateContext stateContext)
        {
            var inputAxi = stateContext.inputAxi;
            var nameSet = AnimationNameSet.Walk;
            if (inputAxi.magnitude > 0)
            {
                if (Mathf.Abs(inputAxi.x) <= 0.001f)
                {
                    entity.Animancer_Play(inputAxi.y > 0 ? nameSet.F : nameSet.B);
                }
                else
                {
                    if (inputAxi.y > 0)
                        entity.Animancer_Play(inputAxi.x > 0 ? nameSet.FR : nameSet.FL);
                    else
                        entity.Animancer_Play(inputAxi.x > 0 ? nameSet.BR : nameSet.BL);
                }
            }
            else
            {
                entity.Animancer_Play(AnimationNameSet.IDLE);
            }
        }
        public override bool CanChange(int from)
        {
            return true;
        }
    }
    public class RunningState : State
    {
        public override void Update(StateContext stateContext)
        {
            var inputAxi = stateContext.inputAxi;
            var nameSet = AnimationNameSet.Dash;
            if (inputAxi.magnitude > 0)
            {
                if (Mathf.Abs(inputAxi.x) <= 0.001f)
                {
                    entity.Animancer_Play(inputAxi.y > 0 ? nameSet.F : nameSet.B);
                }
                else
                {
                    if (inputAxi.y > 0)
                        entity.Animancer_Play(inputAxi.x > 0 ? nameSet.FR : nameSet.FL);
                    else
                        entity.Animancer_Play(inputAxi.x > 0 ? nameSet.BR : nameSet.BL);
                }
            }
            else
            {
                entity.Animancer_Play(AnimationNameSet.IDLE);
            }
        }
        public override bool CanChange(int from)
        {
            return true;
        }
    }

    public class JumpBeginState : AnimationState
    {
        protected void Animation_OnJumpStartEnd()
        {
            phase = Phase.End;
            //if (!entity.Grounded) entity.Animancer_Play(AnimationNameSet.JUMP_KEEP, 0, FadeMode.FromStart);
        }

        public override void OnEnter(StateMachineEntity entity)
        {
            base.OnEnter(entity);
            animancerState = entity.Animancer_Play(AnimationNameSet.JUMP_START, 0, FadeMode.FromStart);
            animancerState.Events.OnEnd = Animation_OnJumpStartEnd;
        }
        public override void OnChange(int from)
        {
            if (animancerState != null)
                animancerState.Events.OnEnd = null;
        }
        public override bool CanChange(int from)
        {
            if (from == EPlayerState.Stand || from == EPlayerState.Walk || from == EPlayerState.Running)
                return entity.Grounded;

            return true;
        }
    }
    public class JumpFallingState : AnimationState
    {
        public override void OnEnter(StateMachineEntity entity)
        {
            base.OnEnter(entity);

            animancerState = entity.Animancer_Play(AnimationNameSet.JUMP_KEEP, 0, FadeMode.FromStart);
        }
        public override bool CanChange(int from)
        {
            if (from == EPlayerState.Stand || from == EPlayerState.Walk || from == EPlayerState.Running)
            {
                return entity.Grounded;
            }

            return true;
        }
    }

    public class JumpLandState : AnimationState
    {
        protected void Animation_OnJumpStartEnd()
        {
            if (!entity.Grounded)
                entity.Animancer_Play(AnimationNameSet.JUMP_KEEP, 0, FadeMode.FromStart);
        }
        public override void OnEnter(StateMachineEntity entity)
        {
            base.OnEnter(entity);
            var moving = entity.stateContext.inputAxi.magnitude > 0;
            animancerState = entity.Animancer_Play(moving ? AnimationNameSet.JUMP_LAND_M : AnimationNameSet.JUMP_LAND_W);
            animancerState.Events.OnEnd = ToPhaseEnd;
        }
        public override void OnChange(int from)
        {
            if (animancerState != null)
                animancerState.Events.OnEnd = null;
        }
        public override bool CanChange(int from)
        {
            if (from == EPlayerState.Stand || from == EPlayerState.Walk || from == EPlayerState.Running)
            {
                return entity.Grounded;
            }

            return true;
        }
    }
#endregion

}