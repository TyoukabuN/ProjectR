using Sirenix.Utilities;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PJR.Timeline.Editor
{
    public static class GUIUtil
    {
        public static void DebugRect(Rect position) => DebugRect(position, Color.green);
        public static void DebugRect(Rect position, Color color)
        {
            if (!(TimelineWindow.instance?.state.debugging ?? false))
                return;

            Handles.BeginGUI();
            var rect = position;
            Vector3 topLeft = new Vector3(rect.xMin, rect.yMin, 0);
            Vector3 topRight = new Vector3(rect.xMax, rect.yMin, 0);
            Vector3 bottomRight = new Vector3(rect.xMax, rect.yMax, 0);
            Vector3 bottomLeft = new Vector3(rect.xMin, rect.yMax, 0);

            // 设置线条颜色
            Handles.color = color;

            // 绘制矩形边框
            Handles.DrawLine(topLeft, topRight);
            Handles.DrawLine(topRight, bottomRight);
            Handles.DrawLine(bottomRight, bottomLeft);
            Handles.DrawLine(bottomLeft, topLeft);

            Handles.DrawLine(topLeft, bottomRight);

            Handles.EndGUI();
        }
        public static void Debug(this Rect rect, Color color) => DebugRect(rect, color);
        public static void Debug(this Rect rect) => DebugRect(rect);

        public static void EventCheck(Rect rect, EventType eventType, Action<Event> callback) => EventCheck(rect, eventType, callback, false, false);
        public static void EventCheck(Rect rect, EventType eventType, Action<Event> callback, bool ctrl, bool alt)
        {
            if (ctrl && !Event.current.control)
                return;
            if (alt && !Event.current.alt)
                return;
            if (Event.current.type == eventType && rect.Contains(Event.current.mousePosition))
                callback?.Invoke(Event.current);
        }

        public static void CheckWheelEvent(Rect rect, Action<Event> callback) => EventCheck(rect, EventType.ScrollWheel, callback);

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
