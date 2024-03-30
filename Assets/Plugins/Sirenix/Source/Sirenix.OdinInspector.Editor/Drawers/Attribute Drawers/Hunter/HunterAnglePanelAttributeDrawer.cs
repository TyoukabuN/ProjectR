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
    public class HunterAnglePanelAttributeDrawer :OdinAttributeDrawer<HunterAnglePanelAttribute>
    {
        private GUIContent centerLabelContent;
        private Vector2 centerLabelSize;
        protected override void Initialize()
        {
            centerLabelContent = new GUIContent($"000.00");
            centerLabelSize = SirenixGUIStyles.CenteredGreyMiniLabel.CalcSize(centerLabelContent);
            base.Initialize();
        }

        public override bool CanDrawTypeFilter(Type type)
        {
            return type == typeof(float);
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
            var angleValue = (float)this.Property.ValueEntry.WeakSmartValue;
            this.centerLabelContent.text = $"{angleValue:F2}";
            var centerLabelRect = new Rect(boxRect.center + Vector2.one, centerLabelSize);
            GUI.Box(centerLabelRect, this.centerLabelContent, SirenixGUIStyles.CenteredGreyMiniLabel);

            var hColor = Handles.color;
            Handles.color = new Color(1f, 1f, 1f, 0.2f);
            Handles.DrawWireArc(boxRect.center, Vector3.back, Vector3.right, 360,boxRect.width/2f-2f, 0.5f);
            Handles.color = hColor;

            
            var vectorValue = Rotate(Vector2.up, Attribute.ClockWise? - angleValue : angleValue);
            var handleCenter = boxRect.center + new Vector2(vectorValue.x * boxRect.width,- vectorValue.y * boxRect.width)/2f;
            var handleSize = Vector2.one * 8f;
            var handleRect = new Rect(Rect.zero);
            handleRect.size = handleSize;
            handleRect.center = handleCenter;
            var color = Handles.color;
            lineColor = color;
            lineColor.a = 0.2f;
            Handles.color = lineColor;
            Handles.DrawDottedLine(boxRect.center, handleRect.center, 0.3f);
            Handles.DrawWireArc(boxRect.center, Vector3.back,Vector3.down, Attribute.ClockWise? -angleValue : angleValue,  boxRect.width/4f);
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
            mousePosToCenter.y = -mousePosToCenter.y;
            var angle =  Vector2.SignedAngle(Vector2.up, mousePosToCenter);
            if (Attribute.ClockWise)
            {
                angle = -angle;
            }
            if (angle < 0f)
            {
                angle += 360f;
            }
            this.Property.ValueEntry.WeakSmartValue = (object)angle;
        }

        public static Vector2 Rotate(Vector2 v, float degrees) {
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
         
            float tx = v.x;
            float ty = v.y;
            v.x = (cos * tx) - (sin * ty);
            v.y = (sin * tx) + (cos * ty);
            return v;
        }
    }
}
#endif
