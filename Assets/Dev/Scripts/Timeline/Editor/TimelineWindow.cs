using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUI;
using Styles = PJR.Timeline.Editor.Styles;
using PJR.Timeline;

namespace PJR.Timeline.Editor
{
    public partial class TimelineWindow : EditorWindow
    {
        public static TimelineWindow instance { get; private set; }

        private WindowState m_State;
        public WindowState state => m_State ??= new WindowState();

        [MenuItem("PJR/Timeline", false, 1)]
        public static void ShowWindow()
        {
            GetWindow<TimelineWindow>();
            instance.Focus();
        }
        private void OnEnable()
        {
            instance = this;
            RegisterEvent(true);
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
            CheckSelectionChange();
        }
        public void OnDestroy()
        {
            RegisterEvent(false);
        }

        void OnGUI()
        {
            Styles.ReloadStylesIfNeeded();

            //GUILayoutLab();

            Draw_Headers();
            TrackViewsGUI();
        }

        void GUILayoutLab()
        {
            var r1 = new Rect(0, 0, 500, 200);
            r1.Debug();

            GUILayout.Space(10);

            //using (new GUILayout.HorizontalScope(GUILayout.Width(250), GUILayout.Height(100), GUILayout.ExpandWidth(false)))
            using (new GUILayout.HorizontalScope(
                GUILayout.Height(100),
                GUILayout.Width(200)
                ))
            {
                GUILayout.Space(50);
                var rect = GUILayoutUtility.GetRect(200, 50);
                //rect.width = 200 - 10;
                //rect.height = 50;
                EditorGUI.DrawRect(rect, Styles.Instance.customSkin.colorTrackHeaderBackground);
                //rect.Debug(Color.red);


                //GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
                //buttonStyle.alignment = TextAnchor.MiddleLeft;
                //buttonStyle.border.left = 10;
                //GUILayout.FlexibleSpace();
                //GUILayout.Button("123", buttonStyle, GUILayout.ExpandWidth(true));
                //GUILayout.FlexibleSpace();
                //GUILayoutUtility.GetLastRect().Debug();


                //var r2 = GUILayoutUtility.GetRect(50, 50,GUILayout.ExpandWidth(false));// new Rect(0, 0, 250, 50);
                //EditorGUI.DrawRect(r2, Color.green);
                //r2.Debug(Color.red);
            }

            GUILayoutUtility.GetLastRect().Debug(Color.blue);
        }

        void Draw_Headers()
        {
            using (new GUILayout.VerticalScope())
            {
                using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
                {
                    Draw_ControlBar();
                }

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
            using (new DisabledScope(instance.state.DisableControlBar()))
            { 
                if (state.debugging)
                {
                    Draw_RepaintButton();
                }
                Draw_GotoBeginingButton();
                Draw_PreviousFrameButton();
                Draw_PlayerButton();
                Draw_NewFrameButton();
                Draw_GotoEndButton();
                GUILayout.FlexibleSpace();
                //headerRect.Debug();
            }
        }

        public void TrackViewsGUI()
        {
            trackRect.Debug();
            EditorGUI.DrawRect(trackRect, Styles.Instance.customSkin.colorSequenceBackground);

            if (GUIUtil.EventCheck(trackRect, EventType.MouseDown))
            {
                state.ClearHotspots();
                Repaint();
                return;
            }

            if (state.NonEditingSequence())
            {
                Draw_NonEditingSequenceTrackView();
                return;
            }    

            GUILayout.BeginArea(trackRect);
            var localTrackRect = trackRect.ToOrigin();

            using (new GUIViewportScope(localTrackRect))
            {
                state.TrackGUI.OnGUI(localTrackRect);
                //var position = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.button, GUILayout.MinWidth(100));
                //sliderValue = CustomSlider(position, sliderValue);
            }

            GUILayout.EndArea();

            //segment-line trackMenu-trackClip
            GUIUtil.DebugRect(headerSizeHandleRect, Color.green, false, true);

            EditorGUIUtility.AddCursorRect(headerSizeHandleRect, MouseCursor.SplitResizeLeftRight);
            headerSizeHandleRect.DragEventCheck((rect) =>
            {
                float relativeX = Event.current.mousePosition.x - rect.x;
                state.trackMenuAreaWidth = Mathf.Clamp(state.trackMenuAreaWidth + relativeX, Constants.trackMenuMinAreaWidth, Constants.trackMenuMaxAreaWidth);
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
            var enabled = state.debugging;
            state.debugging = GUILayout.Toggle(enabled, Styles.debugContent, EditorStyles.toolbarButton);
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
            using (new EditorGUI.DisabledScope(state.NonEditingSequence()))
            { 
                using (new GUILayout.HorizontalScope(EditorStyles.toolbar, GUILayout.Width(state.trackMenuAreaWidth)))
                {
                    GUILayout.Space(15f);
                    Draw_AddTrackButton();
                    GUILayout.FlexibleSpace();
                }
            }
        }

        public class TestClip : Clip
        { 
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
            if (state.NonEditingSequence())
                return;
            var tracks = state.editingSequence.Sequence.Tracks;

            //var track = ScriptableObject.CreateInstance<Track>();
            var track = new Track();
            track.clips = new IClip[] {
                ScriptableObject.CreateInstance(type) as IClip,
            };

            ArrayUtility.Add(ref tracks, track);
            state.editingSequence.Sequence.Tracks = tracks;
            if(state.editingSequence.Asset != null)
                EditorUtility.SetDirty(state.editingSequence.Asset);
            Repaint();
        }

        void Draw_TimelineRuler()
        {
            if (state.NonEditingSequence())
                return;

            EditorGUI.DrawRect(timelineRulerRect, Styles.Instance.customSkin.colorSubSequenceBackground);
            GUIUtil.CheckWheelEvent(timelineRulerRect, evt =>
            {
                state.currentPixelPerFrame -= (int)evt.delta.y;
                state.currentPixelPerFrame = Mathf.Clamp(state.currentPixelPerFrame, Constants.pixelPerFrame, Constants.maxPixelPerFrame);
                Repaint();
            });

            int tickStep = Constants.pixelPerFrame + 1 - (state.currentPixelPerFrame / Constants.pixelPerFrame);
            tickStep /= 2;
            tickStep = Mathf.Max(tickStep, 1);

            timelineRulerRect.Debug();

            GUILayout.BeginArea(timelineRulerRect);

            if (timelineRulerRect.ToOrigin().Contains(Event.current.mousePosition))
            { 
                Debug.Log(Event.current.mousePosition);
            }
            Handles.BeginGUI();
            var rect = timelineRulerRect;
            int frameIndex = 0;
            float longTickStartY = 6f;
            float shortTickStartY = rect.height - 6f;


            for (int i = 0; i < rect.width; i += state.currentPixelPerFrame, frameIndex++)
            {
                if (frameIndex % tickStep == 0)
                {
                    Handles.color = Color.white;
                    Handles.DrawLine(new Vector3(i, longTickStartY), new Vector3(i, rect.height));

                    GUI.Label(new Rect(i, -3f, 40f, 20f), frameIndex.ToString(), Styles.Instance.timeAreaStyles.timelineTick);
                }
                else
                {
                    Handles.DrawLine(new Vector3(i, shortTickStartY), new Vector3(i, rect.height));
                }
            }
            Handles.EndGUI();
            GUILayout.EndArea();
        }

        private float sliderValue = 0f;

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
                            GUIUtility.hotControl = controlID;
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
                        if (GUIUtility.hotControl == controlID)
                            GUIUtility.hotControl = 0;
                        break;
                    }
                default:
                    return value;
            }
            return value;
        }
    }
}
