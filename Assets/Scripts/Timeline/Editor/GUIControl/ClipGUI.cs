using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using static PJR.Timeline.Editor.TimelineWindow;

namespace PJR.Timeline.Editor
{
    public abstract class ClipGUI
    {
        public IClip Clip { get; set; }
        public virtual float CalculateHeight() => Constants.trackHeight + draggedMenuSpace;

        float m_DraggedMenuSpace = 0f;
        public float draggedMenuSpace => m_DraggedMenuSpace;

        protected WindowState windowState => instance.state;

        /// <summary>
        /// 画Track左边TrackMenu
        /// </summary>
        /// <param name="rect"></param>
        public virtual void OnDrawMenu(Rect rect) 
        {
            EditorGUI.DrawRect(rect, GetMenuBgColor());

            rect.Debug();

            var labelSize = Styles.CalcLabelSize(Clip.GetDisplayName());

            GUILayout.Label(new GUIContent(Clip.GetDisplayName(), Clip.GetDisplayName()), GUILayout.Width(labelSize.x), GUILayout.ExpandHeight(true));

            GUILayout.FlexibleSpace();

            using (new MidAlignmentScope.Horizontal())
            { 
                if (GUILayout.Button(Clip.Mute ? Styles.trackMuteEnabledIcon : Styles.trackMuteDisabledIcon, EditorStyles.iconButton))
                {
                    Clip.Mute = !Clip.Mute;
                }
            }

            GUILayoutUtility.GetLastRect().Debug(Color.green);

            DrawMenuHandle(rect);

            var evtRect = rect;
            evtRect.xMin -= Constants.trackMenuLeftSpace;
            EventCheck(evtRect);
        }

        /// <summary>
        /// TrackMenu底部的SplitResizeUpDown
        /// </summary>
        /// <param name="rect"></param>
        public virtual void DrawMenuHandle(Rect rect)
        {
            rect.yMin = rect.yMax - Constants.trackMenuDragHandleHeight;
            rect.Debug(Color.magenta);
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.SplitResizeUpDown);

            var controlId = GUIUtility.GetControlID(FocusType.Passive);
            switch (Event.current.GetTypeForControl(controlId))
            { 
                case EventType.MouseDown:
                    {
                        if (rect.Contains(Event.current.mousePosition))
                        { 
                            //还不知道有没有必要Resize
                            EventType.MouseDown.Use();
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 画Track右边的TrackView里的Clip的入口
        /// </summary>
        /// <param name="rect"></param>
        public virtual void OnDrawTrack(Rect rect) 
        {
            EditorGUI.DrawRect(rect, GetTrackBgColor());

            OnDrawClip(rect);
            EventCheck(rect);
        }
        
        /// <summary>
        /// 画Track右边的TrackView里的Clip
        /// </summary>
        /// <param name="rect"></param>
        protected virtual void OnDrawClip(Rect rect)
        {
            var start = Clip.StartFrame * GUIUtil.windowState.currentPixelPerFrame + rect.xMin;
            var end = Clip.EndFrame * GUIUtil.windowState.currentPixelPerFrame + rect.xMin;
            Rect clipRect = Rect.MinMaxRect(start, rect.yMin, end, rect.yMax);

            if (clipDragging)
                clipRect.x += clipDrag_draggedPixelOffsetClamped;// clipDrag_draggedFrameOffset * windowState.currentPixelPerFrame;
            if (clipResizing)
            { 
                if(clipResie_purpose == ResizePurpose.Left)
                    clipRect.xMin += clipResize_draggedFrameOffset * windowState.currentPixelPerFrame;
                if (clipResie_purpose == ResizePurpose.Right)
                    clipRect.xMax += clipResize_draggedFrameOffset * windowState.currentPixelPerFrame;
            }

            //背景
            EditorGUI.DrawRect(clipRect, Styles.Instance.customSkin.clipBckg);
            //描边
            clipRect.DrawOutline(1 ,Styles.Instance.customSkin.colorSequenceBackground);
            //标题
            DrawLabel(clipRect,GetLabel());

            DrawResizeHandle(clipRect);
            ClipRectEvent(clipRect);
        }

        static readonly GUIContent s_TitleContent = new GUIContent();
        protected virtual void DrawLabel(Rect rect, string title)
        {
            float border = 5;
            rect = rect.Expand(-border, -border, 0, 0);
            
            s_TitleContent.text = title;
            var neededTextWidth = Styles.Instance.fontClip.CalcSize(s_TitleContent).x;
            if (neededTextWidth > rect.width)
                s_TitleContent.text = Styles.Elipsify(title, rect.width, neededTextWidth);
            
            GUI.Label(rect, s_TitleContent, Styles.Instance.fontClip);
        }

        public static string DefaultLabel = "Default Label";
        public virtual string GetLabel() => DefaultLabel;

        /// <summary>
        /// 画Resize用的区域
        /// </summary>
        /// <param name="rect"></param>
        public virtual void DrawResizeHandle(Rect rect)
        {
            var handleWidth = Mathf.Clamp(rect.width * 0.3f, Constants.clipMinHandleWidth, Constants.clipMaxHandleWidth);

            var rectLeft = rect;
            rectLeft.width = handleWidth;
            rectLeft.Debug(false);

            var rectRight = rectLeft;
            rectRight.x = rect.xMax - handleWidth;
            rectRight.Debug(false);

            EditorGUIUtility.AddCursorRect(rectLeft, MouseCursor.SplitResizeLeftRight);
            SplitResizeHandleEvent(rectLeft, ResizePurpose.Left);

            EditorGUIUtility.AddCursorRect(rectRight, MouseCursor.SplitResizeLeftRight);
            SplitResizeHandleEvent(rectRight, ResizePurpose.Right);
        }

        public Color GetMenuBgColor()
        {
            return windowState.hotTrack == this
                ? Styles.Instance.customSkin.colorSelection
                : Styles.Instance.customSkin.colorTrackHeaderBackground;
        }
        public Color GetTrackBgColor()
        {
            return windowState.hotTrack == this
                ? Styles.Instance.customSkin.colorSelection
                : Styles.Instance.customSkin.colorTrackBackground;
        }

        #region Clip Resize
        public enum ResizePurpose
        { 
            None,
            Left,
            Right,
        }
        protected bool clipResizing = false;
        protected ResizePurpose clipResie_purpose = ResizePurpose.None;
        protected int clipResize_draggedFrameOffset = 0;
        protected Vector2 clipResie_startPosition = Vector2.zero;

        public virtual void SplitResizeHandleEvent(Rect position, ResizePurpose resizePurpose)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);

            var eventType = Event.current.GetTypeForControl(controlID);
            switch (eventType)
            {
                case EventType.MouseDown:
                    {
                        if (position.Contains(Event.current.mousePosition) && Event.current.button == 0)
                        {
                            GUIUtility.hotControl = controlID;
                            windowState.hotClip = Clip;
                            clipResie_startPosition = Event.current.mousePosition;
                            clipResize_draggedFrameOffset = 0;
                            clipResie_purpose = resizePurpose;
                            EventType.MouseDown.Use();
                        }
                        break;
                    }
                case EventType.MouseUp:
                    {
                        if (windowState.hotClip != Clip)
                            return;
                        if (!clipResizing)
                            return;
                        windowState.hotClip = null;

                        if (GUIUtility.hotControl == controlID)
                            GUIUtility.hotControl = 0;

                        if (clipResizing)
                        {
                            double rangeOffset = clipResize_draggedFrameOffset / windowState.CurrentFrameRate;
                            if (clipResie_purpose == ResizePurpose.Left)
                                Clip.SetClipStartSafe(Clip.start + rangeOffset);
                            else if (clipResie_purpose == ResizePurpose.Right)
                                Clip.SetClipEndSafe(Clip.end + rangeOffset);
                            Clip.TrySave();

                            clipResie_startPosition = Vector2.zero;
                            clipResizing = false;
                            clipResie_purpose = ResizePurpose.None;
                        }

                        Repaint();
                        EventType.MouseUp.Use();
                        break;
                    }
                case EventType.MouseDrag:
                    {
                        if (GUIUtility.hotControl != controlID)
                            return;
                        clipResizing = true;
                        float pixel = Event.current.mousePosition.x - clipResie_startPosition.x;
                        
                        if(clipResie_purpose == ResizePurpose.Left)
                            clipResize_draggedFrameOffset = Clip.ClampClipStartChange_Frame(Clip.StartFrame + windowState.PixelToFrame(pixel)) - Clip.StartFrame;
                        if (clipResie_purpose == ResizePurpose.Right)
                            clipResize_draggedFrameOffset = Clip.ClampClipEndChange_Frame(Clip.EndFrame + windowState.PixelToFrame(pixel)) - Clip.EndFrame;

                        Repaint();
                        EventType.MouseDrag.Use();
                        break;
                    }
            }
        }
        #endregion

        #region clip Drag
        protected Vector2 clipDrag_startPosition = Vector2.zero;
        protected bool clipDragging = false;
        protected float clipDrag_draggedPixelOffset = 0f;
        protected float clipDrag_draggedPixelOffsetClamped = 0f;
        protected int clipDrag_draggedFrameOffset = 0;
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
                            clipDrag_startPosition = Event.current.mousePosition;
                            clipDrag_draggedPixelOffset = 0;
                            clipDrag_draggedPixelOffsetClamped = 0;
                            EventType.MouseDown.Use();
                        }
                        break;
                    }
                case EventType.MouseUp:
                    {
                        if (!clipDragging)
                            return;
                        if (windowState.hotClip != Clip)
                            return;
                        windowState.hotClip = null;
                        

                        if (GUIUtility.hotControl == controlID)
                            GUIUtility.hotControl = 0;

                        if (clipDragging)
                        {
                            double rangeOffset = windowState.PixelToSecond(clipDrag_draggedPixelOffsetClamped);
                            Clip.start += rangeOffset;
                            Clip.end += rangeOffset;
                            clipDrag_startPosition = Vector2.zero;
                            clipDragging = false;
                        }
                        Repaint();
                        EventType.MouseUp.Use();
                        break;
                    }
                case EventType.MouseDrag:
                    {
                        if (GUIUtility.hotControl != controlID)
                            return;
                        clipDragging = true;
                        float draggedPixelOffset = Event.current.mousePosition.x - clipDrag_startPosition.x;
                        int frames = TimeUtil.ToFrames(windowState.PixelToSecond(draggedPixelOffset), windowState.CurrentFrameRate);

                        if (Clip.ValidRangeChangeableByFrame(frames))
                        {
                            clipDrag_draggedPixelOffsetClamped = windowState.FrameToPixel(frames);
                            clipDrag_draggedFrameOffset = frames ;
                        }
                        clipDrag_draggedPixelOffset = draggedPixelOffset;


                        Repaint();
                        EventType.MouseDrag.Use();
                        break;
                    }
            }
        }
        #endregion


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
            EventType.MouseDown.Use();
            Repaint();
        }
        public virtual void OnContextClick(Rect rect)
        {
            DisplayRightClickMenu(rect);
            EventType.MouseDown.Use();
        }
        public virtual void OnClipClick(Rect rect)
        {
            if (windowState.hotClip == Clip)
                return;
            windowState.hotClip = Clip;
            Debug.Log("Clip Clicked");
            EventType.MouseDown.Use();
        }

        /// <summary>
        /// 右键默认选项
        /// </summary>
        /// <returns></returns>
        protected GenericMenu GetDefaultGenericMenu()
        {
            var menu = new GenericMenu();
            menu.AddDisabledItem(new GUIContent("右键默认选项"));
            menu.AddSeparator($"{Clip.GetDisplayName()}");
            return menu;
        }
        public virtual void DisplayRightClickMenu(Rect rect)
        {
            var menu = GetDefaultGenericMenu();
            menu.ShowAsContext();
        }

        public virtual void Repaint()
        { 
            TimelineWindow.instance?.Repaint();
        }
    }
    public abstract class ClipGUI<TClip> : ClipGUI where TClip : IClip
    {
        public ClipGUI(IClip clip)
        {
            Clip = clip;
        }
    }
    public class DefaultClipGUI : ClipGUI
    {
        public DefaultClipGUI(IClip clip) { 
            Clip = clip;
        }
        public override void OnDrawMenu(Rect rect)
        {
            base.OnDrawMenu(rect);
        }
    }
}