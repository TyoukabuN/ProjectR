﻿using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace PJR.Timeline.Editor
{
    public class DefaultClipDrawer : ClipDrawer<IClip>
    {
        public DefaultClipDrawer(IClip clip):base(clip){}
    }
    public abstract class ClipDrawer<TClip> : ClipDrawer where TClip : IClip
    {
        protected TClip _clip;
        public override IClip IClip => _clip;
        public TClip Clip => _clip;
        public override object PropertyObject => Clip;
        public ClipDrawer(TClip clip)
        {
            _clip = clip;
        }
    }
    public abstract partial class ClipDrawer : TimelineGUIElement
    {
        public abstract IClip IClip { get; }
        /// <summary>
        /// 画Track右边的TrackView里的Clip
        /// </summary>
        /// <param name="rect"></param>
        public virtual void DrawClip(Rect rect)
        {
            var start = IClip.StartFrame * GUIUtil.windowState.currentPixelPerFrame + rect.xMin;
            var end = IClip.EndFrame * GUIUtil.windowState.currentPixelPerFrame + rect.xMin;
            Rect clipRect = Rect.MinMaxRect(start, rect.yMin, end, rect.yMax);

            if (clipDragging)
                clipRect.x += clipDrag_draggedPixelOffsetClamped;
            else if (clipResizing)
            { 
                if(clipResie_purpose == ResizePurpose.Left)
                    clipRect.xMin += clipResize_draggedFrameOffset * windowState.currentPixelPerFrame;
                if (clipResie_purpose == ResizePurpose.Right)
                    clipRect.xMax += clipResize_draggedFrameOffset * windowState.currentPixelPerFrame;
            }

            //背景
            EditorGUI.DrawRect(clipRect, GetClipBgColor(IsSelect));
            //下面的颜色横条
            var underline = clipRect;
            underline.yMin = underline.yMax - 3f; 
            EditorGUI.DrawRect(underline, IClip.Mute? Color.gray : IClip.GetClipColor());
            //左上角
            var tipLightRect = clipRect;
            tipLightRect.width = 8;
            tipLightRect.height = 8;
            GUI.DrawTexture(tipLightRect,IClip.Mute? Styles.Icon_Corner_Off : Styles.Icon_Corner_On);
            //描边
            clipRect.DrawOutline(1 ,GetClipBorderColor(IsSelect));
            //描述(具体作用)
            DrawLabel(clipRect,IClip.GetClipInfo());
            
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

        /// <summary>
        /// 画Resize用的区域
        /// </summary>
        /// <param name="rect"></param>
        public virtual void DrawResizeHandle(Rect rect)
        {
            var handleWidth = Mathf.Clamp(rect.width * 0.3f, Const.clipMinHandleWidth, Const.clipMaxHandleWidth);

            var rectLeft = rect;
            rectLeft.width = handleWidth;

            var rectRight = rectLeft;
            rectRight.x = rect.xMax - handleWidth;

            EditorGUIUtility.AddCursorRect(rectLeft, MouseCursor.SplitResizeLeftRight);
            SplitResizeHandleEvent(rectLeft, ResizePurpose.Left);

            EditorGUIUtility.AddCursorRect(rectRight, MouseCursor.SplitResizeLeftRight);
            SplitResizeHandleEvent(rectRight, ResizePurpose.Right);
        }

        public override void OnDrawOverlapGUI()
        {


            var window = TimelineWindow.instance;
            var rightTrackView = window.trackRect;
            rightTrackView.xMin = window.timelineRulerRect.xMin;
            
            var rulerNTrack = window.timelineRulerRect;
            rulerNTrack.yMax = rightTrackView.yMax;
            
            var rect = rulerNTrack;
            rect.Debug();
            GUILayout.BeginArea(rect);
            rect.ToOrigin();

            {
                var start = IClip.StartFrame * GUIUtil.windowState.currentPixelPerFrame;
                var end = IClip.EndFrame * GUIUtil.windowState.currentPixelPerFrame;
                Rect clipRect = Rect.MinMaxRect(start, 0, end, rect.yMax);
                if (clipDragging)
                    clipRect.x += clipDrag_draggedPixelOffsetClamped;
                else if (clipResizing)
                { 
                    if(clipResie_purpose == ResizePurpose.Left)
                        clipRect.xMin += clipResize_draggedFrameOffset * windowState.currentPixelPerFrame;
                    if (clipResie_purpose == ResizePurpose.Right)
                        clipRect.xMax += clipResize_draggedFrameOffset * windowState.currentPixelPerFrame;
                }
                float lineMinY = Const.timelineRulerHeight;
                
                void DrawEditingFrameLabel(int frameOffset, bool isLeftSide)
                {
                    var labelWidth = 50f;
                    var originX = isLeftSide ? clipRect.xMin : clipRect.xMax;
                    var rect = Rect.MinMaxRect(originX - labelWidth / 2, 0, originX + labelWidth / 2, lineMinY);
                    var editingEndFrame = isLeftSide? IClip.StartFrame + frameOffset :IClip.EndFrame + frameOffset;
                    Graphics.ShadowLabel(rect, editingEndFrame.ToString(), Styles.editingRulerClipFrameLabel,Color.white,Color.black, 14);
                }
                void DrawEditingFrameLabel_Left(int frameOffset) =>
                    DrawEditingFrameLabel(frameOffset, true);
                void DrawEditingFrameLabel_Right(int frameOffset) =>
                    DrawEditingFrameLabel(frameOffset, false);

                clipRect.Debug(Color.white);
                

                Handles.BeginGUI();
                if (clipDragging)
                {
                    //左边虚线
                    Graphics.DrawDottedLine(new Vector3(clipRect.xMin, lineMinY), new Vector3(clipRect.xMin, clipRect.yMax),4, Color.black);
                    //右边虚线
                    Graphics.DrawDottedLine(new Vector3(clipRect.xMax, lineMinY), new Vector3(clipRect.xMax, clipRect.yMax),4, Color.black);
                    //时间尺上的白影
                    var rulerShadow = Rect.MinMaxRect(clipRect.xMin, 0, clipRect.xMax, lineMinY);
                    var rulerShadowColor = Color.white;
                    rulerShadowColor.a = 0.333f;
                    EditorGUI.DrawRect(rulerShadow, rulerShadowColor);
                    //时间尺上的当前Clip的Start和End对应的帧数
                    DrawEditingFrameLabel_Left(clipDrag_draggedFrameOffset);
                    DrawEditingFrameLabel_Right(clipDrag_draggedFrameOffset);
                }
                else if (clipResizing)
                {
                    if (clipResie_purpose == ResizePurpose.Left)
                    {
                        Graphics.DrawDottedLine(new Vector3(clipRect.xMin, lineMinY), new Vector3(clipRect.xMin, clipRect.yMax),4, Color.black);
                        DrawEditingFrameLabel_Left(clipResize_draggedFrameOffset);
                    }

                    if (clipResie_purpose == ResizePurpose.Right)
                    {
                        Graphics.DrawDottedLine(new Vector3(clipRect.xMax, lineMinY), new Vector3(clipRect.xMax, clipRect.yMax),4, Color.black);
                        DrawEditingFrameLabel_Right(clipResize_draggedFrameOffset);
                    }
                }
                Handles.EndGUI();
            }
            
            GUILayout.EndArea();
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


        public void SplitResizeHandleEvent(Rect position, ResizePurpose resizePurpose)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            SplitResizeHandleEvent(controlID, position, resizePurpose);
        }
        public void SplitResizeHandleEvent(int controlID, Rect position, ResizePurpose resizePurpose)
        {
            var eventType = Event.current.GetTypeForControl(controlID);
            switch (eventType)
            {
                case EventType.MouseDown:
                    {
                        if (position.Contains(Event.current.mousePosition) && Event.current.button == 0)
                        {
                            controlID.AsHotControl();
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
                        if (clipResie_purpose != resizePurpose)
                            return;

                        controlID.CleaHotControl();

                        if (clipResizing)
                        {
                            double rangeOffset = clipResize_draggedFrameOffset / windowState.CurrentFrameRate;
                            if (clipResie_purpose == ResizePurpose.Left)
                                IClip.SetClipStartSafe(IClip.start + rangeOffset);
                            else if (clipResie_purpose == ResizePurpose.Right)
                                IClip.SetClipEndSafe(IClip.end + rangeOffset);
                            IClip.TrySave();

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
                            clipResize_draggedFrameOffset = IClip.ClampClipStartChange_Frame(IClip.StartFrame + windowState.PixelToFrame(pixel)) - IClip.StartFrame;
                        if (clipResie_purpose == ResizePurpose.Right)
                            clipResize_draggedFrameOffset = IClip.ClampClipEndChange_Frame(IClip.EndFrame + windowState.PixelToFrame(pixel)) - IClip.EndFrame;
                        SequenceUndo.PushUndo((Object)IClip, "Resize Clip");
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
                        controlID.AsHotControl();
                        clipDrag_startPosition = Event.current.mousePosition;
                        clipDrag_draggedPixelOffset = 0;
                        clipDrag_draggedPixelOffsetClamped = 0;
                        EventType.MouseDown.Use();
                    }
                    break;
                }
                case EventType.MouseUp:
                {
                    if (!IsSelect)
                        return;
                    
                    controlID.CleaHotControl();

                    if (clipDragging)
                    {
                        double rangeOffset = windowState.PixelToTime(clipDrag_draggedPixelOffsetClamped);
                        IClip.start += rangeOffset;
                        IClip.end += rangeOffset;
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
                    int frames = TimeUtil.ToFrames(windowState.PixelToTime(draggedPixelOffset), windowState.CurrentFrameRate);
                    //Debug.Log($"[draggedPixelOffset:{draggedPixelOffset}] [frames:{frames}] [valid:{IClip.ValidRangeChangeableByFrame(frames)}]");
                    
                    //后面可能改成纯用帧判断，而不是时间
                    int clampedFrames = IClip.ClampToValidFrameOffset(frames);
                    clipDrag_draggedPixelOffsetClamped = windowState.FrameToPixel(clampedFrames);
                    clipDrag_draggedFrameOffset = clampedFrames ;
                        
                    clipDrag_draggedPixelOffset = draggedPixelOffset;

                    SequenceUndo.PushUndo((Object)IClip, "Drag Clip");
                    Repaint();
                    EventType.MouseDrag.Use();
                    break;
                }
            }
        }
        #endregion
    }
}