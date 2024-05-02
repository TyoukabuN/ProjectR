using System.Collections.Generic;
using UnityEngine;

namespace PJR
{
    public abstract class ExtraValue : ExtraControl
    {
        public ExtraValue(float duration) : base(duration) { }

        public override void Reset()
        {
            base.Reset();
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
        }
    }   
    public abstract class ExtraValue<T> : ExtraValue
    {
        public virtual T valueRef { get; set; }
        public ExtraValue(float duration) : base(duration) { }
    }
    public static class ExtraValueExtension
    {
        static Dictionary<INumericalControl,List<ExtraValue>> m_Dict = new Dictionary<INumericalControl, List<ExtraValue>>();
        static Dictionary<INumericalControl, Dictionary<string, ExtraValue>> m_MapDict = new Dictionary<INumericalControl, Dictionary<string, ExtraValue>>();

        public static List<ExtraValue> RegisterExtraValue(this INumericalControl obj)
        {
            List<ExtraValue> temp;
            if (obj == null)
                return null;
            if (m_Dict.TryGetValue(obj, out temp))
                return temp;
            temp = new List<ExtraValue>();
            m_Dict.Add(obj, temp);
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
            m_MapDict.Add(obj, temp);
            return temp;
        }

        public static void UnregisterExtraValueMap(this INumericalControl obj)
        {
            if (obj == null && !m_MapDict.ContainsKey(obj))
                return;
            m_MapDict.Remove(obj);
        }

        public static void AddExtraValue(this INumericalControl target, ExtraValue extraValue)
        {
            var repo = RegisterExtraValue(target);
            if (repo == null)
                return;
            repo.Add(extraValue);
        }
        public static ExtraValue AddExtraValue(this INumericalControl target, string key, ExtraValue extraValue)
        {
            return target.AddExtraValueWithKey(key, extraValue);
        }
        public static ExtraValue AddExtraValueWithKey(this INumericalControl target, string key, ExtraValue extraValue)
        {
            if (string.IsNullOrEmpty(key))
                return null;
            var repo = RegisterExtraValueMap(target);
            if (repo == null)
                return null;

            ExtraValue value = null;
            if (!repo.TryGetValue(key, out value))
            {
                value = extraValue;
                repo.Add(key, value);
            }
            else
            {
                repo[key] = extraValue;
            }
            return value;
        }

        public static Vector3 UpdateExtraValue(this INumericalControl target, float deltaTime, out Vector3 vel)
        {
            vel = UpdateExtraValue(target, deltaTime);
            return vel;
        }
        public static Vector3 UpdateExtraValue(this INumericalControl target, float deltaTime = 0)
        {
            var list = RegisterExtraValue(target);
            var map = RegisterExtraValueMap(target);

            Vector3 extendValue = Vector3.zero;

            if (list != null)
            { 
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].Update(deltaTime);
                    if (!list[i].IsValid())
                    {
                        list.RemoveAt(i--);
                        continue;
                    }
                    //extendValue += list[i].velocity;
                }
            }

            if (map != null)
            {
                //clear invaild ExtendSpeedMap item
                string invalidKey = string.Empty;
                do
                {
                    invalidKey = string.Empty;
                    foreach (var item in map)
                    {
                        if (item.Value.IsValid())
                            continue;
                        invalidKey = item.Key;
                        break;
                    }
                    //if (!string.IsNullOrEmpty(invalidKey))
                    //    target.ExtendValueMapRemove(invalidKey);
                }
                while (!string.IsNullOrEmpty(invalidKey));
                //
                foreach (var item in map)
                {
                    if (!item.Value.IsValid())
                        continue;
                    item.Value.Update(Time.fixedDeltaTime);
                    //extendValue += item.Value.velocity;
                }
            }

            return extendValue;
        }

        public static void ExtendValueMapRemove(this INumericalControl target, string key)
        {
            var repo = RegisterExtraValueMap(target);
            if (repo == null)
                return;
            ExtraValue value = null;
            if (!repo.TryGetValue(key, out value))
                return;
            repo.Remove(key);
        }

        public static bool ExtendValueMapExist(this INumericalControl target, string key)
        {
            var repo = RegisterExtraValueMap(target);
            if (repo == null)
                return false;
            return repo.ContainsKey(key);
        }
    }
}