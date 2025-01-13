using NPOI.HPSF;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static PJR.Timeline.Editor.TimelineWindow;
using Constants = PJR.Timeline.Editor.TimelineWindow.Constants;

namespace PJR.Timeline.Editor
{
    public abstract class ClipGUI
    {
        public Clip Clip { get; set; }
        public virtual float CalculateHeight() => TimelineWindow.Constants.trackHeight;
        public virtual void OnDrawMenu(Rect rect) 
        {
            Color backgroundColor = TrackGUI.hotClip == Clip
                ? Styles.Instance.customSkin.colorSelection
                : Styles.Instance.customSkin.colorTrackHeaderBackground;
            EditorGUI.DrawRect(rect, backgroundColor);
            rect.Debug();

            var evtRect = rect;
            evtRect.xMin -= Constants.trackMenuLeftSpace;
            GUIUtil.EventCheck(evtRect, EventType.MouseDown, OnClick);
        }
        public virtual void OnDrawTrack(Rect rect) 
        {
            Color backgroundColor = TrackGUI.hotClip == Clip
                ? Styles.Instance.customSkin.colorSelection
                : Styles.Instance.customSkin.colorTrackBackground;

            EditorGUI.DrawRect(rect, backgroundColor);
            rect.Debug();
            GUIUtil.EventCheck(rect, EventType.MouseDown, OnClick);
        }
        public virtual void OnClick(Event evt)
        {
            if (TrackGUI.hotClip == Clip)
                return;
            TrackGUI.hotClip = Clip;
            instance.Repaint();
        }
    }
    public abstract class ClipGUI<TClip> : ClipGUI where TClip : Clip 
    {
        public ClipGUI(TClip clip)
        {
            Clip = clip;
        }
    }
}