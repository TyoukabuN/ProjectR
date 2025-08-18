using System;
using PJR.Core.Pooling;

namespace PJR.Core.StateMachine
{
    public enum EStatus
    {
        None,
        Running,
        Finish,
        Failure,
        Released,
    }
    public interface IFsm : IPoolableObject 
    {
        public IFsmState CurrentState { get; }
        public bool ChangeState(Type stateType);
        public void Update(IUpdateContext updateContext);
    }

    public interface IFsmState : IPoolableObject
    {
        public EStatus Status { get; }
    }
    public interface IFsmTransition : IPoolableObject
    {
        /// <summary>
        /// 状态只能包含一个这样类型的Transition
        /// </summary>
        public bool IsUnique { get; }
        public bool CanTransition();
        public void OnTransition(){}
    }
}