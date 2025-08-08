using System;
using System.Collections.Generic;
using NPOI.SS.Formula.Functions;
using PJR.Core.Pooling;
using PJR.Core.TypeExtension;
using UnityEngine.Pool;

namespace PJR.Core.StateMachine
{
    public struct TimeInfo
    {
        public static TimeInfo Empty => new()
        {
            CurrentStateTime = 0,
            CurrentUnscaleStateTime = 0,
        };
        public float CurrentStateTime;
        public float CurrentUnscaleStateTime;

        public void Update(float currentStateTime, float currentUnscaleStateTime)
        {
            CurrentStateTime = currentStateTime;
            CurrentUnscaleStateTime = currentUnscaleStateTime;
        }
        public void Update(IUpdateContext updateContext)
        {
            CurrentStateTime += updateContext.DeltaTime;
            CurrentUnscaleStateTime += updateContext.UnscaleDeltaTime;
        }
    }
    public abstract class FsmState<TContext> : IFsmState
    {
        public Dictionary<Type, FsmTransition<TContext>> TransitionMap => _transitionMap;
        public List<FsmTransition<TContext>> Transitions => _transitions;
        public Fsm<TContext> fsm => _fsm;
        public TimeInfo TimeInfo = TimeInfo.Empty;
        public EStatus Status => _status;

        public bool IsRunning => _status == EStatus.Running; 
        public bool IsFinish => _status == EStatus.Finish; 
        public float CurrentStateTime => TimeInfo.CurrentStateTime;
        public float CurrentUnscaleStateTime => TimeInfo.CurrentUnscaleStateTime;
        private Dictionary<Type, FsmTransition<TContext>> _transitionMap;
        private List<FsmTransition<TContext>> _transitions;
        private Fsm<TContext> _fsm;
        private EStatus _status = EStatus.None;

        public FsmState()
        {
        }
        
        #region Phase

        public virtual void OnInit(Fsm<TContext> fsm)
        {
            _fsm = fsm;
            _transitionMap ??= DictionaryPool<Type, FsmTransition<TContext>>.Get();
            _transitions ??= ListPool<FsmTransition<TContext>>.Get();
        }
        public virtual void OnUpate(IUpdateContext context){}
        public virtual void OnEnter(){}
        public virtual void OnExit(){}
        #endregion

        #region Transition
        public virtual bool CanTransition(Type currentStateType) => true;
        public virtual bool AddTransition(Type toType, FsmTransition<TContext> transition)
        {
            if (_transitionMap == null)
                return false;
            if (_transitionMap.TryGetValue(toType, out var exist) && exist != null)
                return false;
            _transitions.Add(transition);
            _transitionMap[toType] = transition;
            return true;
        }
        #endregion

        public virtual void Clear()
        {
            _transitionMap?.Release();
            _transitionMap = null;
            _transitions?.Release();
            _transitions = null;
            _fsm = null;
            _status = EStatus.None;
        }
        public abstract void Release();
    }
   
    public abstract class SimpleTransitionFsmState<TContext> : FsmState<TContext>
    {
        public virtual bool CanTransitionToSelf => false;
        
        public SimpleTransitionFsmState()
        {
        }

        #region Condition
        public override bool CanTransition(Type currentStateType)
        {
            if (CanTransitionToSelf)
                return true;
            return GetType() != currentStateType;
        }
        #endregion

        public abstract override void Release();
    }
}