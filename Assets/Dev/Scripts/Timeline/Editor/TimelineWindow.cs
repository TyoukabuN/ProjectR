using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Styles = PJR.Timeline.Editor.Styles;

namespace PJR.Timeline.Editor
{
    public partial class TimelineWindow : EditorWindow
    {
        public static TimelineWindow instance { get; private set; }

        public WindowState state;

        public TimelineWindow()
        {
            state ??= new WindowState();
        }

        [MenuItem("PJR/Timeline", false, 1)]
        public static void ShowWindow()
        {
            GetWindow<TimelineWindow>();
            instance.Focus();
        }
        private void OnEnable()
        {
            instance = this;
        }
        void OnGUI()
        {
            Styles.ReloadStylesIfNeeded();

            Rect rect = position;
            rect.x = 0;
            rect.y = Constants.timelineAreaYPosition;
            rect.height -= Constants.timelineAreaYPosition;
            EditorGUI.DrawRect(rect, Color.green);

            Draw_Toolbar();
        }

        void Draw_Toolbar()
        {

            using (new GUILayout.VerticalScope())
            {
                using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        Draw_DebugButton();
                        Draw_GotoBeginingButton();
                        Draw_PreviousFrameButton();
                        Draw_PlayerButton();
                        Draw_NewFrameButton();
                        Draw_GotoEndButton();
                        GUILayout.FlexibleSpace();
            headerRect.Debug();
                    }
                }

                using (new GUILayout.HorizontalScope())
                {
                    Draw_HeaderEditBar();
                    Draw_TimelineRuler();
                }

                DrawClips();
            }
        }
        

        #region Control buttons
        void Draw_DebugButton()
        {
            EditorGUI.BeginChangeCheck();
            var enabled = state.debugging;
            state.debugging = GUILayout.Toggle(enabled, Styles.debugContent, EditorStyles.toolbarButton);
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
            using (new GUILayout.HorizontalScope(EditorStyles.toolbar, GUILayout.Width(headerRect.width)))
            {
                GUILayout.Space(15f);
                Draw_AddTrackButton();
                GUILayout.FlexibleSpace();
            }
        }

        void Draw_AddTrackButton()
        {
            if (EditorGUILayout.DropdownButton(Styles.newContent, FocusType.Passive, EditorStyles.toolbarPopup))
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("1"), false, () => { Debug.Log("1"); });
                menu.AddItem(new GUIContent("2"), false, () => { Debug.Log("2"); });
                menu.AddItem(new GUIContent("3"), false, () => { Debug.Log("3"); });
                menu.ShowAsContext();
            }
        }

        void Draw_TimelineRuler()
        {
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

            GUILayout.BeginArea(timelineRulerRect);
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



        public class WindowState
        {
            /// <summary>
            /// current pixel per frame
            /// </summary>
            public int currentPixelPerFrame = Constants.pixelPerFrame;

            public bool debugging = true;

            private TrackGUI trackGUI;
            public TrackGUI TrackGUI => trackGUI ??= new TrackGUI();
        }

        public class TrackGUI
        {
            public List<Clip> clips;
            public WindowState windowState => TimelineWindow.instance.state;

            public TrackGUI()
            {
                DOTest();
            }
            public void OnGUI(Rect totalArea)
            {
                using (new GUILayout.VerticalScope())
                {
                    GUILayout.Space(6f);
                    for (int i = 0; i < clips.Count; i++)
                    {
                        var clip = clips[i];
                        using (new GUILayout.HorizontalScope())
                        {
                            //GUILayout.Button($"Button{i}");
                            //var clipMeumRect = new Rect(0, 1, Constants.defaultHeaderWidth, 100);
                            //clipMeumRect.xMax -= 8f;
                            //clipMeumRect.Debug();
                            //var clipTrackRect = new Rect(Constants.defaultHeaderWidth, 1, totalArea.width - Constants.defaultHeaderWidth, 100);
                            //clipTrackRect.Debug();
                            GUILayout.Space(18f);
                            GUILayout.Box(clip.GetDisplayName(),EditorStyles.helpBox,GUILayout.Height(34));

                            
                        }
                    }
                }

            }

            #region test
            void DOTest()
            { 
                clips = new List<Clip>();
                clips.Add(new TestClip());
                clips.Add(new TestClip());
                clips.Add(new TestClip());
            }

            public class TestClip : Clip
            {
                public int intValue;
            }
            #endregion
        }


        private float sliderValue = 0f;

        public void Draw_MenuTrackBoundary()
        { 

        }

        public void DrawClips()
        {
            EditorGUI.DrawRect(trackRect, Styles.Instance.customSkin.colorSequenceBackground);
            //GUIUtility.DrawBorder(trackRect);
            GUILayout.BeginArea(trackRect);

            var localTrackRect = trackRect.ToOrigin();

            state.TrackGUI.OnGUI(localTrackRect);

            //GUILayout.Space(6f);


            var sliderRt = new Rect(1, 18, 100, 20);
            var position = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.button, GUILayout.MinWidth(100));
            if (Event.current.type == EventType.Repaint)
                Debug.Log($"{position}");
            sliderValue = CustomSlider(position, sliderValue);


            //FlashingButton(sliderRt, new GUIContent("123"), GUI.skin.button);
            //GUIUtil.DebugRect(position, Color.yellow);

            //GUI.Button(new Rect(1, 1, 40, 20), "123");
            //GUI.Label(new Rect(1, 1, 40, 20), GUI.tooltip);

            //GUILayout.Box("123");
            //GUIUtil.DrawBorder(GUILayoutUtility.GetLastRect(), Color.red);
            var rect = GUILayoutUtility.GetAspectRect(10f);
            //Debug.Log($"{Event.current.type} {rect}");
            //GUIUtil.DebugRect(rect, Color.yellow);
            GUILayout.EndArea();
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
