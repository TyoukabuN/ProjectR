using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PJR.Timeline.Editor
{
    public class Styles
    {
        const string k_ImagePath = "Assets/Dev/Scripts/Timeline/Editor/StyleSheets/Images/Icons/{0}.png";
        const string resourcesPath = "Assets/Plugins/com.unity.timeline@1.7.5/Editor/StyleSheets/res/";

        const string k_DarkSkinPath = resourcesPath + "Timeline_DarkSkin.txt";
        const string k_DarkSkinAssetPath = resourcesPath + "Timeline_DarkSkin.asset";
        const string k_LightSkinPath = resourcesPath + "Timeline_LightSkin.txt";
        const string k_LightSkinAssetPath = resourcesPath + "Timeline_LightSkin.asset";
        //Timeline resources
        public const string newTimelineDefaultNameSuffix = "Timeline";

        public static readonly GUIContent referenceTrackLabel = TrTextContent("R", "This track references an external asset");
        public static readonly GUIContent recordingLabel = TrTextContent("Recording...");
        public static readonly GUIContent noTimelineAssetSelected = TrTextContent("To start creating a timeline, select a GameObject");
        public static readonly GUIContent createTimelineOnSelection = TrTextContent("To begin a new timeline with {0}, create {1}");
        public static readonly GUIContent noTimelinesInScene = TrTextContent("No timeline found in the scene");
        public static readonly GUIContent createNewTimelineText = TrTextContent("Create a new Timeline and Director Component for Game Object");
        public static readonly GUIContent previewContent = TrTextContent("Preview", "Enable/disable scene preview mode");
        public static readonly GUIContent previewDisabledContent = L10n.TextContentWithIcon("Preview", "Scene preview is disabled for this TimelineAsset", MessageType.Info);
        public static readonly GUIContent mixOff = TrIconContent("TimelineEditModeMixOFF", "Mix Mode (1)");
        public static readonly GUIContent mixOn = TrIconContent("TimelineEditModeMixON", "Mix Mode (1)");
        public static readonly GUIContent rippleOff = TrIconContent("TimelineEditModeRippleOFF", "Ripple Mode (2)");
        public static readonly GUIContent rippleOn = TrIconContent("TimelineEditModeRippleON", "Ripple Mode (2)");
        public static readonly GUIContent replaceOff = TrIconContent("TimelineEditModeReplaceOFF", "Replace Mode (3)");
        public static readonly GUIContent replaceOn = TrIconContent("TimelineEditModeReplaceON", "Replace Mode (3)");
        public static readonly GUIContent showMarkersOn = TrIconContent("TimelineCollapseMarkerButtonEnabled", "Show / Hide Timeline Markers");
        public static readonly GUIContent showMarkersOff = TrIconContent("TimelineCollapseMarkerButtonDisabled", "Show / Hide Timeline Markers");
        public static readonly GUIContent showMarkersOnTimeline = TrTextContent("Show markers");
        public static readonly GUIContent timelineMarkerTrackHeader = TrTextContentWithIcon("Markers", string.Empty, "TimelineHeaderMarkerIcon");
        public static readonly GUIContent signalTrackIcon = IconContent("TimelineSignal");

        public static readonly GUIContent debugContent = TrTextContent("Debug", "Enable/Debug mode");
        public static readonly GUIContent repaint = TrTextContent("Repaint", "Repaint GUI");
        public static readonly GUIContent reloadSkin = TrTextContent("ReloadSkin", "Reload Skin Object");
        public static readonly GUIContent inspectSkin = TrTextContent("InspectSkin", "Inspect CurrentSkin Object");
        public static readonly GUIContent lightSkin = TrTextContent("LightSkin", "Inspect LightSkin Object");
        public static readonly GUIContent darkSkin = TrTextContent("DrakSkin", "Inspect DrakSkin Object");

        //Unity Default Resources
        public static readonly GUIContent playContent = L10n.IconContent("Animation.Play", "Play the timeline (Space)");
        public static readonly GUIContent gotoBeginingContent = L10n.IconContent("Animation.FirstKey", "Go to the beginning of the timeline (Shift+<)");
        public static readonly GUIContent gotoEndContent = L10n.IconContent("Animation.LastKey", "Go to the end of the timeline (Shift+>)");
        public static readonly GUIContent nextFrameContent = L10n.IconContent("Animation.NextKey", "Go to the next frame");
        public static readonly GUIContent previousFrameContent = L10n.IconContent("Animation.PrevKey", "Go to the previous frame");
        public static readonly GUIContent newContent = L10n.IconContent("CreateAddNew", "Add new tracks.");
        public static readonly GUIContent optionsCogIcon = L10n.IconContent("_Popup", "Options");
        public static readonly GUIContent animationTrackIcon = EditorGUIUtility.IconContent("AnimationClip Icon");
        public static readonly GUIContent audioTrackIcon = EditorGUIUtility.IconContent("AudioSource Icon");
        public static readonly GUIContent playableTrackIcon = EditorGUIUtility.IconContent("cs Script Icon");
        public static readonly GUIContent timelineSelectorArrow = L10n.IconContent("icon dropdown", "Timeline Selector");

        public static readonly Color kMixToolColor = Color.white;
        public static readonly Color kRippleToolColor = new Color(255f / 255f, 210f / 255f, 51f / 255f);
        public static readonly Color kReplaceToolColor = new Color(165f / 255f, 30f / 255f, 30f / 255f);

        public const string markerDefaultStyle = "MarkerItem";

        public GUIStyle groupBackground;
        public GUIStyle displayBackground;
        public GUIStyle fontClip;
        public GUIStyle fontClipLoop;
        public GUIStyle trackHeaderFont;
        public GUIStyle trackGroupAddButton;
        public GUIStyle groupFont;
        public GUIStyle timeCursor;
        public GUIStyle endmarker;
        public GUIStyle tinyFont;
        public GUIStyle foldout;
        public GUIStyle trackMuteButton;
        public GUIStyle trackLockButton;
        public GUIStyle trackRecordButton;
        public GUIStyle playTimeRangeStart;
        public GUIStyle playTimeRangeEnd;
        public GUIStyle selectedStyle;
        public GUIStyle trackSwatchStyle;
        public GUIStyle connector;
        public GUIStyle keyframe;
        public GUIStyle warning;
        public GUIStyle extrapolationHold;
        public GUIStyle extrapolationLoop;
        public GUIStyle extrapolationPingPong;
        public GUIStyle extrapolationContinue;
        public GUIStyle trackMarkerButton;
        public GUIStyle markerMultiOverlay;
        public GUIStyle bottomShadow;
        public GUIStyle trackOptions;
        public GUIStyle infiniteTrack;
        public GUIStyle clipOut;
        public GUIStyle clipIn;
        public GUIStyle trackCurvesButton;
        public GUIStyle trackLockOverlay;
        public GUIStyle activation;
        public GUIStyle playrange;
        public GUIStyle timelineLockButton;
        public GUIStyle trackAvatarMaskButton;
        public GUIStyle markerWarning;
        public GUIStyle editModeBtn;
        public GUIStyle showMarkersBtn;
        public GUIStyle sequenceSwitcher;
        public GUIStyle inlineCurveHandle;
        public GUIStyle timeReferenceButton;
        public GUIStyle trackButtonSuite;
        public GUIStyle previewButtonDisabled;

        public Styles2 timeAreaStyles;

        static Styles s_Instance;
        DirectorNamedColor m_DarkSkinColors;
        DirectorNamedColor m_LightSkinColors;
        DirectorNamedColor m_DefaultSkinColors;
        public static Styles Instance
        {
            get
            {
                if (s_Instance == null || s_Instance.ShouldLoadStyles())
                {
                    s_Instance = new Styles();
                    s_Instance.Initialize();
                }

                return s_Instance;
            }
        }

        public static void ReloadStylesIfNeeded()
        {
            if (Instance.ShouldLoadStyles())
            {
                Instance.LoadStyles();
                if (!Instance.ShouldLoadStyles())
                    Instance.Initialize();
            }
        }

        public DirectorNamedColor customSkin
        {
            get { return EditorGUIUtility.isProSkin ? m_DarkSkinColors : m_LightSkinColors; }
            internal set
            {
                if (EditorGUIUtility.isProSkin)
                    m_DarkSkinColors = value;
                else
                    m_LightSkinColors = value;
            }
        }

        DirectorNamedColor LoadColorSkin(string path)
        {
            var asset = EditorGUIUtility.LoadRequired(path) as TextAsset;

            if (asset != null && !string.IsNullOrEmpty(asset.text))
            {
                return DirectorNamedColor.CreateAndLoadFromText(asset.text);
            }

            return m_DefaultSkinColors;
        }
        DirectorNamedColor LoadColorSkinAsset(string path)
        {
            var asset = AssetDatabase.LoadAssetAtPath<DirectorNamedColor>(path);
            return asset;
        }

        static DirectorNamedColor CreateDefaultSkin()
        {
            var nc = ScriptableObject.CreateInstance<DirectorNamedColor>();
            nc.SetDefault();
            return nc;
        }

        public void ExportSkinToFile()
        {
            if (customSkin == m_DarkSkinColors)
                customSkin.ToText(k_DarkSkinPath);

            if (customSkin == m_LightSkinColors)
                customSkin.ToText(k_LightSkinPath);
        }

        public void ReloadSkin()
        {
            m_DarkSkinColors = LoadColorSkinAsset(k_DarkSkinAssetPath);
            if (m_DarkSkinColors == null)
            {
                m_DarkSkinColors = LoadColorSkin(k_DarkSkinPath);
                AssetDatabase.CreateAsset(m_DarkSkinColors, k_DarkSkinAssetPath);
            }

            m_LightSkinColors = LoadColorSkinAsset(k_LightSkinAssetPath);
            if (m_LightSkinColors == null)
            {
                m_LightSkinColors = LoadColorSkin(k_DarkSkinPath);
                AssetDatabase.CreateAsset(m_LightSkinColors, k_LightSkinAssetPath);
            }
        }

        public void Initialize()
        {
            m_DefaultSkinColors = CreateDefaultSkin();

            ReloadSkin();
        }

        Styles()
        {
            LoadStyles();
        }

        bool ShouldLoadStyles()
        {
            //return endmarker == null ||
            //    endmarker.name == GUISkin.error.name;
            return endmarker == null;
        }

        void LoadStyles()
        {
            endmarker = GetGUIStyle("Icon-Endmarker");
            groupBackground = GetGUIStyle("groupBackground");
            displayBackground = GetGUIStyle("sequenceClip");
            fontClip = GetGUIStyle("Font-Clip");
            trackHeaderFont = GetGUIStyle("sequenceTrackHeaderFont");
            trackGroupAddButton = GetGUIStyle("sequenceTrackGroupAddButton");
            groupFont = GetGUIStyle("sequenceGroupFont");
            timeCursor = GetGUIStyle("Icon-TimeCursor");
            tinyFont = GetGUIStyle("tinyFont");
            foldout = GetGUIStyle("Icon-Foldout");
            trackMuteButton = GetGUIStyle("trackMuteButton");
            trackLockButton = GetGUIStyle("trackLockButton");
            trackRecordButton = GetGUIStyle("trackRecordButton");
            playTimeRangeStart = GetGUIStyle("Icon-PlayAreaStart");
            playTimeRangeEnd = GetGUIStyle("Icon-PlayAreaEnd");
            selectedStyle = GetGUIStyle("Color-Selected");
            trackSwatchStyle = GetGUIStyle("Icon-TrackHeaderSwatch");
            connector = GetGUIStyle("Icon-Connector");
            keyframe = GetGUIStyle("Icon-Keyframe");
            warning = GetGUIStyle("Icon-Warning");
            extrapolationHold = GetGUIStyle("Icon-ExtrapolationHold");
            extrapolationLoop = GetGUIStyle("Icon-ExtrapolationLoop");
            extrapolationPingPong = GetGUIStyle("Icon-ExtrapolationPingPong");
            extrapolationContinue = GetGUIStyle("Icon-ExtrapolationContinue");
            bottomShadow = GetGUIStyle("Icon-Shadow");
            trackOptions = GetGUIStyle("Icon-TrackOptions");
            infiniteTrack = GetGUIStyle("Icon-InfiniteTrack");
            clipOut = GetGUIStyle("Icon-ClipOut");
            clipIn = GetGUIStyle("Icon-ClipIn");
            trackCurvesButton = GetGUIStyle("trackCurvesButton");
            trackLockOverlay = GetGUIStyle("trackLockOverlay");
            activation = GetGUIStyle("Icon-Activation");
            playrange = GetGUIStyle("Icon-Playrange");
            timelineLockButton = GetGUIStyle("IN LockButton");
            trackAvatarMaskButton = GetGUIStyle("trackAvatarMaskButton");
            trackMarkerButton = GetGUIStyle("trackCollapseMarkerButton");
            markerMultiOverlay = GetGUIStyle("MarkerMultiOverlay");
            editModeBtn = GetGUIStyle("editModeBtn");
            showMarkersBtn = GetGUIStyle("showMarkerBtn");
            markerWarning = GetGUIStyle("markerWarningOverlay");
            sequenceSwitcher = GetGUIStyle("sequenceSwitcher");
            inlineCurveHandle = GetGUIStyle("RL DragHandle");
            timeReferenceButton = GetGUIStyle("timeReferenceButton");
            trackButtonSuite = GetGUIStyle("trackButtonSuite");
            previewButtonDisabled = GetGUIStyle("previewButtonDisabled");

            timeAreaStyles = new Styles2();

            //playrangeContent = new GUIContent(GetBackgroundImage(playrange)) { tooltip = L10n.Tr("Toggle play range markers.") };

            fontClipLoop = new GUIStyle(fontClip) { fontStyle = FontStyle.Bold };
        }


        public static GUIStyle GetGUIStyle(string s)
        {
            return EditorStyles.FromUSS(s);
        }

        #region Func
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
        public static GUIContent IconContent(string iconName)
        {
            return EditorGUIUtility.IconContent(iconName == null ? null : ResolveIcon(iconName));
        }

        public static GUIContent TrTextContentWithIcon(string text, string tooltip, string iconName)
        {
            return L10n.TextContentWithIcon(text, tooltip, iconName == null ? null : ResolveIcon(iconName));
        }

        public static GUIContent TrTextContent(string text, string tooltip = null)
        {
            return L10n.TextContent(text, tooltip);
        }

        #endregion
        public class Styles2
        {
            public GUIStyle timelineTick = "AnimationTimelineTick";

            public GUIStyle playhead = "AnimationPlayHead";
        }
    }
}
