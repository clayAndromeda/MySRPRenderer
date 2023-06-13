// これは、カスタムのスクリプタブルレンダーパイプラインと互換性のある単純な unlit シェーダーオブジェクトを定義します。
// ハードコードされた色を適用し、LightMode パスタグの使用法を示します。
// SRP バッチャーとは互換性がありません。

Shader "Examples/SimpleUnlitColor"
{
    SubShader
    {
        Pass
        {
            // LightMode パスタグの値は、ScriptableRenderContext.DrawRenderers の ShaderTagId と一致する必要があります。
            Tags
            {
                "LightMode" = "ExampleLightModeTag"
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float4x4 unity_MatrixVP;
            float4x4 unity_ObjectToWorld;

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float4 worldPos = mul(unity_ObjectToWorld, IN.positionOS);
                OUT.positionCS = mul(unity_MatrixVP, worldPos);
                return OUT;
            }

            float4 frag(Varyings IN) : SV_TARGET
            {
                return float4(0.5, 1, 0.5, 1);
            }
            ENDHLSL
        }
    }
}