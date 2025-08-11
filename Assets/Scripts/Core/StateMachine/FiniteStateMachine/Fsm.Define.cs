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
        public bool CanTransition();
        public void OnTransition(){}
    }
}