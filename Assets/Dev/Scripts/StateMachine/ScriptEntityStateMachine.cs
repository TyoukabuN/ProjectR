using PJR.Systems.Input;
using PJR.ScriptStates.Player;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PJR.ScriptStates
{
    public abstract class ScriptEntityStateMachine<TEntityScriptState> : EntityStateMachine where TEntityScriptState : EntityScriptState
    {
        public virtual EntityScriptState CurrentState { 
            get
            {
                if (states == null)
                    return null;
                return states[CurrentEState];
            }
        }

        public TEntityScriptState[] states;
        public EntityContext _stateContext;
        public LogicEntity ownEntity;

        public Dictionary<int, List<ScriptTransition>> state2transition;

        public ScriptEntityStateMachine(LogicEntity entity) { 
            this.ownEntity = entity;    
        }


        public EntityContext stateContext
        {
            get
            {
                if (_stateContext == null)
                    _stateContext = new EntityContext();
                return _stateContext;
            }
        }
        public override bool State_Change(int ePlayerState) { return true; }

        public override void Update(float deltaTime)
        {
            UpdateContext();
            if (CurrentEState != 0)
            {
                var state = states[CurrentEState];
                if (state != null)
                {
                    state.Update(deltaTime);
                    if (CheckTransition(CurrentEState, out int toState))
                    {
                        State_Change(toState);
                    }
                }
            }
        }
        public bool CheckTransition(int eState, out int toState)
        {
            toState = 0;
            if (!state2transition.TryGetValue(eState, out var transitions))
                return false;
            for (int i = 0; i < transitions.Count; i++)
            {
                var state = states[eState];
                if (state == null || !state.IsValid())
                    continue;
                if (transitions[i].Check(state))
                {
                    //TODO:加个log的开关
                    //LogSystem.Log(transitions[i].ToString());
                    toState = transitions[i].toState;
                    if (states[toState] == null)
                        continue;
                    //return CheckTransition(toState, out toState);
                    return true;
                }
            }
            return false;
        }
    }
}
