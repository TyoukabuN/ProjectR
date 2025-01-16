using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static PJR.Timeline.Editor.TimelineWindow;
using Constants = PJR.Timeline.Editor.TimelineWindow.Constants;

namespace PJR.Timeline.Editor
{
    public abstract class ClipGUI
    {
        private static GUIStyle labelStyle;

        public Clip Clip { get; set; }
        public virtual float CalculateHeight() => Constants.trackHeight;

        protected WindowState windowState => instance.state;

        public virtual void OnDrawMenu(Rect rect) 
        {
            EditorGUI.DrawRect(rect, GetMenuBgColor());
            rect.Debug();

            var labelSize = Styles.CalcLabelSize(Clip.GetDisplayName());

            GUILayout.Label(new GUIContent(Clip.GetDisplayName(), Clip.GetDisplayName()), GUILayout.Width(labelSize.x), GUILayout.ExpandHeight(true));

            GUILayout.FlexibleSpace();


            using (new MidAlignmentScope.Horizontal())
            { 
                if (GUILayout.Button(Clip.mute ? Styles.trackMuteEnabledIcon : Styles.trackMuteDisabledIcon, EditorStyles.iconButton))
                {
                    Clip.mute = !Clip.mute;
                }
            }

            GUILayoutUtility.GetLastRect().Debug(Color.green);


            var evtRect = rect;
            evtRect.xMin -= Constants.trackMenuLeftSpace;
            EventCheck(evtRect);
        }
        public virtual void OnDrawTrack(Rect rect) 
        {
            EditorGUI.DrawRect(rect, GetTrackBgColor());
            //rect.Debug();
            OnDrawClip(rect);

            EventCheck(rect);
        }

        public virtual void OnDrawClip(Rect rect)
        {
            var start = Clip.startFrame * GUIUtil.windowState.currentPixelPerFrame + rect.xMin;
            var end = Clip.endFrame * GUIUtil.windowState.currentPixelPerFrame + rect.xMin;
            Rect clipRect = Rect.MinMaxRect(start, rect.yMin, end, rect.yMax);

            if (clipDragging)
                clipRect.x += clipDrag_draggedFrameOffset * windowState.currentPixelPerFrame;
            if (clipResizing)
            { 
                if(clipResie_purpose == ResizePurpose.Left)
                    clipRect.xMin += clipResize_draggedFrameOffset * windowState.currentPixelPerFrame;
                if (clipResie_purpose == ResizePurpose.Right)
                    clipRect.xMax += clipResize_draggedFrameOffset * windowState.currentPixelPerFrame;
            }

            EditorGUI.DrawRect(clipRect, Styles.Instance.customSkin.clipBckg);
            clipRect.DrawOutline(1 ,Styles.Instance.customSkin.colorSequenceBackground);

            DrawHandle(clipRect);

            ClipRectEvent(clipRect);
        }

        /// <summary>
        /// 画Resize用的区域
        /// </summary>
        /// <param name="rect"></param>
        public virtual void DrawHandle(Rect rect)
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

            switch (Event.current.GetTypeForControl(controlID))
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
                            Event.current.Use();
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
                            double durationOffset = clipResize_draggedFrameOffset / windowState.CurrentFrameRate;
                            if (clipResie_purpose == ResizePurpose.Left)
                                SetClipStartSafe(Clip.start + durationOffset);
                            else if (clipResie_purpose == ResizePurpose.Right)
                                SetClipEndSafe(Clip.end + durationOffset);

                            clipResie_startPosition = Vector2.zero;
                            clipResizing = false;
                            clipResie_purpose = ResizePurpose.None;
                        }
                        Repaint();
                        Event.current.Use();
                        break;
                    }
                case EventType.MouseDrag:
                    {
                        if (GUIUtility.hotControl != controlID)
                            return;
                        clipResizing = true;
                        float pixel = Event.current.mousePosition.x - clipResie_startPosition.x;
                        
                        if(clipResie_purpose == ResizePurpose.Left)
                            clipResize_draggedFrameOffset = ClampClipStartChange_Frame(Clip.startFrame + windowState.PixelToFrame(pixel)) - Clip.startFrame;
                        if (clipResie_purpose == ResizePurpose.Right)
                            clipResize_draggedFrameOffset = ClampClipEndChange_Frame(Clip.endFrame + windowState.PixelToFrame(pixel)) - Clip.endFrame;

                        Repaint();
                        Event.current.Use();
                        break;
                    }
            }
        }
        #endregion

        #region clip Drag
        protected Vector2 clipDrag_startPosition = Vector2.zero;
        protected bool clipDragging = false;
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
                            clipDrag_draggedFrameOffset = 0;
                            Event.current.Use();
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
                            double durationOffset = clipDrag_draggedFrameOffset / windowState.CurrentFrameRate;
                            Clip.start += durationOffset;
                            Clip.end += durationOffset;
                            clipDrag_startPosition = Vector2.zero;
                            clipDragging = false;
                        }
                        Repaint();
                        Event.current.Use();
                        break;
                    }
                case EventType.MouseDrag:
                    {
                        if (GUIUtility.hotControl != controlID)
                            return;
                        clipDragging = true;
                        float pixel = Event.current.mousePosition.x - clipDrag_startPosition.x;
                        if(ValidToChangeRangeByPixel(pixel, out int frameOffset))
                            clipDrag_draggedFrameOffset = frameOffset;

                        Repaint();
                        Event.current.Use();
                        break;
                    }
            }
        }
        #endregion

        public void SetClipRangeSafe(double start, double end)
        {
            if (start < 0)
                start = 0;
            if((end - start) < windowState.CurrentSecondPerFrame)
                end = windowState.CurrentSecondPerFrame;
            Clip.start = start;
            Clip.end = end;
        }
        public void SetClipStartSafe(double start)
        {
            SetClipRangeSafe(start, Clip.end);
        }
        public void SetClipEndSafe(double end)
        {
            SetClipRangeSafe(Clip.start, end);
        }
        public bool ValidToChangeRange(int frames)
        {
            double durationOffset = frames / windowState.CurrentFrameRate;
            return (Clip.start + durationOffset) >= 0;
        }
        public bool ValidToChangeRangeByPixel(float pixel, out int frameOffset)
        {
            frameOffset = windowState.PixelToFrame(pixel);
            return ValidToChangeRange(frameOffset);
        }
        public bool ValidToChangeRangeByPixel(float pixel) => ValidToChangeRangeByPixel(pixel, out var frameOffset);


        #region Clip Start/End Clamp 得出一个限制后的对应单位的Start/End

        public double ClampClipEndChange(double end)
        {
            var start = Clip.start;
            if ((end - start) < windowState.CurrentSecondPerFrame)
                end = start + windowState.CurrentSecondPerFrame;
            return end;
        }
        public float ClampClipEndChange_Pixel(float pixel) => windowState.FrameToPixel(ClampClipEndChange_Frame(windowState.PixelToFrame(pixel)));
        public int ClampClipEndChange_Frame(int frames) => Utility.Time.ToFrames(ClampClipEndChange(frames / windowState.CurrentFrameRate), windowState.CurrentFrameRate);


        public double ClampClipStartChange(double start)
        {
            var end = Clip.end;
            if (start < 0)
                start = 0;
            if((end - start) < windowState.CurrentSecondPerFrame)
                start = end - windowState.CurrentSecondPerFrame;
            return start;
        }
        public float ClampClipStartChange_Pixel(float pixel) => ClampClipStartChange_Frame(windowState.PixelToFrame(pixel));
        public int ClampClipStartChange_Frame(int frames) => Utility.Time.ToFrames(ClampClipStartChange(frames / windowState.CurrentFrameRate), windowState.CurrentFrameRate);
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
            Event.current.Use();
            Repaint();
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

        public virtual void Repaint()
        { 
            TimelineWindow.instance?.Repaint();
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