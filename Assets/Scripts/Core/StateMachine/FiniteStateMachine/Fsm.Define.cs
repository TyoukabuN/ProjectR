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
    public interface IFsm : IReference 
    {
        public IFsmState CurrentState { get; }
        public bool ChangeState(Type stateType);
        public void Update(IUpdateContext updateContext);
        public void Release();
        public bool AddTransition<TFrom, TTo>(IFsmTransition transition);
    }
    public interface IFsmState : IPoolableObject
    {
        public EStatus Status { get; }
    }
    public interface IFsmTransition : IPoolableObject
    {
    }
}