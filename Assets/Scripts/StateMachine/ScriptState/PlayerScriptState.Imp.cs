using Animancer;
using PJR.Systems.Input;
using Sirenix.OdinInspector;

namespace PJR.ScriptStates.Player
{
    public enum EPlayerState
    {
        None = 0,
        Idle = 1,
        Walk = 2,
        Running = 3,
        Jump_Begin = 4,
        Jump_Falling = 5,
        Jump_Land = 6,
        Dushing = 7,
        [LabelText("绊倒")]
        Stumble = 8,
        End = 9,
    }
    public class AnimationState : EntityScriptState
    {
        protected AnimancerState animancerState;
        public override float normalizeTime
        {
            get
            {
                if (animancerState == null)
                    return 1f;
                return animancerState.NormalizedTime;
            }
        }
    }
    public class StandState : EntityScriptState
    {
        public override void Update(float deltaTime)
        {
            entity.entityContext.RevertJumpCount();
            entity.physEntity.Animancer_Play(EntityAnimationDefine.AnimationName.Idle);
        }
        public override bool CanChange(int from)
        {
            return true;
        }
        public override void OnUpdateVelocity(KCContext context)
        {
            base.OnUpdateVelocity(context);

            PlayerKCCFunc.GroundedMove(context);
        }
    }

    public class WalkState : EntityScriptState
    {
        public override void Update(float deltaTime)
        {
            var inputAxi = inputHandle.ReadValueVec2(RegisterKeys.Move, true);
            var nameSet = EntityAnimationDefine.AnimationName.Walk;
            if (inputAxi.magnitude > 0)
            {
                //if (Mathf.Abs(inputAxi.x) <= 0.001f)
                //{
                //    entity.physEntity.Animancer_Play(inputAxi.y > 0 ? nameSet.F : nameSet.B);
                //}
                //else
                //{
                //    if (inputAxi.y > 0)
                //        entity.physEntity.Animancer_Play(inputAxi.x > 0 ? nameSet.FR : nameSet.FL);
                //    else
                //        entity.physEntity.Animancer_Play(inputAxi.x > 0 ? nameSet.BR : nameSet.BL);
                //}
                entity.physEntity.Animancer_Play(EntityAnimationDefine.AnimationName.WALK);
            }
            else
            {
                entity.physEntity.Animancer_Play(EntityAnimationDefine.AnimationName.Idle);
            }
        }
        public override void OnUpdateVelocity(KCContext context)
        {
            base.OnUpdateVelocity(context);

            PlayerKCCFunc.GroundedMove(context);
        }
        public override bool CanChange(int from)
        {
            return true;
        }
    }
    public class RunningState : EntityScriptState
    {
        public override void Update(float deltaTime)
        {
            var inputAxi = inputHandle.ReadValueVec2(RegisterKeys.Move, true);
            var nameSet = EntityAnimationDefine.AnimationName.Dash;
            var dashing = entity.ContainsExtraValue(EntityDefine.ExtraValueKey.SpeedModify);
            if (inputAxi.magnitude > 0 || dashing)
            {
                //if (Mathf.Abs(inputAxi.x) <= 0.001f)
                //{
                //    entity.physEntity.Animancer_Play(inputAxi.y > 0 ? nameSet.F : nameSet.B);
                //}
                //else
                //{
                //    if (inputAxi.y > 0)
                //        entity.physEntity.Animancer_Play(inputAxi.x > 0 ? nameSet.FR : nameSet.FL);
                //    else
                //        entity.physEntity.Animancer_Play(inputAxi.x > 0 ? nameSet.BR : nameSet.BL);
                //}
                if (dashing)
                    entity.physEntity.Animancer_Play(EntityAnimationDefine.AnimationName.Roll_Loop);
                else
                    entity.physEntity.Animancer_Play(EntityAnimationDefine.AnimationName.RUN);
            }
            else
            {
                entity.physEntity.Animancer_Play(EntityAnimationDefine.AnimationName.Idle);
            }
        }
        public override bool CanChange(int from)
        {
            return true;
        }
        public override void OnUpdateVelocity(KCContext context)
        {
            base.OnUpdateVelocity(context);

            PlayerKCCFunc.GroundedMove(context);
        }
    }

    public class JumpBeginState : AnimationState
    {
        public bool requireJump = false;
        protected void Animation_OnJumpStartEnd()
        {
        }

        public override void OnEnter(LogicEntity entity)
        {
            base.OnEnter(entity);
            requireJump = true;
            if (entity.physEntity.Grounded)
                animancerState = entity.physEntity.Animancer_Play(EntityAnimationDefine.AnimationName.Jump_Start, 0, FadeMode.FromStart);
            else
                animancerState = entity.physEntity.Animancer_Play(EntityAnimationDefine.AnimationName.Jump_Air_Start, 0, FadeMode.FromStart);
            //animancerState.Events.OnEnd = Animation_OnJumpStartEnd;
        }
        public override void OnChange(int from)
        {
            if (animancerState != null)
                animancerState.Events.OnEnd = null;
        }
        public override bool CanChange(int from)
        {
            if (from == (int)EPlayerState.Idle || from == (int)EPlayerState.Walk || from == (int)EPlayerState.Running)
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
                PlayerKCCFunc.EnterToAir(context);
                phase = Phase.End;
            }else
                PlayerKCCFunc.OnAir(context);
        }
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
        }
    }
    public class JumpFallingState : AnimationState
    {
        public override void OnEnter(LogicEntity entity)
        {
            base.OnEnter(entity);

            animancerState = entity.physEntity.Animancer_Play(EntityAnimationDefine.AnimationName.Jump_Fall_Loop);
        }
        public override bool CanChange(int from)
        {
            if (from == (int)EPlayerState.Idle || from == (int)EPlayerState.Walk || from == (int)EPlayerState.Running)
            {
                return entity.physEntity.motor.GroundingStatus.IsStableOnGround;
            }

            return true;
        }
        public override void OnUpdateVelocity(KCContext context)
        {
            base.OnUpdateVelocity(context);

            PlayerKCCFunc.OnAir(context);
        }
    }

    public class JumpLandState : AnimationState
    {
        public override void OnEnter(LogicEntity entity)
        {
            base.OnEnter(entity);
            var moving = entity.inputHandle.ReadValueVec2(RegisterKeys.Move).magnitude > 0;
            animancerState = entity.physEntity.Animancer_Play(EntityAnimationDefine.AnimationName.Jump_Land);
            entity.entityContext.RevertJumpCount();
            ToPhaseEnd();
        }
        //public override void OnChange(int from)

        public override bool CanChange(int from)
        {
            if (from == (int)EPlayerState.Idle || from == (int)EPlayerState.Walk || from == (int)EPlayerState.Running)
            {
                return entity.physEntity.Grounded;
            }

            return true;
        }

        public override void OnUpdateVelocity(KCContext context)
        {
            base.OnUpdateVelocity(context);
            PlayerKCCFunc.GroundedMove(context);
        }
    }

    /// <summary>
    /// 绊倒
    /// </summary>
    public class StumbleState : AnimationState
    {
        public override void OnEnter(LogicEntity entity)
        {
            base.OnEnter(entity);
            animancerState = entity.physEntity.Animancer_Play(EntityAnimationDefine.AnimationName.Stumble);
            ToPhaseEnd();
        }
        public override void OnUpdateVelocity(KCContext context)
        {
            PlayerKCCFunc.InHurt(context);
        }
        public override void OnUpdateRotation(KCContext context)
        {
        }
        public override void OnChange(int from)
        {
            base.OnChange(from);
        }
    }


}