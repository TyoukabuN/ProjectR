using System;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using static PJR.Timeline.Editor.TimelineWindow;

namespace PJR.Timeline.Editor
{
    public abstract partial class TimelineGUIElement : IDisposable
    {
        protected WindowState windowState => instance.State;
        public virtual bool IsSelect => instance.State.Hotspot == this;

        public Action SelectMethed;
        public Action DeSelectMethed;
        public TimelineGUIElement()
        {
            Register(this);
            SelectMethed = Default_SelectMethod;
            DeSelectMethed = Default_DeSelectMethod;
        }
        public virtual object PropertyObject => null;
        private PropertyTree _propertyTree;
        public virtual PropertyTree PropertyTree
        {
            get
            {
                if (PropertyObject == null)
                    return null;
                _propertyTree ??= PropertyTree.Create(PropertyObject);
                return _propertyTree;
            }
        }
        public virtual void Select()=> SelectMethed?.Invoke();
        public virtual void DeSelect() => DeSelectMethed?.Invoke();
        public virtual void OnDeselect() {}
        protected void Default_SelectMethod() => TimelineWindow.instance.State.Hotspot = this;
        protected void Default_DeSelectMethod() => TimelineWindow.instance.State.UnSetHotspot(this);
        public virtual void Repaint() => TimelineWindow.instance?.Repaint();
        public virtual void Dispose()
        {
            _propertyTree?.Dispose();
            _propertyTree = null;
        }
        public virtual void OnDrawOverlapGUI()
        {
        }
    }

    public abstract partial class TimelineGUIElement
    {
        private static List<TimelineGUIElement> _allGUIElements;
        public static List<TimelineGUIElement> AllGUIElements => _allGUIElements ??= new List<TimelineGUIElement>(8);

        public static void Register(TimelineGUIElement guiElement)
        {
            if (guiElement == null)
                return;
            if (AllGUIElements.Contains(guiElement))
                return;
            AllGUIElements.Add(guiElement);
        }

        public static void UnRegister(TimelineGUIElement guiElement)
        {
            if (guiElement == null)
                return;
            if (!AllGUIElements.Contains(guiElement))
                return;
            guiElement.Dispose();
            AllGUIElements.Remove(guiElement);
        }

        public static void DisposeAll()
        {
            if(_allGUIElements == null)
                return;
            for (var i = _allGUIElements.Count - 1; i >= 0; i--)
            {
                if(_allGUIElements[i] == null)
                    continue;
                _allGUIElements[i].Dispose();
                if (_allGUIElements[i].IsSelect)
                    Selection.activeObject = null;
            }
            _allGUIElements.Clear();
        }
    }
}