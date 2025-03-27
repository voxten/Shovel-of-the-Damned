Shader "Custom/IceDissolve"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _DissolveTex ("Dissolve Texture", 2D) = "white" {}
        _DissolveAmount ("Dissolve Amount", Range(0,1)) = 0
        _EdgeColor ("Edge Color", Color) = (0, 0.8, 1, 1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex, _DissolveTex;
            float4 _Color, _EdgeColor;
            float _DissolveAmount;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                float dissolve = tex2D(_DissolveTex, i.uv).r;

                if (dissolve < _DissolveAmount)
                {
                    discard; // Removes pixels based on dissolve texture
                }

                // Add an edge glow effect
                if (dissolve < _DissolveAmount + 0.1)
                {
                    return _EdgeColor;
                }

                return col;
            }
            ENDCG
        }
    }
}
