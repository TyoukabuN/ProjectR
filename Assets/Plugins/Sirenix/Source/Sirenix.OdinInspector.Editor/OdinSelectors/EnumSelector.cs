//-----------------------------------------------------------------------
// <copyright file="EnumSelector.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor
{
#pragma warning disable

    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
    using UnityEditor;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// A feature-rich enum selector with support for flag enums.
    /// </summary>
    /// <example>
    /// <code>
    /// KeyCode someEnumValue;
    ///
    /// [OnInspectorGUI]
    /// void OnInspectorGUI()
    /// {
    ///     // Use the selector manually. See the documentation for OdinSelector for more information.
    ///     if (GUILayout.Button("Open Enum Selector"))
    ///     {
    ///         EnumSelector&lt;KeyCode&gt; selector = new EnumSelector&lt;KeyCode&gt;();
    ///         selector.SetSelection(this.someEnumValue);
    ///         selector.SelectionConfirmed += selection =&gt; this.someEnumValue = selection.FirstOrDefault();
    ///         selector.ShowInPopup(); // Returns the Odin Editor Window instance, in case you want to mess around with that as well.
    ///     }
    ///
    ///     // Draw an enum dropdown field which uses the EnumSelector popup:
    ///     this.someEnumValue = EnumSelector&lt;KeyCode&gt;.DrawEnumField(new GUIContent("My Label"), this.someEnumValue);
    /// }
    ///
    /// // All Odin Selectors can be rendered anywhere with Odin. This includes the EnumSelector.
    /// EnumSelector&lt;KeyCode&gt; inlineSelector;
    ///
    /// [ShowInInspector]
    /// EnumSelector&lt;KeyCode&gt; InlineSelector
    /// {
    ///     get { return this.inlineSelector ?? (this.inlineSelector = new EnumSelector&lt;KeyCode&gt;()); }
    ///     set { }
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="OdinSelector{T}"/>
    /// <seealso cref="TypeSelector"/>
    /// <seealso cref="GenericSelector{T}"/>
    /// <seealso cref="OdinMenuTree"/>
    /// <seealso cref="OdinEditorWindow"/>
    public class EnumSelector<T> : OdinSelector<T>
    {
        private static readonly StringBuilder SB = new StringBuilder();
        private static readonly Func<T, T, bool> EqualityComparer = PropertyValueEntry<T>.EqualityComparer;

        private static Color highlightLineColor = EditorGUIUtility.isProSkin ? new Color(0.5f, 1f, 0, 1f) : new Color(0.015f, 0.68f, 0.015f, 1f);
        private static Color selectedMaskBgColor = EditorGUIUtility.isProSkin ? new Color(0.5f, 1f, 0, 0.1f) : new Color(0.02f, 0.537f, 0, 0.31f);
        private static readonly string title = typeof(T).Name.SplitPascalCase();
        private float maxEnumLabelWidth = 0;
        private ulong curentValue;
        private ulong curentMouseOverValue;
        
        public static bool DrawSearchToolbar = true;

        #region Modified By Hunter (jwaybee) -- Thursday, January 13, 2022

        private static string longestPath;
        private static int menuDepth;

        #endregion
        

        static EnumSelector()
        {
            if (typeof(T).IsEnum)
            {
                #region Modified By Hunter (HB)
                var deta = typeof(T).GetAttribute<DynaEnumTextAttribute>();
                #endregion
                
                var fields = typeof(T).GetFields(Flags.StaticPublicDeclaredOnly);
                int maxDepth = 1;
                foreach (var field in fields)
                {
                    try
                    {
                        var obs = field.GetAttribute<ObsoleteAttribute>(true);
                        var lblText = field.GetAttribute<LabelTextAttribute>(true);
                        var msg = field.GetAttribute<InfoBoxAttribute>(true);
                        var hide = field.GetAttribute<HideInInspector>();

                        EnumTypeUtilities<T>.EnumMember val = new EnumTypeUtilities<T>.EnumMember();
                        val.Value = (T)Enum.Parse(typeof(T), field.Name);
                        val.Name = field.Name;
                        val.NiceName = val.Name.SplitPascalCase();
                        val.IsObsolete = obs != null;
                        val.Message = obs == null ? "" : obs.Message;
                        val.Hide = hide != null;

                        #region Modified By Hunter (jb) -- 2022年1月13日

                        val.Path = "";
                        
                        if (!EnumTypeUtilities<T>.IsFlagEnum)
                        {
                            if (typeof(T).GetField(field.Name).GetCustomAttribute(typeof(HunterSelectionPathAttribute), false) is HunterSelectionPathAttribute pathAttr)
                            {
                                var path = pathAttr.path;
                                if (!string.IsNullOrWhiteSpace(path))
                                {
                                    var s = path.Split('/');
                                    if (s.Length > 0)
                                    {
                                        path = "";
                                        maxDepth = Mathf.Max(s.Length, maxDepth);
                                        foreach (var ss in s)
                                        {
                                            path += ss + "/";
                                        }
                                        
                                        val.Path = path;
                                    }    
                                }
                            }                
                        }

                        #endregion

                        if (lblText != null)
                        {
                            val.NiceName = lblText.Text ?? "";

                            if (lblText.NicifyText)
                            {
                                val.NiceName = ObjectNames.NicifyVariableName(val.NiceName);
                            }
                        }
                        
                        #region Modified By Hunter (HB) 
                        if (deta != null)
                        {
                            if (DynaEnumTextAttribute.rs != null && DynaEnumTextAttribute.rs.ContainsKey(deta.resolverType))
                            {
                                val.NiceName = DynaEnumTextAttribute.rs[deta.resolverType].Resolve(lblText.Text);
                            }
                        }
                        #endregion
                    }
                    catch { continue; }

                    menuDepth = maxDepth;
                }
            }
        }

        /// <summary>
        /// By default, the enum type will be drawn as the title for the selector. No title will be drawn if the string is null or empty.
        /// </summary>
        public override string Title
        {
            get
            {
                if (GeneralDrawerConfig.Instance.DrawEnumTypeTitle)
                {
                    return title;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is flag enum.
        /// </summary>
        public bool IsFlagEnum { get { return EnumTypeUtilities<T>.IsFlagEnum; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumSelector{T}"/> class.
        /// </summary>
        public EnumSelector()
        {
            if (!typeof(T).IsEnum)
            {
                throw new NotSupportedException(typeof(T).GetNiceFullName() + " is not an enum type.");
            }

            if (Event.current != null)
            {
                foreach (var item in Enum.GetNames(typeof(T)))
                {
                    maxEnumLabelWidth = Mathf.Max(maxEnumLabelWidth, SirenixGUIStyles.Label.CalcSize(new GUIContent(item)).x);
                }

                if (this.Title != null)
                {
                    var titleAndSearch = Title + "                      ";
                    maxEnumLabelWidth = Mathf.Max(maxEnumLabelWidth, SirenixGUIStyles.Label.CalcSize(new GUIContent(titleAndSearch)).x);
                }
            }
        }

        /// <summary>
        /// Populates the tree with all enum values.
        /// </summary>
        protected override void BuildSelectionTree(OdinMenuTree tree)
        {
            tree.Selection.SupportsMultiSelect = IsFlagEnum;
            tree.Config.DrawSearchToolbar = DrawSearchToolbar;
            var enumVals = EnumTypeUtilities<T>.AllEnumMemberInfos;
            foreach (var item in enumVals)
            {
                if (item.Hide) continue;
                tree.Add(item.NiceName, item);
            }

            //tree.AddRange(enumValues, x => Enum.GetName(typeof(T), x).SplitPascalCase());

            if (IsFlagEnum)
            {
                tree.DefaultMenuStyle.Offset += 15;
                if (!enumVals.Where(x => x.Value != null).Select(x => Convert.ToInt64(x.Value)).Contains(0))
                {
                    tree.MenuItems.Insert(0, new OdinMenuItem(tree, GetNoneValueString(), new EnumTypeUtilities<T>.EnumMember()
                    {
                        Value = GetZeroValue(),
                        Name = "None",
                        NiceName = "None",
                        IsObsolete = false,
                        Message = ""
                    }));
                }
                tree.EnumerateTree().ForEach(x => x.OnDrawItem += DrawEnumFlagItem);
                this.DrawConfirmSelectionButton = false;
            }
            else
            {
                #region Modified By Hunter (jwaybee) -- Thursday, January 13, 2022

                if (menuDepth > 1)
                {
                    tree.DefaultMenuStyle.Offset += 10;
                }

                #endregion
                tree.EnumerateTree().ForEach(x => x.OnDrawItem += DrawEnumItem);
            }

            tree.EnumerateTree().ForEach(x => x.OnDrawItem += DrawEnumInfo);
        }

        private static T GetZeroValue()
        {
            var backingType = Enum.GetUnderlyingType(typeof(T));

            // Yes, this is insane. Yes, C# makes us do this.
            object backingZero = Convert.ChangeType(0, backingType);
            return (T)backingZero;
        }

        private void DrawEnumInfo(OdinMenuItem obj)
        {
            if (!(obj.Value is EnumTypeUtilities<T>.EnumMember))
            {
                return;
            }

            var member = (EnumTypeUtilities<T>.EnumMember)obj.Value;
            var hasMessage = !string.IsNullOrEmpty(member.Message);

            if (member.IsObsolete)
            {
                var rect = obj.Rect.Padding(5, 3).AlignRight(16).AlignCenterY(16);
                GUI.DrawTexture(rect, EditorIcons.TestInconclusive);
            }
            else if (hasMessage)
            {
                var rect = obj.Rect.Padding(5, 3).AlignRight(16).AlignCenterY(16);
                GUI.DrawTexture(rect, EditorIcons.ConsoleInfoIcon);
            }

            if (hasMessage)
            {
                GUI.Label(obj.Rect, new GUIContent("", member.Message));
            }
        }

        private bool wasMouseDown = false;

        private void DrawEnumItem(OdinMenuItem obj)
        {
            
            if (Event.current.type == EventType.MouseDown && obj.Rect.Contains(Event.current.mousePosition))
            {
                obj.Select();
                Event.current.Use();
                wasMouseDown = true;
            }

            if (wasMouseDown)
            {
                GUIHelper.RequestRepaint();
            }

            if (wasMouseDown == true && Event.current.type == EventType.MouseDrag && obj.Rect.Contains(Event.current.mousePosition))
            {
                obj.Select();
            }

            if (Event.current.type == EventType.MouseUp)
            {
                wasMouseDown = false;
                #region Modified By Hunter (jwaybee) -- Thursday, January 13, 2022

                if (obj.IsSelected && obj.Rect.Contains(Event.current.mousePosition) && obj.ChildMenuItems.Count<=0)
                //if (obj.IsSelected && obj.Rect.Contains(Event.current.mousePosition))
                {
                    obj.MenuTree.Selection.ConfirmSelection();
                }
                #endregion
                
            }
        }

        [OnInspectorGUI, PropertyOrder(-1000)]
        private void SpaceToggleEnumFlag()
        {
            if (this.SelectionTree != OdinMenuTree.ActiveMenuTree)
            {
                return;
            }

            if (IsFlagEnum && Event.current.keyCode == KeyCode.Space && Event.current.type == EventType.KeyDown && this.SelectionTree != null)
            {
                foreach (var item in this.SelectionTree.Selection)
                {
                    this.ToggleEnumFlag(item);
                }

                this.TriggerSelectionChanged();

                Event.current.Use();
            }
        }

        /// <summary>
        /// When ShowInPopup is called, without a specified window width, this method gets called.
        /// Here you can calculate and give a good default width for the popup.
        /// The default implementation returns 0, which will let the popup window determine the width itself. This is usually a fixed value.
        /// </summary>
        protected override float DefaultWindowWidth()
        {
            #region Modified By Hunter (jwaybee) -- Thursday, January 13, 2022

            if (menuDepth > 1) return 400;

            #endregion
            return Mathf.Clamp(maxEnumLabelWidth + 50, 160, 400);
        }

        private void DrawEnumFlagItem(OdinMenuItem obj)
        {
            if ((Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseUp) && obj.Rect.Contains(Event.current.mousePosition))
            {
                if (Event.current.type == EventType.MouseDown)
                {
                    ToggleEnumFlag(obj);

                    this.TriggerSelectionChanged();
                }
                Event.current.Use();
            }

            if (Event.current.type == EventType.Repaint)
            {
                var val = (ulong)Convert.ToInt64(GetMenuItemEnumValue(obj));
                var isPowerOfTwo = (val & (val - 1)) == 0;

                if (val != 0 && !isPowerOfTwo)
                {
                    var isMouseOver = obj.Rect.Contains(Event.current.mousePosition);
                    if (isMouseOver)
                    {
                        curentMouseOverValue = val;
                    }
                    else if (val == curentMouseOverValue)
                    {
                        curentMouseOverValue = 0;
                    }
                }

                var chked = (val & this.curentValue) == val && !((val == 0 && this.curentValue != 0));
                var highlight = val != 0 && isPowerOfTwo && (val & this.curentMouseOverValue) == val && !((val == 0 && this.curentMouseOverValue != 0));

                if (highlight)
                {
                    EditorGUI.DrawRect(obj.Rect.AlignLeft(6).Padding(2), highlightLineColor);
                }

                if (chked || isPowerOfTwo)
                {
                    var rect = obj.Rect.AlignLeft(30).AlignCenter(EditorIcons.TestPassed.width, EditorIcons.TestPassed.height);
                    if (chked)
                    {
                        if (isPowerOfTwo)
                        {
                            if (!EditorGUIUtility.isProSkin)
                            {
                                var tmp = GUI.color;
                                GUI.color = new Color(1, 0.7f, 1, 1);
                                GUI.DrawTexture(rect, EditorIcons.TestPassed);
                                GUI.color = tmp;
                            }
                            else
                            {
                                GUI.DrawTexture(rect, EditorIcons.TestPassed);
                            }
                        }
                        else
                        {
                            EditorGUI.DrawRect(obj.Rect.AlignTop(obj.Rect.height - (EditorGUIUtility.isProSkin ? 1 : 0)), selectedMaskBgColor);
                        }
                    }
                    else
                    {
                        GUI.DrawTexture(rect, EditorIcons.TestNormal);
                    }
                }
            }
        }

        private void ToggleEnumFlag(OdinMenuItem obj)
        {
            var val = (ulong)Convert.ToInt64(GetMenuItemEnumValue(obj));
            if ((val & this.curentValue) == val)
            {
                this.curentValue = val == 0 ? 0 : (this.curentValue & ~val);
            }
            else
            {
                this.curentValue = this.curentValue | val;
            }

            if (Event.current.clickCount >= 2)
            {
                Event.current.Use();
            }
        }

        /// <summary>
        /// Gets the currently selected enum value.
        /// </summary>
        public override IEnumerable<T> GetCurrentSelection()
        {
            if (IsFlagEnum)
            {
                yield return (T)Enum.ToObject(typeof(T), this.curentValue);
            }
            else
            {
                if (this.SelectionTree.Selection.Count > 0)
                {
                    yield return (T)Enum.ToObject(typeof(T), GetMenuItemEnumValue(this.SelectionTree.Selection.Last()));
                }
            }
        }

        /// <summary>
        /// Selects an enum.
        /// </summary>
        public override void SetSelection(T selected)
        {
            if (IsFlagEnum)
            {
                this.curentValue = (ulong)Convert.ToInt64(selected);
            }
            else
            {
                var selection = this.SelectionTree.EnumerateTree().Where(x => Convert.ToInt64(GetMenuItemEnumValue(x)) == Convert.ToInt64(selected));
                this.SelectionTree.Selection.AddRange(selection);
            }
        }

        private static object GetMenuItemEnumValue(OdinMenuItem item)
        {
            if (item.Value is EnumTypeUtilities<T>.EnumMember)
            {
                var member = (EnumTypeUtilities<T>.EnumMember)item.Value;
                return member.Value;
            }

            return default(T);
        }

        /// <summary>
        /// Draws an enum selector field using the enum selector.
        /// </summary>
        public static T DrawEnumField(GUIContent label, GUIContent contentLabel, T value, GUIStyle style = null)
        {
            int id;
            bool hasFocus;
            Rect rect;
            Action<EnumSelector<T>> bindSelector;
            Func<IEnumerable<T>> getResult;

            SirenixEditorGUI.GetFeatureRichControlRect(label, out id, out hasFocus, out rect);

            if (DrawSelectorButton(rect, contentLabel, style ?? EditorStyles.popup, id, out bindSelector, out getResult))
            {
                var selector = new EnumSelector<T>();

                if (!EditorGUI.showMixedValue)
                {
                    selector.SetSelection(value);
                }

                var window = selector.ShowInPopup(new Vector2(rect.xMin, rect.yMax));

                if (EnumTypeUtilities<T>.IsFlagEnum)
                {
                    window.OnClose += selector.SelectionTree.Selection.ConfirmSelection;
                }

                bindSelector(selector);

                if ((int)Application.platform == 16) // LinuxEditor
                {
                    GUIUtility.ExitGUI();
                }
            }

            if (getResult != null)
            {
                value = getResult().FirstOrDefault();
            }

            return value;
        }

        /// <summary>
        /// Draws an enum selector field using the enum selector.
        /// </summary>
        public static T DrawEnumField(GUIContent label, T value, GUIStyle style = null)
        {
            string display;

            if (EditorGUI.showMixedValue)
            {
                display = SirenixEditorGUI.MixedValueDashChar;
            }
            else
            {
                display = GetValueString(value);
            }

            return DrawEnumField(label, new GUIContent(display), value, style);
        }

        /// <summary>
        /// Draws an enum selector field using the enum selector.
        /// </summary>
        public static T DrawEnumField(Rect rect, GUIContent label, GUIContent contentLabel, T value, GUIStyle style = null)
        {
            int id;
            bool hasFocus;
            Action<EnumSelector<T>> bindSelector;
            Func<IEnumerable<T>> getResult;

            rect = SirenixEditorGUI.GetFeatureRichControl(rect, label, out id, out hasFocus);

            if (DrawSelectorButton(rect, contentLabel, style ?? EditorStyles.popup, id, out bindSelector, out getResult))
            {
                var selector = new EnumSelector<T>();

                if (!EditorGUI.showMixedValue)
                {
                    selector.SetSelection(value);
                }

                var window = selector.ShowInPopup(new Vector2(rect.xMin, rect.yMax));

                if (EnumTypeUtilities<T>.IsFlagEnum)
                {
                    window.OnClose += selector.SelectionTree.Selection.ConfirmSelection;
                }

                bindSelector(selector);

                if ((int)Application.platform == 16) // LinuxEditor
                {
                    GUIUtility.ExitGUI();
                }
            }

            if (getResult != null)
            {
                value = getResult().FirstOrDefault();
            }

            return value;
        }

        /// <summary>
        /// Draws an enum selector field using the enum selector.
        /// </summary>
        public static T DrawEnumField(Rect rect, GUIContent label, T value, GUIStyle style = null)
        {
            #region Modified By Hunter (hunter_hb) -- 2023年9月8日
            // var display = (EnumTypeUtilities<T>.IsFlagEnum && Convert.ToInt64(value) == 0) ? GetNoneValueString() : (EditorGUI.showMixedValue ? SirenixEditorGUI.MixedValueDashChar : value.ToString().SplitPascalCase());
            string display;
            if (EditorGUI.showMixedValue)
            {
                display = SirenixEditorGUI.MixedValueDashChar;
            }
            else
            {
                display = GetValueString(value);
            }
            #endregion
            
            return DrawEnumField(rect, label, new GUIContent(display), value, style);
        }

        private static string GetNoneValueString()
        {
            var name = Enum.GetName(typeof(T), GetZeroValue());
            if (name != null) return name.SplitPascalCase();
            return "None";
        }

        private static string GetValueString(T value)
        {
            var enumVals = EnumTypeUtilities<T>.AllEnumMemberInfos;

            for (int i = 0; i < enumVals.Length; i++)
            {
                var val = enumVals[i];

                if (EqualityComparer(val.Value, value))
                {
                    return val.NiceName;
                }
            }

            if (EnumTypeUtilities<T>.IsFlagEnum)
            {
                var val64 = Convert.ToInt64(value);

                if (val64 == 0)
                {
                    return GetNoneValueString();
                }

                SB.Length = 0;

                for (int i = 0; i < enumVals.Length; i++)
                {
                    var val = enumVals[i];
                    var flags = Convert.ToInt64(val.Value);
                    if (flags == 0) continue;

                    if ((val64 & flags) == flags)
                    {
                        if (SB.Length > 0) SB.Append(", ");
                        SB.Append(val.NiceName);
                    }
                }

                return SB.ToString();
            }

            //var display = (isFlagEnum && Convert.ToInt64(value) == 0) ? GetNoneValueString() : (EditorGUI.showMixedValue ? SirenixEditorGUI.MixedValueDashChar : GetValueString(value));

            return value.ToString().SplitPascalCase();
        }

        #region Modified By Hunter (jb) -- 2022年3月25日

        public static T DrawEnumToolbarLayout(T selected, float height = 22, int paddingTop = 4)
        {
            if (EnumTypeUtilities<T>.IsFlagEnum)
            {
                SirenixEditorGUI.MessageBox("带flag的没写, 有需要再说");
                return selected;
            }
            SirenixEditorGUI.BeginHorizontalToolbar(height, paddingTop);
            var enumVals = EnumTypeUtilities<T>.AllEnumMemberInfos;
            for (int i = 0; i < enumVals.Length; i++)
            {
                var ev = enumVals[i];
                if(ev.Hide) continue;
                if (SirenixEditorGUI.ToolbarTab(ev.Value.Equals(selected),ev.NiceName))
                {
                    selected = ev.Value;
                }
            }      
            SirenixEditorGUI.EndHorizontalToolbar();
            return selected;
        }

        #endregion
    }
}
#endif