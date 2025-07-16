using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Styles = PJR.Timeline.Editor.Styles;
using static PJR.Timeline.Editor.TimelineWindow;

namespace PJR.Timeline.Editor
{
    public class TrackGUI
    {
        public List<Track> tracks => windowState.SequenceHandle.SequenceAsset?.Tracks;
        public WindowState windowState => TimelineWindow.instance.State;
        public Rect position => TimelineWindow.instance.position;
        public TrackGUI()
        {
        }

        public virtual float CalculateHeight()
        {
            return Const.trackHeight;
        }

        GUIStyle backgroundStyle = new GUIStyle(GUI.skin.box)
        {
            padding = new RectOffset(0, 0, 0, 0), // 内边距
            margin = new RectOffset(0, 0, 0, 0),     // 外边距
            alignment = TextAnchor.MiddleLeft        // 文本对齐方式
        };
        public virtual void OnGUI(Rect totalArea)
        {
            if (tracks == null)
                return;

            using (var changeCheck = new EditorGUI.ChangeCheckScope())
            {
                using (new GUILayout.HorizontalScope(GUILayout.Width(instance.trackRect.width)))
                {
                    var trackMenuAreaWidth = windowState.trackMenuAreaWidth;
                    var usedPixelX = trackMenuAreaWidth;
                    //TrackMenu（左边）
                    using (new GUILayout.VerticalScope(GUILayout.Width(trackMenuAreaWidth)))
                    {
                        GUILayout.Space(Const.trackMenuAreaTop);
                        for (int i = 0; i < tracks.Count; i++)
                        {
                            GUILayout.Space(Const.trackMenuPadding);
                            var clip = tracks[i]?.clips[0];
                            if (clip == null)
                                continue;

                            var clipGUI = GetClipGUI(clip);
                            if (clipGUI == null)
                                continue;

                            using (new GUILayout.HorizontalScope(GUILayout.Height(clipGUI.CalculateHeight())))
                            {
                                GUILayout.Space(Const.trackMenuLeftSpace);
                                var menuRect = GUILayoutUtility.GetRect(0, clipGUI.CalculateHeight(),
                                    GUILayout.ExpandWidth(false));
                                menuRect.width = trackMenuAreaWidth - Const.trackMenuLeftSpace;
                                clipGUI.OnDrawMenu(menuRect);
                            }
                        }
                    }

                    //Menu和Track之间的空间
                    //用于修改HeaderWidth
                    GUILayout.Space(Const.headerSizeHandleWidth + Const.timelineRulerXOffset);
                    usedPixelX += Const.headerSizeHandleWidth + Const.timelineRulerXOffset;
                    
                    //TrackClip（右边）
                    using (new GUILayout.VerticalScope(GUILayout.Width(position.width - usedPixelX)))
                    {
                        //GUILayoutUtility.GetRect(50, 50).Debug();
                        GUILayout.Space(Const.trackMenuAreaTop);

                        for (int i = 0; i < tracks.Count; i++)
                        {
                            GUILayout.Space(Const.trackMenuPadding);
                            var clip = tracks[i]?.clips[0];
                            if (clip == null)
                                continue;

                            var clipGUI = GetClipGUI(clip);
                            if (clipGUI == null)
                                continue;

                            using (new GUILayout.HorizontalScope(GUILayout.Height(clipGUI.CalculateHeight())))
                            {
                                clipGUI.OnDrawTrack(GUILayoutUtility.GetRect(0, clipGUI.CalculateHeight()));
                            }
                        }
                    }
                }

                if (changeCheck.changed)
                    OnTrakcChanged();
            }
        }

        public void OnTrakcChanged()
        {
            windowState?.TrySetSequenceAssetDirty();
        }

        private Dictionary<IClip, TrackDrawer> clip2clipGUI;
        public TrackDrawer GetClipGUI(Clip clip)
        {
            TrackDrawer trackDrawerObj = null;
            if (clip == null)
                return trackDrawerObj;

            if (clip2clipGUI == null)
                clip2clipGUI = new Dictionary<IClip, TrackDrawer>();

            if (!clip2clipGUI.TryGetValue(clip, out trackDrawerObj))
            {
                if(TryGetBindingClipGUI(clip, out var bindingClipGUI))
                    trackDrawerObj = bindingClipGUI;
                else
                    trackDrawerObj = new DefaultTrackDrawer(clip);

                clip2clipGUI[clip] = trackDrawerObj;
            }
            return trackDrawerObj;
        }

        public bool TryGetBindingClipGUI(IClip clip, out TrackDrawer bindingTrackDrawer)
        {
            bindingTrackDrawer = null;
            var bindingClipGUIAttribute = Attribute.GetCustomAttribute(clip.GetType(), typeof(BindingTrackDrawerAttribute)) as BindingTrackDrawerAttribute;
            if (bindingClipGUIAttribute == null)
                return false;
            if (!bindingClipGUIAttribute.Type.InheritsFrom(typeof(TrackDrawer<>)))
                return false;
            bindingTrackDrawer = Activator.CreateInstance(bindingClipGUIAttribute.Type, clip) as TrackDrawer;
            return true;
        }
    }
}
