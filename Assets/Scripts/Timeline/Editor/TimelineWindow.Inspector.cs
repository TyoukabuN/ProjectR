using System;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUI;

namespace PJR.Timeline.Editor
{
    public partial class TimelineWindow : EditorWindow
    {
        [CustomEditor(typeof(TimelineWindow))]
        public class Inspector : UnityEditor.Editor
        {
            private int _lastPropertyTreeHash;

            public Inspector()
            {
                var state = instance?.State;
                if (state != null)
                {
                    state.OnHotspotChanged -= OnHotspotChanged;
                    state.OnHotspotChanged += OnHotspotChanged;
                }
            }
            private void OnHotspotChanged(TimelineGUIElement prev, TimelineGUIElement current)
            {
                Repaint();
            }
            public override void OnInspectorGUI()
            {
                var propertyTree = instance?.State?.Hotspot?.PropertyTree;
                if (propertyTree == null)
                {
                    DrawNonSelected();
                    return;
                }
                
                propertyTree.BeginDraw(true);
                EditorGUI.BeginChangeCheck();
                propertyTree.DrawProperties();
                if (EditorGUI.EndChangeCheck())
                {
                    instance.State.SetHasUnsavedChanges(true);
                }
                propertyTree.EndDraw();
            }

            private void DrawNonSelected()
            {
                GUILayout.Label("没有选中任何UI元素");
            }
        }
    }
}