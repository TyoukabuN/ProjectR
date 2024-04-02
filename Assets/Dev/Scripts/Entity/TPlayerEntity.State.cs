using System.Collections;
using System.Collections.Generic;
using PJR;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PJR
{
    [RequireComponent(typeof(PlayerInput))]
    public partial class TPlayerEntity : StateMachineEntity, INumericalControl, IActionControl
    {
        protected void Awake_State()
        {
            int length = (int)EPlayerState.End - 1;
            states = new State[length];
            state2transition = new Dictionary<int, Transition[]>();


            states[EPlayerState.Stand] = new StandState();
            states[EPlayerState.Walk] = new WalkState();
            state2transition[EPlayerState.Walk] = new Transition[]
            {
                Transition<Trans_OnRunning>.Get(EPlayerState.Running),
            };

            states[EPlayerState.Running] = new RunningState();
            state2transition[EPlayerState.Running] = new Transition[]
            {
                Transition<Trans_OnRunning>.Get(EPlayerState.Walk).SetInverse(true),
            };

            states[EPlayerState.Jump_Begin] = new JumpBeginState();
            state2transition[EPlayerState.Jump_Begin] = new Transition[] 
            {
                Transition<Trans_OnStateFinish>.Get(EPlayerState.Jump_Falling).SetCanExitNormalizeTime(0.95f),
                Transition<Trans_OnGrounded>.Get(EPlayerState.Jump_Land).SetCanExitNormalizeTime(0.5f),
            };


            states[EPlayerState.Jump_Falling] = new JumpFallingState();
            state2transition[EPlayerState.Jump_Falling] = new Transition[]
            {
                Transition<Trans_OnGrounded>.Get(EPlayerState.Jump_Land),
            };

            states[EPlayerState.Jump_Land] = new JumpLandState();
            state2transition[EPlayerState.Jump_Land] = new Transition[] 
            {
                Transition<Trans_OnRunning>.Get(EPlayerState.Running),
                Transition<Trans_OnWalking>.Get(EPlayerState.Walk),
                Transition<Trans_OnStateFinish>.Get(EPlayerState.Stand),

            };

            State_Change(EPlayerState.Stand);
        }
        protected override bool State_Change(int ePlayerState)
        {
            if (ePlayerState == currentEState)
            { 
                states[(int)currentEState].OnEnter(this);
                return true;
            }
            if (ePlayerState == EPlayerState.None)
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
            states[(int)currentEState].OnEnter(this);
            return true;
        }

        protected override void State_UpdateContext()
        {
            stateContext.inputAxi = inputAxi;
            stateContext.mouseDelta = mouseDelta;
            stateContext.runValue = runValue;
            stateContext.grounded = Grounded? 1 : 0;
        }
        // Update is called once per frame
        protected override void Update_State()
        {
            base.Update_State();
        }
    }
}
