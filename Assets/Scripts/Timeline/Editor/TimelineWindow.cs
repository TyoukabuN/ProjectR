using System;
using System.Collections.Generic;
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

            Draw_TimelineRuler_TrackView();
                
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
            if (!SequenceUnitCreateHelper.CreateTrack(State.editingSequence.SequenceAsset, type))
                return;
            State.requireRepaint = true;
        }

        void Draw_TimelineRuler()
        {
            if (State.NonEditingSequence())
                return;

            EditorGUI.DrawRect(timelineRulerRect, Styles.Instance.customSkin.colorSubSequenceBackground);
            GUIUtil.CheckWheelEvent(timelineRulerRect, evt =>
            {
                State.currentPixelPerFrame -= (int)evt.delta.y;
                State.currentPixelPerFrame = Mathf.Clamp(State.currentPixelPerFrame, Constants.pixelPerFrame, Constants.maxPixelPerFrame);
                Repaint();
            });

            int tickStep = Constants.pixelPerFrame + 1 - (State.currentPixelPerFrame / Constants.pixelPerFrame);
            tickStep /= 2;
            tickStep = Mathf.Max(tickStep, 1);

            timelineRulerRect.Debug();
            //新开一个坐标系
            GUILayout.BeginArea(timelineRulerRect);

            if (timelineRulerRect.ToOrigin().Contains(Event.current.mousePosition))
            { 
                //Debug.Log(Event.current.mousePosition);
            }
            Handles.BeginGUI();
            var rect = timelineRulerRect;
            int frameIndex = 0;
            float longTickStartY = 6f;
            float shortTickStartY = rect.height - 6f;


            for (int i = 0; i < rect.width; i += State.currentPixelPerFrame, frameIndex++)
            {
                if (frameIndex % tickStep == 0)
                {
                    using (new Handles.DrawingScope(Color.white))
                    {
                        Handles.DrawLine(new Vector3(i, longTickStartY), new Vector3(i, rect.height));

                        GUI.Label(new Rect(i + 1f, -3f, 40f, 20f), frameIndex.ToString(),
                            Styles.Instance.timeAreaStyles.timelineTick);
                    }
                }
                else
                {
                    using (new Handles.DrawingScope(Color.white * 0.733f))
                        Handles.DrawLine(new Vector3(i, shortTickStartY), new Vector3(i, rect.height));
                }
            }
            Handles.EndGUI();
            GUILayout.EndArea();


            //TrackView下的刻度
            var rulerInTrackView = new Rect(
                State.trackMenuAreaWidth, 
                Constants.clipStartPositionY, 
                position.width - State.trackMenuAreaWidth, 
                position.height - Constants.clipStartPositionY);
            
            //新开一个坐标系
            GUILayout.BeginArea(rulerInTrackView);
            Handles.BeginGUI();
            
            rect = rulerInTrackView;
            frameIndex = 0;
            longTickStartY = 0f;
            shortTickStartY = 0f;

            for (int i = 0; i < rect.width; i += State.currentPixelPerFrame, frameIndex++)
            {
                if (frameIndex % tickStep == 0)
                {
                    using (new Handles.DrawingScope(Color.white))
                    {
                        Handles.DrawLine(new Vector3(i, longTickStartY), new Vector3(i, rect.height));

                        GUI.Label(new Rect(i + 1f, -3f, 40f, 20f), frameIndex.ToString(),
                            Styles.Instance.timeAreaStyles.timelineTick);
                    }
                }
                else
                {
                    using (new Handles.DrawingScope(Color.white * 0.733f))
                        Handles.DrawLine(new Vector3(i, shortTickStartY), new Vector3(i, rect.height));
                }
            }
            Handles.EndGUI();
            GUILayout.EndArea();
        }

        void Draw_TimelineRuler_TrackView()
        {
            if (State.NonEditingSequence())
                return;
            
            //TrackView下的刻度
            var rect = new Rect(
                State.trackMenuAreaWidth, 
                Constants.clipStartPositionY, 
                position.width - State.trackMenuAreaWidth, 
                position.height - Constants.clipStartPositionY);

            DrawRect(rect, Styles.Instance.customSkin.colorSubSequenceBackground);

            int tickStep = Constants.pixelPerFrame + 1 - (State.currentPixelPerFrame / Constants.pixelPerFrame);
            tickStep /= 2;
            tickStep = Mathf.Max(tickStep, 1);

            rect.Debug();
            //新开一个坐标系
            GUILayout.BeginArea(rect);
            //debug用的local坐标log
            // if (rect.ToOrigin().Contains(Event.current.mousePosition))
            //     Debug.Log(Event.current.mousePosition);
            
            Handles.BeginGUI();
            int frameIndex = 0;
            float longTickStartY = 0f;

            var color = Color.white * 0.533f;

            for (int i = 0; i < rect.width; i += State.currentPixelPerFrame, frameIndex++)
            {
                if (frameIndex % tickStep == 0)
                {
                    using (new Handles.DrawingScope(color))
                        Handles.DrawLine(new Vector3(i, longTickStartY), new Vector3(i, rect.height));
                }
            }
            Handles.EndGUI();
            GUILayout.EndArea();
        }

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
