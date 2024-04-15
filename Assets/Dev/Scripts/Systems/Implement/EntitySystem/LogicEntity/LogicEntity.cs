using System;
using PJR.ScriptStates;
using UnityEngine;

namespace PJR
{
    public abstract class LogicEntity : INumericalControl
    {
        public PhysEntity physEntity;
        public GameObject gameObject;
        public Transform transform;
        public InputHandle inputHandle;
        public EntityContext entityContext;
        public ScriptEntityStateMachine<EntityScriptState> scriptStateMachine;
        public EntityPhysicsConfigAsset physicsConfig;

        public virtual void Init() { }
        public virtual void Update() { }
        public virtual void LateUpdate() { }
        public virtual void Destroy() { }
        public virtual void OnCreate(EntityContext context) { }
    }
}
