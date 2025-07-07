using System;
using NPOI.HSSF.Record.Cont;
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
            //trackView的背景，放这是不想它遮住时间尺刻度
            EditorGUI.DrawRect(trackRect, Styles.Instance.customSkin.colorSequenceBackground);
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

        struct MainScaleMarkInfo
        {
            public int patternIndex;
            public int framePerMainScaleMark;
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
                var sign = -Mathf.Sign(evt.delta.y);
                //按比例缩放系数
                State.currentPixelPerFrameScaleFactor *= 1 + sign * 0.0833f;
                State.currentPixelPerFrameScaleFactor = Mathf.Clamp01(State.currentPixelPerFrameScaleFactor);
                State.currentPixelPerFrame = State.currentPixelPerFrameScaleFactor * Const.MaxPixelPerFrame;
                Repaint();
            });

            timelineRulerRect.Debug();
            
            var rect = timelineRulerRect;
            //新开一个坐标系
            GUILayout.BeginArea(rect);
            //将rect移动回原点方便计算
            if (rect.ToOrigin().Contains(Event.current.mousePosition))
            {
            }
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
            //这里是为了找到一个所占像素大于MinMajorGraduationPixel的主刻度线帧数间隔
            while (pixelPerMainScaleMark < Const.MinMainScaleMarkPixel)
            {
                framePerMainScaleMark *= Const.RulerScaleMarkingPattern[tScaleIndex++ % Const.RulerScaleMarkingPattern.Length];
                pixelPerMainScaleMark = pixelPerFrame * framePerMainScaleMark;
            }
            
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
                if (index > 300)
                {
                    UnityEngine.Debug.LogWarning("绘制时间尺的循环数>300");
                    break;
                }
                
                var mainScaleMarkInfo = GetMaxMainScaleMarkInfo(frame);
                var framePerScaleMark = mainScaleMarkInfo.framePerMainScaleMark * pixelPerFrame;
                
                if (frame % framePerMainScaleMark == 0)
                {
                    GUI.Label(new Rect(totalPixel + 1f, -3f, 50f, 20f), frame.ToString(), Styles.Instance.timeAreaStyles.timelineTick);
                    Handles.DrawLine(new Vector3(totalPixel, rect.height), new Vector3(totalPixel, longTickStartY),0);
                    //DrawVerticalLineFast(totalPixel, longTickStartY, rect.height, new Color(1f, 1f, 1f, 0.5f));
                }
                else
                {
                    //根据比例缩小非主刻度
                    float markPct = GetPct(Const.MinPixelPerScaleMark,50f,framePerScaleMark);
                    float markMinY = Mathf.Lerp(longTickStartY, rect.height, 1f - markPct);// rect.height - cursorHeight * ; 
                    //下到上画
                    Handles.DrawLine(new Vector3(totalPixel, rect.height), new Vector3(totalPixel, markMinY),0);
                }
            }
            Handles.EndGUI();
            GUILayout.EndArea();
            
            //TrackView背景部分的刻度线
            rect = trackRect;
            rect.xMin = timelineRulerRect.xMin;
            GUILayout.BeginArea(rect);
            rect.ToOrigin().Contains(Event.current.mousePosition);
            Handles.BeginGUI();
            totalPixel = 0;
            index = 0;
            var color = Color.white * 0.533f;
            for (
                int frame = 0; 
                totalPixel < rect.width; //限制时间尺区域
                frame += frameStep, //帧数步长
                totalPixel += pixelPerStep,
                index++
            )
            {
                var mainScaleMarkInfo = GetMaxMainScaleMarkInfo(frame);
                var framePerScaleMark = mainScaleMarkInfo.framePerMainScaleMark * pixelPerFrame;

                if(framePerScaleMark > Const.MinPixelPerBgScaleMark)
                {
                    using(new Handles.DrawingScope(color))
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
