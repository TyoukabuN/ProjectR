using System.Collections.Generic;
using PJR.LogicUnits;
using PJR.Systems;
using UnityEngine;
using System.Linq;

namespace PJR
{
    public abstract partial class LogicEntity
    {
        private List<EntityLogicUnit> entityLogicComponents = new List<EntityLogicUnit>(64);

        public EntityLogicUnit AddLogicUnit<T>() where T : EntityLogicUnit
        {
            var unit = System.Activator.CreateInstance<T>();
            if (unit == null)
            {
                Debug.LogError($"Failure to create LogicUnit {nameof(T)}");
                return null;
            }
            entityLogicComponents.Add(unit);
            return unit;
        }
        public EntityLogicUnit GetLogicUnit<T>() where T : EntityLogicUnit
        {
            var unit = entityLogicComponents.FirstOrDefault();
            if (unit == null)
            {
                Debug.LogError($"Found not LogicUnit {nameof(T)}");
                return null;
            }
            return unit;
        }
        public bool TryGetLogicUnit<T>(out EntityLogicUnit unit) where T : EntityLogicUnit
        {
            unit = entityLogicComponents.FirstOrDefault();
            if (unit == null)
            {
                Debug.LogError($"Found not LogicUnit {nameof(T)}");
                return false;
            }
            return true;
        }
        public EntityLogicUnit GetOrAddLogicUnit<T>() where T : EntityLogicUnit
        {
            if (TryGetLogicUnit<T>(out var unit))
                return unit;
            return AddLogicUnit<T>();
        }
    }
}
