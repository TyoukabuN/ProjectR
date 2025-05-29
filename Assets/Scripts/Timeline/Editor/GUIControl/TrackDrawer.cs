using System;
using UnityEditor;
using UnityEngine;
using static PJR.Timeline.Editor.TimelineWindow;

namespace PJR.Timeline.Editor
{
    public abstract class TrackDrawer : TimelineGUIElement
    {
        public abstract IClip IClip { get; }
        public virtual float CalculateHeight() => Constants.trackHeight + DraggedMenuSpace;

        float _draggedMenuSpace = 0f;
        public float DraggedMenuSpace => _draggedMenuSpace;
        protected WindowState windowState => instance.state;
        
        
        protected MenuDrawer _menuDrawer;
        public virtual Type MenuDrawerType => typeof(DefaultMenuDrawer);
        public virtual MenuDrawer MenuDrawer  => _menuDrawer ??= Activator.CreateInstance(MenuDrawerType,IClip) as MenuDrawer;
        
        
        protected ClipDrawer _clipDrawer;
        public virtual Type ClipDrawerType => typeof(DefaultClipDrawer);
        public virtual ClipDrawer ClipDrawer => _clipDrawer ??= Activator.CreateInstance(ClipDrawerType,IClip) as ClipDrawer; 

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
            menu.AddSeparator($"{IClip.GetClipName()}");
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
    
    public abstract class TrackDrawer<TClip> : TrackDrawer where TClip : IClip
    {
        protected TClip _clip;
        public override IClip IClip => _clip;
        public TClip Clip => _clip;
        public override MenuDrawer MenuDrawer=> _menuDrawer ??= new DefaultMenuDrawer(IClip);
        public override ClipDrawer ClipDrawer=> _clipDrawer ??= new DefaultClipDrawer(IClip);

        public TrackDrawer(TClip clip)
        {
            _clip = clip;
        }
    }
    public class DefaultTrackDrawer : TrackDrawer<IClip>
    {
        public DefaultTrackDrawer(IClip clip):base(clip) { 
        }
        public override void OnDrawMenu(Rect rect)
        {
            base.OnDrawMenu(rect);
        }
    }
}