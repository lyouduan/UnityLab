Shader "Custom/GBuffer"
{
    Properties
    {
        _Albedo ("Albedo Map", 2D) = "white" {}
        _Color ("Albedo Tint", Color) = (1,1,1,1)

        _Roughness ("Roughness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        [Toggle] _Use_MetalMap ("Use Metal Map", Float) = 1
        _MetallicGlossMap ("Metallic Map", 2D) = "white" {}

        [Toggle] _Use_OcclusionMap ("Use Occlusion Map", Float) = 0
        _OcclusionMap ("Occlusion Map", 2D) = "white" {}

        [Toggle] _Use_Normal_Map ("Use Normal Map", Float) = 1
        [Normal] _BumpMap ("Normal Map", 2D) = "bump" {}

        _EmissionMap ("Emission Map", 2D) = "black" {}
    }

    SubShader
    {
        Pass
        {
            Tags { "LightMode" = "GBuffer"}
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_Albedo);
            TEXTURE2D(_MetallicGlossMap);
            TEXTURE2D(_OcclusionMap);
            TEXTURE2D(_EmissionMap);
            TEXTURE2D(_BumpMap);
            sampler sampler_linear_repeat;
            sampler sampler_linear_clamp;
            sampler sampler_point_clamp;

            float4 _Color;
            float4 _Albedo_ST;
            float _Roughness;
            float _Metallic;

            float _Use_MetalMap;
            float _Use_OcclusionMap;
            float _Use_NormalMap;

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                float4 worldPos = mul(unity_ObjectToWorld, IN.positionOS);
                OUT.positionCS = mul(unity_MatrixVP, worldPos);
                OUT.normal = TransformObjectToWorldNormal(IN.normal); 
                OUT.uv = TRANSFORM_TEX(IN.uv, _Albedo);
                return OUT;
            }
            
            struct MRTOutput
            {
                float4 MRT0 : SV_Target0;
                float4 MRT1 : SV_Target1;
                float4 MRT2 : SV_Target2;
                float4 MRT3 : SV_Target3;
            };

            MRTOutput frag (Varyings IN)
			{
                /*
                if(IN.uv.x < 0)
                {
                 IN.uv.x = 1 +IN.uv.x; 
                }
                if(IN.uv.x > 1)
				{
				 IN.uv.x = IN.uv.x - 1; 
				}
                if(IN.uv.y < 0)
                {
                 IN.uv.y = 1 +IN.uv.y; 
                }
                if(IN.uv.y > 1)
				{
				 IN.uv.y = IN.uv.y - 1; 
				}
                */
				float4 albedo = SAMPLE_TEXTURE2D(_Albedo, sampler_linear_repeat, IN.uv) * _Color;
                float3 emission = SAMPLE_TEXTURE2D(_EmissionMap, sampler_linear_repeat, IN.uv).rgb;
                float3 normal = _Use_NormalMap ? UnpackNormal(SAMPLE_TEXTURE2D(_BumpMap, sampler_linear_repeat, IN.uv)) :IN.normal;
                float ao = SAMPLE_TEXTURE2D(_OcclusionMap, sampler_linear_repeat, IN.uv).g;
                float metallic = _Use_MetalMap ? SAMPLE_TEXTURE2D(_MetallicGlossMap, sampler_linear_repeat, IN.uv).r : _Metallic;
                float roughness = _Use_MetalMap ? SAMPLE_TEXTURE2D(_MetallicGlossMap, sampler_linear_repeat, IN.uv).g : _Roughness;
                
                MRTOutput OUT;
				OUT.MRT0 = albedo;
				OUT.MRT1 = float4(normal * 0.5 + 0.5, 1);
				OUT.MRT2 = float4(roughness, metallic, ao, 1);
				OUT.MRT3 = float4(emission, 1);
				return OUT;
			}
            ENDHLSL
        }
    }
}
