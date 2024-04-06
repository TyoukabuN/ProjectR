using JetBrains.Annotations;
using PJR.ScriptStates.Player;

namespace PJR
{
    public partial class PlayerEntity : LogicEntity
    {
        protected void Init_State()
        {
            stateMachine = new PlayerScriptStateMachine(this);
            stateMachine.Init();
        }
        protected void Update_State()
        {
            stateMachine?.Update();
        }
    }
}
