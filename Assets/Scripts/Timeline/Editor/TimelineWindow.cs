using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using static UnityEditor.EditorGUI;

namespace PJR.Timeline.Editor
{
    public partial class TimelineWindow : EditorWindow
    {
        public static TimelineWindow instance { get; private set; }

        private WindowState _state;
        public WindowState State => _state ??= new WindowState();

        [MenuItem("PJR/Timeline", false, 1)]
        public static TimelineWindow ShowWindow()
        {
            instance = GetWindow<TimelineWindow>();
            instance.titleContent = new GUIContent("TimelineWindow");
            instance.Focus();
            return instance;
        }
        
        [OnOpenAsset(1)]
        public static bool OnDoubleClick(int instanceID, int line)
        {
            var asset = AssetDatabase.GetMainAssetTypeAtPath(AssetDatabase.GetAssetPath(instanceID));
            if (asset == typeof(Sequence))
            {
                ShowWindow()?.Selection_CheckSelectionChange();
                return true;
            }
            return false;
        }
        private void OnEnable()
        {
            instance = this;
            Selection_CheckSelectionChange();
            RegisterEvent(true);
        }
        
        void OnLostFocus()
        {
            Repaint();
        }
        void RegisterEvent(bool enable)
        {
            Selection.selectionChanged -= OnSelectionChanged;
            if (enable)
            { 
                Selection.selectionChanged += OnSelectionChanged;
            }
        }
        void OnSelectionChanged()
        {
            Selection_CheckSelectionChange();
        }
        public void OnDisable() => Clear();
        public void OnDestroy() => Clear();
        public void Clear()
        {
            RegisterEvent(false);
            TimelineGUIElement.DisposeAll();
        }
        public override void SaveChanges()
        {
            base.SaveChanges();
            State?.SaveSequenceAsset();
        }
        void OnGUI()
        {
            Styles.ReloadStylesIfNeeded();

            if (CheckRepaintRequired())
                return;

            if (!State.editingSequence.IsEmpty && !State.editingSequence.Valid)
            {
                State.editingSequence = EditingSequare.Empty;
                return;
            }

            Draw_Headers(); 
            TrackViewsGUI(); 
        }

        /// <summary>
        /// 检测是不是要重新绘制
        /// </summary>
        bool CheckRepaintRequired()
        {
            if (Event.current.type != EventType.Repaint)
                return false;
            if (!State.requireRepaint)
                return false;
            State.requireRepaint = false;
            //GUIUtility.ExitGUI();
            Repaint();
            return true;
        }
      
        void Draw_Headers()
        {
            using(new GUILayout.VerticalScope())
            {
                using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
                    Draw_ControlBar();

                using (new GUILayout.HorizontalScope())
                {
                    Draw_HeaderEditBar();
                    Draw_TimelineRuler_New();
                    //Draw_TimelineRuler();
                }
            }
        }

        void Draw_ControlBar()
        {
            Draw_DebugButton();
            using (new DisabledScope(instance.State.IsControlBarDisabled()))
            { 
                if (State.debugging)
                    Draw_DebugButtons();
                Draw_GotoBeginingButton();
                Draw_PreviousFrameButton();
                Draw_PlayerButton();
                Draw_NewFrameButton();
                Draw_GotoEndButton();
                GUILayout.FlexibleSpace();
            }
        }

        void Draw_DebugButtons()
        { 
            Draw_RepaintButton();
        }

        public void TrackViewsGUI()
        {
            trackRect.Debug();
            EditorGUI.DrawRect(trackRect, Styles.Instance.customSkin.colorSequenceBackground);

            //Draw_TimelineRuler_TrackView();
                
            //分割TrackMenu和View边界部分
            DrawSpliterAboutTrackMenuAndView();
            
            if (State.NonEditingSequence())
            {
                Draw_NonEditingSequenceTrackView();
                return;
            }    

            GUILayout.BeginArea(trackRect);
            var localTrackRect = trackRect.ToOrigin();
            
            using (new GUIViewportScope(localTrackRect))
            {
                State.TrackGUI.OnGUI(localTrackRect);
            }
            GUILayout.EndArea();
            
            //点击下方空白区域后,取消Hotspot
            if (EventUtil.EventCheck(trackRect, EventType.MouseDown, false))
            {
                State.ClearHotspot();
                Repaint();
                return;
            }
        }

        /// <summary>
        /// 分割TrackMenu和View边界部分
        /// </summary>
        void DrawSpliterAboutTrackMenuAndView()
        {
            if(State.debugging)
                GUIUtil.DebugRect(headerSizeHandleRect, Color.cyan, false, true);

            EditorGUIUtility.AddCursorRect(headerSizeHandleRect, MouseCursor.SplitResizeLeftRight);
            headerSizeHandleRect.DragEventCheck((rect) =>
            {
                float relativeX = Event.current.mousePosition.x - rect.x;
                State.trackMenuAreaWidth = Mathf.Clamp(State.trackMenuAreaWidth + relativeX, Constants.trackMenuMinAreaWidth, Constants.trackMenuMaxAreaWidth);
            });
        }

        public void Draw_NonEditingSequenceTrackView()
        {
            using (new GUIViewportScope(trackRect))
            {
                using (new GUILayout.VerticalScope())
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(Define.Label_NonEditingSequenceTip, Styles.centerAlignmentLabel);
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Nothing", GUILayout.ExpandWidth(false)))
                        {

                        }
                        GUILayout.FlexibleSpace();
                    }
                    GUILayoutUtility.GetLastRect().Debug(Color.red);
                    GUILayout.FlexibleSpace();
                }
                GUILayoutUtility.GetLastRect().Debug();
            }
        }

        #region Control buttons
        void Draw_DebugButton()
        {
            EditorGUI.BeginChangeCheck();
            var enabled = State.debugging;
            State.debugging = GUILayout.Toggle(enabled, Styles.debugContent, EditorStyles.toolbarButton);
        }
        void Draw_RepaintButton()
        {
            EditorGUI.BeginChangeCheck();
            if (GUILayout.Button(Styles.repaint, EditorStyles.toolbarButton))
            {
                Repaint();
                //OdinEditorWindow.InspectObject(Styles.Instance.customSkin);
            } 
        }

        void Draw_GotoBeginingButton()
        {
            if (GUILayout.Button(Styles.gotoBeginingContent, EditorStyles.toolbarButton))
            {
            }
        }
        void Draw_PreviousFrameButton()
        {
            if (GUILayout.Button(Styles.previousFrameContent, EditorStyles.toolbarButton))
            {
            }
        }
        void Draw_PlayerButton()
        {
            if (GUILayout.Button(Styles.playContent, EditorStyles.toolbarButton))
            {
            }
        }
        void Draw_NewFrameButton()
        {
            if (GUILayout.Button(Styles.nextFrameContent, EditorStyles.toolbarButton))
            {
            }
        }
        void Draw_GotoEndButton()
        {
            if (GUILayout.Button(Styles.gotoEndContent, EditorStyles.toolbarButton))
            {
            }
        }
        #endregion

        void Draw_HeaderEditBar()
        {
            using (new EditorGUI.DisabledScope(State.NonEditingSequence()))
            { 
                using (new GUILayout.HorizontalScope(EditorStyles.toolbar, GUILayout.Width(State.trackMenuAreaWidth)))
                {
                    GUILayout.Space(15f);
                    Draw_AddTrackButton();
                    GUILayout.FlexibleSpace();
                }
            }
        }

        void Draw_AddTrackButton()
        {
            if (EditorGUILayout.DropdownButton(Styles.newContent, FocusType.Passive, EditorStyles.toolbarPopup))
            {
                var menu = Global.GetTrackCreateMenu(OnCreateTrack);
                menu?.ShowAsContext();
            }
        }
        void OnCreateTrack(Type type)
        {
            if (State.NonEditingSequence())
                return;
            if (!SequenceUnitCreateHelper.CreateTrack(State.editingSequence.Sequence, type))
                return;
            State.requireRepaint = true;
        }

        public struct RuleScalingDescription
        {
            public static RuleScalingDescription Default 
                => new()
                {
                    IncludeFrames = 1,
                    TotalPixel = MaxPixelPerFrame,
                    RulerScalingPatternIndex = -1,
                };
            //每个scope占多少可见帧
            public int IncludeFrames;
            //每个scope占多少像素
            public int TotalPixel;
            //缩放显示游标模式索引
            public int RulerScalingPatternIndex;
        }

        //Ruler的缩放显示游标模式
        public static int[] RulerScalingPattern = { 5, 2, 3 ,2};
        //一帧可以占的最大像素
        public static int MaxPixelPerFrame = (int)(50f * 3.6); // 180px
        //一帧可以占的最大像素
        public static int SubMaxPixelPerFrame = (int)(50f * 3.0); // 150px
        
        public static int rulerScaleUnit = 3;
        //最小的游标显示像素
        public static int MinCursorVisiblePixel = 3;
        
        public static RuleScalingDescription GetRulerScopeDesc(float scaleFactor)
        {
            int i = -1;
            var ruleScope = RuleScalingDescription.Default;
            
            float pixelPerFrame = 180 * scaleFactor;
            ruleScope.TotalPixel = (int)(ruleScope.IncludeFrames * pixelPerFrame);
            while (ruleScope.TotalPixel < 50)
            {
                if (i > 1000)
                {
                    UnityEngine.Debug.LogError("DeadLoop");
                    return RuleScalingDescription.Default;
                }

                i++;
                ruleScope.IncludeFrames *= RulerScalingPattern[i % RulerScalingPattern.Length];
                ruleScope.RulerScalingPatternIndex = i;
                ruleScope.TotalPixel = (int)(ruleScope.IncludeFrames * pixelPerFrame);
            }

            return ruleScope;
        }

        struct IntervalInfo
        {
            public int patternIndex;
            public int framePerlabelInterval;
        }

        void Draw_TimelineRuler_New()
        {
            if (State.NonEditingSequence())
                return;

            EditorGUI.DrawRect(timelineRulerRect, Styles.Instance.customSkin.colorSubSequenceBackground);
            GUIUtil.CheckWheelEvent(trackRect, evt =>
            {
                UnityEngine.Debug.Log($"[evt.delta.y: {evt.delta.y}]");
                //滑轮上滑是ZoomIn(sign:-1 unit:-3)
                //State.currentRulerScaleFactor += Mathf.Sign(evt.delta.y) * 0.433f;
                var sign = -Mathf.Sign(evt.delta.y);
                State.currentRulerScaleFactor *= 1 + sign * 0.0833f;
                State.currentRulerScaleFactor = Mathf.Clamp01(State.currentRulerScaleFactor);
                State.currentPixelPerFrame = State.currentRulerScaleFactor * 180;
                Repaint();
            });

            timelineRulerRect.Debug();
            //新开一个坐标系
            GUILayout.BeginArea(timelineRulerRect);

            if (timelineRulerRect.ToOrigin().Contains(Event.current.mousePosition))
            {
            }
            Handles.BeginGUI();
            var rect = timelineRulerRect;
            int frameIndex = 0;
            float longTickStartY = 6f;
            float shortTickStartY = rect.height - 6f;

            var rulerScope = GetRulerScopeDesc(State.currentRulerScaleFactor);
            //var cursorWidth = rulerScope.TotalPixel / rulerScope.IncludeFrames;

            int frameUnit = 1;
            int patternIndex = 0;
            float pixelPerFrame = 180 * State.currentRulerScaleFactor;
            float pixelPerCursor = pixelPerFrame * frameUnit;
            while (pixelPerCursor < 5)
            {
                frameUnit *= RulerScalingPattern[patternIndex++ % RulerScalingPattern.Length];
                pixelPerCursor = pixelPerFrame * frameUnit;
            }

            patternIndex = 0;
            int framePerlabelInterval = 1;
            float labelIncludePixel = pixelPerFrame * framePerlabelInterval;
            while (labelIncludePixel < 50)
            {
                framePerlabelInterval *= RulerScalingPattern[patternIndex++ % RulerScalingPattern.Length];
                labelIncludePixel = pixelPerFrame * framePerlabelInterval;
            }


            IntervalInfo GetIntervalInfo(int frame)
            {
                var info = new IntervalInfo
                {
                    patternIndex = 0,
                    framePerlabelInterval = 1
                };
                if (frame <= 0)
                    return info;
                int temp = 1;
                while (true)
                {
                    temp *= RulerScalingPattern[info.patternIndex++ % RulerScalingPattern.Length];
                    if(frame % temp != 0)
                        break;
                    info.framePerlabelInterval = temp;
                }
                return info;
            }

            float GetPct(float min, float max, float value, bool clamp = true)
            {
                if (value <= min)
                    return 0;
                if (value >= max)
                    return 1;
                float range = max - min;
                if(clamp)
                    return Mathf.Clamp01((value - min) / range);
                return (value - min) / range;
            }

            float totalPixel = 0;
            float pixelCount = 0;
            int index = 0;
            int cursorCount = 0;
            for (
                    int frame = 0; 
                    totalPixel < rect.width;
                    frame += frameUnit,
                    cursorCount += frameUnit,
                    totalPixel += pixelPerCursor,
                    pixelCount += pixelPerCursor,
                    index++
                )
            {
                if(index > 200)
                    break;

                var intervalInfo = GetIntervalInfo(frame);
                
                if (frame % framePerlabelInterval == 0)
                {
                    GUI.Label(new Rect(totalPixel + 1f, -3f, 50f, 20f), frame.ToString(), Styles.Instance.timeAreaStyles.timelineTick);
                    Handles.DrawLine(new Vector3(totalPixel, rect.height), new Vector3(totalPixel, longTickStartY),0);
                }
                else
                {
                    var p = intervalInfo.framePerlabelInterval * pixelPerFrame;
                    float cursorPct = GetPct(5,50f,p);
                    //float cursorPct = pixelPerCursor / labelIncludePixel;
                    float cursorMinY = Mathf.Lerp(longTickStartY, rect.height, 1f - cursorPct);// rect.height - cursorHeight * ; 
                    //下到上画
                    Handles.DrawLine(new Vector3(totalPixel, rect.height), new Vector3(totalPixel, cursorMinY),0);
                }
            }
            
            
            // int index = 0;
            // float cursorWidthCounter = 0;
            // for (int i = 0; i < rect.width; 
            //      i += cursorWidth,
            //      cursorWidthCounter += cursorWidth,
            //     index ++)
            // {
            //     if(index > 100)
            //         break;
            //     if (i <= 0)
            //     {
            //         Handles.DrawLine(new Vector3(i, longTickStartY), new Vector3(i, rect.height));
            //         GUI.Label(new Rect(i + 1f, -3f, 50f, 20f), frameIndex.ToString(),
            //             Styles.Instance.timeAreaStyles.timelineTick);
            //         frameIndex += rulerScope.IncludeFrames;
            //     }
            //     else if (cursorWidthCounter >= rulerScope.TotalPixel)
            //     {
            //         cursorWidthCounter -= rulerScope.TotalPixel;
            //         using (new Handles.DrawingScope(Color.white))
            //         {
            //             Handles.DrawLine(new Vector3(i, longTickStartY), new Vector3(i, rect.height));
            //
            //             GUI.Label(new Rect(i + 1f, -3f, 50f, 20f), frameIndex.ToString(),
            //                 Styles.Instance.timeAreaStyles.timelineTick);
            //         }
            //         frameIndex += rulerScope.IncludeFrames;
            //     }
            //     else
            //     {
            //         using (new Handles.DrawingScope(Color.white * 0.733f))
            //             Handles.DrawLine(new Vector3(i, shortTickStartY), new Vector3(i, rect.height));
            //     }
            // }

            Handles.EndGUI();
            GUILayout.EndArea();
        }

        // void Draw_TimelineRuler_TrackView()
        // {
        //     if (State.NonEditingSequence())
        //         return;
        //     
        //     //TrackView下的刻度
        //     var rect = new Rect(
        //         State.trackMenuAreaWidth, 
        //         Constants.clipStartPositionY, 
        //         position.width - State.trackMenuAreaWidth, 
        //         position.height - Constants.clipStartPositionY);
        //
        //     DrawRect(rect, Styles.Instance.customSkin.colorSubSequenceBackground);
        //
        //     int tickStep = Constants.pixelPerFrame + 1 - (State.currentPixelPerFrame / Constants.pixelPerFrame);
        //     tickStep /= 2;
        //     tickStep = Mathf.Max(tickStep, 1);
        //
        //     rect.Debug();
        //     //新开一个坐标系
        //     GUILayout.BeginArea(rect);
        //     //debug用的local坐标log
        //     // if (rect.ToOrigin().Contains(Event.current.mousePosition))
        //     //     Debug.Log(Event.current.mousePosition);
        //     
        //     Handles.BeginGUI();
        //     int frameIndex = 0;
        //     float longTickStartY = 0f;
        //
        //     var color = Color.white * 0.533f;
        //
        //     for (int i = 0; i < rect.width; i += State.currentPixelPerFrame, frameIndex++)
        //     {
        //         if (frameIndex % tickStep == 0)
        //         {
        //             using (new Handles.DrawingScope(color))
        //                 Handles.DrawLine(new Vector3(i, longTickStartY), new Vector3(i, rect.height));
        //         }
        //     }
        //     Handles.EndGUI();
        //     GUILayout.EndArea();
        // }

        //private float sliderValue = 0f;

        public void Draw_MenuTrackBoundary()
        { 
        }
        public static float CustomSlider(Rect position, float value)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);

            Action changeValue = () =>
            {
                if (GUIUtility.hotControl != controlID)
                    return;
                float relativeX = Event.current.mousePosition.x - position.x;
                value = Mathf.Clamp01(relativeX / position.width);
                GUI.changed = true;
                Event.current.Use();
            };

            switch (Event.current.GetTypeForControl(controlID))
            { 
                case EventType.Repaint:
                    {
                        int pixelWidth = (int)Mathf.Lerp(1f, position.width, value);
                        Rect targetRect = new Rect(position) { width = pixelWidth };
                        var color = Color.Lerp(Color.red, Color.green, value);
                        //GUI.DrawTexture(targetRect,)
                        EditorGUI.DrawRect(targetRect, color);
                        break;
                    }
                case EventType.MouseDown:
                    {
                        if (position.Contains(Event.current.mousePosition) && Event.current.button == 0)
                            controlID.AsHotControl();
                        break;
                    }
                case EventType.MouseDrag:
                    {
                        changeValue();
                        break;
                    }
                case EventType.MouseUp:
                    {
                        changeValue();
                        controlID.CleaHotControl();
                        break;
                    }
                default:
                    return value;
            }
            return value;
        }
    }
}
