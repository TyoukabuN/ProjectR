using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PJR.Timeline.Editor
{
    public partial class TimelineWindow
    {
        public float headerHeight => Const.clipStartPositionY_Offseted;

        public Rect controlBarRect =>
            new(0.0f, Const.timelineAreaYPosition, position.width, Const.timelineRulerHeight);
        public Rect headerRect
            => new(0.0f, Const.clipStartPositionY_Offseted, State.currentTrackMenuAreaWidth, position.height - Const.timelineAreaYPosition); 

        //时间尺区域
        public Rect timelineRulerRect
            => new(State.currentTrackMenuAreaWidth + Const.timelineRulerXOffset + Const.headerSizeHandleWidth, Const.timelineRulerHeight, position.width - State.currentTrackMenuAreaWidth, Const.timelineRulerHeight);

        public Rect trackRect
        {
            get
            {
                var yMinHeight = headerHeight - 1;
                return new Rect(0, yMinHeight, position.width, position.height - yMinHeight - 0);
            }
        }
        
        //调整TrackView两边大小的Handle的区域
        public Rect headerSizeHandleRect
            =>Rect.MinMaxRect(State.currentTrackMenuAreaWidth, trackRect.yMin, State.currentTrackMenuAreaWidth + Const.headerSizeHandleWidth, trackRect.yMax);
            //=>Rect.MinMaxRect(State.trackMenuAreaWidth - Const.headerSizeHandleWidth/2 - Const.headerSizeHandleWidth/2, trackRect.yMin, State.trackMenuAreaWidth + Const.headerSizeHandleWidth/2, trackRect.yMax);
        
        
        public Rect trackClipAreaRect
        {
            get
            {
                var temp = trackRect;
                temp.xMin = timelineRulerRect.xMin; 
                return temp;
            }
        }
    }
}
