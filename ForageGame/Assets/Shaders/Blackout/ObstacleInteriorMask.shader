Shader "Custom/ObstacleInteriorMask"
{
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" }

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        struct Attributes { float4 positionOS : POSITION; };
        struct Varyings   { float4 positionCS : SV_POSITION; };
        Varyings vert(Attributes i)
        {
            Varyings o;
            o.positionCS = TransformObjectToHClip(i.positionOS.xyz);
            return o;
        }
        half4 fragBlack(Varyings i) : SV_Target { return half4(0,0,0,1); }
        ENDHLSL

        // Pass 0: back faces, +1 where BEHIND scene depth
        Pass
        {
            Name "VolumeBack"
            Tags { "LightMode"="UniversalForward" }
            Cull Front  ZWrite Off  ZTest LEqual  ColorMask 0
            Offset 1, 1   // pushes away from camera so an obstacle's own surface cancels out
            Stencil { Ref 0 Comp Always ZFail IncrWrap }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment fragBlack
            ENDHLSL
        }

        // Pass 1: front faces, -1 where BEHIND scene depth
        Pass
        {
            Name "VolumeFront"
            Tags { "LightMode"="UniversalForward" }
            Cull Back  ZWrite Off  ZTest LEqual  ColorMask 0
            Offset 1, 1
            Stencil { Ref 0 Comp Always ZFail DecrWrap }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment fragBlack
            ENDHLSL
        }

        // Pass 2: fill marked pixels with black (and reset stencil)
        Pass
        {
            Name "Fill"
            Tags { "LightMode"="UniversalForward" }
            Cull Off  ZWrite Off  ZTest Always
            Stencil { Ref 0 Comp NotEqual Pass Zero }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment fragBlack
            ENDHLSL
        }
    }
}