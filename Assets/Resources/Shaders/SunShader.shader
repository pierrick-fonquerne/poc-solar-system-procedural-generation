Shader "Custom/SunShader"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 0, 1)
        _Emission ("Emission", Range(0, 5)) = 1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex;
        float _Emission;
        float4 _Color;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            o.Albedo = _Color.rgb;
            o.Emission = _Color.rgb * _Emission;
        }
        ENDCG
    }
    FallBack "Diffuse"
}