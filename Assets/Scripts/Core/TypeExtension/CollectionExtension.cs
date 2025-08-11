using System.Collections.Generic;
using UnityEngine.Pool;

namespace PJR.Core.TypeExtension
{
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
        public static T First<T>(this List<T> self)
        {
            if (self == null)
                return default;
            return self[0];
        }
        public static T Last<T>(this List<T> self)
        {
            if (self == null)
                return default;
            return self[^1];
        }
    }
}