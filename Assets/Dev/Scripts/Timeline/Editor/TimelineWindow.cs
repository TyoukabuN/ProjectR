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
            EditorGUI.DrawRect(rect, Color.gray);
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
            GUIUtility.CheckWheelEvent(timelineRulerRect, evt =>
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

        public void DrawClips()
        {
            //GUIUtility.DrawBorder(trackRect);

            GUILayout.BeginArea(trackRect);
            GUIUtility.DrawBorder(trackRect.ToLocal(), Color.blue);
            GUILayout.Space(6f);
            GUILayout.Box("123");
            GUIUtility.DrawBorder(GUILayoutUtility.GetLastRect(), Color.red);
            var rect = GUILayoutUtility.GetAspectRect(1).Shrink(3);
            Debug.Log(rect);
            GUIUtility.DrawBorder(rect, Color.red);
            GUILayout.EndArea();
        }

       
    }
}
