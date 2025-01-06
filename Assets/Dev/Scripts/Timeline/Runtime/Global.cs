using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static PJR.Timeline.Define;
using static PJR.Timeline.Utility;

namespace PJR.Timeline
{
    public static class Global
    {
        public static Clip2ClipHandleFunc Clip2ClipHandleFunc = Default_Clip2ClipHandleFunc;

        public static ClipHandle Default_Clip2ClipHandleFunc(Clip clip)
        {
            if (clipType2HandleType == null)
            {
                clipType2HandleType = new Dictionary<Type, Type>();
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                var derivedTypes = assemblies
                .SelectMany(assembly => assembly.GetTypes()) // 获取所有类型
                .Where(type => type.InheritsFrom(typeof(ClipHandle<>)) && !type.IsAbstract) // 筛选继承类，排除抽象类
                .ToList();
                foreach (var _handleType in derivedTypes)
                {
                    var clipType = Utility.GetGenericType(_handleType, typeof(ClipHandle<>));
                    if (clipType == null || clipType2HandleType.ContainsKey(clipType))
                        continue;
                    clipType2HandleType.Add(clipType, _handleType);
                    Debug.Log($"{clipType.Name}  {_handleType.Name}");
                }
            }

            if (!clipType2HandleType.TryGetValue(clip.GetType(), out Type handleType))
                return null;

            return Activator.CreateInstance(handleType, clip) as ClipHandle;
        }

        static Dictionary<Type, Type> clipType2HandleType = null;
    }
}
