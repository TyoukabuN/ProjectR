using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace PJR.Timeline.Editor
{
    public static class Graphics
    {
        public static class _GUIContent
        {
            private static readonly GUIContent s_Text = new GUIContent();
            public static GUIContent Temp(string t)
            {
                s_Text.text = t;
                s_Text.tooltip = string.Empty;
                return s_Text;
            }
        }
        public static void ShadowLabel(Rect rect, string text, GUIStyle style, Color textColor, Color shadowColor)
        {
            ShadowLabel(rect, _GUIContent.Temp(text), style, textColor, shadowColor);
        }

        public static void ShadowLabel(Rect rect, GUIContent content, GUIStyle style, Color textColor,
            Color shadowColor)
        {
            var shadowRect = rect;
            shadowRect.xMin += 2.0f;
            shadowRect.yMin += 2.0f;
            style.normal.textColor = shadowColor;
            style.hover.textColor = shadowColor;
            GUI.Label(shadowRect, content, style);

            style.normal.textColor = textColor;
            style.hover.textColor = textColor;
            GUI.Label(rect, content, style);
        }
        
        public static void ShadowLabel(Rect rect, string text, GUIStyle style, Color textColor, Color shadowColor, int fontSize)
        {
            ShadowLabel(rect, _GUIContent.Temp(text), style, textColor, shadowColor,fontSize);
        }
        public static void ShadowLabel(Rect rect, GUIContent content, GUIStyle style, Color textColor,
            Color shadowColor, int fontSize)
        {
            var shadowRect = rect;
            shadowRect.xMin += 2.0f;
            shadowRect.yMin += 2.0f;
            style.normal.textColor = shadowColor;
            style.hover.textColor = shadowColor;
            style.fontSize = fontSize; 
            GUI.Label(shadowRect, content, style);

            style.normal.textColor = textColor;
            style.hover.textColor = textColor;
            style.fontSize = fontSize; 
            GUI.Label(rect, content, style);
        }

        public static void DrawLine(Vector3 p1, Vector3 p2, Color color)
        {
            var c = Handles.color;
            Handles.color = color;
            Handles.DrawLine(p1, p2);
            Handles.color = c;
        }

        public static void DrawPolygonAA(Color color, Vector3[] vertices)
        {
            var prevColor = Handles.color;
            Handles.color = color;
            Handles.DrawAAConvexPolygon(vertices);
            Handles.color = prevColor;
        }

        private static MethodInfo _applyWireMaterialMethod;
        private static bool _initialized;
        
        private static void Initialize()
        {
            if (_initialized) return;

            // 获取HandleUtility类型
            var handleUtilityType = typeof(UnityEditor.HandleUtility);
        
            // 获取ApplyWireMaterial方法
            _applyWireMaterialMethod = handleUtilityType.GetMethod(
                "ApplyWireMaterial",
                BindingFlags.NonPublic | BindingFlags.Static,
                null,
                new[] { typeof(UnityEngine.Rendering.CompareFunction) },
                null
            );

            _initialized = true;
        }
        
        public static bool ApplyWireMaterial(UnityEngine.Rendering.CompareFunction zTest)
        {
            Initialize();

            if (_applyWireMaterialMethod == null)
                return false;
            _applyWireMaterialMethod.Invoke(null, new object[] { zTest });
            return true;
        }
        
        public static void DrawDottedLine(Vector3 p1, Vector3 p2, float segmentsLength, Color col)
        {
            if(!ApplyWireMaterial( CompareFunction.Always))
                return;

            GL.Begin(GL.LINES);
            GL.Color(col);

            var length = Vector3.Distance(p1, p2); // ignore z component
            var count = Mathf.CeilToInt(length / segmentsLength);
            for (var i = 0; i < count; i += 2)
            {
                GL.Vertex((Vector3.Lerp(p1, p2, i * segmentsLength / length)));
                GL.Vertex((Vector3.Lerp(p1, p2, (i + 1) * segmentsLength / length)));
            }

            GL.End();
        }

        // public static void DrawLineAtTime(TimelineWindow.WindowState state, double time, Color color, bool dotted = false)
        // {
        //     var t = state.TimeToPixel(time);
        //
        //     var p0 = new Vector3(t, state.timeAreaRect.yMax);
        //     var p1 = new Vector3(t, state.timeAreaRect.yMax + state.windowHeight - Const.sliderWidth);
        //
        //     if (dotted)
        //         DrawDottedLine(p0, p1, 4.0f, color);
        //     else
        //         DrawLine(p0, p1, color);
        // }

        public static void DrawTextureRepeated(Rect area, Texture texture)
        {
            if (texture == null || Event.current.type != EventType.Repaint)
                return;

            GUI.BeginClip(area);
            int w = Mathf.CeilToInt(area.width / texture.width);
            int h = Mathf.CeilToInt(area.height / texture.height);
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    GUI.DrawTexture(new Rect(x * texture.width, y * texture.height, texture.width, texture.height),
                        texture);
                }
            }

            GUI.EndClip();
        }

        public static void DrawShadow(Rect clientRect)
        {
            var rect = clientRect;
            rect.height = Const.shadowUnderTimelineHeight;
            GUI.Box(rect, GUIContent.none, Styles.Instance.bottomShadow);
        }

        public static void DrawBackgroundRect(TimelineWindow.WindowState state, Rect rect, bool subSequenceMode = false)
        {
            Color c = subSequenceMode
                ? Styles.Instance.customSkin.colorSubSequenceBackground
                : Styles.Instance.customSkin.colorSequenceBackground;
            EditorGUI.DrawRect(rect, c);
            if (state.IsEditingAPrefabAsset())
            {
                c = new Color(0.132f, 0.231f, 0.33f, 1f);
                c.a = 0.5f;
                EditorGUI.DrawRect(rect, c);
            }
        }

        public static Rect CalculateTextBoxSize(Rect trackRect, GUIStyle font, GUIContent content, float padding)
        {
            Rect textRect = trackRect;
            textRect.width = font.CalcSize(content).x + padding;
            textRect.x += (trackRect.width - textRect.width) / 2f;
            textRect.height -= 4f;
            textRect.y += 2f;
            return textRect;
        }
    }
}