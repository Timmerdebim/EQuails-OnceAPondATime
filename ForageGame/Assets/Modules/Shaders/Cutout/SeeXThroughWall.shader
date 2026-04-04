Shader "UnityLibrary/URP/Effects/SeeThroughSprite"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        _BlurSize("Blur Size", float) = 3
        _SeeThroughOpacity("Opacity", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent" 
            "Queue" = "Transparent+1" // Draw after normal sprites
            "RenderPipeline" = "UniversalPipeline"
            "PreviewType" = "Plane"
        }

        Pass
        {
            Name "SeeThroughSprite"
            Tags { "LightMode" = "UniversalForward" }

            ZTest Greater  
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off       // Sprites are 2-sided

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color : COLOR; // Helper for SpriteRenderer color
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float4 _MainTex_TexelSize;
                float _BlurSize;
                float _SeeThroughOpacity;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.color = IN.color * _Color; // Combine vertex color with material tint
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                
                // Blur offsets
                float2 offX = float2(_MainTex_TexelSize.x * _BlurSize, 0);
                float2 offY = float2(0, _MainTex_TexelSize.y * _BlurSize);

                // 4-Tap Blur
                half4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv) * 0.2;
                c += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + offX) * 0.2;
                c += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv - offX) * 0.2;
                c += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + offY) * 0.2;
                c += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv - offY) * 0.2;

                c *= IN.color;
                c.a *= _SeeThroughOpacity;

                return c;
            }
            ENDHLSL
        }
    }
}