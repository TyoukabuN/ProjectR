using System.Collections.Generic;
using PJR.ScriptStates;
using PJR.LogicUnits;
using PJR.Systems;
using UnityEngine;
using System.Linq;

namespace PJR
{
    public abstract partial class LogicEntity : INumericalControl
    {
        public virtual string entityName { get;}
        public PhysEntity physEntity;
        public GameObject gameObject => physEntity?.gameObject;
        public Transform transform => physEntity?.transform;

        public InputHandle inputHandle;
        public EntityContext entityContext;
        public ScriptEntityStateMachine<EntityScriptState> scriptStateMachine;
        public EntityPhysicsConfigAsset physicsConfig;
        public PhysEntityComponentRequire physRequire = PhysEntityComponentRequire.Default;
        public Dictionary<string, ExtraValue> ExtraValueMap { get; set; }

        public virtual void Init() { }
        public virtual void Update() { }
        public virtual void LateUpdate() { }
        public virtual void Destroy() { }
        public virtual bool OnCreate(EntityContext context) {
            physEntity = EntitySystem.CreatePhysEntity();
            if (physEntity == null)
            {
                LogSystem.LogError("Failure to create a PhysEntity", true);
                return false;
            }    
            physEntity.CreateAvatar(this, physRequire, OnAvatarLoadDone, OnDrawGizmos);
            return true;
        }
        protected virtual void OnAvatarLoadDone(PhysEntity physEntity) { }
        public virtual void EnterState(int state) { }
        public virtual void OnDrawGizmos() { }
    }
}
