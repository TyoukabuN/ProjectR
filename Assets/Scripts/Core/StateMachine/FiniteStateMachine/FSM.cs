using System;
using System.Collections.Generic;
using System.Linq;
using PJR.Core.Pooling;
using UnityEngine.EventSystems;
using UnityEngine.Pool;

namespace PJR.Core.StateMachine
{
    public interface IFSM : IReference 
    {
        public IFSMState CurrentState { get; }
        public bool ChangeState(Type stateType);
    }
    public interface IFSMState
    {
        public bool IsFinish { get; }
    }
}

namespace PJR.Core.StateMachine
{
    public class FSM<TUpdateContext> : IFSM , IPoolableObject
    {
        public IFSMState CurrentState => _currentState;
        public bool AnyState => (_states?.Count ?? 0) > 0;
        
        private List<FSMState<TUpdateContext>> _states;
        private FSMState<TUpdateContext> _currentState;

        public FSM()
        {
            _states = ListPool<FSMState<TUpdateContext>>.Get();
        }

        public static FSM<TUpdateContext> Get()
        {
            var temp = GenerialPool<FSM<TUpdateContext>>.Get();
            return temp;
        }
        public void Release() => GenerialPool<FSM<TUpdateContext>>.Release(this);
        
        public void Clear()
        {
            if (_states != null)
            {
                ListPool< FSMState<TUpdateContext>>.Release(_states);
                _states = null;
            }
        }

        public bool ChangeState(Type stateType)
        {
            if (stateType == null)
                return false;
            if (_currentState?.GetType() == stateType)
                return true;

            return true;
        }

        public TState GetState<TState>() where TState : FSMState<TUpdateContext>
        {
            if (!AnyState)
                return null;
            return _states.FirstOrDefault(x => x.GetType() == typeof(TState)) as TState;
        }

    }
    public abstract class FSMState<TUpdateContext> : IFSMState
    {
        public abstract bool IsFinish { get; }
    
    #region Phase
        public abstract void OnInit(TUpdateContext context);
        public abstract void OnUpate(TUpdateContext context);
        public abstract void OnEnter(TUpdateContext context);
        public abstract void OnExit(TUpdateContext context);
        public abstract void OnRelease();
    #endregion

    #region Condition
        public abstract void CanChange();
    #endregion
    }
}
