//-----------------------------------------------------------------------
// <copyright file="LazyEditorIcon.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.Utilities.Editor
{
#pragma warning disable

    using System;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Lazy loading Editor Icon.
    /// </summary>
    public class LazyEditorIcon : EditorIcon, IDisposable
    {
        private static readonly string iconShader = @"
Shader ""Hidden/Sirenix/Editor/GUIIcon""
{
	Properties
	{
        _MainTex(""Texture"", 2D) = ""white"" {}
        _Color(""Color"", Color) = (1,1,1,1)
	}
    SubShader
	{
        Blend SrcAlpha Zero
        Pass
        {
            CGPROGRAM
                " + "#" + @"pragma vertex vert
                " + "#" + @"pragma fragment frag
                " + "#" + @"include ""UnityCG.cginc""

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
                float4 _Color;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
				{
                    // drop shadow:
                    // float texelSize = 1.0 / 34.0;
                    // float2 shadowUv = clamp(i.uv + float2(-texelSize, texelSize * 2), float2(0, 0), float2(1, 1));
                    // fixed4 shadow = fixed4(0, 0, 0, tex2D(_MainTex, shadowUv).a); 

					fixed4 col = _Color;
					col.a *= tex2D(_MainTex, i.uv).a;

                    // drop shadow:
                    // col = lerp(shadow, col, col.a);

					return col;
				}
			ENDCG
		}
	}
}
";

        private static Color inactiveColorPro = new Color(0.40f, 0.40f, 0.40f, 1);
        private static Color activeColorPro = new Color(0.55f, 0.55f, 0.55f, 1);
        private static Color highlightedColorPro = new Color(0.90f, 0.90f, 0.90f, 1);

        private static Color inactiveColor = new Color(0.72f, 0.72f, 0.72f, 1);
        private static Color activeColor = new Color(0.40f, 0.40f, 0.40f, 1);
        private static Color highlightedColor = new Color(0.20f, 0.20f, 0.20f, 1);

        private static Material iconMat;

        private Texture2D icon;
        private Texture inactive;
        private Texture active;
        private Texture highlighted;
        private string data;
        private int width;
        private int height;

        /// <summary>
        /// Loads an EditorIcon from the spritesheet.
        /// </summary>
        public LazyEditorIcon(int width, int height, string base64ImageDataPngOrJPG)
        {
            this.width = width;
            this.height = height;
            this.data = base64ImageDataPngOrJPG;
        }

        /// <summary>
        /// Gets the icon's highlight texture.
        /// </summary>
        public override Texture Highlighted
        {
            get
            {
                if (this.highlighted == null)
                {
                    this.highlighted = this.RenderIcon(EditorGUIUtility.isProSkin ? highlightedColorPro : highlightedColor);
                }

                return this.highlighted;
            }
        }

        /// <summary>
        /// Gets the icon's active texture.
        /// </summary>
        public override Texture Active
        {
            get
            {
                if (this.active == null)
                {
                    this.active = this.RenderIcon(EditorGUIUtility.isProSkin ? activeColorPro : activeColor);
                }
                return this.active;
            }
        }

        /// <summary>
        /// Gets the icon's inactive texture.
        /// </summary>
        public override Texture Inactive
        {
            get
            {
                if (this.inactive == null)
                {
                    this.inactive = this.RenderIcon(EditorGUIUtility.isProSkin ? inactiveColorPro : inactiveColor);
                }

                return this.inactive;
            }
        }

        /// <summary>
        /// Not yet documented.
        /// </summary>
        public override Texture2D Raw
        {
            get
            {
                if (this.icon == null)
                {
                    var bytes = Convert.FromBase64String(this.data);
                    this.icon = TextureUtilities.LoadImage(this.width, this.height, bytes);
                }

                return this.icon;
            }
        }

        public void Dispose()
        {
            if (this.icon != null) UnityEngine.Object.DestroyImmediate(this.icon);
            if (this.inactive != null) UnityEngine.Object.DestroyImmediate(this.inactive);
            if (this.active != null) UnityEngine.Object.DestroyImmediate(this.active);
            if (this.highlighted != null) UnityEngine.Object.DestroyImmediate(this.highlighted);
        }

        private Texture RenderIcon(Color color)
        {
            if (iconMat == null || iconMat.shader == null)
            {
                const string key = "odin_inspector_lazy_icons";
                int matInstanceId = SessionState.GetInt(key, 0); // survive assembly reloads.

                if (matInstanceId != 0)
                {
                    iconMat = EditorUtility.InstanceIDToObject(matInstanceId) as Material;
                }

                if (iconMat == null)
                {
                    var shader = ShaderUtil.CreateShaderAsset(iconShader);
                    iconMat = new Material(shader);

                    UnityEngine.Object.DontDestroyOnLoad(shader);
                    UnityEngine.Object.DontDestroyOnLoad(iconMat);
                    iconMat.hideFlags = HideFlags.DontUnloadUnusedAsset;
                    SessionState.SetInt(key, iconMat.GetInstanceID());
                }
            }

            iconMat.SetColor("_Color", color);

            var prevSRGB = GL.sRGBWrite;
            GL.sRGBWrite = true;
            RenderTexture prev = RenderTexture.active;
            var rt = RenderTexture.GetTemporary(this.width, this.height, 0);
            RenderTexture.active = rt;
            GL.Clear(false, true, new Color(1, 1, 1, 0));
            Graphics.Blit(this.Raw, rt, iconMat);

            Texture2D texture = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false, true);
            texture.filterMode = FilterMode.Bilinear;
            texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            texture.alphaIsTransparency = true;
            texture.Apply();

            RenderTexture.ReleaseTemporary(rt);
            RenderTexture.active = prev;
            GL.sRGBWrite = prevSRGB;
            return texture;
        }
    }
}
#endif