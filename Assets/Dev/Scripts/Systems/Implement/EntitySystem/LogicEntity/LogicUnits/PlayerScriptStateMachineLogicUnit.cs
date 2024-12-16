using PJR.LogicUnits;
using PJR.ScriptStates;
using PJR.ScriptStates.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJR
{ 
    public class PlayerScriptStateMachineLogicUnit : EntityLogicUnit
    {
        public ScriptEntityStateMachine<EntityScriptState> scriptStateMachine;
        public override bool OnInit(object dependency)
        {
            if(!base.OnInit(dependency))
                return false;
            scriptStateMachine = new PlayerScriptStateMachine(logicEntity);
            scriptStateMachine.Init();
            logicEntity.physEntity.onUpdateVelocity += OnUpdateVelocity;
            logicEntity.physEntity.onUpdateRotation += OnUpdateRotation;
            return true;
        }
        public override void OnUpdate(float deltaTime)
        {
            scriptStateMachine?.Update(deltaTime);
        }
        public override void OnDestroy()
        {
            logicEntity.physEntity.onUpdateVelocity -= OnUpdateVelocity;
            logicEntity.physEntity.onUpdateRotation -= OnUpdateRotation;
        }
        public void EnterState(int state)
        {
            scriptStateMachine.State_Change(state);
        }
        protected virtual void FillKCContext(KCContext context)
        {
            CopyInputKCContent(context);
            context.physConfig = logicEntity.physicsConfig;
        }
        public virtual void CopyInputKCContent(KCContext context)
        {
            context.inputAxi = logicEntity.InputKCContent.inputAxi;
            context.rawMoveInputVector = logicEntity.InputKCContent.rawMoveInputVector;
            context.moveInputVector = logicEntity.InputKCContent.moveInputVector;
            context.lookInputVector = logicEntity.InputKCContent.lookInputVector;
            context.direction = logicEntity.InputKCContent.direction;
            context.inputHandle = logicEntity.InputKCContent.inputHandle;
        }
        protected virtual void OnUpdateRotation(KCContext context)
        {
            if (scriptStateMachine == null)
                return;

            FillKCContext(context);

            scriptStateMachine.CurrentState?.OnUpdateRotation(context);
        }
        protected virtual void OnUpdateVelocity(KCContext context)
        {
            if (scriptStateMachine == null)
                return;

            FillKCContext(context);

            //更新额外值
            //UpdateExtraValue(context.deltaTime);

            //状态的速度控制
            scriptStateMachine.CurrentState?.OnUpdateVelocity(context);
            //额外速度控制
            //UpdateExtraVelocity(context.deltaTime, out var extraVec3);
            //if (extraVec3.y > 0)
            //    context.motor.ForceUnground();
            //context.currentVelocity += extraVec3;
        }
    }
}
