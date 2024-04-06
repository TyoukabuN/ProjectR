using System.Collections.Generic;
using PJR.ScriptStates;
using PJR.ScriptStates.Player;

namespace PJR.ScriptStates.Player
{
    public class PlayerScriptStateMachine : ScriptEntityStateMachine
    {
        public override int CurrentState => currentState;

        private int currentState = EPlayerState.None;

        public PlayerScriptStateMachine(LogicEntity entity) : base(entity) { }

        public override void Init()
        {
            int length = (int)EPlayerState.End - 1;
            states = new ScriptState[length];
            state2transition = new Dictionary<int, ScriptTransition[]>();


            states[EPlayerState.Stand] = new StandState();
            states[EPlayerState.Walk] = new WalkState();
            state2transition[EPlayerState.Walk] = new ScriptTransition[]
            {
                ScriptTransition<Trans_OnRunning>.Get(EPlayerState.Running),
            };

            states[EPlayerState.Running] = new RunningState();
            state2transition[EPlayerState.Running] = new ScriptTransition[]
            {
                ScriptTransition<Trans_OnRunning>.Get(EPlayerState.Walk).SetInverse(true),
            };

            states[EPlayerState.Jump_Begin] = new JumpBeginState();
            state2transition[EPlayerState.Jump_Begin] = new ScriptTransition[]
            {
                ScriptTransition<Trans_OnStateFinish>.Get(EPlayerState.Jump_Falling).SetCanExitNormalizeTime(0.95f),
                ScriptTransition<Trans_OnGrounded>.Get(EPlayerState.Jump_Land).SetCanExitNormalizeTime(0.5f),
            };


            states[EPlayerState.Jump_Falling] = new JumpFallingState();
            state2transition[EPlayerState.Jump_Falling] = new ScriptTransition[]
            {
                ScriptTransition<Trans_OnGrounded>.Get(EPlayerState.Jump_Land),
            };

            states[EPlayerState.Jump_Land] = new JumpLandState();
            state2transition[EPlayerState.Jump_Land] = new ScriptTransition[]
            {
                ScriptTransition<Trans_OnRunning>.Get(EPlayerState.Running),
                ScriptTransition<Trans_OnWalking>.Get(EPlayerState.Walk),
                ScriptTransition<Trans_OnStateFinish>.Get(EPlayerState.Stand),

            };

            State_Change(EPlayerState.Stand);
        }

        public override bool State_Change(int ePlayerState)
        {
            if (ePlayerState == currentState)
            {
                states[(int)currentState].OnEnter(ownEntity);
                return true;
            }
            if (ePlayerState == EPlayerState.None)
                return false;
            if (states[(int)ePlayerState] == null)
                return false;
            if (states[(int)currentState] != null)
            {
                if (!states[(int)currentState].CanChange(ePlayerState))
                    return false;
                states[(int)currentState].OnChange(ePlayerState);
            }
            currentState = ePlayerState;
            states[(int)currentState].OnEnter(ownEntity);
            return true;
        }

        public override void UpdateContext()
        {
            //stateContext.inputAxi = inputAxi;
            //stateContext.mouseDelta = mouseDelta;
            //stateContext.runValue = runValue;
            //stateContext.grounded = Grounded ? 1 : 0;
        }
        // Update is called once per frame
        public override void Update()
        {
            base.Update();
        }
    }
}
