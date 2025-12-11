Shader "UnityLibrary/URP/System/StencilMask_Dithered_Production"
{
    Properties
    {
        _EdgePower ("Edge Gradient Power", Float) = 2.0
        // NOTE: In this shader, DitherScale acts as 'Fade Amount' (0 = Invisible, 1 = Visible)
        // We keep the name '_DitherScale' to maintain compatibility with your C# script.
        _DitherScale ("Fade Amount", Range(0, 1)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Geometry-1" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            // Invisible pass - strictly writes to Stencil Buffer
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

            // High-quality Interleaved Gradient Noise (Cinematic Grain)
            // Replaces the blocky Bayer matrix with smooth, pseudo-random noise.
            float InterleavedGradientNoise(float2 pixelPos)
            {
                const float3 magic = float3(0.06711056, 0.00583715, 52.9829189);
                return frac(magic.z * frac(dot(pixelPos, magic.xy)));
            }

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
                // 1. Calculate Fresnel Gradient (0 at edges, 1 at center)
                // This creates the "Cone" shape fading at the sides
                float NdotV = saturate(abs(dot(IN.normalWS, IN.viewDirWS)));
                float shapeGradient = pow(NdotV, _EdgePower);

                // 2. Generate Noise based on Screen Position
                // We multiply by ScreenParams to ensure noise is pixel-perfect (1:1 with screen pixels)
                float2 pixelPos = (IN.screenPos.xy / IN.screenPos.w) * _ScreenParams.xy;
                float noise = InterleavedGradientNoise(pixelPos);

                // 3. Combine Gradient with the Master Fade (_DitherScale)
                // This effectively determines the probability of a pixel existing.
                float currentOpacity = shapeGradient * _DitherScale;

                // 4. Discard pixel if the opacity is lower than the noise threshold
                // This creates the dithered transparency effect.
                clip(currentOpacity - noise);

                return half4(0,0,0,0);
            }
            ENDHLSL
        }
    }
}