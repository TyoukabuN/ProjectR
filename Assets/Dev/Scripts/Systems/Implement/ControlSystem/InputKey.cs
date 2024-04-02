using System;
using System.Collections.Generic;
using UnityEngine;

namespace PJR
{
    [Serializable]
    public partial class InputKey
    {
        public bool IsTypeWrap => type != null;
        //
        public bool isEnumType = false;
        public Enum enumValue = null;
        public Type type = null;
        public string strValue = string.Empty;
        public int intValue = -1;
        public int category = -1;
        //
        public MotionFlag motionFlag;
        private InputKey(int category, string strValue)
        {
            this.strValue = strValue;
            motionFlag = new MotionFlag(strValue);

            Cache.CacheWrap(this);
        }
        private InputKey(string strValue, int category = -1) : this(category, strValue) { }

        private InputKey(Type type, string strValue)
        {
            //isEnumType = true;
            this.type = type;
            this.strValue = strValue;
            motionFlag = new MotionFlag(strValue);
            Cache.CacheWrap(this);
        }
        /// <summary>
        /// 注册wrap
        /// </summary>
        /// <param name="strValue"></param>
        /// <param name="category">不能大于<see cref="Cache.typeInterval"/></param>
        /// <returns></returns>

        public static InputKey Register(int category, string strValue)
        {
            if (!Cache.TryGetWrap(strValue, out var wrap))
                wrap = new InputKey(strValue, category);

            return wrap;
        }
        public static InputKey Register(string strValue, int category = -1) => Register(category, strValue);
        /// <summary>
        /// 注册wrap，兼容旧的ActionSet用
        /// </summary>
        /// <param name="enumType">分类用的类型</param>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public static InputKey Register(Type enumType, string strValue)
        {
            if (!Cache.TryGetWrap(strValue, out var wrap))
                wrap = new InputKey(enumType, strValue);
            return wrap;
        }

        public static class Cache
        {
            /// <summary>
            /// 类型自动申请大类防止重复
            /// </summary>
            private static int typeCount = 0;
            /// <summary>
            /// 大类间隔
            /// </summary>
            private static int typeInterval = 1000000;

            private static Dictionary<Type, int> type2category = new Dictionary<Type, int>();
            private static Dictionary<int, Dictionary<int, InputKey>> type2wraps = new Dictionary<int, Dictionary<int, InputKey>>();
            private static Dictionary<string, InputKey> string2wrap = new Dictionary<string, InputKey>();
            public static void CacheWrap(InputKey wrap)
            {
                type2category ??= new Dictionary<Type, int>();
                type2wraps ??= new Dictionary<int, Dictionary<int, InputKey>>();
                string2wrap ??= new Dictionary<string, InputKey>();

                //cache string
                if (!string.IsNullOrEmpty(wrap.strValue))
                {
                    if (string2wrap.ContainsKey(wrap.strValue))
                    {
                        Debug.LogWarning($"[EntityActionTagWrap][Cache] 重复Cache  [strValue]:{wrap.strValue}");
                    }
                    string2wrap[wrap.strValue] = wrap;
                }

                //type 2 category
                if (wrap.type != null)
                {
                    if (!type2category.TryGetValue(wrap.type, out int category))
                    {
                        type2category[wrap.type] = category = (++typeCount) * typeInterval;
                    }
                    wrap.category = category;
                }

                //gen cache dic
                if (wrap.category > 0)
                {
                    if (!type2wraps.TryGetValue(wrap.category, out var wraps))
                    {
                        wraps = new Dictionary<int, InputKey>();
                        type2wraps[wrap.category] = wraps;
                    }

                    //cache intValue
                    if (wrap.intValue >= 0)
                    {
                        if (wraps.ContainsKey(wrap.intValue))
                        {
                            Debug.LogWarning($"[EntityActionTagWrap][Cache] 重复Cache  [Type]:{wrap.type} [enumValue]:{wrap.enumValue}");
                        }
                        wraps[wrap.intValue] = wrap;
                    }
                    else //other
                    {
                        wraps[wraps.Count] = wrap;
                    }
                }

            }
            public static bool TryGetCategory(Type type, out int category)
            {
                return type2category.TryGetValue(type, out category);
            }
            public static bool TryGetWrap(Type enumType, int enumValue, out InputKey wrap)
            {
                wrap = null;

                if (!type2category.TryGetValue(enumType, out int category))
                    return false;

                if (!type2wraps.TryGetValue(category, out var wraps))
                    return false;

                return wraps.TryGetValue(enumValue, out wrap);
            }
            public static bool TryGetWrap(Enum enumValue, out InputKey wrap)
            {
                return TryGetWrap(enumValue.GetType(), (int)(object)enumValue, out wrap);
            }
            public static bool TryGetWrap(string strValue, out InputKey wrap)
            {
                return string2wrap.TryGetValue(strValue, out wrap);
            }
        }
    }
}