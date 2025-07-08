using System;
using UnityEditor;
using UnityEngine;

namespace PJR.Timeline.Editor
{
    public class DefaultMenuDrawer : MenuDrawer
    {
        public DefaultMenuDrawer(Clip clip) : base(clip){}
    }
    public partial class MenuDrawer : TimelineGUIElement
    {
        public Clip Clip;
        public MenuDrawer(Clip clip) => Reset(clip);
        public override object PropertyObject => Clip.Track;

        public void Reset(Clip clip)
        {
            Clip = clip;
            CreateContextMenuMethod = null;
        }

        public Color GetMenuBgColor()
        {
            return windowState.Hotspot == this
                ? Styles.Instance.customSkin.colorSelection
                : Styles.Instance.customSkin.colorTrackHeaderBackground;
        }
        public virtual void Draw(Rect rect) => DrawTrackMenu(rect);
        public void DrawTrackMenu(Rect rect)
        {
            EditorGUI.DrawRect(rect, GetMenuBgColor(IsSelect));

            var labelSize = Styles.CalcLabelSize(Clip.GetClipName());

            GUILayout.Label(new GUIContent(Clip.GetClipName(), Clip.GetClipName()), GUILayout.Width(labelSize.x),
                GUILayout.ExpandHeight(true));

            GUILayout.FlexibleSpace();

            using (new MidAlignmentScope.Horizontal())
            {
                if (GUILayout.Button(Clip.Mute ? Styles.trackMuteEnabledIcon : Styles.trackMuteDisabledIcon,
                        EditorStyles.iconButton))
                {
                    Clip.Mute = !Clip.Mute;
                }
            }

            var evtRect = rect;
            evtRect.xMin -= Const.trackMenuLeftSpace;
            EventUtil.MouseEvent.LeftOrRightClick(evtRect, OnClick, OnContextClick);
        }
        
        public virtual void OnClick(Rect rect)
        {
            if (windowState.Hotspot == this)
                return;
            windowState.Hotspot = this;
            EventType.MouseDown.Use();
            Repaint();
        }

        public virtual void OnContextClick(Rect rect)
        {
            DisplayRightClickMenu(rect);
            EventType.MouseDown.Use();
        }

        public Func<GenericMenu> CreateContextMenuMethod;
        public void DisplayRightClickMenu(Rect rect)
        {
            if (CreateContextMenuMethod == null)
                return;
            var menu = CreateContextMenuMethod.Invoke();
            menu?.ShowAsContext();
        }
    }
}