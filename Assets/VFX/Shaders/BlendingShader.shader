Shader "UI/MultiBlend"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        [Enum(Normal,0, Additive,1, Multiply,2, Screen,3)]
        _BlendMode ("Blend Mode", Float) = 0
    }

    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
        }

        ZWrite Off
        Cull Off
        Lighting Off

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha // default (Normal)

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _Color;
            float _BlendMode;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv) * i.color;

                // Fake blend modes in shader (since real Blend is fixed per pass)
                if (_BlendMode == 1) // Additive
                {
                    return tex * tex.a;
                }
                else if (_BlendMode == 2) // Multiply
                {
                    return fixed4(tex.rgb * tex.a, tex.a);
                }
                else if (_BlendMode == 3) // Screen
                {
                    return fixed4(1 - (1 - tex.rgb) * (1 - tex.a), tex.a);
                }

                return tex; // Normal
            }
            ENDCG
        }
    }
}