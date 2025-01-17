using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Styles = PJR.Timeline.Editor.Styles;
using static PJR.Timeline.Editor.TimelineWindow;

namespace PJR.Timeline.Editor
{
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

        GUIStyle backgroundStyle = new GUIStyle(GUI.skin.box)
        {
            padding = new RectOffset(0, 0, 0, 0), // 内边距
            margin = new RectOffset(0, 0, 0, 0),     // 外边距
            alignment = TextAnchor.MiddleLeft        // 文本对齐方式
        };
        public virtual void OnGUI(Rect totalArea)
        {
            using (new GUILayout.HorizontalScope(GUILayout.Width(instance.trackRect.width)))
            {
                var trackMenuAreaWidth = windowState.trackMenuAreaWidth - 2;
                //TrackMenu（左边）
                using (new GUILayout.VerticalScope(GUILayout.Width(trackMenuAreaWidth)))
                {
                    GUILayout.Space(Constants.trackMenuAreaTop);
                    for (int i = 0; i < clips.Count; i++)
                    {
                        GUILayout.Space(Constants.trackMenuPadding);
                        var clip = clips[i];
                        var clipGUI = GetClipGUI(clip);
                        if (clipGUI == null)
                            continue;

                        using (new GUILayout.HorizontalScope(GUILayout.Height(clipGUI.CalculateHeight())))
                        {
                            GUILayout.Space(Constants.trackMenuLeftSpace);
                            var menuRect = GUILayoutUtility.GetRect(0, clipGUI.CalculateHeight(), GUILayout.ExpandWidth(false));
                            menuRect.width = trackMenuAreaWidth - Constants.trackMenuLeftSpace;
                            clipGUI.OnDrawMenu(menuRect);
                        }
                    }
                }
                GUILayoutUtility.GetLastRect().Debug(Color.red);

                //Menu和Track之间的空间
                //用于修改HeaderWidth
                GUILayout.Space(instance.headerSizeHandleRect.width);

                //TrackClip（右边）
                using (new GUILayout.VerticalScope(GUILayout.Width(position.width - windowState.trackMenuAreaWidth + windowState.headerSizeHandleRect.width / 2)))
                {
                    //GUILayoutUtility.GetRect(50, 50).Debug();
                    GUILayout.Space(Constants.trackMenuAreaTop);

                    for (int i = 0; i < clips.Count; i++)
                    {
                        GUILayout.Space(Constants.trackMenuPadding);
                        var clip = clips[i];
                        var clipGUI = GetClipGUI(clip);
                        if (clipGUI == null)
                            continue;

                        using (new GUILayout.HorizontalScope(GUILayout.Height(clipGUI.CalculateHeight())))
                        {
                            clipGUI.OnDrawTrack(GUILayoutUtility.GetRect(0, clipGUI.CalculateHeight()));
                        }
                    }
                }
            }
            GUILayoutUtility.GetLastRect().Debug(Color.red);
        }


        private Dictionary<Clip, ClipGUI> clip2clipGUI;
        public ClipGUI GetClipGUI(Clip clip)
        {
            ClipGUI guiObject = null;
            if (clip == null)
                return guiObject;

            if (clip2clipGUI == null)
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
            clips.Add(new TestClip("TestClip1"));
            clips.Add(new TestClip("TestClip2"));
            clips.Add(new TestClip("TestClip3"));
        }

        public class TestClip : Clip
        {
            public int intValue;

            public TestClip()
            {
                m_Start = 0;
                m_End = Define.SPF_Gane * 10;
                clipName = nameof(TestClip);
            }
            public TestClip(string name):base()
            {
                clipName = name;
            }
        }


        public class TestClipGUI : ClipGUI<TestClip>
        {
            public TestClipGUI(TestClip clip) : base(clip)
            {
            }

            public override void OnDrawMenu(Rect rect)
            {
                base.OnDrawMenu(rect);

            }
        }
        #endregion
    }
}
