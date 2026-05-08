Shader "Custom/WorldReveal"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _LightMap;

            float4 _MainTex_ST;

            float2 _WorldMin;
            float2 _WorldSize;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 baseColor = tex2D(_MainTex, i.uv);

                // Convert world position to lightmap UV
                float2 lightUV;

                lightUV.x = (i.worldPos.x - _WorldMin.x) / _WorldSize.x;
                lightUV.y = (i.worldPos.z - _WorldMin.y) / _WorldSize.y;

                float light = tex2D(_LightMap, lightUV).r;

                // Darken unrevealed areas
                baseColor.rgb *= light;

                return baseColor;
            }

            ENDHLSL
        }
    }
}