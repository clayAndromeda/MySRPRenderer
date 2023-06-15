#ifndef CLAY_RENDERER_TRANSFORMATION_HLSL
#define CLAY_RENDERER_TRANSFORMATION_HLSL

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"

// x = 1 or -1（1なら正射影カメラ, 2なら透視投影カメラ）
// y = near plane
// z = far plane
// w = 1 / far plane（投影行列で、深度を正規化する時に使う）
float4 _ProjectionParams;

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

#endif