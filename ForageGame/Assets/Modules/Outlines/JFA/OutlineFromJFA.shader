Shader "Hidden/OutlineFromJFA"
{
    Properties
    {
        _JFATex ("Texture", 2D) = "white" {}
        _SilhouetteTex ("Silhouette Texture", 2D) = "black" {}
        _DepthTex ("Depth Texture", 2D) = "white" {}
        _OutlineWidth ("Outline Width", Float) = 1.0
        _OutlineColor ("Outline Color", Color) = (0.5, 0.8, 1, 1)
    }
    SubShader
    {
        Cull Off ZWrite On ZTest LEqual

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
            TEXTURE2D(_DepthTex);
            SAMPLER(sampler_DepthTex);
    
            float4 _OutlineColor;
            float _OutlineWidth;
            float4 _ScreenParams; // x = width, y = height, z = 1/width, w = 1/height
            
            struct Varyings { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };

            Varyings Vert(uint id : SV_VertexID)
            {
                Varyings o;
                o.pos = GetFullScreenTriangleVertexPosition(id);
                o.uv  = GetFullScreenTriangleTexCoord(id);
                return o;
            }
            
            struct FragOutput
            {
                float4 color : SV_Target;
                float depth : SV_Depth;
            };
            
            float SampleSeedDepth(float2 pixel_uv, float2 seed_uv)
            {
                float2 texelSize = _ScreenParams.zw; // zw = 1/width, 1/height
                float maxDepth = 0.0;
                
                for (int x = -1; x <= 1; x++)
                for (int y = -1; y <= 1; y++)
                {
                    float2 offset = float2(x, y);
                    float2 pos = seed_uv * _ScreenParams.xy + offset;
                    float depth = LOAD_TEXTURE2D(_DepthTex, pos).r;
                    maxDepth = max(maxDepth, depth);
                }
                
                return maxDepth;
                
                float2 seed_pos = seed_uv * _ScreenParams.xy;
                float depth = LOAD_TEXTURE2D(_DepthTex, int2(seed_pos)).r;
                // depth = SAMPLE_TEXTURE2D(_DepthTex, sampler_DepthTex, seed_uv).r;
                return depth;
            }
            
            FragOutput Frag(Varyings i)
            {
                FragOutput o;
                
                float2 seed_uv = SAMPLE_TEXTURE2D(_JFATex, sampler_JFATex, i.uv).rg;
                
                // Convert to pixel space to fix aspect ratio and get correct fwidth scale
                float2 pixelPos = i.uv * _ScreenParams.xy;
                float2 seedPixelPos = seed_uv * _ScreenParams.xy;
                float distanceToSeed = length(pixelPos - seedPixelPos);

                float silhouetteAlpha = SAMPLE_TEXTURE2D(_SilhouetteTex, sampler_SilhouetteTex, i.uv).r;

                float softOutline = smoothstep(_OutlineWidth, _OutlineWidth - fwidth(distanceToSeed), distanceToSeed);
                float finalOutline = softOutline * (1.0 - silhouetteAlpha);
                
                if (finalOutline <= 0.0)
                {
                    discard;
                }
                
                float outlineAlpha = _OutlineColor.a * finalOutline;
                float4 outColor = _OutlineColor * outlineAlpha;
                o.color = outColor;
                
                float seedDepth = SampleSeedDepth(i.uv, seed_uv);
                
                o.depth = seedDepth;
                
                o.color = float4(seedDepth, seedDepth, seedDepth, 1);
                return o;

            }
            ENDHLSL
        }
    }
}