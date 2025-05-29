using System;
using UnityEditor;
using UnityEngine;

namespace PJR.Timeline.Editor
{
    public class DefaultMenuDrawer : MenuDrawer
    {
        public DefaultMenuDrawer(IClip clip) : base(clip){}
    }
    public partial class MenuDrawer : TimelineGUIElement
    {
        public IClip Clip;
        public MenuDrawer(IClip clip) => Reset(clip);
        public void Reset(IClip clip)
        {
            Clip = clip;
        }

        public Color GetMenuBgColor()
        {
            return windowState.Hotspot == this
                ? Styles.Instance.customSkin.colorSelection
                : Styles.Instance.customSkin.colorTrackHeaderBackground;
        }

        public DrawContext GetDrawContext(Rect rect)
        {
            return new()
            {
                rect = rect,
                clip = Clip,
                selected = IsSelect,
                onLeftClick = OnClick,
                onRightClick = OnContextClick
            };
        }
        public virtual void Draw(Rect rect) => DrawTrackMenu(rect);
        public void DrawTrackMenu(Rect rect)
        {
            EditorGUI.DrawRect(rect, GetMenuBgColor(IsSelect));

            rect.Debug();

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

            GUILayoutUtility.GetLastRect().Debug(Color.green);

            var evtRect = rect;
            evtRect.xMin -= Constants.trackMenuLeftSpace;
            EventUtil.MouseEvent.LeftOrRightClick(evtRect, OnClick, OnContextClick);
        }
        
        protected virtual void OnClick(Rect rect)
        {
            if (windowState.Hotspot == this)
                return;
            windowState.Hotspot = this;
            EventType.MouseDown.Use();
            Repaint();
        }

        protected virtual void OnContextClick(Rect rect)
        {
            DisplayRightClickMenu(rect);
            EventType.MouseDown.Use();
        }

        /// <summary>
        /// 右键默认选项
        /// </summary>
        /// <returns></returns>
        protected virtual GenericMenu GetGenericMenu(IClip clip)
        {
            var menu = new GenericMenu();
            menu.AddDisabledItem(new GUIContent("右键默认选项"));
            menu.AddSeparator($"{clip.GetClipName()}");
            return menu;
        }

        public virtual void DisplayRightClickMenu(Rect rect)
        {
            var menu = GetGenericMenu(Clip);
            menu.ShowAsContext();
        }
    }
}