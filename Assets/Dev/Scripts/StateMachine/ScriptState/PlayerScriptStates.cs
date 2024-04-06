using Animancer;
using UnityEngine;

namespace PJR.ScriptStates.Player
{
    public static class EPlayerState
    {
        public static int None = 0;
        public static int Stand = 1;
        public static int Walk = 2;
        public static int Running = 3;
        public static int Jump_Begin = 4;
        public static int Jump_Falling = 5;
        public static int Jump_Land = 6;
        public static int Dushing = 7;
        public static int End = 8;
    }
    public class AnimationState : ScriptState
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
    public class StandState : ScriptState
    {
        public override void Update(StateContext stateContext)
        {
            entity.physEntity.Animancer_Play(AnimationNameSet.IDLE);
        }
        public override bool CanChange(int from)
        {
            return true;
        }
    }

    public class WalkState : ScriptState
    {
        public override void Update(StateContext stateContext)
        {
            var inputAxi = inputHandle.ReadValue<Vector3>(InputKey.RegisterKeys.Move);
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
        public override bool CanChange(int from)
        {
            return true;
        }
    }
    public class RunningState : ScriptState
    {
        public override void Update(StateContext stateContext)
        {
            var inputAxi = inputHandle.ReadValue<Vector3>(InputKey.RegisterKeys.Move);
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
    }

    public class JumpBeginState : AnimationState
    {
        protected void Animation_OnJumpStartEnd()
        {
            phase = Phase.End;
            //if (!entity.physEntity.Grounded) entity.physEntity.Animancer_Play(AnimationNameSet.JUMP_KEEP, 0, FadeMode.FromStart);
        }

        public override void OnEnter(LogicEntity entity)
        {
            base.OnEnter(entity);
            animancerState = entity.physEntity.Animancer_Play(AnimationNameSet.JUMP_START, 0, FadeMode.FromStart);
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
                return entity.physEntity.Grounded;

            return true;
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
            if (from == EPlayerState.Stand || from == EPlayerState.Walk || from == EPlayerState.Running)
            {
                return entity.physEntity.Grounded;
            }

            return true;
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
            var moving = entity.inputHandle.ReadValue<Vector3>(InputKey.RegisterKeys.Move).magnitude > 0;
            animancerState = entity.physEntity.Animancer_Play(moving ? AnimationNameSet.JUMP_LAND_M : AnimationNameSet.JUMP_LAND_W);
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
                return entity.physEntity.Grounded;
            }

            return true;
        }
    }
}