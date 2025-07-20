Shader "Debug/HiZMipViewer"
{
    Properties
    {
        _MainTex("Depth HiZ", 2D) = "white" {}
        _MipLevel("Mip Level", Range(0, 10)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            ZTest Always 
            Cull Off
            ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            sampler2D _MainTex;
            float _MipLevel;

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float2 uv : TEXCOORD0; float4 pos : SV_POSITION; };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float depth = tex2Dlod(_MainTex, float4(i.uv, 0, _MipLevel)).r;
                return float4(depth, depth, depth, 1); // 灰度显示
            }
            ENDCG
        }
    }
}