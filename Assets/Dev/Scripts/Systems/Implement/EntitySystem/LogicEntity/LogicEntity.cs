using System;
using System.Collections.Generic;
using PJR.ScriptStates;
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

        public Dictionary<string, ExtraValue> ExtraValueMap { get; set; }

        public virtual void Init() { }
        public virtual void Update() { }
        public virtual void LateUpdate() { }
        public virtual void Destroy() { }
        public virtual void OnCreate(EntityContext context) { }
        public virtual void EnterState(int state) { }
    }
}
