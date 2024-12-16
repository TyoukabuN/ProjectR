using UnityEngine;

namespace PJR.LogicUnits
{
    public interface ILogicUnit
    {
        public int SortOrder { get;}
        public bool Enable { get; set; }
        public bool Valid { get;}
        public bool OnInit(System.Object obj);
        public void OnEnable();
        public void OnDisable() { }
        public void OnUpdate() => OnUpdate(Time.deltaTime);
        public void OnUpdate(float deltaTime);
        public void OnLatedUpdate();
        public void OnDestroy();
        public void RemoveSelf();
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
        public virtual int SortOrder => 0;
        public virtual string name => string.Empty;
        public virtual bool OnInit(System.Object obj) { return false; }
        public virtual void OnEnable() { }
        public virtual void OnDisable() { }
        public virtual void OnUpdate(float deltaTime) { }
        public virtual void OnLatedUpdate() { }
        public virtual void OnDestroy() { }
        public virtual void RemoveSelf() { valid = false; }
    }

    public abstract partial class EntityLogicUnit : LogicUnit
    {
        protected LogicEntity logicEntity;
        public LogicEntity LogicEntity => logicEntity;

        public override bool OnInit(System.Object dependency) {
            if (dependency is not PJR.LogicEntity)
            {
                Debug.LogError($"Worry dependency type with {dependency} in {nameof(EntityLogicUnit)}");
                return false;
            }
            logicEntity = (LogicEntity)dependency;
            return true;
        }
        public override void RemoveSelf()
        {
            LogicEntity?.RemoveLoginUnit(this);
        }
    }
}
