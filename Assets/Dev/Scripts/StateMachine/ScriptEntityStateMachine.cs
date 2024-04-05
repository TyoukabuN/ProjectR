using PJR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJR
{
    public abstract class ScriptEntityStateMachine : EntityStateMachine
    {
        public State[] states;
        public StateContext _stateContext;

        public Dictionary<int, Transition[]> state2transition;

        public StateContext stateContext
        {
            get
            {
                if (_stateContext == null)
                    _stateContext = new StateContext();
                return _stateContext;
            }
        }
        public override bool State_Change(int ePlayerState) { return true; }

        public override void Update()
        {
            UpdateContext();
            if (CurrentState != 0)
            {
                var state = states[CurrentState];
                if (state != null)
                {
                    state.Update(stateContext);
                    if (CheckTransition(CurrentState, out int toState))
                    {
                        State_Change(toState);
                    }
                }
            }
        }
        public override void UpdateContext() { }
        public bool CheckTransition(int state, out int toState)
        {
            toState = 0;
            if (!state2transition.TryGetValue(state, out var transitions))
                return false;
            for (int i = 0; i < transitions.Length; i++)
            {
                if (transitions[i].Check(states[state]))
                {
                    toState = transitions[i].toState;
                    if (states[toState] == null)
                        continue;
                    return true;
                }
            }
            return false;
        }
    }
}
