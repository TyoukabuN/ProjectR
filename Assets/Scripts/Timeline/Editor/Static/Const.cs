using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJR.Timeline.Editor
{
    public static class Const
    {
        /// <summary>
        /// 默认每帧占多少像素
        /// </summary>
        public const int DefaultPixelPerFrame = 60;
        public const float DefaultPixelPerFrameScaleFactor = 0.333f;
        /// <summary>
        /// Ruler的主刻度线间隔模式
        /// 时间尺以一定的规律改变主刻度线间隔(缩放)
        /// </summary>
        public static readonly int[] RulerScaleMarkingPattern = { 5, 2, 3 ,2};
        //一帧可以占的最大像素
        public const int MaxPixelPerFrame = (int)(50f * 3.6); // 180px
        //一帧可以占的最大像素
        public const int SubMaxPixelPerFrame = (int)(50f * 3.0); // 150px
        //最小的可显示刻度线像素
        public const int MinPixelPerScaleMark = 3;
        //最小的可显示<主>刻度线像素
        public const int MinMainScaleMarkPixel = 50;
        //最小的可显示背景刻度线像素
        public const int MinPixelPerBgScaleMark = 30;
        //缩放速度
        public const float ScalingSpeed = 0.0833f;
        

        public const float timelineAreaYPosition = 19.0f;
        public const float timelineRulerHeight = 21.0f;
        public const float timelineRulerXOffset = 6;
        public const float timelineRulerPreSpace = 5.0f;
        public const float clipStartOffsetY = 3;
        public const float clipStartPositionY_Offseted = timelineAreaYPosition + timelineRulerHeight + clipStartOffsetY;
        public const float clipStartPositionY = timelineAreaYPosition + timelineRulerHeight;

        public const float trackHeight = 30f;
        public const float trackMenuLeftSpace = 18f;
        public const float trackMenuAreaTop = 6f;
        public const float trackMenuPadding = 3f;

        public const float headerSizeHandleWidth = 4;

        //调整TrackMenu大小(高度)的handle的Height
        public const float trackMenuDragHandleHeight = 4f;

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
    }
}