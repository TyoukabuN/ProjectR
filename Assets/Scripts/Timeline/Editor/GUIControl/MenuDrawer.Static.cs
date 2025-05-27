using System;
using UnityEditor;
using UnityEngine;

namespace PJR.Timeline.Editor
{
    public partial class MenuDrawer
    {
        public struct DrawContext
        {
            public Rect rect;
            public IClip clip;
            public bool selected;
            public Action<Rect> onLeftClick;
            public Action<Rect> onRightClick;
        }
        public static Color GetMenuBgColor(bool selected)
        {
            return selected
                ? Styles.Instance.customSkin.colorSelection
                : Styles.Instance.customSkin.colorTrackHeaderBackground;
        }
        public static void DrawTrackMenu(DrawContext context)
        {
            DrawTrackMenu(
                context.rect,
                context.clip,
                context.selected,
                context.onLeftClick,
                context.onRightClick
            );
        }
        
        public static void DrawTrackMenu(Rect rect, IClip clip) => DrawTrackMenu(rect, clip, false, null, null);
        public static void DrawTrackMenu(Rect rect, IClip clip, bool selected, Action<Rect> onLeftClick,
            Action<Rect> onRightClick)
        {
            EditorGUI.DrawRect(rect, GetMenuBgColor(selected));

            rect.Debug();

            var labelSize = Styles.CalcLabelSize(clip.GetDisplayName());

            GUILayout.Label(new GUIContent(clip.GetDisplayName(), clip.GetDisplayName()), GUILayout.Width(labelSize.x),
                GUILayout.ExpandHeight(true));

            GUILayout.FlexibleSpace();

            using (new MidAlignmentScope.Horizontal())
            {
                if (GUILayout.Button(clip.Mute ? Styles.trackMuteEnabledIcon : Styles.trackMuteDisabledIcon,
                        EditorStyles.iconButton))
                {
                    clip.Mute = !clip.Mute;
                }
            }

            GUILayoutUtility.GetLastRect().Debug(Color.green);

            //DrawMenuHandle(rect);

            var evtRect = rect;
            evtRect.xMin -= Constants.trackMenuLeftSpace;
            EventUtil.MouseEvent.LeftOrRightClick(evtRect, onLeftClick, onRightClick);
        }

        /// <summary>
        /// TrackMenu底部的SplitResizeUpDown
        /// </summary>
        /// <param name="rect"></param>
        public static void DrawMenuHandle(Rect rect)
        {
            rect.yMin = rect.yMax - Constants.trackMenuDragHandleHeight;
            rect.Debug(Color.magenta);
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.SplitResizeUpDown);

            var controlId = GUIUtility.GetControlID(FocusType.Passive);
            switch (Event.current.GetTypeForControl(controlId))
            {
                case EventType.MouseDown:
                {
                    if (rect.Contains(Event.current.mousePosition))
                    {
                        //还不知道有没有必要Resize
                        EventType.MouseDown.Use();
                    }

                    break;
                }
            }
        }
    }
}