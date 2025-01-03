using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PJR.Timeline
{
    public class Styles
    {
        const string k_ImagePath = "Assets/Dev/Scripts/Timeline/Editor/StyleSheets/Images/Icons/{0}.png";

        //Timeline resources


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
        #endregion
    }
}
