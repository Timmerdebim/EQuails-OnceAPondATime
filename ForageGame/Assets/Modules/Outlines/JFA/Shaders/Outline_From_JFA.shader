Shader "Hidden/OutlineFromJFA"
{
    Properties
    {
        _JFATex ("Texture", 2D) = "white" {}
        _SilhouetteTex ("Silhouette Texture", 2D) = "black" {}
        _DepthTex ("Depth Texture", 2D) = "white" {}
        
        _OutlineWidth ("Outline Width", Float) = 1.0
        _OutlineColor ("Outline Color", Color) = (0.5, 0.8, 1, 1)
        
        _DoWobbleBool ("Do Wobble", Float) = 0.0
        _WobbleNoiseScale ("Wobble Noise Scale", Float) = 0.2
        _WobbleNoiseSpeed ("Wobble Noise Speed", Float) = 0.5
        _WobbleMaxIndentFactor ("Wobble Noise Max Indent Factor", Float) = 0.5
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
            
            float4 _Time;
            float _DoWobbleBool;
            float _WobbleNoiseScale;
            float _WobbleNoiseSpeed;
            float _WobbleMaxIndentFactor;
            
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
            
            // ----- UTILITY FUNCTIONS -----
            
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
            
            float hash3(float3 p)
            {
                p = float3(dot(p, float3(127.1, 311.7, 74.7)),
                           dot(p, float3(269.5, 183.3, 246.1)),
                           dot(p, float3(113.5, 271.9, 124.6)));
                return frac(sin(p.x + p.y + p.z) * 43758.5453);
            }

            float valueNoise3D(float3 p)
            {
                float3 i = floor(p);
                float3 f = frac(p);
                
                // Cubic smoothstep interpolation
                float3 u = f * f * (3.0 - 2.0 * f);
                
                float a = hash3(i + float3(0, 0, 0));
                float b = hash3(i + float3(1, 0, 0));
                float c = hash3(i + float3(0, 1, 0));
                float d = hash3(i + float3(1, 1, 0));
                float e = hash3(i + float3(0, 0, 1));
                float f_ = hash3(i + float3(1, 0, 1));
                float g = hash3(i + float3(0, 1, 1));
                float h = hash3(i + float3(1, 1, 1));
                
                // Trilinear interpolation
                return lerp(
                    lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y),
                    lerp(lerp(e, f_, u.x), lerp(g, h, u.x), u.y),
                    u.z
                );
            }

            float noise(float2 p, float t)
            {
                float value = 0.0;
                float amplitude = 1.0;
                float frequency = 1.0;
                int octaves = 4;
                
                for (int i = 0; i < octaves; i++)
                {
                    value += amplitude * valueNoise3D(float3(p * frequency, t * frequency));
                    amplitude *= 0.5;
                    frequency *= 2.0;
                }
                
                return value;
            }
            
            // ----- OUTLINE EFFECTS -----
            
            float SilhouetteMask(float2 pixel_uv)
            {
                float silhouetteAlpha = SAMPLE_TEXTURE2D(_SilhouetteTex, sampler_SilhouetteTex, pixel_uv).r;
                return (1.0 - silhouetteAlpha);
            }
            
            float OutlineMask(float outlineWidth, float distanceToSeed)
            {
                return smoothstep(outlineWidth, outlineWidth - fwidth(distanceToSeed), distanceToSeed);
            }
            
            float ApplyWobbleToOutlineWidth(float outlineWidth, float2 seed_uv)
            {
                if (_DoWobbleBool < 0.5) return 1.0; // No wobble
                
                float noiseValue = noise(seed_uv / _WobbleNoiseScale, _Time.y * _WobbleNoiseSpeed);
                float wobble = lerp(0, _WobbleMaxIndentFactor * outlineWidth, noiseValue);
                
                float newWidth = outlineWidth - wobble;
                return newWidth;
                
            }
            
            FragOutput Frag(Varyings i)
            {
                FragOutput o;
                
                float2 pixel_uv = i.uv;
                float2 seed_uv = GetSeedUV(pixel_uv);
                
                float distanceToSeed = DistanceToSeed(pixel_uv, seed_uv);

                float outlineWidth = _OutlineWidth;
                
                if (_DoWobbleBool >= 0.5)
                {
                    outlineWidth = ApplyWobbleToOutlineWidth(outlineWidth, seed_uv);
                }
                
                float outlineAlpha = 1;
                outlineAlpha = outlineAlpha * OutlineMask(outlineWidth, distanceToSeed);
                outlineAlpha = outlineAlpha * SilhouetteMask(pixel_uv);
                
                if (outlineAlpha <= 0.0)
                {
                    discard;
                }
                
                o.color = float4(_OutlineColor.rgb, _OutlineColor.a * outlineAlpha);                
                float seedDepth = SampleSeedDepth(i.uv, seed_uv);
                
                o.depth = seedDepth;
                
                // o.color = float4(seedDepth, seedDepth, seedDepth, 1);
                // o.color = float4(noise(pixel_uv * _WobbleNoiseScale + _Time.y * _WobbleNoiseSpeed).xxx, 1);
                return o;

            }
            ENDHLSL
        }
    }
}