using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJR
{
    public abstract class LogicEntity : IDisposable
    {
        public PhysEntity physEntity;
        public InputHandle inputHandle;
        public StateContext stateContext;
        public EntityStateMachine stateMachine;

        public virtual void Init() { }
        public virtual void Update() { }
        public virtual void Dispose() { }
    }
}
