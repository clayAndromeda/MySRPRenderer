Shader "ClayRenderer/SRP0001/UnlitTransparent"
{
    Properties
    {
        _MainTex ("_MainTex (RGBA)", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        // Tags { "RenderType"="Transparent" "IgnoreProjector"="True" }
        Cull Off Lighting Off ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            Tags { "LightMode" = "SRP0001_Pass" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Assets/ClayRenderer/Runtime/General/ShaderLibrary/Transformation.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            CBUFFER_START(UnityPerMaterial)
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            CBUFFER_END

            v2f vert(appdata v)
            {
                v2f o;
                // 頂点座標をクリップ空間に変換
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv) * _Color;
                return col;
            }
            
            ENDHLSL
        }
    }
}
