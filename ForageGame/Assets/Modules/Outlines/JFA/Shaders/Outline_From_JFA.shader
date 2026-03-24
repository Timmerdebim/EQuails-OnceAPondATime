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
        Blend SrcAlpha OneMinusSrcAlpha
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
            }
            
            float DistanceToSeed(float2 pixel_uv, float2 seed_uv)
            {
                // Convert to pixel space to fix aspect ratio and get correct fwidth scale
                float2 pixelPos = pixel_uv * _ScreenParams.xy;
                float2 seedPixelPos = seed_uv * _ScreenParams.xy;
                float distanceToSeed = length(pixelPos - seedPixelPos);
                return distanceToSeed;
            }
            
            float2 GetSeedUV(float2 pixel_uv)
            {
                return SAMPLE_TEXTURE2D(_JFATex, sampler_JFATex, pixel_uv).rg;
            }
            
            float SilhouetteMask(float2 pixel_uv)
            {
                float silhouetteAlpha = SAMPLE_TEXTURE2D(_SilhouetteTex, sampler_SilhouetteTex, pixel_uv).r;
                return (1.0 - silhouetteAlpha);
            }
            
            float SmoothOutlineEdge(float distanceToSeed)
            {
                return smoothstep(_OutlineWidth, _OutlineWidth - fwidth(distanceToSeed), distanceToSeed);
            }
            
            FragOutput Frag(Varyings i)
            {
                FragOutput o;
                
                float2 pixel_uv = i.uv;
                float2 seed_uv = GetSeedUV(pixel_uv);
                
                float distanceToSeed = DistanceToSeed(pixel_uv, seed_uv);

                float outlineAlpha = 1;
                outlineAlpha = outlineAlpha * SmoothOutlineEdge(distanceToSeed);
                outlineAlpha = outlineAlpha * SilhouetteMask(pixel_uv);
                
                if (outlineAlpha <= 0.0)
                {
                    discard;
                }
                
                o.color = float4(_OutlineColor.rgb, _OutlineColor.a * outlineAlpha);                
                float seedDepth = SampleSeedDepth(i.uv, seed_uv);
                
                o.depth = seedDepth;
                
                // o.color = float4(seedDepth, seedDepth, seedDepth, 1);
                return o;

            }
            ENDHLSL
        }
    }
}