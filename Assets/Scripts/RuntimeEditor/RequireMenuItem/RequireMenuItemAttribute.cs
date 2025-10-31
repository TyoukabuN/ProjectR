#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PJR.Config;
using UnityEditor;
using UnityEngine;

namespace PJR.Editor
{
    public class RequireMenuItemAttribute : Attribute
    {
        public string MenuName { get; }
        public string Tags { get; }
        public int Order { get; }

        public RequireMenuItemAttribute(string menuName , string tags, int order)
        {
            MenuName = menuName;
            Tags = tags;
            Order = order;
        }
        public RequireMenuItemAttribute(string menuName, int order) : this(menuName, String.Empty, order)
        {
        }
        public RequireMenuItemAttribute(string menuName): this(menuName, String.Empty, 0)
        {
        }
    }
    
    public interface IMenuItemRequire : IComparable<IMenuItemRequire>
    {
        public string MenuName { get; }
        public void Add2GenericMenu(GenericMenu menu);
        public void Add2GenericMenu(GenericMenu menu, string menuName);
        public int Order { get; }
    }
    
    public struct ConfigMenuItemRequire : IMenuItemRequire 
    {
        private MethodInfo _methodInfo;
        private RequireMenuItemAttribute _requireAttribute;
        public string MenuName => _requireAttribute.MenuName;
        public int Order => _requireAttribute.Order;

        public string FinalMenuName => $"{MenuName}";
        public ConfigMenuItemRequire(MethodInfo methodInfo, RequireMenuItemAttribute requireAttribute)
        {
            _methodInfo = methodInfo;
            _requireAttribute = requireAttribute;
        }
        public void Add2GenericMenu(GenericMenu menu) => menu.AddItem(new GUIContent(MenuName), false, InvoleMethod);
        public void Add2GenericMenu(GenericMenu menu, string menuName) => menu.AddItem(new GUIContent(menuName), false, InvoleMethod);
        public void InvoleMethod()
        {
            _methodInfo?.Invoke(null, null);
        }
        public int CompareTo(ConfigMenuItemRequire other)
        {
            return Order.CompareTo(other.Order);
        }

        public int CompareTo(IMenuItemRequire other)
        {
            if (ReferenceEquals(other, null))
                return -1;
            return Order.CompareTo(other.Order);
        }
    }

    public static class RequireMenuItemTool
    {
        private static List<IMenuItemRequire> _shortcutRequire;
        
        public static IEnumerable<IMenuItemRequire> CollectConfigMenuItemRequire()
        {
            if (_shortcutRequire == null)
            {
                _shortcutRequire = new List<IMenuItemRequire>();

                var menuItemTypes = 
                    TypeCache.GetMethodsWithAttribute<RequireMenuItemAttribute>()
                        .Where(x => !x.IsAbstract && x.IsStatic);
                
                foreach (var methodInfo in menuItemTypes)
                {
                    _shortcutRequire.Add(new ConfigMenuItemRequire(methodInfo, methodInfo.GetAttribute<RequireMenuItemAttribute>()));
                }
            }
            return _shortcutRequire;
        }

        public static T GetAttribute<T>(this ICustomAttributeProvider member, bool inherit) where T : Attribute
        {
            T[] array = member.GetAttributes<T>(inherit).ToArray<T>();
            return array != null && array.Length != 0 ? array[0] : default (T);
        }

        /// <summary>
        /// Returns the first found non-inherited custom attribute of type T on this member
        /// Returns null if none was found
        /// </summary>
        public static T GetAttribute<T>(this ICustomAttributeProvider member) where T : Attribute
        {
            return member.GetAttribute<T>(false);
        }
        
        public static IEnumerable<T> GetAttributes<T>(
            this ICustomAttributeProvider member,
            bool inherit)
            where T : Attribute
        {
            try
            {
                return member.GetCustomAttributes(typeof (T), inherit).Cast<T>();
            }
            catch
            {
                return (IEnumerable<T>) new T[0];
            }
        }
    }
}
#endif