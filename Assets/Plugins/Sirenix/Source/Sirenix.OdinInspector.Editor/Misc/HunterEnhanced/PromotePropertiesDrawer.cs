using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEngine;
using SGUI = Sirenix.Utilities.Editor.SirenixEditorGUI;
using SStyle = Sirenix.Utilities.Editor.SirenixGUIStyles;

namespace Sirenix.OdinInspector.Editor
{
    public class PromotePropertiesDrawer : OdinValueDrawer<PromoteProperties>
    {
        private Dictionary<string, List<InspectorProperty>> promptGroups = new Dictionary<string, List<InspectorProperty>>();
        private Dictionary<string, float> groupTabWidths = new Dictionary<string, float>();
        private HashSet<string> selectTabs = new HashSet<string>();
        private bool drawFoldout;
        private bool drawAsTabGroup;
        private bool cacheToPath;
        private bool foldoutVisiable;

        protected override void Initialize()
        {
            var attr = Property.GetAttribute<PromotePropertyDrawingSettingsAttribute>();
            if (attr != null)
            {
                drawFoldout = attr.FoldOut;
                drawAsTabGroup = attr.DrawAsTabGroup;
                cacheToPath = attr.CachePropertyPaths;
            }
            RefreshParams();
        }

        private bool showParentProperty;

        private const string EmptyGroupName = "*未分组";

        private void RefreshParams()
        {
            // var prompts = this.Property.SerializationRoot.Children.Recurse().Where(p => p.GetAttribute<PromotePropertyAttribute>() != null).ToList();
            var prompts = new List<InspectorProperty>();
            GetPromotedPropertyRecurse(this.Property.SerializationRoot?.ValueEntry?.TypeOfValue, this.Property.SerializationRoot, ref prompts);
            var groups = prompts.GroupBy(p => p.GetAttribute<PromotePropertyAttribute>().@group);
            promptGroups = new Dictionary<string, List<InspectorProperty>>();
            groupTabWidths = new Dictionary<string, float>();
            foreach (var g in groups)
            {
                var calTabStr = string.IsNullOrEmpty(g.Key) ? EmptyGroupName : g.Key;
                var tabKey = string.IsNullOrEmpty(g.Key) ? EmptyGroupName : g.Key;
                promptGroups[tabKey] = g.ToList();
                // SStyle.ToolbarTab.CalcMinMaxWidth(GUIHelper.TempContent(calTabStr),out var min, out var max);
                // groupTabWidths[g.Key] = min;
                groupTabWidths[g.Key] = SStyle.ToolbarTab.CalcSize(GUIHelper.TempContent(calTabStr)).x;
                if (selectTabs.Count == 0)
                {
                    selectTabs.Add(g.Key);
                }
            }
        }

        private void GetPromotedPropertyRecurse(Type SerializationRootType, InspectorProperty property, ref List<InspectorProperty> props)
        {
            var attr = property.GetAttribute<PromotePropertyAttribute>();
            bool add = false;
            if (attr != null)
            {
                add = attr.matchType == null || (SerializationRootType != null && attr.matchType.IsAssignableFrom(SerializationRootType));
            }
            if (add)
            {
                props.Add(property);
                return;
            }
            else
            {
                for (int i = 0; i < property.Children.Count; i++)
                {
                    var c = property.Children[i];
                    GetPromotedPropertyRecurse(SerializationRootType,c, ref props);
                }
            }
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            try
            {
                if ((promptGroups?.Count ?? 0) > 0)
                {
                    SGUI.BeginBox();
                    SGUI.BeginBoxHeader();
                    GUILayout.BeginHorizontal();
                    if (drawFoldout)
                    {
                        foldoutVisiable = SGUI.Foldout(foldoutVisiable, label);
                    }
                    else
                    {
                        GUILayout.Label(label);
                    }

                    GUILayout.FlexibleSpace();
                    showParentProperty = GUILayout.Toggle(showParentProperty, $"显示字段路径");
                    if (GUILayout.Button(GUIHelper.TempContent("刷新")))
                    {
                        this.RefreshParams();
                    }

                    GUILayout.EndHorizontal();
                    SGUI.EndBoxHeader();
                    if (!drawFoldout || foldoutVisiable)
                    {
                        DrawContent();
                    }
                    SGUI.EndBox();
                }
            }
            catch
            {
            }
        }


        private void DrawContent()
        {
            if (drawAsTabGroup && promptGroups.Count > 1)
            {
                SGUI.BeginHorizontalToolbar(25f);
                foreach (var tab in groupTabWidths.Keys)
                {
                    var selected = SGUI.ToolbarTab(selectTabs.Contains(tab), GUIHelper.TempContent(tab));
                    if (selected)
                    {
                        if(!Event.current.control)
                            selectTabs.Clear();
                        selectTabs.Add(tab);
                    }
                    else if(selectTabs.Count>1) selectTabs.Remove(tab);
                }
                SGUI.EndHorizontalToolbar();

                foreach (var kv in promptGroups)
                {
                    if(!selectTabs.Contains(kv.Key)) continue;
                    foreach (var p in kv.Value)
                    {
                        p.Draw();
                        if (showParentProperty)
                        {
                            GUILayout.Label(GUIHelper.TempContent($"<color=grey><size=10>{p.Parent?.NiceName}:{p.NiceName}({p.Path})</size></color>"), SStyle.RichTextLabel);
                        }
                    }
                }
            }
            else
            {
                foreach (var kv in promptGroups)
                {
                    if (kv.Key == EmptyGroupName) continue;
                    SGUI.BeginBox(kv.Key);
                    foreach (var p in kv.Value)
                    {
                        p.Draw();
                        if (showParentProperty)
                        {
                            GUILayout.Label(GUIHelper.TempContent($"<color=grey><size=10>{p.Parent?.NiceName}:{p.NiceName}({p.Path})</size></color>"), SStyle.RichTextLabel);
                        }
                    }

                    SGUI.EndBox();
                }

                if (promptGroups.TryGetValue(EmptyGroupName, out var g))
                {
                    foreach (var p in g)
                    {
                        p.Draw();
                    }
                }
            }
        }
    }
}
