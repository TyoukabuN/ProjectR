using PJR.Systems;

namespace PJR.ScriptStates
{
    public abstract class EntityScriptState : ScriptState
    {
        public LogicEntity entity;
        public InputHandle inputHandle => entity.inputHandle;
        public EntityScriptState():base() {}
        public virtual void HandleInput(LogicEntity entity) { }
        public virtual void Update(float deltaTime) { }
        public virtual void FixedUpdate() { }

        public virtual bool IsValid()
        {
            return entity != null;
        }
        #region Phase
        public override void OnEnter(LogicEntity entity) { base.OnEnter(this.entity = entity);}
        #endregion

        #region Entity Collision
        public virtual void OnGrounded() { }
        public virtual void OnUpdateVelocity(KCContext context) { }
        public virtual void OnUpdateRotation(KCContext context) {
            PlayerKCCFunc.CommonRotation(context);
        }
        #endregion
    }
}