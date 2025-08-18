using System.Collections.Generic;
using System.Linq;
using UnityEngine.Pool;

namespace PJR.Core.TypeExtension
{
    /// <summary>
    /// System集合类的扩展方法
    /// </summary>
    public static class CollectionExtension
    {
        public static void Release<T>(this List<T> self)
        {
            if(self == null)
                return;
            ListPool<T>.Release(self);
        }        
        public static void Release<TKey, TValue>(this Dictionary<TKey, TValue> self)
        {
            if(self == null)
                return;
            DictionaryPool<TKey, TValue>.Release(self);
        }
        public static bool AnyItem<T>(this List<T> self)
        {
            if (self == null)
                return false;
            return self.Count > 0;
        }
        public static bool WithinRange<T>(this List<T> self, int index)
        {
            if (self == null)
                return false;
            return index >= 0 && index < self.Count;
        }
        public static T First<T>(this List<T> self) 
            => self.AnyItem() ? self[0] : default;
        public static T Last<T>(this List<T> self) 
            => self.AnyItem() ? self[0] : default;
    }
}