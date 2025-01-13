using NPOI.SS.Formula.Functions;
using Sirenix.OdinInspector.Editor;
using System;
using System.Collections.Generic;
using System.Security.Policy;
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

            //Rect rect = position;
            //rect.x = 0;
            //rect.y = Constants.timelineAreaYPosition;
            //rect.height -= Constants.timelineAreaYPosition;
            //EditorGUI.DrawRect(rect, Color.green);

            Draw_Toolbar();
        }

        void Draw_Toolbar()
        {

            using (new GUILayout.VerticalScope())
            {

                using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
                {
                    Draw_DebugButton();
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

                using (new GUILayout.HorizontalScope())
                {
                    Draw_HeaderEditBar();
                    Draw_TimelineRuler();
                }

                //segment-line trackMenu-trackClip
                var rect = GUILayoutUtility.GetRect(3, position.height);
                rect.xMin = state.trackMenuAreaWidth - 2;
                rect.xMax = state.trackMenuAreaWidth + 2;
                state.headerSizeHandleRect = rect;

                DrawClips();

                //segment-line trackMenu-trackClip
                GUIUtil.DebugRect(rect, Color.green, false, true);

                rect.DragEventCheck((rect) =>
                {
                    float relativeX = Event.current.mousePosition.x - rect.x;
                    state.trackMenuAreaWidth = Mathf.Clamp(state.trackMenuAreaWidth + relativeX, Constants.trackMenuMinAreaWidth, Constants.trackMenuMaxAreaWidth);
                });
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
            //using (new GUILayout.HorizontalScope(EditorStyles.toolbar, GUILayout.Width(headerRect.width)))
            using (new GUILayout.HorizontalScope(EditorStyles.toolbar, GUILayout.Width(state.trackMenuAreaWidth)))
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

            public float trackMenuAreaWidth = Constants.trackMenuDefaultAreaWidth;

            public Rect headerSizeHandleRect;
        }

        public class TrackGUI
        {
            public List<Clip> clips;
            public WindowState windowState => TimelineWindow.instance.state;

            public Rect position => TimelineWindow.instance.position;
            public TrackGUI()
            {
                DOTest();
            }

            public virtual float CalculateHeight()
            {
                return Constants.trackHeight;
            }
            public virtual void OnGUI(Rect totalArea)
            {
                using (new GUILayout.HorizontalScope())
                {
                    //TrackMenu（左边）
                    using (new GUILayout.VerticalScope(GUILayout.Width(windowState.trackMenuAreaWidth - windowState.headerSizeHandleRect.width / 2)))
                    {
                        GUILayout.Space(Constants.trackMenuAreaTop);
                        for (int i = 0; i < clips.Count; i++)
                        {
                            GUILayout.Space(Constants.trackMenuPadding);
                            var clip = clips[i];
                            var clipGUI = GetClipGUI(clip);
                            if (clipGUI == null)
                                continue;

                            using (new GUILayout.HorizontalScope())
                            {
                                GUILayout.Space(Constants.trackMenuLeftSpace);
                                clipGUI.OnDrawMenu(GUILayoutUtility.GetRect(0, clipGUI.CalculateHeight()));
                            }
                        }
                    }

                    //Menu和Track之间的空间
                    //用于修改HeaderWidth
                    GUILayout.Space(4);

                    //TrackClip（右边）
                    using (new GUILayout.VerticalScope(GUILayout.Width(position.width - windowState.trackMenuAreaWidth + windowState.headerSizeHandleRect.width / 2)))
                    {
                        GUILayout.Space(Constants.trackMenuAreaTop);

                        for (int i = 0; i < clips.Count; i++)
                        {
                            GUILayout.Space(Constants.trackMenuPadding);
                            var clip = clips[i];
                            var clipGUI = GetClipGUI(clip);
                            if (clipGUI == null)
                                continue;

                            using (new GUILayout.HorizontalScope())
                            {
                                clipGUI.OnDrawTrack(GUILayoutUtility.GetRect(0, clipGUI.CalculateHeight()));
                            }
                        }
                    }
                }
            }

            private Dictionary<Clip, ClipGUI> clip2clipGUI; 
            public ClipGUI GetClipGUI(Clip clip)
            {
                ClipGUI guiObject = null;
                if (clip == null)
                    return guiObject;

                if(clip2clipGUI == null)
                    clip2clipGUI = new Dictionary<Clip, ClipGUI>();

                #region test
                if (clip2clipGUI.TryGetValue(clip, out guiObject))
                    return guiObject;

                if (clip is TestClip)
                { 
                    guiObject = new TestClipGUI(clip as TestClip);
                    clip2clipGUI[clip] = guiObject;
                }

                return null;
                #endregion
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

                public override string GetDisplayName()
                {
                    return "TestClip";
                }
            }

            public static Clip hotClip = null;
            public class TestClipGUI : ClipGUI<TestClip>
            {
                public TestClipGUI(TestClip clip) : base(clip)
                { 
                }

                public override void OnDrawMenu(Rect menuRect)
                {
                    base.OnDrawMenu(menuRect);

                }
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
            GUILayout.BeginArea(trackRect);

            var localTrackRect = trackRect.ToOrigin();

            state.TrackGUI.OnGUI(localTrackRect);

            var position = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.button, GUILayout.MinWidth(100));
            if (Event.current.type == EventType.Repaint)
                Debug.Log($"{position}");
            sliderValue = CustomSlider(position, sliderValue);

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
