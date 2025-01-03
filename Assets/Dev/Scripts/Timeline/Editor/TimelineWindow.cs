using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Styles = PJR.Timeline.Styles;

namespace PJR.Timeline
{
    public partial class TimelineWindow : EditorWindow
    {
        public static TimelineWindow instance { get; private set; }

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
            Draw_Toolbar();
            //Draw_TimelineRuler();
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

                Draw_HeaderEditBar();
            }
        }

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
            GUILayout.BeginArea(timelineRulerRect);
            using(new GUILayout.HorizontalScope())
            {
                GUILayout.Button("Button", GUILayout.Width(64f));
            }
            GUILayout.EndArea();
        }
    }
}
