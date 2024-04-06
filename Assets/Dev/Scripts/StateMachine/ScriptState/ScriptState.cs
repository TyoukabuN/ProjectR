using UnityEngine;

namespace PJR.ScriptStates
{

    public abstract class ScriptState
    {
        public enum Phase
        {
            Running,
            End,
        }
        public Phase phase;

        public LogicEntity entity;

        public InputHandle inputHandle => entity.inputHandle;
        public ScriptState() { OnInit(); }
        public virtual void HandleInput(LogicEntity entity) { }
        public virtual void Update(EntityContext stateContext) { }
        public virtual void FixedUpdate(EntityContext stateContext) { }
        public virtual bool CanChange(int from) { return true; }
        public virtual float normalizeTime { get { return 1f; } }

        #region Phase
        protected void ToPhaseEnd() { phase = Phase.End; }
        public virtual void OnInit() { }
        public virtual void OnEnter(LogicEntity entity) { phase = Phase.Running; this.entity = entity; }
        public virtual void OnUpdate() { }
        public virtual void OnChange(int from) { }
        public virtual void OnFinish() { }
        #endregion

        #region Entity Collision
        public virtual void OnGrounded() { }
        #endregion
    }
}
