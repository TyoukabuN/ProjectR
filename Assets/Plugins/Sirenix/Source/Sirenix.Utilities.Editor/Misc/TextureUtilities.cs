//-----------------------------------------------------------------------
// <copyright file="TextureUtilities.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.Utilities.Editor
{
#pragma warning disable

    using System;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Collection of texture functions.
    /// </summary>
    public static class TextureUtilities
    {
        private static Material extractSpriteMaterial;

        private static readonly string extractSpriteShader = @"
            Shader ""Hidden/Sirenix/Editor/GUIIcon""
            {
	            Properties
	            {
                    _MainTex(""Texture"", 2D) = ""white"" {}
                    _Color(""Color"", Color) = (1,1,1,1)
                    _Rect(""Rect"", Vector) = (0,0,0,0)
                    _TexelSize(""TexelSize"", Vector) = (0,0,0,0)
	            }
                SubShader
	            {
                    Blend SrcAlpha OneMinusSrcAlpha
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
                            float4 _Rect;

                            v2f vert(appdata v)
                            {
                                v2f o;
                                o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                                o.uv = v.uv;
                                return o;
                            }

                            fixed4 frag(v2f i) : SV_Target
				            {
                                float2 uv = i.uv;
                                uv *= _Rect.zw;
					            uv += _Rect.xy;
					            return tex2D(_MainTex, uv);
				            }
			            ENDCG
		            }
	            }
            }";


        /// <summary>
        /// Creates a new texture with no mimapping, linier colors, and calls texture.LoadImage(bytes), DontDestroyOnLoad(tex) and sets hideFlags = DontUnloadUnusedAsset | DontSaveInEditor.
        /// 
        /// Old description no longer relevant as we've moved past version 2017.
        /// Loads an image from bytes with the specified width and height. Use this instead of someTexture.LoadImage() if you're compiling to an assembly. Unity has moved the method in 2017, 
        /// and Unity's assembly updater is not able to fix it for you. This searches for a proper LoadImage method in multiple locations, and also handles type name conflicts.
        /// </summary>
        public static Texture2D LoadImage(int width, int height, byte[] bytes)
        {
            var tex = new Texture2D(width, height, TextureFormat.ARGB32, false, true);
            tex.LoadImage(bytes);
            UnityEngine.Object.DontDestroyOnLoad(tex);
            tex.hideFlags = HideFlags.DontUnloadUnusedAsset | HideFlags.DontSaveInEditor;
            return tex;
        }


        /// <summary>
        /// Crops a Texture2D into a new Texture2D.
        /// </summary>
        public static Texture2D CropTexture(this Texture texture, Rect source)
        {
            RenderTexture prev = RenderTexture.active;
            var rt = RenderTexture.GetTemporary(texture.width, texture.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 8);
            RenderTexture.active = rt;
            GL.Clear(false, true, new Color(1, 1, 1, 0));
            Graphics.Blit(texture, rt);
            Texture2D clone = new Texture2D((int)source.width, (int)source.height, TextureFormat.ARGB32, true, false);
            clone.filterMode = FilterMode.Point;
            clone.ReadPixels(source, 0, 0);
            clone.Apply();
            RenderTexture.active = prev;
            RenderTexture.ReleaseTemporary(rt);
            return clone;
        }
        
        /// <summary>
        /// Resizes a texture by blitting, this allows you to resize unreadable textures.
        /// </summary>
        public static Texture2D ResizeByBlit(this Texture texture, int width, int height, FilterMode filterMode = FilterMode.Bilinear)
        {
            RenderTexture prev = RenderTexture.active;
            var           rt   = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 1);
            rt.filterMode        = FilterMode.Bilinear;
            RenderTexture.active = rt;
            GL.Clear(false, true, new Color(1, 1, 1, 0));
            var prevSRGB = GL.sRGBWrite;
            GL.sRGBWrite = false;
            Graphics.Blit(texture, rt);
            Texture2D clone = new Texture2D((int)width, (int)height, TextureFormat.ARGB32, true, false);
            clone.filterMode = filterMode;
            clone.ReadPixels(new Rect(0,0, width, height), 0, 0);
            clone.Apply();
            RenderTexture.active = prev;
            RenderTexture.ReleaseTemporary(rt);
            GL.sRGBWrite = prevSRGB;
            return clone;
        }


        /// <summary>
        /// Converts a Sprite to a Texture2D.
        /// </summary>
        /// <param name="sprite"></param>
        /// <returns></returns>
        public static Texture2D ConvertSpriteToTexture(Sprite sprite)
        {
            var rect = sprite.rect;

            if (extractSpriteMaterial == null || extractSpriteMaterial.shader == null)
            {
                extractSpriteMaterial = new Material(ShaderUtil.CreateShaderAsset(extractSpriteShader));
            }

            extractSpriteMaterial.SetVector("_TexelSize", new Vector2(1f / sprite.texture.width, 1f / sprite.texture.height));
            extractSpriteMaterial.SetVector("_Rect", new Vector4(
                rect.x / sprite.texture.width,
                rect.y / sprite.texture.height,
                rect.width / sprite.texture.width,
                rect.height / sprite.texture.height
            ));

            var prevSRGB = GL.sRGBWrite;
            GL.sRGBWrite = false;
            RenderTexture prev = RenderTexture.active;
            var rt = RenderTexture.GetTemporary((int)rect.width, (int)rect.height, 0);
            RenderTexture.active = rt;
            GL.Clear(false, true, new Color(1, 1, 1, 0));
            Graphics.Blit(sprite.texture, rt, extractSpriteMaterial);
            Texture2D texture = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, true, false);
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