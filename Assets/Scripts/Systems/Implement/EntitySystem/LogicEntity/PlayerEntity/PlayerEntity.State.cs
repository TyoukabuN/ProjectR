namespace PJR
{
    public partial class PlayerEntity
    {
        private PlayerScriptStateMachineLogicUnit playerScriptStateMachineLogicUnit;

        private PlayerScriptStateMachineLogicUnit PlayerScriptStateMachineLogicUnit => playerScriptStateMachineLogicUnit;
        protected void Init_State()
        {
            playerScriptStateMachineLogicUnit = AddLogicUnit<PlayerScriptStateMachineLogicUnit>();
        }

        public override void EnterState(int state)
        {
            base.EnterState(state);
            playerScriptStateMachineLogicUnit?.EnterState(state);
        }
        protected virtual void OnUpdateVelocity(KCContext context)
        {
            //更新额外值
            this.UpdateExtraValue(context.deltaTime);
            //额外速度控制
            this.UpdateExtraVelocity(context.deltaTime, out var extraVec3);
            if (extraVec3.y > 0)
                context.motor.ForceUnground();
            context.currentVelocity += extraVec3;
        }
    }
}
