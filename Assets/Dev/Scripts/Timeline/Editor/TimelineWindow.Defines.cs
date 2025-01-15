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
                return Constants.clipStartPositionY;
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
            get { return new Rect(0.0f, Constants.clipStartPositionY, state.trackMenuAreaWidth, position.height - Constants.timelineAreaYPosition); }
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

        public static class Constants
        {
            public const float timelineAreaYPosition = 19.0f;
            public const float timelineRulerHeight = 21.0f;
            public const float timelineRulerPreSpace = 5.0f;
            public const float clipStartPositionY = timelineAreaYPosition + timelineRulerHeight + 3;

            public const float trackHeight = 30f;
            public const float trackMenuLeftSpace = 18f;
            public const float trackMenuAreaTop = 6f;
            public const float trackMenuPadding = 3f;

            //clip左右调整宽度的箭头
            public const float clipMaxHandleWidth = 10f;
            public const float clipMinHandleWidth = 1f;

            //track左边
            public const float trackMenuDefaultAreaWidth = 315f;
            public const float trackMenuMinAreaWidth = 195.0f;
            public const float trackMenuMaxAreaWidth = 650.0f;


            /////
            public const float timeAreaHeight = 22.0f;
            public const float timeAreaMinWidth = 50.0f;
            public const float timeAreaShownRangePadding = 5.0f;

            public const float markerRowHeight = 18.0f;
            public const float markerRowYPosition = timelineAreaYPosition + timeAreaHeight;

            public const float defaultHeaderWidth = 315.0f;
            public const float defaultBindingAreaWidth = 40.0f;

            public const float minHeaderWidth = 195.0f;
            public const float maxHeaderWidth = 650.0f;
            public const float headerSplitterWidth = 6.0f;
            public const float headerSplitterVisualWidth = 2.0f;

            public const float maxTimeAreaScaling = 90000.0f;
            public const float timeCodeWidth = 100.0f; // Enough space to display up to 9999 without clipping

            public const float sliderWidth = 15;
            public const float shadowUnderTimelineHeight = 15.0f;
            public const float createButtonWidth = 70.0f;

            public const float selectorWidth = 23.0f;
            public const float cogButtonWidth = 25.0f;

            public const float trackHeaderBindingHeight = 18.0f;
            public const float trackHeaderButtonSize = 16.0f;
            public const float trackHeaderButtonPadding = 2.0f;
            public const float trackBindingMaxSize = 300.0f;
            public const float trackBindingPadding = 5.0f;

            public const float trackInsertionMarkerHeight = 1f;
            public const float trackResizeHandleHeight = 7f;
            public const float inlineCurveContentPadding = 2.0f;

            public const float playControlsWidth = 300;

            public const int autoPanPaddingInPixels = 50;

            public const float overlayTextPadding = 40.0f;

            /// <summary>
            /// 默认每帧占多少像素
            /// </summary>
            public const int pixelPerFrame = 10;
            /// <summary>
            /// 最大默认每帧占多少像素的倍率
            /// </summary>
            public const int maxPixelPerFrameRate = 10;
            /// <summary>
            /// 最大默认每帧占多少像素
            /// </summary>
            public const int maxPixelPerFrame = pixelPerFrame * maxPixelPerFrameRate;
        }
    }
}
