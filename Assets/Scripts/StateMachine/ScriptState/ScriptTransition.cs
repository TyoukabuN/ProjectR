using System;
using System.Collections.Generic;
using PJR.Core.Pooling;

namespace PJR.ScriptStates
{
    public abstract class ScriptTransition
    {
        public int toState = -1;
        public bool inverse = false;

        protected ScriptTransition() { }
        protected ScriptTransition(int toState)
        {
            this.toState = toState;
        }
        public virtual bool Check(EntityScriptState state) => true;

        public float canExitNormalizeTime = 0;
        public ScriptTransition SetCanExitNormalizeTime(float canExitNormalizeTime = 0f)
        {
            this.canExitNormalizeTime = canExitNormalizeTime;
            return this;
        }
        public ScriptTransition SetInverse(bool inverse)
        {
            this.inverse = inverse;
            return this;
        }

        public ScriptTransition SetToState(int toState)
        {
            this.toState = toState;
            return this;
        }

        public virtual void OnGet() { }
        public virtual void OnRelease() { }
    }
    public abstract class ScriptTransition<TransitionType> : ScriptTransition where TransitionType : ScriptTransition
    {
        private static Dictionary<Type, Queue<TransitionType>> typeToTransition;
        protected ScriptTransition() { }
        protected ScriptTransition(int toState) : base(toState)
        {
        }
        private static Queue<TransitionType> GetQueue()
        {
            if (typeToTransition == null)
                typeToTransition = new Dictionary<Type, Queue<TransitionType>>();

            Type type = typeof(TransitionType);
            if (!typeToTransition.TryGetValue(typeof(TransitionType), out var queue))
            {
                queue = new Queue<TransitionType>();
                typeToTransition[type] = queue;
            }
            return queue;
        }
        public static TransitionType Get(int toState)
        {
            var list = GetQueue();

            TransitionType transition = null;
            if (list.Count > 0)
            {
                transition = list.Dequeue();
            }
            else
            {
                transition = (TransitionType)Activator.CreateInstance(typeof(TransitionType), true);
                transition.toState = toState;
            }

            transition.OnGet();
            return transition;
        }

        public static bool Release(TransitionType transition)
        {
            if (transition == null)
                return false;
            var list = GetQueue();
            list.Enqueue(transition);

            transition.OnRelease();
            return true;
        }
    }
}