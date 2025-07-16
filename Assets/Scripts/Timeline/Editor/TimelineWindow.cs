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
            if (asset == typeof(SequenceAsset))
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

            //trackView的背景，放这是不想它遮住时间尺刻度
            EditorGUI.DrawRect(trackRect, Styles.Instance.customSkin.colorSequenceBackground);
            Draw_Headers(); 
            TrackViewsGUI();
            Draw_OverlapGUI();
        }

        /// <summary>
        /// 最上层的GUI,时间尺右边之类的
        /// </summary>
        private void Draw_OverlapGUI()
        {
            if (State != null)
                State.Hotspot?.OnDrawOverlapGUI();
            Draw_TimeRulerCursor();
        }
        
        /// <summary>
        /// 有时间尺的游标,就是播放的时候显示当前时间的白线
        /// </summary>
        private void Draw_TimeRulerCursor()
        {
            if (State.NonEditingSequence())
                return;
            //将上面ruler部分和track右边部分合成一个rect
            var rightTrackView = trackRect;
            rightTrackView.xMin = timelineRulerRect.xMin;
            
            var rulerNTrack = timelineRulerRect;
            rulerNTrack.yMax = rightTrackView.yMax;
            
            var rect = rulerNTrack;
            //新开一个坐标系
            GUILayout.BeginArea(rect);
            //将rect移动回原点方便计算,因为也已经BeginArea了
            rect.ToOrigin();
            Handles.BeginGUI();
            
            float posX = State.TimeToPixel(State.SequenceHandle.time);
            float minY = 12f;
            Handles.DrawLine(new Vector3(posX, rect.height), new Vector3(posX, minY),0);
            
            //游标上的帧数
            if (State.draggingRulerCursor)
            {
                var labelWidth = 50f;
                var originX = posX;
                var rectLabel = Rect.MinMaxRect(originX - labelWidth / 2, 0, originX + labelWidth / 2, Const.timelineRulerHeight);
                string cursorLabel = State.ToFrames(State.SequenceHandle.time).ToString();
                Graphics.ShadowLabel(rectLabel, cursorLabel, Styles.editingRulerClipFrameLabel, Color.white,
                    Color.black, 14);
            }

            Handles.EndGUI();
            GUILayout.EndArea();
            
            //拖拽游标
            var dragRect = timelineRulerRect;
            dragRect.Debug();
            dragRect.DragEventCheck(
                rt =>
                {
                    State.draggingRulerCursor = true;
                    float px = Event.current.mousePosition.x - rt.x;
                    int f = State.PixelRoundToFrame(px);
                    if(State.debugging)
                        Debug.Log($"[px:{px}] [f:{f}]");
                    State.SequenceHandle.time = State.FromFrames(f);
                },
                rt =>
                {
                    float px = Event.current.mousePosition.x - rt.x;
                    int f = State.PixelRoundToFrame(px);
                    State.SequenceHandle.time = State.FromFrames(f);
                    State.RefreshWindow(true);
                },
                rt =>
                {
                    State.draggingRulerCursor = false;
                    State.RefreshWindow(true);
                }
            );

        }
        /// <summary>
        /// 检测是不是要重新绘制
        /// </summary>
        bool CheckRepaintRequired()
        {
            if (Event.current.type != EventType.Repaint)
                return false;
            if (!State.RequireRepaint)
                return false;
            State.RequireRepaint = false;
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
                    Draw_TimelineRuler();
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
                
                Draw_TimeCodeGUI();
                GUILayout.FlexibleSpace();
            }
        }
        
        void Draw_DebugButtons()
        { 
            Draw_RepaintButton();
        }

        public void TrackViewsGUI()
        {
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
            GUIUtil.DebugRect(headerSizeHandleRect, Color.cyan, false, true);

            EditorGUIUtility.AddCursorRect(headerSizeHandleRect, MouseCursor.SplitResizeLeftRight);
            headerSizeHandleRect.DragEventCheck((rect) =>
            {
                float relativeX = Event.current.mousePosition.x - rect.x;
                State.trackMenuAreaWidth = Mathf.Clamp(State.trackMenuAreaWidth + relativeX, Const.trackMenuMinAreaWidth, Const.trackMenuMaxAreaWidth);
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
                    GUILayout.FlexibleSpace();
                }
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

        void Draw_TimeCodeGUI()
        {
            //from unity timeline
            // const string timeFieldHint = "TimelineWindow-TimeCodeGUI";
            //
            // EditorGUI.BeginChangeCheck();
            // var currentTime = State.EditingSequenceAsset.SequenceAsset != null ? TimeReferenceUtility.ToTimeString(State.EditingSequenceAsset.time, "0.####") : "0";
            // //var r = EditorGUILayout.GetControlRect(false, EditorGUI.kSingleLineHeight, EditorStyles.toolbarTextField, GUILayout.Width(Const.timeCodeWidth));
            // var r = EditorGUILayout.GetControlRect(false, 18, EditorStyles.toolbarTextField, GUILayout.Width(Const.timeCodeWidth));
            // var id = GUIUtility.GetControlID(timeFieldHint.GetHashCode(), FocusType.Keyboard, r);
            // var newCurrentTime = EditorGUIReflection.DelayedTextField(r, id, GUIContent.none, currentTime, null, EditorStyles.toolbarTextField);
            //
            //
            // if (EditorGUI.EndChangeCheck())
            // {
            //     Debug.Log($"newCurrentTime: {newCurrentTime}");
            //     State.EditingSequenceAsset.time = TimeReferenceUtility.FromTimeString(newCurrentTime);
            // }
            
            //这里先处理以<帧>为单位的的时间修改,后面有需求再加秒
            if (State.SequenceHandle == null)
                return;
            int tFrame = State.ToFrames(State.SequenceHandle.time);
            tFrame = EditorGUILayout.IntField(tFrame);
            if (State.SequenceHandle != null && State.SequenceHandle.Valid)
                State.SequenceHandle.time = State.FromFrames(tFrame);
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
            if (!SequenceUnitCreateHelper.CreateTrack(State.SequenceHandle.SequenceAsset, type))
                return;
            State.RequireRepaint = true;
        }

        struct MainScaleMarkInfo
        {
            public int patternIndex;
            public int framePerMainScaleMark;
        }
        void Draw_TimelineRuler()
        {
            if (State.NonEditingSequence())
                return;

            //将上面ruler部分和track右边部分合成一个rect
            var rightTrackView = trackRect;
            rightTrackView.xMin = timelineRulerRect.xMin;
            
            var rulerNTrack = timelineRulerRect;
            rulerNTrack.yMax = rightTrackView.yMax;
    
            EditorGUI.DrawRect(timelineRulerRect, Styles.Instance.customSkin.colorSubSequenceBackground);
            GUIUtil.CheckWheelEvent(rulerNTrack, evt =>
            {
                //UnityEngine.Debug.Log($"[evt.delta.y: {evt.delta.y}]");
                //滑轮上滑是ZoomIn(sign:-1 unit:-3)
                var sign = -Mathf.Sign(evt.delta.y);
                //按比例缩放系数
                State.currentPixelPerFrameScaleFactor *= 1 + sign * Const.ScalingSpeed;
                State.currentPixelPerFrameScaleFactor = Mathf.Clamp01(State.currentPixelPerFrameScaleFactor);
                State.currentPixelPerFrame = State.currentPixelPerFrameScaleFactor * Const.MaxPixelPerFrame;
                Repaint();
            });

            
            var rect = rulerNTrack;
            //新开一个坐标系
            GUILayout.BeginArea(rect);
            //将rect移动回原点方便计算,因为也已经BeginArea了
            rect.ToOrigin();
            
            Handles.BeginGUI();
            int frameIndex = 0;
            float longTickStartY = 6f;

            //找<刻度帧数步长>和对应<每步长所占像素>
            
            int frameStep = 1;//时间尺绘制循环的帧数步长  
            int tScaleIndex = 0;//临时时间尺主刻度线间隔模式索引
            //时间尺的缩放的本质是对<像素/帧>的缩放
            float pixelPerFrame = State.currentPixelPerFrame;
            //绘制时间尺的时候每一步都会绘制一个刻度线
            //每个刻度线的间隔不能小于MinCursorVisiblePixel(3)像素,否则增加绘制循环步长 (为了缩放时间尺的视觉效果)
            //从而增加间隔刻度线像素间隔
            float pixelPerStep = pixelPerFrame * frameStep;
            while (pixelPerStep < Const.MinPixelPerScaleMark)
            {
                frameStep *= Const.RulerScaleMarkingPattern[tScaleIndex++ % Const.RulerScaleMarkingPattern.Length];
                pixelPerStep = pixelPerFrame * frameStep;
            }
            
            //找<每主刻度所占帧数>和对应<每主刻度所占像素>
            
            tScaleIndex = 0;
            //每个主刻度占多少帧
            int framePerMainScaleMark = 1;
            //每个主刻度占多少像素
            float pixelPerMainScaleMark = pixelPerFrame * framePerMainScaleMark;
            //找到可以正常绘制的刻度线像素间隔，以及对应的时间尺主刻度线间隔模式索引
            //{5，2，3，2} i=1循环累乘可以得到下面的主刻度帧数间隔 
            //1，5，10，30，60，300，600，1800，3600，18000，36000，108000，216000...
            //这里是为了找到一个所占像素大于MinMainScaleMarkPixel的主刻度线帧数间隔
            while (pixelPerMainScaleMark < Const.MinMainScaleMarkPixel)
            {
                framePerMainScaleMark *= Const.RulerScaleMarkingPattern[tScaleIndex++ % Const.RulerScaleMarkingPattern.Length];
                pixelPerMainScaleMark = pixelPerFrame * framePerMainScaleMark;
            }


            int overflowNum = 500;
            
            float totalPixel = 0;
            int index = 0;
            for (
                    int frame = 0; 
                    totalPixel < rect.width; //限制时间尺区域
                    frame += frameStep, //帧数步长
                    totalPixel += pixelPerStep,
                    index++
                )
            {
                if (index > overflowNum)
                {
                    UnityEngine.Debug.LogWarning($"绘制时间尺的循环数>{overflowNum}");
                    break;
                }
                
                var mainScaleMarkInfo = GetMaxMainScaleMarkInfo(frame);
                var framePerScaleMark = mainScaleMarkInfo.framePerMainScaleMark * pixelPerFrame;
                
                if (frame % framePerMainScaleMark == 0)
                {
                    GUI.Label(new Rect(totalPixel + 1f, -3f, 50f, 20f), frame.ToString(), Styles.Instance.timeAreaStyles.timelineTick);
                    Handles.DrawLine(new Vector3(totalPixel, timelineRulerRect.height), new Vector3(totalPixel, longTickStartY),0);
                    //DrawVerticalLineFast(totalPixel, longTickStartY, rect.height, new Color(1f, 1f, 1f, 0.5f));
                }
                else
                {
                    //根据比例缩小非主刻度
                    float markPct = GetPct(Const.MinPixelPerScaleMark,50f,framePerScaleMark);
                    float markMinY = Mathf.Lerp(longTickStartY, timelineRulerRect.height, 1f - markPct);// rect.height - cursorHeight * ; 
                    //下到上画
                    Handles.DrawLine(new Vector3(totalPixel, timelineRulerRect.height), new Vector3(totalPixel, markMinY),0);
                }
                
                //TrackView背景部分的透明的(就是暗点的)刻度线
                if(index <=0 || framePerScaleMark > Const.MinPixelPerBgScaleMark)
                {
                    using(new Handles.DrawingScope(Color.white * 0.333f))
                        Handles.DrawLine(new Vector3(totalPixel, rect.height), new Vector3(totalPixel, 0),0);
                }
            }
            Handles.EndGUI();
            GUILayout.EndArea();
        }
        
        /// <summary>
        /// 根据刻度帧数找最大的主刻度信息
        /// </summary>
        /// <param name="frame">刻度帧数</param>
        /// <returns></returns>
        MainScaleMarkInfo GetMaxMainScaleMarkInfo(int frame)
        {
            var info = new MainScaleMarkInfo
            {
                patternIndex = 0,
                framePerMainScaleMark = 1
            };
            if (frame <= 0)
                return info;
            int temp = 1;
            while (true)
            {
                temp *= Const.RulerScaleMarkingPattern[info.patternIndex++ % Const.RulerScaleMarkingPattern.Length];
                if(frame % temp != 0)
                    break;
                info.framePerMainScaleMark = temp;
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
        
        public static void DrawVerticalLineFast(float x, float minY, float maxY, Color color)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                GL.Color(color);
                GL.Vertex(new Vector3(x - 0.5f, minY, 0.0f));
                GL.Vertex(new Vector3(x + 0.5f, minY, 0.0f));
                GL.Vertex(new Vector3(x + 0.5f, maxY, 0.0f));
                GL.Vertex(new Vector3(x - 0.5f, maxY, 0.0f));
            }
            else
            {
                GL.Color(color);
                GL.Vertex(new Vector3(x, minY, 0.0f));
                GL.Vertex(new Vector3(x, maxY, 0.0f));
            }
        }
     
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
