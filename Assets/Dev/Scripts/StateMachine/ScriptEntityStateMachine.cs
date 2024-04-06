using System.Collections.Generic;

namespace PJR.ScriptStates
{
    public abstract class ScriptEntityStateMachine : EntityStateMachine
    {
        public ScriptState[] states;
        public EntityContext _stateContext;
        public LogicEntity ownEntity;

        public Dictionary<int, ScriptTransition[]> state2transition;

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
