using NPOI.HPSF;
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
            Draw_HeaderEditBar();
            Draw_TimelineRuler();
        }

        public void Draw_ControlBar()
        { 

        }
        public void Draw_HeaderEditBar()
        {
            using (new GUILayout.HorizontalScope(EditorStyles.toolbar, GUILayout.Width(sequenceHeaderRect.width)))
            {
                GUILayout.Space(15f);
                Draw_AddTrackButton();
                GUILayout.FlexibleSpace();
            }
        } 

        public void Draw_AddTrackButton()
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
            //GUI.BeginGroup(sequenceTimelineRulerRect);
            GUILayout.BeginArea(sequenceTimelineRulerRect);
            using(new GUILayout.HorizontalScope())
            {
                GUILayout.Button("Button", GUILayout.Width(64f));
            }
            //GUI.Button(new Rect(0, 0, 64, 19), new GUIContent("Button"));
            GUILayout.EndArea();
            //GUI.EndGroup();
        }
    }
}
