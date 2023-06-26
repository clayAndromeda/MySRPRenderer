#ifndef CLAY_RENDERER_TRANSFORMATION_HLSL
#define CLAY_RENDERER_TRANSFORMATION_HLSL

#include "InputMacro.hlsl"
#include "UnityBuiltIn.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"

// Object空間からView空間に変換する
float3 TransformObjectToViewPos(float3 positionOS)
{
    // Object -> World -> Viewと座標空間を変換する
    return mul(GetWorldToViewMatrix(), mul(GetObjectToWorldMatrix(), float4(positionOS, 1.0))).xyz;
}

// クリップ空間からスクリーン座標を計算する
float4 ComputeScreenPos(float4 positionCS)
{
    float4 o = positionCS * 0.5f;
    o.xy = float2(o.x, o.y * _ProjectionParams.x) + o.w;
    o.zw = positionCS.zw;
    return o;
}

// 3D空間の位置を受け取り、UV座標の開始位置やステレオレンダリングを考慮しながら、それをスクリーン座標に変換する
float4 ComputeGrabScreenPos(float4 pos)
{
    #if UNITY_UV_STARTS_AT_TOP
    float scale = -1.0;
    #else
    float scale = 1.0;
    #endif

    float4 o = pos * 0.5f;
    o.xy = float2(o.x, o.y * scale) + o.w;

    #ifdef UNITY_SINGLE_PASS_STEREO
    o.xy = TransformStereoScreenSpaceTex(o.xy, pos.w);
    #endif

    o.zw = pos.zw;
    return o;
}

#endif