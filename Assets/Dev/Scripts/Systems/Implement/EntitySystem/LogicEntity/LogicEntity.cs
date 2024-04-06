using System;
using PJR.ScriptStates;

namespace PJR
{
    public abstract class LogicEntity : IDisposable
    {
        public PhysEntity physEntity;
        public InputHandle inputHandle;
        public EntityContext entityContext;
        public EntityStateMachine stateMachine;

        public virtual void Init() { }
        public virtual void Update() { }
        public virtual void Dispose() { }
        public virtual void OnCreate(EntityContext context) { }
    }
}
