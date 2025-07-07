using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PJR.Timeline.Editor
{
    public partial class TimelineWindow
    {
        public float headerHeight
        {
            get {
                return Const.clipStartPositionY_Offseted;
            }
        }
        public Rect controlBarRect
        {
            get {
                return new Rect(0.0f, Const.timelineAreaYPosition, position.width, Const.timelineRulerHeight);
            }
        }
        public Rect headerRect
        {
            //get { return new Rect(0.0f, Constants.clipStartPositionY, Constants.defaultHeaderWidth, position.height - Constants.timelineAreaYPosition); }
            get { return new Rect(0.0f, Const.clipStartPositionY_Offseted, State.trackMenuAreaWidth, position.height - Const.timelineAreaYPosition); }
        }
        public Rect timelineRulerRect
        {
            //get { return new Rect(headerRect.width, Constants.timelineRulerHeight, position.width - headerRect.width, Constants.timelineRulerHeight); }
            get { return new Rect(State.trackMenuAreaWidth, Const.timelineRulerHeight, position.width - State.trackMenuAreaWidth, Const.timelineRulerHeight); }
        }

        public Rect trackRect
        {
            get
            {
                var yMinHeight = headerHeight - 1;
                return new Rect(0, yMinHeight, position.width, position.height - yMinHeight - 0);
            }
        }
        public Rect headerSizeHandleRect
        {
            get
            {
                return Rect.MinMaxRect(State.trackMenuAreaWidth - 2, trackRect.yMin, State.trackMenuAreaWidth + 2, trackRect.yMax);
            }
        }

       
    }
}
