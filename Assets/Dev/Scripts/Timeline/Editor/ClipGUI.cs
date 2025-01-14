using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static PJR.Timeline.Editor.TimelineWindow;
using Constants = PJR.Timeline.Editor.TimelineWindow.Constants;

namespace PJR.Timeline.Editor
{
    public abstract class ClipGUI
    {
        public Clip Clip { get; set; }
        public virtual float CalculateHeight() => TimelineWindow.Constants.trackHeight;

        protected WindowState windowState => TimelineWindow.instance.state;
        public virtual void OnDrawMenu(Rect rect) 
        {
            Color backgroundColor = windowState.hotTrack == this
                ? Styles.Instance.customSkin.colorSelection
                : Styles.Instance.customSkin.colorTrackHeaderBackground;
            EditorGUI.DrawRect(rect, backgroundColor);
            rect.Debug();

            var evtRect = rect;
            evtRect.xMin -= Constants.trackMenuLeftSpace;
            EventCheck(evtRect);
        }
        public virtual void OnDrawTrack(Rect rect) 
        {
            Color backgroundColor = windowState.hotTrack == this
                ? Styles.Instance.customSkin.colorSelection
                : Styles.Instance.customSkin.colorTrackBackground;
            
            EditorGUI.DrawRect(rect, backgroundColor);
            //rect.Debug();
            OnDrawClip(rect);

            EventCheck(rect);
        }
        public virtual void OnDrawClip(Rect rect)
        {
            var start = Clip.startFrame * GUIUtil.windowState.currentPixelPerFrame + rect.xMin;
            var end = Clip.endFrame * GUIUtil.windowState.currentPixelPerFrame + rect.xMin;
            Rect clipRect = Rect.MinMaxRect(start, rect.yMin, end, rect.yMax);

            EditorGUI.DrawRect(clipRect, Styles.Instance.customSkin.clipBckg);
            clipRect.Debug();

            ClipRectEvent(clipRect);
        }

        public virtual void ClipRectEvent(Rect position)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);

            switch (Event.current.GetTypeForControl(controlID))
            { 
                case EventType.MouseDown:
                    {
                        if (position.Contains(Event.current.mousePosition) && Event.current.button == 0)
                        { 
                            GUIUtility.hotControl = controlID;
                            windowState.hotClip = Clip;
                        }
                        break;
                    }
                case EventType.MouseUp:
                    {
                        if (GUIUtility.hotControl == controlID)
                            GUIUtility.hotControl = 0;
                        if (windowState.hotClip == Clip)
                            windowState.hotClip = null;
                        break;
                    }
                case EventType.MouseDrag:
                    {
                        if (GUIUtility.hotControl != controlID)
                            return;
                        break;
                    }
            }
        }

        protected void EventCheck(Rect rect)
        {
            if (EventCheck(rect, EventType.MouseDown) && Event.current.button == 0)
                OnClick(rect);
            if (EventCheck(rect, EventType.MouseDown) && Event.current.button == 1)
                OnContextClick(rect);
        }
        protected bool EventCheck(Rect rect, EventType eventType)
        {
            if (!rect.Contains(Event.current.mousePosition))
                return false;
            return Event.current.type == eventType;
        }
        public virtual void OnClick(Rect rect)
        {
            if (windowState.hotTrack == this)
                return;
            windowState.hotTrack = this;
            Event.current.Use();
            instance.Repaint();
        }
        public virtual void OnContextClick(Rect rect)
        {
            if (windowState.hotTrack == this)
                return;
            windowState.hotTrack = this;
            DisplayRightClickMenu(rect);
            Event.current.Use();
        }
        public virtual void OnClipClick(Rect rect)
        {
            if (windowState.hotClip == Clip)
                return;
            windowState.hotClip = Clip;
            Debug.Log("Clip Clicked");
            Event.current.Use();
        }

        /// <summary>
        /// 右键默认选项
        /// </summary>
        /// <returns></returns>
        protected GenericMenu GetDefaultGenericMenu()
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("右键默认选项"), false, () => { Debug.Log("1"); });
            return menu;
        }
        public virtual void DisplayRightClickMenu(Rect rect)
        {
            var menu = GetDefaultGenericMenu();
            menu.ShowAsContext();
        }
    }
    public abstract class ClipGUI<TClip> : ClipGUI where TClip : Clip 
    {
        public ClipGUI(TClip clip)
        {
            Clip = clip;
        }
    }
}