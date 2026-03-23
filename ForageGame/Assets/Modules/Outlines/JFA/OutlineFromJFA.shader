Shader "Hidden/OutlineFromJFA"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineWidth ("Outline Width", Float) = 1.0
        _OutlineColor ("Outline Color", Color) = (0.5, 0.8, 1, 1)
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
    
            float4 _OutlineColor;
            float _OutlineWidth;
            
            struct Varyings { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };

            Varyings Vert(uint id : SV_VertexID)
            {
                Varyings o;
                o.pos = GetFullScreenTriangleVertexPosition(id);
                o.uv  = GetFullScreenTriangleTexCoord(id);
                return o;
            }

            float4 Frag(Varyings i) : SV_Target
            {
                float2 seed_uv = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv).rg;
                float2 uv = i.uv;
                float distanceToSeed = distance(seed_uv, uv);
                
                if (distanceToSeed <= 0.001)
                {
                    return float4(0, 0, 0, 0);
                }
                
                if (distanceToSeed < _OutlineWidth)
                {
                    return _OutlineColor;
                }
                else
                {
                    return float4(0, 0, 0, 0); 
                }
            }
            ENDHLSL
        }
    }
}