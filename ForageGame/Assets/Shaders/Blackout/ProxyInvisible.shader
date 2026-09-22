Shader "Hidden/ProxyInvisible"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        Pass
        {
            ColorMask 0
            ZWrite Off
        }
    }
}