Shader "Custom/CelRender"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _MainColor("Main Color", Color) = (1,1,1,1)

        _ShadowColor ("Shadow Color", Color) = (0.7, 0.7, 0.8, 1.0)
	    _ShadowRange ("Shadow Range", Range(0, 1)) = 0.5
        _ShadowSmooth("Shadow Smooth", Range(0, 1)) = 0.2

        _RimColor("Rim Color", Color) = (1,1,1,1)
        _RimSmooth("Rim Smooth", Range(0, 1)) = 0.2
        _RimMin ("Rin Min", Range(0, 1)) = 0.1
        _RimMax ("Rin Min", Range(0, 1)) = 0.5

        [Space(10)]
	    _OutlineWidth ("Outline Width", Range(0.01, 2)) = 0.24
        _OutLineColor ("OutLine Color", Color) = (0.5,0.5,0.5,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            Tags {"LightMode"="ForwardBase"}

            Cull Back

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
	        #include "Lighting.cginc"
            #include "AutoLight.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            half4 _MainColor;

            half4 _ShadowColor;
            half _ShadowRange;
            half _ShadowSmooth;

            half4 _RimColor;
            half _RimSmooth;
            half _RimMin;
            half _RimMax;
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            half4 frag(v2f i) : SV_TARGET 
            {
                half4 color = 0;
                half4 mainTex = tex2D(_MainTex, i.uv);

                half3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
                half3 worldNormal = normalize(i.worldNormal);
                half3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);

                half halfLambert = dot(worldNormal, worldLightDir) * 0.5 +0.5;
                half ramp = smoothstep(0, _ShadowSmooth, halfLambert - _ShadowRange);
                half3 diffuse = lerp(_ShadowColor, _MainColor, ramp);

                // 增加边缘光
                half halfVec = saturate(dot(viewDir, worldNormal));
                half f = 1.0 - halfVec;
                 half rim = smoothstep(_RimMin, _RimMax, f);
                rim = smoothstep(0, _RimSmooth, rim);
                half3 rimColor = rim * _RimColor.rgb * _RimColor.a;

                diffuse *= mainTex;
                color.rgb = _LightColor0 * (diffuse + rimColor);

                return color;
            }
            ENDCG
        }   

        Pass
        {
            Cull Front

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            half _OutlineWidth;
            half4 _OutLineColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float4 tangent : TANGENT;
                float4 vertColor : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 vertColor : COLOR;
            };

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                // 直接在局部坐标描边，会因投影变换导致描边粗细变化
                //o.vertex = UnityObjectToClipPos(float4(v.vertex.xyz + v.normal * _OutlineWidth * 0.1 ,1));
                // 在NDC空间进行描边
                float4 pos = UnityObjectToClipPos(v.vertex); // 
                //float3 viewNormal = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal.xyz);
                float3 viewNormal = mul((float3x3)UNITY_MATRIX_IT_MV, v.tangent.xyz);
                float3 ndcNormal = normalize(TransformViewToProjection(viewNormal.xyz));
                //将近裁剪面右上角位置的顶点变换到观察空间
                float4 nearUpperRight = mul(unity_CameraInvProjection, float4(1, 1, UNITY_NEAR_CLIP_VALUE, _ProjectionParams.y));
                float aspect = abs(nearUpperRight.y / nearUpperRight.x) ;//求得屏幕宽高比
                ndcNormal.x *= aspect;
                pos.xy += 0.1 * _OutlineWidth * ndcNormal.xy * v.vertColor.a; ;
                o.vertex = pos;
                o.vertColor = v.vertColor.rgb;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return fixed4(_OutLineColor * i.vertColor, 0);
            }
            ENDCG
        }
       
    }
    FallBack "Diffuse"
}
