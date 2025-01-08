using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
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
                        Draw_GotoBeginingButton();
                        Draw_PreviousFrameButton();
                        Draw_PlayerButton();
                        Draw_NewFrameButton();
                        Draw_GotoEndButton();
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
        void Draw_GotoBeginingButton()
        {
            if (GUILayout.Button(Styles.gotoBeginingContent, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
            {
            }
        }

        void Draw_PreviousFrameButton()
        {
            if (GUILayout.Button(Styles.previousFrameContent, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
            {
            }
        }
        void Draw_PlayerButton()
        {
            if (GUILayout.Button(Styles.playContent, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
            {
            }
        }
        void Draw_NewFrameButton()
        {
            if (GUILayout.Button(Styles.nextFrameContent, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
            {
            }
        }
        void Draw_GotoEndButton()
        {
            if (GUILayout.Button(Styles.gotoEndContent, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
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
            var style = Styles.newContent;
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
        }

        private float sliderValue = 0f;
        public void DrawClips()
        {
            EditorGUI.DrawRect(trackRect, Styles.Instance.customSkin.colorSequenceBackground);
            //GUIUtility.DrawBorder(trackRect);
            GUILayout.BeginArea(trackRect);
            GUIUtil.DrawBorder(trackRect.ToLocal(), Color.blue);
            GUILayout.Space(6f);


            var sliderRt = new Rect(1, 18, 100, 20);
            var position = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.button, GUILayout.MinWidth(100));
            if (Event.current.type == EventType.Repaint)
                Debug.Log($"{position}");
            sliderValue = CustomSlider(position, sliderValue);


            //FlashingButton(sliderRt, new GUIContent("123"), GUI.skin.button);
            GUIUtil.DrawBorder(position, Color.yellow);

            //GUI.Button(new Rect(1, 1, 40, 20), "123");
            //GUI.Label(new Rect(1, 1, 40, 20), GUI.tooltip);

            //GUILayout.Box("123");
            //GUIUtil.DrawBorder(GUILayoutUtility.GetLastRect(), Color.red);
            var rect = GUILayoutUtility.GetAspectRect(10f);
            //Debug.Log($"{Event.current.type} {rect}");
            GUIUtil.DrawBorder(rect, Color.yellow);
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

        public static bool FlashingButton(Rect rc, GUIContent content, GUIStyle style)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);

            // Get (or create) the state object
            var state = (FlashingButtonInfo)GUIUtility.GetStateObject(
                                                 typeof(FlashingButtonInfo),
                                                 controlID);

            switch (Event.current.GetTypeForControl(controlID))
            {
                case EventType.Repaint:
                    {
                        var color = state.IsFlashing(controlID)
                            ? Color.red
                            : Color.white;
                        //style.Draw(rc, content, controlID);
                        EditorGUI.DrawRect(rc, color);
                        break;
                    }
                case EventType.MouseDrag:
                    {
                        Event.current.Use();
                        GUI.changed = true; 
                        break;
                    }
                case EventType.MouseDown:
                    {
                        if (rc.Contains(Event.current.mousePosition)
                         && Event.current.button == 0
                         && GUIUtility.hotControl == 0)
                        {
                            GUIUtility.hotControl = controlID;
                            state.MouseDownNow();
                        }
                        break;
                    }
                case EventType.MouseUp:
                    {
                        if (GUIUtility.hotControl == controlID)
                            GUIUtility.hotControl = 0;
                        break;
                    }
            }

            return GUIUtility.hotControl == controlID;
        }

        public class FlashingButtonInfo
        {
            private double mouseDownAt;

            public void MouseDownNow()
            {
                mouseDownAt = EditorApplication.timeSinceStartup;
            }

            public bool IsFlashing(int controlID)
            {
                if (GUIUtility.hotControl != controlID)
                    return false;

                double elapsedTime = EditorApplication.timeSinceStartup - mouseDownAt;
                Debug.Log(elapsedTime);
                if (elapsedTime < 2f)
                    return false;

                return (int)((elapsedTime - 2f) / 0.1f) % 2 == 0;
            }
        }
    }
}
