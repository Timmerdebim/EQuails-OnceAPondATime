Shader "Hidden/OutlineFromJFA"
{
    Properties
    {
        _JFATex ("Texture", 2D) = "white" {}
        _SilhouetteTex ("Silhouette Texture", 2D) = "black" {}
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

            TEXTURE2D(_JFATex);
            SAMPLER(sampler_JFATex);
            TEXTURE2D(_SilhouetteTex);
            SAMPLER(sampler_SilhouetteTex);
    
            float4 _OutlineColor;
            float _OutlineWidth;
            float4 _ScreenParams;
            
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
                float2 seed_uv = SAMPLE_TEXTURE2D(_JFATex, sampler_JFATex, i.uv).rg;
                
                // Convert to pixel space to fix aspect ratio and get correct fwidth scale
                float2 pixelPos = i.uv * _ScreenParams.xy;
                float2 seedPixelPos = seed_uv * _ScreenParams.xy;
                float distanceToSeed = length(pixelPos - seedPixelPos);

                float silhouetteAlpha = SAMPLE_TEXTURE2D(_SilhouetteTex, sampler_SilhouetteTex, i.uv).r;

                float softOutline = smoothstep(_OutlineWidth, _OutlineWidth - fwidth(distanceToSeed), distanceToSeed);
                float finalOutline = softOutline * (1.0 - silhouetteAlpha);
                
                float outlineAlpha = _OutlineColor.a * finalOutline;
                float4 outColor = _OutlineColor * outlineAlpha;
                return outColor;
            }
            ENDHLSL
        }
    }
}