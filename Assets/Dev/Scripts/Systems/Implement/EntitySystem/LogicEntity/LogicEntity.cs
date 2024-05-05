using System;
using System.Collections.Generic;
using PJR.ScriptStates;
using Unity.VisualScripting;
using UnityEngine;

namespace PJR
{
    public abstract class LogicEntity : INumericalControl
    {
        public virtual string entityName { get;}
        public PhysEntity physEntity;
        public GameObject gameObject;
        public Transform transform;
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
        public virtual void OnCreate(EntityContext context) {
            var physEntity = EntitySystem.CreatePhysEntity();
            //
            physEntity.CreateAvatar(this);
            physEntity.logicEntity = this;
            physEntity.physRequire = physRequire;
            physEntity.onAvatarLoadDone += OnAvatarLoadDone;
            physEntity.onDrawGizmos += OnDrawGizmos;

            //
            this.physEntity = physEntity;
            this.gameObject = physEntity.gameObject;
            this.transform = physEntity.transform;
        }
        protected virtual void OnAvatarLoadDone(PhysEntity physEntity) { }

        public virtual void EnterState(int state) { }
        public virtual void OnDrawGizmos() { }
    }
}
