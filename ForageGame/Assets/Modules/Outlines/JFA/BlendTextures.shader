Shader "Custom/BlendTextures"
{
    Properties
    {
        _Tex1("Texture 1", 2D) = "white" {}
        _Tex2("Texture 2", 2D) = "white" {}
    }
    SubShader
    {
        ZWrite Off ZTest Always Cull Off
        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

            TEXTURE2D(_Tex1);
            SAMPLER(sampler_Tex1);
            TEXTURE2D(_Tex2);
            SAMPLER(sampler_Tex2);

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
                float4 color1 = SAMPLE_TEXTURE2D(_Tex1, sampler_Tex1, i.uv);
                float4 color2 = SAMPLE_TEXTURE2D(_Tex2, sampler_Tex2, i.uv);
                
                // Lerp based on alpha
                return lerp(color1, color2, color2.a);
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}