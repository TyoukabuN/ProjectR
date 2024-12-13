using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

namespace PJR.LogicUnits
{
    public abstract class ILogicUnitDependant<ILogicUnitType> where ILogicUnitType : ILogicUnit
    {
        private List<ILogicUnitType> units = new List<ILogicUnitType>(64);

        public T AddLogicUnit<T>() where T : ILogicUnitType
        {
            var unit = System.Activator.CreateInstance<T>();
            if (unit == null)
            {
                Debug.LogError($"Failure to create LogicUnit {nameof(T)}");
                return default;
            }
            unit.OnInit();
            if (!unit.Valid)
            {
                unit.OnDestroy();
                return default;
            }

            units.Add(unit);
            return unit;
        }
        private bool FindUnitByType<T>(out T unit) where T : ILogicUnitType => FindUnitByType<T>(out int index, out unit);
        private bool FindUnitByType<T>(out int index, out T unit) where T : ILogicUnitType
        {
            index = 0;
            unit = default;
            if (units == null || units.Count <= 0)
                return false;
            for (; index < units.Count; index++)
            {
                var temp = units[index];
                if (temp is not T)
                    continue;

                unit = (T)temp;
                if (unit == null)
                    continue;

                return true;
            }
            return false;
        }
        public T GetLogicUnit<T>() where T : ILogicUnitType
        {
            if(!FindUnitByType<T>(out T unit))
            {
                Debug.LogError($"Found not LogicUnit {nameof(T)}");
                return default;
            }
            return unit;
        }
        public bool TryGetLogicUnit<T>(out T unit) where T : ILogicUnitType
        {
            if (!FindUnitByType<T>(out unit))
            {
                Debug.LogError($"Found not LogicUnit {nameof(T)}");
                return default;
            }
            return true;
        }
        public T GetOrAddLogicUnit<T>() where T : ILogicUnitType
        {
            if (TryGetLogicUnit<T>(out var unit))
                return unit;
            return AddLogicUnit<T>();
        }
        public void RemoveLoginUnit<T>() where T : ILogicUnitType
        {
            if (!TryGetLogicUnit<T>(out var unit))
                return;
            unit.OnDestroy();
            units.Remove(unit);
        }
        public void RemoveLoginUnit<T>(T unit) where T : ILogicUnitType
        {
            if(units.Remove(unit))
                unit.OnDestroy();
        }
    }
}
