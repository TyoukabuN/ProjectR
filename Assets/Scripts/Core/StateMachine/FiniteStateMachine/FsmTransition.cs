namespace PJR.Core.StateMachine
{
    public abstract class FsmTransition<TContext> : IFsmTransition
    {
        public virtual bool IsUnique => false;
        public Fsm<TContext> Fsm => _fsm;
        public FsmState<TContext> From => _from;
        public FsmState<TContext> To => _to;
        public float CurrentStateTime => _from?.CurrentStateTime ?? 0;
        public float CurrentUnscaleStateTime => _from?.CurrentUnscaleStateTime ?? 0;
        
        private Fsm<TContext> _fsm;
        private FsmState<TContext> _from;
        private FsmState<TContext> _to;

        public abstract bool CanTransition();
        public virtual void OnTransition(){}
        public virtual void OnInit(Fsm<TContext> fsm, FsmState<TContext> from,FsmState<TContext> to)
        {
            _fsm = fsm;
            _from = from;
            _to = to;
        }

        public virtual void Clear()
        {
            _fsm = null;
        }
        public abstract void Release();
    }
}