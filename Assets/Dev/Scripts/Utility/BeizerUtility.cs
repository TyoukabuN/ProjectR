using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJR
{
    public static class BeizerUtility
    {
        private static Vector3[] vec3Cache = new Vector3[100];

        static BeizerUtility()
        {
            if (vec3Cache == null || vec3Cache.Length <= 0)
                vec3Cache = new Vector3[100];
        }

        public static List<Vector3> SampleBeizerCurve(List<Vector3> plist, int step)
        {
            if (plist.Count <= 1)
                return null;
            var temp = new List<Vector3>();
            for (int i = 0; i < step + 1; i++)
            {
                float t = (float)i / step;
                Vector3 point;
                if (t <= 0)
                    point = plist[0];
                else if (t >= 1)
                    point = plist[plist.Count - 1];
                else
                    point = DegNBeizer(t, plist.ToArray(), plist.Count);
                temp.Add(point);
            }
            return temp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="plist"></param>
        /// <param name="count">用来减少list的创建</param>
        /// <returns></returns>
        public static Vector3 DegNBeizer(float t, Vector3[] plist, int count)
        {
            if (count == 2)
                return LinearBeizer(plist[0], plist[1], t);
            int index = 0;
            for (int i = 0; i < count - 1; i++, index++)
            {
                vec3Cache[index] = LinearBeizer(plist[i], plist[i + 1], t);
            }
            return DegNBeizer(t, vec3Cache, index);
        }
        public static Vector3 LinearBeizer(Vector3 p0, Vector3 p1, float t)
        {
            return (1 - t) * p0 + t * p1;
        }
    }
}