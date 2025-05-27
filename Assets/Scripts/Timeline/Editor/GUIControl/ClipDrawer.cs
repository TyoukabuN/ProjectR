using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace PJR.Timeline.Editor
{
    public partial class ClipDrawer : TimelineGUIElement
    {
        public IClip Clip;
        public ClipDrawer(IClip clip) => Reset(clip);
        public void Reset(IClip clip)
        {
            Clip = clip;
        }

        /// <summary>
        /// 画Track右边的TrackView里的Clip
        /// </summary>
        /// <param name="rect"></param>
        public virtual void DrawClip(Rect rect)
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
            EditorGUI.DrawRect(clipRect, GetClipBgColor(IsSelect));
            //描边
            clipRect.DrawOutline(1 ,GetClipBorderColor(IsSelect));
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
                            Select();
                            clipResie_startPosition = Event.current.mousePosition;
                            clipResize_draggedFrameOffset = 0;
                            clipResie_purpose = resizePurpose;
                            EventType.MouseDown.Use();
                        }
                        break;
                    }
                case EventType.MouseUp:
                    {
                        if (!IsSelect)
                            return;
                        if (!clipResizing)
                            return;
                        DeSelect();

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
                        Select();
                        GUIUtility.hotControl = controlID;
                        Select();
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
                    {
                        return;
                    }
                    if (!IsSelect)
                        return;
                    
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
    }
}