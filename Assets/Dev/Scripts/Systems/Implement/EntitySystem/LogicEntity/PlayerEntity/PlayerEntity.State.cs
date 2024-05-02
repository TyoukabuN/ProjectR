using UnityEngine;
using PJR.Input;
using PJR.ScriptStates.Player;
using System.Runtime.InteropServices.ComTypes;

namespace PJR
{
    public partial class PlayerEntity : LogicEntity
    {
        protected void Init_State()
        {
            scriptStateMachine = new PlayerScriptStateMachine(this);
            scriptStateMachine.Init();
            physEntity.onUpdateVelocity += OnUpdateVelocity;
            physEntity.onUpdateRotation += OnUpdateRotation;
        }

        public override void EnterState(int state)
        {
            base.EnterState(state);
            scriptStateMachine.State_Change(state);
        }
        protected void Update_State()
        {
            scriptStateMachine?.Update();
        }

        protected void Destroy_State()
        {
            physEntity.onUpdateVelocity -= OnUpdateVelocity;
            physEntity.onUpdateRotation -= OnUpdateRotation;
        }
        protected void FillKCContext(KCContext context)
        {
            CopyInputKCContent(context);
            context.physConfig = physicsConfig;
        }
        protected void OnUpdateRotation(KCContext context)
        {
            if (scriptStateMachine == null)
                return;

            FillKCContext(context);

            scriptStateMachine.CurrentState?.OnUpdateRotation(context);
        }
        protected void OnUpdateVelocity(KCContext context)
        {
            if (scriptStateMachine == null) 
                return;

            FillKCContext(context);

            //状态的速度控制
            scriptStateMachine.CurrentState?.OnUpdateVelocity(context);
            ////额外速度控制
            this.UpdateExtraVelocity(context.deltaTime,out var extraVec3);
            if (extraVec3.y > 0)
                context.motor.ForceUnground();
            context.currentVelocity += extraVec3;
        }
    }
}
