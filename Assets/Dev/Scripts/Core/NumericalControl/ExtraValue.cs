using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PJR
{
    public class ExtraValue : ExtraControl
    {
        public object valueRef = null;
        public ExtraValue(object valueRef,float duration) : base(duration) {
            this.valueRef = valueRef;
        }
        public override void Reset()
        {
            base.Reset();
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
        }
    }
    //public abstract class ExtraValue<T> : ExtraValue
    //{
    //    public T valueRef { get; set; }
    //    public ExtraValue(float duration) : base(duration) { }
    //    public override void Reset()
    //    {
    //        base.Reset();
    //    }

    //    public override void Update(float deltaTime)
    //    {
    //        base.Update(deltaTime);
    //    }
    //}
    public static class ExtraValueExtension
    {
        static Dictionary<INumericalControl,List<ExtraValue>> m_Dict = new Dictionary<INumericalControl, List<ExtraValue>>();
        static Dictionary<INumericalControl, Dictionary<string, ExtraValue>> m_MapDict = new Dictionary<INumericalControl, Dictionary<string, ExtraValue>>();

        public static List<ExtraValue> RegisterExtraValue(this INumericalControl host)
        {
            List<ExtraValue> temp;
            if (host == null)
                return null;
            if (m_Dict.TryGetValue(host, out temp))
                return temp;
            temp = new List<ExtraValue>();
            m_Dict.Add(host, temp);
            return temp;
        }

        public static void UnregisterExtraValue(this INumericalControl obj)
        {
            if (obj == null && !m_Dict.ContainsKey(obj))
                return;
            m_Dict.Remove(obj);
        }

        public static Dictionary<string, ExtraValue> RegisterExtraValueMap(this INumericalControl obj)
        {
            Dictionary<string, ExtraValue> temp;
            if (obj == null)
                return null;
            if (m_MapDict.TryGetValue(obj, out temp))
                return temp;
            temp = new Dictionary<string, ExtraValue>();
            obj.ExtraValueMap = temp;
            m_MapDict.Add(obj, temp);
            return temp;
        }

        public static void UnregisterExtraValueMap(this INumericalControl obj)
        {
            if (obj == null && !m_MapDict.ContainsKey(obj))
                return;
            m_MapDict.Remove(obj);
        }

        public static ExtraValue AddExtraValue(this INumericalControl target, string key, object valueRef, float duration = -1)
        {
            return target.AddExtraValueWithKey(key, valueRef, duration);
        }
        public static ExtraValue AddExtraValueWithKey(this INumericalControl target, string key, object valueRef, float duration = -1)
        {
            if (string.IsNullOrEmpty(key))
                return null;
            var repo = RegisterExtraValueMap(target);
            if (repo == null)
                return null;

            if (!repo.TryGetValue(key, out var value))
            {
                value = new ExtraValue(valueRef,duration);
                repo.Add(key, value);
            }
            else
            {
                repo[key].valueRef = valueRef;
                repo[key].duration = duration;
                repo[key].Reset();
            }
            return value;
        }

        public static void UpdateExtraValue(this INumericalControl target,float deltaTime)
        {
            var map = target.ExtraValueMap ?? RegisterExtraValueMap(target);

            if (map != null)
            {
                //clear invaild ExtendSpeedMap item
                for (int i = 0; i < map.Count; i++)
                {
                    var item = map.ElementAt(i);
                    if (item.Value.IsValid())
                    {
                        item.Value.Update(deltaTime);
                        continue;
                    }
                    map.Remove(item.Key);
                    i--;
                }
            }
        }
        public static bool ContainsExtraValue(this INumericalControl target, string key)
        {
            if (target == null || string.IsNullOrEmpty(key))
                return false;
            var repo = target.ExtraValueMap;
            if (repo == null || !repo.TryGetValue(key, out var extraValue))
                return false;
            return true;
        }
        public static bool TryGetExtraValue(this INumericalControl target, string key ,out object value)
        {
            value = null;
            if (target == null || string.IsNullOrEmpty(key))
                return false;
            var repo = target.ExtraValueMap;
            if (repo == null || !repo.TryGetValue(key, out var extraValue))
                return false;
            value = extraValue.valueRef;
            return true;
        }
        public static bool TryGetExtraValue<T>(this INumericalControl target, string key, T defalutValue, out T value)
        {
            value = default(T);
            if (target == null || string.IsNullOrEmpty(key))
                return false;
            var repo = target.ExtraValueMap;
            if (repo == null || !repo.TryGetValue(key, out var extraValue))
                return false;
            if (extraValue.valueRef == null)
                return false;
            if (!(extraValue.valueRef is T))
            {
                LogSystem.LogError($"类型对不上 T:{typeof(T)}  valueRef:{extraValue.GetType()}");
                return false;
            }
            value = (T)extraValue.valueRef;
            return true;
        }

        public static T GetExtraValue<T>(this INumericalControl target, string key, T defalutValue)
        {
            if (target == null || string.IsNullOrEmpty(key))
                return defalutValue;
            var repo = target.ExtraValueMap;
            if (repo == null || !repo.TryGetValue(key, out var extraValue))
                return defalutValue;
            return (T)extraValue.valueRef;
        }

        public static void RemoveExtendValue(this INumericalControl target, string key)
        {
            var repo = RegisterExtraValueMap(target);
            if (repo == null)
                return;
            ExtraValue value = null;
            if (!repo.TryGetValue(key, out value))
                return;
            repo.Remove(key);
        }

    }
}