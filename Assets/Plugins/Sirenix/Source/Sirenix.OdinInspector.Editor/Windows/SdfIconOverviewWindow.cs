//-----------------------------------------------------------------------
// <copyright file="SdfIconOverviewWindow.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
#pragma warning disable

namespace Sirenix.OdinInspector.Editor.Internal
{
#pragma warning disable

    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.OdinInspector.Editor.Validation;
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.PackageManager.UI;
    using UnityEngine;

    public class SdfIconOverviewWindow : EditorWindow
    {
        [NonSerialized] private SdfIcon[] icons;
        private string prevSearch = "123";
        private string searchFilter;
        private GUIStyle padding;
        private float size = 50;
        private float scrollPos;
        private float scrollMax;
        private Color backColor = new Color(0.1f, 0.1f, 0.1f, 1);
        private Color iconColor = new Color(1, 1, 1, 1);
        private float f;
        private static bool subPixel = true;
        internal Action<SdfIconType> onSelect;
        internal SdfIconType? selected;

        private void OnEnable()
        {
            this.titleContent = new GUIContent("Sdf Icon Overview");
        }

        public static void ShowWindow() => GetWindow<SdfIconOverviewWindow>();

        void OnGUI()
        {
            // Update
            if (Event.current.type == EventType.Layout)
            {
                if (prevSearch != searchFilter || (icons == null || icons.Length == 0))
                {
                    if (string.IsNullOrEmpty(searchFilter))
                    {
                        icons = SdfIcons.AllIcons;

                    }
                    else
                    {
                        icons = SdfIcons.AllIcons.Where(x => FuzzySearch.Contains(searchFilter, x.Name)).ToArray();
                    }

                    prevSearch = searchFilter;
                    padding = padding ?? new GUIStyle() { padding = new RectOffset(20, 20, 10, 10), };
                }
            }

            // Draw filters
            GUILayout.BeginHorizontal(padding);
            {
                GUILayout.BeginVertical();
                searchFilter = EditorGUILayout.TextField("Search", searchFilter);
                size = EditorGUILayout.Slider("Size", size, 10, 128);
                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                backColor = EditorGUILayout.ColorField("Preview back color", backColor);
                iconColor = EditorGUILayout.ColorField("Preview icon color", iconColor);
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();

            this.scrollPos = EditorGUILayout.BeginScrollView(new Vector2(0, this.scrollPos)).y;
            {
                var area = GUILayoutUtility.GetRect(0, this.scrollMax, GUIStyle.none, GUILayoutOptions.ExpandHeight());
                EditorGUI.DrawRect(area, backColor);

                // Draw Icons
                var draw = Event.current.type == EventType.Repaint;
                if (draw || Event.current.type == EventType.MouseDown)
                {
                    var selectedIndex = selected.HasValue ? (int)selected.Value : -1;
                    float yMax = 0f;
                    var padding = 10;
                    var s = size + padding;
                    int num = (int)(area.width / s);
                    var remain = (area.width % s) / num;
                    s += remain;
                    area.width += 1;
                    var mp = Event.current.mousePosition;
                    int mouseOver = -1;
                    Rect mouseOverRect = default;

                    for (int i = 0; i < icons.Length; i++)
                    {
                        var cell = area.SplitGrid(s, s, i);
                        yMax = Math.Max(yMax, cell.bottom);

                        if (cell.Contains(mp))
                        {
                            mouseOver = i;
                            cell = cell.AlignCenter(size + padding * 2, size + padding * 2);
                            mouseOverRect = cell;

                        }
                        else
                        {
                            cell = cell.AlignCenter(size, size);
                        }

                        if (selectedIndex == i)
                        {
                            var selectedBgColor = iconColor;
                            selectedBgColor.a *= 0.3f;
                            GUI.DrawTexture(cell, Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0, selectedBgColor, 0, 5);
                        }

                        if (subPixel)
                        {
                            SdfIcons.DrawIcon(cell.Padding(5), icons[i], iconColor, backColor);
                        }
                        else
                        {
                            SdfIcons.DrawIcon(cell.Padding(5), icons[i], iconColor, backColor);
                        }
                    }

                    this.scrollMax = yMax;

                    if (mouseOver >= 0)
                    {
                        var style = new GUIStyle(SirenixGUIStyles.WhiteLabel);
                        style.fontStyle = FontStyle.Bold;

                        var name = icons[mouseOver].Name;
                        var size = style.CalcSize(new GUIContent(name));
                        var pos = Event.current.mousePosition + new Vector2(30, 5);
                        var rect = new Rect(pos, size);

                        var push = rect.xMax - area.xMax + 20;
                        if (push > 0)
                        {
                            rect.x -= push;
                        }

                        EditorGUI.DrawRect(rect.Expand(10, 5), Color.black);
                        GUI.Label(rect, name, style);

                        if (mouseOverRect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
                        {
                            if (onSelect != null)
                            {
                                EditorApplication.delayCall += () =>
                                {
                                    onSelect((SdfIconType)icons[mouseOver].Index);
                                };
                            }
                        }
                    }
                }
            }
            EditorGUILayout.EndScrollView();

            this.Repaint();

            if (Event.current.type == EventType.MouseDown)
            {
                GUIHelper.RemoveFocusControl();
            }
        }

        public static Rect Split(Rect rect, int index, int length)
        {
            if (length == 1)
                return rect;

            var count = Math.Max(1, (int)(Math.Sqrt(length - 1))) + 1;

            var x = index % count;
            var y = index / count;

            rect.width = rect.width / count;
            rect.height = rect.height / count;
            rect.x = rect.x + x * rect.width;
            rect.y = rect.y + y * rect.height;
            return rect;
        }
    }

    public static class SdfIconSelector
    {
        private static int selectorControlId = -1;
        private static SdfIconType? selectorIcon = null;
        private static EditorWindow window;

        public static SdfIconType SelectIcon(SdfIconType selected, int controlId, bool show)
        {
            if (show)
            {
                window = GUIHelper.CurrentWindow;
                selectorControlId = controlId;
                var overview = EditorWindow.CreateInstance<SdfIconOverviewWindow>();
                overview.ShowAuxWindow();
                overview.selected = selected;
                overview.onSelect = (x) =>
                {
                    if (window)
                    {
                        window.Repaint();
                        selectorIcon = x;
                    }
                    overview.Close();
                };
            }

            if (selectorIcon.HasValue && controlId == selectorControlId)
            {
                EditorGUIUtility.hotControl = controlId;
                window = null;
                GUI.changed = true;
                var val = selectorIcon.Value;
                selectorIcon = null;
                selectorControlId = -1;
                return val;
            }

            return selected;
        }
    }
}
#endif