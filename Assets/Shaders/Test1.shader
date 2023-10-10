Shader "Custom/NewSurfaceShader"
{
    Properties
    {
        _WaveHeight("Wave Height", float) = 1
        _WaveLength("Wave Length", float) = 1
        _Speed("Speed", float) = 1

        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"= "Opaque" }
        LOD 100
        
        CGPROGRAM

        #pragma surface surf Standard fullforwardshadows vertex:vert
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input 
        {
            float2 uv_MainTex;
        };
        half _Glossiness, _Metallic, _Speed, _WaveLength, _WaveHeight;
        fixed4 _Color;
        
        float getHeight(half x, half y)
        {
            half rad = sqrt(x * x + y * y);
            float wavefunc = _WaveHeight * sin((_Speed * _Time.y * 0.5 + rad/_WaveLength));
            return wavefunc;
        }
        void vert (inout appdata_full v)  
        {
             v.vertex.y = getHeight(v.vertex.x, v.vertex.y);
        }
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = _Color.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}