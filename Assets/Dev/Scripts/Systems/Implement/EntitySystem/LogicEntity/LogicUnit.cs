using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEditor.Build.Content;
using UnityEngine;
using System;

namespace PJR.LogicUnits
{
    public interface ILogicUnit
    {
        public bool Enable { get; set; }
        public bool Valid { get;}
        public void OnInit() { }
        public void OnEnable() { }
        public void OnDisable() { }
        public void OnUpdate() => OnUpdate(Time.deltaTime);
        public void OnUpdate(float deltaTime) { }
        public void OnFixedUpdate() { }
        public void OnLatedUpdate() { }
        public void OnDestroy() { }
        public void RemoveSelf() { }

    }

    public abstract partial class LogicUnit : ILogicUnit
    {
        bool _enabled = false;
        public bool Enable {
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

        protected bool manualUpdate = false;
        public virtual bool ManualUpdate
        { 
            get=> manualUpdate;
            set => manualUpdate = value;
        }


        protected bool valid = true;
        public virtual bool Valid => valid;
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
        public virtual void RemoveSelf() { }

    }

    public abstract partial class LogicUnit<ILogicUnitDependantType> : LogicUnit where ILogicUnitDependantType : ILogicUnitDependant<T>
    {
        public abstract ILogicUnitDependant<LogicUnit<ILogicUnitDependantType>> Dependant { get; }
        public abstract void OnInit(ILogicUnitDependantType dependency);

        public override void RemoveSelf()
        {
            Dependant.RemoveLoginUnit(this);
        }
    }

    public abstract class EntityLogicUnit : LogicUnit<ILogicUnitDependant<EntityLogicUnit>>
    {

    }
}
