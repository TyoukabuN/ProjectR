using System;
using UnityEditor;
using UnityEngine;

namespace PJR.Timeline.Editor
{
    public partial class MenuDrawer
    {
        public static Color GetMenuBgColor(bool selected)
        {
            return selected
                ? Styles.Instance.customSkin.colorSelection
                : Styles.Instance.customSkin.colorTrackHeaderBackground;
        }
        /// <summary>
        /// TrackMenu底部的SplitResizeUpDown
        /// </summary>
        /// <param name="rect"></param>
        public static void DrawMenuHandle(Rect rect)
        {
            rect.yMin = rect.yMax - Const.trackMenuDragHandleHeight;
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