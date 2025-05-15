Shader "Hidden/Custom/Tree Soft Occlusion Leaves Rendertex" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,0)
        _MainTex ("Main Texture", 2D) = "white" {}
        _Cutoff ("Alpha cutoff", Range(0.25,0.9)) = 0.5

        // These are here only to provide default values
        [HideInInspector] _TreeInstanceColor ("TreeInstanceColor", Vector) = (1,1,1,1)
        [HideInInspector] _TreeInstanceScale ("TreeInstanceScale", Vector) = (1,1,1,1)
        [HideInInspector] _SquashAmount ("Squash", Float) = 1
    }

    SubShader {
        Lighting On

            CGPROGRAM
            #pragma surface surf Lambert fullforwardshadows
            #pragma vertex bark
            #define WRITE_ALPHA_1 1
            //#define USE_CUSTOM_LIGHT_DIR 1
            #include "CustomTreeLibrary.cginc"

            sampler2D _MainTex;
            fixed4 _Color;
            float _Cutoff;

            struct Input
            {
                float2 uv_MainTex;
            };
            void surf (Input IN, inout SurfaceOutput o) 
            {
                fixed4 tex = tex2D(_MainTex, IN.uv_MainTex) * _Color;
                clip(tex.a - _Cutoff);
                o.Albedo = tex.rgb;
                o.Alpha = tex.a;
            }
            ENDCG
    }

    Fallback "Hidden/Nature/Tree Soft Occlusion Leaves Rendertex"
}