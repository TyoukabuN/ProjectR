using System.Collections.Generic;
using PJR.Input;
using PJR.ScriptStates;
using PJR.ScriptStates.Player;

namespace PJR.ScriptStates.Player
{
    public class PlayerScriptStateMachine : ScriptEntityStateMachine<EntityScriptState>
    {
        public override int CurrentEState => currentEState;

        private int currentEState = (int)EPlayerState.None;
        public PlayerScriptStateMachine(LogicEntity entity) : base(entity) { }

        public override void Init()
        {
            int length = (int)(int)EPlayerState.End - 1;
            states = new EntityScriptState[length];
            state2transition = new Dictionary<int, List<ScriptTransition>>();

            //
            states[(int)EPlayerState.Stand] = new StandState();
            state2transition[(int)EPlayerState.Stand] = new List<ScriptTransition>
            {
                ScriptTransition<Trans_OnRunning>.Get((int)EPlayerState.Running),
                ScriptTransition<Trans_OnWalking>.Get((int)EPlayerState.Walk),
            };

            states[(int)EPlayerState.Walk] = new WalkState();
            state2transition[(int)EPlayerState.Walk] = new List<ScriptTransition>
            {
                ScriptTransition<Trans_OnRunning>.Get((int)EPlayerState.Running),
                ScriptTransition<Trans_OnWalking>.Get((int)EPlayerState.Stand).SetInverse(true),
            };

            states[(int)EPlayerState.Running] = new RunningState();
            state2transition[(int)EPlayerState.Running] = new List<ScriptTransition>
            {
                ScriptTransition<Trans_OnRunning>.Get((int)EPlayerState.Walk).SetInverse(true),
                ScriptTransition<Trans_OnWalking>.Get((int)EPlayerState.Stand).SetInverse(true),
            };



            states[(int)EPlayerState.Jump_Begin] = new JumpBeginState();
            state2transition[(int)EPlayerState.Jump_Begin] = new List<ScriptTransition>
            {
                ScriptTransition<Trans_OnStateFinish>.Get((int)EPlayerState.Jump_Falling).SetCanExitNormalizeTime(0.95f),
                ScriptTransition<Trans_OnGrounded>.Get((int)EPlayerState.Jump_Land).SetCanExitNormalizeTime(0.5f),
            };

            states[(int)EPlayerState.Jump_Falling] = new JumpFallingState();
            state2transition[(int)EPlayerState.Jump_Falling] = new List<ScriptTransition>
            {
                ScriptTransition<Trans_OnGrounded>.Get((int)EPlayerState.Jump_Land),
            };

            states[(int)EPlayerState.Jump_Land] = new JumpLandState();
            state2transition[(int)EPlayerState.Jump_Land] = new List<ScriptTransition>
            {
                ScriptTransition<Trans_OnRunning>.Get((int)EPlayerState.Running),
                ScriptTransition<Trans_OnWalking>.Get((int)EPlayerState.Walk),
                ScriptTransition<Trans_OnGrounded>.Get((int)EPlayerState.Stand),
            };

            //判断不在地面上的转换
            AddTransTo<Trans_OnGrounded>(EPlayerState.Jump_Falling,
                new int[] {
                    (int)EPlayerState.Stand,
                    (int)EPlayerState.Walk,
                    (int)EPlayerState.Running,
                    (int)EPlayerState.Jump_Land
                }, true);

            //可以进入跳跃状态的转换
            AddTransTo<Trans_JumpInputed>(EPlayerState.Jump_Begin,
            new int[] {
                (int)EPlayerState.Stand,
                (int)EPlayerState.Walk,
                (int)EPlayerState.Running,
                (int)EPlayerState.Jump_Falling,
                (int)EPlayerState.Jump_Land,
            });

            State_Change((int)EPlayerState.Stand);
        }

        public void AddTransTo<TransitionType>(EPlayerState toState,int[] states, bool inverse = false) where TransitionType : ScriptTransition
        {
            AddTransTo<TransitionType>((int)toState, states, inverse);
        }

        public void AddTransTo<TransitionType>(int toState,int[] states,bool inverse = false) where TransitionType : ScriptTransition
        {
            for (int i = 0; i < states.Length; i++)
            { 
                int state = states[i];
                if (!state2transition.TryGetValue(state, out var transList))
                    continue;
                var trans = ScriptTransition<TransitionType>.Get(toState).SetInverse(inverse);
                transList.Add(trans);
            }
        }

        public override bool State_Change(int ePlayerState)
        {
            if (ePlayerState == currentEState)
            {
                states[(int)currentEState].OnEnter(ownEntity);
                return true;
            }
            if (ePlayerState == (int)EPlayerState.None)
                return false;
            if (states[(int)ePlayerState] == null)
                return false;
            if (states[(int)currentEState] != null)
            {
                if (!states[(int)currentEState].CanChange(ePlayerState))
                    return false;
                states[(int)currentEState].OnChange(ePlayerState);
            }
            currentEState = ePlayerState;
            states[(int)currentEState].OnEnter(ownEntity);
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
