using System;
using UnityEditor;
using UnityEngine;

namespace PJR.Timeline.Editor
{
    public class DefaultMenuDrawer : MenuDrawer
    {
        public DefaultMenuDrawer(Track track,IClip clip) : base(track,clip){}
    }
    public partial class MenuDrawer : TimelineGUIElement
    {
        public Track Track;
        public IClip IClip;
        public MenuDrawer(Track track, IClip clip) => Reset(track, clip);
        public override object PropertyObject => Track;

        public void Reset(Track track,IClip clip)
        {
            Track = track;
            IClip = clip;
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

            rect.Debug();

            var labelSize = Styles.CalcLabelSize(IClip.GetClipName());

            GUILayout.Label(new GUIContent(IClip.GetClipName(), IClip.GetClipName()), GUILayout.Width(labelSize.x),
                GUILayout.ExpandHeight(true));

            GUILayout.FlexibleSpace();

            using (new MidAlignmentScope.Horizontal())
            {
                if (GUILayout.Button(IClip.Mute ? Styles.trackMuteEnabledIcon : Styles.trackMuteDisabledIcon,
                        EditorStyles.iconButton))
                {
                    IClip.Mute = !IClip.Mute;
                }
            }

            GUILayoutUtility.GetLastRect().Debug(Color.green);

            var evtRect = rect;
            evtRect.xMin -= Constants.trackMenuLeftSpace;
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
        /// <summary>
        /// 右键默认选项
        /// </summary>
        /// <returns></returns>
        protected GenericMenu GetDefaultGenericMenu()
        {
            var menu = new GenericMenu();
            menu.AddDisabledItem(new GUIContent("右键选项"));
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("删除"),false,()=>{ });
            menu.AddSeparator("");
            IClip?.GetContextMenu(menu);
            return menu;
        }
        protected virtual void OnCreateContextMenu(GenericMenu menu){}
        public void DisplayRightClickMenu(Rect rect)
        {
            var menu = GetDefaultGenericMenu();
            menu.ShowAsContext();
        }
    }
}