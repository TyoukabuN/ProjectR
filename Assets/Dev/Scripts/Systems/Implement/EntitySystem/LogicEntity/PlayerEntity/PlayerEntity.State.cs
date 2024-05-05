using UnityEngine;
using PJR.Input;
using PJR.ScriptStates.Player;
using System.Runtime.InteropServices.ComTypes;

namespace PJR
{
    public partial class PlayerEntity
    {
        protected override void Init_State()
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
    }
}
