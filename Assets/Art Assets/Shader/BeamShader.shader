Shader "Custom/LightBeam"
{
    Properties
    {
        _Color ("Beam Color", Color) = (1, 1, 0.8, 0.05)
        _FalloffPow ("Falloff Power", Float) = 1.5
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha One          
        ZWrite Off                  
        Cull Front                  

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _Color;
            float  _FalloffPow;

            struct appdata { float4 vertex : POSITION; float3 normal : NORMAL; };
            struct v2f    { float4 pos : SV_POSITION; float  falloff : TEXCOORD0; };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos     = UnityObjectToClipPos(v.vertex);
                o.falloff = pow(v.vertex.z, _FalloffPow);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 c = _Color;
                c.a *= i.falloff;   
                return c;
            }
            ENDCG
        }
    }
}