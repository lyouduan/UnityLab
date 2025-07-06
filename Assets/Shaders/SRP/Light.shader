Shader "Custom/Light"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
       
		Pass
		{
			Cull Off ZWrite On ZTest Always
			HLSLPROGRAM

			#pragma vertex vertexPass
			#pragma fragment fragmentPass
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "../Common/Light.hlsl"
			#include "../Common/BRDF.hlsl"

			struct Attributes
			{
				float4 positionOS   : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 positionWS : VAR_POSITION;
			};

			Varyings vertexPass(Attributes IN)
			{
				Varyings OUT;
				float4 worldPos = mul(unity_ObjectToWorld, IN.positionOS);
                OUT.positionCS = mul(unity_MatrixVP,worldPos);
				OUT.positionWS = worldPos;
				OUT.uv = IN.uv; 
				return OUT;
			}

			TEXTURE2D(_gDepth);
			TEXTURE2D(_GT0);
			TEXTURE2D(_GT1);
			TEXTURE2D(_GT2);
			TEXTURE2D(_GT3);
			sampler sampler_linear_repeat;
			sampler sampler_linear_clamp;
			sampler sampler_point_clamp;

			float4 fragmentPass(Varyings IN, out float depthOut : SV_Depth) : SV_Target
			{
				float4 albedo = SAMPLE_TEXTURE2D(_GT0, sampler_linear_clamp, IN.uv);
				float3 N = SAMPLE_TEXTURE2D(_GT1, sampler_linear_clamp, IN.uv).xyz * 2 - 1;
				float3 RMA = SAMPLE_TEXTURE2D(_GT2, sampler_linear_clamp, IN.uv).rgb;
				float roughness = RMA.r;
				float metallic  = RMA.g;
				float ao = RMA.b;
				float3 emission = SAMPLE_TEXTURE2D(_GT3, sampler_linear_clamp, IN.uv).rgb;

				// Direct PBR
				Light light = GetDirectionalLight();
				float3 cameraPos = _WorldSpaceCameraPos;
				float3 V = normalize(cameraPos - IN.positionWS.xyz);
				float3 L = normalize(light.direction);
				float3 H = normalize(V + L);

				float NdotL = max(dot(N, L), 0);
				float NdotV = max(dot(N, V), 0);
				float NdotH = max(dot(N, H), 0);
				float HdotV = max(dot(H, V), 0);

				float r2 = roughness * roughness;
				float3 F0 = lerp(0.04, albedo, metallic);
				float D = NDF(NdotH,r2);
				float3 F = SchlickFresnel(HdotV, F0);
				float G = GGX(NdotL, NdotV, r2);
				
				float3 k_specular =  (D * F * G * 0.25) / (NdotL * NdotV + 0.0001);
				float3 k_diffuse = albedo;
				float3 kd = (1 - F) * (1 - metallic);
				float3 DirectLightResult = (kd * k_diffuse + k_specular)  * light.color * NdotL;

				// ibl indirect BRDF
				// TODO

				float3 iblDiffuseResult= 0;
        		float3 iblSpecularResult = 0;

        		float3 IndirectResult = iblDiffuseResult + iblSpecularResult;

				float3 color = DirectLightResult + IndirectResult;

				float d = SAMPLE_TEXTURE2D(_gDepth, sampler_point_clamp, IN.uv).r;
				depthOut = d;

				return float4(color + emission, 1.0);
			}

			ENDHLSL
		}
       
    }
    FallBack "Diffuse"
}
