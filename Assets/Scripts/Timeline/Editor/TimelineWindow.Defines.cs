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
                return Constants.clipStartPositionY_Offseted;
            }
        }
        public Rect controlBarRect
        {
            get {
                return new Rect(0.0f, Constants.timelineAreaYPosition, position.width, Constants.timelineRulerHeight);
            }
        }
        public Rect headerRect
        {
            //get { return new Rect(0.0f, Constants.clipStartPositionY, Constants.defaultHeaderWidth, position.height - Constants.timelineAreaYPosition); }
            get { return new Rect(0.0f, Constants.clipStartPositionY_Offseted, state.trackMenuAreaWidth, position.height - Constants.timelineAreaYPosition); }
        }
        public Rect timelineRulerRect
        {
            //get { return new Rect(headerRect.width, Constants.timelineRulerHeight, position.width - headerRect.width, Constants.timelineRulerHeight); }
            get { return new Rect(state.trackMenuAreaWidth, Constants.timelineRulerHeight, position.width - state.trackMenuAreaWidth, Constants.timelineRulerHeight); }
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
                return Rect.MinMaxRect(state.trackMenuAreaWidth - 2, trackRect.yMin, state.trackMenuAreaWidth + 2, trackRect.yMax);
            }
        }

       
    }
}
