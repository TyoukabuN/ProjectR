using Sirenix.Utilities;
using System;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace PJR.Timeline.Editor
{
    /// <summary>
    /// 画GUI相关的工具类
    /// </summary>
    public static class GUIUtil
    {
        public static TimelineWindow.WindowState windowState => TimelineWindow.instance?.State;

        public static void DrawOutline(this Rect rect, float size, Color color)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Color color2 = GUI.color;
                GUI.color *= color;
                GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, size), EditorGUIUtility.whiteTexture);
                GUI.DrawTexture(new Rect(rect.x, rect.yMax - size, rect.width, size), EditorGUIUtility.whiteTexture);
                GUI.DrawTexture(new Rect(rect.x, rect.y + 1f, size, rect.height - 2f * size), EditorGUIUtility.whiteTexture);
                GUI.DrawTexture(new Rect(rect.xMax - size, rect.y + 1f, size, rect.height - 2f * size), EditorGUIUtility.whiteTexture);
                GUI.color = color2;
            }
        }
        public static void DebugRect(Rect position) => DebugRect(position, Color.green, true, false);
        public static void DebugRect(Rect position, Color color, bool displaySize, bool forceDraw)
        {
            if (!forceDraw && !(windowState?.debugging ?? false))
                return;

            Handles.BeginGUI();
            var rect = position;
            Vector3 topLeft = new Vector3(rect.xMin, rect.yMin, 0);
            Vector3 topRight = new Vector3(rect.xMax, rect.yMin, 0);
            Vector3 bottomRight = new Vector3(rect.xMax, rect.yMax, 0);
            Vector3 bottomLeft = new Vector3(rect.xMin, rect.yMax, 0);
            Vector3 center = (topLeft + bottomRight) * 0.5f;

            // 设置线条颜色
            Handles.color = color;

            // 绘制矩形边框
            Handles.DrawLine(topLeft, topRight);
            Handles.DrawLine(topRight, bottomRight);
            Handles.DrawLine(bottomRight, bottomLeft);
            Handles.DrawLine(bottomLeft, topLeft);
            
            //在中显示大小
            if (displaySize)
            {
                var temp = topLeft;
                temp.x += 1f;
                temp.y += 1f;
                Handles.Label(topLeft, new GUIContent(rect.ToString()), EditorStyles.miniLabel);
            }
            
            //对角线
            if (rect.width > 50)
                Handles.DrawLine(topLeft, bottomRight);

            Handles.EndGUI();
        }
        public static void Debug(this Rect rect, bool displaySize, bool forceDraw) => DebugRect(rect, Color.green, displaySize, forceDraw);
        public static void Debug(this Rect rect, Color color, bool forceDraw) => DebugRect(rect, color, true, forceDraw);
        public static void Debug(this Rect rect, bool displaySize) => DebugRect(rect, Color.green, displaySize, false);
        public static void Debug(this Rect rect, Color color) => DebugRect(rect, color, true, false);
        public static void Debug(this Rect rect) => DebugRect(rect);
   
        public static void CheckWheelEvent(Rect rect, Action<Event> callback) => EventUtil.EventCheck(rect, EventType.ScrollWheel, callback);

        /// <summary>
        /// 将Rect原点归零
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Rect ToOrigin(this Rect rect)
        {
            rect.x = 0;
            rect.y = 0;
            return rect;
        }
        public static Rect Shrink(this Rect rect) => rect.Expand(-1);
        public static Rect Shrink(this Rect rect, float pixel) => rect.Expand(-pixel);
    }

    public static class UIControlUtil
    {
        public static Button GetButton() => GetButton(null);
        public static Button GetButton(string name)
        {
            Button button = new Button();
            button.name = name;
            button.SetMargin(3, 1);
            button.SetPadding(5, 2);

            return button;
        }

        public static void SetMargin0(this VisualElement visualElement)=> SetMargin(visualElement, 0,0,0,0);
        public static void SetMargin(this VisualElement visualElement, StyleLength lr, StyleLength tb)=> SetMargin(visualElement, lr, lr, tb, tb);
        public static void SetMargin(this VisualElement visualElement, StyleLength left, StyleLength right, StyleLength top, StyleLength bottom)
        {
            visualElement.style.marginLeft = left;
            visualElement.style.marginRight = right;
            visualElement.style.marginTop = top;
            visualElement.style.marginBottom = bottom;
        }
        public static void SetPadding0(this VisualElement visualElement) => SetPadding(visualElement, 0, 0, 0, 0);
        public static void SetPadding(this VisualElement visualElement, StyleLength lr, StyleLength tb)=> SetPadding(visualElement, lr, lr, tb, tb);
        public static void SetPadding(this VisualElement visualElement, StyleLength left, StyleLength right, StyleLength top, StyleLength bottom)
        {
            visualElement.style.paddingLeft = left;
            visualElement.style.paddingRight = right;
            visualElement.style.paddingTop = top;
            visualElement.style.paddingBottom = bottom;
        }
        public static void SetBorderRadius0(this VisualElement visualElement) => SetBorderRadius(visualElement, 0, 0, 0, 0);
        public static void SetBorderRadius(this VisualElement visualElement, StyleLength topLeft, StyleLength topRight, StyleLength bottomLeft, StyleLength bottomRight)
        {
            visualElement.style.borderTopLeftRadius = topLeft;
            visualElement.style.borderTopRightRadius = topRight;
            visualElement.style.borderBottomLeftRadius = bottomLeft;
            visualElement.style.borderBottomRightRadius = bottomRight;
        }
    }
}
