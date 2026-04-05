Shader "Custom/CaveSpotlight"
{
    Properties
    {
        _Color ("Overlay Color", Color) = (0,0,0,1)
        _Center ("Spot Center (Viewport)", Vector) = (0.5, 0.5, 0, 0)
        _Radius ("Spot Radius", Range(0, 1)) = 0.2
        _Softness ("Edge Softness", Range(0, 1)) = 0.1
        _AspectRatio ("Aspect Ratio", Float) = 1.77
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            fixed4 _Color;
            float4 _Center;
            float _Radius;
            float _Softness;
            float _AspectRatio;

            v2f vert (appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // Correct UVs for aspect ratio so the circle isn't an oval
                float2 uv = i.uv - _Center.xy;
                uv.x *= _AspectRatio;
                
                // Calculate distance from center
                float dist = length(uv);
                
                // Smoothstep for soft edges (0 inside radius, 1 outside)
                float alpha = smoothstep(_Radius, _Radius + _Softness, dist);
                
                return fixed4(_Color.rgb, _Color.a * alpha);
            }
            ENDCG
        }
    }
}