using PJR.Core.Pooling;
using UnityEngine.EventSystems;

namespace PJR.Core.StateMachine
{
    public class OnFinish<TContext> : FsmTransition<TContext>
    {
        public override bool IsUnique => true;
        public override bool CanTransition()
        {
            return From?.IsFinish ?? false;
        }
        public static OnFinish<TContext> Get() => GenerialPool<OnFinish<TContext>>.Get();
        public override void Release() => GenerialPool<OnFinish<TContext>>.Release(this);
    }
}
