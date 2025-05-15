Shader "Custom/Tree Soft Occlusion Bark" 
{
    Properties 
    {
        _Color ("Main Color", Color) = (1,1,1,0)
        _MainTex ("Main Texture", 2D) = "white" {}

        // These are here only to provide default values
        [HideInInspector] _TreeInstanceColor ("TreeInstanceColor", Vector) = (1,1,1,1)
        [HideInInspector] _TreeInstanceScale ("TreeInstanceScale", Vector) = (1,1,1,1)
        [HideInInspector] _SquashAmount ("Squash", Float) = 1
    }

    SubShader 
    {
        Tags 
        {
            "IgnoreProjector"="True"
            "Queue" = "Geometry"
            "RenderType" = "TreeOpaque"
            //"DisableBatching"="True"
        }

        Lighting On

        CGPROGRAM
        #pragma surface surf Lambert fullforwardshadows
        #pragma vertex bark

        #include "CustomTreeLibrary.cginc"
        #include "UnityCG.cginc"

        sampler2D _MainTex;
        fixed4 _Color;

        struct Input
        {
            float2 uv_MainTex;
        };
        void surf (Input IN, inout SurfaceOutput o) 
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color * UNITY_ACCESS_INSTANCED_PROP(UnityTerrain, _TreeInstanceColor);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG

        Pass {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #include "UnityCG.cginc"
            #include "TerrainEngine.cginc"

            struct v2f {
                V2F_SHADOW_CASTER;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                fixed4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            v2f vert( appdata v )
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                TerrainAnimateTree(v.vertex, v.color.w);
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                UNITY_TRANSFER_INSTANCE_ID(v, o)
                return o;
            }

            float4 frag( v2f i ) : SV_Target
            {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }

    Dependency "BillboardShader" = "Hidden/Custom/Tree Soft Occlusion Bark Rendertex"
    Fallback "Nature/Tree Soft Occlusion Bark"
}