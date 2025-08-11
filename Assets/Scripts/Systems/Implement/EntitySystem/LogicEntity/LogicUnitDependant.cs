using System;
using System.Collections.Generic;
using UnityEngine;

namespace PJR.LogicUnits
{
    public abstract class LogicUnitDependant<ILogicUnitType> where ILogicUnitType : LogicUnit
    {
        public static UnitCompareBySortOrder UnitSorter => unitSorter ??= new UnitCompareBySortOrder();
        static UnitCompareBySortOrder unitSorter = new UnitCompareBySortOrder();

        // 默认初始容量
        private const int DEFAULT_CAPACITY = 8;
        protected List<ILogicUnitType> units;

        protected LogicUnitDependant(int initialCapacity = DEFAULT_CAPACITY)
        {
            units = new List<ILogicUnitType>(initialCapacity);
        }

        protected bool AnyLogicUnits => units != null && units.Count > 0; 
        protected void LogicUnits_OnUpdate(float deltaTime)
        {
            if (!AnyLogicUnits)
                return;
            for (int i = 0; i < units.Count; i++)
            {
                var unit = units[i];
                if (NullCheckAndRemove(ref i))
                    continue;
                unit.OnUpdate(deltaTime);
            }
        }
        protected void LogicUnits_OnLatedUpdate()
        {
            if (!AnyLogicUnits)
                return;
            for (int i = 0; i < units.Count; i++)
            {
                var unit = units[i];
                if (NullCheckAndRemove(ref i))
                    continue;
                unit.OnLatedUpdate();
            }
        }
        protected void LogicUnits_OnDestroy()
        {
            if (!AnyLogicUnits)
                return;
            for (int i = 0; i < units.Count; i++)
            {
                var unit = units[i];
                if (NullCheckAndRemove(ref i))
                    continue;
                unit.OnDestroy();
            }
        }

        private bool NullCheckAndRemove(ref int i)
        {
            var unit = units[i];
            if (unit == null || !unit.Valid)
            {
                units.RemoveAt(i--);
                return true;
            }
            return false;
        }
        public T AddLogicUnit<T>() where T : ILogicUnitType
        {
            var unit = Activator.CreateInstance<T>();
            if (unit == null)
            {
                Debug.LogError($"Failure to create LogicUnit {nameof(T)}");
                return default;
            }
            unit.OnInit(this);
            if (!unit.Valid)
            {
                unit.OnDestroy();
                return default;
            }

            units.Add(unit);
            units.Sort(UnitSorter);
            return unit;
        }

        protected bool FindUnitByType<T>(out T unit) where T : ILogicUnitType => FindUnitByType<T>(out int index, out unit);
        protected bool FindUnitByType<T>(out int index, out T unit) where T : ILogicUnitType
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
            if (!FindUnitByType<T>(out var unit))
            {
                Debug.LogError($"Found not LogicUnit {nameof(T)}");
                return;
            }
            RemoveLoginUnit(unit);
        }
        public void RemoveLoginUnit<T>(T unit) where T : ILogicUnitType
        {
            if(units.Remove(unit))
                unit.OnDestroy();
        }

        public class UnitCompareBySortOrder : IComparer<ILogicUnitType>
        {
            public int Compare(ILogicUnitType a, ILogicUnitType b)
            {
                if (a.SortOrder == b.SortOrder)
                    return 0;
                return a.SortOrder > b.SortOrder ? -1 : 1;//SortOrder越小unit越先被更新
            }
        }
    }
}
