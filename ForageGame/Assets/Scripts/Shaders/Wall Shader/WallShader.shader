Shader "UnityLibrary/URP/Effects/WallWithViewHole"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        
        // How wide the viewing tunnel is
        _CutoutRadius ("Hole Radius", Float) = 1.0
        // How soft the edges of the hole are
        _EdgeSoftness ("Edge Softness", Float) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float4 color : COLOR;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float _CutoutRadius;
                float _EdgeSoftness;
            CBUFFER_END

            // Set by the C# script
            float3 _GlobalPlayerPos;

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.color = IN.color * _Color;
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                col *= IN.color;

                // --- VIEW LINE MATH ---
                
                float3 camPos = _WorldSpaceCameraPos;
                float3 pixelPos = IN.positionWS;
                float3 playerPos = _GlobalPlayerPos;

                // 1. Vector from Camera to Player (The View Ray)
                float3 camToPlayer = playerPos - camPos;
                
                // 2. Vector from Camera to this Pixel (The Wall)
                float3 camToPixel = pixelPos - camPos;

                // 3. Project "Camera->Pixel" onto "Camera->Player" to find position along the ray
                //    Result 't' is 0.0 at Camera, 1.0 at Player
                float t = dot(camToPixel, camToPlayer) / dot(camToPlayer, camToPlayer);
                
                // 4. Optimization: Only cut holes if the wall is IN FRONT of the player (t < 1.0)
                //    and in front of the camera (t > 0.0)
                float mask = 1.0;
                
                if (t > 0.0 && t < 1.0)
                {
                    // Find the closest point on the View Ray to this pixel
                    float3 closestPointOnRay = camPos + camToPlayer * t;
                    
                    // Distance from the pixel to that ray
                    float dist = distance(pixelPos, closestPointOnRay);
                    
                    // Calculate transparency
                    mask = smoothstep(_CutoutRadius, _CutoutRadius + _EdgeSoftness, dist);
                }
                
                col.a *= mask;
                // ----------------------

                return col;
            }
            ENDHLSL
        }
    }
}