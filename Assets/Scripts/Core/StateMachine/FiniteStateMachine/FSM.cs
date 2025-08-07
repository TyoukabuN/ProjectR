using System;
using System.Collections.Generic;
using System.Linq;
using PJR.Core.Pooling;
using PJR.Core.TypeExtension;
using UnityEngine;
using UnityEngine.Pool;

namespace PJR.Core.StateMachine
{
    public interface IFSM : IReference 
    {
        public IFSMState CurrentState { get; }
        public bool ChangeState(Type stateType);
    }
    public interface IFSMState : IPoolableObject
    {
    }
}

namespace PJR.Core.TypeExtension
{
    public static class CollectionExtension
    {
        public static void Release<T>(this List<T> self)
        {
            if(self == null)
                return;
            ListPool<T>.Release(self);
        }        
        public static void Release<TKey, TValue>(this Dictionary<TKey, TValue> self)
        {
            if(self == null)
                return;
            DictionaryPool<TKey, TValue>.Release(self);
        }
    }
}

namespace PJR.Core.StateMachine
{
    public class FSM<TContext> : IFSM , IPoolableObject
    {
        public IFSMState CurrentState => _currentState;
        public bool AnyState => (_states?.Count ?? 0) > 0;
        public TContext Context => _context;
        private List<FSMState<TContext>> _states;
        private FSMState<TContext> _currentState;
        private Dictionary<Type, FSMState<TContext>> _type2stateMap;
        private TContext _context;

        public static FSM<TContext> Get(TContext context, IEnumerable<FSMState<TContext>> states)
        {
            var temp = GenerialPool<FSM<TContext>>.Get();
            temp.Init(context, states);
            return temp;
        }
        public static FSM<TContext> Get(TContext context, params FSMState<TContext>[] states)
        {
            var temp = GenerialPool<FSM<TContext>>.Get();
            temp.Init(context, states);
            return temp;
        }
        private void Init(TContext context, IEnumerable<FSMState<TContext>> states)
        {
            Reset();
            _context = context;
            foreach (var state in states)
            {
                state.OnInit(this);
                _states.Add(state);
                _type2stateMap[state.GetType()] = state;
            }
        }
        public bool AddState(FSMState<TContext> state)
        {
            if (state == null)
                return false;
            if (_type2stateMap.Any(x => x.Key == state.GetType()))
            {
                Debug.Log($"Already exist state with same type: {state.GetType().Name}");
                return false;
            }
            
            _states.Add(state);
            _type2stateMap[state.GetType()] = state;

            return true;
        }

        public void Release() => GenerialPool<FSM<TContext>>.Release(this);
        private void Reset()
        {
            _states ??= ListPool<FSMState<TContext>>.Get();
            _type2stateMap ??= DictionaryPool<Type, FSMState<TContext>>.Get();
        }
        public void Clear()
        {
            if (_states != null)
            {
                foreach (var state in _states)
                    state?.Release();
            }
            //
            _states?.Release();
            _type2stateMap?.Release();
            //
            _states = null;
            _type2stateMap = null;
        }

        public void Update(IUpdateContext updateContext)
        {
            if (_currentState == null)
                return;
            _currentState.OnUpate(updateContext, this);
        }

        public bool ChangeState<T>() where T : FSMState<TContext> 
            => ChangeState(typeof(T));
        public bool ChangeState(Type stateType)
        {
            if (stateType == null)
                return false;
            if (!_type2stateMap.TryGetValue(stateType, out var targetState))
                return false;
            if (_currentState != null)
            {
                if (!_currentState.CanTransition(targetState.GetType()))
                {
                    Debug.LogWarning($"Can't change state from [{_currentState.GetType().Name}] to [{targetState.GetType().Name}]");
                    return false;
                }
            }
            _currentState?.OnExit(this);
            _currentState = targetState;
            targetState?.OnEnter(this);
            return true;
        }
        public TState GetState<TState>() where TState : FSMState<TContext>
        {
            if (!AnyState)
                return null;
            TState state = _states.FirstOrDefault(x => x.GetType() == typeof(TState)) as TState;
            if (state == null)
            {
                Debug.Log($"Found not State with Type: {typeof(TState).Name}");
                return null;
            }
            return state;
        }
        public bool TryGetState<TState>(out TState ret) where TState : FSMState<TContext>
        {
            ret = GetState<TState>();
            return ret != null;
        }
    }
}
