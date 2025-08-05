using InfinityCode.UltimateEditorEnhancer;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PJR.Config;
using UnityEditor;
using UnityEngine;

namespace PJR.Editor
{
    [InitializeOnLoad]
    public class ConfigSystemLeftToolBar
    {
        public static Action OnLeftClick;
        private static GUIContent content;

        static ConfigSystemLeftToolBar()
        {
            ToolbarManager.AddLeftToolbar("ConfigMenu", OnGUI);
        }

        private static void DrawIcon()
        {
            if (content == null)
                content = new GUIContent(LoadIcon("MetaFile Icon"), "ConfigMenu");
            if (GUILayoutUtils.Button(content, InfinityCode.UltimateEditorEnhancer.Styles.appToolbarButtonLeft, GUILayout.Width(32), GUILayout.Height(18)) == ButtonEvent.click)
            {
                Event e = Event.current;

                if (e.button == 0)
                {
                    //if (OnLeftClick != null) OnLeftClick();
                    ShowContextMenu();
                }
                //else if (e.button == 1) ShowContextMenu();
            }
        }

        private static Texture LoadIcon(string name)
        {
            var icon = (Texture)EditorGUIUtility.Load(name);
            if (icon != null)
                icon.filterMode = FilterMode.Bilinear;
            return icon;
        }

        private static void OnGUI()
        {
            DrawIcon();
        }

        private static void ShowContextMenu()
        {
            GenericMenu menu = new GenericMenu();

            foreach (var require in CollectConfigMenuItemRequire())
            {
                require.Add2GenericMenu(menu);
            }

            menu.ShowAsContext();
        }

        private static List<IConfigMenuItemRequire> _shortcutRequire;
        private static bool _hadMenuItemRequire = false;

        public static IEnumerable<IConfigMenuItemRequire> CollectConfigMenuItemRequire()
        {
            _shortcutRequire ??= new List<IConfigMenuItemRequire>();

            if (!_hadMenuItemRequire)
            {
                var menuItemTypes = TypeCache.GetMethodsWithAttribute<RequireConfigMenuItemAttribute>().Where(x => !x.IsAbstract);
                foreach (var type in menuItemTypes)
                {
                    _shortcutRequire.Add(new ConfigMenuItemRequire(type, type.GetAttribute<RequireConfigMenuItemAttribute>()));
                }
                _hadMenuItemRequire = true;
            }

            return _shortcutRequire;
        }

        public interface IConfigMenuItemRequire : IComparable<IConfigMenuItemRequire>
        {
            public void Add2GenericMenu(GenericMenu menu);
            public int Order { get; }
        }

        public struct ConfigMenuItemRequire : IConfigMenuItemRequire 
        {
            private MethodInfo _methodInfo;
            private RequireConfigMenuItemAttribute _requireAttribute;
            public string MenuName => _requireAttribute.MenuName;
            public int Order => _requireAttribute.Order;

            public string FinalMenuName => $"{MenuName}";
            public ConfigMenuItemRequire(MethodInfo methodInfo, RequireConfigMenuItemAttribute requireAttribute)
            {
                _methodInfo = methodInfo;
                _requireAttribute = requireAttribute;
            }
            public void Add2GenericMenu(GenericMenu menu)
            {
                menu.AddItem(FinalMenuName, false, InvoleMethod);
            }
            public void InvoleMethod()
            {
                _methodInfo?.Invoke(null, null);
            }
            public int CompareTo(ConfigMenuItemRequire other)
            {
                return Order.CompareTo(other.Order);
            }

            public int CompareTo(IConfigMenuItemRequire other)
            {
                if (ReferenceEquals(other, null))
                    return -1;
                return Order.CompareTo(other.Order);
            }
        }
    }

    public static class GenericMenuExtension
    {
        public static void AddItem(this GenericMenu menu, string contentStr, bool on, GenericMenu.MenuFunction func)
        {
            menu.AddItem(new GUIContent(contentStr), on, func);
        }
    }
}
