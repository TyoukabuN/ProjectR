using System;
using System.Collections.Generic;
using PJR.Core.Pooling;

namespace PJR.Core.StateMachine
{
    public abstract class FSMState<TContext> : IFSMState
    {
        public FSMState()
        {
        }
        #region Phase
        public virtual void OnInit(FSM<TContext> fsm){}
        public virtual void OnUpate(IUpdateContext context,FSM<TContext> fsm){}
        public virtual void OnEnter(FSM<TContext> fsm){}
        public virtual void OnExit(FSM<TContext> fsm){}
        #endregion

        #region Condition

        public virtual bool CanTransition(Type currentStateType) => true;
        #endregion

        public virtual void Clear(){}
        public abstract void Release();
    }
   
    public abstract class FSMStateWhitTransition<TContext> : FSMState<TContext>
    {
        public List<FSMStateTransition<TContext>> Transitions;
        
        public List<FSMStateTransition<TContext>> _transitions;

        public FSMStateWhitTransition()
        {
        }
        #region Phase
        public override void OnInit(FSM<TContext> fsm){}
        public override void OnUpate(IUpdateContext context,FSM<TContext> fsm){}
        public override void OnEnter(FSM<TContext> fsm){}
        public override void OnExit(FSM<TContext> fsm){}
        #endregion

        #region Condition
        public override bool CanTransition(Type currentStateType)
        {
            return true;
        }
        #endregion
        public override void Clear(){}
        public abstract override void Release();
    }
    
    public abstract class SimpleTransitionFSMState<TContext> : FSMState<TContext>
    {
        public virtual bool CanTransitionToSelf => false;
        
        public SimpleTransitionFSMState()
        {
        }
        #region Phase
        public override void OnInit(FSM<TContext> fsm){}
        public override void OnUpate(IUpdateContext context,FSM<TContext> fsm){}
        public override void OnEnter(FSM<TContext> fsm){}
        public override void OnExit(FSM<TContext> fsm){}
        #endregion

        #region Condition
        public override bool CanTransition(Type currentStateType)
        {
            if (CanTransitionToSelf)
                return true;
            return GetType() != currentStateType;
        }
        #endregion

        public override void Clear(){}
        public abstract override void Release();
    }
}