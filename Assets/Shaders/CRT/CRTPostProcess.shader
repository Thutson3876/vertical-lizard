Shader "Custom/CRTPostProcess"
{
    Properties
    {
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}
        _ScanlineTexture ("Scanline Texture", 2D) = "white" {}
        _Curvature ("Curvature", Float) = 0.1
        _VignetteWidth ("Vignette Width", Float) = 0.1
        _Tiling ("Tiling", Float) = 2.0
        _Alpha ("Alpha", Float) = 0.75
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 grabPos : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.grabPos = ComputeGrabScreenPos(o.vertex);
                return o;
            }

            sampler2D _MainTex;
            sampler2D _ScanlineTexture;
            
            float _Curvature;
            float _VignetteWidth;

            float _Tiling;
            float _Alpha;

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv * 2.0 - 1.0;
                float2 offset = uv.yx / _Curvature;

                uv = uv + uv * offset * offset;
                uv = uv * 0.5 + 0.5;

                fixed4 col = tex2D(_ScanlineTexture, uv * _Tiling);
                
                if (uv.x <= 0.0 || 1.0 <= uv.x || uv.y <= 0.0 || 1.0 <= uv.y)
                {
                    col = 0.0;
                    return col;
                }
                uv = uv * 2.0 - 1.0;
                
                float2 vignette = _VignetteWidth / _ScreenParams.xy;
                vignette = smoothstep(0.0, vignette, 1.0 - abs(uv));
                vignette = saturate(vignette);

                col.g *= (sin(i.uv.y * _ScreenParams.y * 2.0) + 1.0) * 0.15 + 1.0;
                col.rb *= (cos(i.uv.y * _ScreenParams.y * 2.0) + 1.0) * 0.135 + 1.0;

                half4 bgColor = tex2D(_MainTex, uv * 0.5 + 0.5);
                
                return lerp(bgColor, saturate(col), _Alpha) * vignette.x * vignette.y;
            }
            ENDCG
        }
    }
}
