using NPOI.SS.Formula.Functions;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static PJR.Timeline.Define;
using static PJR.Timeline.Utility;

namespace PJR.Timeline
{
    public static class Global
    {
        public static Clip2ClipHandleFunc Clip2ClipHandleFunc = Default_Clip2ClipHandleFunc;

        static Dictionary<Type, Type> clipType2HandleType = null;
        public static ClipRunner Default_Clip2ClipHandleFunc(IClip clip)
        {
            if (clipType2HandleType == null)
            {
                clipType2HandleType = new Dictionary<Type, Type>();
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                var derivedTypes = assemblies
                .SelectMany(assembly => assembly.GetTypes()) // 获取所有类型
                .Where(type => type.InheritsFrom(typeof(ClipRunner<>)) && !type.IsAbstract) // 筛选继承类，排除抽象类
                .ToList();
                foreach (var _handleType in derivedTypes)
                {
                    var clipType = Utility.GetGenericType(_handleType, typeof(ClipRunner<>));
                    if (clipType == null || clipType2HandleType.ContainsKey(clipType))
                        continue;
                    clipType2HandleType.Add(clipType, _handleType);
                    Debug.Log($"{clipType.Name}  {_handleType.Name}");
                }
            }

            if (!clipType2HandleType.TryGetValue(clip.GetType(), out Type handleType))
                return null;

            return Activator.CreateInstance(handleType, clip) as ClipRunner;
        }


#if UNITY_EDITOR
        static Type[] _clipTypes;
        public static Type[] GetAllClipType()
        {
            if (_clipTypes == null)
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                _clipTypes = assemblies
                .SelectMany(assembly => assembly.GetTypes()) // 获取所有类型
                .Where(type => type.InheritsFrom(typeof(Clip)) && !type.IsAbstract) // 筛选继承类，排除抽象类
                .ToArray();
            }
            return _clipTypes;
        }
        static GenericMenu _trackCreateMenu;
        public static GenericMenu GetTrackCreateMenu(Action<Type> onCreateTrack)
        {
            if (_trackCreateMenu == null)
            {
                var menu = new GenericMenu();
                for (int i = 0; i < GetAllClipType().Length; i++)
                {
                    var clipType = GetAllClipType()[i];
                    menu.AddItem(new GUIContent(clipType.GetNiceName()), false, () => { onCreateTrack?.Invoke(clipType); });
                }
                _trackCreateMenu = menu;
            }
            return _trackCreateMenu;
        }

#endif
    }
}
