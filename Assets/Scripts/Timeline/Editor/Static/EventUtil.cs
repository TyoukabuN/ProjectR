﻿using System;
using UnityEngine;

namespace PJR.Timeline.Editor
{
    public static class EventUtil
    {
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

        public static bool EventCheck(Rect rect, EventType eventType) => EventCheck(rect, eventType, true, false, false);
        public static bool EventCheck(Rect rect, EventType eventType, bool useEvent) => EventCheck(rect, eventType, useEvent, false, false);

        public static bool EventCheck(Rect rect, EventType eventType, bool useEvent,bool ctrl, bool alt)
        {
            var controlID = GUIUtility.GetControlID(FocusType.Passive);

            if (ctrl && !Event.current.control)
                return false;
            if (alt && !Event.current.alt)
                return false;
            if (Event.current.GetTypeForControl(controlID) == eventType && rect.Contains(Event.current.mousePosition))
            { 
                if(useEvent) 
                    eventType.Use();
                return true;
            }
            return false;
        }

        public static Vector2 msPos => Event.current.mousePosition;

        public static void DragEventCheck(this Rect position, Action<Rect> OnMouseDrag)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            if (OnMouseDrag == null)
            {
                if (GUIUtility.hotControl == controlID)
                    GUIUtility.hotControl = 0;
                return;
            }

            switch (Event.current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                {
                    if (position.Contains(msPos) && Event.current.button == 0)
                        GUIUtility.hotControl = controlID;
                    break;
                }
                case EventType.MouseUp:
                {
                    if (GUIUtility.hotControl == controlID)
                        GUIUtility.hotControl = 0;
                    break;
                }
                case EventType.MouseDrag:
                {
                    if (GUIUtility.hotControl != controlID)
                        return;
                    OnMouseDrag.Invoke(position);
                    EventType.MouseDrag.Use();
                    break;
                }
            }
        }
        
        public static void UseCurrentEvent()
        {
            Event.current.Use();
        }
        public static void Use(this EventType eventType)
        {
            //走通用的use,不然后面use多起来了,都不知道哪里use了
            //这里可以加个log,
            //获知你直接在这里断点,来看看哪次use覆盖了
            //UnityEngine.Debug.Log($"[Event Using] {eventType}");
            Event.current.Use();
        }
        
        public static class MouseEvent
        {
            public static void LeftOrRightClick(Rect rect, Action<Rect> onLeftClick, Action<Rect> onRightClick)
                => LeftOrRightClick(rect, onLeftClick, onRightClick, true);
            public static void LeftOrRightClick(Rect rect, Action<Rect> onLeftClick, Action<Rect> onRightClick, bool useEvent)
            {
                if (EventUtil.EventCheck(rect, EventType.MouseDown,useEvent) && Event.current.button == 0)
                    onLeftClick(rect);
                if (EventUtil.EventCheck(rect, EventType.MouseDown,useEvent) && Event.current.button == 1)
                    onRightClick(rect);
            }
        }
    }
}