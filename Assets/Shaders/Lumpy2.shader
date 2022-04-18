Shader "Custom/Lumpy2"
{

Properties
    {
        _EmissionColor("Color Tint", Color) = (1, 1, 1, 1)
        _Color("Rim Color", Color) = (1, 1, 1, 1)
        _RimPower("Rim Power", Range(0, 10.0)) = 1.0
        _EmissionStrength("Emission Strength", Range(0.0, 2.0)) = 1.0
        _InnerEmissionStrength("Inner Emission Strength", Range(0.0, 2.0)) = 1.0
    }
    
    SubShader
    {
        Tags{ "RenderType" = "Transparent" }

        CGPROGRAM
        #pragma surface surf Lambert

        struct Input 
        {
            float4 color : Color;
            float3 viewDir;
        };

        float4 _EmissionColor;
        float4 _Color;
        float _RimPower;
        float _EmissionStrength;
        float _InnerEmissionStrength;

        void surf(Input IN, inout SurfaceOutput o)
        {
            IN.color = _EmissionColor;
            half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
            o.Emission = ((_Color.rgb * pow(rim, _RimPower)) + (_EmissionColor / pow(rim, _RimPower) * _InnerEmissionStrength)) * _EmissionStrength;
        }

        ENDCG
    }
        FallBack "Diffuse"
}﻿
