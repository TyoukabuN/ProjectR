using System;
using UnityEditor;
using UnityEngine;

namespace PJR.Timeline.Editor
{
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
        public virtual void Draw(Rect rect) => DrawTrackMenu(GetDrawContext(rect));
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
            menu.AddSeparator($"{clip.GetDisplayName()}");
            return menu;
        }

        public virtual void DisplayRightClickMenu(Rect rect)
        {
            var menu = GetGenericMenu(Clip);
            menu.ShowAsContext();
        }
    }
}