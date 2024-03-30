#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Utilities;
    using Utilities.Editor;
    using UnityEditor;
    using UnityEngine;
    using System.Collections;
    using Sirenix.OdinInspector.Editor.ValueResolvers;
    public class HunterVector2PanelAttributeDrawer :OdinAttributeDrawer<HunterVector2PanelAttribute>
    {
        private GUIContent centerLabelContent;
        private Vector2 centerLabelSize;
        protected override void Initialize()
        {
            centerLabelContent = new GUIContent($"{this.Attribute.Center.x:F1},{this.Attribute.Center.y:F1}");
            centerLabelSize = SirenixGUIStyles.CenteredGreyMiniLabel.CalcSize(centerLabelContent);
            base.Initialize();
        }

        public override bool CanDrawTypeFilter(Type type)
        {
            return type == typeof(Vector2);
        }

        private bool dragging = false;
        protected override void DrawPropertyLayout(GUIContent label)
        {
            this.CallNextDrawer(label);
            var padding = 4f;
            var drawWidth = Mathf.Min(Attribute.Width,GUIHelper.ContextWidth);
            var drawHeight = (drawWidth - padding * 2f) / 1f + padding * 2f;
            var rect = GUILayoutUtility.GetRect(width:drawWidth, drawHeight);
            GUI.BeginClip(rect);
            
            //框框
            var boxRect = new Rect();
            boxRect.position = Vector2.one * padding;
            boxRect.size = new Vector2(drawWidth - padding *2f, drawHeight - padding*2f);
            GUI.Box(boxRect, GUIContent.none, SirenixGUIStyles.BoxContainer);
            //分割线
            var centerHLineRect = boxRect;
            centerHLineRect.height = 1f; 
            centerHLineRect.center = boxRect.center;
            var centerVLineRect = boxRect;
            centerVLineRect.width = 1f;
            centerVLineRect.center = boxRect.center;
            var lineColor = new Color(1f, 1f, 1f, 0.2f);
            EditorGUI.DrawRect(centerHLineRect, lineColor);
            EditorGUI.DrawRect(centerVLineRect, lineColor);
            
            //中心
            var centerLabelRect = new Rect(boxRect.center + Vector2.one, centerLabelSize);
            GUI.Box(centerLabelRect, this.centerLabelContent, SirenixGUIStyles.CenteredGreyMiniLabel);

            if (Attribute.ClampOffsetMagnitude || Attribute.SnapMagnitude)
            {
                var hColor = Handles.color;
                Handles.color = new Color(1f, 1f, 1f, 0.2f);
                Handles.DrawWireArc(boxRect.center, Vector3.back, Vector3.right, 360,boxRect.width/2f-2f, 0.5f);
                Handles.color = hColor;
            }
            
            var vectorValue = (Vector2)this.Property.ValueEntry.WeakSmartValue - this.Attribute.Center;
            var handleCenter = boxRect.center + new Vector2(vectorValue.x * boxRect.width / Attribute.Radius,- vectorValue.y * boxRect.width / Attribute.Radius)/2f;
            var handleSize = Vector2.one * 8f;
            var handleRect = new Rect(Rect.zero);
            handleRect.size = handleSize;
            handleRect.center = handleCenter;
            var color = Handles.color;
            lineColor = color;
            lineColor.a = 0.3f;
            Handles.color = lineColor;
            Handles.DrawDottedLine(boxRect.center, handleRect.center, 0.3f);
            Handles.color = color;
            if (handleRect.Overlaps(boxRect))
            {
                GUI.Box(handleRect, EditorIcons.TestNormal, GUIStyle.none);
                EditorGUIUtility.AddCursorRect(handleRect, MouseCursor.MoveArrow);
                
            }
            var e = Event.current;
            if (e.type == EventType.MouseDown&& boxRect.Contains(e.mousePosition))
            {
                dragging = true;
                UpdateDragPos(e, boxRect);
                e.Use();
            }
            else if (e.type == EventType.MouseUp)
            {
                dragging = false;
            }
            else if (e.type == EventType.MouseDrag)
            {
                if (dragging)
                {
                    UpdateDragPos(e, boxRect);
                }
            }
            GUI.EndClip();
        }

        private void UpdateDragPos(Event e, Rect boxRect)
        {
            var mousePos = e.mousePosition;
            mousePos = new Vector2(Mathf.Clamp(mousePos.x, boxRect.xMin, boxRect.xMax), Mathf.Clamp(mousePos.y, boxRect.yMin, boxRect.yMax));
            var mousePosToCenter = mousePos - boxRect.center;
            var offset = new Vector2(mousePosToCenter.x * Attribute.Radius / boxRect.width, -mousePosToCenter.y * Attribute.Radius / boxRect.width) * 2f;
            if (Attribute.SnapMagnitude)
            {
                if (offset.magnitude < 0f)
                {
                    offset = Vector2.up;
                }
                offset = offset.normalized * Attribute.Radius;
            }
            else if (Attribute.ClampOffsetMagnitude)
            {
                offset = Vector2.ClampMagnitude(offset, Attribute.Radius);
            }

            var newVectorValue = Attribute.Center + offset;
            this.Property.ValueEntry.WeakSmartValue = (object)newVectorValue;
        }
    }
}
#endif