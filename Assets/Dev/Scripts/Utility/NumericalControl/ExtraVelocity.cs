using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PJR
{

    public class ExtraVelocity: ExtraControl
    {
        public Vector3 force;
        private Vector3 _forceDamped;
        /// <summary>
        /// 衰减
        ///[=-1 ] 时 使用counterNormalize作为衰减系数 linear
        ///[= 0 ] 时;不衰减
        ///[> 0 ] 时;为线性衰减forceDamped -= forceDamped * damp * deltaTime
        /// </summary>
        private float damp = 0f;

        public Vector3 velocity
        {
            get
            {
                if (!IsValid())
                    return Vector3.zero;
                if (damp <= -1)
                    return force * counterNormalize;
                if (damp == 0)
                    return force;
                return _forceDamped;
            }
        }
        public ExtraVelocity(Vector3 force, float duration, float damp):base(duration)
        {
            Init(force, duration, damp);
        }
        public void Init(Vector3 force, float duration, float damp)
        {
            base.Init(duration);
            this.force = force;
            this.damp = damp;
            this._forceDamped = force;
        }
        public override void Reset()
        {
            base.Reset();
            this._forceDamped = force;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            _forceDamped -= _forceDamped * damp * deltaTime;
        }
    }
    public static class ExtraVelocityExtension
    {
        static Dictionary<INumericalControl,List<ExtraVelocity>> m_Dict = new Dictionary<INumericalControl, List<ExtraVelocity>>();
        static Dictionary<INumericalControl, Dictionary<string, ExtraVelocity>> m_MapDict = new Dictionary<INumericalControl, Dictionary<string, ExtraVelocity>>();

        public static List<ExtraVelocity> RegisterExtraVelocity(this INumericalControl obj)
        {
            List<ExtraVelocity> temp;
            if (obj == null)
                return null;
            if (m_Dict.TryGetValue(obj, out temp))
                return temp;
            temp = new List<ExtraVelocity>();
            m_Dict.Add(obj, temp);
            return temp;
        }

        public static void UnregisterExtraVelocity(this INumericalControl obj)
        {
            if (obj == null && !m_Dict.ContainsKey(obj))
                return;
            m_Dict.Remove(obj);
        }

        public static Dictionary<string, ExtraVelocity> RegisterExtraVelocityMap(this INumericalControl obj)
        {
            Dictionary<string, ExtraVelocity> temp;
            if (obj == null)
                return null;
            if (m_MapDict.TryGetValue(obj, out temp))
                return temp;
            temp = new Dictionary<string, ExtraVelocity>();
            m_MapDict.Add(obj, temp);
            return temp;
        }

        public static void UnregisterExtraVelocityMap(this INumericalControl obj)
        {
            if (obj == null && !m_MapDict.ContainsKey(obj))
                return;
            m_MapDict.Remove(obj);
        }

        public static void AddExtraVelocity(this INumericalControl target, Vector3 force, float duration, float damp = -1)
        {
            var repo = RegisterExtraVelocity(target);
            if (repo == null)
                return;
            repo.Add(new ExtraVelocity(force, duration, damp));
        }
        public static ExtraVelocity AddExtraVelocity(this INumericalControl target, string key, Vector3 force, float duration, float damp = -1, bool coverExist = true)
        {
            return target.AddExtraVelocityWithKey(key, force, duration, damp, coverExist);
        }
        public static ExtraVelocity AddExtraVelocityWithKey(this INumericalControl target, string key, Vector3 force, float duration, float damp = -1, bool coverExist = true)
        {
            if (string.IsNullOrEmpty(key))
                return null;
            var repo = RegisterExtraVelocityMap(target);
            if (repo == null)
                return null;

            ExtraVelocity value = null;
            if (!repo.TryGetValue(key, out value))
            {
                value = new ExtraVelocity(force, duration, damp);
                repo.Add(key, value);
            }
            else if (!coverExist)
            {
                duration += value.counter >= 0 ? value.counter : 0;
            }
            value.Init(force, duration, damp);
            return value;
        }

        public static Vector3 UpdateExtraVelocity(this INumericalControl target, float deltaTime, out Vector3 vel)
        {
            vel = UpdateExtraVelocity(target, deltaTime);
            return vel;
        }
        public static Vector3 UpdateExtraVelocity(this INumericalControl target, float deltaTime = 0)
        {
            var list = RegisterExtraVelocity(target);
            var map = RegisterExtraVelocityMap(target);

            Vector3 extendVelocity = Vector3.zero;

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
                    extendVelocity += list[i].velocity;
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
                    if (!string.IsNullOrEmpty(invalidKey))
                        target.ExtendVelocityMapRemove(invalidKey);
                }
                while (!string.IsNullOrEmpty(invalidKey));
                //
                foreach (var item in map)
                {
                    if (!item.Value.IsValid())
                        continue;
                    item.Value.Update(Time.fixedDeltaTime);
                    extendVelocity += item.Value.velocity;
                }
            }

            return extendVelocity;
        }

        public static void ExtendVelocityMapRemove(this INumericalControl target, string key)
        {
            var repo = RegisterExtraVelocityMap(target);
            if (repo == null)
                return;
            ExtraVelocity value = null;
            if (!repo.TryGetValue(key, out value))
                return;
            repo.Remove(key);
        }

        public static bool ExtendVelocityMapExist(this INumericalControl target, string key)
        {
            var repo = RegisterExtraVelocityMap(target);
            if (repo == null)
                return false;
            return repo.ContainsKey(key);
        }
    }
}