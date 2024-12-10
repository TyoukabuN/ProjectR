using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

namespace PJR
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
                if(_enabled)
                    OnEnable();
                else
                    OnDisable();
            }
        }
        public virtual int priority => 0;
        public virtual string name => string.Empty;
        public virtual void OnInit() { }
        public virtual void OnEnable() { }
        public virtual void OnDisable() { }
        public virtual void OnUpdate(float deltaTime) { }
        public virtual void OnFixedUpdate() { }
        public virtual void OnLatedUpdate() { }
        public virtual void OnDestroy() { }
    }
}
