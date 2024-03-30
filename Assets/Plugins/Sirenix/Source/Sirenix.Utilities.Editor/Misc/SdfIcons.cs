//-----------------------------------------------------------------------
// <copyright file="SdfIcons.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEditor;
using System;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector;

namespace Sirenix.OdinInspector.Editor
{
#pragma warning disable

    public static class SdfIcons
    {
        private static Vector4 _uv;
        private static Color _color;
        private static Color _bgColor;
        private static Material sdfMaterial;
        private static Texture2D sdfTexture;
        private const int sdfPadding = 8;

        private const string shader = @"Shader ""Hidden/Sirenix/SdfIconShader""
{
    SubShader
	{
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
                sampler2D _SirenixOdin_SdfTex;
                float4 _SirenixOdin_Color;
                float4 _SirenixOdin_BgColor;
                float4 _SirenixOdin_Uv;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                float sampleDist(float2 uv, float dx, float edge, float padding)
                {
	                float dist = tex2D(_SirenixOdin_SdfTex, uv).a;
	                float p = -abs((dx * 3072.0) / -padding);
	                float a = min(1, max(0, edge - p * 0.33333));
	                float b = max(0, min(1, edge + p * 0.33333));
	                return smoothstep(b, a, dist);
                }

                float4 frag(v2f i) : SV_Target
				{
                    float2 uv = i.uv;
                    uv.y = 1 - uv.y;
                    uv.x = _SirenixOdin_Uv.x + uv.x * _SirenixOdin_Uv.z;
                    uv.y = _SirenixOdin_Uv.y + uv.y * _SirenixOdin_Uv.w;
                    uv.y = 1 - uv.y;

                    float alpha = 0.0;

                    if (_SirenixOdin_BgColor.a > 0.01)
                    {
                        float3 colorBg = _SirenixOdin_BgColor.rgb;
			            float3 colorFg = _SirenixOdin_Color.rgb;

			            float edge = 0.5019608;
			            float padding = 8;
			            float dx = ddx(uv.x);
			            float2 t = float2(dx * 0.333333, 0);
			            float3 subDist = float3(
				            sampleDist(uv.xy - t, dx, edge, padding),
				            sampleDist(uv.xy, dx, edge, padding),
				            sampleDist(uv.xy + t, dx, edge, padding));
			            float3 color = lerp(colorBg, colorFg, clamp(subDist, 0.0, 1.0));

			            float alpha = min(1, subDist.r + subDist.g + subDist.b);
			            float4 col = float4(color, alpha * _SirenixOdin_Color.a);

                        return col;

                    } else {
			            float edge = 0.5019608;
			            float padding = 8;
			            float dx = ddx(uv.x);
			            float alpha = sampleDist(uv, dx, edge, padding);
					    float4 col = _SirenixOdin_Color;
                        col.a *= alpha;

					    return col;
                    }
				}
			ENDCG
		}
	}
}
";

        public static SdfIcon[] AllIcons = new SdfIcon[]
        {
            new SdfIcon(0, (char)0x7FF6, 80, 46, new SdfUvRect(0.00032552084f, 0.00032552084f, 0.026367188f, 0.0152994795f)),
            new SdfIcon(1, (char)0x01F1, 82, 80, new SdfUvRect(0.0003255208f, 0.015950521f, 0.027018229f, 0.041992188f)),
            new SdfIcon(2, (char)0x02F1, 81, 80, new SdfUvRect(0.0003255208f, 0.042643227f, 0.026692707f, 0.06868489f)),
            new SdfIcon(3, (char)0x03F1, 72, 72, new SdfUvRect(0.0003255209f, 0.06933594f, 0.02376302f, 0.09277344f)),
            new SdfIcon(4, (char)0x04F1, 65, 72, new SdfUvRect(0.0003255208f, 0.093424484f, 0.021484375f, 0.11686198f)),
            new SdfIcon(5, (char)0x05F1, 72, 72, new SdfUvRect(0.0003255209f, 0.11751302f, 0.02376302f, 0.14095053f)),
            new SdfIcon(6, (char)0x06F1, 72, 65, new SdfUvRect(0.00032552076f, 0.14160156f, 0.02376302f, 0.16276042f)),
            new SdfIcon(7, (char)0x07F1, 72, 72, new SdfUvRect(0.0003255209f, 0.16341145f, 0.02376302f, 0.18684897f)),
            new SdfIcon(8, (char)0x08F1, 72, 72, new SdfUvRect(0.0003255207f, 0.1875f, 0.02376302f, 0.2109375f)),
            new SdfIcon(9, (char)0x09F1, 72, 66, new SdfUvRect(0.0003255209f, 0.21158853f, 0.02376302f, 0.23307292f)),
            new SdfIcon(10, (char)0x0AF1, 76, 76, new SdfUvRect(0.00032552084f, 0.23372395f, 0.025065104f, 0.25846353f)),
            new SdfIcon(11, (char)0x0BF1, 72, 72, new SdfUvRect(0.0003255207f, 0.2591146f, 0.02376302f, 0.28255206f)),
            new SdfIcon(12, (char)0x0CF1, 80, 72, new SdfUvRect(0.0003255211f, 0.28320312f, 0.026367188f, 0.30664062f)),
            new SdfIcon(13, (char)0x0DF1, 80, 72, new SdfUvRect(0.0003255207f, 0.3072917f, 0.026367188f, 0.33072916f)),
            new SdfIcon(14, (char)0x0EF1, 76, 72, new SdfUvRect(0.0003255207f, 0.33138022f, 0.025065104f, 0.3548177f)),
            new SdfIcon(15, (char)0x0FF1, 72, 76, new SdfUvRect(0.00032552084f, 0.35546875f, 0.02376302f, 0.3802083f)),
            new SdfIcon(16, (char)0x10F1, 72, 76, new SdfUvRect(0.00032552084f, 0.38085938f, 0.02376302f, 0.40559894f)),
            new SdfIcon(17, (char)0x11F1, 76, 72, new SdfUvRect(0.0003255211f, 0.40625f, 0.025065104f, 0.4296875f)),
            new SdfIcon(18, (char)0x12F1, 72, 62, new SdfUvRect(0.00032552052f, 0.43033856f, 0.02376302f, 0.4505208f)),
            new SdfIcon(19, (char)0x13F1, 62, 72, new SdfUvRect(0.0003255211f, 0.45117188f, 0.020507812f, 0.47460938f)),
            new SdfIcon(20, (char)0x14F1, 62, 72, new SdfUvRect(0.0003255207f, 0.47526044f, 0.020507812f, 0.4986979f)),
            new SdfIcon(21, (char)0x15F1, 72, 62, new SdfUvRect(0.00032552052f, 0.49934897f, 0.02376302f, 0.51953125f)),
            new SdfIcon(22, (char)0x16F1, 68, 72, new SdfUvRect(0.0003255211f, 0.52018225f, 0.022460938f, 0.5436198f)),
            new SdfIcon(23, (char)0x17F1, 68, 72, new SdfUvRect(0.0003255211f, 0.5442708f, 0.022460938f, 0.5677084f)),
            new SdfIcon(24, (char)0x18F1, 80, 80, new SdfUvRect(0.00032552058f, 0.5683594f, 0.026367188f, 0.59440106f)),
            new SdfIcon(25, (char)0x19F1, 80, 80, new SdfUvRect(0.00032552058f, 0.59505206f, 0.026367188f, 0.62109375f)),
            new SdfIcon(26, (char)0x1AF1, 80, 80, new SdfUvRect(0.0003255213f, 0.62174475f, 0.026367188f, 0.6477865f)),
            new SdfIcon(27, (char)0x1BF1, 80, 80, new SdfUvRect(0.00032552058f, 0.6484375f, 0.026367188f, 0.6744792f)),
            new SdfIcon(28, (char)0x1CF1, 80, 80, new SdfUvRect(0.00032552058f, 0.6751302f, 0.026367188f, 0.7011719f)),
            new SdfIcon(29, (char)0x1DF1, 80, 80, new SdfUvRect(0.0003255213f, 0.7018229f, 0.026367188f, 0.7278646f)),
            new SdfIcon(30, (char)0x1EF1, 65, 65, new SdfUvRect(0.00032552052f, 0.7285156f, 0.021484375f, 0.7496745f)),
            new SdfIcon(31, (char)0x1FF1, 80, 80, new SdfUvRect(0.00032552058f, 0.7503255f, 0.026367188f, 0.7763672f)),
            new SdfIcon(32, (char)0x20F1, 80, 80, new SdfUvRect(0.0003255213f, 0.7770182f, 0.026367188f, 0.80305994f)),
            new SdfIcon(33, (char)0x21F1, 80, 80, new SdfUvRect(0.00032552058f, 0.80371094f, 0.026367188f, 0.8297526f)),
            new SdfIcon(34, (char)0x22F1, 80, 80, new SdfUvRect(0.00032552058f, 0.8304036f, 0.026367188f, 0.8564453f)),
            new SdfIcon(35, (char)0x23F1, 66, 65, new SdfUvRect(0.00032552143f, 0.8570963f, 0.021809895f, 0.87825525f)),
            new SdfIcon(36, (char)0x24F1, 46, 48, new SdfUvRect(0.00032552003f, 0.87890625f, 0.01529948f, 0.89453125f)),
            new SdfIcon(37, (char)0x25F1, 80, 80, new SdfUvRect(0.0003255213f, 0.89518225f, 0.026367188f, 0.921224f)),
            new SdfIcon(38, (char)0x26F1, 80, 80, new SdfUvRect(0.00032552058f, 0.921875f, 0.026367188f, 0.9479167f)),
            new SdfIcon(39, (char)0x27F1, 80, 72, new SdfUvRect(0.0003255211f, 0.9485677f, 0.026367188f, 0.97200525f)),
            new SdfIcon(40, (char)0x28F1, 52, 72, new SdfUvRect(0.0003255203f, 0.97265625f, 0.017252605f, 0.99609375f)),
            new SdfIcon(41, (char)0x29F1, 80, 80, new SdfUvRect(0.027669271f, 0.015950521f, 0.053710938f, 0.041992188f)),
            new SdfIcon(42, (char)0x2AF1, 80, 80, new SdfUvRect(0.054361977f, 0.015950521f, 0.08040364f, 0.041992188f)),
            new SdfIcon(43, (char)0x2BF1, 72, 80, new SdfUvRect(0.08105469f, 0.015950521f, 0.10449219f, 0.041992188f)),
            new SdfIcon(44, (char)0x2CF1, 48, 46, new SdfUvRect(0.02701823f, 0.00032552084f, 0.04264323f, 0.0152994795f)),
            new SdfIcon(45, (char)0x2DF1, 80, 80, new SdfUvRect(0.105143234f, 0.015950521f, 0.1311849f, 0.041992188f)),
            new SdfIcon(46, (char)0x2EF1, 80, 80, new SdfUvRect(0.13183594f, 0.015950521f, 0.15787761f, 0.041992188f)),
            new SdfIcon(47, (char)0x2FF1, 72, 52, new SdfUvRect(0.15852864f, 0.015950521f, 0.18196616f, 0.032877605f)),
            new SdfIcon(48, (char)0x30F1, 78, 66, new SdfUvRect(0.18261719f, 0.015950521f, 0.20800781f, 0.0374349f)),
            new SdfIcon(49, (char)0x31F1, 72, 65, new SdfUvRect(0.20865884f, 0.015950521f, 0.23209636f, 0.037109375f)),
            new SdfIcon(50, (char)0x32F1, 72, 65, new SdfUvRect(0.23274739f, 0.015950521f, 0.25618488f, 0.037109375f)),
            new SdfIcon(51, (char)0x33F1, 80, 80, new SdfUvRect(0.25683594f, 0.015950521f, 0.2828776f, 0.041992188f)),
            new SdfIcon(52, (char)0x34F1, 80, 80, new SdfUvRect(0.28352866f, 0.015950521f, 0.3095703f, 0.041992188f)),
            new SdfIcon(53, (char)0x35F1, 48, 46, new SdfUvRect(0.04329427f, 0.00032552084f, 0.058919273f, 0.0152994795f)),
            new SdfIcon(54, (char)0x36F1, 80, 80, new SdfUvRect(0.31022137f, 0.015950521f, 0.336263f, 0.041992188f)),
            new SdfIcon(55, (char)0x37F1, 80, 80, new SdfUvRect(0.33691406f, 0.015950521f, 0.36295572f, 0.041992188f)),
            new SdfIcon(56, (char)0x38F1, 72, 52, new SdfUvRect(0.36360678f, 0.015950521f, 0.38704425f, 0.032877605f)),
            new SdfIcon(57, (char)0x39F1, 80, 80, new SdfUvRect(0.3876953f, 0.015950521f, 0.41373697f, 0.041992188f)),
            new SdfIcon(58, (char)0x3AF1, 80, 80, new SdfUvRect(0.41438803f, 0.015950521f, 0.4404297f, 0.041992188f)),
            new SdfIcon(59, (char)0x3BF1, 80, 80, new SdfUvRect(0.44108075f, 0.015950521f, 0.46712238f, 0.041992188f)),
            new SdfIcon(60, (char)0x3CF1, 80, 80, new SdfUvRect(0.46777344f, 0.015950521f, 0.4938151f, 0.041992188f)),
            new SdfIcon(61, (char)0x3DF1, 80, 80, new SdfUvRect(0.49446616f, 0.015950521f, 0.5205078f, 0.041992188f)),
            new SdfIcon(62, (char)0x3EF1, 80, 80, new SdfUvRect(0.5211588f, 0.015950521f, 0.54720056f, 0.041992188f)),
            new SdfIcon(63, (char)0x3FF1, 65, 66, new SdfUvRect(0.54785156f, 0.015950521f, 0.56901044f, 0.0374349f)),
            new SdfIcon(64, (char)0x40F1, 80, 80, new SdfUvRect(0.56966144f, 0.015950521f, 0.5957031f, 0.041992188f)),
            new SdfIcon(65, (char)0x41F1, 80, 80, new SdfUvRect(0.5963541f, 0.015950521f, 0.6223959f, 0.041992188f)),
            new SdfIcon(66, (char)0x42F1, 80, 80, new SdfUvRect(0.6230469f, 0.015950521f, 0.64908856f, 0.041992188f)),
            new SdfIcon(67, (char)0x43F1, 80, 80, new SdfUvRect(0.64973956f, 0.015950521f, 0.67578125f, 0.041992188f)),
            new SdfIcon(68, (char)0x44F1, 66, 66, new SdfUvRect(0.67643225f, 0.015950521f, 0.6979167f, 0.0374349f)),
            new SdfIcon(69, (char)0x45F1, 46, 48, new SdfUvRect(0.6985677f, 0.015950521f, 0.7135417f, 0.031575523f)),
            new SdfIcon(70, (char)0x46F1, 80, 80, new SdfUvRect(0.7141927f, 0.015950521f, 0.7402344f, 0.041992188f)),
            new SdfIcon(71, (char)0x47F1, 80, 80, new SdfUvRect(0.7408854f, 0.015950521f, 0.7669271f, 0.041992188f)),
            new SdfIcon(72, (char)0x48F1, 52, 72, new SdfUvRect(0.7675781f, 0.015950521f, 0.78450525f, 0.039388023f)),
            new SdfIcon(73, (char)0x49F1, 80, 80, new SdfUvRect(0.78515625f, 0.015950521f, 0.81119794f, 0.041992188f)),
            new SdfIcon(74, (char)0x4AF1, 80, 80, new SdfUvRect(0.81184894f, 0.015950521f, 0.8378906f, 0.041992188f)),
            new SdfIcon(75, (char)0x4BF1, 72, 80, new SdfUvRect(0.8385416f, 0.015950521f, 0.8619792f, 0.041992188f)),
            new SdfIcon(76, (char)0x4CF1, 72, 80, new SdfUvRect(0.8626302f, 0.015950521f, 0.88606775f, 0.041992188f)),
            new SdfIcon(77, (char)0x4DF1, 80, 80, new SdfUvRect(0.88671875f, 0.015950521f, 0.91276044f, 0.041992188f)),
            new SdfIcon(78, (char)0x4EF1, 80, 80, new SdfUvRect(0.91341144f, 0.015950521f, 0.9394531f, 0.041992188f)),
            new SdfIcon(79, (char)0x4FF1, 80, 65, new SdfUvRect(0.9401041f, 0.015950521f, 0.9661459f, 0.037109375f)),
            new SdfIcon(80, (char)0x50F1, 80, 65, new SdfUvRect(0.9667969f, 0.015950521f, 0.99283856f, 0.037109375f)),
            new SdfIcon(81, (char)0x51F1, 74, 80, new SdfUvRect(0.027669271f, 0.042643227f, 0.051757812f, 0.06868489f)),
            new SdfIcon(82, (char)0x52F1, 62, 63, new SdfUvRect(0.027669271f, 0.06933594f, 0.047851562f, 0.08984375f)),
            new SdfIcon(83, (char)0x53F1, 62, 80, new SdfUvRect(0.027669271f, 0.0904948f, 0.047851562f, 0.11653645f)),
            new SdfIcon(84, (char)0x54F1, 62, 80, new SdfUvRect(0.027669271f, 0.1171875f, 0.047851562f, 0.14322917f)),
            new SdfIcon(85, (char)0x55F1, 80, 80, new SdfUvRect(0.05240885f, 0.042643227f, 0.078450516f, 0.06868489f)),
            new SdfIcon(86, (char)0x56F1, 79, 72, new SdfUvRect(0.07910156f, 0.042643227f, 0.1048177f, 0.06608073f)),
            new SdfIcon(87, (char)0x57F1, 79, 72, new SdfUvRect(0.10546875f, 0.042643227f, 0.1311849f, 0.06608073f)),
            new SdfIcon(88, (char)0x58F1, 79, 72, new SdfUvRect(0.13183594f, 0.042643227f, 0.1575521f, 0.06608073f)),
            new SdfIcon(89, (char)0x59F1, 79, 72, new SdfUvRect(0.15820312f, 0.042643227f, 0.18391928f, 0.06608073f)),
            new SdfIcon(90, (char)0x5AF1, 80, 65, new SdfUvRect(0.18457031f, 0.042643227f, 0.21061198f, 0.06380208f)),
            new SdfIcon(91, (char)0x5BF1, 80, 66, new SdfUvRect(0.21126302f, 0.042643227f, 0.23730469f, 0.0641276f)),
            new SdfIcon(92, (char)0x5CF1, 80, 66, new SdfUvRect(0.23795572f, 0.042643227f, 0.26399738f, 0.0641276f)),
            new SdfIcon(93, (char)0x5DF1, 80, 66, new SdfUvRect(0.26464844f, 0.042643227f, 0.2906901f, 0.0641276f)),
            new SdfIcon(94, (char)0x5EF1, 80, 66, new SdfUvRect(0.29134116f, 0.042643227f, 0.3173828f, 0.0641276f)),
            new SdfIcon(95, (char)0x5FF1, 80, 66, new SdfUvRect(0.31803387f, 0.042643227f, 0.3440755f, 0.0641276f)),
            new SdfIcon(96, (char)0x60F1, 80, 66, new SdfUvRect(0.34472656f, 0.042643227f, 0.37076822f, 0.0641276f)),
            new SdfIcon(97, (char)0x61F1, 80, 66, new SdfUvRect(0.37141928f, 0.042643227f, 0.39746094f, 0.0641276f)),
            new SdfIcon(98, (char)0x62F1, 80, 65, new SdfUvRect(0.398112f, 0.042643227f, 0.42415363f, 0.06380208f)),
            new SdfIcon(99, (char)0x63F1, 80, 66, new SdfUvRect(0.4248047f, 0.042643227f, 0.45084634f, 0.0641276f)),
            new SdfIcon(100, (char)0x64F1, 80, 66, new SdfUvRect(0.4514974f, 0.042643227f, 0.47753906f, 0.0641276f)),
            new SdfIcon(101, (char)0x65F1, 80, 66, new SdfUvRect(0.47819012f, 0.042643227f, 0.5042318f, 0.0641276f)),
            new SdfIcon(102, (char)0x66F1, 80, 66, new SdfUvRect(0.5048828f, 0.042643227f, 0.5309245f, 0.0641276f)),
            new SdfIcon(103, (char)0x67F1, 80, 66, new SdfUvRect(0.5315755f, 0.042643227f, 0.5576172f, 0.0641276f)),
            new SdfIcon(104, (char)0x68F1, 80, 66, new SdfUvRect(0.5582682f, 0.042643227f, 0.58430994f, 0.0641276f)),
            new SdfIcon(105, (char)0x69F1, 80, 66, new SdfUvRect(0.58496094f, 0.042643227f, 0.6110026f, 0.0641276f)),
            new SdfIcon(106, (char)0x6AF1, 80, 66, new SdfUvRect(0.6116536f, 0.042643227f, 0.6376953f, 0.0641276f)),
            new SdfIcon(107, (char)0x6BF1, 80, 66, new SdfUvRect(0.6383463f, 0.042643227f, 0.66438806f, 0.0641276f)),
            new SdfIcon(108, (char)0x6CF1, 80, 65, new SdfUvRect(0.66503906f, 0.042643227f, 0.69108075f, 0.06380208f)),
            new SdfIcon(109, (char)0x6DF1, 80, 66, new SdfUvRect(0.69173175f, 0.042643227f, 0.71777344f, 0.0641276f)),
            new SdfIcon(110, (char)0x6EF1, 80, 65, new SdfUvRect(0.71842444f, 0.042643227f, 0.7444662f, 0.06380208f)),
            new SdfIcon(111, (char)0x6FF1, 80, 66, new SdfUvRect(0.7451172f, 0.042643227f, 0.7711589f, 0.0641276f)),
            new SdfIcon(112, (char)0x70F1, 72, 80, new SdfUvRect(0.027669271f, 0.1438802f, 0.051106773f, 0.16992188f)),
            new SdfIcon(113, (char)0x71F1, 72, 80, new SdfUvRect(0.027669271f, 0.1705729f, 0.051106773f, 0.1966146f)),
            new SdfIcon(114, (char)0x72F1, 72, 80, new SdfUvRect(0.027669271f, 0.19726562f, 0.051106773f, 0.2233073f)),
            new SdfIcon(115, (char)0x73F1, 72, 80, new SdfUvRect(0.027669271f, 0.22395833f, 0.051106773f, 0.25f)),
            new SdfIcon(116, (char)0x74F1, 72, 80, new SdfUvRect(0.027669271f, 0.25065106f, 0.051106773f, 0.2766927f)),
            new SdfIcon(117, (char)0x75F1, 72, 80, new SdfUvRect(0.027669271f, 0.27734375f, 0.051106773f, 0.3033854f)),
            new SdfIcon(118, (char)0x76F1, 72, 80, new SdfUvRect(0.027669271f, 0.30403647f, 0.051106773f, 0.33007812f)),
            new SdfIcon(119, (char)0x77F1, 72, 80, new SdfUvRect(0.027669271f, 0.3307292f, 0.051106773f, 0.3567708f)),
            new SdfIcon(120, (char)0x78F1, 72, 80, new SdfUvRect(0.027669271f, 0.35742188f, 0.051106773f, 0.38346353f)),
            new SdfIcon(121, (char)0x79F1, 72, 80, new SdfUvRect(0.027669271f, 0.3841146f, 0.051106773f, 0.41015625f)),
            new SdfIcon(122, (char)0x7AF1, 72, 72, new SdfUvRect(0.027669271f, 0.4108073f, 0.051106773f, 0.43424478f)),
            new SdfIcon(123, (char)0x7BF1, 80, 72, new SdfUvRect(0.7718099f, 0.042643227f, 0.79785156f, 0.06608073f)),
            new SdfIcon(124, (char)0x7CF1, 80, 72, new SdfUvRect(0.79850256f, 0.042643227f, 0.8245443f, 0.06608073f)),
            new SdfIcon(125, (char)0x7DF1, 80, 80, new SdfUvRect(0.8251953f, 0.042643227f, 0.851237f, 0.06868489f)),
            new SdfIcon(126, (char)0x7EF1, 72, 72, new SdfUvRect(0.027669271f, 0.43489584f, 0.051106773f, 0.4583333f)),
            new SdfIcon(127, (char)0x7FF1, 80, 77, new SdfUvRect(0.851888f, 0.042643227f, 0.8779297f, 0.06770833f)),
            new SdfIcon(128, (char)0x80F1, 80, 77, new SdfUvRect(0.8785807f, 0.042643227f, 0.90462244f, 0.06770833f)),
            new SdfIcon(129, (char)0x81F1, 80, 73, new SdfUvRect(0.90527344f, 0.042643227f, 0.9313151f, 0.06640625f)),
            new SdfIcon(130, (char)0x82F1, 80, 73, new SdfUvRect(0.9319661f, 0.042643227f, 0.9580078f, 0.06640625f)),
            new SdfIcon(131, (char)0x83F1, 80, 77, new SdfUvRect(0.9586588f, 0.042643227f, 0.98470056f, 0.06770833f)),
            new SdfIcon(132, (char)0x84F1, 80, 77, new SdfUvRect(0.05240885f, 0.06933594f, 0.078450516f, 0.09440104f)),
            new SdfIcon(133, (char)0x85F1, 80, 61, new SdfUvRect(0.05240885f, 0.095052086f, 0.078450516f, 0.11490885f)),
            new SdfIcon(134, (char)0x86F1, 80, 48, new SdfUvRect(0.05240885f, 0.1155599f, 0.078450516f, 0.1311849f)),
            new SdfIcon(135, (char)0x87F1, 80, 48, new SdfUvRect(0.05240885f, 0.13183594f, 0.078450516f, 0.14746094f)),
            new SdfIcon(136, (char)0x88F1, 80, 48, new SdfUvRect(0.05240885f, 0.14811197f, 0.078450516f, 0.16373698f)),
            new SdfIcon(137, (char)0x89F1, 72, 80, new SdfUvRect(0.027669271f, 0.45898438f, 0.051106773f, 0.48502603f)),
            new SdfIcon(138, (char)0x8AF1, 72, 80, new SdfUvRect(0.027669271f, 0.4856771f, 0.051106773f, 0.51171875f)),
            new SdfIcon(139, (char)0x8BF1, 80, 58, new SdfUvRect(0.05240885f, 0.16438802f, 0.078450516f, 0.18326823f)),
            new SdfIcon(140, (char)0x8CF1, 72, 72, new SdfUvRect(0.027669271f, 0.51236975f, 0.051106773f, 0.5358073f)),
            new SdfIcon(141, (char)0x8DF1, 82, 53, new SdfUvRect(0.07910156f, 0.06933594f, 0.105794266f, 0.08658854f)),
            new SdfIcon(142, (char)0x8EF1, 72, 76, new SdfUvRect(0.027669271f, 0.5364583f, 0.051106773f, 0.56119794f)),
            new SdfIcon(143, (char)0x8FF1, 72, 76, new SdfUvRect(0.027669271f, 0.56184894f, 0.051106773f, 0.58658856f)),
            new SdfIcon(144, (char)0x90F1, 66, 58, new SdfUvRect(0.027669271f, 0.58723956f, 0.04915365f, 0.6061198f)),
            new SdfIcon(145, (char)0x91F1, 65, 58, new SdfUvRect(0.027669271f, 0.6067708f, 0.048828125f, 0.62565106f)),
            new SdfIcon(146, (char)0x92F1, 80, 71, new SdfUvRect(0.05240885f, 0.18391927f, 0.078450516f, 0.20703125f)),
            new SdfIcon(147, (char)0x93F1, 80, 71, new SdfUvRect(0.05240885f, 0.20768228f, 0.078450516f, 0.23079428f)),
            new SdfIcon(148, (char)0x94F1, 80, 71, new SdfUvRect(0.05240885f, 0.23144531f, 0.078450516f, 0.25455728f)),
            new SdfIcon(149, (char)0x95F1, 66, 81, new SdfUvRect(0.027669271f, 0.62630206f, 0.04915365f, 0.6526693f)),
            new SdfIcon(150, (char)0x96F1, 65, 81, new SdfUvRect(0.02766927f, 0.6533203f, 0.048828125f, 0.6796875f)),
            new SdfIcon(151, (char)0x97F1, 66, 81, new SdfUvRect(0.027669271f, 0.6803385f, 0.04915365f, 0.70670575f)),
            new SdfIcon(152, (char)0x98F1, 65, 81, new SdfUvRect(0.027669271f, 0.70735675f, 0.048828125f, 0.733724f)),
            new SdfIcon(153, (char)0x99F1, 66, 81, new SdfUvRect(0.02766927f, 0.734375f, 0.04915365f, 0.7607422f)),
            new SdfIcon(154, (char)0x9AF1, 66, 81, new SdfUvRect(0.027669271f, 0.7613932f, 0.04915365f, 0.78776044f)),
            new SdfIcon(155, (char)0x9BF1, 65, 81, new SdfUvRect(0.027669271f, 0.78841144f, 0.048828125f, 0.8147787f)),
            new SdfIcon(156, (char)0x9CF1, 66, 81, new SdfUvRect(0.02766927f, 0.8154297f, 0.04915365f, 0.8417969f)),
            new SdfIcon(157, (char)0x9DF1, 65, 81, new SdfUvRect(0.027669271f, 0.8424479f, 0.048828125f, 0.8688151f)),
            new SdfIcon(158, (char)0x9EF1, 66, 81, new SdfUvRect(0.027669271f, 0.8694661f, 0.04915365f, 0.8958334f)),
            new SdfIcon(159, (char)0x9FF1, 65, 81, new SdfUvRect(0.02766927f, 0.8964844f, 0.048828125f, 0.92285156f)),
            new SdfIcon(160, (char)0xA0F1, 66, 81, new SdfUvRect(0.027669271f, 0.92350256f, 0.04915365f, 0.9498698f)),
            new SdfIcon(161, (char)0xA1F1, 65, 81, new SdfUvRect(0.027669271f, 0.9505208f, 0.048828125f, 0.97688806f)),
            new SdfIcon(162, (char)0xA2F1, 65, 81, new SdfUvRect(0.05240885f, 0.25520834f, 0.0735677f, 0.2815755f)),
            new SdfIcon(163, (char)0xA3F1, 65, 81, new SdfUvRect(0.05240885f, 0.28222656f, 0.0735677f, 0.30859375f)),
            new SdfIcon(164, (char)0xA4F1, 65, 81, new SdfUvRect(0.05240885f, 0.3092448f, 0.0735677f, 0.33561197f)),
            new SdfIcon(165, (char)0xA5F1, 65, 80, new SdfUvRect(0.05240885f, 0.33626303f, 0.0735677f, 0.3623047f)),
            new SdfIcon(166, (char)0xA6F1, 80, 80, new SdfUvRect(0.05240885f, 0.36295575f, 0.078450516f, 0.38899738f)),
            new SdfIcon(167, (char)0xA7F1, 82, 81, new SdfUvRect(0.07910156f, 0.087239586f, 0.105794266f, 0.113606766f)),
            new SdfIcon(168, (char)0xA8F1, 80, 80, new SdfUvRect(0.05240885f, 0.38964844f, 0.078450516f, 0.4156901f)),
            new SdfIcon(169, (char)0xA9F1, 80, 80, new SdfUvRect(0.05240885f, 0.41634116f, 0.078450516f, 0.4423828f)),
            new SdfIcon(170, (char)0xAAF1, 80, 80, new SdfUvRect(0.05240885f, 0.44303387f, 0.078450516f, 0.4690755f)),
            new SdfIcon(171, (char)0xABF1, 80, 80, new SdfUvRect(0.05240885f, 0.46972656f, 0.078450516f, 0.49576822f)),
            new SdfIcon(172, (char)0xACF1, 80, 80, new SdfUvRect(0.05240885f, 0.49641928f, 0.078450516f, 0.52246094f)),
            new SdfIcon(173, (char)0xADF1, 80, 80, new SdfUvRect(0.05240885f, 0.52311194f, 0.078450516f, 0.5491537f)),
            new SdfIcon(174, (char)0xAEF1, 80, 80, new SdfUvRect(0.05240885f, 0.5498047f, 0.078450516f, 0.5758464f)),
            new SdfIcon(175, (char)0xAFF1, 80, 80, new SdfUvRect(0.05240885f, 0.5764974f, 0.078450516f, 0.60253906f)),
            new SdfIcon(176, (char)0xB0F1, 80, 80, new SdfUvRect(0.05240885f, 0.60319006f, 0.078450516f, 0.6292318f)),
            new SdfIcon(177, (char)0xB1F1, 72, 58, new SdfUvRect(0.027669271f, 0.97753906f, 0.051106773f, 0.9964193f)),
            new SdfIcon(178, (char)0xB2F1, 80, 80, new SdfUvRect(0.05240885f, 0.6298828f, 0.078450516f, 0.6559245f)),
            new SdfIcon(179, (char)0xB3F1, 80, 58, new SdfUvRect(0.05240885f, 0.6565755f, 0.078450516f, 0.67545575f)),
            new SdfIcon(180, (char)0xB4F1, 80, 80, new SdfUvRect(0.05240885f, 0.67610675f, 0.078450516f, 0.70214844f)),
            new SdfIcon(181, (char)0xB5F1, 82, 82, new SdfUvRect(0.07910156f, 0.11425781f, 0.105794266f, 0.14095053f)),
            new SdfIcon(182, (char)0xB6F1, 80, 80, new SdfUvRect(0.05240885f, 0.70279944f, 0.078450516f, 0.7288412f)),
            new SdfIcon(183, (char)0xB7F1, 80, 80, new SdfUvRect(0.05240885f, 0.7294922f, 0.078450516f, 0.7555339f)),
            new SdfIcon(184, (char)0xB8F1, 80, 80, new SdfUvRect(0.05240885f, 0.7561849f, 0.078450516f, 0.78222656f)),
            new SdfIcon(185, (char)0xB9F1, 66, 80, new SdfUvRect(0.05240885f, 0.78287756f, 0.07389323f, 0.8089193f)),
            new SdfIcon(186, (char)0xBAF1, 72, 72, new SdfUvRect(0.05240885f, 0.8095703f, 0.07584635f, 0.8330078f)),
            new SdfIcon(187, (char)0xBBF1, 73, 73, new SdfUvRect(0.05240885f, 0.8336588f, 0.076171875f, 0.8574219f)),
            new SdfIcon(188, (char)0xBCF1, 66, 76, new SdfUvRect(0.05240885f, 0.8580729f, 0.07389323f, 0.8828125f)),
            new SdfIcon(189, (char)0xBDF1, 76, 65, new SdfUvRect(0.05240885f, 0.8834635f, 0.07714844f, 0.90462244f)),
            new SdfIcon(190, (char)0xBEF1, 76, 66, new SdfUvRect(0.05240885f, 0.90527344f, 0.07714844f, 0.9267578f)),
            new SdfIcon(191, (char)0xBFF1, 72, 72, new SdfUvRect(0.05240885f, 0.9274088f, 0.07584635f, 0.9508464f)),
            new SdfIcon(192, (char)0xC0F1, 72, 72, new SdfUvRect(0.05240885f, 0.9514974f, 0.07584635f, 0.97493494f)),
            new SdfIcon(193, (char)0xC1F1, 66, 76, new SdfUvRect(0.07910156f, 0.14160156f, 0.10058594f, 0.16634116f)),
            new SdfIcon(194, (char)0xC2F1, 80, 66, new SdfUvRect(0.05240885f, 0.97558594f, 0.078450516f, 0.9970703f)),
            new SdfIcon(195, (char)0xC3F1, 80, 66, new SdfUvRect(0.07910156f, 0.16699219f, 0.10514323f, 0.18847656f)),
            new SdfIcon(196, (char)0xC4F1, 80, 80, new SdfUvRect(0.07910156f, 0.1891276f, 0.10514323f, 0.21516928f)),
            new SdfIcon(197, (char)0xC5F1, 80, 80, new SdfUvRect(0.07910156f, 0.21582031f, 0.10514323f, 0.24186198f)),
            new SdfIcon(198, (char)0xC6F1, 66, 80, new SdfUvRect(0.07910156f, 0.24251302f, 0.10058594f, 0.2685547f)),
            new SdfIcon(199, (char)0xC7F1, 80, 82, new SdfUvRect(0.07910156f, 0.26920575f, 0.10514323f, 0.29589844f)),
            new SdfIcon(200, (char)0xC8F1, 80, 82, new SdfUvRect(0.07910156f, 0.2965495f, 0.10514323f, 0.3232422f)),
            new SdfIcon(201, (char)0xC9F1, 72, 65, new SdfUvRect(0.07910156f, 0.32389325f, 0.10253906f, 0.34505206f)),
            new SdfIcon(202, (char)0xCAF1, 80, 80, new SdfUvRect(0.07910156f, 0.34570312f, 0.10514323f, 0.37174478f)),
            new SdfIcon(203, (char)0xCBF1, 80, 69, new SdfUvRect(0.07910156f, 0.37239584f, 0.10514323f, 0.39485675f)),
            new SdfIcon(204, (char)0xCCF1, 80, 69, new SdfUvRect(0.07910156f, 0.3955078f, 0.10514323f, 0.41796875f)),
            new SdfIcon(205, (char)0xCDF1, 80, 52, new SdfUvRect(0.10644531f, 0.06933594f, 0.13248698f, 0.086263016f)),
            new SdfIcon(206, (char)0xCEF1, 80, 52, new SdfUvRect(0.13313802f, 0.06933594f, 0.15917969f, 0.086263016f)),
            new SdfIcon(207, (char)0xCFF1, 65, 43, new SdfUvRect(0.059570312f, 0.00032552084f, 0.080729164f, 0.014322917f)),
            new SdfIcon(208, (char)0xD0F1, 65, 43, new SdfUvRect(0.08138021f, 0.00032552084f, 0.10253906f, 0.014322917f)),
            new SdfIcon(209, (char)0xD1F1, 80, 80, new SdfUvRect(0.07910156f, 0.4186198f, 0.10514323f, 0.44466144f)),
            new SdfIcon(210, (char)0xD2F1, 80, 80, new SdfUvRect(0.07910156f, 0.4453125f, 0.10514323f, 0.47135416f)),
            new SdfIcon(211, (char)0xD3F1, 65, 65, new SdfUvRect(0.07910156f, 0.47200522f, 0.100260414f, 0.49316406f)),
            new SdfIcon(212, (char)0xD4F1, 65, 65, new SdfUvRect(0.07910156f, 0.49381512f, 0.100260414f, 0.514974f)),
            new SdfIcon(213, (char)0xD5F1, 82, 72, new SdfUvRect(0.07910156f, 0.515625f, 0.105794266f, 0.5390625f)),
            new SdfIcon(214, (char)0xD6F1, 82, 64, new SdfUvRect(0.07910156f, 0.5397135f, 0.105794266f, 0.5605469f)),
            new SdfIcon(215, (char)0xD7F1, 80, 80, new SdfUvRect(0.07910156f, 0.5611979f, 0.10514323f, 0.5872396f)),
            new SdfIcon(216, (char)0xD8F1, 80, 80, new SdfUvRect(0.07910156f, 0.5878906f, 0.10514323f, 0.6139323f)),
            new SdfIcon(217, (char)0xD9F1, 70, 80, new SdfUvRect(0.07910156f, 0.6145833f, 0.101888016f, 0.640625f)),
            new SdfIcon(218, (char)0xDAF1, 70, 80, new SdfUvRect(0.07910156f, 0.641276f, 0.101888016f, 0.66731775f)),
            new SdfIcon(219, (char)0xDBF1, 72, 79, new SdfUvRect(0.07910156f, 0.66796875f, 0.10253906f, 0.69368494f)),
            new SdfIcon(220, (char)0xDCF1, 72, 79, new SdfUvRect(0.07910156f, 0.69433594f, 0.10253906f, 0.7200521f)),
            new SdfIcon(221, (char)0xDDF1, 76, 81, new SdfUvRect(0.07910156f, 0.7207031f, 0.10384114f, 0.7470703f)),
            new SdfIcon(222, (char)0xDEF1, 80, 80, new SdfUvRect(0.07910156f, 0.7477213f, 0.10514323f, 0.77376306f)),
            new SdfIcon(223, (char)0xDFF1, 66, 80, new SdfUvRect(0.07910156f, 0.77441406f, 0.10058594f, 0.80045575f)),
            new SdfIcon(224, (char)0xE0F1, 66, 80, new SdfUvRect(0.07910156f, 0.80110675f, 0.10058594f, 0.82714844f)),
            new SdfIcon(225, (char)0xE1F1, 80, 80, new SdfUvRect(0.07910156f, 0.82779944f, 0.10514323f, 0.8538412f)),
            new SdfIcon(226, (char)0xE2F1, 80, 80, new SdfUvRect(0.07910156f, 0.8544922f, 0.10514323f, 0.8805339f)),
            new SdfIcon(227, (char)0xE3F1, 80, 80, new SdfUvRect(0.07910156f, 0.8811849f, 0.10514323f, 0.90722656f)),
            new SdfIcon(228, (char)0xE4F1, 80, 80, new SdfUvRect(0.07910156f, 0.90787756f, 0.10514323f, 0.9339193f)),
            new SdfIcon(229, (char)0xE5F1, 80, 80, new SdfUvRect(0.07910156f, 0.9345703f, 0.10514323f, 0.960612f)),
            new SdfIcon(230, (char)0xE6F1, 80, 80, new SdfUvRect(0.07910156f, 0.961263f, 0.10514323f, 0.9873047f)),
            new SdfIcon(231, (char)0xE7F1, 80, 80, new SdfUvRect(0.10644531f, 0.087239586f, 0.13248698f, 0.11328125f)),
            new SdfIcon(232, (char)0xE8F1, 80, 80, new SdfUvRect(0.13313802f, 0.087239586f, 0.15917969f, 0.11328125f)),
            new SdfIcon(233, (char)0xE9F1, 80, 80, new SdfUvRect(0.15983072f, 0.087239586f, 0.1858724f, 0.11328125f)),
            new SdfIcon(234, (char)0xEAF1, 80, 80, new SdfUvRect(0.18652344f, 0.087239586f, 0.21256511f, 0.11328125f)),
            new SdfIcon(235, (char)0xEBF1, 80, 80, new SdfUvRect(0.21321614f, 0.087239586f, 0.23925781f, 0.11328125f)),
            new SdfIcon(236, (char)0xECF1, 80, 80, new SdfUvRect(0.23990884f, 0.087239586f, 0.2659505f, 0.11328125f)),
            new SdfIcon(237, (char)0xEDF1, 80, 80, new SdfUvRect(0.26660156f, 0.087239586f, 0.29264322f, 0.11328125f)),
            new SdfIcon(238, (char)0xEEF1, 80, 80, new SdfUvRect(0.29329428f, 0.087239586f, 0.31933594f, 0.11328125f)),
            new SdfIcon(239, (char)0xEFF1, 80, 80, new SdfUvRect(0.319987f, 0.087239586f, 0.34602863f, 0.11328125f)),
            new SdfIcon(240, (char)0xF0F1, 80, 80, new SdfUvRect(0.3466797f, 0.087239586f, 0.37272134f, 0.11328125f)),
            new SdfIcon(241, (char)0xF1F1, 80, 80, new SdfUvRect(0.3733724f, 0.087239586f, 0.39941406f, 0.11328125f)),
            new SdfIcon(242, (char)0xF2F1, 80, 80, new SdfUvRect(0.40006512f, 0.087239586f, 0.42610675f, 0.11328125f)),
            new SdfIcon(243, (char)0xF3F1, 80, 80, new SdfUvRect(0.4267578f, 0.087239586f, 0.45279947f, 0.11328125f)),
            new SdfIcon(244, (char)0xF4F1, 80, 80, new SdfUvRect(0.45345053f, 0.087239586f, 0.4794922f, 0.11328125f)),
            new SdfIcon(245, (char)0xF5F1, 80, 80, new SdfUvRect(0.48014325f, 0.087239586f, 0.50618494f, 0.11328125f)),
            new SdfIcon(246, (char)0xF6F1, 80, 80, new SdfUvRect(0.50683594f, 0.087239586f, 0.5328776f, 0.11328125f)),
            new SdfIcon(247, (char)0xF7F1, 80, 80, new SdfUvRect(0.5335286f, 0.087239586f, 0.5595703f, 0.11328125f)),
            new SdfIcon(248, (char)0xF8F1, 80, 80, new SdfUvRect(0.5602213f, 0.087239586f, 0.58626306f, 0.11328125f)),
            new SdfIcon(249, (char)0xF9F1, 80, 80, new SdfUvRect(0.58691406f, 0.087239586f, 0.61295575f, 0.11328125f)),
            new SdfIcon(250, (char)0xFAF1, 80, 80, new SdfUvRect(0.61360675f, 0.087239586f, 0.63964844f, 0.11328125f)),
            new SdfIcon(251, (char)0xFBF1, 80, 80, new SdfUvRect(0.64029944f, 0.087239586f, 0.6663412f, 0.11328125f)),
            new SdfIcon(252, (char)0xFCF1, 80, 80, new SdfUvRect(0.6669922f, 0.087239586f, 0.6930339f, 0.11328125f)),
            new SdfIcon(253, (char)0xFDF1, 80, 80, new SdfUvRect(0.6936849f, 0.087239586f, 0.71972656f, 0.11328125f)),
            new SdfIcon(254, (char)0xFEF1, 80, 80, new SdfUvRect(0.72037756f, 0.087239586f, 0.7464193f, 0.11328125f)),
            new SdfIcon(255, (char)0xFFF1, 80, 80, new SdfUvRect(0.7470703f, 0.087239586f, 0.773112f, 0.11328125f)),
            new SdfIcon(256, (char)0x00F2, 80, 80, new SdfUvRect(0.773763f, 0.087239586f, 0.7998047f, 0.11328125f)),
            new SdfIcon(257, (char)0x01F2, 80, 80, new SdfUvRect(0.8004557f, 0.087239586f, 0.82649744f, 0.11328125f)),
            new SdfIcon(258, (char)0x02F2, 80, 80, new SdfUvRect(0.82714844f, 0.087239586f, 0.8531901f, 0.11328125f)),
            new SdfIcon(259, (char)0x03F2, 80, 80, new SdfUvRect(0.8538411f, 0.087239586f, 0.8798828f, 0.11328125f)),
            new SdfIcon(260, (char)0x04F2, 80, 80, new SdfUvRect(0.8805338f, 0.087239586f, 0.90657556f, 0.11328125f)),
            new SdfIcon(261, (char)0x05F2, 80, 80, new SdfUvRect(0.90722656f, 0.087239586f, 0.93326825f, 0.11328125f)),
            new SdfIcon(262, (char)0x06F2, 80, 80, new SdfUvRect(0.93391925f, 0.087239586f, 0.95996094f, 0.11328125f)),
            new SdfIcon(263, (char)0x07F2, 80, 80, new SdfUvRect(0.96061194f, 0.087239586f, 0.9866537f, 0.11328125f)),
            new SdfIcon(264, (char)0x08F2, 80, 80, new SdfUvRect(0.10644531f, 0.1139323f, 0.13248698f, 0.13997397f)),
            new SdfIcon(265, (char)0x09F2, 80, 80, new SdfUvRect(0.10644531f, 0.140625f, 0.13248698f, 0.16666667f)),
            new SdfIcon(266, (char)0x0AF2, 80, 80, new SdfUvRect(0.10644531f, 0.1673177f, 0.13248698f, 0.19335938f)),
            new SdfIcon(267, (char)0x0BF2, 80, 80, new SdfUvRect(0.10644531f, 0.1940104f, 0.13248698f, 0.2200521f)),
            new SdfIcon(268, (char)0x0CF2, 80, 80, new SdfUvRect(0.10644531f, 0.22070312f, 0.13248698f, 0.2467448f)),
            new SdfIcon(269, (char)0x0DF2, 80, 80, new SdfUvRect(0.10644531f, 0.24739583f, 0.13248698f, 0.2734375f)),
            new SdfIcon(270, (char)0x0EF2, 80, 80, new SdfUvRect(0.10644531f, 0.27408856f, 0.13248698f, 0.3001302f)),
            new SdfIcon(271, (char)0x0FF2, 80, 80, new SdfUvRect(0.10644531f, 0.30078125f, 0.13248698f, 0.3268229f)),
            new SdfIcon(272, (char)0x10F2, 80, 80, new SdfUvRect(0.10644531f, 0.32747397f, 0.13248698f, 0.35351562f)),
            new SdfIcon(273, (char)0x11F2, 80, 80, new SdfUvRect(0.10644531f, 0.3541667f, 0.13248698f, 0.3802083f)),
            new SdfIcon(274, (char)0x12F2, 80, 80, new SdfUvRect(0.10644531f, 0.38085938f, 0.13248698f, 0.40690103f)),
            new SdfIcon(275, (char)0x13F2, 80, 80, new SdfUvRect(0.10644531f, 0.4075521f, 0.13248698f, 0.43359375f)),
            new SdfIcon(276, (char)0x14F2, 80, 80, new SdfUvRect(0.10644531f, 0.4342448f, 0.13248698f, 0.46028644f)),
            new SdfIcon(277, (char)0x15F2, 80, 80, new SdfUvRect(0.10644531f, 0.4609375f, 0.13248698f, 0.48697916f)),
            new SdfIcon(278, (char)0x16F2, 80, 80, new SdfUvRect(0.10644531f, 0.48763022f, 0.13248698f, 0.5136719f)),
            new SdfIcon(279, (char)0x17F2, 80, 80, new SdfUvRect(0.10644531f, 0.5143229f, 0.13248698f, 0.5403646f)),
            new SdfIcon(280, (char)0x18F2, 80, 80, new SdfUvRect(0.10644531f, 0.5410156f, 0.13248698f, 0.5670573f)),
            new SdfIcon(281, (char)0x19F2, 80, 66, new SdfUvRect(0.10644531f, 0.5677083f, 0.13248698f, 0.58919275f)),
            new SdfIcon(282, (char)0x1AF2, 80, 80, new SdfUvRect(0.10644531f, 0.58984375f, 0.13248698f, 0.61588544f)),
            new SdfIcon(283, (char)0x1BF2, 80, 80, new SdfUvRect(0.10644531f, 0.61653644f, 0.13248698f, 0.6425781f)),
            new SdfIcon(284, (char)0x1CF2, 80, 58, new SdfUvRect(0.10644531f, 0.6432291f, 0.13248698f, 0.6621094f)),
            new SdfIcon(285, (char)0x1DF2, 80, 76, new SdfUvRect(0.10644531f, 0.6627604f, 0.13248698f, 0.6875f)),
            new SdfIcon(286, (char)0x1EF2, 80, 76, new SdfUvRect(0.10644531f, 0.688151f, 0.13248698f, 0.7128906f)),
            new SdfIcon(287, (char)0x1FF2, 80, 58, new SdfUvRect(0.10644531f, 0.7135416f, 0.13248698f, 0.7324219f)),
            new SdfIcon(288, (char)0x20F2, 80, 66, new SdfUvRect(0.10644531f, 0.7330729f, 0.13248698f, 0.7545573f)),
            new SdfIcon(289, (char)0x21F2, 80, 66, new SdfUvRect(0.10644531f, 0.7552083f, 0.13248698f, 0.77669275f)),
            new SdfIcon(290, (char)0x22F2, 76, 77, new SdfUvRect(0.10644531f, 0.77734375f, 0.1311849f, 0.8024089f)),
            new SdfIcon(291, (char)0x23F2, 76, 77, new SdfUvRect(0.10644531f, 0.8030599f, 0.1311849f, 0.828125f)),
            new SdfIcon(292, (char)0x24F2, 80, 66, new SdfUvRect(0.10644531f, 0.828776f, 0.13248698f, 0.85026044f)),
            new SdfIcon(293, (char)0x25F2, 80, 66, new SdfUvRect(0.10644531f, 0.85091144f, 0.13248698f, 0.8723959f)),
            new SdfIcon(294, (char)0x26F2, 80, 66, new SdfUvRect(0.10644531f, 0.8730469f, 0.13248698f, 0.89453125f)),
            new SdfIcon(295, (char)0x27F2, 80, 66, new SdfUvRect(0.10644531f, 0.89518225f, 0.13248698f, 0.9166667f)),
            new SdfIcon(296, (char)0x28F2, 80, 66, new SdfUvRect(0.10644531f, 0.9173177f, 0.13248698f, 0.9388021f)),
            new SdfIcon(297, (char)0x29F2, 64, 46, new SdfUvRect(0.10319011f, 0.00032552084f, 0.12402344f, 0.0152994795f)),
            new SdfIcon(298, (char)0x2AF2, 80, 80, new SdfUvRect(0.10644531f, 0.9394531f, 0.13248698f, 0.9654948f)),
            new SdfIcon(299, (char)0x2BF2, 80, 80, new SdfUvRect(0.10644531f, 0.9661458f, 0.13248698f, 0.9921875f)),
            new SdfIcon(300, (char)0x2CF2, 64, 46, new SdfUvRect(0.124674484f, 0.00032552084f, 0.14550781f, 0.0152994795f)),
            new SdfIcon(301, (char)0x2DF2, 46, 64, new SdfUvRect(0.13313802f, 0.1139323f, 0.14811198f, 0.13476562f)),
            new SdfIcon(302, (char)0x2EF2, 80, 80, new SdfUvRect(0.13313802f, 0.13541666f, 0.15917969f, 0.16145834f)),
            new SdfIcon(303, (char)0x2FF2, 80, 80, new SdfUvRect(0.13313802f, 0.16210938f, 0.15917969f, 0.18815105f)),
            new SdfIcon(304, (char)0x30F2, 46, 64, new SdfUvRect(0.14876302f, 0.1139323f, 0.16373698f, 0.13476562f)),
            new SdfIcon(305, (char)0x31F2, 46, 64, new SdfUvRect(0.16438802f, 0.1139323f, 0.17936198f, 0.13476562f)),
            new SdfIcon(306, (char)0x32F2, 80, 80, new SdfUvRect(0.13313802f, 0.18880208f, 0.15917969f, 0.21484375f)),
            new SdfIcon(307, (char)0x33F2, 80, 80, new SdfUvRect(0.13313802f, 0.21549478f, 0.15917969f, 0.24153647f)),
            new SdfIcon(308, (char)0x34F2, 46, 64, new SdfUvRect(0.18001302f, 0.1139323f, 0.19498698f, 0.13476562f)),
            new SdfIcon(309, (char)0x35F2, 64, 46, new SdfUvRect(0.14615884f, 0.00032552084f, 0.16699219f, 0.0152994795f)),
            new SdfIcon(310, (char)0x36F2, 80, 80, new SdfUvRect(0.13313802f, 0.2421875f, 0.15917969f, 0.26822916f)),
            new SdfIcon(311, (char)0x37F2, 80, 80, new SdfUvRect(0.13313802f, 0.26888022f, 0.15917969f, 0.29492188f)),
            new SdfIcon(312, (char)0x38F2, 64, 46, new SdfUvRect(0.16764322f, 0.00032552084f, 0.18847656f, 0.0152994795f)),
            new SdfIcon(313, (char)0x39F2, 77, 76, new SdfUvRect(0.13313802f, 0.29557294f, 0.15820312f, 0.3203125f)),
            new SdfIcon(314, (char)0x3AF2, 77, 76, new SdfUvRect(0.13313802f, 0.32096356f, 0.15820312f, 0.34570312f)),
            new SdfIcon(315, (char)0x3BF2, 77, 76, new SdfUvRect(0.13313802f, 0.3463542f, 0.15820312f, 0.37109375f)),
            new SdfIcon(316, (char)0x3CF2, 77, 76, new SdfUvRect(0.13313802f, 0.3717448f, 0.15820312f, 0.39648438f)),
            new SdfIcon(317, (char)0x3DF2, 77, 76, new SdfUvRect(0.13313802f, 0.39713544f, 0.15820312f, 0.421875f)),
            new SdfIcon(318, (char)0x3EF2, 77, 76, new SdfUvRect(0.13313802f, 0.42252606f, 0.15820312f, 0.44726562f)),
            new SdfIcon(319, (char)0x3FF2, 77, 76, new SdfUvRect(0.13313802f, 0.4479167f, 0.15820312f, 0.47265625f)),
            new SdfIcon(320, (char)0x40F2, 77, 76, new SdfUvRect(0.13313802f, 0.4733073f, 0.15820312f, 0.49804688f)),
            new SdfIcon(321, (char)0x41F2, 77, 76, new SdfUvRect(0.13313802f, 0.49869794f, 0.15820312f, 0.5234375f)),
            new SdfIcon(322, (char)0x42F2, 77, 76, new SdfUvRect(0.13313802f, 0.5240885f, 0.15820312f, 0.5488281f)),
            new SdfIcon(323, (char)0x43F2, 77, 73, new SdfUvRect(0.13313802f, 0.5494791f, 0.15820312f, 0.5732422f)),
            new SdfIcon(324, (char)0x44F2, 77, 76, new SdfUvRect(0.13313802f, 0.5738932f, 0.15820312f, 0.5986328f)),
            new SdfIcon(325, (char)0x45F2, 77, 73, new SdfUvRect(0.13313802f, 0.5992838f, 0.15820312f, 0.6230469f)),
            new SdfIcon(326, (char)0x46F2, 80, 65, new SdfUvRect(0.13313802f, 0.6236979f, 0.15917969f, 0.6448568f)),
            new SdfIcon(327, (char)0x47F2, 80, 58, new SdfUvRect(0.19563802f, 0.1139323f, 0.22167969f, 0.1328125f)),
            new SdfIcon(328, (char)0x48F2, 80, 66, new SdfUvRect(0.13313802f, 0.6455078f, 0.15917969f, 0.6669922f)),
            new SdfIcon(329, (char)0x49F2, 80, 76, new SdfUvRect(0.13313802f, 0.6676432f, 0.15917969f, 0.6923828f)),
            new SdfIcon(330, (char)0x4AF2, 80, 76, new SdfUvRect(0.13313802f, 0.6930338f, 0.15917969f, 0.71777344f)),
            new SdfIcon(331, (char)0x4BF2, 80, 76, new SdfUvRect(0.13313802f, 0.71842444f, 0.15917969f, 0.74316406f)),
            new SdfIcon(332, (char)0x4CF2, 80, 78, new SdfUvRect(0.13313802f, 0.74381506f, 0.15917969f, 0.76920575f)),
            new SdfIcon(333, (char)0x4DF2, 80, 78, new SdfUvRect(0.13313802f, 0.76985675f, 0.15917969f, 0.79524744f)),
            new SdfIcon(334, (char)0x4EF2, 80, 78, new SdfUvRect(0.13313802f, 0.79589844f, 0.15917969f, 0.82128906f)),
            new SdfIcon(335, (char)0x4FF2, 80, 78, new SdfUvRect(0.13313802f, 0.82194006f, 0.15917969f, 0.84733075f)),
            new SdfIcon(336, (char)0x50F2, 80, 78, new SdfUvRect(0.13313802f, 0.84798175f, 0.15917969f, 0.87337244f)),
            new SdfIcon(337, (char)0x51F2, 80, 78, new SdfUvRect(0.13313802f, 0.87402344f, 0.15917969f, 0.89941406f)),
            new SdfIcon(338, (char)0x52F2, 80, 78, new SdfUvRect(0.13313802f, 0.90006506f, 0.15917969f, 0.92545575f)),
            new SdfIcon(339, (char)0x53F2, 80, 78, new SdfUvRect(0.13313802f, 0.92610675f, 0.15917969f, 0.95149744f)),
            new SdfIcon(340, (char)0x54F2, 80, 76, new SdfUvRect(0.13313802f, 0.95214844f, 0.15917969f, 0.97688806f)),
            new SdfIcon(341, (char)0x55F2, 80, 76, new SdfUvRect(0.15983072f, 0.13541666f, 0.1858724f, 0.16015625f)),
            new SdfIcon(342, (char)0x56F2, 80, 78, new SdfUvRect(0.15983072f, 0.16080728f, 0.1858724f, 0.18619792f)),
            new SdfIcon(343, (char)0x57F2, 80, 78, new SdfUvRect(0.15983072f, 0.18684895f, 0.1858724f, 0.2122396f)),
            new SdfIcon(344, (char)0x58F2, 80, 78, new SdfUvRect(0.15983072f, 0.21289062f, 0.1858724f, 0.23828125f)),
            new SdfIcon(345, (char)0x59F2, 80, 78, new SdfUvRect(0.15983072f, 0.23893228f, 0.1858724f, 0.2643229f)),
            new SdfIcon(346, (char)0x5AF2, 80, 78, new SdfUvRect(0.15983072f, 0.26497397f, 0.1858724f, 0.29036456f)),
            new SdfIcon(347, (char)0x5BF2, 80, 78, new SdfUvRect(0.15983072f, 0.29101562f, 0.1858724f, 0.31640625f)),
            new SdfIcon(348, (char)0x5CF2, 80, 78, new SdfUvRect(0.15983072f, 0.3170573f, 0.1858724f, 0.3424479f)),
            new SdfIcon(349, (char)0x5DF2, 80, 78, new SdfUvRect(0.15983072f, 0.34309897f, 0.1858724f, 0.36848956f)),
            new SdfIcon(350, (char)0x5EF2, 80, 78, new SdfUvRect(0.15983072f, 0.36914062f, 0.1858724f, 0.39453125f)),
            new SdfIcon(351, (char)0x5FF2, 80, 78, new SdfUvRect(0.15983072f, 0.3951823f, 0.1858724f, 0.4205729f)),
            new SdfIcon(352, (char)0x60F2, 80, 78, new SdfUvRect(0.15983072f, 0.42122397f, 0.1858724f, 0.44661456f)),
            new SdfIcon(353, (char)0x61F2, 80, 78, new SdfUvRect(0.15983072f, 0.44726562f, 0.1858724f, 0.47265625f)),
            new SdfIcon(354, (char)0x62F2, 80, 78, new SdfUvRect(0.15983072f, 0.4733073f, 0.1858724f, 0.4986979f)),
            new SdfIcon(355, (char)0x63F2, 80, 78, new SdfUvRect(0.15983072f, 0.49934897f, 0.1858724f, 0.5247396f)),
            new SdfIcon(356, (char)0x64F2, 80, 78, new SdfUvRect(0.15983072f, 0.5253906f, 0.1858724f, 0.55078125f)),
            new SdfIcon(357, (char)0x65F2, 80, 78, new SdfUvRect(0.15983072f, 0.55143225f, 0.1858724f, 0.57682294f)),
            new SdfIcon(358, (char)0x66F2, 80, 76, new SdfUvRect(0.18652344f, 0.13541666f, 0.21256511f, 0.16015625f)),
            new SdfIcon(359, (char)0x67F2, 80, 76, new SdfUvRect(0.21321614f, 0.13541666f, 0.23925781f, 0.16015625f)),
            new SdfIcon(360, (char)0x68F2, 80, 76, new SdfUvRect(0.23990884f, 0.13541666f, 0.2659505f, 0.16015625f)),
            new SdfIcon(361, (char)0x69F2, 67, 44, new SdfUvRect(0.1891276f, 0.00032552084f, 0.2109375f, 0.0146484375f)),
            new SdfIcon(362, (char)0x6AF2, 80, 80, new SdfUvRect(0.15983072f, 0.57747394f, 0.1858724f, 0.6035156f)),
            new SdfIcon(363, (char)0x6BF2, 80, 80, new SdfUvRect(0.15983072f, 0.6041666f, 0.1858724f, 0.6302084f)),
            new SdfIcon(364, (char)0x6CF2, 80, 80, new SdfUvRect(0.15983072f, 0.6308594f, 0.1858724f, 0.65690106f)),
            new SdfIcon(365, (char)0x6DF2, 80, 80, new SdfUvRect(0.15983072f, 0.65755206f, 0.1858724f, 0.68359375f)),
            new SdfIcon(366, (char)0x6EF2, 50, 44, new SdfUvRect(0.21158853f, 0.00032552084f, 0.2278646f, 0.0146484375f)),
            new SdfIcon(367, (char)0x6FF2, 77, 49, new SdfUvRect(0.15983072f, 0.06933594f, 0.18489584f, 0.08528645f)),
            new SdfIcon(368, (char)0x70F2, 74, 70, new SdfUvRect(0.26660156f, 0.13541666f, 0.2906901f, 0.15820312f)),
            new SdfIcon(369, (char)0x71F2, 74, 70, new SdfUvRect(0.29134116f, 0.13541666f, 0.3154297f, 0.15820312f)),
            new SdfIcon(370, (char)0x72F2, 63, 49, new SdfUvRect(0.18554688f, 0.06933594f, 0.20605469f, 0.08528645f)),
            new SdfIcon(371, (char)0x73F2, 72, 72, new SdfUvRect(0.31608075f, 0.13541666f, 0.33951822f, 0.15885417f)),
            new SdfIcon(372, (char)0x74F2, 72, 48, new SdfUvRect(0.20670572f, 0.06933594f, 0.23014323f, 0.08496094f)),
            new SdfIcon(373, (char)0x75F2, 72, 72, new SdfUvRect(0.34016928f, 0.13541666f, 0.36360675f, 0.15885417f)),
            new SdfIcon(374, (char)0x76F2, 48, 72, new SdfUvRect(0.3642578f, 0.13541666f, 0.3798828f, 0.15885417f)),
            new SdfIcon(375, (char)0x77F2, 48, 72, new SdfUvRect(0.38053387f, 0.13541666f, 0.39615884f, 0.15885417f)),
            new SdfIcon(376, (char)0x78F2, 62, 45, new SdfUvRect(0.22851562f, 0.00032552084f, 0.24869792f, 0.014973959f)),
            new SdfIcon(377, (char)0x79F2, 70, 34, new SdfUvRect(0.24934895f, 0.00032552084f, 0.2721354f, 0.0113932295f)),
            new SdfIcon(378, (char)0x7AF2, 34, 70, new SdfUvRect(0.98535156f, 0.042643227f, 0.9964193f, 0.06542969f)),
            new SdfIcon(379, (char)0x7BF2, 34, 70, new SdfUvRect(0.9873047f, 0.087239586f, 0.99837244f, 0.11002604f)),
            new SdfIcon(380, (char)0x7CF2, 70, 34, new SdfUvRect(0.27278647f, 0.00032552084f, 0.2955729f, 0.0113932295f)),
            new SdfIcon(381, (char)0x7DF2, 52, 66, new SdfUvRect(0.13313802f, 0.97753906f, 0.15006511f, 0.99902344f)),
            new SdfIcon(382, (char)0x7EF2, 70, 60, new SdfUvRect(0.22233072f, 0.1139323f, 0.24511719f, 0.13346355f)),
            new SdfIcon(383, (char)0x7FF2, 61, 70, new SdfUvRect(0.3968099f, 0.13541666f, 0.41666666f, 0.15820312f)),
            new SdfIcon(384, (char)0x80F2, 61, 70, new SdfUvRect(0.41731772f, 0.13541666f, 0.43717447f, 0.15820312f)),
            new SdfIcon(385, (char)0x81F2, 70, 60, new SdfUvRect(0.24576822f, 0.1139323f, 0.2685547f, 0.13346355f)),
            new SdfIcon(386, (char)0x82F2, 70, 46, new SdfUvRect(0.29622397f, 0.00032552084f, 0.3190104f, 0.0152994795f)),
            new SdfIcon(387, (char)0x83F2, 52, 66, new SdfUvRect(0.43782553f, 0.13541666f, 0.4547526f, 0.15690105f)),
            new SdfIcon(388, (char)0x84F2, 46, 70, new SdfUvRect(0.45540366f, 0.13541666f, 0.4703776f, 0.15820312f)),
            new SdfIcon(389, (char)0x85F2, 46, 70, new SdfUvRect(0.47102866f, 0.13541666f, 0.4860026f, 0.15820312f)),
            new SdfIcon(390, (char)0x86F2, 70, 46, new SdfUvRect(0.31966147f, 0.00032552084f, 0.3424479f, 0.0152994795f)),
            new SdfIcon(391, (char)0x87F2, 80, 80, new SdfUvRect(0.15983072f, 0.68424475f, 0.1858724f, 0.7102865f)),
            new SdfIcon(392, (char)0x88F2, 80, 80, new SdfUvRect(0.15983072f, 0.7109375f, 0.1858724f, 0.7369792f)),
            new SdfIcon(393, (char)0x89F2, 80, 80, new SdfUvRect(0.15983072f, 0.7376302f, 0.1858724f, 0.7636719f)),
            new SdfIcon(394, (char)0x8AF2, 80, 80, new SdfUvRect(0.15983072f, 0.7643229f, 0.1858724f, 0.7903646f)),
            new SdfIcon(395, (char)0x8BF2, 72, 80, new SdfUvRect(0.15983072f, 0.7910156f, 0.18326823f, 0.8170573f)),
            new SdfIcon(396, (char)0x8CF2, 72, 80, new SdfUvRect(0.15983072f, 0.8177083f, 0.18326823f, 0.84375f)),
            new SdfIcon(397, (char)0x8DF2, 72, 80, new SdfUvRect(0.15983072f, 0.844401f, 0.18326823f, 0.87044275f)),
            new SdfIcon(398, (char)0x8EF2, 72, 80, new SdfUvRect(0.15983072f, 0.87109375f, 0.18326823f, 0.89713544f)),
            new SdfIcon(399, (char)0x8FF2, 72, 80, new SdfUvRect(0.15983072f, 0.89778644f, 0.18326823f, 0.9238281f)),
            new SdfIcon(400, (char)0x90F2, 72, 80, new SdfUvRect(0.15983072f, 0.9244791f, 0.18326823f, 0.9505209f)),
            new SdfIcon(401, (char)0x91F2, 80, 80, new SdfUvRect(0.15983072f, 0.9511719f, 0.1858724f, 0.97721356f)),
            new SdfIcon(402, (char)0x92F2, 81, 81, new SdfUvRect(0.18652344f, 0.16080728f, 0.21289062f, 0.18717448f)),
            new SdfIcon(403, (char)0x93F2, 80, 80, new SdfUvRect(0.21354166f, 0.16080728f, 0.23958334f, 0.18684897f)),
            new SdfIcon(404, (char)0x94F2, 80, 62, new SdfUvRect(0.26920575f, 0.1139323f, 0.29524738f, 0.1341146f)),
            new SdfIcon(405, (char)0x95F2, 80, 61, new SdfUvRect(0.29589844f, 0.1139323f, 0.3219401f, 0.13378906f)),
            new SdfIcon(406, (char)0x96F2, 80, 62, new SdfUvRect(0.32259116f, 0.1139323f, 0.3486328f, 0.1341146f)),
            new SdfIcon(407, (char)0x97F2, 80, 61, new SdfUvRect(0.34928387f, 0.1139323f, 0.3753255f, 0.13378906f)),
            new SdfIcon(408, (char)0x98F2, 80, 62, new SdfUvRect(0.37597656f, 0.1139323f, 0.40201822f, 0.1341146f)),
            new SdfIcon(409, (char)0x99F2, 80, 61, new SdfUvRect(0.40266928f, 0.1139323f, 0.42871094f, 0.13378906f)),
            new SdfIcon(410, (char)0x9AF2, 80, 80, new SdfUvRect(0.24023438f, 0.16080728f, 0.26627603f, 0.18684897f)),
            new SdfIcon(411, (char)0x9BF2, 80, 80, new SdfUvRect(0.2669271f, 0.16080728f, 0.29296875f, 0.18684897f)),
            new SdfIcon(412, (char)0x9CF2, 82, 78, new SdfUvRect(0.2936198f, 0.16080728f, 0.3203125f, 0.18619792f)),
            new SdfIcon(413, (char)0x9DF2, 82, 78, new SdfUvRect(0.32096356f, 0.16080728f, 0.34765625f, 0.18619792f)),
            new SdfIcon(414, (char)0x9EF2, 80, 61, new SdfUvRect(0.429362f, 0.1139323f, 0.45540363f, 0.13378906f)),
            new SdfIcon(415, (char)0x9FF2, 82, 73, new SdfUvRect(0.48665366f, 0.13541666f, 0.5133464f, 0.15917969f)),
            new SdfIcon(416, (char)0xA0F2, 82, 73, new SdfUvRect(0.5139974f, 0.13541666f, 0.5406901f, 0.15917969f)),
            new SdfIcon(417, (char)0xA1F2, 82, 58, new SdfUvRect(0.4560547f, 0.1139323f, 0.48274738f, 0.1328125f)),
            new SdfIcon(418, (char)0xA2F2, 81, 58, new SdfUvRect(0.48339844f, 0.1139323f, 0.5097656f, 0.1328125f)),
            new SdfIcon(419, (char)0xA3F2, 82, 79, new SdfUvRect(0.3483073f, 0.16080728f, 0.375f, 0.18652344f)),
            new SdfIcon(420, (char)0xA4F2, 82, 79, new SdfUvRect(0.37565106f, 0.16080728f, 0.40234375f, 0.18652344f)),
            new SdfIcon(421, (char)0xA5F2, 34, 59, new SdfUvRect(0.5104166f, 0.1139323f, 0.5214844f, 0.13313803f)),
            new SdfIcon(422, (char)0xA6F2, 82, 81, new SdfUvRect(0.4029948f, 0.16080728f, 0.4296875f, 0.18717448f)),
            new SdfIcon(423, (char)0xA7F2, 82, 81, new SdfUvRect(0.43033856f, 0.16080728f, 0.45703125f, 0.18717448f)),
            new SdfIcon(424, (char)0xA8F2, 81, 65, new SdfUvRect(0.5413411f, 0.13541666f, 0.5677084f, 0.15657553f)),
            new SdfIcon(425, (char)0xA9F2, 82, 81, new SdfUvRect(0.4576823f, 0.16080728f, 0.484375f, 0.18717448f)),
            new SdfIcon(426, (char)0xAAF2, 82, 81, new SdfUvRect(0.48502606f, 0.16080728f, 0.51171875f, 0.18717448f)),
            new SdfIcon(427, (char)0xABF2, 82, 81, new SdfUvRect(0.51236975f, 0.16080728f, 0.5390625f, 0.18717448f)),
            new SdfIcon(428, (char)0xACF2, 82, 81, new SdfUvRect(0.5397135f, 0.16080728f, 0.56640625f, 0.18717448f)),
            new SdfIcon(429, (char)0xADF2, 80, 62, new SdfUvRect(0.5221354f, 0.1139323f, 0.5481771f, 0.1341146f)),
            new SdfIcon(430, (char)0xAEF2, 80, 61, new SdfUvRect(0.5488281f, 0.1139323f, 0.5748698f, 0.13378906f)),
            new SdfIcon(431, (char)0xAFF2, 81, 77, new SdfUvRect(0.56705725f, 0.16080728f, 0.5934245f, 0.1858724f)),
            new SdfIcon(432, (char)0xB0F2, 81, 77, new SdfUvRect(0.5940755f, 0.16080728f, 0.62044275f, 0.1858724f)),
            new SdfIcon(433, (char)0xB1F2, 80, 62, new SdfUvRect(0.5755208f, 0.1139323f, 0.6015625f, 0.1341146f)),
            new SdfIcon(434, (char)0xB2F2, 80, 61, new SdfUvRect(0.6022135f, 0.1139323f, 0.62825525f, 0.13378906f)),
            new SdfIcon(435, (char)0xB3F2, 82, 78, new SdfUvRect(0.62109375f, 0.16080728f, 0.6477865f, 0.18619792f)),
            new SdfIcon(436, (char)0xB4F2, 82, 81, new SdfUvRect(0.6484375f, 0.16080728f, 0.67513025f, 0.18717448f)),
            new SdfIcon(437, (char)0xB5F2, 82, 82, new SdfUvRect(0.18652344f, 0.18782552f, 0.21321616f, 0.21451823f)),
            new SdfIcon(438, (char)0xB6F2, 82, 78, new SdfUvRect(0.67578125f, 0.16080728f, 0.702474f, 0.18619792f)),
            new SdfIcon(439, (char)0xB7F2, 80, 68, new SdfUvRect(0.5683594f, 0.13541666f, 0.59440106f, 0.1575521f)),
            new SdfIcon(440, (char)0xB8F2, 80, 68, new SdfUvRect(0.59505206f, 0.13541666f, 0.62109375f, 0.1575521f)),
            new SdfIcon(441, (char)0xB9F2, 82, 81, new SdfUvRect(0.703125f, 0.16080728f, 0.72981775f, 0.18717448f)),
            new SdfIcon(442, (char)0xBAF2, 82, 81, new SdfUvRect(0.73046875f, 0.16080728f, 0.7571615f, 0.18717448f)),
            new SdfIcon(443, (char)0xBBF2, 82, 80, new SdfUvRect(0.7578125f, 0.16080728f, 0.78450525f, 0.18684897f)),
            new SdfIcon(444, (char)0xBCF2, 82, 80, new SdfUvRect(0.78515625f, 0.16080728f, 0.811849f, 0.18684897f)),
            new SdfIcon(445, (char)0xBDF2, 81, 76, new SdfUvRect(0.62174475f, 0.13541666f, 0.648112f, 0.16015625f)),
            new SdfIcon(446, (char)0xBEF2, 81, 76, new SdfUvRect(0.648763f, 0.13541666f, 0.67513025f, 0.16015625f)),
            new SdfIcon(447, (char)0xBFF2, 80, 76, new SdfUvRect(0.67578125f, 0.13541666f, 0.70182294f, 0.16015625f)),
            new SdfIcon(448, (char)0xC0F2, 80, 76, new SdfUvRect(0.70247394f, 0.13541666f, 0.7285156f, 0.16015625f)),
            new SdfIcon(449, (char)0xC1F2, 80, 61, new SdfUvRect(0.62890625f, 0.1139323f, 0.65494794f, 0.13378906f)),
            new SdfIcon(450, (char)0xC2F2, 82, 66, new SdfUvRect(0.7291666f, 0.13541666f, 0.7558594f, 0.15690105f)),
            new SdfIcon(451, (char)0xC3F2, 81, 66, new SdfUvRect(0.7565104f, 0.13541666f, 0.7828776f, 0.15690105f)),
            new SdfIcon(452, (char)0xC4F2, 82, 58, new SdfUvRect(0.65559894f, 0.1139323f, 0.6822917f, 0.1328125f)),
            new SdfIcon(453, (char)0xC5F2, 82, 58, new SdfUvRect(0.6829427f, 0.1139323f, 0.70963544f, 0.1328125f)),
            new SdfIcon(454, (char)0xC6F2, 78, 74, new SdfUvRect(0.7835286f, 0.13541666f, 0.8089193f, 0.15950522f)),
            new SdfIcon(455, (char)0xC7F2, 80, 80, new SdfUvRect(0.8125f, 0.16080728f, 0.8385417f, 0.18684897f)),
            new SdfIcon(456, (char)0xC8F2, 70, 48, new SdfUvRect(0.23079427f, 0.06933594f, 0.25358072f, 0.08496094f)),
            new SdfIcon(457, (char)0xC9F2, 80, 74, new SdfUvRect(0.8095703f, 0.13541666f, 0.835612f, 0.15950522f)),
            new SdfIcon(458, (char)0xCAF2, 80, 74, new SdfUvRect(0.836263f, 0.13541666f, 0.8623047f, 0.15950522f)),
            new SdfIcon(459, (char)0xCBF2, 80, 74, new SdfUvRect(0.8629557f, 0.13541666f, 0.88899744f, 0.15950522f)),
            new SdfIcon(460, (char)0xCCF2, 80, 74, new SdfUvRect(0.88964844f, 0.13541666f, 0.9156901f, 0.15950522f)),
            new SdfIcon(461, (char)0xCDF2, 80, 80, new SdfUvRect(0.8391927f, 0.16080728f, 0.8652344f, 0.18684897f)),
            new SdfIcon(462, (char)0xCEF2, 80, 72, new SdfUvRect(0.9163411f, 0.13541666f, 0.9423828f, 0.15885417f)),
            new SdfIcon(463, (char)0xCFF2, 72, 72, new SdfUvRect(0.9430338f, 0.13541666f, 0.9664714f, 0.15885417f)),
            new SdfIcon(464, (char)0xD0F2, 78, 81, new SdfUvRect(0.8658854f, 0.16080728f, 0.89127606f, 0.18717448f)),
            new SdfIcon(465, (char)0xD1F2, 78, 80, new SdfUvRect(0.89192706f, 0.16080728f, 0.91731775f, 0.18684897f)),
            new SdfIcon(466, (char)0xD2F2, 70, 79, new SdfUvRect(0.91796875f, 0.16080728f, 0.94075525f, 0.18652344f)),
            new SdfIcon(467, (char)0xD3F2, 70, 71, new SdfUvRect(0.9671224f, 0.13541666f, 0.9899089f, 0.15852866f)),
            new SdfIcon(468, (char)0xD4F2, 78, 63, new SdfUvRect(0.71028644f, 0.1139323f, 0.7356771f, 0.13444011f)),
            new SdfIcon(469, (char)0xD5F2, 80, 80, new SdfUvRect(0.94140625f, 0.16080728f, 0.96744794f, 0.18684897f)),
            new SdfIcon(470, (char)0xD6F2, 80, 80, new SdfUvRect(0.96809894f, 0.16080728f, 0.9941406f, 0.18684897f)),
            new SdfIcon(471, (char)0xD7F2, 80, 66, new SdfUvRect(0.15983072f, 0.97786456f, 0.1858724f, 0.999349f)),
            new SdfIcon(472, (char)0xD8F2, 80, 66, new SdfUvRect(0.18652344f, 0.21516927f, 0.21256511f, 0.23665366f)),
            new SdfIcon(473, (char)0xD9F2, 80, 65, new SdfUvRect(0.18652344f, 0.23730469f, 0.21256511f, 0.25846353f)),
            new SdfIcon(474, (char)0xDAF2, 80, 66, new SdfUvRect(0.18652344f, 0.2591146f, 0.21256511f, 0.28059894f)),
            new SdfIcon(475, (char)0xDBF2, 80, 66, new SdfUvRect(0.18652344f, 0.28125f, 0.21256511f, 0.30273438f)),
            new SdfIcon(476, (char)0xDCF2, 80, 65, new SdfUvRect(0.18652344f, 0.30338544f, 0.21256511f, 0.32454425f)),
            new SdfIcon(477, (char)0xDDF2, 85, 85, new SdfUvRect(0.21386719f, 0.18782552f, 0.24153647f, 0.2154948f)),
            new SdfIcon(478, (char)0xDEF2, 76, 72, new SdfUvRect(0.18652344f, 0.3251953f, 0.21126303f, 0.3486328f)),
            new SdfIcon(479, (char)0xDFF2, 58, 80, new SdfUvRect(0.18652344f, 0.34928387f, 0.20540366f, 0.3753255f)),
            new SdfIcon(480, (char)0xE0F2, 76, 72, new SdfUvRect(0.18652344f, 0.37597656f, 0.21126303f, 0.39941406f)),
            new SdfIcon(481, (char)0xE1F2, 73, 73, new SdfUvRect(0.18652344f, 0.40006512f, 0.21028647f, 0.42382812f)),
            new SdfIcon(482, (char)0xE2F2, 40, 70, new SdfUvRect(0.18652344f, 0.4244792f, 0.19954428f, 0.44726562f)),
            new SdfIcon(483, (char)0xE3F2, 73, 73, new SdfUvRect(0.18652344f, 0.4479167f, 0.21028647f, 0.4716797f)),
            new SdfIcon(484, (char)0xE4F2, 80, 80, new SdfUvRect(0.18652344f, 0.47233075f, 0.21256511f, 0.49837238f)),
            new SdfIcon(485, (char)0xE5F2, 80, 80, new SdfUvRect(0.18652344f, 0.49902344f, 0.21256511f, 0.5250651f)),
            new SdfIcon(486, (char)0xE6F2, 80, 80, new SdfUvRect(0.18652344f, 0.5257161f, 0.21256511f, 0.5517578f)),
            new SdfIcon(487, (char)0xE7F2, 80, 80, new SdfUvRect(0.18652344f, 0.5524088f, 0.21256511f, 0.57845056f)),
            new SdfIcon(488, (char)0xE8F2, 80, 80, new SdfUvRect(0.18652344f, 0.57910156f, 0.21256511f, 0.60514325f)),
            new SdfIcon(489, (char)0xE9F2, 80, 80, new SdfUvRect(0.18652344f, 0.60579425f, 0.21256511f, 0.63183594f)),
            new SdfIcon(490, (char)0xEAF2, 48, 20, new SdfUvRect(0.34309897f, 0.00032552084f, 0.35872394f, 0.0068359375f)),
            new SdfIcon(491, (char)0xEBF2, 58, 66, new SdfUvRect(0.18652344f, 0.63248694f, 0.20540366f, 0.6539714f)),
            new SdfIcon(492, (char)0xECF2, 58, 66, new SdfUvRect(0.18652344f, 0.6546224f, 0.20540366f, 0.6761068f)),
            new SdfIcon(493, (char)0xEDF2, 80, 66, new SdfUvRect(0.18652344f, 0.6767578f, 0.21256511f, 0.6982422f)),
            new SdfIcon(494, (char)0xEEF2, 80, 66, new SdfUvRect(0.18652344f, 0.6988932f, 0.21256511f, 0.7203776f)),
            new SdfIcon(495, (char)0xEFF2, 80, 80, new SdfUvRect(0.18652344f, 0.7210286f, 0.21256511f, 0.7470703f)),
            new SdfIcon(496, (char)0xF0F2, 80, 80, new SdfUvRect(0.18652344f, 0.7477213f, 0.21256511f, 0.77376306f)),
            new SdfIcon(497, (char)0xF1F2, 80, 80, new SdfUvRect(0.18652344f, 0.77441406f, 0.21256511f, 0.80045575f)),
            new SdfIcon(498, (char)0xF2F2, 80, 80, new SdfUvRect(0.18652344f, 0.80110675f, 0.21256511f, 0.82714844f)),
            new SdfIcon(499, (char)0xF3F2, 80, 80, new SdfUvRect(0.18652344f, 0.82779944f, 0.21256511f, 0.8538412f)),
            new SdfIcon(500, (char)0xF4F2, 80, 80, new SdfUvRect(0.18652344f, 0.8544922f, 0.21256511f, 0.8805339f)),
            new SdfIcon(501, (char)0xF5F2, 80, 80, new SdfUvRect(0.18652344f, 0.8811849f, 0.21256511f, 0.90722656f)),
            new SdfIcon(502, (char)0xF6F2, 80, 80, new SdfUvRect(0.18652344f, 0.90787756f, 0.21256511f, 0.9339193f)),
            new SdfIcon(503, (char)0xF7F2, 80, 80, new SdfUvRect(0.18652344f, 0.9345703f, 0.21256511f, 0.960612f)),
            new SdfIcon(504, (char)0xF8F2, 80, 80, new SdfUvRect(0.18652344f, 0.961263f, 0.21256511f, 0.9873047f)),
            new SdfIcon(505, (char)0xF9F2, 80, 80, new SdfUvRect(0.2421875f, 0.18782552f, 0.26822916f, 0.21386719f)),
            new SdfIcon(506, (char)0xFAF2, 80, 80, new SdfUvRect(0.26888022f, 0.18782552f, 0.29492188f, 0.21386719f)),
            new SdfIcon(507, (char)0xFBF2, 80, 80, new SdfUvRect(0.29557294f, 0.18782552f, 0.32161456f, 0.21386719f)),
            new SdfIcon(508, (char)0xFCF2, 80, 80, new SdfUvRect(0.32226562f, 0.18782552f, 0.34830728f, 0.21386719f)),
            new SdfIcon(509, (char)0xFDF2, 80, 80, new SdfUvRect(0.34895834f, 0.18782552f, 0.375f, 0.21386719f)),
            new SdfIcon(510, (char)0xFEF2, 80, 80, new SdfUvRect(0.37565106f, 0.18782552f, 0.4016927f, 0.21386719f)),
            new SdfIcon(511, (char)0xFFF2, 80, 80, new SdfUvRect(0.40234375f, 0.18782552f, 0.4283854f, 0.21386719f)),
            new SdfIcon(512, (char)0x00F3, 83, 66, new SdfUvRect(0.42903647f, 0.18782552f, 0.4560547f, 0.2093099f)),
            new SdfIcon(513, (char)0x01F3, 80, 68, new SdfUvRect(0.45670575f, 0.18782552f, 0.48274738f, 0.20996094f)),
            new SdfIcon(514, (char)0x02F3, 80, 68, new SdfUvRect(0.48339844f, 0.18782552f, 0.5094401f, 0.20996094f)),
            new SdfIcon(515, (char)0x03F3, 72, 72, new SdfUvRect(0.5100911f, 0.18782552f, 0.5335287f, 0.21126303f)),
            new SdfIcon(516, (char)0x04F3, 72, 72, new SdfUvRect(0.5341797f, 0.18782552f, 0.5576172f, 0.21126303f)),
            new SdfIcon(517, (char)0x05F3, 72, 76, new SdfUvRect(0.5582682f, 0.18782552f, 0.58170575f, 0.21256511f)),
            new SdfIcon(518, (char)0x06F3, 72, 76, new SdfUvRect(0.58235675f, 0.18782552f, 0.6057943f, 0.21256511f)),
            new SdfIcon(519, (char)0x07F3, 72, 81, new SdfUvRect(0.6064453f, 0.18782552f, 0.6298828f, 0.21419272f)),
            new SdfIcon(520, (char)0x08F3, 72, 81, new SdfUvRect(0.6305338f, 0.18782552f, 0.6539714f, 0.21419272f)),
            new SdfIcon(521, (char)0x09F3, 28, 28, new SdfUvRect(0.359375f, 0.00032552084f, 0.36848956f, 0.0094401045f)),
            new SdfIcon(522, (char)0x0AF3, 80, 72, new SdfUvRect(0.6546224f, 0.18782552f, 0.68066406f, 0.21126303f)),
            new SdfIcon(523, (char)0x0BF3, 65, 80, new SdfUvRect(0.68131506f, 0.18782552f, 0.702474f, 0.21386719f)),
            new SdfIcon(524, (char)0x0CF3, 66, 80, new SdfUvRect(0.703125f, 0.18782552f, 0.7246094f, 0.21386719f)),
            new SdfIcon(525, (char)0x0DF3, 66, 80, new SdfUvRect(0.7252604f, 0.18782552f, 0.7467448f, 0.21386719f)),
            new SdfIcon(526, (char)0x0EF3, 82, 70, new SdfUvRect(0.7473958f, 0.18782552f, 0.77408856f, 0.21061198f)),
            new SdfIcon(527, (char)0x0FF3, 72, 79, new SdfUvRect(0.77473956f, 0.18782552f, 0.7981771f, 0.21354167f)),
            new SdfIcon(528, (char)0x10F3, 72, 79, new SdfUvRect(0.7988281f, 0.18782552f, 0.8222656f, 0.21354167f)),
            new SdfIcon(529, (char)0x11F3, 66, 80, new SdfUvRect(0.8229166f, 0.18782552f, 0.84440106f, 0.21386719f)),
            new SdfIcon(530, (char)0x12F3, 82, 82, new SdfUvRect(0.84505206f, 0.18782552f, 0.8717448f, 0.21451823f)),
            new SdfIcon(531, (char)0x13F3, 65, 80, new SdfUvRect(0.8723958f, 0.18782552f, 0.8935547f, 0.21386719f)),
            new SdfIcon(532, (char)0x14F3, 78, 68, new SdfUvRect(0.8942057f, 0.18782552f, 0.9195964f, 0.20996094f)),
            new SdfIcon(533, (char)0x15F3, 78, 68, new SdfUvRect(0.9202474f, 0.18782552f, 0.94563806f, 0.20996094f)),
            new SdfIcon(534, (char)0x16F3, 80, 80, new SdfUvRect(0.94628906f, 0.18782552f, 0.97233075f, 0.21386719f)),
            new SdfIcon(535, (char)0x17F3, 80, 80, new SdfUvRect(0.97298175f, 0.18782552f, 0.99902344f, 0.21386719f)),
            new SdfIcon(536, (char)0x18F3, 80, 80, new SdfUvRect(0.21386719f, 0.21614583f, 0.23990886f, 0.2421875f)),
            new SdfIcon(537, (char)0x19F3, 80, 80, new SdfUvRect(0.21386719f, 0.24283853f, 0.23990886f, 0.2688802f)),
            new SdfIcon(538, (char)0x1AF3, 80, 80, new SdfUvRect(0.21386719f, 0.26953125f, 0.23990886f, 0.2955729f)),
            new SdfIcon(539, (char)0x1BF3, 80, 80, new SdfUvRect(0.21386719f, 0.29622397f, 0.23990886f, 0.32226562f)),
            new SdfIcon(540, (char)0x1CF3, 80, 80, new SdfUvRect(0.21386719f, 0.3229167f, 0.23990886f, 0.3489583f)),
            new SdfIcon(541, (char)0x1DF3, 80, 80, new SdfUvRect(0.21386719f, 0.34960938f, 0.23990886f, 0.37565103f)),
            new SdfIcon(542, (char)0x1EF3, 80, 80, new SdfUvRect(0.21386719f, 0.3763021f, 0.23990886f, 0.40234375f)),
            new SdfIcon(543, (char)0x1FF3, 80, 80, new SdfUvRect(0.21386719f, 0.4029948f, 0.23990886f, 0.42903644f)),
            new SdfIcon(544, (char)0x20F3, 80, 80, new SdfUvRect(0.21386719f, 0.4296875f, 0.23990886f, 0.45572916f)),
            new SdfIcon(545, (char)0x21F3, 80, 80, new SdfUvRect(0.21386719f, 0.45638022f, 0.23990886f, 0.48242188f)),
            new SdfIcon(546, (char)0x22F3, 80, 80, new SdfUvRect(0.21386719f, 0.48307294f, 0.23990886f, 0.5091146f)),
            new SdfIcon(547, (char)0x23F3, 80, 80, new SdfUvRect(0.21386719f, 0.5097656f, 0.23990886f, 0.5358073f)),
            new SdfIcon(548, (char)0x24F3, 80, 80, new SdfUvRect(0.21386719f, 0.5364583f, 0.23990886f, 0.5625f)),
            new SdfIcon(549, (char)0x25F3, 80, 80, new SdfUvRect(0.21386719f, 0.563151f, 0.23990886f, 0.58919275f)),
            new SdfIcon(550, (char)0x26F3, 80, 80, new SdfUvRect(0.21386719f, 0.58984375f, 0.23990886f, 0.61588544f)),
            new SdfIcon(551, (char)0x27F3, 80, 80, new SdfUvRect(0.21386719f, 0.61653644f, 0.23990886f, 0.6425781f)),
            new SdfIcon(552, (char)0x28F3, 80, 80, new SdfUvRect(0.21386719f, 0.6432291f, 0.23990886f, 0.6692709f)),
            new SdfIcon(553, (char)0x29F3, 80, 80, new SdfUvRect(0.21386719f, 0.6699219f, 0.23990886f, 0.69596356f)),
            new SdfIcon(554, (char)0x2AF3, 80, 80, new SdfUvRect(0.21386719f, 0.69661456f, 0.23990886f, 0.72265625f)),
            new SdfIcon(555, (char)0x2BF3, 80, 80, new SdfUvRect(0.21386719f, 0.72330725f, 0.23990886f, 0.749349f)),
            new SdfIcon(556, (char)0x2CF3, 80, 66, new SdfUvRect(0.21386719f, 0.75f, 0.23990886f, 0.7714844f)),
            new SdfIcon(557, (char)0x2DF3, 80, 80, new SdfUvRect(0.21386719f, 0.7721354f, 0.23990886f, 0.7981771f)),
            new SdfIcon(558, (char)0x2EF3, 80, 80, new SdfUvRect(0.21386719f, 0.7988281f, 0.23990886f, 0.8248698f)),
            new SdfIcon(559, (char)0x2FF3, 80, 65, new SdfUvRect(0.21386719f, 0.8255208f, 0.23990886f, 0.8466797f)),
            new SdfIcon(560, (char)0x30F3, 76, 70, new SdfUvRect(0.21386719f, 0.8473307f, 0.23860678f, 0.8701172f)),
            new SdfIcon(561, (char)0x31F3, 76, 70, new SdfUvRect(0.21386719f, 0.8707682f, 0.23860678f, 0.8935547f)),
            new SdfIcon(562, (char)0x32F3, 80, 80, new SdfUvRect(0.21386719f, 0.8942057f, 0.23990886f, 0.92024744f)),
            new SdfIcon(563, (char)0x33F3, 80, 80, new SdfUvRect(0.21386719f, 0.92089844f, 0.23990886f, 0.9469401f)),
            new SdfIcon(564, (char)0x34F3, 80, 80, new SdfUvRect(0.21386719f, 0.9475911f, 0.23990886f, 0.9736328f)),
            new SdfIcon(565, (char)0x35F3, 80, 80, new SdfUvRect(0.24055989f, 0.21614583f, 0.26660156f, 0.2421875f)),
            new SdfIcon(566, (char)0x36F3, 80, 80, new SdfUvRect(0.26725262f, 0.21614583f, 0.29329425f, 0.2421875f)),
            new SdfIcon(567, (char)0x37F3, 80, 80, new SdfUvRect(0.2939453f, 0.21614583f, 0.31998697f, 0.2421875f)),
            new SdfIcon(568, (char)0x38F3, 80, 80, new SdfUvRect(0.32063803f, 0.21614583f, 0.3466797f, 0.2421875f)),
            new SdfIcon(569, (char)0x39F3, 80, 80, new SdfUvRect(0.34733075f, 0.21614583f, 0.37337238f, 0.2421875f)),
            new SdfIcon(570, (char)0x3AF3, 82, 72, new SdfUvRect(0.37402344f, 0.21614583f, 0.40071613f, 0.23958334f)),
            new SdfIcon(571, (char)0x3BF3, 82, 72, new SdfUvRect(0.4013672f, 0.21614583f, 0.42805988f, 0.23958334f)),
            new SdfIcon(572, (char)0x3CF3, 26, 48, new SdfUvRect(0.01595052f, 0.87890625f, 0.024414064f, 0.89453125f)),
            new SdfIcon(573, (char)0x3DF3, 80, 80, new SdfUvRect(0.42871094f, 0.21614583f, 0.4547526f, 0.2421875f)),
            new SdfIcon(574, (char)0x3EF3, 80, 60, new SdfUvRect(0.7363281f, 0.1139323f, 0.7623698f, 0.13346355f)),
            new SdfIcon(575, (char)0x3FF3, 80, 68, new SdfUvRect(0.21386719f, 0.9742838f, 0.23990886f, 0.9964193f)),
            new SdfIcon(576, (char)0x40F3, 80, 68, new SdfUvRect(0.45540366f, 0.21614583f, 0.4814453f, 0.23828125f)),
            new SdfIcon(577, (char)0x41F3, 80, 60, new SdfUvRect(0.7630208f, 0.1139323f, 0.7890625f, 0.13346355f)),
            new SdfIcon(578, (char)0x42F3, 79, 79, new SdfUvRect(0.48209637f, 0.21614583f, 0.5078125f, 0.24186198f)),
            new SdfIcon(579, (char)0x43F3, 80, 42, new SdfUvRect(0.36914062f, 0.00032552084f, 0.39518228f, 0.013997396f)),
            new SdfIcon(580, (char)0x44F3, 80, 80, new SdfUvRect(0.5084635f, 0.21614583f, 0.53450525f, 0.2421875f)),
            new SdfIcon(581, (char)0x45F3, 66, 80, new SdfUvRect(0.53515625f, 0.21614583f, 0.5566406f, 0.2421875f)),
            new SdfIcon(582, (char)0x46F3, 66, 80, new SdfUvRect(0.5572916f, 0.21614583f, 0.57877606f, 0.2421875f)),
            new SdfIcon(583, (char)0x47F3, 66, 80, new SdfUvRect(0.57942706f, 0.21614583f, 0.6009115f, 0.2421875f)),
            new SdfIcon(584, (char)0x48F3, 66, 80, new SdfUvRect(0.6015625f, 0.21614583f, 0.6230469f, 0.2421875f)),
            new SdfIcon(585, (char)0x49F3, 66, 80, new SdfUvRect(0.6236979f, 0.21614583f, 0.6451823f, 0.2421875f)),
            new SdfIcon(586, (char)0x4AF3, 66, 80, new SdfUvRect(0.6458333f, 0.21614583f, 0.66731775f, 0.2421875f)),
            new SdfIcon(587, (char)0x4BF3, 66, 80, new SdfUvRect(0.66796875f, 0.21614583f, 0.6894531f, 0.2421875f)),
            new SdfIcon(588, (char)0x4CF3, 66, 80, new SdfUvRect(0.6901041f, 0.21614583f, 0.71158856f, 0.2421875f)),
            new SdfIcon(589, (char)0x4DF3, 80, 80, new SdfUvRect(0.71223956f, 0.21614583f, 0.73828125f, 0.2421875f)),
            new SdfIcon(590, (char)0x4EF3, 80, 80, new SdfUvRect(0.73893225f, 0.21614583f, 0.764974f, 0.2421875f)),
            new SdfIcon(591, (char)0x4FF3, 66, 80, new SdfUvRect(0.765625f, 0.21614583f, 0.7871094f, 0.2421875f)),
            new SdfIcon(592, (char)0x50F3, 66, 80, new SdfUvRect(0.7877604f, 0.21614583f, 0.8092448f, 0.2421875f)),
            new SdfIcon(593, (char)0x51F3, 66, 80, new SdfUvRect(0.8098958f, 0.21614583f, 0.83138025f, 0.2421875f)),
            new SdfIcon(594, (char)0x52F3, 66, 80, new SdfUvRect(0.83203125f, 0.21614583f, 0.8535156f, 0.2421875f)),
            new SdfIcon(595, (char)0x53F3, 66, 80, new SdfUvRect(0.8541666f, 0.21614583f, 0.87565106f, 0.2421875f)),
            new SdfIcon(596, (char)0x54F3, 66, 80, new SdfUvRect(0.87630206f, 0.21614583f, 0.8977865f, 0.2421875f)),
            new SdfIcon(597, (char)0x55F3, 66, 80, new SdfUvRect(0.8984375f, 0.21614583f, 0.9199219f, 0.2421875f)),
            new SdfIcon(598, (char)0x56F3, 66, 80, new SdfUvRect(0.9205729f, 0.21614583f, 0.9420573f, 0.2421875f)),
            new SdfIcon(599, (char)0x57F3, 66, 80, new SdfUvRect(0.9427083f, 0.21614583f, 0.96419275f, 0.2421875f)),
            new SdfIcon(600, (char)0x58F3, 66, 80, new SdfUvRect(0.96484375f, 0.21614583f, 0.9863281f, 0.2421875f)),
            new SdfIcon(601, (char)0x59F3, 66, 80, new SdfUvRect(0.24055989f, 0.24283853f, 0.26204425f, 0.2688802f)),
            new SdfIcon(602, (char)0x5AF3, 66, 80, new SdfUvRect(0.24055989f, 0.26953125f, 0.26204425f, 0.2955729f)),
            new SdfIcon(603, (char)0x5BF3, 66, 80, new SdfUvRect(0.24055989f, 0.29622397f, 0.26204425f, 0.32226562f)),
            new SdfIcon(604, (char)0x5CF3, 66, 80, new SdfUvRect(0.24055989f, 0.3229167f, 0.26204425f, 0.3489583f)),
            new SdfIcon(605, (char)0x5DF3, 80, 80, new SdfUvRect(0.2626953f, 0.24283853f, 0.28873697f, 0.2688802f)),
            new SdfIcon(606, (char)0x5EF3, 80, 80, new SdfUvRect(0.28938803f, 0.24283853f, 0.3154297f, 0.2688802f)),
            new SdfIcon(607, (char)0x5FF3, 66, 80, new SdfUvRect(0.24055989f, 0.34960938f, 0.26204425f, 0.37565103f)),
            new SdfIcon(608, (char)0x60F3, 66, 80, new SdfUvRect(0.24055989f, 0.3763021f, 0.26204425f, 0.40234375f)),
            new SdfIcon(609, (char)0x61F3, 66, 80, new SdfUvRect(0.24055989f, 0.4029948f, 0.26204425f, 0.42903644f)),
            new SdfIcon(610, (char)0x62F3, 66, 80, new SdfUvRect(0.24055989f, 0.4296875f, 0.26204425f, 0.45572916f)),
            new SdfIcon(611, (char)0x63F3, 66, 80, new SdfUvRect(0.24055989f, 0.45638022f, 0.26204425f, 0.48242188f)),
            new SdfIcon(612, (char)0x64F3, 66, 80, new SdfUvRect(0.24055989f, 0.48307294f, 0.26204425f, 0.5091146f)),
            new SdfIcon(613, (char)0x65F3, 66, 80, new SdfUvRect(0.24055989f, 0.5097656f, 0.26204425f, 0.5358073f)),
            new SdfIcon(614, (char)0x66F3, 66, 80, new SdfUvRect(0.24055989f, 0.5364583f, 0.26204425f, 0.5625f)),
            new SdfIcon(615, (char)0x67F3, 66, 80, new SdfUvRect(0.24055989f, 0.563151f, 0.26204425f, 0.58919275f)),
            new SdfIcon(616, (char)0x68F3, 66, 80, new SdfUvRect(0.24055989f, 0.58984375f, 0.26204425f, 0.61588544f)),
            new SdfIcon(617, (char)0x69F3, 66, 80, new SdfUvRect(0.24055989f, 0.61653644f, 0.26204425f, 0.6425781f)),
            new SdfIcon(618, (char)0x6AF3, 66, 80, new SdfUvRect(0.24055989f, 0.6432291f, 0.26204425f, 0.6692709f)),
            new SdfIcon(619, (char)0x6BF3, 66, 80, new SdfUvRect(0.24055989f, 0.6699219f, 0.26204425f, 0.69596356f)),
            new SdfIcon(620, (char)0x6CF3, 66, 80, new SdfUvRect(0.24055989f, 0.69661456f, 0.26204425f, 0.72265625f)),
            new SdfIcon(621, (char)0x6DF3, 66, 80, new SdfUvRect(0.24055989f, 0.72330725f, 0.26204425f, 0.749349f)),
            new SdfIcon(622, (char)0x6EF3, 66, 80, new SdfUvRect(0.24055989f, 0.75f, 0.26204425f, 0.7760417f)),
            new SdfIcon(623, (char)0x6FF3, 66, 80, new SdfUvRect(0.24055989f, 0.7766927f, 0.26204425f, 0.8027344f)),
            new SdfIcon(624, (char)0x70F3, 66, 80, new SdfUvRect(0.24055989f, 0.8033854f, 0.26204425f, 0.8294271f)),
            new SdfIcon(625, (char)0x71F3, 66, 80, new SdfUvRect(0.24055989f, 0.8300781f, 0.26204425f, 0.8561198f)),
            new SdfIcon(626, (char)0x72F3, 66, 80, new SdfUvRect(0.24055989f, 0.8567708f, 0.26204425f, 0.8828125f)),
            new SdfIcon(627, (char)0x73F3, 66, 80, new SdfUvRect(0.24055989f, 0.8834635f, 0.26204425f, 0.90950525f)),
            new SdfIcon(628, (char)0x74F3, 66, 80, new SdfUvRect(0.24055989f, 0.91015625f, 0.26204425f, 0.93619794f)),
            new SdfIcon(629, (char)0x75F3, 66, 80, new SdfUvRect(0.24055989f, 0.93684894f, 0.26204425f, 0.9628906f)),
            new SdfIcon(630, (char)0x76F3, 66, 80, new SdfUvRect(0.24055989f, 0.9635416f, 0.26204425f, 0.9895834f)),
            new SdfIcon(631, (char)0x77F3, 66, 80, new SdfUvRect(0.31608075f, 0.24283853f, 0.3375651f, 0.2688802f)),
            new SdfIcon(632, (char)0x78F3, 66, 80, new SdfUvRect(0.33821616f, 0.24283853f, 0.3597005f, 0.2688802f)),
            new SdfIcon(633, (char)0x79F3, 66, 80, new SdfUvRect(0.36035156f, 0.24283853f, 0.38183594f, 0.2688802f)),
            new SdfIcon(634, (char)0x7AF3, 66, 80, new SdfUvRect(0.382487f, 0.24283853f, 0.40397134f, 0.2688802f)),
            new SdfIcon(635, (char)0x7BF3, 66, 80, new SdfUvRect(0.4046224f, 0.24283853f, 0.42610675f, 0.2688802f)),
            new SdfIcon(636, (char)0x7CF3, 66, 80, new SdfUvRect(0.4267578f, 0.24283853f, 0.4482422f, 0.2688802f)),
            new SdfIcon(637, (char)0x7DF3, 66, 80, new SdfUvRect(0.44889325f, 0.24283853f, 0.4703776f, 0.2688802f)),
            new SdfIcon(638, (char)0x7EF3, 66, 80, new SdfUvRect(0.47102866f, 0.24283853f, 0.492513f, 0.2688802f)),
            new SdfIcon(639, (char)0x7FF3, 66, 80, new SdfUvRect(0.49316406f, 0.24283853f, 0.51464844f, 0.2688802f)),
            new SdfIcon(640, (char)0x80F3, 66, 80, new SdfUvRect(0.51529944f, 0.24283853f, 0.5367839f, 0.2688802f)),
            new SdfIcon(641, (char)0x81F3, 66, 80, new SdfUvRect(0.5374349f, 0.24283853f, 0.5589193f, 0.2688802f)),
            new SdfIcon(642, (char)0x82F3, 66, 80, new SdfUvRect(0.5595703f, 0.24283853f, 0.5810547f, 0.2688802f)),
            new SdfIcon(643, (char)0x83F3, 66, 80, new SdfUvRect(0.5817057f, 0.24283853f, 0.6031901f, 0.2688802f)),
            new SdfIcon(644, (char)0x84F3, 66, 80, new SdfUvRect(0.6038411f, 0.24283853f, 0.62532556f, 0.2688802f)),
            new SdfIcon(645, (char)0x85F3, 66, 80, new SdfUvRect(0.62597656f, 0.24283853f, 0.64746094f, 0.2688802f)),
            new SdfIcon(646, (char)0x86F3, 66, 80, new SdfUvRect(0.64811194f, 0.24283853f, 0.6695964f, 0.2688802f)),
            new SdfIcon(647, (char)0x87F3, 66, 80, new SdfUvRect(0.6702474f, 0.24283853f, 0.6917318f, 0.2688802f)),
            new SdfIcon(648, (char)0x88F3, 66, 80, new SdfUvRect(0.6923828f, 0.24283853f, 0.7138672f, 0.2688802f)),
            new SdfIcon(649, (char)0x89F3, 66, 80, new SdfUvRect(0.7145182f, 0.24283853f, 0.7360026f, 0.2688802f)),
            new SdfIcon(650, (char)0x8AF3, 66, 80, new SdfUvRect(0.7366536f, 0.24283853f, 0.75813806f, 0.2688802f)),
            new SdfIcon(651, (char)0x8BF3, 66, 80, new SdfUvRect(0.75878906f, 0.24283853f, 0.78027344f, 0.2688802f)),
            new SdfIcon(652, (char)0x8CF3, 66, 80, new SdfUvRect(0.78092444f, 0.24283853f, 0.8024089f, 0.2688802f)),
            new SdfIcon(653, (char)0x8DF3, 66, 80, new SdfUvRect(0.8030599f, 0.24283853f, 0.8245443f, 0.2688802f)),
            new SdfIcon(654, (char)0x8EF3, 66, 80, new SdfUvRect(0.8251953f, 0.24283853f, 0.8466797f, 0.2688802f)),
            new SdfIcon(655, (char)0x8FF3, 66, 80, new SdfUvRect(0.8473307f, 0.24283853f, 0.8688151f, 0.2688802f)),
            new SdfIcon(656, (char)0x90F3, 66, 80, new SdfUvRect(0.8694661f, 0.24283853f, 0.89095056f, 0.2688802f)),
            new SdfIcon(657, (char)0x91F3, 66, 80, new SdfUvRect(0.89160156f, 0.24283853f, 0.91308594f, 0.2688802f)),
            new SdfIcon(658, (char)0x92F3, 66, 80, new SdfUvRect(0.91373694f, 0.24283853f, 0.9352214f, 0.2688802f)),
            new SdfIcon(659, (char)0x93F3, 66, 80, new SdfUvRect(0.9358724f, 0.24283853f, 0.9573568f, 0.2688802f)),
            new SdfIcon(660, (char)0x94F3, 66, 80, new SdfUvRect(0.9580078f, 0.24283853f, 0.9794922f, 0.2688802f)),
            new SdfIcon(661, (char)0x95F3, 66, 80, new SdfUvRect(0.2626953f, 0.26953125f, 0.2841797f, 0.2955729f)),
            new SdfIcon(662, (char)0x96F3, 66, 80, new SdfUvRect(0.2626953f, 0.29622397f, 0.2841797f, 0.32226562f)),
            new SdfIcon(663, (char)0x97F3, 66, 80, new SdfUvRect(0.2626953f, 0.3229167f, 0.2841797f, 0.3489583f)),
            new SdfIcon(664, (char)0x98F3, 66, 80, new SdfUvRect(0.2626953f, 0.34960938f, 0.2841797f, 0.37565103f)),
            new SdfIcon(665, (char)0x99F3, 66, 80, new SdfUvRect(0.2626953f, 0.3763021f, 0.2841797f, 0.40234375f)),
            new SdfIcon(666, (char)0x9AF3, 66, 80, new SdfUvRect(0.2626953f, 0.4029948f, 0.2841797f, 0.42903644f)),
            new SdfIcon(667, (char)0x9BF3, 66, 80, new SdfUvRect(0.2626953f, 0.4296875f, 0.2841797f, 0.45572916f)),
            new SdfIcon(668, (char)0x9CF3, 66, 80, new SdfUvRect(0.2626953f, 0.45638022f, 0.2841797f, 0.48242188f)),
            new SdfIcon(669, (char)0x9DF3, 66, 80, new SdfUvRect(0.2626953f, 0.48307294f, 0.2841797f, 0.5091146f)),
            new SdfIcon(670, (char)0x9EF3, 66, 80, new SdfUvRect(0.2626953f, 0.5097656f, 0.2841797f, 0.5358073f)),
            new SdfIcon(671, (char)0x9FF3, 66, 80, new SdfUvRect(0.2626953f, 0.5364583f, 0.2841797f, 0.5625f)),
            new SdfIcon(672, (char)0xA0F3, 66, 80, new SdfUvRect(0.2626953f, 0.563151f, 0.2841797f, 0.58919275f)),
            new SdfIcon(673, (char)0xA1F3, 66, 80, new SdfUvRect(0.2626953f, 0.58984375f, 0.2841797f, 0.61588544f)),
            new SdfIcon(674, (char)0xA2F3, 66, 80, new SdfUvRect(0.2626953f, 0.61653644f, 0.2841797f, 0.6425781f)),
            new SdfIcon(675, (char)0xA3F3, 66, 80, new SdfUvRect(0.2626953f, 0.6432291f, 0.2841797f, 0.6692709f)),
            new SdfIcon(676, (char)0xA4F3, 66, 80, new SdfUvRect(0.2626953f, 0.6699219f, 0.2841797f, 0.69596356f)),
            new SdfIcon(677, (char)0xA5F3, 66, 80, new SdfUvRect(0.2626953f, 0.69661456f, 0.2841797f, 0.72265625f)),
            new SdfIcon(678, (char)0xA6F3, 66, 80, new SdfUvRect(0.2626953f, 0.72330725f, 0.2841797f, 0.749349f)),
            new SdfIcon(679, (char)0xA7F3, 66, 80, new SdfUvRect(0.2626953f, 0.75f, 0.2841797f, 0.7760417f)),
            new SdfIcon(680, (char)0xA8F3, 66, 80, new SdfUvRect(0.2626953f, 0.7766927f, 0.2841797f, 0.8027344f)),
            new SdfIcon(681, (char)0xA9F3, 66, 80, new SdfUvRect(0.2626953f, 0.8033854f, 0.2841797f, 0.8294271f)),
            new SdfIcon(682, (char)0xAAF3, 66, 80, new SdfUvRect(0.2626953f, 0.8300781f, 0.2841797f, 0.8561198f)),
            new SdfIcon(683, (char)0xABF3, 66, 80, new SdfUvRect(0.2626953f, 0.8567708f, 0.2841797f, 0.8828125f)),
            new SdfIcon(684, (char)0xACF3, 66, 80, new SdfUvRect(0.2626953f, 0.8834635f, 0.2841797f, 0.90950525f)),
            new SdfIcon(685, (char)0xADF3, 66, 80, new SdfUvRect(0.2626953f, 0.91015625f, 0.2841797f, 0.93619794f)),
            new SdfIcon(686, (char)0xAEF3, 66, 80, new SdfUvRect(0.2626953f, 0.93684894f, 0.2841797f, 0.9628906f)),
            new SdfIcon(687, (char)0xAFF3, 66, 80, new SdfUvRect(0.2626953f, 0.9635416f, 0.2841797f, 0.9895834f)),
            new SdfIcon(688, (char)0xB0F3, 66, 80, new SdfUvRect(0.28483075f, 0.26953125f, 0.3063151f, 0.2955729f)),
            new SdfIcon(689, (char)0xB1F3, 66, 80, new SdfUvRect(0.30696616f, 0.26953125f, 0.3284505f, 0.2955729f)),
            new SdfIcon(690, (char)0xB2F3, 66, 80, new SdfUvRect(0.32910156f, 0.26953125f, 0.35058594f, 0.2955729f)),
            new SdfIcon(691, (char)0xB3F3, 66, 80, new SdfUvRect(0.351237f, 0.26953125f, 0.37272134f, 0.2955729f)),
            new SdfIcon(692, (char)0xB4F3, 66, 80, new SdfUvRect(0.3733724f, 0.26953125f, 0.39485675f, 0.2955729f)),
            new SdfIcon(693, (char)0xB5F3, 66, 80, new SdfUvRect(0.3955078f, 0.26953125f, 0.4169922f, 0.2955729f)),
            new SdfIcon(694, (char)0xB6F3, 66, 80, new SdfUvRect(0.41764325f, 0.26953125f, 0.4391276f, 0.2955729f)),
            new SdfIcon(695, (char)0xB7F3, 66, 80, new SdfUvRect(0.43977866f, 0.26953125f, 0.461263f, 0.2955729f)),
            new SdfIcon(696, (char)0xB8F3, 66, 80, new SdfUvRect(0.46191406f, 0.26953125f, 0.48339844f, 0.2955729f)),
            new SdfIcon(697, (char)0xB9F3, 66, 80, new SdfUvRect(0.4840495f, 0.26953125f, 0.5055339f, 0.2955729f)),
            new SdfIcon(698, (char)0xBAF3, 66, 80, new SdfUvRect(0.5061849f, 0.26953125f, 0.5276693f, 0.2955729f)),
            new SdfIcon(699, (char)0xBBF3, 66, 80, new SdfUvRect(0.5283203f, 0.26953125f, 0.5498047f, 0.2955729f)),
            new SdfIcon(700, (char)0xBCF3, 66, 80, new SdfUvRect(0.5504557f, 0.26953125f, 0.5719401f, 0.2955729f)),
            new SdfIcon(701, (char)0xBDF3, 66, 80, new SdfUvRect(0.5725911f, 0.26953125f, 0.59407556f, 0.2955729f)),
            new SdfIcon(702, (char)0xBEF3, 66, 80, new SdfUvRect(0.59472656f, 0.26953125f, 0.61621094f, 0.2955729f)),
            new SdfIcon(703, (char)0xBFF3, 66, 80, new SdfUvRect(0.61686194f, 0.26953125f, 0.6383464f, 0.2955729f)),
            new SdfIcon(704, (char)0xC0F3, 66, 80, new SdfUvRect(0.6389974f, 0.26953125f, 0.6604818f, 0.2955729f)),
            new SdfIcon(705, (char)0xC1F3, 72, 80, new SdfUvRect(0.6611328f, 0.26953125f, 0.6845703f, 0.2955729f)),
            new SdfIcon(706, (char)0xC2F3, 69, 80, new SdfUvRect(0.6852213f, 0.26953125f, 0.7076823f, 0.2955729f)),
            new SdfIcon(707, (char)0xC3F3, 80, 80, new SdfUvRect(0.7083333f, 0.26953125f, 0.734375f, 0.2955729f)),
            new SdfIcon(708, (char)0xC4F3, 80, 80, new SdfUvRect(0.735026f, 0.26953125f, 0.76106775f, 0.2955729f)),
            new SdfIcon(709, (char)0xC5F3, 80, 80, new SdfUvRect(0.76171875f, 0.26953125f, 0.78776044f, 0.2955729f)),
            new SdfIcon(710, (char)0xC6F3, 65, 44, new SdfUvRect(0.39583334f, 0.00032552084f, 0.4169922f, 0.0146484375f)),
            new SdfIcon(711, (char)0xC7F3, 65, 44, new SdfUvRect(0.41764325f, 0.00032552084f, 0.43880206f, 0.0146484375f)),
            new SdfIcon(712, (char)0xC8F3, 80, 80, new SdfUvRect(0.78841144f, 0.26953125f, 0.8144531f, 0.2955729f)),
            new SdfIcon(713, (char)0xC9F3, 80, 80, new SdfUvRect(0.8151041f, 0.26953125f, 0.8411459f, 0.2955729f)),
            new SdfIcon(714, (char)0xCAF3, 65, 44, new SdfUvRect(0.43945312f, 0.00032552084f, 0.46061197f, 0.0146484375f)),
            new SdfIcon(715, (char)0xCBF3, 68, 80, new SdfUvRect(0.8417969f, 0.26953125f, 0.8639323f, 0.2955729f)),
            new SdfIcon(716, (char)0xCCF3, 68, 80, new SdfUvRect(0.8645833f, 0.26953125f, 0.88671875f, 0.2955729f)),
            new SdfIcon(717, (char)0xCDF3, 80, 80, new SdfUvRect(0.88736975f, 0.26953125f, 0.9134115f, 0.2955729f)),
            new SdfIcon(718, (char)0xCEF3, 80, 80, new SdfUvRect(0.9140625f, 0.26953125f, 0.9401042f, 0.2955729f)),
            new SdfIcon(719, (char)0xCFF3, 62, 66, new SdfUvRect(0.9407552f, 0.26953125f, 0.9609375f, 0.29101562f)),
            new SdfIcon(720, (char)0xD0F3, 80, 69, new SdfUvRect(0.9615885f, 0.26953125f, 0.98763025f, 0.2919922f)),
            new SdfIcon(721, (char)0xD1F3, 80, 68, new SdfUvRect(0.28483075f, 0.29622397f, 0.31087238f, 0.31835938f)),
            new SdfIcon(722, (char)0xD2F3, 80, 68, new SdfUvRect(0.28483075f, 0.31901044f, 0.31087238f, 0.3411458f)),
            new SdfIcon(723, (char)0xD3F3, 80, 72, new SdfUvRect(0.28483075f, 0.34179688f, 0.31087238f, 0.36523438f)),
            new SdfIcon(724, (char)0xD4F3, 80, 68, new SdfUvRect(0.28483075f, 0.36588544f, 0.31087238f, 0.3880208f)),
            new SdfIcon(725, (char)0xD5F3, 80, 68, new SdfUvRect(0.28483075f, 0.38867188f, 0.31087238f, 0.41080728f)),
            new SdfIcon(726, (char)0xD6F3, 80, 69, new SdfUvRect(0.28483075f, 0.41145834f, 0.31087238f, 0.43391925f)),
            new SdfIcon(727, (char)0xD7F3, 80, 68, new SdfUvRect(0.28483075f, 0.4345703f, 0.31087238f, 0.45670572f)),
            new SdfIcon(728, (char)0xD8F3, 80, 69, new SdfUvRect(0.28483075f, 0.45735678f, 0.31087238f, 0.4798177f)),
            new SdfIcon(729, (char)0xD9F3, 72, 65, new SdfUvRect(0.28483075f, 0.48046875f, 0.30826822f, 0.5016276f)),
            new SdfIcon(730, (char)0xDAF3, 52, 57, new SdfUvRect(0.7897135f, 0.1139323f, 0.8066406f, 0.13248698f)),
            new SdfIcon(731, (char)0xDBF3, 65, 47, new SdfUvRect(0.25423178f, 0.06933594f, 0.27539062f, 0.084635414f)),
            new SdfIcon(732, (char)0xDCF3, 70, 50, new SdfUvRect(0.2760417f, 0.06933594f, 0.29882812f, 0.08561198f)),
            new SdfIcon(733, (char)0xDDF3, 80, 80, new SdfUvRect(0.28483075f, 0.5022786f, 0.31087238f, 0.5283203f)),
            new SdfIcon(734, (char)0xDEF3, 80, 80, new SdfUvRect(0.28483075f, 0.5289713f, 0.31087238f, 0.55501306f)),
            new SdfIcon(735, (char)0xDFF3, 80, 80, new SdfUvRect(0.28483075f, 0.55566406f, 0.31087238f, 0.58170575f)),
            new SdfIcon(736, (char)0xE0F3, 70, 73, new SdfUvRect(0.28483075f, 0.58235675f, 0.3076172f, 0.6061198f)),
            new SdfIcon(737, (char)0xE1F3, 70, 73, new SdfUvRect(0.28483075f, 0.6067708f, 0.3076172f, 0.6305339f)),
            new SdfIcon(738, (char)0xE2F3, 82, 82, new SdfUvRect(0.31152344f, 0.29622397f, 0.33821613f, 0.32291666f)),
            new SdfIcon(739, (char)0xE3F3, 82, 82, new SdfUvRect(0.3388672f, 0.29622397f, 0.36555988f, 0.32291666f)),
            new SdfIcon(740, (char)0xE4F3, 82, 82, new SdfUvRect(0.36621094f, 0.29622397f, 0.39290363f, 0.32291666f)),
            new SdfIcon(741, (char)0xE5F3, 82, 82, new SdfUvRect(0.3935547f, 0.29622397f, 0.42024738f, 0.32291666f)),
            new SdfIcon(742, (char)0xE6F3, 80, 78, new SdfUvRect(0.28483075f, 0.6311849f, 0.31087238f, 0.65657556f)),
            new SdfIcon(743, (char)0xE7F3, 66, 80, new SdfUvRect(0.28483075f, 0.65722656f, 0.3063151f, 0.68326825f)),
            new SdfIcon(744, (char)0xE8F3, 66, 80, new SdfUvRect(0.28483075f, 0.68391925f, 0.3063151f, 0.70996094f)),
            new SdfIcon(745, (char)0xE9F3, 58, 81, new SdfUvRect(0.28483075f, 0.71061194f, 0.30371094f, 0.7369792f)),
            new SdfIcon(746, (char)0xEAF3, 58, 81, new SdfUvRect(0.28483075f, 0.7376302f, 0.30371094f, 0.76399744f)),
            new SdfIcon(747, (char)0xEBF3, 80, 80, new SdfUvRect(0.28483075f, 0.76464844f, 0.31087238f, 0.7906901f)),
            new SdfIcon(748, (char)0xECF3, 80, 80, new SdfUvRect(0.28483075f, 0.7913411f, 0.31087238f, 0.8173828f)),
            new SdfIcon(749, (char)0xEDF3, 80, 79, new SdfUvRect(0.28483075f, 0.8180338f, 0.31087238f, 0.84375f)),
            new SdfIcon(750, (char)0xEEF3, 80, 80, new SdfUvRect(0.28483075f, 0.844401f, 0.31087238f, 0.87044275f)),
            new SdfIcon(751, (char)0xEFF3, 80, 80, new SdfUvRect(0.28483075f, 0.87109375f, 0.31087238f, 0.89713544f)),
            new SdfIcon(752, (char)0xF0F3, 79, 80, new SdfUvRect(0.28483075f, 0.89778644f, 0.31054688f, 0.9238281f)),
            new SdfIcon(753, (char)0xF1F3, 80, 80, new SdfUvRect(0.28483075f, 0.9244791f, 0.31087238f, 0.9505209f)),
            new SdfIcon(754, (char)0xF2F3, 80, 80, new SdfUvRect(0.28483075f, 0.9511719f, 0.31087238f, 0.97721356f)),
            new SdfIcon(755, (char)0xF3F3, 80, 80, new SdfUvRect(0.42089844f, 0.29622397f, 0.4469401f, 0.32226562f)),
            new SdfIcon(756, (char)0xF4F3, 80, 80, new SdfUvRect(0.44759116f, 0.29622397f, 0.4736328f, 0.32226562f)),
            new SdfIcon(757, (char)0xF5F3, 72, 53, new SdfUvRect(0.2994792f, 0.06933594f, 0.32291666f, 0.08658854f)),
            new SdfIcon(758, (char)0xF6F3, 72, 53, new SdfUvRect(0.32356772f, 0.06933594f, 0.3470052f, 0.08658854f)),
            new SdfIcon(759, (char)0xF7F3, 80, 62, new SdfUvRect(0.8072916f, 0.1139323f, 0.8333334f, 0.1341146f)),
            new SdfIcon(760, (char)0xF8F3, 72, 72, new SdfUvRect(0.47428387f, 0.29622397f, 0.49772134f, 0.31966144f)),
            new SdfIcon(761, (char)0xF9F3, 72, 72, new SdfUvRect(0.4983724f, 0.29622397f, 0.52180994f, 0.31966144f)),
            new SdfIcon(762, (char)0xFAF3, 80, 80, new SdfUvRect(0.52246094f, 0.29622397f, 0.5485026f, 0.32226562f)),
            new SdfIcon(763, (char)0xFBF3, 72, 72, new SdfUvRect(0.5491536f, 0.29622397f, 0.5725912f, 0.31966144f)),
            new SdfIcon(764, (char)0xFCF3, 72, 72, new SdfUvRect(0.5732422f, 0.29622397f, 0.5966797f, 0.31966144f)),
            new SdfIcon(765, (char)0xFDF3, 72, 37, new SdfUvRect(0.46126303f, 0.00032552084f, 0.4847005f, 0.012369792f)),
            new SdfIcon(766, (char)0xFEF3, 37, 72, new SdfUvRect(0.9869791f, 0.21614583f, 0.99902344f, 0.23958334f)),
            new SdfIcon(767, (char)0xFFF3, 79, 72, new SdfUvRect(0.5973307f, 0.29622397f, 0.6230469f, 0.31966144f)),
            new SdfIcon(768, (char)0x00F4, 70, 80, new SdfUvRect(0.6236979f, 0.29622397f, 0.6464844f, 0.32226562f)),
            new SdfIcon(769, (char)0x01F4, 77, 80, new SdfUvRect(0.6471354f, 0.29622397f, 0.67220056f, 0.32226562f)),
            new SdfIcon(770, (char)0x02F4, 77, 80, new SdfUvRect(0.67285156f, 0.29622397f, 0.6979167f, 0.32226562f)),
            new SdfIcon(771, (char)0x03F4, 70, 80, new SdfUvRect(0.6985677f, 0.29622397f, 0.7213542f, 0.32226562f)),
            new SdfIcon(772, (char)0x04F4, 67, 80, new SdfUvRect(0.7220052f, 0.29622397f, 0.7438151f, 0.32226562f)),
            new SdfIcon(773, (char)0x05F4, 68, 80, new SdfUvRect(0.7444661f, 0.29622397f, 0.76660156f, 0.32226562f)),
            new SdfIcon(774, (char)0x06F4, 67, 80, new SdfUvRect(0.76725256f, 0.29622397f, 0.7890625f, 0.32226562f)),
            new SdfIcon(775, (char)0x07F4, 68, 80, new SdfUvRect(0.7897135f, 0.29622397f, 0.811849f, 0.32226562f)),
            new SdfIcon(776, (char)0x08F4, 76, 80, new SdfUvRect(0.8125f, 0.29622397f, 0.8372396f, 0.32226562f)),
            new SdfIcon(777, (char)0x09F4, 76, 80, new SdfUvRect(0.8378906f, 0.29622397f, 0.86263025f, 0.32226562f)),
            new SdfIcon(778, (char)0x0AF4, 51, 60, new SdfUvRect(0.8339844f, 0.1139323f, 0.85058594f, 0.13346355f)),
            new SdfIcon(779, (char)0x0BF4, 80, 58, new SdfUvRect(0.85123694f, 0.1139323f, 0.8772787f, 0.1328125f)),
            new SdfIcon(780, (char)0x0CF4, 80, 66, new SdfUvRect(0.28483075f, 0.97786456f, 0.31087238f, 0.999349f)),
            new SdfIcon(781, (char)0x0DF4, 80, 66, new SdfUvRect(0.86328125f, 0.29622397f, 0.88932294f, 0.3177083f)),
            new SdfIcon(782, (char)0x0EF4, 80, 66, new SdfUvRect(0.88997394f, 0.29622397f, 0.9160156f, 0.3177083f)),
            new SdfIcon(783, (char)0x0FF4, 80, 66, new SdfUvRect(0.9166666f, 0.29622397f, 0.9427084f, 0.3177083f)),
            new SdfIcon(784, (char)0x10F4, 80, 66, new SdfUvRect(0.9433594f, 0.29622397f, 0.96940106f, 0.3177083f)),
            new SdfIcon(785, (char)0x11F4, 80, 66, new SdfUvRect(0.97005206f, 0.29622397f, 0.99609375f, 0.3177083f)),
            new SdfIcon(786, (char)0x12F4, 80, 58, new SdfUvRect(0.8779297f, 0.1139323f, 0.9039714f, 0.1328125f)),
            new SdfIcon(787, (char)0x13F4, 65, 65, new SdfUvRect(0.31152344f, 0.32356772f, 0.33268228f, 0.34472656f)),
            new SdfIcon(788, (char)0x14F4, 65, 76, new SdfUvRect(0.31152344f, 0.34537762f, 0.33268228f, 0.3701172f)),
            new SdfIcon(789, (char)0x15F4, 82, 77, new SdfUvRect(0.33333334f, 0.32356772f, 0.36002603f, 0.3486328f)),
            new SdfIcon(790, (char)0x16F4, 82, 77, new SdfUvRect(0.3606771f, 0.32356772f, 0.38736978f, 0.3486328f)),
            new SdfIcon(791, (char)0x17F4, 82, 77, new SdfUvRect(0.38802084f, 0.32356772f, 0.41471353f, 0.3486328f)),
            new SdfIcon(792, (char)0x18F4, 82, 81, new SdfUvRect(0.33333334f, 0.34928387f, 0.36002603f, 0.37565103f)),
            new SdfIcon(793, (char)0x19F4, 82, 81, new SdfUvRect(0.33333334f, 0.3763021f, 0.36002603f, 0.40266925f)),
            new SdfIcon(794, (char)0x1AF4, 82, 81, new SdfUvRect(0.33333334f, 0.4033203f, 0.36002603f, 0.4296875f)),
            new SdfIcon(795, (char)0x1BF4, 72, 80, new SdfUvRect(0.33333334f, 0.43033856f, 0.3567708f, 0.4563802f)),
            new SdfIcon(796, (char)0x1CF4, 72, 80, new SdfUvRect(0.33333334f, 0.45703125f, 0.3567708f, 0.4830729f)),
            new SdfIcon(797, (char)0x1DF4, 72, 80, new SdfUvRect(0.33333334f, 0.48372397f, 0.3567708f, 0.5097656f)),
            new SdfIcon(798, (char)0x1EF4, 65, 72, new SdfUvRect(0.31152344f, 0.37076825f, 0.33268228f, 0.39420572f)),
            new SdfIcon(799, (char)0x1FF4, 65, 72, new SdfUvRect(0.31152344f, 0.39485678f, 0.33268228f, 0.41829425f)),
            new SdfIcon(800, (char)0x20F4, 65, 72, new SdfUvRect(0.31152344f, 0.4189453f, 0.33268228f, 0.4423828f)),
            new SdfIcon(801, (char)0x21F4, 65, 72, new SdfUvRect(0.31152344f, 0.44303387f, 0.33268228f, 0.46647134f)),
            new SdfIcon(802, (char)0x22F4, 70, 72, new SdfUvRect(0.4153646f, 0.32356772f, 0.43815103f, 0.3470052f)),
            new SdfIcon(803, (char)0x23F4, 70, 72, new SdfUvRect(0.4388021f, 0.32356772f, 0.46158853f, 0.3470052f)),
            new SdfIcon(804, (char)0x24F4, 78, 72, new SdfUvRect(0.4622396f, 0.32356772f, 0.4876302f, 0.3470052f)),
            new SdfIcon(805, (char)0x25F4, 78, 72, new SdfUvRect(0.48828125f, 0.32356772f, 0.5136719f, 0.3470052f)),
            new SdfIcon(806, (char)0x26F4, 80, 65, new SdfUvRect(0.5143229f, 0.32356772f, 0.5403646f, 0.34472656f)),
            new SdfIcon(807, (char)0x27F4, 80, 69, new SdfUvRect(0.5410156f, 0.32356772f, 0.5670573f, 0.34602863f)),
            new SdfIcon(808, (char)0x28F4, 80, 80, new SdfUvRect(0.33333334f, 0.5104166f, 0.359375f, 0.5364584f)),
            new SdfIcon(809, (char)0x29F4, 80, 72, new SdfUvRect(0.5677083f, 0.32356772f, 0.59375f, 0.3470052f)),
            new SdfIcon(810, (char)0x2AF4, 80, 72, new SdfUvRect(0.594401f, 0.32356772f, 0.62044275f, 0.3470052f)),
            new SdfIcon(811, (char)0x2BF4, 80, 72, new SdfUvRect(0.62109375f, 0.32356772f, 0.64713544f, 0.3470052f)),
            new SdfIcon(812, (char)0x2CF4, 80, 58, new SdfUvRect(0.9046224f, 0.1139323f, 0.93066406f, 0.1328125f)),
            new SdfIcon(813, (char)0x2DF4, 80, 58, new SdfUvRect(0.93131506f, 0.1139323f, 0.9573568f, 0.1328125f)),
            new SdfIcon(814, (char)0x2EF4, 80, 80, new SdfUvRect(0.33333334f, 0.5371094f, 0.359375f, 0.56315106f)),
            new SdfIcon(815, (char)0x2FF4, 80, 80, new SdfUvRect(0.33333334f, 0.56380206f, 0.359375f, 0.58984375f)),
            new SdfIcon(816, (char)0x30F4, 80, 80, new SdfUvRect(0.33333334f, 0.59049475f, 0.359375f, 0.6165365f)),
            new SdfIcon(817, (char)0x31F4, 80, 80, new SdfUvRect(0.33333334f, 0.6171875f, 0.359375f, 0.6432292f)),
            new SdfIcon(818, (char)0x32F4, 80, 80, new SdfUvRect(0.33333334f, 0.6438802f, 0.359375f, 0.6699219f)),
            new SdfIcon(819, (char)0x33F4, 80, 80, new SdfUvRect(0.33333334f, 0.6705729f, 0.359375f, 0.6966146f)),
            new SdfIcon(820, (char)0x34F4, 28, 53, new SdfUvRect(0.017903645f, 0.97265625f, 0.02701823f, 0.9899089f)),
            new SdfIcon(821, (char)0x35F4, 80, 70, new SdfUvRect(0.64778644f, 0.32356772f, 0.6738281f, 0.34635416f)),
            new SdfIcon(822, (char)0x36F4, 80, 72, new SdfUvRect(0.6744791f, 0.32356772f, 0.7005209f, 0.3470052f)),
            new SdfIcon(823, (char)0x37F4, 80, 80, new SdfUvRect(0.33333334f, 0.6972656f, 0.359375f, 0.7233073f)),
            new SdfIcon(824, (char)0x38F4, 80, 80, new SdfUvRect(0.33333334f, 0.7239583f, 0.359375f, 0.75f)),
            new SdfIcon(825, (char)0x39F4, 76, 80, new SdfUvRect(0.33333334f, 0.750651f, 0.3580729f, 0.77669275f)),
            new SdfIcon(826, (char)0x3AF4, 76, 80, new SdfUvRect(0.33333334f, 0.77734375f, 0.3580729f, 0.80338544f)),
            new SdfIcon(827, (char)0x3BF4, 76, 80, new SdfUvRect(0.33333334f, 0.80403644f, 0.3580729f, 0.8300781f)),
            new SdfIcon(828, (char)0x3CF4, 76, 80, new SdfUvRect(0.33333334f, 0.8307291f, 0.3580729f, 0.8567709f)),
            new SdfIcon(829, (char)0x3DF4, 76, 80, new SdfUvRect(0.33333334f, 0.8574219f, 0.3580729f, 0.88346356f)),
            new SdfIcon(830, (char)0x3EF4, 76, 80, new SdfUvRect(0.33333334f, 0.88411456f, 0.3580729f, 0.91015625f)),
            new SdfIcon(831, (char)0x3FF4, 76, 80, new SdfUvRect(0.33333334f, 0.91080725f, 0.3580729f, 0.936849f)),
            new SdfIcon(832, (char)0x40F4, 76, 80, new SdfUvRect(0.33333334f, 0.9375f, 0.3580729f, 0.9635417f)),
            new SdfIcon(833, (char)0x41F4, 76, 80, new SdfUvRect(0.33333334f, 0.9641927f, 0.3580729f, 0.9902344f)),
            new SdfIcon(834, (char)0x42F4, 76, 80, new SdfUvRect(0.3606771f, 0.34928387f, 0.38541666f, 0.3753255f)),
            new SdfIcon(835, (char)0x43F4, 76, 80, new SdfUvRect(0.38606772f, 0.34928387f, 0.41080728f, 0.3753255f)),
            new SdfIcon(836, (char)0x44F4, 76, 80, new SdfUvRect(0.41145834f, 0.34928387f, 0.4361979f, 0.3753255f)),
            new SdfIcon(837, (char)0x45F4, 76, 80, new SdfUvRect(0.43684897f, 0.34928387f, 0.46158853f, 0.3753255f)),
            new SdfIcon(838, (char)0x46F4, 76, 80, new SdfUvRect(0.4622396f, 0.34928387f, 0.48697916f, 0.3753255f)),
            new SdfIcon(839, (char)0x47F4, 76, 80, new SdfUvRect(0.48763022f, 0.34928387f, 0.5123698f, 0.3753255f)),
            new SdfIcon(840, (char)0x48F4, 80, 81, new SdfUvRect(0.3606771f, 0.37597656f, 0.38671875f, 0.40234375f)),
            new SdfIcon(841, (char)0x49F4, 65, 58, new SdfUvRect(0.9580078f, 0.1139323f, 0.9791667f, 0.1328125f)),
            new SdfIcon(842, (char)0x4AF4, 65, 58, new SdfUvRect(0.31152344f, 0.4671224f, 0.33268228f, 0.4860026f)),
            new SdfIcon(843, (char)0x4BF4, 65, 58, new SdfUvRect(0.31152344f, 0.48665366f, 0.33268228f, 0.5055339f)),
            new SdfIcon(844, (char)0x4CF4, 78, 80, new SdfUvRect(0.5130208f, 0.34928387f, 0.5384115f, 0.3753255f)),
            new SdfIcon(845, (char)0x4DF4, 78, 80, new SdfUvRect(0.5390625f, 0.34928387f, 0.5644531f, 0.3753255f)),
            new SdfIcon(846, (char)0x4EF4, 80, 46, new SdfUvRect(0.48535156f, 0.00032552084f, 0.51139325f, 0.0152994795f)),
            new SdfIcon(847, (char)0x4FF4, 80, 50, new SdfUvRect(0.34765625f, 0.06933594f, 0.3736979f, 0.08561198f)),
            new SdfIcon(848, (char)0x50F4, 80, 53, new SdfUvRect(0.37434897f, 0.06933594f, 0.40039062f, 0.08658854f)),
            new SdfIcon(849, (char)0x51F4, 80, 53, new SdfUvRect(0.4010417f, 0.06933594f, 0.4270833f, 0.08658854f)),
            new SdfIcon(850, (char)0x52F4, 48, 76, new SdfUvRect(0.9801432f, 0.24283853f, 0.99576825f, 0.26757812f)),
            new SdfIcon(851, (char)0x53F4, 66, 80, new SdfUvRect(0.5651041f, 0.34928387f, 0.58658856f, 0.3753255f)),
            new SdfIcon(852, (char)0x54F4, 66, 80, new SdfUvRect(0.58723956f, 0.34928387f, 0.608724f, 0.3753255f)),
            new SdfIcon(853, (char)0x55F4, 80, 65, new SdfUvRect(0.7011719f, 0.32356772f, 0.72721356f, 0.34472656f)),
            new SdfIcon(854, (char)0x56F4, 80, 65, new SdfUvRect(0.72786456f, 0.32356772f, 0.75390625f, 0.34472656f)),
            new SdfIcon(855, (char)0x57F4, 80, 80, new SdfUvRect(0.609375f, 0.34928387f, 0.6354167f, 0.3753255f)),
            new SdfIcon(856, (char)0x58F4, 80, 80, new SdfUvRect(0.6360677f, 0.34928387f, 0.6621094f, 0.3753255f)),
            new SdfIcon(857, (char)0x59F4, 80, 70, new SdfUvRect(0.75455725f, 0.32356772f, 0.780599f, 0.34635416f)),
            new SdfIcon(858, (char)0x5AF4, 80, 70, new SdfUvRect(0.78125f, 0.32356772f, 0.8072917f, 0.34635416f)),
            new SdfIcon(859, (char)0x5BF4, 80, 70, new SdfUvRect(0.8079427f, 0.32356772f, 0.8339844f, 0.34635416f)),
            new SdfIcon(860, (char)0x5CF4, 80, 72, new SdfUvRect(0.8346354f, 0.32356772f, 0.8606771f, 0.3470052f)),
            new SdfIcon(861, (char)0x5DF4, 80, 72, new SdfUvRect(0.8613281f, 0.32356772f, 0.8873698f, 0.3470052f)),
            new SdfIcon(862, (char)0x5EF4, 80, 72, new SdfUvRect(0.8880208f, 0.32356772f, 0.9140625f, 0.3470052f)),
            new SdfIcon(863, (char)0x5FF4, 80, 72, new SdfUvRect(0.9147135f, 0.32356772f, 0.94075525f, 0.3470052f)),
            new SdfIcon(864, (char)0x60F4, 80, 72, new SdfUvRect(0.94140625f, 0.32356772f, 0.96744794f, 0.3470052f)),
            new SdfIcon(865, (char)0x61F4, 80, 80, new SdfUvRect(0.6627604f, 0.34928387f, 0.6888021f, 0.3753255f)),
            new SdfIcon(866, (char)0x62F4, 80, 80, new SdfUvRect(0.6894531f, 0.34928387f, 0.7154948f, 0.3753255f)),
            new SdfIcon(867, (char)0x63F4, 80, 80, new SdfUvRect(0.7161458f, 0.34928387f, 0.7421875f, 0.3753255f)),
            new SdfIcon(868, (char)0x64F4, 80, 80, new SdfUvRect(0.7428385f, 0.34928387f, 0.76888025f, 0.3753255f)),
            new SdfIcon(869, (char)0x65F4, 80, 80, new SdfUvRect(0.76953125f, 0.34928387f, 0.79557294f, 0.3753255f)),
            new SdfIcon(870, (char)0x66F4, 80, 80, new SdfUvRect(0.79622394f, 0.34928387f, 0.8222656f, 0.3753255f)),
            new SdfIcon(871, (char)0x67F4, 80, 80, new SdfUvRect(0.8229166f, 0.34928387f, 0.8489584f, 0.3753255f)),
            new SdfIcon(872, (char)0x68F4, 66, 82, new SdfUvRect(0.3606771f, 0.4029948f, 0.38216144f, 0.4296875f)),
            new SdfIcon(873, (char)0x69F4, 70, 81, new SdfUvRect(0.3606771f, 0.43033856f, 0.38346353f, 0.45670572f)),
            new SdfIcon(874, (char)0x6AF4, 70, 81, new SdfUvRect(0.3606771f, 0.45735678f, 0.38346353f, 0.48372394f)),
            new SdfIcon(875, (char)0x6BF4, 66, 82, new SdfUvRect(0.3606771f, 0.484375f, 0.38216144f, 0.51106775f)),
            new SdfIcon(876, (char)0x6CF4, 62, 80, new SdfUvRect(0.31152344f, 0.5061849f, 0.33170572f, 0.53222656f)),
            new SdfIcon(877, (char)0x6DF4, 62, 80, new SdfUvRect(0.31152344f, 0.53287756f, 0.33170572f, 0.5589193f)),
            new SdfIcon(878, (char)0x6EF4, 58, 81, new SdfUvRect(0.31152344f, 0.5595703f, 0.33040363f, 0.5859375f)),
            new SdfIcon(879, (char)0x6FF4, 58, 81, new SdfUvRect(0.31152344f, 0.5865885f, 0.33040363f, 0.61295575f)),
            new SdfIcon(880, (char)0x70F4, 62, 62, new SdfUvRect(0.31152344f, 0.61360675f, 0.33170572f, 0.63378906f)),
            new SdfIcon(881, (char)0x71F4, 72, 42, new SdfUvRect(0.51204425f, 0.00032552084f, 0.5354818f, 0.013997396f)),
            new SdfIcon(882, (char)0x72F4, 80, 80, new SdfUvRect(0.8496094f, 0.34928387f, 0.87565106f, 0.3753255f)),
            new SdfIcon(883, (char)0x73F4, 73, 59, new SdfUvRect(0.96809894f, 0.32356772f, 0.991862f, 0.34277344f)),
            new SdfIcon(884, (char)0x74F4, 78, 53, new SdfUvRect(0.42773438f, 0.06933594f, 0.453125f, 0.08658854f)),
            new SdfIcon(885, (char)0x75F4, 72, 60, new SdfUvRect(0.87630206f, 0.34928387f, 0.8997396f, 0.3688151f)),
            new SdfIcon(886, (char)0x76F4, 73, 61, new SdfUvRect(0.9003906f, 0.34928387f, 0.9241537f, 0.36914062f)),
            new SdfIcon(887, (char)0x77F4, 71, 56, new SdfUvRect(0.9248047f, 0.34928387f, 0.9479167f, 0.367513f)),
            new SdfIcon(888, (char)0x78F4, 72, 56, new SdfUvRect(0.9485677f, 0.34928387f, 0.97200525f, 0.367513f)),
            new SdfIcon(889, (char)0x79F4, 60, 52, new SdfUvRect(0.45377606f, 0.06933594f, 0.47330728f, 0.086263016f)),
            new SdfIcon(890, (char)0x7AF4, 58, 80, new SdfUvRect(0.31152344f, 0.63444006f, 0.33040363f, 0.6604818f)),
            new SdfIcon(891, (char)0x7BF4, 58, 80, new SdfUvRect(0.31152344f, 0.6611328f, 0.33040363f, 0.6871745f)),
            new SdfIcon(892, (char)0x7CF4, 80, 61, new SdfUvRect(0.97265625f, 0.34928387f, 0.99869794f, 0.36914062f)),
            new SdfIcon(893, (char)0x7DF4, 80, 62, new SdfUvRect(0.3606771f, 0.51171875f, 0.38671875f, 0.53190106f)),
            new SdfIcon(894, (char)0x7EF4, 80, 82, new SdfUvRect(0.3606771f, 0.53255206f, 0.38671875f, 0.5592448f)),
            new SdfIcon(895, (char)0x7FF4, 80, 82, new SdfUvRect(0.3606771f, 0.5598958f, 0.38671875f, 0.58658856f)),
            new SdfIcon(896, (char)0x80F4, 80, 65, new SdfUvRect(0.3606771f, 0.58723956f, 0.38671875f, 0.60839844f)),
            new SdfIcon(897, (char)0x81F4, 80, 66, new SdfUvRect(0.3606771f, 0.60904944f, 0.38671875f, 0.6305339f)),
            new SdfIcon(898, (char)0x82F4, 82, 82, new SdfUvRect(0.3873698f, 0.37597656f, 0.4140625f, 0.40266925f)),
            new SdfIcon(899, (char)0x83F4, 80, 73, new SdfUvRect(0.3606771f, 0.6311849f, 0.38671875f, 0.65494794f)),
            new SdfIcon(900, (char)0x84F4, 80, 73, new SdfUvRect(0.3606771f, 0.65559894f, 0.38671875f, 0.679362f)),
            new SdfIcon(901, (char)0x85F4, 80, 76, new SdfUvRect(0.3606771f, 0.680013f, 0.38671875f, 0.7047526f)),
            new SdfIcon(902, (char)0x86F4, 80, 76, new SdfUvRect(0.3606771f, 0.7054036f, 0.38671875f, 0.73014325f)),
            new SdfIcon(903, (char)0x87F4, 80, 76, new SdfUvRect(0.3606771f, 0.73079425f, 0.38671875f, 0.7555339f)),
            new SdfIcon(904, (char)0x88F4, 80, 76, new SdfUvRect(0.3606771f, 0.7561849f, 0.38671875f, 0.7809245f)),
            new SdfIcon(905, (char)0x89F4, 80, 76, new SdfUvRect(0.3606771f, 0.7815755f, 0.38671875f, 0.8063151f)),
            new SdfIcon(906, (char)0x8AF4, 80, 76, new SdfUvRect(0.3606771f, 0.8069661f, 0.38671875f, 0.83170575f)),
            new SdfIcon(907, (char)0x8BF4, 80, 76, new SdfUvRect(0.3606771f, 0.83235675f, 0.38671875f, 0.8570964f)),
            new SdfIcon(908, (char)0x8CF4, 80, 76, new SdfUvRect(0.3606771f, 0.8577474f, 0.38671875f, 0.882487f)),
            new SdfIcon(909, (char)0x8DF4, 58, 80, new SdfUvRect(0.31152344f, 0.6878255f, 0.33040363f, 0.7138672f)),
            new SdfIcon(910, (char)0x8EF4, 68, 81, new SdfUvRect(0.3606771f, 0.883138f, 0.3828125f, 0.90950525f)),
            new SdfIcon(911, (char)0x8FF4, 68, 81, new SdfUvRect(0.3606771f, 0.91015625f, 0.3828125f, 0.93652344f)),
            new SdfIcon(912, (char)0x90F4, 58, 80, new SdfUvRect(0.31152344f, 0.7145182f, 0.33040363f, 0.74055994f)),
            new SdfIcon(913, (char)0x91F4, 82, 80, new SdfUvRect(0.41471356f, 0.37597656f, 0.44140625f, 0.40201822f)),
            new SdfIcon(914, (char)0x92F4, 82, 69, new SdfUvRect(0.4420573f, 0.37597656f, 0.46875f, 0.3984375f)),
            new SdfIcon(915, (char)0x93F4, 72, 80, new SdfUvRect(0.3606771f, 0.93717444f, 0.38411456f, 0.9632162f)),
            new SdfIcon(916, (char)0x94F4, 76, 81, new SdfUvRect(0.3606771f, 0.9638672f, 0.38541666f, 0.9902344f)),
            new SdfIcon(917, (char)0x95F4, 80, 81, new SdfUvRect(0.46940106f, 0.37597656f, 0.4954427f, 0.40234375f)),
            new SdfIcon(918, (char)0x96F4, 80, 81, new SdfUvRect(0.49609375f, 0.37597656f, 0.52213544f, 0.40234375f)),
            new SdfIcon(919, (char)0x97F4, 76, 81, new SdfUvRect(0.52278644f, 0.37597656f, 0.54752606f, 0.40234375f)),
            new SdfIcon(920, (char)0x98F4, 58, 80, new SdfUvRect(0.31152344f, 0.74121094f, 0.33040363f, 0.7672526f)),
            new SdfIcon(921, (char)0x99F4, 58, 80, new SdfUvRect(0.31152344f, 0.7679036f, 0.33040363f, 0.7939453f)),
            new SdfIcon(922, (char)0x9AF4, 58, 80, new SdfUvRect(0.31152344f, 0.7945963f, 0.33040363f, 0.82063806f)),
            new SdfIcon(923, (char)0x9BF4, 58, 80, new SdfUvRect(0.31152344f, 0.82128906f, 0.33040363f, 0.84733075f)),
            new SdfIcon(924, (char)0x9CF4, 66, 80, new SdfUvRect(0.54817706f, 0.37597656f, 0.5696615f, 0.40201822f)),
            new SdfIcon(925, (char)0x9DF4, 66, 80, new SdfUvRect(0.5703125f, 0.37597656f, 0.5917969f, 0.40201822f)),
            new SdfIcon(926, (char)0x9EF4, 72, 72, new SdfUvRect(0.5924479f, 0.37597656f, 0.61588544f, 0.39941406f)),
            new SdfIcon(927, (char)0x9FF4, 80, 72, new SdfUvRect(0.61653644f, 0.37597656f, 0.6425781f, 0.39941406f)),
            new SdfIcon(928, (char)0xA0F4, 53, 72, new SdfUvRect(0.31152344f, 0.84798175f, 0.32877603f, 0.8714193f)),
            new SdfIcon(929, (char)0xA1F4, 66, 80, new SdfUvRect(0.6432291f, 0.37597656f, 0.66471356f, 0.40201822f)),
            new SdfIcon(930, (char)0xA2F4, 66, 80, new SdfUvRect(0.66536456f, 0.37597656f, 0.686849f, 0.40201822f)),
            new SdfIcon(931, (char)0xA3F4, 80, 72, new SdfUvRect(0.6875f, 0.37597656f, 0.7135417f, 0.39941406f)),
            new SdfIcon(932, (char)0xA4F4, 80, 58, new SdfUvRect(0.7141927f, 0.37597656f, 0.7402344f, 0.39485675f)),
            new SdfIcon(933, (char)0xA5F4, 80, 58, new SdfUvRect(0.7408854f, 0.37597656f, 0.7669271f, 0.39485675f)),
            new SdfIcon(934, (char)0xA6F4, 81, 58, new SdfUvRect(0.7675781f, 0.37597656f, 0.7939453f, 0.39485675f)),
            new SdfIcon(935, (char)0xA7F4, 80, 58, new SdfUvRect(0.7945963f, 0.37597656f, 0.82063806f, 0.39485675f)),
            new SdfIcon(936, (char)0xA8F4, 80, 72, new SdfUvRect(0.82128906f, 0.37597656f, 0.84733075f, 0.39941406f)),
            new SdfIcon(937, (char)0xA9F4, 80, 72, new SdfUvRect(0.84798175f, 0.37597656f, 0.87402344f, 0.39941406f)),
            new SdfIcon(938, (char)0xAAF4, 80, 80, new SdfUvRect(0.87467444f, 0.37597656f, 0.9007162f, 0.40201822f)),
            new SdfIcon(939, (char)0xABF4, 80, 80, new SdfUvRect(0.9013672f, 0.37597656f, 0.9274089f, 0.40201822f)),
            new SdfIcon(940, (char)0xACF4, 80, 80, new SdfUvRect(0.9280599f, 0.37597656f, 0.95410156f, 0.40201822f)),
            new SdfIcon(941, (char)0xADF4, 72, 66, new SdfUvRect(0.95475256f, 0.37597656f, 0.9781901f, 0.39746094f)),
            new SdfIcon(942, (char)0xAEF4, 70, 60, new SdfUvRect(0.3873698f, 0.4033203f, 0.41015625f, 0.42285156f)),
            new SdfIcon(943, (char)0xAFF4, 72, 72, new SdfUvRect(0.4108073f, 0.4033203f, 0.43424478f, 0.4267578f)),
            new SdfIcon(944, (char)0xB0F4, 80, 80, new SdfUvRect(0.4108073f, 0.42740887f, 0.43684894f, 0.4534505f)),
            new SdfIcon(945, (char)0xB1F4, 80, 80, new SdfUvRect(0.4108073f, 0.45410156f, 0.43684894f, 0.48014322f)),
            new SdfIcon(946, (char)0xB2F4, 80, 80, new SdfUvRect(0.4108073f, 0.48079428f, 0.43684894f, 0.50683594f)),
            new SdfIcon(947, (char)0xB3F4, 46, 78, new SdfUvRect(0.31152344f, 0.8720703f, 0.32649738f, 0.89746094f)),
            new SdfIcon(948, (char)0xB4F4, 58, 72, new SdfUvRect(0.31152344f, 0.89811194f, 0.33040363f, 0.9215495f)),
            new SdfIcon(949, (char)0xB5F4, 80, 80, new SdfUvRect(0.4108073f, 0.50748694f, 0.43684894f, 0.5335287f)),
            new SdfIcon(950, (char)0xB6F4, 80, 80, new SdfUvRect(0.4108073f, 0.5341797f, 0.43684894f, 0.5602214f)),
            new SdfIcon(951, (char)0xB7F4, 80, 80, new SdfUvRect(0.4108073f, 0.5608724f, 0.43684894f, 0.58691406f)),
            new SdfIcon(952, (char)0xB8F4, 80, 80, new SdfUvRect(0.4108073f, 0.58756506f, 0.43684894f, 0.6136068f)),
            new SdfIcon(953, (char)0xB9F4, 80, 80, new SdfUvRect(0.4108073f, 0.6142578f, 0.43684894f, 0.6402995f)),
            new SdfIcon(954, (char)0xBAF4, 80, 80, new SdfUvRect(0.4108073f, 0.6409505f, 0.43684894f, 0.6669922f)),
            new SdfIcon(955, (char)0xBBF4, 80, 80, new SdfUvRect(0.4108073f, 0.6676432f, 0.43684894f, 0.69368494f)),
            new SdfIcon(956, (char)0xBCF4, 80, 80, new SdfUvRect(0.4108073f, 0.69433594f, 0.43684894f, 0.7203776f)),
            new SdfIcon(957, (char)0xBDF4, 80, 80, new SdfUvRect(0.4108073f, 0.7210286f, 0.43684894f, 0.7470703f)),
            new SdfIcon(958, (char)0xBEF4, 80, 80, new SdfUvRect(0.4108073f, 0.7477213f, 0.43684894f, 0.77376306f)),
            new SdfIcon(959, (char)0xBFF4, 80, 65, new SdfUvRect(0.43489584f, 0.4033203f, 0.4609375f, 0.42447916f)),
            new SdfIcon(960, (char)0xC0F4, 80, 65, new SdfUvRect(0.46158856f, 0.4033203f, 0.4876302f, 0.42447916f)),
            new SdfIcon(961, (char)0xC1F4, 80, 80, new SdfUvRect(0.4108073f, 0.77441406f, 0.43684894f, 0.80045575f)),
            new SdfIcon(962, (char)0xC2F4, 80, 80, new SdfUvRect(0.4108073f, 0.80110675f, 0.43684894f, 0.82714844f)),
            new SdfIcon(963, (char)0xC3F4, 48, 52, new SdfUvRect(0.47395834f, 0.06933594f, 0.4895833f, 0.086263016f)),
            new SdfIcon(964, (char)0xC4F4, 38, 52, new SdfUvRect(0.49023438f, 0.06933594f, 0.5026042f, 0.086263016f)),
            new SdfIcon(965, (char)0xC5F4, 84, 80, new SdfUvRect(0.4375f, 0.42740887f, 0.46484375f, 0.4534505f)),
            new SdfIcon(966, (char)0xC6F4, 80, 80, new SdfUvRect(0.4108073f, 0.82779944f, 0.43684894f, 0.8538412f)),
            new SdfIcon(967, (char)0xC7F4, 80, 80, new SdfUvRect(0.4108073f, 0.8544922f, 0.43684894f, 0.8805339f)),
            new SdfIcon(968, (char)0xC8F4, 80, 80, new SdfUvRect(0.4108073f, 0.8811849f, 0.43684894f, 0.90722656f)),
            new SdfIcon(969, (char)0xC9F4, 81, 81, new SdfUvRect(0.4375f, 0.45410156f, 0.4638672f, 0.48046875f)),
            new SdfIcon(970, (char)0xCAF4, 75, 75, new SdfUvRect(0.4108073f, 0.90787756f, 0.43522134f, 0.9322917f)),
            new SdfIcon(971, (char)0xCBF4, 81, 81, new SdfUvRect(0.4375f, 0.4811198f, 0.4638672f, 0.507487f)),
            new SdfIcon(972, (char)0xCCF4, 80, 80, new SdfUvRect(0.4108073f, 0.9329427f, 0.43684894f, 0.9589844f)),
            new SdfIcon(973, (char)0xCDF4, 80, 80, new SdfUvRect(0.4108073f, 0.9596354f, 0.43684894f, 0.9856771f)),
            new SdfIcon(974, (char)0xCEF4, 80, 80, new SdfUvRect(0.4654948f, 0.42740887f, 0.49153644f, 0.4534505f)),
            new SdfIcon(975, (char)0xCFF4, 80, 65, new SdfUvRect(0.48828125f, 0.4033203f, 0.51432294f, 0.42447916f)),
            new SdfIcon(976, (char)0xD0F4, 80, 65, new SdfUvRect(0.51497394f, 0.4033203f, 0.5410156f, 0.42447916f)),
            new SdfIcon(977, (char)0xD1F4, 65, 65, new SdfUvRect(0.31152344f, 0.9222005f, 0.33268228f, 0.9433594f)),
            new SdfIcon(978, (char)0xD2F4, 66, 80, new SdfUvRect(0.3873698f, 0.42350262f, 0.40885416f, 0.44954425f)),
            new SdfIcon(979, (char)0xD3F4, 65, 80, new SdfUvRect(0.31152344f, 0.9440104f, 0.33268228f, 0.9700521f)),
            new SdfIcon(980, (char)0xD4F4, 80, 80, new SdfUvRect(0.4921875f, 0.42740887f, 0.5182292f, 0.4534505f)),
            new SdfIcon(981, (char)0xD5F4, 80, 65, new SdfUvRect(0.5416666f, 0.4033203f, 0.5677084f, 0.42447916f)),
            new SdfIcon(982, (char)0xD6F4, 80, 65, new SdfUvRect(0.5683594f, 0.4033203f, 0.59440106f, 0.42447916f)),
            new SdfIcon(983, (char)0xD7F4, 80, 80, new SdfUvRect(0.5188802f, 0.42740887f, 0.5449219f, 0.4534505f)),
            new SdfIcon(984, (char)0xD8F4, 80, 65, new SdfUvRect(0.59505206f, 0.4033203f, 0.62109375f, 0.42447916f)),
            new SdfIcon(985, (char)0xD9F4, 80, 65, new SdfUvRect(0.62174475f, 0.4033203f, 0.6477865f, 0.42447916f)),
            new SdfIcon(986, (char)0xDAF4, 65, 65, new SdfUvRect(0.31152344f, 0.9707031f, 0.33268228f, 0.991862f)),
            new SdfIcon(987, (char)0xDBF4, 80, 65, new SdfUvRect(0.6484375f, 0.4033203f, 0.6744792f, 0.42447916f)),
            new SdfIcon(988, (char)0xDCF4, 80, 65, new SdfUvRect(0.6751302f, 0.4033203f, 0.7011719f, 0.42447916f)),
            new SdfIcon(989, (char)0xDDF4, 80, 65, new SdfUvRect(0.7018229f, 0.4033203f, 0.7278646f, 0.42447916f)),
            new SdfIcon(990, (char)0xDEF4, 80, 80, new SdfUvRect(0.5455729f, 0.42740887f, 0.5716146f, 0.4534505f)),
            new SdfIcon(991, (char)0xDFF4, 80, 65, new SdfUvRect(0.7285156f, 0.4033203f, 0.7545573f, 0.42447916f)),
            new SdfIcon(992, (char)0xE0F4, 80, 65, new SdfUvRect(0.7552083f, 0.4033203f, 0.78125f, 0.42447916f)),
            new SdfIcon(993, (char)0xE1F4, 65, 65, new SdfUvRect(0.3873698f, 0.4501953f, 0.40852863f, 0.47135416f)),
            new SdfIcon(994, (char)0xE2F4, 58, 80, new SdfUvRect(0.9788411f, 0.37597656f, 0.9977214f, 0.40201822f)),
            new SdfIcon(995, (char)0xE3F4, 80, 56, new SdfUvRect(0.781901f, 0.4033203f, 0.80794275f, 0.42154947f)),
            new SdfIcon(996, (char)0xE4F4, 80, 56, new SdfUvRect(0.80859375f, 0.4033203f, 0.83463544f, 0.42154947f)),
            new SdfIcon(997, (char)0xE5F4, 80, 65, new SdfUvRect(0.83528644f, 0.4033203f, 0.8613281f, 0.42447916f)),
            new SdfIcon(998, (char)0xE6F4, 80, 66, new SdfUvRect(0.8619791f, 0.4033203f, 0.8880209f, 0.4248047f)),
            new SdfIcon(999, (char)0xE7F4, 58, 80, new SdfUvRect(0.3873698f, 0.47200522f, 0.40625f, 0.49804688f)),
            new SdfIcon(1000, (char)0xE8F4, 82, 82, new SdfUvRect(0.46451825f, 0.45410156f, 0.49121094f, 0.48079425f)),
            new SdfIcon(1001, (char)0xE9F4, 80, 80, new SdfUvRect(0.5722656f, 0.42740887f, 0.5983073f, 0.4534505f)),
            new SdfIcon(1002, (char)0xEAF4, 74, 74, new SdfUvRect(0.5989583f, 0.42740887f, 0.6230469f, 0.45149738f)),
            new SdfIcon(1003, (char)0xEBF4, 74, 74, new SdfUvRect(0.6236979f, 0.42740887f, 0.6477865f, 0.45149738f)),
            new SdfIcon(1004, (char)0xECF4, 58, 80, new SdfUvRect(0.3873698f, 0.49869794f, 0.40625f, 0.5247396f)),
            new SdfIcon(1005, (char)0xEDF4, 58, 80, new SdfUvRect(0.3873698f, 0.5253906f, 0.40625f, 0.5514323f)),
            new SdfIcon(1006, (char)0xEEF4, 80, 66, new SdfUvRect(0.8886719f, 0.4033203f, 0.91471356f, 0.4248047f)),
            new SdfIcon(1007, (char)0xEFF4, 80, 65, new SdfUvRect(0.91536456f, 0.4033203f, 0.94140625f, 0.42447916f)),
            new SdfIcon(1008, (char)0xF0F4, 80, 65, new SdfUvRect(0.94205725f, 0.4033203f, 0.968099f, 0.42447916f)),
            new SdfIcon(1009, (char)0xF1F4, 80, 65, new SdfUvRect(0.96875f, 0.4033203f, 0.9947917f, 0.42447916f)),
            new SdfIcon(1010, (char)0xF2F4, 80, 80, new SdfUvRect(0.6484375f, 0.42740887f, 0.6744792f, 0.4534505f)),
            new SdfIcon(1011, (char)0xF3F4, 80, 80, new SdfUvRect(0.6751302f, 0.42740887f, 0.7011719f, 0.4534505f)),
            new SdfIcon(1012, (char)0xF4F4, 48, 54, new SdfUvRect(0.9798177f, 0.1139323f, 0.99544275f, 0.13151042f)),
            new SdfIcon(1013, (char)0xF5F4, 48, 54, new SdfUvRect(0.3873698f, 0.5520833f, 0.40299478f, 0.5696615f)),
            new SdfIcon(1014, (char)0xF6F4, 57, 80, new SdfUvRect(0.3873698f, 0.5703125f, 0.40592447f, 0.5963542f)),
            new SdfIcon(1015, (char)0xF7F4, 57, 80, new SdfUvRect(0.3873698f, 0.5970052f, 0.40592447f, 0.6230469f)),
            new SdfIcon(1016, (char)0xF8F4, 80, 80, new SdfUvRect(0.7018229f, 0.42740887f, 0.7278646f, 0.4534505f)),
            new SdfIcon(1017, (char)0xF9F4, 80, 80, new SdfUvRect(0.7285156f, 0.42740887f, 0.7545573f, 0.4534505f)),
            new SdfIcon(1018, (char)0xFAF4, 80, 80, new SdfUvRect(0.7552083f, 0.42740887f, 0.78125f, 0.4534505f)),
            new SdfIcon(1019, (char)0xFBF4, 80, 80, new SdfUvRect(0.781901f, 0.42740887f, 0.80794275f, 0.4534505f)),
            new SdfIcon(1020, (char)0xFCF4, 80, 80, new SdfUvRect(0.80859375f, 0.42740887f, 0.83463544f, 0.4534505f)),
            new SdfIcon(1021, (char)0xFDF4, 80, 80, new SdfUvRect(0.83528644f, 0.42740887f, 0.8613281f, 0.4534505f)),
            new SdfIcon(1022, (char)0xFEF4, 48, 48, new SdfUvRect(0.5032552f, 0.06933594f, 0.51888025f, 0.08496094f)),
            new SdfIcon(1023, (char)0xFFF4, 68, 72, new SdfUvRect(0.3873698f, 0.6236979f, 0.4095052f, 0.64713544f)),
            new SdfIcon(1024, (char)0x00F5, 80, 68, new SdfUvRect(0.8619791f, 0.42740887f, 0.8880209f, 0.44954425f)),
            new SdfIcon(1025, (char)0x01F5, 80, 72, new SdfUvRect(0.8886719f, 0.42740887f, 0.91471356f, 0.45084634f)),
            new SdfIcon(1026, (char)0x02F5, 80, 66, new SdfUvRect(0.91536456f, 0.42740887f, 0.94140625f, 0.44889322f)),
            new SdfIcon(1027, (char)0x03F5, 80, 66, new SdfUvRect(0.94205725f, 0.42740887f, 0.968099f, 0.44889322f)),
            new SdfIcon(1028, (char)0x04F5, 80, 80, new SdfUvRect(0.96875f, 0.42740887f, 0.9947917f, 0.4534505f)),
            new SdfIcon(1029, (char)0x05F5, 80, 80, new SdfUvRect(0.4375f, 0.508138f, 0.46354166f, 0.5341797f)),
            new SdfIcon(1030, (char)0x06F5, 80, 80, new SdfUvRect(0.4375f, 0.5348307f, 0.46354166f, 0.56087244f)),
            new SdfIcon(1031, (char)0x07F5, 80, 80, new SdfUvRect(0.4375f, 0.56152344f, 0.46354166f, 0.5875651f)),
            new SdfIcon(1032, (char)0x08F5, 80, 80, new SdfUvRect(0.4375f, 0.5882161f, 0.46354166f, 0.6142578f)),
            new SdfIcon(1033, (char)0x09F5, 80, 80, new SdfUvRect(0.4375f, 0.6149088f, 0.46354166f, 0.64095056f)),
            new SdfIcon(1034, (char)0x0AF5, 80, 80, new SdfUvRect(0.4375f, 0.64160156f, 0.46354166f, 0.66764325f)),
            new SdfIcon(1035, (char)0x0BF5, 80, 80, new SdfUvRect(0.4375f, 0.66829425f, 0.46354166f, 0.69433594f)),
            new SdfIcon(1036, (char)0x0CF5, 39, 52, new SdfUvRect(0.51953125f, 0.06933594f, 0.53222656f, 0.086263016f)),
            new SdfIcon(1037, (char)0x0DF5, 80, 50, new SdfUvRect(0.53287756f, 0.06933594f, 0.5589193f, 0.08561198f)),
            new SdfIcon(1038, (char)0x0EF5, 80, 79, new SdfUvRect(0.4375f, 0.69498694f, 0.46354166f, 0.7207031f)),
            new SdfIcon(1039, (char)0x0FF5, 72, 78, new SdfUvRect(0.4375f, 0.7213541f, 0.4609375f, 0.7467448f)),
            new SdfIcon(1040, (char)0x10F5, 76, 20, new SdfUvRect(0.5361328f, 0.00032552084f, 0.56087244f, 0.0068359375f)),
            new SdfIcon(1041, (char)0x11F5, 76, 29, new SdfUvRect(0.56152344f, 0.0003255208f, 0.58626306f, 0.009765625f)),
            new SdfIcon(1042, (char)0x12F5, 76, 40, new SdfUvRect(0.58691406f, 0.00032552084f, 0.6116537f, 0.0133463545f)),
            new SdfIcon(1043, (char)0x13F5, 76, 52, new SdfUvRect(0.5595703f, 0.06933594f, 0.58430994f, 0.086263016f)),
            new SdfIcon(1044, (char)0x14F5, 76, 65, new SdfUvRect(0.4375f, 0.7473958f, 0.46223956f, 0.7685547f)),
            new SdfIcon(1045, (char)0x15F5, 80, 65, new SdfUvRect(0.4375f, 0.7692057f, 0.46354166f, 0.7903646f)),
            new SdfIcon(1046, (char)0x16F5, 80, 65, new SdfUvRect(0.4375f, 0.7910156f, 0.46354166f, 0.8121745f)),
            new SdfIcon(1047, (char)0x17F5, 80, 80, new SdfUvRect(0.4375f, 0.8128255f, 0.46354166f, 0.8388672f)),
            new SdfIcon(1048, (char)0x18F5, 80, 80, new SdfUvRect(0.4375f, 0.8395182f, 0.46354166f, 0.86555994f)),
            new SdfIcon(1049, (char)0x19F5, 58, 58, new SdfUvRect(0.3873698f, 0.64778644f, 0.40625f, 0.6666667f)),
            new SdfIcon(1050, (char)0x1AF5, 58, 58, new SdfUvRect(0.3873698f, 0.6673177f, 0.40625f, 0.68619794f)),
            new SdfIcon(1051, (char)0x1BF5, 58, 58, new SdfUvRect(0.3873698f, 0.68684894f, 0.40625f, 0.7057292f)),
            new SdfIcon(1052, (char)0x1CF5, 58, 58, new SdfUvRect(0.3873698f, 0.7063802f, 0.40625f, 0.72526044f)),
            new SdfIcon(1053, (char)0x1DF5, 81, 57, new SdfUvRect(0.4375f, 0.86621094f, 0.4638672f, 0.8847656f)),
            new SdfIcon(1054, (char)0x1EF5, 81, 57, new SdfUvRect(0.4375f, 0.8854166f, 0.4638672f, 0.9039714f)),
            new SdfIcon(1055, (char)0x1FF5, 69, 57, new SdfUvRect(0.3873698f, 0.72591144f, 0.40983072f, 0.7444662f)),
            new SdfIcon(1056, (char)0x20F5, 70, 57, new SdfUvRect(0.3873698f, 0.7451172f, 0.41015625f, 0.7636719f)),
            new SdfIcon(1057, (char)0x21F5, 80, 80, new SdfUvRect(0.4375f, 0.9046224f, 0.46354166f, 0.93066406f)),
            new SdfIcon(1058, (char)0x22F5, 80, 80, new SdfUvRect(0.4375f, 0.93131506f, 0.46354166f, 0.9573568f)),
            new SdfIcon(1059, (char)0x23F5, 80, 80, new SdfUvRect(0.4375f, 0.9580078f, 0.46354166f, 0.9840495f)),
            new SdfIcon(1060, (char)0x24F5, 80, 80, new SdfUvRect(0.491862f, 0.45410156f, 0.5179037f, 0.48014322f)),
            new SdfIcon(1061, (char)0x25F5, 80, 80, new SdfUvRect(0.5185547f, 0.45410156f, 0.5445964f, 0.48014322f)),
            new SdfIcon(1062, (char)0x26F5, 80, 80, new SdfUvRect(0.5452474f, 0.45410156f, 0.57128906f, 0.48014322f)),
            new SdfIcon(1063, (char)0x27F5, 80, 80, new SdfUvRect(0.57194006f, 0.45410156f, 0.5979818f, 0.48014322f)),
            new SdfIcon(1064, (char)0x28F5, 66, 77, new SdfUvRect(0.3873698f, 0.7643229f, 0.40885416f, 0.78938806f)),
            new SdfIcon(1065, (char)0x29F5, 81, 81, new SdfUvRect(0.5986328f, 0.45410156f, 0.625f, 0.48046875f)),
            new SdfIcon(1066, (char)0x2AF5, 81, 81, new SdfUvRect(0.625651f, 0.45410156f, 0.65201825f, 0.48046875f)),
            new SdfIcon(1067, (char)0x2BF5, 80, 41, new SdfUvRect(0.6123047f, 0.0003255208f, 0.6383464f, 0.013671875f)),
            new SdfIcon(1068, (char)0x2CF5, 70, 80, new SdfUvRect(0.3873698f, 0.79003906f, 0.41015625f, 0.81608075f)),
            new SdfIcon(1069, (char)0x2DF5, 81, 82, new SdfUvRect(0.65266925f, 0.45410156f, 0.6790365f, 0.48079425f)),
            new SdfIcon(1070, (char)0x2EF5, 81, 82, new SdfUvRect(0.6796875f, 0.45410156f, 0.7060547f, 0.48079425f)),
            new SdfIcon(1071, (char)0x2FF5, 76, 80, new SdfUvRect(0.7067057f, 0.45410156f, 0.7314453f, 0.48014322f)),
            new SdfIcon(1072, (char)0x30F5, 76, 80, new SdfUvRect(0.7320963f, 0.45410156f, 0.75683594f, 0.48014322f)),
            new SdfIcon(1073, (char)0x31F5, 76, 80, new SdfUvRect(0.75748694f, 0.45410156f, 0.78222656f, 0.48014322f)),
            new SdfIcon(1074, (char)0x32F5, 76, 80, new SdfUvRect(0.78287756f, 0.45410156f, 0.8076172f, 0.48014322f)),
            new SdfIcon(1075, (char)0x33F5, 76, 80, new SdfUvRect(0.8082682f, 0.45410156f, 0.8330078f, 0.48014322f)),
            new SdfIcon(1076, (char)0x34F5, 76, 80, new SdfUvRect(0.8336588f, 0.45410156f, 0.85839844f, 0.48014322f)),
            new SdfIcon(1077, (char)0x35F5, 76, 80, new SdfUvRect(0.85904944f, 0.45410156f, 0.88378906f, 0.48014322f)),
            new SdfIcon(1078, (char)0x36F5, 76, 80, new SdfUvRect(0.88444006f, 0.45410156f, 0.9091797f, 0.48014322f)),
            new SdfIcon(1079, (char)0x37F5, 76, 80, new SdfUvRect(0.9098307f, 0.45410156f, 0.9345703f, 0.48014322f)),
            new SdfIcon(1080, (char)0x38F5, 76, 80, new SdfUvRect(0.9352213f, 0.45410156f, 0.95996094f, 0.48014322f)),
            new SdfIcon(1081, (char)0x39F5, 76, 80, new SdfUvRect(0.96061194f, 0.45410156f, 0.98535156f, 0.48014322f)),
            new SdfIcon(1082, (char)0x3AF5, 76, 80, new SdfUvRect(0.46451825f, 0.4814453f, 0.4892578f, 0.507487f)),
            new SdfIcon(1083, (char)0x3BF5, 76, 80, new SdfUvRect(0.46451825f, 0.508138f, 0.4892578f, 0.5341797f)),
            new SdfIcon(1084, (char)0x3CF5, 75, 80, new SdfUvRect(0.46451825f, 0.5348307f, 0.48893228f, 0.56087244f)),
            new SdfIcon(1085, (char)0x3DF5, 75, 80, new SdfUvRect(0.46451825f, 0.56152344f, 0.48893228f, 0.5875651f)),
            new SdfIcon(1086, (char)0x3EF5, 76, 80, new SdfUvRect(0.46451825f, 0.5882161f, 0.4892578f, 0.6142578f)),
            new SdfIcon(1087, (char)0x3FF5, 76, 80, new SdfUvRect(0.46451825f, 0.6149088f, 0.4892578f, 0.64095056f)),
            new SdfIcon(1088, (char)0x40F5, 76, 69, new SdfUvRect(0.46451825f, 0.64160156f, 0.4892578f, 0.6640625f)),
            new SdfIcon(1089, (char)0x41F5, 76, 69, new SdfUvRect(0.46451825f, 0.6647135f, 0.4892578f, 0.6871745f)),
            new SdfIcon(1090, (char)0x42F5, 80, 76, new SdfUvRect(0.48990887f, 0.4814453f, 0.51595056f, 0.50618494f)),
            new SdfIcon(1091, (char)0x43F5, 80, 76, new SdfUvRect(0.51660156f, 0.4814453f, 0.54264325f, 0.50618494f)),
            new SdfIcon(1092, (char)0x44F5, 80, 72, new SdfUvRect(0.54329425f, 0.4814453f, 0.56933594f, 0.5048828f)),
            new SdfIcon(1093, (char)0x45F5, 76, 79, new SdfUvRect(0.46451825f, 0.6878255f, 0.4892578f, 0.7135417f)),
            new SdfIcon(1094, (char)0x46F5, 76, 79, new SdfUvRect(0.46451825f, 0.7141927f, 0.4892578f, 0.7399089f)),
            new SdfIcon(1095, (char)0x47F5, 74, 79, new SdfUvRect(0.46451825f, 0.7405599f, 0.48860675f, 0.76627606f)),
            new SdfIcon(1096, (char)0x48F5, 80, 79, new SdfUvRect(0.48990887f, 0.50683594f, 0.51595056f, 0.5325521f)),
            new SdfIcon(1097, (char)0x49F5, 80, 79, new SdfUvRect(0.48990887f, 0.5332031f, 0.51595056f, 0.5589193f)),
            new SdfIcon(1098, (char)0x4AF5, 74, 79, new SdfUvRect(0.46451825f, 0.76692706f, 0.48860675f, 0.79264325f)),
            new SdfIcon(1099, (char)0x4BF5, 66, 80, new SdfUvRect(0.3873698f, 0.81673175f, 0.40885416f, 0.84277344f)),
            new SdfIcon(1100, (char)0x4CF5, 66, 80, new SdfUvRect(0.3873698f, 0.84342444f, 0.40885416f, 0.8694662f)),
            new SdfIcon(1101, (char)0x4DF5, 80, 65, new SdfUvRect(0.56998694f, 0.4814453f, 0.5960287f, 0.5026042f)),
            new SdfIcon(1102, (char)0x4EF5, 80, 65, new SdfUvRect(0.5966797f, 0.4814453f, 0.6227214f, 0.5026042f)),
            new SdfIcon(1103, (char)0x4FF5, 80, 80, new SdfUvRect(0.48990887f, 0.5595703f, 0.51595056f, 0.585612f)),
            new SdfIcon(1104, (char)0x50F5, 80, 80, new SdfUvRect(0.48990887f, 0.586263f, 0.51595056f, 0.6123047f)),
            new SdfIcon(1105, (char)0x51F5, 80, 54, new SdfUvRect(0.6233724f, 0.4814453f, 0.64941406f, 0.49902344f)),
            new SdfIcon(1106, (char)0x52F5, 80, 54, new SdfUvRect(0.65006506f, 0.4814453f, 0.6761068f, 0.49902344f)),
            new SdfIcon(1107, (char)0x53F5, 80, 65, new SdfUvRect(0.6767578f, 0.4814453f, 0.7027995f, 0.5026042f)),
            new SdfIcon(1108, (char)0x54F5, 80, 65, new SdfUvRect(0.7034505f, 0.4814453f, 0.7294922f, 0.5026042f)),
            new SdfIcon(1109, (char)0x55F5, 80, 80, new SdfUvRect(0.48990887f, 0.6129557f, 0.51595056f, 0.63899744f)),
            new SdfIcon(1110, (char)0x56F5, 80, 80, new SdfUvRect(0.48990887f, 0.63964844f, 0.51595056f, 0.6656901f)),
            new SdfIcon(1111, (char)0x57F5, 50, 54, new SdfUvRect(0.3873698f, 0.8701172f, 0.4036458f, 0.8876953f)),
            new SdfIcon(1112, (char)0x58F5, 50, 54, new SdfUvRect(0.3873698f, 0.8883463f, 0.4036458f, 0.9059245f)),
            new SdfIcon(1113, (char)0x59F5, 80, 65, new SdfUvRect(0.7301432f, 0.4814453f, 0.75618494f, 0.5026042f)),
            new SdfIcon(1114, (char)0x5AF5, 80, 65, new SdfUvRect(0.75683594f, 0.4814453f, 0.7828776f, 0.5026042f)),
            new SdfIcon(1115, (char)0x5BF5, 80, 80, new SdfUvRect(0.48990887f, 0.6663411f, 0.51595056f, 0.6923828f)),
            new SdfIcon(1116, (char)0x5CF5, 80, 80, new SdfUvRect(0.48990887f, 0.6930338f, 0.51595056f, 0.71907556f)),
            new SdfIcon(1117, (char)0x5DF5, 80, 54, new SdfUvRect(0.7835286f, 0.4814453f, 0.8095703f, 0.49902344f)),
            new SdfIcon(1118, (char)0x5EF5, 80, 54, new SdfUvRect(0.8102213f, 0.4814453f, 0.83626306f, 0.49902344f)),
            new SdfIcon(1119, (char)0x5FF5, 80, 65, new SdfUvRect(0.83691406f, 0.4814453f, 0.86295575f, 0.5026042f)),
            new SdfIcon(1120, (char)0x60F5, 80, 65, new SdfUvRect(0.86360675f, 0.4814453f, 0.88964844f, 0.5026042f)),
            new SdfIcon(1121, (char)0x61F5, 80, 80, new SdfUvRect(0.48990887f, 0.71972656f, 0.51595056f, 0.74576825f)),
            new SdfIcon(1122, (char)0x62F5, 80, 80, new SdfUvRect(0.48990887f, 0.74641925f, 0.51595056f, 0.77246094f)),
            new SdfIcon(1123, (char)0x63F5, 50, 54, new SdfUvRect(0.3873698f, 0.9065755f, 0.4036458f, 0.9241537f)),
            new SdfIcon(1124, (char)0x64F5, 50, 54, new SdfUvRect(0.3873698f, 0.9248047f, 0.4036458f, 0.9423828f)),
            new SdfIcon(1125, (char)0x65F5, 80, 80, new SdfUvRect(0.48990887f, 0.77311194f, 0.51595056f, 0.7991537f)),
            new SdfIcon(1126, (char)0x66F5, 80, 80, new SdfUvRect(0.48990887f, 0.7998047f, 0.51595056f, 0.8258464f)),
            new SdfIcon(1127, (char)0x67F5, 80, 80, new SdfUvRect(0.48990887f, 0.8264974f, 0.51595056f, 0.85253906f)),
            new SdfIcon(1128, (char)0x68F5, 80, 80, new SdfUvRect(0.48990887f, 0.85319006f, 0.51595056f, 0.8792318f)),
            new SdfIcon(1129, (char)0x69F5, 80, 80, new SdfUvRect(0.48990887f, 0.8798828f, 0.51595056f, 0.9059245f)),
            new SdfIcon(1130, (char)0x6AF5, 46, 46, new SdfUvRect(0.6389974f, 0.00032552084f, 0.6539714f, 0.0152994795f)),
            new SdfIcon(1131, (char)0x6BF5, 80, 76, new SdfUvRect(0.89029944f, 0.4814453f, 0.9163412f, 0.50618494f)),
            new SdfIcon(1132, (char)0x6CF5, 69, 80, new SdfUvRect(0.3873698f, 0.9430338f, 0.40983072f, 0.96907556f)),
            new SdfIcon(1133, (char)0x6DF5, 74, 80, new SdfUvRect(0.46451825f, 0.79329425f, 0.48860675f, 0.81933594f)),
            new SdfIcon(1134, (char)0x6EF5, 74, 80, new SdfUvRect(0.46451825f, 0.81998694f, 0.48860675f, 0.8460287f)),
            new SdfIcon(1135, (char)0x6FF5, 74, 80, new SdfUvRect(0.46451825f, 0.8466797f, 0.48860675f, 0.8727214f)),
            new SdfIcon(1136, (char)0x70F5, 65, 67, new SdfUvRect(0.3873698f, 0.96972656f, 0.40852863f, 0.9915365f)),
            new SdfIcon(1137, (char)0x71F5, 65, 67, new SdfUvRect(0.46451825f, 0.8733724f, 0.48567706f, 0.8951823f)),
            new SdfIcon(1138, (char)0x72F5, 65, 66, new SdfUvRect(0.46451825f, 0.8958333f, 0.48567706f, 0.91731775f)),
            new SdfIcon(1139, (char)0x73F5, 65, 66, new SdfUvRect(0.46451825f, 0.91796875f, 0.48567706f, 0.9394531f)),
            new SdfIcon(1140, (char)0x74F5, 75, 62, new SdfUvRect(0.46451825f, 0.9401041f, 0.48893228f, 0.9602865f)),
            new SdfIcon(1141, (char)0x75F5, 75, 62, new SdfUvRect(0.46451825f, 0.9609375f, 0.48893228f, 0.9811198f)),
            new SdfIcon(1142, (char)0x76F5, 65, 67, new SdfUvRect(0.9169922f, 0.4814453f, 0.93815106f, 0.50325525f)),
            new SdfIcon(1143, (char)0x77F5, 65, 67, new SdfUvRect(0.93880206f, 0.4814453f, 0.95996094f, 0.50325525f)),
            new SdfIcon(1144, (char)0x78F5, 65, 66, new SdfUvRect(0.96061194f, 0.4814453f, 0.9817709f, 0.5029297f)),
            new SdfIcon(1145, (char)0x79F5, 65, 67, new SdfUvRect(0.48990887f, 0.9065755f, 0.51106775f, 0.92838544f)),
            new SdfIcon(1146, (char)0x7AF5, 75, 61, new SdfUvRect(0.48990887f, 0.92903644f, 0.51432294f, 0.94889325f)),
            new SdfIcon(1147, (char)0x7BF5, 75, 62, new SdfUvRect(0.48990887f, 0.94954425f, 0.51432294f, 0.96972656f)),
            new SdfIcon(1148, (char)0x7CF5, 68, 65, new SdfUvRect(0.48990887f, 0.97037756f, 0.5120443f, 0.9915365f)),
            new SdfIcon(1149, (char)0x7DF5, 66, 80, new SdfUvRect(0.51660156f, 0.50683594f, 0.53808594f, 0.5328776f)),
            new SdfIcon(1150, (char)0x7EF5, 66, 80, new SdfUvRect(0.53873694f, 0.50683594f, 0.5602214f, 0.5328776f)),
            new SdfIcon(1151, (char)0x7FF5, 84, 84, new SdfUvRect(0.51660156f, 0.5335286f, 0.5439453f, 0.56087244f)),
            new SdfIcon(1152, (char)0x80F5, 82, 67, new SdfUvRect(0.5608724f, 0.50683594f, 0.5875651f, 0.5286459f)),
            new SdfIcon(1153, (char)0x81F5, 76, 64, new SdfUvRect(0.5882161f, 0.50683594f, 0.61295575f, 0.5276693f)),
            new SdfIcon(1154, (char)0x82F5, 80, 80, new SdfUvRect(0.61360675f, 0.50683594f, 0.63964844f, 0.5328776f)),
            new SdfIcon(1155, (char)0x83F5, 80, 80, new SdfUvRect(0.64029944f, 0.50683594f, 0.6663412f, 0.5328776f)),
            new SdfIcon(1156, (char)0x84F5, 80, 80, new SdfUvRect(0.6669922f, 0.50683594f, 0.6930339f, 0.5328776f)),
            new SdfIcon(1157, (char)0x85F5, 80, 79, new SdfUvRect(0.6936849f, 0.50683594f, 0.71972656f, 0.5325521f)),
            new SdfIcon(1158, (char)0x86F5, 82, 78, new SdfUvRect(0.72037756f, 0.50683594f, 0.7470703f, 0.53222656f)),
            new SdfIcon(1159, (char)0x87F5, 80, 78, new SdfUvRect(0.7477213f, 0.50683594f, 0.77376306f, 0.53222656f)),
            new SdfIcon(1160, (char)0x88F5, 82, 78, new SdfUvRect(0.77441406f, 0.50683594f, 0.8011068f, 0.53222656f)),
            new SdfIcon(1161, (char)0x89F5, 65, 80, new SdfUvRect(0.8017578f, 0.50683594f, 0.8229167f, 0.5328776f)),
            new SdfIcon(1162, (char)0x8AF5, 80, 80, new SdfUvRect(0.8235677f, 0.50683594f, 0.8496094f, 0.5328776f)),
            new SdfIcon(1163, (char)0x8BF5, 80, 80, new SdfUvRect(0.8502604f, 0.50683594f, 0.8763021f, 0.5328776f)),
            new SdfIcon(1164, (char)0x8CF5, 72, 72, new SdfUvRect(0.8769531f, 0.50683594f, 0.9003906f, 0.53027344f)),
            new SdfIcon(1165, (char)0x8DF5, 72, 72, new SdfUvRect(0.9010416f, 0.50683594f, 0.9244792f, 0.53027344f)),
            new SdfIcon(1166, (char)0x8EF5, 80, 65, new SdfUvRect(0.9251302f, 0.50683594f, 0.9511719f, 0.5279948f)),
            new SdfIcon(1167, (char)0x8FF5, 80, 65, new SdfUvRect(0.9518229f, 0.50683594f, 0.9778646f, 0.5279948f)),
            new SdfIcon(1168, (char)0x90F5, 80, 80, new SdfUvRect(0.51660156f, 0.56152344f, 0.54264325f, 0.5875651f)),
            new SdfIcon(1169, (char)0x91F5, 80, 80, new SdfUvRect(0.51660156f, 0.5882161f, 0.54264325f, 0.6142578f)),
            new SdfIcon(1170, (char)0x92F5, 52, 52, new SdfUvRect(0.58496094f, 0.06933594f, 0.60188806f, 0.086263016f)),
            new SdfIcon(1171, (char)0x93F5, 52, 52, new SdfUvRect(0.60253906f, 0.06933594f, 0.6194662f, 0.086263016f)),
            new SdfIcon(1172, (char)0x94F5, 64, 76, new SdfUvRect(0.9785156f, 0.50683594f, 0.999349f, 0.53157556f)),
            new SdfIcon(1173, (char)0x95F5, 66, 76, new SdfUvRect(0.51660156f, 0.6149088f, 0.53808594f, 0.63964844f)),
            new SdfIcon(1174, (char)0x96F5, 74, 80, new SdfUvRect(0.51660156f, 0.64029944f, 0.5406901f, 0.6663412f)),
            new SdfIcon(1175, (char)0x97F5, 74, 80, new SdfUvRect(0.51660156f, 0.6669922f, 0.5406901f, 0.6930339f)),
            new SdfIcon(1176, (char)0x98F5, 80, 80, new SdfUvRect(0.51660156f, 0.6936849f, 0.54264325f, 0.71972656f)),
            new SdfIcon(1177, (char)0x99F5, 74, 72, new SdfUvRect(0.51660156f, 0.72037756f, 0.5406901f, 0.7438151f)),
            new SdfIcon(1178, (char)0x9AF5, 82, 80, new SdfUvRect(0.51660156f, 0.7444661f, 0.5432943f, 0.7705078f)),
            new SdfIcon(1179, (char)0x9BF5, 64, 76, new SdfUvRect(0.51660156f, 0.7711588f, 0.53743494f, 0.79589844f)),
            new SdfIcon(1180, (char)0x9CF5, 66, 80, new SdfUvRect(0.51660156f, 0.79654944f, 0.53808594f, 0.8225912f)),
            new SdfIcon(1181, (char)0x9DF5, 80, 71, new SdfUvRect(0.51660156f, 0.8232422f, 0.54264325f, 0.8463542f)),
            new SdfIcon(1182, (char)0x9EF5, 80, 71, new SdfUvRect(0.51660156f, 0.8470052f, 0.54264325f, 0.8701172f)),
            new SdfIcon(1183, (char)0x9FF5, 72, 76, new SdfUvRect(0.51660156f, 0.8707682f, 0.54003906f, 0.8955078f)),
            new SdfIcon(1184, (char)0xA0F5, 78, 80, new SdfUvRect(0.51660156f, 0.8961588f, 0.5419922f, 0.92220056f)),
            new SdfIcon(1185, (char)0xA1F5, 80, 80, new SdfUvRect(0.51660156f, 0.92285156f, 0.54264325f, 0.94889325f)),
            new SdfIcon(1186, (char)0xA2F5, 80, 80, new SdfUvRect(0.51660156f, 0.94954425f, 0.54264325f, 0.97558594f)),
            new SdfIcon(1187, (char)0xA3F5, 80, 40, new SdfUvRect(0.6546224f, 0.00032552084f, 0.68066406f, 0.0133463545f)),
            new SdfIcon(1188, (char)0xA4F5, 80, 62, new SdfUvRect(0.51660156f, 0.97623694f, 0.54264325f, 0.9964193f)),
            new SdfIcon(1189, (char)0xA5F5, 80, 62, new SdfUvRect(0.5445963f, 0.5335286f, 0.57063806f, 0.55371094f)),
            new SdfIcon(1190, (char)0xA6F5, 80, 62, new SdfUvRect(0.57128906f, 0.5335286f, 0.59733075f, 0.55371094f)),
            new SdfIcon(1191, (char)0xA7F5, 80, 62, new SdfUvRect(0.59798175f, 0.5335286f, 0.62402344f, 0.55371094f)),
            new SdfIcon(1192, (char)0xA8F5, 66, 82, new SdfUvRect(0.5445963f, 0.55436194f, 0.56608075f, 0.5810547f)),
            new SdfIcon(1193, (char)0xA9F5, 82, 66, new SdfUvRect(0.56673175f, 0.55436194f, 0.5934245f, 0.5758464f)),
            new SdfIcon(1194, (char)0xAAF5, 80, 80, new SdfUvRect(0.56673175f, 0.5764974f, 0.59277344f, 0.60253906f)),
            new SdfIcon(1195, (char)0xABF5, 66, 80, new SdfUvRect(0.5445963f, 0.5817057f, 0.56608075f, 0.60774744f)),
            new SdfIcon(1196, (char)0xACF5, 80, 65, new SdfUvRect(0.5940755f, 0.55436194f, 0.6201172f, 0.5755209f)),
            new SdfIcon(1197, (char)0xADF5, 80, 66, new SdfUvRect(0.6207682f, 0.55436194f, 0.64680994f, 0.5758464f)),
            new SdfIcon(1198, (char)0xAEF5, 66, 80, new SdfUvRect(0.5445963f, 0.60839844f, 0.56608075f, 0.6344401f)),
            new SdfIcon(1199, (char)0xAFF5, 71, 71, new SdfUvRect(0.56673175f, 0.60319006f, 0.58984375f, 0.6263021f)),
            new SdfIcon(1200, (char)0xB0F5, 71, 71, new SdfUvRect(0.56673175f, 0.6269531f, 0.58984375f, 0.6500651f)),
            new SdfIcon(1201, (char)0xB1F5, 79, 75, new SdfUvRect(0.56673175f, 0.6507161f, 0.59244794f, 0.67513025f)),
            new SdfIcon(1202, (char)0xB2F5, 79, 75, new SdfUvRect(0.56673175f, 0.67578125f, 0.59244794f, 0.7001953f)),
            new SdfIcon(1203, (char)0xB3F5, 80, 80, new SdfUvRect(0.56673175f, 0.7008463f, 0.59277344f, 0.72688806f)),
            new SdfIcon(1204, (char)0xB4F5, 82, 82, new SdfUvRect(0.59342444f, 0.5764974f, 0.6201172f, 0.6031901f)),
            new SdfIcon(1205, (char)0xB5F5, 82, 82, new SdfUvRect(0.6207682f, 0.5764974f, 0.64746094f, 0.6031901f)),
            new SdfIcon(1206, (char)0xB6F5, 82, 82, new SdfUvRect(0.64811194f, 0.5764974f, 0.6748047f, 0.6031901f)),
            new SdfIcon(1207, (char)0xB7F5, 82, 82, new SdfUvRect(0.6754557f, 0.5764974f, 0.70214844f, 0.6031901f)),
            new SdfIcon(1208, (char)0xB8F5, 82, 82, new SdfUvRect(0.70279944f, 0.5764974f, 0.7294922f, 0.6031901f)),
            new SdfIcon(1209, (char)0xB9F5, 82, 82, new SdfUvRect(0.7301432f, 0.5764974f, 0.75683594f, 0.6031901f)),
            new SdfIcon(1210, (char)0xBAF5, 82, 82, new SdfUvRect(0.75748694f, 0.5764974f, 0.7841797f, 0.6031901f)),
            new SdfIcon(1211, (char)0xBBF5, 82, 82, new SdfUvRect(0.7848307f, 0.5764974f, 0.81152344f, 0.6031901f)),
            new SdfIcon(1212, (char)0xBCF5, 82, 82, new SdfUvRect(0.81217444f, 0.5764974f, 0.8388672f, 0.6031901f)),
            new SdfIcon(1213, (char)0xBDF5, 82, 82, new SdfUvRect(0.8395182f, 0.5764974f, 0.86621094f, 0.6031901f)),
            new SdfIcon(1214, (char)0xBEF5, 82, 82, new SdfUvRect(0.86686194f, 0.5764974f, 0.8935547f, 0.6031901f)),
            new SdfIcon(1215, (char)0xBFF5, 82, 82, new SdfUvRect(0.8942057f, 0.5764974f, 0.92089844f, 0.6031901f)),
            new SdfIcon(1216, (char)0xC0F5, 82, 82, new SdfUvRect(0.92154944f, 0.5764974f, 0.9482422f, 0.6031901f)),
            new SdfIcon(1217, (char)0xC1F5, 82, 82, new SdfUvRect(0.9488932f, 0.5764974f, 0.97558594f, 0.6031901f)),
            new SdfIcon(1218, (char)0xC2F5, 80, 72, new SdfUvRect(0.56673175f, 0.72753906f, 0.59277344f, 0.75097656f)),
            new SdfIcon(1219, (char)0xC3F5, 80, 72, new SdfUvRect(0.56673175f, 0.75162756f, 0.59277344f, 0.7750651f)),
            new SdfIcon(1220, (char)0xC4F5, 65, 58, new SdfUvRect(0.62467444f, 0.5335286f, 0.6458334f, 0.5524089f)),
            new SdfIcon(1221, (char)0xC5F5, 65, 58, new SdfUvRect(0.6464844f, 0.5335286f, 0.66764325f, 0.5524089f)),
            new SdfIcon(1222, (char)0xC6F5, 65, 58, new SdfUvRect(0.66829425f, 0.5335286f, 0.6894531f, 0.5524089f)),
            new SdfIcon(1223, (char)0xC7F5, 65, 58, new SdfUvRect(0.6901041f, 0.5335286f, 0.71126306f, 0.5524089f)),
            new SdfIcon(1224, (char)0xC8F5, 65, 58, new SdfUvRect(0.71191406f, 0.5335286f, 0.73307294f, 0.5524089f)),
            new SdfIcon(1225, (char)0xC9F5, 65, 58, new SdfUvRect(0.73372394f, 0.5335286f, 0.7548828f, 0.5524089f)),
            new SdfIcon(1226, (char)0xCAF5, 80, 66, new SdfUvRect(0.64746094f, 0.55436194f, 0.6735026f, 0.5758464f)),
            new SdfIcon(1227, (char)0xCBF5, 80, 72, new SdfUvRect(0.56673175f, 0.7757161f, 0.59277344f, 0.7991537f)),
            new SdfIcon(1228, (char)0xCCF5, 80, 72, new SdfUvRect(0.56673175f, 0.7998047f, 0.59277344f, 0.8232422f)),
            new SdfIcon(1229, (char)0xCDF5, 46, 81, new SdfUvRect(0.5445963f, 0.6350911f, 0.5595703f, 0.6614584f)),
            new SdfIcon(1230, (char)0xCEF5, 46, 81, new SdfUvRect(0.5445963f, 0.6621094f, 0.5595703f, 0.68847656f)),
            new SdfIcon(1231, (char)0xCFF5, 46, 81, new SdfUvRect(0.5445963f, 0.68912756f, 0.5595703f, 0.7154948f)),
            new SdfIcon(1232, (char)0xD0F5, 73, 81, new SdfUvRect(0.56673175f, 0.8238932f, 0.5904948f, 0.85026044f)),
            new SdfIcon(1233, (char)0xD1F5, 76, 81, new SdfUvRect(0.56673175f, 0.85091144f, 0.5914714f, 0.8772787f)),
            new SdfIcon(1234, (char)0xD2F5, 46, 81, new SdfUvRect(0.5445963f, 0.7161458f, 0.5595703f, 0.74251306f)),
            new SdfIcon(1235, (char)0xD3F5, 28, 70, new SdfUvRect(0.9905599f, 0.13541666f, 0.9996745f, 0.15820312f)),
            new SdfIcon(1236, (char)0xD4F5, 70, 28, new SdfUvRect(0.68131506f, 0.00032552084f, 0.70410156f, 0.0094401045f)),
            new SdfIcon(1237, (char)0xD5F5, 80, 58, new SdfUvRect(0.7555338f, 0.5335286f, 0.78157556f, 0.5524089f)),
            new SdfIcon(1238, (char)0xD6F5, 80, 58, new SdfUvRect(0.78222656f, 0.5335286f, 0.80826825f, 0.5524089f)),
            new SdfIcon(1239, (char)0xD7F5, 80, 58, new SdfUvRect(0.80891925f, 0.5335286f, 0.83496094f, 0.5524089f)),
            new SdfIcon(1240, (char)0xD8F5, 80, 58, new SdfUvRect(0.83561194f, 0.5335286f, 0.8616537f, 0.5524089f)),
            new SdfIcon(1241, (char)0xD9F5, 72, 80, new SdfUvRect(0.56673175f, 0.8779297f, 0.5901693f, 0.9039714f)),
            new SdfIcon(1242, (char)0xDAF5, 66, 80, new SdfUvRect(0.5445963f, 0.74316406f, 0.56608075f, 0.76920575f)),
            new SdfIcon(1243, (char)0xDBF5, 81, 82, new SdfUvRect(0.59342444f, 0.6038411f, 0.6197917f, 0.6305339f)),
            new SdfIcon(1244, (char)0xDCF5, 72, 81, new SdfUvRect(0.56673175f, 0.9046224f, 0.5901693f, 0.9309896f)),
            new SdfIcon(1245, (char)0xDDF5, 70, 76, new SdfUvRect(0.56673175f, 0.9316406f, 0.58951825f, 0.95638025f)),
            new SdfIcon(1246, (char)0xDEF5, 70, 76, new SdfUvRect(0.56673175f, 0.95703125f, 0.58951825f, 0.9817709f)),
            new SdfIcon(1247, (char)0xDFF5, 66, 72, new SdfUvRect(0.5445963f, 0.76985675f, 0.56608075f, 0.7932943f)),
            new SdfIcon(1248, (char)0xE0F5, 66, 72, new SdfUvRect(0.5445963f, 0.7939453f, 0.56608075f, 0.8173828f)),
            new SdfIcon(1249, (char)0xE1F5, 66, 80, new SdfUvRect(0.5445963f, 0.8180338f, 0.56608075f, 0.84407556f)),
            new SdfIcon(1250, (char)0xE2F5, 66, 80, new SdfUvRect(0.5445963f, 0.84472656f, 0.56608075f, 0.87076825f)),
            new SdfIcon(1251, (char)0xE3F5, 82, 72, new SdfUvRect(0.6204427f, 0.6038411f, 0.64713544f, 0.6272787f)),
            new SdfIcon(1252, (char)0xE4F5, 82, 72, new SdfUvRect(0.64778644f, 0.6038411f, 0.6744792f, 0.6272787f)),
            new SdfIcon(1253, (char)0xE5F5, 82, 72, new SdfUvRect(0.6751302f, 0.6038411f, 0.70182294f, 0.6272787f)),
            new SdfIcon(1254, (char)0xE6F5, 82, 80, new SdfUvRect(0.6204427f, 0.6279297f, 0.64713544f, 0.6539714f)),
            new SdfIcon(1255, (char)0xE7F5, 82, 80, new SdfUvRect(0.6204427f, 0.6546224f, 0.64713544f, 0.68066406f)),
            new SdfIcon(1256, (char)0xE8F5, 80, 68, new SdfUvRect(0.59342444f, 0.6311849f, 0.6194662f, 0.6533203f)),
            new SdfIcon(1257, (char)0xE9F5, 80, 56, new SdfUvRect(0.8623047f, 0.5335286f, 0.8883464f, 0.5517578f)),
            new SdfIcon(1258, (char)0xEAF5, 80, 66, new SdfUvRect(0.6741536f, 0.55436194f, 0.7001953f, 0.5758464f)),
            new SdfIcon(1259, (char)0xEBF5, 82, 81, new SdfUvRect(0.6204427f, 0.68131506f, 0.64713544f, 0.7076823f)),
            new SdfIcon(1260, (char)0xECF5, 80, 65, new SdfUvRect(0.7008463f, 0.55436194f, 0.72688806f, 0.5755209f)),
            new SdfIcon(1261, (char)0xEDF5, 80, 65, new SdfUvRect(0.72753906f, 0.55436194f, 0.75358075f, 0.5755209f)),
            new SdfIcon(1262, (char)0xEEF5, 71, 80, new SdfUvRect(0.97623694f, 0.5764974f, 0.999349f, 0.60253906f)),
            new SdfIcon(1263, (char)0xEFF5, 80, 69, new SdfUvRect(0.59342444f, 0.6539713f, 0.6194662f, 0.6764323f)),
            new SdfIcon(1264, (char)0xF0F5, 48, 57, new SdfUvRect(0.9824219f, 0.4814453f, 0.9980469f, 0.49999997f)),
            new SdfIcon(1265, (char)0xF1F5, 67, 55, new SdfUvRect(0.46451825f, 0.9817708f, 0.48632812f, 0.9996745f)),
            new SdfIcon(1266, (char)0xF2F5, 78, 56, new SdfUvRect(0.8889974f, 0.5335286f, 0.91438806f, 0.5517578f)),
            new SdfIcon(1267, (char)0xF3F5, 78, 56, new SdfUvRect(0.91503906f, 0.5335286f, 0.9404297f, 0.5517578f)),
            new SdfIcon(1268, (char)0xF4F5, 40, 57, new SdfUvRect(0.20019531f, 0.4244792f, 0.21321616f, 0.44303384f)),
            new SdfIcon(1269, (char)0xF5F5, 72, 58, new SdfUvRect(0.9410807f, 0.5335286f, 0.96451825f, 0.5524089f)),
            new SdfIcon(1270, (char)0xF6F5, 52, 64, new SdfUvRect(0.5445963f, 0.87141925f, 0.56152344f, 0.8922526f)),
            new SdfIcon(1271, (char)0xF7F5, 74, 55, new SdfUvRect(0.96516925f, 0.5335286f, 0.9892578f, 0.5514323f)),
            new SdfIcon(1272, (char)0xF8F5, 80, 80, new SdfUvRect(0.59342444f, 0.6770833f, 0.6194662f, 0.703125f)),
            new SdfIcon(1273, (char)0xF9F5, 76, 72, new SdfUvRect(0.59342444f, 0.703776f, 0.61816406f, 0.72721356f)),
            new SdfIcon(1274, (char)0xFAF5, 80, 80, new SdfUvRect(0.59342444f, 0.72786456f, 0.6194662f, 0.75390625f)),
            new SdfIcon(1275, (char)0xFBF5, 76, 72, new SdfUvRect(0.59342444f, 0.75455725f, 0.61816406f, 0.7779948f)),
            new SdfIcon(1276, (char)0xFCF5, 80, 80, new SdfUvRect(0.59342444f, 0.7786458f, 0.6194662f, 0.8046875f)),
            new SdfIcon(1277, (char)0xFDF5, 80, 80, new SdfUvRect(0.59342444f, 0.8053385f, 0.6194662f, 0.83138025f)),
            new SdfIcon(1278, (char)0xFEF5, 80, 80, new SdfUvRect(0.59342444f, 0.83203125f, 0.6194662f, 0.85807294f)),
            new SdfIcon(1279, (char)0xFFF5, 69, 80, new SdfUvRect(0.59342444f, 0.85872394f, 0.61588544f, 0.8847656f)),
            new SdfIcon(1280, (char)0x00F6, 69, 80, new SdfUvRect(0.59342444f, 0.8854166f, 0.61588544f, 0.9114584f)),
            new SdfIcon(1281, (char)0x01F6, 80, 80, new SdfUvRect(0.59342444f, 0.9121094f, 0.6194662f, 0.93815106f)),
            new SdfIcon(1282, (char)0x02F6, 58, 48, new SdfUvRect(0.6201172f, 0.06933594f, 0.63899744f, 0.08496094f)),
            new SdfIcon(1283, (char)0x03F6, 80, 72, new SdfUvRect(0.59342444f, 0.93880206f, 0.6194662f, 0.9622396f)),
            new SdfIcon(1284, (char)0x04F6, 74, 74, new SdfUvRect(0.59342444f, 0.9628906f, 0.61751306f, 0.9869792f)),
            new SdfIcon(1285, (char)0x05F6, 72, 70, new SdfUvRect(0.70247394f, 0.6038411f, 0.7259115f, 0.6266276f)),
            new SdfIcon(1286, (char)0x06F6, 72, 80, new SdfUvRect(0.6204427f, 0.7083333f, 0.64388025f, 0.734375f)),
            new SdfIcon(1287, (char)0x07F6, 80, 80, new SdfUvRect(0.6204427f, 0.735026f, 0.6464844f, 0.76106775f)),
            new SdfIcon(1288, (char)0x08F6, 80, 80, new SdfUvRect(0.6204427f, 0.76171875f, 0.6464844f, 0.78776044f)),
            new SdfIcon(1289, (char)0x09F6, 81, 45, new SdfUvRect(0.70475256f, 0.00032552084f, 0.7311198f, 0.014973959f)),
            new SdfIcon(1290, (char)0x0AF6, 53, 54, new SdfUvRect(0.5445963f, 0.8929036f, 0.561849f, 0.9104818f)),
            new SdfIcon(1291, (char)0x0BF6, 53, 54, new SdfUvRect(0.5445963f, 0.9111328f, 0.561849f, 0.92871094f)),
            new SdfIcon(1292, (char)0x0CF6, 69, 54, new SdfUvRect(0.75423175f, 0.55436194f, 0.77669275f, 0.5719401f)),
            new SdfIcon(1293, (char)0x0DF6, 69, 54, new SdfUvRect(0.77734375f, 0.55436194f, 0.7998047f, 0.5719401f)),
            new SdfIcon(1294, (char)0x0EF6, 40, 54, new SdfUvRect(0.98600256f, 0.45410156f, 0.99902344f, 0.4716797f)),
            new SdfIcon(1295, (char)0x0FF6, 40, 54, new SdfUvRect(0.5445963f, 0.92936194f, 0.5576172f, 0.9469401f)),
            new SdfIcon(1296, (char)0x10F6, 69, 66, new SdfUvRect(0.8004557f, 0.55436194f, 0.8229167f, 0.5758464f)),
            new SdfIcon(1297, (char)0x11F6, 69, 66, new SdfUvRect(0.8235677f, 0.55436194f, 0.8460287f, 0.5758464f)),
            new SdfIcon(1298, (char)0x12F6, 66, 80, new SdfUvRect(0.5445963f, 0.9475911f, 0.56608075f, 0.9736328f)),
            new SdfIcon(1299, (char)0x13F6, 80, 66, new SdfUvRect(0.8466797f, 0.55436194f, 0.8727214f, 0.5758464f)),
            new SdfIcon(1300, (char)0x14F6, 80, 72, new SdfUvRect(0.7265625f, 0.6038411f, 0.7526042f, 0.6272787f)),
            new SdfIcon(1301, (char)0x15F6, 80, 76, new SdfUvRect(0.6204427f, 0.78841144f, 0.6464844f, 0.81315106f)),
            new SdfIcon(1302, (char)0x16F6, 68, 80, new SdfUvRect(0.6204427f, 0.81380206f, 0.6425781f, 0.83984375f)),
            new SdfIcon(1303, (char)0x17F6, 82, 62, new SdfUvRect(0.8733724f, 0.55436194f, 0.9000651f, 0.5745443f)),
            new SdfIcon(1304, (char)0x18F6, 80, 80, new SdfUvRect(0.6204427f, 0.84049475f, 0.6464844f, 0.8665365f)),
            new SdfIcon(1305, (char)0x19F6, 42, 34, new SdfUvRect(0.7317708f, 0.00032552084f, 0.74544275f, 0.0113932295f)),
            new SdfIcon(1306, (char)0x1AF6, 60, 47, new SdfUvRect(0.63964844f, 0.06933594f, 0.6591797f, 0.084635414f)),
            new SdfIcon(1307, (char)0x1BF6, 78, 65, new SdfUvRect(0.9007161f, 0.55436194f, 0.9261068f, 0.5755209f)),
            new SdfIcon(1308, (char)0x1CF6, 78, 59, new SdfUvRect(0.9267578f, 0.55436194f, 0.95214844f, 0.57356775f)),
            new SdfIcon(1309, (char)0x1DF6, 80, 72, new SdfUvRect(0.7532552f, 0.6038411f, 0.7792969f, 0.6272787f)),
            new SdfIcon(1310, (char)0x1EF6, 80, 72, new SdfUvRect(0.7799479f, 0.6038411f, 0.8059896f, 0.6272787f)),
            new SdfIcon(1311, (char)0x1FF6, 80, 72, new SdfUvRect(0.8066406f, 0.6038411f, 0.8326823f, 0.6272787f)),
            new SdfIcon(1312, (char)0x20F6, 80, 72, new SdfUvRect(0.8333333f, 0.6038411f, 0.859375f, 0.6272787f)),
            new SdfIcon(1313, (char)0x21F6, 82, 81, new SdfUvRect(0.6204427f, 0.8671875f, 0.64713544f, 0.8935547f)),
            new SdfIcon(1314, (char)0x22F6, 80, 80, new SdfUvRect(0.6204427f, 0.8942057f, 0.6464844f, 0.92024744f)),
            new SdfIcon(1315, (char)0x23F6, 80, 80, new SdfUvRect(0.6204427f, 0.92089844f, 0.6464844f, 0.9469401f)),
            new SdfIcon(1316, (char)0x24F6, 80, 80, new SdfUvRect(0.6204427f, 0.9475911f, 0.6464844f, 0.9736328f)),
            new SdfIcon(1317, (char)0x25F6, 80, 80, new SdfUvRect(0.64778644f, 0.6279297f, 0.6738281f, 0.6539714f)),
            new SdfIcon(1318, (char)0x26F6, 80, 80, new SdfUvRect(0.6744791f, 0.6279297f, 0.7005209f, 0.6539714f)),
            new SdfIcon(1319, (char)0x27F6, 80, 80, new SdfUvRect(0.7011719f, 0.6279297f, 0.72721356f, 0.6539714f)),
            new SdfIcon(1320, (char)0x28F6, 80, 80, new SdfUvRect(0.72786456f, 0.6279297f, 0.75390625f, 0.6539714f)),
            new SdfIcon(1321, (char)0x29F6, 80, 80, new SdfUvRect(0.75455725f, 0.6279297f, 0.780599f, 0.6539714f)),
            new SdfIcon(1322, (char)0x2AF6, 46, 46, new SdfUvRect(0.74609375f, 0.00032552084f, 0.76106775f, 0.0152994795f)),
            new SdfIcon(1323, (char)0x2BF6, 80, 62, new SdfUvRect(0.95279944f, 0.55436194f, 0.9788412f, 0.5745443f)),
            new SdfIcon(1324, (char)0x2CF6, 80, 80, new SdfUvRect(0.78125f, 0.6279297f, 0.8072917f, 0.6539714f)),
            new SdfIcon(1325, (char)0x2DF6, 80, 80, new SdfUvRect(0.8079427f, 0.6279297f, 0.8339844f, 0.6539714f)),
            new SdfIcon(1326, (char)0x2EF6, 82, 82, new SdfUvRect(0.64778644f, 0.6546224f, 0.6744792f, 0.6813151f)),
            new SdfIcon(1327, (char)0x2FF6, 80, 81, new SdfUvRect(0.64778644f, 0.6819661f, 0.6738281f, 0.7083334f)),
            new SdfIcon(1328, (char)0x30F6, 81, 83, new SdfUvRect(0.64778644f, 0.7089844f, 0.6741537f, 0.7360026f)),
            new SdfIcon(1329, (char)0x31F6, 81, 83, new SdfUvRect(0.64778644f, 0.7366536f, 0.6741537f, 0.7636719f)),
            new SdfIcon(1330, (char)0x32F6, 80, 80, new SdfUvRect(0.8346354f, 0.6279297f, 0.8606771f, 0.6539714f)),
            new SdfIcon(1331, (char)0x33F6, 62, 52, new SdfUvRect(0.6598307f, 0.06933594f, 0.68001306f, 0.086263016f)),
            new SdfIcon(1332, (char)0x34F6, 80, 80, new SdfUvRect(0.8613281f, 0.6279297f, 0.8873698f, 0.6539714f)),
            new SdfIcon(1333, (char)0x35F6, 53, 70, new SdfUvRect(0.5445963f, 0.9742838f, 0.561849f, 0.9970703f)),
            new SdfIcon(1334, (char)0x36F6, 50, 72, new SdfUvRect(0.860026f, 0.6038411f, 0.8763021f, 0.6272787f)),
            new SdfIcon(1335, (char)0x37F6, 48, 65, new SdfUvRect(0.9794922f, 0.55436194f, 0.9951172f, 0.5755209f)),
            new SdfIcon(1336, (char)0x38F6, 80, 81, new SdfUvRect(0.64778644f, 0.7643229f, 0.6738281f, 0.7906901f)),
            new SdfIcon(1337, (char)0x39F6, 48, 65, new SdfUvRect(0.8769531f, 0.6038411f, 0.8925781f, 0.625f)),
            new SdfIcon(1338, (char)0x3AF6, 52, 65, new SdfUvRect(0.8932291f, 0.6038411f, 0.91015625f, 0.625f)),
            new SdfIcon(1339, (char)0x3BF6, 65, 20, new SdfUvRect(0.5361328f, 0.0074869795f, 0.5572917f, 0.013997396f)),
            new SdfIcon(1340, (char)0x3CF6, 26, 61, new SdfUvRect(0.15071614f, 0.97753906f, 0.15917969f, 0.9973959f)),
            new SdfIcon(1341, (char)0x3DF6, 66, 80, new SdfUvRect(0.8880208f, 0.6279297f, 0.90950525f, 0.6539714f)),
            new SdfIcon(1342, (char)0x3EF6, 66, 80, new SdfUvRect(0.91015625f, 0.6279297f, 0.9316406f, 0.6539714f)),
            new SdfIcon(1343, (char)0x3FF6, 66, 80, new SdfUvRect(0.9322916f, 0.6279297f, 0.95377606f, 0.6539714f)),
            new SdfIcon(1344, (char)0x40F6, 66, 80, new SdfUvRect(0.95442706f, 0.6279297f, 0.9759115f, 0.6539714f)),
            new SdfIcon(1345, (char)0x41F6, 65, 80, new SdfUvRect(0.9765625f, 0.6279297f, 0.9977214f, 0.6539714f)),
            new SdfIcon(1346, (char)0x42F6, 58, 81, new SdfUvRect(0.64778644f, 0.7913411f, 0.6666667f, 0.8177084f)),
            new SdfIcon(1347, (char)0x43F6, 73, 73, new SdfUvRect(0.6204427f, 0.9742838f, 0.64420575f, 0.9980469f)),
            new SdfIcon(1348, (char)0x44F6, 80, 80, new SdfUvRect(0.64778644f, 0.8183594f, 0.6738281f, 0.84440106f)),
            new SdfIcon(1349, (char)0x45F6, 81, 65, new SdfUvRect(0.91080725f, 0.6038411f, 0.9371745f, 0.625f)),
            new SdfIcon(1350, (char)0x46F6, 32, 66, new SdfUvRect(0.98828125f, 0.26953125f, 0.99869794f, 0.29101562f)),
            new SdfIcon(1351, (char)0x47F6, 78, 81, new SdfUvRect(0.64778644f, 0.84505206f, 0.6731771f, 0.8714193f)),
            new SdfIcon(1352, (char)0x48F6, 80, 81, new SdfUvRect(0.64778644f, 0.8720703f, 0.6738281f, 0.8984375f)),
            new SdfIcon(1353, (char)0x49F6, 80, 67, new SdfUvRect(0.9378255f, 0.6038411f, 0.9638672f, 0.62565106f)),
            new SdfIcon(1354, (char)0x4AF6, 80, 67, new SdfUvRect(0.9645182f, 0.6038411f, 0.99055994f, 0.62565106f)),
            new SdfIcon(1355, (char)0x4BF6, 82, 81, new SdfUvRect(0.64778644f, 0.8990885f, 0.6744792f, 0.92545575f)),
            new SdfIcon(1356, (char)0x4CF6, 82, 81, new SdfUvRect(0.64778644f, 0.92610675f, 0.6744792f, 0.952474f)),
            new SdfIcon(1357, (char)0x4DF6, 65, 65, new SdfUvRect(0.64778644f, 0.953125f, 0.6689453f, 0.9742839f)),
            new SdfIcon(1358, (char)0x4EF6, 48, 66, new SdfUvRect(0.64778644f, 0.9749349f, 0.6634115f, 0.9964193f)),
            new SdfIcon(1359, (char)0x4FF6, 80, 79, new SdfUvRect(0.6751302f, 0.6546224f, 0.7011719f, 0.68033856f)),
            new SdfIcon(1360, (char)0x50F6, 80, 80, new SdfUvRect(0.6751302f, 0.68098956f, 0.7011719f, 0.70703125f)),
            new SdfIcon(1361, (char)0x51F6, 80, 80, new SdfUvRect(0.6751302f, 0.70768225f, 0.7011719f, 0.733724f)),
            new SdfIcon(1362, (char)0x52F6, 80, 76, new SdfUvRect(0.7018229f, 0.6546224f, 0.7278646f, 0.679362f)),
            new SdfIcon(1363, (char)0x53F6, 80, 76, new SdfUvRect(0.7285156f, 0.6546224f, 0.7545573f, 0.679362f)),
            new SdfIcon(1364, (char)0x54F6, 66, 80, new SdfUvRect(0.6751302f, 0.734375f, 0.6966146f, 0.7604167f)),
            new SdfIcon(1365, (char)0x55F6, 66, 80, new SdfUvRect(0.6751302f, 0.7610677f, 0.6966146f, 0.7871094f)),
            new SdfIcon(1366, (char)0x56F6, 81, 80, new SdfUvRect(0.7018229f, 0.68098956f, 0.7281901f, 0.70703125f)),
            new SdfIcon(1367, (char)0x57F6, 66, 66, new SdfUvRect(0.7552083f, 0.6546224f, 0.77669275f, 0.6761068f)),
            new SdfIcon(1368, (char)0x58F6, 80, 80, new SdfUvRect(0.6751302f, 0.7877604f, 0.7011719f, 0.8138021f)),
            new SdfIcon(1369, (char)0x59F6, 66, 66, new SdfUvRect(0.77734375f, 0.6546224f, 0.7988281f, 0.6761068f)),
            new SdfIcon(1370, (char)0x5AF6, 80, 80, new SdfUvRect(0.6751302f, 0.8144531f, 0.7011719f, 0.8404948f)),
            new SdfIcon(1371, (char)0x5BF6, 70, 82, new SdfUvRect(0.6751302f, 0.8411458f, 0.6979167f, 0.86783856f)),
            new SdfIcon(1372, (char)0x5DF6, 80, 80, new SdfUvRect(0.6751302f, 0.86848956f, 0.7011719f, 0.89453125f)),
            new SdfIcon(1373, (char)0x5EF6, 80, 80, new SdfUvRect(0.6751302f, 0.89518225f, 0.7011719f, 0.921224f)),
            new SdfIcon(1374, (char)0x5CF6, 81, 58, new SdfUvRect(0.7994791f, 0.6546224f, 0.8258464f, 0.6735026f)),
            new SdfIcon(1375, (char)0x5FF6, 80, 80, new SdfUvRect(0.6751302f, 0.921875f, 0.7011719f, 0.9479167f)),
            new SdfIcon(1376, (char)0x60F6, 80, 78, new SdfUvRect(0.8264974f, 0.6546224f, 0.85253906f, 0.68001306f)),
            new SdfIcon(1377, (char)0x61F6, 80, 52, new SdfUvRect(0.68066406f, 0.06933594f, 0.70670575f, 0.086263016f)),
            new SdfIcon(1378, (char)0x62F6, 73, 80, new SdfUvRect(0.6751302f, 0.9485677f, 0.69889325f, 0.9746094f)),
            new SdfIcon(1379, (char)0x63F6, 83, 82, new SdfUvRect(0.7018229f, 0.70768225f, 0.7288412f, 0.734375f)),
            new SdfIcon(1380, (char)0x64F6, 82, 82, new SdfUvRect(0.7018229f, 0.735026f, 0.7285156f, 0.76171875f)),
            new SdfIcon(1381, (char)0x65F6, 82, 79, new SdfUvRect(0.85319006f, 0.6546224f, 0.8798828f, 0.68033856f)),
            new SdfIcon(1382, (char)0x66F6, 80, 80, new SdfUvRect(0.7288411f, 0.68098956f, 0.7548828f, 0.70703125f)),
            new SdfIcon(1383, (char)0x67F6, 71, 80, new SdfUvRect(0.7555338f, 0.68098956f, 0.7786459f, 0.70703125f)),
            new SdfIcon(1384, (char)0x68F6, 63, 80, new SdfUvRect(0.7792969f, 0.68098956f, 0.7998047f, 0.70703125f)),
            new SdfIcon(1385, (char)0x69F6, 80, 80, new SdfUvRect(0.8004557f, 0.68098956f, 0.82649744f, 0.70703125f)),
            new SdfIcon(1386, (char)0x6AF6, 81, 73, new SdfUvRect(0.8805338f, 0.6546224f, 0.90690106f, 0.67838544f)),
            new SdfIcon(1387, (char)0x6BF6, 80, 65, new SdfUvRect(0.90755206f, 0.6546224f, 0.93359375f, 0.67578125f)),
            new SdfIcon(1388, (char)0x6CF6, 80, 80, new SdfUvRect(0.82714844f, 0.68098956f, 0.8531901f, 0.70703125f)),
            new SdfIcon(1389, (char)0x6DF6, 80, 80, new SdfUvRect(0.8538411f, 0.68098956f, 0.8798828f, 0.70703125f)),
            new SdfIcon(1390, (char)0x6EF6, 80, 72, new SdfUvRect(0.93424475f, 0.6546224f, 0.9602865f, 0.67805994f)),
            new SdfIcon(1391, (char)0x6FF6, 80, 72, new SdfUvRect(0.9609375f, 0.6546224f, 0.9869792f, 0.67805994f)),
            new SdfIcon(1392, (char)0x70F6, 80, 80, new SdfUvRect(0.8805338f, 0.68098956f, 0.90657556f, 0.70703125f)),
            new SdfIcon(1393, (char)0x71F6, 74, 80, new SdfUvRect(0.90722656f, 0.68098956f, 0.9313151f, 0.70703125f)),
            new SdfIcon(1394, (char)0x72F6, 80, 80, new SdfUvRect(0.9319661f, 0.68098956f, 0.9580078f, 0.70703125f)),
            new SdfIcon(1395, (char)0x73F6, 80, 80, new SdfUvRect(0.9586588f, 0.68098956f, 0.98470056f, 0.70703125f)),
            new SdfIcon(1396, (char)0x74F6, 80, 80, new SdfUvRect(0.7018229f, 0.76236975f, 0.7278646f, 0.7884115f)),
            new SdfIcon(1397, (char)0x75F6, 78, 78, new SdfUvRect(0.7018229f, 0.7890625f, 0.72721356f, 0.8144531f)),
            new SdfIcon(1398, (char)0x76F6, 80, 69, new SdfUvRect(0.6751302f, 0.9752604f, 0.7011719f, 0.9977214f)),
            new SdfIcon(1399, (char)0x77F6, 80, 66, new SdfUvRect(0.7018229f, 0.8151041f, 0.7278646f, 0.83658856f)),
            new SdfIcon(1400, (char)0x78F6, 80, 69, new SdfUvRect(0.7018229f, 0.83723956f, 0.7278646f, 0.85970056f)),
            new SdfIcon(1401, (char)0x79F6, 80, 73, new SdfUvRect(0.7018229f, 0.86035156f, 0.7278646f, 0.8841146f)),
            new SdfIcon(1402, (char)0x7AF6, 80, 76, new SdfUvRect(0.7018229f, 0.8847656f, 0.7278646f, 0.90950525f)),
            new SdfIcon(1403, (char)0x7BF6, 80, 80, new SdfUvRect(0.7018229f, 0.91015625f, 0.7278646f, 0.93619794f)),
            new SdfIcon(1404, (char)0x7CF6, 80, 58, new SdfUvRect(0.7018229f, 0.93684894f, 0.7278646f, 0.9557292f)),
            new SdfIcon(1405, (char)0x7DF6, 80, 53, new SdfUvRect(0.70735675f, 0.06933594f, 0.73339844f, 0.08658854f)),
            new SdfIcon(1406, (char)0x7EF6, 80, 80, new SdfUvRect(0.7018229f, 0.9563802f, 0.7278646f, 0.9824219f)),
            new SdfIcon(1407, (char)0x80F6, 82, 82, new SdfUvRect(0.7294922f, 0.70768225f, 0.75618494f, 0.734375f)),
            new SdfIcon(1408, (char)0x81F6, 82, 82, new SdfUvRect(0.75683594f, 0.70768225f, 0.7835287f, 0.734375f)),
            new SdfIcon(1409, (char)0x82F6, 65, 80, new SdfUvRect(0.7841797f, 0.70768225f, 0.80533856f, 0.733724f)),
            new SdfIcon(1410, (char)0x83F6, 80, 76, new SdfUvRect(0.80598956f, 0.70768225f, 0.83203125f, 0.7324219f)),
            new SdfIcon(1411, (char)0x84F6, 80, 76, new SdfUvRect(0.83268225f, 0.70768225f, 0.858724f, 0.7324219f)),
            new SdfIcon(1412, (char)0x85F6, 80, 82, new SdfUvRect(0.859375f, 0.70768225f, 0.8854167f, 0.734375f)),
            new SdfIcon(1413, (char)0x86F6, 80, 80, new SdfUvRect(0.8860677f, 0.70768225f, 0.9121094f, 0.733724f)),
            new SdfIcon(1414, (char)0x87F6, 80, 80, new SdfUvRect(0.9127604f, 0.70768225f, 0.9388021f, 0.733724f)),
            new SdfIcon(1415, (char)0x88F6, 62, 81, new SdfUvRect(0.9394531f, 0.70768225f, 0.95963544f, 0.7340495f)),
            new SdfIcon(1416, (char)0x89F6, 62, 81, new SdfUvRect(0.96028644f, 0.70768225f, 0.98046875f, 0.7340495f)),
            new SdfIcon(1417, (char)0x8AF6, 34, 59, new SdfUvRect(0.9876302f, 0.6546224f, 0.99869794f, 0.6738281f)),
            new SdfIcon(1418, (char)0x8BF6, 80, 73, new SdfUvRect(0.7294922f, 0.735026f, 0.7555339f, 0.75878906f)),
            new SdfIcon(1419, (char)0x8CF6, 80, 73, new SdfUvRect(0.7294922f, 0.75944006f, 0.7555339f, 0.7832031f)),
            new SdfIcon(1420, (char)0x8DF6, 34, 59, new SdfUvRect(0.98535156f, 0.68098956f, 0.9964193f, 0.7001953f)),
            new SdfIcon(1421, (char)0x8EF6, 80, 73, new SdfUvRect(0.7294922f, 0.7838541f, 0.7555339f, 0.8076172f)),
            new SdfIcon(1422, (char)0x8FF6, 80, 73, new SdfUvRect(0.7294922f, 0.8082682f, 0.7555339f, 0.83203125f)),
            new SdfIcon(1423, (char)0x90F6, 34, 59, new SdfUvRect(0.98111975f, 0.70768225f, 0.9921875f, 0.72688806f)),
            new SdfIcon(1424, (char)0x91F6, 80, 73, new SdfUvRect(0.7294922f, 0.83268225f, 0.7555339f, 0.8564453f)),
            new SdfIcon(1425, (char)0x92F6, 80, 73, new SdfUvRect(0.7294922f, 0.8570963f, 0.7555339f, 0.8808594f)),
            new SdfIcon(1426, (char)0x93F6, 80, 73, new SdfUvRect(0.7294922f, 0.8815104f, 0.7555339f, 0.90527344f)),
            new SdfIcon(1427, (char)0x94F6, 80, 73, new SdfUvRect(0.7294922f, 0.90592444f, 0.7555339f, 0.9296875f)),
            new SdfIcon(1428, (char)0x95F6, 34, 59, new SdfUvRect(0.7294922f, 0.9303385f, 0.74055994f, 0.9495443f)),
            new SdfIcon(1429, (char)0x96F6, 81, 74, new SdfUvRect(0.7561849f, 0.735026f, 0.7825521f, 0.7591146f)),
            new SdfIcon(1430, (char)0x97F6, 81, 74, new SdfUvRect(0.7832031f, 0.735026f, 0.8095703f, 0.7591146f)),
            new SdfIcon(1431, (char)0x98F6, 34, 59, new SdfUvRect(0.74121094f, 0.9303385f, 0.7522787f, 0.9495443f)),
            new SdfIcon(1432, (char)0x99F6, 80, 73, new SdfUvRect(0.7294922f, 0.9501953f, 0.7555339f, 0.9739584f)),
            new SdfIcon(1433, (char)0x9AF6, 80, 73, new SdfUvRect(0.7294922f, 0.9746094f, 0.7555339f, 0.99837244f)),
            new SdfIcon(1434, (char)0x9BF6, 80, 80, new SdfUvRect(0.7561849f, 0.7597656f, 0.78222656f, 0.7858073f)),
            new SdfIcon(1435, (char)0x9CF6, 80, 80, new SdfUvRect(0.7561849f, 0.7864583f, 0.78222656f, 0.8125f)),
            new SdfIcon(1436, (char)0x9DF6, 80, 80, new SdfUvRect(0.7561849f, 0.813151f, 0.78222656f, 0.83919275f)),
            new SdfIcon(1437, (char)0x9EF6, 82, 48, new SdfUvRect(0.73404944f, 0.06933594f, 0.7607422f, 0.08496094f)),
            new SdfIcon(1438, (char)0x9FF6, 80, 76, new SdfUvRect(0.7561849f, 0.83984375f, 0.78222656f, 0.8645834f)),
            new SdfIcon(1439, (char)0xA0F6, 80, 76, new SdfUvRect(0.7561849f, 0.8652344f, 0.78222656f, 0.889974f)),
            new SdfIcon(1440, (char)0xA1F6, 80, 60, new SdfUvRect(0.8102213f, 0.735026f, 0.83626306f, 0.7545573f)),
            new SdfIcon(1441, (char)0xA2F6, 34, 59, new SdfUvRect(0.83691406f, 0.735026f, 0.8479818f, 0.7542318f)),
            new SdfIcon(1442, (char)0xA3F6, 34, 59, new SdfUvRect(0.8486328f, 0.735026f, 0.85970056f, 0.7542318f)),
            new SdfIcon(1443, (char)0xA4F6, 80, 80, new SdfUvRect(0.7561849f, 0.890625f, 0.78222656f, 0.9166667f)),
            new SdfIcon(1444, (char)0xA5F6, 80, 80, new SdfUvRect(0.7561849f, 0.9173177f, 0.78222656f, 0.9433594f)),
            new SdfIcon(1445, (char)0xA6F6, 80, 80, new SdfUvRect(0.7561849f, 0.9440104f, 0.78222656f, 0.9700521f)),
            new SdfIcon(1446, (char)0xA7F6, 80, 37, new SdfUvRect(0.76171875f, 0.00032552084f, 0.78776044f, 0.012369792f)),
            new SdfIcon(1447, (char)0xA8F6, 48, 80, new SdfUvRect(0.7561849f, 0.9707031f, 0.77180994f, 0.9967448f)),
            new SdfIcon(1448, (char)0xA9F6, 81, 67, new SdfUvRect(0.86035156f, 0.735026f, 0.88671875f, 0.75683594f)),
            new SdfIcon(1449, (char)0xAAF6, 72, 72, new SdfUvRect(0.88736975f, 0.735026f, 0.9108073f, 0.75846356f)),
            new SdfIcon(1450, (char)0xABF6, 80, 48, new SdfUvRect(0.7613932f, 0.06933594f, 0.78743494f, 0.08496094f)),
            new SdfIcon(1451, (char)0xACF6, 80, 48, new SdfUvRect(0.78808594f, 0.06933594f, 0.8141276f, 0.08496094f)),
            new SdfIcon(1452, (char)0xADF6, 80, 80, new SdfUvRect(0.78287756f, 0.7597656f, 0.8089193f, 0.7858073f)),
            new SdfIcon(1453, (char)0xAEF6, 80, 80, new SdfUvRect(0.8095703f, 0.7597656f, 0.835612f, 0.7858073f)),
            new SdfIcon(1454, (char)0xAFF6, 79, 80, new SdfUvRect(0.836263f, 0.7597656f, 0.8619792f, 0.7858073f)),
            new SdfIcon(1455, (char)0xB0F6, 58, 53, new SdfUvRect(0.8147786f, 0.06933594f, 0.8336589f, 0.08658854f)),
            new SdfIcon(1456, (char)0xB1F6, 80, 76, new SdfUvRect(0.8626302f, 0.7597656f, 0.8886719f, 0.78450525f)),
            new SdfIcon(1457, (char)0xB2F6, 82, 81, new SdfUvRect(0.78287756f, 0.7864583f, 0.8095703f, 0.81282556f)),
            new SdfIcon(1458, (char)0xB3F6, 80, 81, new SdfUvRect(0.78287756f, 0.81347656f, 0.8089193f, 0.83984375f)),
            new SdfIcon(1459, (char)0xB4F6, 82, 81, new SdfUvRect(0.78287756f, 0.84049475f, 0.8095703f, 0.866862f)),
            new SdfIcon(1460, (char)0xB5F6, 80, 81, new SdfUvRect(0.78287756f, 0.867513f, 0.8089193f, 0.89388025f)),
            new SdfIcon(1461, (char)0xB6F6, 34, 59, new SdfUvRect(0.9114583f, 0.735026f, 0.92252606f, 0.7542318f)),
            new SdfIcon(1462, (char)0xB7F6, 82, 81, new SdfUvRect(0.78287756f, 0.89453125f, 0.8095703f, 0.92089844f)),
            new SdfIcon(1463, (char)0xB8F6, 80, 81, new SdfUvRect(0.78287756f, 0.92154944f, 0.8089193f, 0.9479167f)),
            new SdfIcon(1464, (char)0xB9F6, 82, 82, new SdfUvRect(0.78287756f, 0.9485677f, 0.8095703f, 0.97526044f)),
            new SdfIcon(1465, (char)0xBAF6, 82, 81, new SdfUvRect(0.8102213f, 0.7864583f, 0.83691406f, 0.81282556f)),
            new SdfIcon(1466, (char)0xBBF6, 80, 81, new SdfUvRect(0.83756506f, 0.7864583f, 0.8636068f, 0.81282556f)),
            new SdfIcon(1467, (char)0xBCF6, 82, 82, new SdfUvRect(0.8102213f, 0.81347656f, 0.83691406f, 0.8401693f)),
            new SdfIcon(1468, (char)0xBDF6, 80, 82, new SdfUvRect(0.8102213f, 0.8408203f, 0.83626306f, 0.86751306f)),
            new SdfIcon(1469, (char)0xBEF6, 82, 81, new SdfUvRect(0.8642578f, 0.7864583f, 0.89095056f, 0.81282556f)),
            new SdfIcon(1470, (char)0xBFF6, 80, 81, new SdfUvRect(0.89160156f, 0.7864583f, 0.91764325f, 0.81282556f)),
            new SdfIcon(1471, (char)0xC0F6, 80, 80, new SdfUvRect(0.8893229f, 0.7597656f, 0.9153646f, 0.7858073f)),
            new SdfIcon(1472, (char)0xC1F6, 80, 80, new SdfUvRect(0.9160156f, 0.7597656f, 0.9420573f, 0.7858073f)),
            new SdfIcon(1473, (char)0xC2F6, 34, 59, new SdfUvRect(0.92317706f, 0.735026f, 0.9342448f, 0.7542318f)),
            new SdfIcon(1474, (char)0xC3F6, 80, 73, new SdfUvRect(0.9348958f, 0.735026f, 0.9609375f, 0.75878906f)),
            new SdfIcon(1475, (char)0xC4F6, 80, 73, new SdfUvRect(0.9615885f, 0.735026f, 0.98763025f, 0.75878906f)),
            new SdfIcon(1476, (char)0xC5F6, 80, 72, new SdfUvRect(0.9427083f, 0.7597656f, 0.96875f, 0.7832031f)),
            new SdfIcon(1477, (char)0xC6F6, 80, 58, new SdfUvRect(0.969401f, 0.7597656f, 0.99544275f, 0.7786459f)),
            new SdfIcon(1478, (char)0xC7F6, 80, 58, new SdfUvRect(0.78287756f, 0.97591144f, 0.8089193f, 0.9947917f)),
            new SdfIcon(1479, (char)0xC8F6, 80, 58, new SdfUvRect(0.91829425f, 0.7864583f, 0.94433594f, 0.80533856f)),
            new SdfIcon(1480, (char)0xC9F6, 80, 58, new SdfUvRect(0.94498694f, 0.7864583f, 0.9710287f, 0.80533856f)),
            new SdfIcon(1481, (char)0xCAF6, 80, 58, new SdfUvRect(0.9716797f, 0.7864583f, 0.9977214f, 0.80533856f)),
            new SdfIcon(1482, (char)0xCBF6, 80, 58, new SdfUvRect(0.8102213f, 0.86816406f, 0.83626306f, 0.8870443f)),
            new SdfIcon(1483, (char)0xCCF6, 72, 80, new SdfUvRect(0.8102213f, 0.8876953f, 0.8336589f, 0.913737f)),
            new SdfIcon(1484, (char)0xCDF6, 80, 73, new SdfUvRect(0.8102213f, 0.914388f, 0.83626306f, 0.93815106f)),
            new SdfIcon(1485, (char)0xCEF6, 80, 72, new SdfUvRect(0.8102213f, 0.93880206f, 0.83626306f, 0.9622396f)),
            new SdfIcon(1486, (char)0xCFF6, 80, 72, new SdfUvRect(0.8102213f, 0.9628906f, 0.83626306f, 0.9863281f)),
            new SdfIcon(1487, (char)0xD0F6, 80, 73, new SdfUvRect(0.83756506f, 0.81347656f, 0.8636068f, 0.8372396f)),
            new SdfIcon(1488, (char)0xD1F6, 80, 72, new SdfUvRect(0.8642578f, 0.81347656f, 0.8902995f, 0.83691406f)),
            new SdfIcon(1489, (char)0xD2F6, 80, 72, new SdfUvRect(0.8909505f, 0.81347656f, 0.9169922f, 0.83691406f)),
            new SdfIcon(1490, (char)0xD3F6, 80, 73, new SdfUvRect(0.9176432f, 0.81347656f, 0.94368494f, 0.8372396f)),
            new SdfIcon(1491, (char)0xD4F6, 80, 81, new SdfUvRect(0.83756506f, 0.8378906f, 0.8636068f, 0.8642578f)),
            new SdfIcon(1492, (char)0xD5F6, 80, 80, new SdfUvRect(0.83756506f, 0.8649088f, 0.8636068f, 0.89095056f)),
            new SdfIcon(1493, (char)0xD6F6, 80, 40, new SdfUvRect(0.78841144f, 0.00032552084f, 0.8144531f, 0.0133463545f)),
            new SdfIcon(1494, (char)0xD7F6, 80, 40, new SdfUvRect(0.8151041f, 0.00032552084f, 0.8411459f, 0.0133463545f)),
            new SdfIcon(1495, (char)0xD8F6, 80, 40, new SdfUvRect(0.8417969f, 0.00032552084f, 0.86783856f, 0.0133463545f)),
            new SdfIcon(1496, (char)0xD9F6, 80, 40, new SdfUvRect(0.86848956f, 0.00032552084f, 0.89453125f, 0.0133463545f)),
            new SdfIcon(1497, (char)0xDAF6, 80, 40, new SdfUvRect(0.89518225f, 0.00032552084f, 0.921224f, 0.0133463545f)),
            new SdfIcon(1498, (char)0xDBF6, 44, 80, new SdfUvRect(0.83756506f, 0.89160156f, 0.85188806f, 0.91764325f)),
            new SdfIcon(1499, (char)0xDCF6, 44, 80, new SdfUvRect(0.83756506f, 0.91829425f, 0.85188806f, 0.94433594f)),
            new SdfIcon(1500, (char)0xDDF6, 58, 80, new SdfUvRect(0.83756506f, 0.94498694f, 0.8564453f, 0.9710287f)),
            new SdfIcon(1501, (char)0xDEF6, 80, 40, new SdfUvRect(0.921875f, 0.00032552084f, 0.9479167f, 0.0133463545f)),
            new SdfIcon(1502, (char)0xDFF6, 80, 76, new SdfUvRect(0.83756506f, 0.9716797f, 0.8636068f, 0.9964193f)),
            new SdfIcon(1503, (char)0xE0F6, 34, 59, new SdfUvRect(0.98828125f, 0.735026f, 0.999349f, 0.7542318f)),
            new SdfIcon(1504, (char)0xE1F6, 80, 40, new SdfUvRect(0.9485677f, 0.00032552084f, 0.9746094f, 0.0133463545f)),
            new SdfIcon(1505, (char)0xE2F6, 80, 72, new SdfUvRect(0.94433594f, 0.81347656f, 0.9703776f, 0.83691406f)),
            new SdfIcon(1506, (char)0xE3F6, 80, 58, new SdfUvRect(0.9710286f, 0.81347656f, 0.9970703f, 0.8323568f)),
            new SdfIcon(1507, (char)0xE4F6, 42, 80, new SdfUvRect(0.8642578f, 0.8378906f, 0.8779297f, 0.8639323f)),
            new SdfIcon(1508, (char)0xE5F6, 42, 80, new SdfUvRect(0.8785807f, 0.8378906f, 0.8922526f, 0.8639323f)),
            new SdfIcon(1509, (char)0xE6F6, 80, 76, new SdfUvRect(0.8929036f, 0.8378906f, 0.9189453f, 0.86263025f)),
            new SdfIcon(1510, (char)0xE7F6, 80, 76, new SdfUvRect(0.9195963f, 0.8378906f, 0.94563806f, 0.86263025f)),
            new SdfIcon(1511, (char)0xE8F6, 80, 69, new SdfUvRect(0.94628906f, 0.8378906f, 0.97233075f, 0.86035156f)),
            new SdfIcon(1512, (char)0xE9F6, 80, 69, new SdfUvRect(0.97298175f, 0.8378906f, 0.99902344f, 0.86035156f)),
            new SdfIcon(1513, (char)0xEAF6, 80, 72, new SdfUvRect(0.8642578f, 0.8645833f, 0.8902995f, 0.8880209f)),
            new SdfIcon(1514, (char)0xEBF6, 80, 66, new SdfUvRect(0.8909505f, 0.8645833f, 0.9169922f, 0.88606775f)),
            new SdfIcon(1515, (char)0xECF6, 80, 66, new SdfUvRect(0.9176432f, 0.8645833f, 0.94368494f, 0.88606775f)),
            new SdfIcon(1516, (char)0xEDF6, 34, 59, new SdfUvRect(0.85253906f, 0.89160156f, 0.8636068f, 0.9108073f)),
            new SdfIcon(1517, (char)0xEEF6, 34, 59, new SdfUvRect(0.94433594f, 0.8645833f, 0.9554037f, 0.88378906f)),
            new SdfIcon(1518, (char)0xEFF6, 80, 61, new SdfUvRect(0.9560547f, 0.8645833f, 0.9820964f, 0.8844401f)),
            new SdfIcon(1519, (char)0xF0F6, 80, 61, new SdfUvRect(0.8642578f, 0.8886719f, 0.8902995f, 0.9085287f)),
            new SdfIcon(1520, (char)0xF1F6, 44, 80, new SdfUvRect(0.8642578f, 0.9091797f, 0.87858075f, 0.9352214f)),
            new SdfIcon(1521, (char)0xF2F6, 44, 80, new SdfUvRect(0.8642578f, 0.9358724f, 0.87858075f, 0.96191406f)),
            new SdfIcon(1522, (char)0xF3F6, 80, 48, new SdfUvRect(0.8343099f, 0.06933594f, 0.86035156f, 0.08496094f)),
            new SdfIcon(1523, (char)0xF4F6, 80, 48, new SdfUvRect(0.86100256f, 0.06933594f, 0.8870443f, 0.08496094f)),
            new SdfIcon(1524, (char)0xF5F6, 80, 58, new SdfUvRect(0.8642578f, 0.96256506f, 0.8902995f, 0.9814453f)),
            new SdfIcon(1525, (char)0xF6F6, 80, 58, new SdfUvRect(0.8909505f, 0.8886719f, 0.9169922f, 0.9075521f)),
            new SdfIcon(1526, (char)0xF7F6, 81, 65, new SdfUvRect(0.8909505f, 0.9082031f, 0.91731775f, 0.929362f)),
            new SdfIcon(1527, (char)0xF8F6, 66, 80, new SdfUvRect(0.8909505f, 0.930013f, 0.91243494f, 0.9560547f)),
            new SdfIcon(1528, (char)0xF9F6, 66, 80, new SdfUvRect(0.8909505f, 0.9567057f, 0.91243494f, 0.98274744f)),
            new SdfIcon(1529, (char)0xFAF6, 66, 80, new SdfUvRect(0.91796875f, 0.9082031f, 0.9394531f, 0.9342448f)),
            new SdfIcon(1530, (char)0xFBF6, 66, 80, new SdfUvRect(0.9401041f, 0.9082031f, 0.96158856f, 0.9342448f)),
            new SdfIcon(1531, (char)0xFCF6, 80, 40, new SdfUvRect(0.8876953f, 0.06933594f, 0.913737f, 0.082356766f)),
            new SdfIcon(1532, (char)0xFDF6, 80, 73, new SdfUvRect(0.96223956f, 0.9082031f, 0.98828125f, 0.9319662f)),
            new SdfIcon(1533, (char)0xFEF6, 80, 73, new SdfUvRect(0.91796875f, 0.9348958f, 0.94401044f, 0.9586589f)),
            new SdfIcon(1534, (char)0xFFF6, 80, 73, new SdfUvRect(0.91796875f, 0.9593099f, 0.94401044f, 0.98307294f))
        };

        private static int iDontKnowWhatElseToDoAndImTiredOfUnity;

        private static Material Material
        {
            get
            {
                if (sdfMaterial == null)
                {
                    const string key           = "odin_sdf_material2";
                    int          matInstanceId = SessionState.GetInt(key, 0); // survive assembly reloads.

                    if (matInstanceId != 0)
                        sdfMaterial = EditorUtility.InstanceIDToObject(matInstanceId) as Material;

                    if (sdfMaterial == null)
                    {
                        var shader = ShaderUtil.CreateShaderAsset(SdfIcons.shader);
                        sdfMaterial = new Material(shader);

                        UnityEngine.Object.DontDestroyOnLoad(shader);
                        UnityEngine.Object.DontDestroyOnLoad(sdfMaterial);
                        sdfMaterial.hideFlags = HideFlags.DontUnloadUnusedAsset;

                        SessionState.SetInt(key, sdfMaterial.GetInstanceID());
                    }

                    UnityEngine.Object.DontDestroyOnLoad(sdfMaterial);

                    // Note that we are using global shader properties because changing
                    // shader properties of a material instance, causes the scene view to repaint. In older versions of Unit at least.

                    // Also if we use instance properties, things break.

                    // You can test this by creating an instance of a prefab in a scene, add a component to the instance, apply changes, and boom. The textures now disappear. And we would need to set it again.
                    // But globals does not suffer from this problem.
                    Shader.SetGlobalColor("_SirenixOdin_Color", new Color(1, 1, 1, 1));
                    Shader.SetGlobalColor("_SirenixOdin_BgColor", new Color(0, 0, 0, 0));
                    Shader.SetGlobalVector("_SirenixOdin_Uv", new Vector4(0, 0, 1, 1));
                    Shader.SetGlobalTexture("_SirenixOdin_SdfTex", SdfTexture);

                    _uv      = new Vector4(0, 0, 1, 1);
                    _color   = new Color(1, 1, 1, 1);
                    _bgColor = new Color(0, 0, 0, 0);
                }

                return sdfMaterial;
            }
        }

        private static Texture2D SdfTexture
        {
            get
            {
                if (sdfTexture == null)
                {
					var sdfPath = SirenixAssetPaths.OdinPath + "Assets/Editor/SdfIconAtlas.png";
                    sdfTexture  = AssetDatabase.LoadAssetAtPath<Texture2D>(sdfPath);
					
					if (sdfTexture == null) // First time the unity project imported, this texture will not load. But after that, it never happens again.
                    {
                        sdfMaterial = null;
                        return null;
					}
					
                    sdfTexture.hideFlags = HideFlags.DontUnloadUnusedAsset;
                    UnityEngine.Object.DontDestroyOnLoad(sdfTexture);
                }

                return sdfTexture;
            }
        }

        [InitializeOnLoadMethod]
        private static void FixBug()
        {
            // Fix: In order for SdfIcons to work the first couple of frames, we need to initialize the material early.
            // It aparently doesn't work right away.
            sdfMaterial   = Material;
            sdfTexture = SdfTexture;

            if (sdfTexture == null)
                sdfMaterial = null;
        }

        public static Texture2D CreateTransparentIconTexture(SdfIconType iconType, Color color, int width, int height, int padding)
        {
            var tex = SdfTexture;
            if (tex == null)
                return null;

            var icon  = AllIcons[(int)iconType];
            var rect  = new Rect(0, 0, width, height);
            var s     = Math.Min(rect.width, rect.height);
            var ratio = rect.width / icon.Width;
            rect = rect.AlignCenter(s + sdfPadding * ratio, s + sdfPadding * ratio);

            if (icon.Width < icon.Height)
            {
                ratio = icon.Width / (float)icon.Height;
                var newWidth = rect.width * ratio;
                rect.x     += (rect.width - newWidth) * 0.5f;
                rect.width =  newWidth;
            }
            else
            {
                ratio = icon.Height / (float)icon.Width;
                var newHeight = rect.height * ratio;
                rect.y      += (rect.height - newHeight) * 0.5f;
                rect.height =  newHeight;
            }

            SetColor(color);
            SetBgColor(default);
            SetUv(new Vector4(icon.Uv.X, icon.Uv.Y, icon.Uv.Width, icon.Uv.Height));

            var prev = RenderTexture.active;
            var rt   = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 8);
            RenderTexture.active = rt;
            GL.Clear(false, true, new Color(0, 0, 0, 0));
            var prevsRGB = GL.sRGBWrite;
            GL.sRGBWrite = false;
            Graphics.Blit(null, rt, Material);
            var clone = new Texture2D(width, height, TextureFormat.ARGB32, false, false);
            clone.filterMode = FilterMode.Bilinear;
            clone.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            clone.Apply();
            RenderTexture.active = prev;
            RenderTexture.ReleaseTemporary(rt);
            GL.sRGBWrite = prevsRGB;
            return clone;
        }


        public static void DrawIcon(Rect rect, SdfIcon icon, Color color, Color bgColor)
        {
            if (icon.Index <= 0)
                return;

            if (Event.current.type != EventType.Repaint)
                return;

            if (rect.width <= 0 || rect.height <= 0)
                return;

            var tex = SdfTexture;
            if (tex == null)
                return;

            var s     = Math.Min(rect.width, rect.height);
            var ratio = rect.width / icon.Width;
            rect = rect.AlignCenter(s + sdfPadding * ratio, s + sdfPadding * ratio);

            if (icon.Width < icon.Height)
            {
                ratio = icon.Width / (float)icon.Height;
                var newWidth = rect.width * ratio;
                rect.x     += (rect.width - newWidth) * 0.5f;
                rect.width =  newWidth;
            }
            else
            {
                ratio = icon.Height / (float)icon.Width;
                var newHeight = rect.height * ratio;
                rect.y      += (rect.height - newHeight) * 0.5f;
                rect.height =  newHeight;
            }

            var unclipped = GUIClipInfo.Unclip(rect);
            var clipRect  = GUIClipInfo.TopMostRect;

            if (!unclipped.Overlaps(clipRect))
                return;

            var uvX      = icon.Uv.X;
            var uvY      = icon.Uv.Y;
            var uvWidth  = icon.Uv.Width;
            var uvHeight = icon.Uv.Height;
            var clipped  = rect;

            var clipTop = clipRect.y - unclipped.y;
            if (clipTop > 0)
            {
                var amount = clipTop / rect.height;
                uvY            += icon.Uv.Height * amount;
                uvHeight       -= icon.Uv.Height * amount;
                clipped.y      += clipTop;
                clipped.height -= clipTop;
            }

            var clipBottom = unclipped.yMax - clipRect.yMax;
            if (clipBottom > 0)
            {
                var amount = clipBottom / rect.height;
                uvHeight       -= uvHeight * amount;
                clipped.height -= clipBottom;
            }

            var clipLeft = clipRect.x - unclipped.x;
            if (clipLeft > 0)
            {
                var amount = clipLeft / rect.width;
                uvX           += icon.Uv.Width * amount;
                uvWidth       -= icon.Uv.Width * amount;
                clipped.x     += clipLeft;
                clipped.width -= clipLeft;
            }

            var clipRight = unclipped.xMax - clipRect.xMax;
            if (clipRight > 0)
            {
                var amount = clipRight / rect.width;
                uvWidth       -= uvWidth * amount;
                clipped.width -= clipRight;
            }


            color *= GUI.color;

            if (!GUI.enabled)
            {
                color.a *= 0.4f;
            }

            SetColor(color);
            SetBgColor(bgColor);
            SetUv(new Vector4(uvX, uvY, uvWidth, uvHeight));

            Graphics.DrawTexture(clipped, tex, Material);
        }

        public static void DrawIcon(Rect rect, SdfIcon icon, Color color)
        {
            DrawIcon(rect, icon, color, new Color(0, 0, 0, 0));
        }

        public static void DrawIcon(Rect rect, SdfIconType icon, Color color, Color bgColor)
        {
            DrawIcon(rect, AllIcons[(int)icon], color, bgColor);
        }

        public static void DrawIcon(Rect rect, SdfIconType icon, Color color)
        {
            DrawIcon(rect, AllIcons[(int)icon], color, new Color(0, 0, 0, 0));
        }

        public static void DrawIcon(Rect rect, SdfIconType icon)
        {
            DrawIcon(rect, AllIcons[(int)icon], EditorStyles.label.normal.textColor, new Color(0, 0, 0, 0));
        }

        // Note that we are using global shader properties because changing
        // shader properties of a material instance, causes the scene view to repaint.

        private static void SetColor(Color color)
        {
            if (_color != color)
            {
                Shader.SetGlobalColor("_SirenixOdin_Color", color);
                Material.SetColor("_SirenixOdin_Color", color);
                _color = color;
            }
        }

        private static void SetBgColor(Color bgColor)
        {
            if (_bgColor != bgColor)
            {
                Shader.SetGlobalColor("_SirenixOdin_BgColor", bgColor);
                Material.SetColor("_SirenixOdin_BgColor", bgColor);
                _bgColor = bgColor;
            }
        }

        private static void SetUv(Vector4 uv)
        {
            if (_uv != uv)
            {
                Shader.SetGlobalVector("_SirenixOdin_Uv", uv);
                Material.SetVector("_SirenixOdin_Uv", uv);
                _uv = uv;
            }
        }
    }

    public struct SdfIcon
    {
        public int Index;
        public char Char;
        public int Width;
        public int Height;
        public SdfUvRect Uv;

        public string Name
        {
            get { return Enum.GetName(typeof(SdfIconType), this.Index); }
        }

        public SdfIcon(int index, char @char, int width, int height, SdfUvRect uv)
        {
            Index  = index;
            Char   = @char;
            Width  = width;
            Height = height;
            Uv     = uv;
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct SdfUvRect
    {
        public static readonly SdfUvRect Identity = new SdfUvRect(0, 0, 1, 1);

        [FieldOffset(0)]
        public Vector4 V4;

        [FieldOffset(0)]
        public Vector2 TL;

        [FieldOffset(0)]
        public float Left, X;

        [FieldOffset(4)]
        public float Top, Y;

        [FieldOffset(8)]
        public float Right, Z;

        [FieldOffset(12)]
        public float Bottom, W;

        public Vector2 Center
        {
            get { return new Vector2((Left + Right) * 0.5f, (Top + Bottom) * 0.5f); }
        }

        public SdfUvRect(Vector4 v4) : this()
        {
            this.V4 = v4;
        }

        public SdfUvRect(float left, float top, float right, float bottom) : this()
        {
            this.Left   = left;
            this.Top    = top;
            this.Right  = right;
            this.Bottom = bottom;
        }

        public float Width
        {
            get { return this.Right - Left; }
            set { this.Right = Left + value; }
        }

        public float Height
        {
            get { return this.Bottom - Top; }
            set { this.Bottom = Top + value; }
        }

        public static Vector2 operator *(SdfUvRect region, Vector2 uv)
        {
            return new Vector2(region.X + uv.x * region.Width, region.Y + uv.y * region.Height);
        }

        public SdfUvRect Mul(SdfUvRect uv)
        {
            var x = this.Left;
            var y = this.Top;

            x += this.Width * uv.X;
            y += this.Height * uv.Y;

            return new SdfUvRect(x, y,
                x + Width * uv.Width,
                y + Height * uv.Height
            );
        }
    }
}
#endif