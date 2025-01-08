using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PJR.Timeline.Editor
{
    public static class GUIUtil
    {
        public static void DrawBorder(Rect position) => DrawBorder(position, Color.red);
        public static void DrawBorder(Rect position, Color color)
        {
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

        public static Rect ToLocal(this Rect rect)
        {
            rect.x = 0;
            rect.y = 0;
            return rect;
        }
        public static Rect Shrink(this Rect rect) => rect.Expand(-1);
        public static Rect Shrink(this Rect rect, float pixel) => rect.Expand(-pixel);
    }
}
