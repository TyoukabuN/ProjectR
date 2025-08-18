using System.Collections.Generic;
using System.Linq;
using PJR.Core.TypeExtension;
using UnityEngine.Pool;

namespace PJR.Core.StateMachine
{
    /// <summary>
    /// 给Fsm(FsmState)本地记录运行时间用的结构
    /// </summary>
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
        public List<FsmTransition<TContext>> Transitions => _transitions;
        public Fsm<TContext> fsm => _fsm;
        public TimeInfo TimeInfo = TimeInfo.Empty;
        public EStatus Status
        {
            get => _status;
            protected set => _status = value;
        }
        public float CurrentStateTime => TimeInfo.CurrentStateTime;
        public float CurrentUnscaleStateTime => TimeInfo.CurrentUnscaleStateTime;
        public bool IsRunning => _status == EStatus.Running; 
        public bool IsFinish => _status == EStatus.Finish; 
        public bool IsFailure => _status == EStatus.Failure;
        
        
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
            _transitions ??= ListPool<FsmTransition<TContext>>.Get();
        }
        public virtual void OnUpate(IUpdateContext context){}

        public virtual void OnEnter() => Status = EStatus.Running;
        public virtual void OnExit() => Status = EStatus.None;
        #endregion

        #region Transition
        public virtual bool CanTransition(FsmState<TContext> currentStateType) => true;
        public virtual bool AddTransition(FsmTransition<TContext> transition)
        {
            if (_transitions == null)
                return false;
            var transitionType = transition.GetType();
            // 限制只能存在一个OnFinish
            if (transition.IsUnique && _transitions.Any(x => transitionType.IsInstanceOfType(x)))
                return false;

            _transitions.Add(transition);
            return true;
        }
        #endregion

        public virtual void Clear()
        {
            _transitions?.Release();
            _transitions = null;
            _fsm = null;
            _status = EStatus.None;
        }
        public abstract void Release();
    }
   
    public abstract class SimpleTransitionFsmState<TContext> : FsmState<TContext>
    {
        public bool CanTransitionToSelf => false;

        #region Condition
        public override bool CanTransition(FsmState<TContext> currentStateType)
        {
            if (CanTransitionToSelf)
                return true;
            return !ReferenceEquals(this, currentStateType);
        }
        #endregion

        public abstract override void Release();
    }
}