using UnityEngine;
using PJR.Input;
using PJR.ScriptStates.Player;

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

        protected void OnUpdateVelocity(KCCContext context)
        {
            if (scriptStateMachine == null) 
                return;

            var inputAxi = inputHandle.ReadValueVec2(RegisterKeys.Move, true);
            context.inputAxi = inputAxi;
            context.direction = Quaternion.Euler(0, physEntity.transform.eulerAngles.y, 0) * new Vector3(inputAxi.x, 0, inputAxi.y);

            scriptStateMachine.CurrentState?.OnUpdateVelocity(context);
        }
        protected void OnUpdateRotation(KCCContext context)
        {
            if (scriptStateMachine == null) 
                return;

            scriptStateMachine.CurrentState?.OnUpdateRotation(context);
        }
    }
}
