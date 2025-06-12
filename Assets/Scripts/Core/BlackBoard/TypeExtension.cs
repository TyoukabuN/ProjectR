using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace PJR.BlackBoard
{
    public static class TypeExtension
    {
        private static readonly Dictionary<Type, MethodInfo> MethodCache = new Dictionary<Type, MethodInfo>();

        public static IEnumerable<Type> GetFiTypeFilter(this object obj)
        {
            if (obj is Type type)
            {
                if (!MethodCache.TryGetValue(type, out var method))
                {
                    method = type.GetMethod("GetFiTypeFilter", BindingFlags.Static | BindingFlags.Public);
                    if (method != null && method.IsStatic && method.ReturnType == typeof(IEnumerable<Type>))
                    {
                        MethodCache[type] = method;
                    }
                }

                if (method != null)
                {
                    return (IEnumerable<Type>)method.Invoke(null, null);
                }
    
            }
            return Enumerable.Empty<Type>();
        }
    }
}


