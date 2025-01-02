using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PJR.Timeline
{
    public partial class TimelineWindow
    {
        const string k_ImagePath = "Assets/Dev/Scripts/Timeline/Editor/StyleSheets/Images/Icons/{0}.png";

        public Rect sequenceHeaderRect
        {
            get { return new Rect(0.0f, WindowConstants.markerRowYPosition, WindowConstants.defaultHeaderWidth, position.height - WindowConstants.timeAreaYPosition); }
        }
        public Rect sequenceTimelineRulerRect
        {
            get { return new Rect(sequenceHeaderRect.width, 0, position.width - sequenceHeaderRect.width, WindowConstants.timeAreaYPosition); }
        }
        
        public Rect sequenceTimeRuleRect
        {
            get { return new Rect(0.0f, WindowConstants.markerRowYPosition, WindowConstants.defaultHeaderWidth, position.height - WindowConstants.timeAreaYPosition); }
        }

        public static GUIContent TrIconContent(string iconName, string tooltip = null)
        {
            var content = new GUIContent();
            content.tooltip = tooltip;

            var icon = AssetDatabase.LoadAssetAtPath<Texture2D>(ResolveIcon(iconName));
            if (icon != null)
            {
                icon.filterMode = FilterMode.Bilinear;
                content.image = LightenTexture(icon);
            }
            return content;
        }
        static string ResolveIcon(string icon)
        {
            return string.Format(k_ImagePath, icon);
        }
        static Texture2D LightenTexture(Texture2D texture)
        {
            if (!texture)
            {
                return texture;
            }

            Texture2D texture2D = new Texture2D(texture.width, texture.height);
            Color[] pixels = texture2D.GetPixels();
            Color[] pixels2 = texture.GetPixels();
            for (int i = 0; i < pixels2.Length; i++)
            {
                pixels[i] = LightenColor(pixels2[i]);
            }

            texture2D.hideFlags = HideFlags.HideAndDontSave;
            texture2D.SetPixels(pixels);
            texture2D.Apply();
            return texture2D;
        }

        static Color LightenColor(Color color)
        {
            Color.RGBToHSV(color, out var H, out var _, out var _);
            Color result = Color.HSVToRGB((H + 0.5f) % 1f, 0f, 0.8f);
            result.a = color.a;
            return result;
        }

        static class WindowConstants
        {
            public const float timeAreaYPosition = 19.0f;
            public const float timeAreaHeight = 22.0f;
            public const float timeAreaMinWidth = 50.0f;
            public const float timeAreaShownRangePadding = 5.0f;

            public const float markerRowHeight = 18.0f;
            public const float markerRowYPosition = timeAreaYPosition + timeAreaHeight;

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
}
