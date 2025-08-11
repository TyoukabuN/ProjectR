using System;
using System.Collections.Generic;
using System.Linq;
using PJR.Core.Pooling;
using PJR.Core.TypeExtension;
using UnityEngine;
using UnityEngine.Pool;

namespace PJR.Core.StateMachine
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TContext">Fsm里的state,transition都可能用的一个数据引用</typeparam>
    public class Fsm<TContext> : IFsm
    {
        public IFsmState CurrentState => _currentState;
        public Type CurrentStateType => _currentStateType;
        public bool AnyStateNode => (_states?.Count ?? 0) > 0;
        public TContext Context => _context;
        public float CurrentStateTime => TimeInfo.CurrentStateTime;
        public float CurrentUnscaleStateTime => TimeInfo.CurrentUnscaleStateTime;
        
        private List<FsmState<TContext>> _states;
        private FsmState<TContext> _currentState;
        private Type _currentStateType;
        //todo:感觉不能用Type做key
        private Dictionary<Type, FsmState<TContext>> _type2stateMap;
        private TContext _context;
        private EStatus _status;
        public TimeInfo TimeInfo = TimeInfo.Empty;

        public static Fsm<TContext> Get(TContext context, IEnumerable<FsmState<TContext>> states)
        {
            var temp = GenerialPool<Fsm<TContext>>.Get();
            temp.Init(context, states);
            return temp;
        }
        public static Fsm<TContext> Get(TContext context, params FsmState<TContext>[] states)
        {
            var temp = GenerialPool<Fsm<TContext>>.Get();
            temp.Init(context, states);
            return temp;
        }
        private void Init(TContext context, IEnumerable<FsmState<TContext>> states)
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
        public void ConnectStatesWithOnFinish()
        {
            if (_states == null || _states.Count <= 1)
                return;
            for (var i = 0; i < _states.Count - 1; i++)
                AddTransition(_states[i], _states[i + 1], OnFinish<TContext>.Get());
        }
        public void Release() => GenerialPool<Fsm<TContext>>.Release(this);
        private void Reset()
        {
            _status = EStatus.None;
            TimeInfo = TimeInfo.Empty;
            _states = ListPool<FsmState<TContext>>.Get();
            _type2stateMap ??= DictionaryPool<Type, FsmState<TContext>>.Get();
        }
        public void Clear()
        {
            if (_states != null)
            {
                foreach (var state in _states)
                    state?.Release();
            }
            //
            _states.Release();
            _type2stateMap?.Release();
            //
            _states = null;
            _type2stateMap = null;
        }

        public bool AddState(FsmState<TContext> state)
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

        public bool AddAndConnectWithOnFinish(FsmState<TContext> state, OnFinish<TContext> transitionFromLastToThis)
        {
            var lastState = _states.Last();
            if (!AddState(state))
                return false;
            if (lastState == null)
                return false;
            if (!AddTransition(lastState, state, transitionFromLastToThis))
                return false;
            return true;
        }

        public bool AddTransition<TFrom, TTo>(IFsmTransition transition)
            => AddTransition(typeof(TFrom), typeof(TTo), (FsmTransition<TContext>)transition);
        public bool AddTransition(Type fromType , Type toType, FsmTransition<TContext> transition)
        {
            if (transition == null)
                return false;
            if (!TryGetState(fromType, out var fromState))
                return false;
            if (!TryGetState(toType, out var toState))
                return false;
            return AddTransition(fromState, toState, transition);
        }
        public bool AddTransition(FsmState<TContext> fromState , FsmState<TContext> toState, FsmTransition<TContext> transition)
        {
            if (transition == null)
                return false;
            if (!fromState.AddTransition(transition))
                return false;
            transition.OnInit(this, fromState, toState);
            return true;
        }

        public void Update(IUpdateContext updateContext)
        {
            if (_currentState == null)
                return;

            if (CheckTransition(out var willTransitionTo))
                ChangeState(willTransitionTo);
            
            TimeInfo.Update(updateContext);
            
            if(_currentState.IsFinish)
                return;
            _currentState.TimeInfo.Update(updateContext);
            _currentState.OnUpate(updateContext);
        }

        public bool CheckTransition(out FsmState<TContext> toState)
        {
            toState = null;
            if (_currentState == null)
                return false;
            
            using var passedTransitions = PooledList<FsmTransition<TContext>>.Get();
            
            if (CheckTransition_Recursion(_currentState, passedTransitions))
            {
                for (var i = 0; i < passedTransitions.List.Count; i++)
                {
                    var passedTransition = passedTransitions.List[i];
                    passedTransition.OnTransition();
                }
                toState = passedTransitions.Last.To;

                Debug.Log($"发生状态跳转: [{_currentState.GetType().Name}] → [{toState.GetType().Name}]");

                return true;
            }

            return false;
        }
        
        private bool CheckTransition_Recursion(FsmState<TContext> from,List<FsmTransition<TContext>> passedTransitions)
        {
            if (from == null)
                return false;
            foreach (var transition in from.Transitions)
            {
                if(transition == null)
                    continue;
                if(!transition.CanTransition())
                    continue;
                passedTransitions.Add(transition);
                CheckTransition_Recursion(transition.To, passedTransitions);
                return true;
            }
            return false;
        }

        public bool ChangeState<T>() => ChangeState(typeof(T));
        public bool ChangeState(Type stateType)
        {
            if (stateType == null)
                return false;
            if (!_type2stateMap.TryGetValue(stateType, out var targetState))
            {
                Debug.LogWarning($"没有找到对应类型的状态 [{stateType.GetType().Name}]");
                return false;
            }
            return ChangeState(targetState);
        }
        public bool ChangeState(int stateIndex)
            => ChangeState(_states[stateIndex]);
        public bool ChangeState(FsmState<TContext> targetState)
        {
            if (targetState == null)
                return false;
            
            if (_currentState != null)
            {
                if (!_currentState.CanTransition(targetState))
                {
                    Debug.LogWarning($"Can't change state from [{_currentState.GetType().Name}] to [{targetState.GetType().Name}]");
                    return false;
                }
            }

            if (_currentState != null)
            {
                _currentState.OnExit();
                _currentState.TimeInfo = TimeInfo.Empty;
            }
            
            TimeInfo = TimeInfo.Empty;
            _currentState = targetState;
            targetState.TimeInfo = TimeInfo.Empty;
            targetState.OnEnter();
            return true;
        }
        
        
        public TState GetState<TState>() where TState : FsmState<TContext>
        {
            if (!AnyStateNode)
                return null;
            TState state = _states.FirstOrDefault(x => x.GetType() == typeof(TState)) as TState;
            if (state == null)
            {
                Debug.Log($"Found not State with Type: {typeof(TState).Name}");
                return null;
            }
            return state;
        }
        public FsmState<TContext> GetState(Type type)
        {
            if (!AnyStateNode)
                return null;
            var state = _states.FirstOrDefault(x => x.GetType() == type) as FsmState<TContext>;
            if (state == null)
            {
                Debug.Log($"Found not State with Type: {type.Name}");
                return null;
            }
            return state;
        }
        public bool TryGetState<TState>(out FsmState<TContext> ret) where TState : FsmState<TContext>
            => TryGetState(typeof(TState), out ret);
        public bool TryGetState(Type type, out FsmState<TContext> ret)
        {
            ret = GetState(type);
            return ret != null;
        }
    }
}
