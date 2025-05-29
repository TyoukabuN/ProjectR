using System;
using static PJR.Timeline.Editor.TimelineWindow;

namespace PJR.Timeline.Editor
{
    public abstract class TimelineGUIElement
    {
        protected WindowState windowState => instance.state;
        public virtual bool IsSelect => instance.state.Hotspot == this;

        public Action SelectMethed;
        public Action DeSelectMethed;
        
        public TimelineGUIElement()
        {
            SelectMethed = Default_SelectMethod;
            DeSelectMethed = Default_DeSelectMethod;
        }
        public virtual void Select()=> SelectMethed?.Invoke();
        public virtual void DeSelect() => DeSelectMethed?.Invoke();
        public virtual void OnDeselect()
        {
        }
        protected void Default_SelectMethod() => TimelineWindow.instance.state.Hotspot = this;
        protected void Default_DeSelectMethod() => TimelineWindow.instance.state.UnSetHotspot(this);

        public virtual void Repaint() => TimelineWindow.instance?.Repaint();
    }
}