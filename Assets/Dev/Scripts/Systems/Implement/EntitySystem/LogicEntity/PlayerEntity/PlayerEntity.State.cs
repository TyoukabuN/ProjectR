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
        protected void Update_State()
        {
            scriptStateMachine?.Update();
        }

        protected void Destroy_State()
        {
            physEntity.onUpdateVelocity -= OnUpdateVelocity;
            physEntity.onUpdateRotation -= OnUpdateRotation;
        }

        protected void OnUpdateVelocity(KCContext context)
        {
            if (scriptStateMachine == null) 
                return;

            var inputAxi = inputHandle.ReadValueVec2(RegisterKeys.Move);
            context.inputAxi = inputAxi;
            context.inputAxiVec3 = Vector3.ClampMagnitude(new Vector3(inputAxi.x, 0f, inputAxi.y), 1f);
            context.direction = context.inputAxiVec3;
            context.inputHandle = inputHandle;
            //context.direction = Quaternion.Euler(0, physEntity.transform.eulerAngles.y, 0) * new Vector3(inputAxi.x, 0, inputAxi.y);
            context.physConfig = physicsConfig;

            //状态的速度控制
            scriptStateMachine.CurrentState?.OnUpdateVelocity(context);
            ////额外速度控制
            this.UpdateExtraVelocity(context.deltaTime,out var extraVec3);
            if (extraVec3.y > 0)
                context.motor.ForceUnground();
            context.currentVelocity += extraVec3;
        }
        protected void OnUpdateRotation(KCContext context)
        {
            if (scriptStateMachine == null) 
                return;

            scriptStateMachine.CurrentState?.OnUpdateRotation(context);
        }
    }
}
