using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEditor.Build.Content;
using UnityEngine;

namespace PJR.LogicUnits
{
    public abstract partial class LogicUnit
    {
        bool _enabled = false;
        public bool enabled {
            get => _enabled;
            set {
                if (_enabled == value)
                    return;
                _enabled = value;
                if (_enabled)
                    OnEnable();
                else
                    OnDisable();
            }
        }
        protected bool requireUpdate = true;
        protected bool RequireUpdate => requireUpdate;

        public virtual int priority => 0;
        public virtual string name => string.Empty;
        public virtual void OnInit() { }
        public virtual void OnEnable() { }
        public virtual void OnDisable() { }
        public virtual void OnUpdate() => OnUpdate(Time.deltaTime);
        public virtual void OnUpdate(float deltaTime) { }
        public virtual void OnFixedUpdate() { }
        public virtual void OnLatedUpdate() { }
        public virtual void OnDestroy() { }

    }

    public abstract partial class LogicUnit<DependencyType> : LogicUnit
    {
        private DependencyType _dependency;
        public DependencyType Dependency;
        public abstract void OnInit(DependencyType dependency);
    }

    public abstract class EntityLogicUnit : LogicUnit<LogicEntity>
    {
        public LogicEntity LogicEntity => Dependency;
    }
}
