/*
 * Copyright (c) 2025 Rune Skovbo Johansen
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

Shader "Dither 3D/Opaque"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _MaskMap ("Mask Map", 2D) = "white" {}
        [Toggle] _UseMaskMap("Use Mask Map?", Float) = 0.0
        _EmissionMap ("Emission", 2D) = "white" {}
		_EmissionColor ("Emission Color", Color) = (0,0,0,0)
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        
        _DitherColor("Dither Color", Color) = (1, 1, 1, 1)
        _FadeInColor("Fade In Color", Range(0,1)) = 0.5
        _PostExposure("Post Exposure", Float) = 1.0

        [Header(Dither Input Brightness)]
        _InputExposure ("Exposure", Range(0,5)) = 1
        _InputOffset ("Offset", Range(-1,1)) = 0

        [Header(Dither Settings)]
        [DitherPatternProperty] _DitherMode ("Pattern", Int) = 3
        [HideInInspector] _DitherTex ("Dither 3D Texture", 3D) = "" {}
        [HideInInspector] _DitherRampTex ("Dither Ramp Texture", 2D) = "white" {}
        _Scale ("Dot Scale", Range(2,10)) = 5.0
        _SizeVariability ("Dot Size Variability", Range(0,1)) = 0
        _Contrast ("Dot Contrast", Range(0,2)) = 1
        _StretchSmoothness ("Stretch Smoothness", Range(0,2)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert finalcolor:mycolor

        #pragma target 3.5
        #pragma multi_compile_fog
        #pragma multi_compile __ DITHERCOL_GRAYSCALE DITHERCOL_RGB DITHERCOL_CMYK
        #pragma multi_compile __ INVERSE_DOTS
        #pragma multi_compile __ RADIAL_COMPENSATION
        #pragma multi_compile __ QUANTIZE_LAYERS
        #pragma multi_compile __ DEBUG_FRACTAL

        #include "Dither3DInclude.cginc"

        sampler2D _MainTex;
        sampler2D _BumpMap;
        sampler2D _EmissionMap;
        sampler2D _MaskMap;

        float _FadeInColor;
        float _PostExposure;
        float4 _DitherColor;

        float _UseMaskMap;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float2 uv_EmissionMap;
            float2 uv_DitherTex;
            float4 screenPos;
            UNITY_FOG_COORDS(4)
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        fixed4 _EmissionColor;

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            float4 clipPos = UnityObjectToClipPos(v.vertex);
            UNITY_TRANSFER_FOG(o, clipPos);
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
            o.Emission = (o.Albedo * 0.5) + tex2D(_EmissionMap, IN.uv_EmissionMap) * _EmissionColor;

            float4 maskMap = tex2D(_MaskMap, IN.uv_MainTex);

            if (_UseMaskMap == 0.0)
            {
                o.Metallic = _Metallic;
                o.Smoothness = _Glossiness;
            }
            else
            {
                o.Metallic = maskMap.r;
                o.Smoothness = maskMap.a;
            }
            
            o.Alpha = c.a;
        }

        void mycolor (Input IN, SurfaceOutputStandard o, inout fixed4 color)
        {
            UNITY_APPLY_FOG(IN.fogCoord, color);

            float4 oldColor = color;
            
            color.r = GetDither3DColor(IN.uv_DitherTex, IN.screenPos, color.r);
            color.g = GetDither3DColor(IN.uv_DitherTex, IN.screenPos, color.g);
            color.b = GetDither3DColor(IN.uv_DitherTex, IN.screenPos, color.b);

            color = lerp(color, oldColor, _FadeInColor);
            
            /*color = lerp(color, float4(o.Emission, color.a), _FadeInColor)
                * _PostExposure;*/
        }
        ENDCG
    }
    FallBack "Diffuse"
}
