using System;
using System.Collections.Generic;

namespace PJR.Input
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
        public Flag256 flag;
        private InputKey(int category, string strValue)
        {
            this.category = category;
            this.strValue = strValue;
            flag = FlagDefine.InputFlag.StringToFlag(strValue);

            Cache.CacheWrap(this);
        }
        private InputKey(string strValue, int category = -1) : this(category, strValue) { }

        private InputKey(Type type, string strValue)
        {
            //isEnumType = true;
            this.type = type;
            this.strValue = strValue;

            flag = FlagDefine.InputFlag.StringToFlag(strValue);
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
            if (!Cache.TryGetKey(strValue, out var wrap))
                wrap = new InputKey(strValue, category);

            return wrap;
        }
        public static InputKey Register(Enum enumValue, string strValue)
        {
            return Register((int)Enum.ToObject(enumValue.GetType(),enumValue), strValue);
        }

        public static implicit operator String(InputKey inputKey)
        {
            return inputKey.strValue;
        }
        public static implicit operator Flag256(InputKey inputKey)
        {
            return inputKey.flag;
        }
        public static implicit operator int(InputKey inputKey)
        {
            return inputKey.category;
        }

        public static class Cache
        {
            private static Dictionary<int, Dictionary<int, InputKey>> type2wraps = new Dictionary<int, Dictionary<int, InputKey>>();
            private static Dictionary<string, InputKey> string2wrap = new Dictionary<string, InputKey>();
            public static void CacheWrap(InputKey wrap)
            {
                type2wraps ??= new Dictionary<int, Dictionary<int, InputKey>>();
                string2wrap ??= new Dictionary<string, InputKey>();

                //cache string
                if (!string.IsNullOrEmpty(wrap.strValue))
                {
                    if (string2wrap.ContainsKey(wrap.strValue))
                    { 
                        LogSystem.LogError($"[EntityActionTagWrap][Cache] 重复Cache  [strValue]:{wrap.strValue}");
                    }
                    string2wrap[wrap.strValue] = wrap;
                }
                //gen cache dic
                if (wrap.category >= 0)
                {
                    if (!type2wraps.TryGetValue(wrap.category, out var wraps))
                    {
                        wraps = new Dictionary<int, InputKey>();
                        type2wraps[wrap.category] = wraps;
                    }

                    wraps[wraps.Count] = wrap;
                }
                else
                {
                    LogSystem.LogError($"[EntityActionTagWrap][Cache] 错误category  [category]:{wrap.category}");
                }

            }
            public static bool TryGetKey(string strValue, out InputKey wrap)
            {
                return string2wrap.TryGetValue(strValue, out wrap);
            }
            public static bool TryGetKeys(int category, out Dictionary<int, InputKey> wraps)
            {
                return type2wraps.TryGetValue(category, out wraps);
            }
        }
    }
}