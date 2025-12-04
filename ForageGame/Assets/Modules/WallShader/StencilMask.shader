Shader "UnityLibrary/URP/System/StencilMask_Dithered_Production"
{
    Properties
    {
        _EdgePower ("Edge Gradient Power", Float) = 2.0
        _DitherScale ("Dither Scale", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Geometry-1" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            // Do not write any color to the screen (Invisible)
            ColorMask 0
            
            ZWrite Off
            ZTest Always

            Stencil
            {
                Ref 1
                Comp Always
                Pass Replace
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 viewDirWS : TEXCOORD1;
                float4 screenPos : TEXCOORD3;
            };

            CBUFFER_START(UnityPerMaterial)
                float _EdgePower;
                float _DitherScale;
            CBUFFER_END

            // Standard 4x4 Bayer Dither Matrix
            static const float4x4 ditherMatrix = {
                0.0625, 0.5625, 0.1875, 0.6875,
                0.8125, 0.3125, 0.9375, 0.4375,
                0.2500, 0.7500, 0.1250, 0.6250,
                1.0000, 0.5000, 0.8750, 0.3750
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                
                float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.viewDirWS = GetWorldSpaceNormalizeViewDir(positionWS);
                
                OUT.screenPos = ComputeScreenPos(OUT.positionCS);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // 1. Calculate Fresnel (0 at edges, 1 at center)
                float NdotV = abs(dot(IN.normalWS, IN.viewDirWS));
                float gradient = pow(NdotV, _EdgePower);

                // 2. Dither Logic
                float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
                float2 pixelPos = screenUV * _ScreenParams.xy * _DitherScale;
                
                uint x = (uint)pixelPos.x % 4;
                uint y = (uint)pixelPos.y % 4;
                float threshold = ditherMatrix[x][y];

                // 3. Discard pixel if gradient is weak
                // This prevents the Stencil from being written in these spots
                clip(gradient - threshold);

                return half4(0,0,0,0);
            }
            ENDHLSL
        }
    }
}