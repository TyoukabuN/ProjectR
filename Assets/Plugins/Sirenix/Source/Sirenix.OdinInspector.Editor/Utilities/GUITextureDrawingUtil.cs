#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
using System;
using UnityEditor;
using UnityEngine;

namespace Sirenix.OdinInspector.Editor.Internal
{
#pragma warning disable

    public static class GUITextureDrawingUtil
    {
        private static Material material;
        private static float _greyScale;
        private static Color _guiColor;
        private static Vector4 _uv;
        private static Color _hueColor;
        private const string shader = @"Shader ""Hidden/Sirenix/OdinGUIShader""
{
	SubShader
	{
		Lighting Off
		Cull Off
		ZWrite Off
		ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			" + @"#pragma vertex vert
			" + @"#pragma fragment frag
			" + @"#include ""UnityCG.cginc""

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float _SirenixOdin_GreyScale;
			float4 _SirenixOdin_GUIColor;
			float4 _SirenixOdin_GUIUv;
			float4 _SirenixOdin_HueColor;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			float test1(float x, float y)
			{
				if (x >= y) {
					return 0;
				}
				else {
					return 1;
				}
			}

			float test2(float x, float y)
			{
				return step(x, y);
			}

			float3 rgb2hsv(float3 c) {
				float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
				float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

				float d = q.x - min(q.w, q.y);
				float e = 1.0e-10;
				return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
			}

			float3 hsv2rgb(float3 c) {
				c = float3(c.x, clamp(c.yz, 0.0, 1.0));
				float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
				float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
				return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
			}

			float4 frag(v2f i) : SV_Target
			{
				float2 uv = i.uv;
				uv.y = 1 - uv.y;
				uv.x = _SirenixOdin_GUIUv.x + uv.x * _SirenixOdin_GUIUv.z;
				uv.y = _SirenixOdin_GUIUv.y + uv.y * _SirenixOdin_GUIUv.w;
				uv.y = 1 - uv.y;

				// Greyscale
				float4 col = tex2D(_MainTex, uv);
				float3 greyScale = (0.3 * col.r) + (0.59 * col.g) + (0.11 * col.b);
				col.rgb = lerp(col.rgb, greyScale, _SirenixOdin_GreyScale);

				// Change hue
				float3 h = col.rgb;
				h = rgb2hsv(h);
				float hue = rgb2hsv(_SirenixOdin_HueColor.rgb).x;
				h.x = hue;
				h = hsv2rgb(h);
				col.rgb = lerp(col.rgb, h, _SirenixOdin_HueColor.a);

				// Blend color
				col *= _SirenixOdin_GUIColor;

				return col;
			}
			ENDCG
		}
	}
}
";

        public static Material Material
        {
            get
            {
                if (material == null)
                {
                    const string key = "odin_gui_material1";
                    int matInstanceId = SessionState.GetInt(key, 0); // survive assembly reloads.

                    if (matInstanceId != 0)
                    {
                        material = EditorUtility.InstanceIDToObject(matInstanceId) as Material;
                    }

                    if (material == null)
                    {
                        var s = ShaderUtil.CreateShaderAsset(shader);
                        material = new Material(s);

                        UnityEngine.Object.DontDestroyOnLoad(s);
                        UnityEngine.Object.DontDestroyOnLoad(material);
                        material.hideFlags = HideFlags.DontUnloadUnusedAsset;

                        SessionState.SetInt(key, material.GetInstanceID());
                    }

                    // Note that we are using global shader properties because changing
                    // shader properties of a material instance, causes the scene view to repaint.
                    Shader.SetGlobalColor("_SirenixOdin_GUIColor", new Color(1, 1, 1, 1));
                    Shader.SetGlobalColor("_SirenixOdin_HueColor", new Color(1, 1, 1, 0));
                    Shader.SetGlobalVector("_SirenixOdin_GUIUv", new Vector4(0, 0, 1, 1));
                    Shader.SetGlobalFloat("_SirenixOdin_GreyScale", 0);
                    Shader.SetGlobalFloat("_SirenixOdin_GreyScale", 0);
                    _uv = new Vector4(0, 0, 1, 1);
                    _hueColor = new Color(1, 1, 1, 0);
                    _greyScale = 0f;
                    _guiColor = new Color(1, 1, 1, 1);
                }

                return material;
            }
        }

        internal static Rect CalculateScaledTextureRects(Rect position, ScaleMode scaleMode, float imageAspect, out Rect uvRect)
        {
            float num = position.width / position.height;
            switch (scaleMode)
            {
                case ScaleMode.StretchToFill:
                    uvRect = new Rect(0f, 0f, 1f, 1f);
                    return position;
                case ScaleMode.ScaleAndCrop:
                    if (num > imageAspect)
                    {
                        float num4 = imageAspect / num;
                        uvRect = new Rect(0f, (1f - num4) * 0.5f, 1f, num4);
                    }
                    else
                    {
                        float num5 = num / imageAspect;
                        uvRect = new Rect(0.5f - num5 * 0.5f, 0f, num5, 1f);
                    }
                    return position;
                case ScaleMode.ScaleToFit:
                    if (num > imageAspect)
                    {
                        float num2 = imageAspect / num;
                        uvRect = new Rect(0f, 0f, 1f, 1f);
                        return new Rect(position.xMin + position.width * (1f - num2) * 0.5f, position.yMin, num2 * position.width, position.height);
                    }
                    else
                    {
                        float num3 = num / imageAspect;
                        uvRect = new Rect(0f, 0f, 1f, 1f);
                        return new Rect(position.xMin, position.yMin + position.height * (1f - num3) * 0.5f, position.width, num3 * position.height);
                    }
            }

            throw new NotImplementedException();
        }

        public static void DrawTexture(Rect rect, Texture2D texture, ScaleMode scaleMode, Color color, Color hueColor, float greyScale = 1)
        {
            if (Event.current.type != EventType.Repaint)
                return;

            if (rect.width <= 0 || rect.height <= 0)
                return;

            rect = CalculateScaledTextureRects(rect, scaleMode, (float)texture.width / texture.height, out var uvRect);

            var unclipped = GUIClipInfo.Unclip(rect);
            var clipRect = GUIClipInfo.TopMostRect;

            if (!unclipped.Overlaps(clipRect))
                return;

            var uvXOrig = uvRect.x;
            var uvYOrig = uvRect.y;
            var uvWidthOrig = uvRect.width;
            var uvHeightOrig = uvRect.height;

            var uvX = uvXOrig;
            var uvY = uvYOrig;
            var uvWidth = uvWidthOrig;
            var uvHeight = uvHeightOrig;
            var clipped = rect;

            var clipTop = clipRect.y - unclipped.y;
            if (clipTop > 0)
            {
                var amount = clipTop / rect.height;
                uvY += uvHeightOrig * amount;
                uvHeight -= uvHeightOrig * amount;
                clipped.y += clipTop;
                clipped.height -= clipTop;
            }

            var clipBottom = unclipped.yMax - clipRect.yMax;
            if (clipBottom > 0)
            {
                var amount = clipBottom / rect.height;
                uvHeight -= uvHeight * amount;
                clipped.height -= clipBottom;
            }

            var clipLeft = clipRect.x - unclipped.x;
            if (clipLeft > 0)
            {
                var amount = clipLeft / rect.width;
                uvX += uvWidthOrig * amount;
                uvWidth -= uvWidthOrig * amount;
                clipped.x += clipLeft;
                clipped.width -= clipLeft;
            }

            var clipRight = unclipped.xMax - clipRect.xMax;
            if (clipRight > 0)
            {
                var amount = clipRight / rect.width;
                uvWidth -= uvWidth * amount;
                clipped.width -= clipRight;
            }

            color *= GUI.color;

            if (!GUI.enabled)
                color.a *= 0.4f;

            SetHueColor(hueColor);
            SetGUIColor(color);
            SetGreyScale(greyScale);
            SetUv(new Vector4(uvX, uvY, uvWidth, uvHeight));
            Graphics.DrawTexture(clipped, texture, Material);
        }

        public static void SetProperties(Color guiColor, Color hueColor, Vector4 uv, float greyScale)
        {
            SetGUIColor(guiColor);
            SetHueColor(hueColor);
            SetUv(uv);
            SetGreyScale(greyScale);
        }

        private static void SetGUIColor(Color color)
        {
            if (_guiColor != color)
            {
                Shader.SetGlobalColor("_SirenixOdin_GUIColor", color);
                _guiColor = color;
            }
        }

        private static void SetHueColor(Color color)
        {
            if (_hueColor != color)
            {
                Shader.SetGlobalColor("_SirenixOdin_HueColor", color);
                _hueColor = color;
            }
        }

        private static void SetGreyScale(float factor)
        {
            if (_greyScale != factor)
            {
                Shader.SetGlobalFloat("_SirenixOdin_GreyScale", factor);
                _greyScale = factor;
            }
        }

        private static void SetUv(Vector4 uv)
        {
            if (_uv != uv)
            {
                Shader.SetGlobalVector("_SirenixOdin_GUIUv", uv);
                _uv = uv;
            }
        }
    }
}
#endif