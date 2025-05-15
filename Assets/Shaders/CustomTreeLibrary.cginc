// Upgrade NOTE: commented out 'float4x4 _CameraToWorld', a built-in variable
// Upgrade NOTE: replaced '_CameraToWorld' with 'unity_CameraToWorld'

// Upgrade NOTE: commented out 'float4x4 _CameraToWorld', a built-in variable
// Upgrade NOTE: replaced '_CameraToWorld' with 'unity_CameraToWorld'

// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Shared tree shader functionality for Unity 2.x tree shaders

#include "HLSLSupport.cginc"
#include "UnityCG.cginc"
#include "CustomTerrainEngine.cginc"



#ifdef USE_CUSTOM_LIGHT_DIR
CBUFFER_START(UnityTerrainImposter)
    float3 _TerrainTreeLightDirections[4];
    float4 _TerrainTreeLightColors[4];
CBUFFER_END
#endif

CBUFFER_START(UnityPerCamera2)
// float4x4 _CameraToWorld;
CBUFFER_END

float _HalfOverCutoff;

struct v2f
{
    float4 pos : SV_POSITION;
    float4 uv : TEXCOORD0;
    half4 color : TEXCOORD1;
    UNITY_FOG_COORDS(2)
    UNITY_VERTEX_OUTPUT_STEREO
};


void leaves(inout appdata_tree v)
{
    UNITY_SETUP_INSTANCE_ID(v);
    
    TerrainAnimateTree(v.vertex, v.color.w);
}

void bark(inout appdata_tree v)
{
    UNITY_SETUP_INSTANCE_ID(v);
    
    TerrainAnimateTree(v.vertex, v.color.w);
}

v2f billboard(appdata_tree_billboard v)
{
    v2f o;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    TerrainBillboardTree(v.vertex, v.texcoord1.xy, v.texcoord.y);
    
    float3 viewpos = UnityObjectToViewPos(v.vertex);
    o.pos = UnityObjectToClipPos(v.vertex);

    float4 lightDir = 0;
    float4 lightColor = 0;
    //lightDir.w = 0.0;

    float4 light = UNITY_LIGHTMODEL_AMBIENT;

    for (int i = 0; i < 4; i++)
    {
        float atten = 1.0;
#ifdef USE_CUSTOM_LIGHT_DIR
            lightDir.xyz = _TerrainTreeLightDirections[i];
            lightColor = _TerrainTreeLightColors[i];
#else
        float3 toLight = unity_LightPosition[i].xyz - viewpos.xyz * unity_LightPosition[i].w;
        toLight.z *= -1.0;
        lightDir.xyz = mul((float3x3) unity_CameraToWorld, normalize(toLight));
        float lengthSq = dot(toLight, toLight);
        atten = 1.0 / (1.0 + lengthSq * unity_LightAtten[i].z);

        lightColor.rgb = unity_LightColor[i].rgb;
#endif

        //lightDir.xyz *= _Occlusion;
        //float occ = dot(v.tangent, lightDir);
        //occ = max(0, occ);
        //occ += _BaseLight;
        //light += lightColor * (occ * atten);
    }

    o.color = light * v.color;
    o.color.a = 0.5 * _HalfOverCutoff;
    
    
    o.uv.x = v.texcoord.x;
    o.uv.y = v.texcoord.y > 0;
    UNITY_TRANSFER_FOG(o, o.pos);
    //UNITY_TRANSFER_INSTANCE_ID(v, o);

    return o;
}

