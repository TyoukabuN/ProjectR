using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

namespace PJR.Timeline.Editor
{
    public partial class TimelineWindow
    {
        public class WindowState
        {
            public EditingSequare editingSequence;
            public bool DisableControlBar() => !AnyEditingSequence();
            public bool AnyEditingSequence() => editingSequence.Sequence != null;
            public bool NonEditingSequence() => !AnyEditingSequence();

            /// <summary>
            /// 时间轴的放大缩小
            /// </summary>
            public int currentPixelPerFrame = Constants.pixelPerFrame;
            /// <summary>
            /// debugging=true会绘制一些额外的GUI
            /// </summary>
            public bool debugging = true;

            private TrackGUI trackGUI;
            public TrackGUI TrackGUI => trackGUI ??= new TrackGUI();

            #region 一些动态的Rect
            public float trackMenuAreaWidth = Constants.trackMenuDefaultAreaWidth;
            public Rect headerSizeHandleRect;// = instance.headerSizeHandleRect;
            #endregion

            #region 一些hotspot
            public ClipGUI hotTrack = null;
            public IClip hotClip = null;
            public void ClearHotspots()
            {
                hotTrack = null;
                hotClip = null;
            }
            #endregion

            #region 一些单位转换用的方法
            public int PixelToFrame(float pixel)
            {
                return (int)(pixel / currentPixelPerFrame);
            }
            public double PixelToSecond(float pixel)
            {
                return (int)(pixel / currentPixelPerFrame) / CurrentFrameRate;
            }
            public float FrameToPixel(int frames)
            {
                return frames * currentPixelPerFrame;
            }

            public double CurrentFrameRate
            {
                get
                {
                    return Define.FPS_Default;
                }
            }
            public double CurrentSecondPerFrame => 1 / CurrentFrameRate;
            #endregion

        }
        public struct EditingSequare
        {
            public static EditingSequare Empty = new EditingSequare();

            public Sequence Sequence;
            public SequenceAsset Asset;
        }
    }
}
