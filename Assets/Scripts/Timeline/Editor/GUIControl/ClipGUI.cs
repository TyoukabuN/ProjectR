using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using static PJR.Timeline.Editor.TimelineWindow;

namespace PJR.Timeline.Editor
{
    public abstract class ClipGUI : TimelineGUIElement
    {
        public IClip Clip { get; set; }
        public virtual float CalculateHeight() => Constants.trackHeight + draggedMenuSpace;

        float m_DraggedMenuSpace = 0f;
        public float draggedMenuSpace => m_DraggedMenuSpace;

        protected WindowState windowState => instance.state;

        protected MenuDrawer _menuDrawer;
        protected MenuDrawer MenuDrawer=> _menuDrawer ??= new(Clip);
        
        protected ClipDrawer _clipDrawer;
        protected ClipDrawer ClipDrawer=> _clipDrawer ??= new(Clip);
        
        public static Color GetTrackBgColor(bool selected)
        {
            return selected
                ? Styles.Instance.customSkin.colorSelection
                : Styles.Instance.customSkin.colorTrackBackground;
        }

        //以选中Menu为准
        public override bool IsSelect => MenuDrawer?.IsSelect ?? false;
        //Track选中时，也选中Menu
        public override void Select()
        {
            base.Select();
            MenuDrawer?.Select();
        }

        /// <summary>
        /// 画Track左边TrackMenu
        /// </summary>
        /// <param name="rect"></param>
        public virtual void OnDrawMenu(Rect rect) 
        {
            MenuDrawer.Draw(rect);
        }

        /// <summary>
        /// 画Track右边的TrackView里的Clip的入口
        /// </summary>
        /// <param name="rect"></param>
        public virtual void OnDrawTrack(Rect rect) 
        {
            EditorGUI.DrawRect(rect, GetTrackBgColor(IsSelect));

            OnDrawClip(rect);
            EventUtil.MouseEvent.LeftOrRightClick(rect,OnClick,OnContextClick);
        }
        
        /// <summary>
        /// 画Track右边的TrackView里的Clip
        /// </summary>
        /// <param name="rect"></param>
        protected virtual void OnDrawClip(Rect rect)
        {
            ClipDrawer.DrawClip(rect);
        }
        public virtual void OnClick(Rect rect)
        {
            if (IsSelect)
                return;
            Select();
            EventType.MouseDown.Use();
            Repaint();
        }
        public virtual void OnContextClick(Rect rect)
        {
            DisplayRightClickMenu(rect);
            EventType.MouseDown.Use();
        }
        public virtual void OnClipClick(Rect rect)
        {
            if (IsSelect)
                return;
            Select();
            Debug.Log("Clip Clicked");
            EventType.MouseDown.Use();
        }

        /// <summary>
        /// 右键默认选项
        /// </summary>
        /// <returns></returns>
        protected GenericMenu GetDefaultGenericMenu()
        {
            var menu = new GenericMenu();
            menu.AddDisabledItem(new GUIContent("右键默认选项"));
            menu.AddSeparator($"{Clip.GetDisplayName()}");
            return menu;
        }
        public virtual void DisplayRightClickMenu(Rect rect)
        {
            var menu = GetDefaultGenericMenu();
            menu.ShowAsContext();
        }

        public virtual void Repaint()
        { 
            TimelineWindow.instance?.Repaint();
        }
    }
    public abstract class ClipGUI<TClip> : ClipGUI where TClip : IClip
    {
        public ClipGUI(IClip clip)
        {
            Clip = clip;
        }
    }
    public class DefaultClipGUI : ClipGUI
    {
        public DefaultClipGUI(IClip clip) { 
            Clip = clip;
        }
        public override void OnDrawMenu(Rect rect)
        {
            base.OnDrawMenu(rect);
        }
    }
}