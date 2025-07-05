Shader "Custom/CustomPBR"
{
    Properties
    {
        _Tint ("Color", Color) = (1,1,1,1)
        _LUT ("LUT", 2D) =  "white" {}
        _MainTex ("Texture", 2D) = "white" {}
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
        [Gamma] _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
		Tags {
			"LightMode" = "ForwardBase"
		}
        CGPROGRAM

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0
        #pragma vertex vertexPass
        #pragma fragment fragmentPass

        #include "UnityPBSLighting.cginc"

        struct Input
        {
            float4 vertex : POSITION;
		    float3 normal : NORMAL;
            float2 uv : TEXCOORD0;
        };
        
        struct ps
        {
            float4 vertex : SV_POSITION;
	    	float2 uv : TEXCOORD0;
	    	float3 normal : TEXCOORD1;
		    float3 worldPos : TEXCOORD2;
        };

        float4 _Tint;
        float  _Metallic;
        float _Smoothness;
        sampler2D  _MainTex;
        sampler2D  _LUT;
        float4 _MainTex_ST;

        ps vertexPass(Input vin)
        {
            ps vout;
            vout.vertex =  UnityObjectToClipPos(vin.vertex);
            vout.worldPos = mul(unity_ObjectToWorld, vin.vertex);
            vout.uv = TRANSFORM_TEX(vin.uv, _MainTex);
            vout.normal = UnityObjectToWorldNormal(vin.normal);
		    vout.normal = normalize(vout.normal);
            return vout;
        }
        
        float3 fresnelSchlickRoughness(float cosTheta, float3 F0, float roughness)
        {
	        return F0 + (max(float3(1.0 - roughness, 1.0 - roughness, 1.0 - roughness), F0) - F0) * pow(1.0 - cosTheta, 5.0);
        }

        float4 fragmentPass(ps i) : SV_TARGET
        {

            i.normal = normalize(i.normal);
	        float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
        	float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
	        float3 lightColor = _LightColor0.rgb;
	        float3 halfVector = normalize(lightDir + viewDir);  //半角向量

	        float perceptualRoughness = 1 - _Smoothness;

	        float roughness = perceptualRoughness * perceptualRoughness;
	        float squareRoughness = roughness * roughness;

	        float nl = max(saturate(dot(i.normal, lightDir)), 0.000001);//防止除0
        	float nv = max(saturate(dot(i.normal, viewDir)), 0.000001);
	        float vh = max(saturate(dot(viewDir, halfVector)), 0.000001);
	        float lh = max(saturate(dot(lightDir, halfVector)), 0.000001);
	        float nh = max(saturate(dot(i.normal, halfVector)), 0.000001);

            float3 Albedo = _Tint * tex2D(_MainTex, i.uv);

            // NDF
            float lerpSquareRoughness = pow(lerp(0.002, 1, roughness), 2);//Unity把roughness lerp到了0.002
            float D = lerpSquareRoughness / (pow((pow(nh, 2) * (lerpSquareRoughness - 1) + 1), 2) * UNITY_PI);

            // Geomerty
            float kInDirectLight = pow(squareRoughness + 1, 2) / 8.0;
            float G1 = nl / lerp(nl, 1, kInDirectLight);
            float G2 = nv / lerp(nv, 1, kInDirectLight);
            float G = G1 * G2;

            // Fresnel_Schlick
            float3 F0 = lerp(unity_ColorSpaceDielectricSpec.rgb, Albedo, _Metallic);
            float3 F = F0 + (1 - F0) * exp2((-5.55473 * vh - 6.98316) * vh);
            float3 SpecularResult = (D * G * F * 0.25) / (nv * nl);
            

            float3 kd = (1 - F)*(1 - _Metallic);
        	float3 specColor = SpecularResult * lightColor * nl * UNITY_PI;
            float3 diffColor = kd * Albedo * lightColor * nl;
        	float3 DirectLightResult = diffColor + specColor;
            
            half3 ambient_contrib = ShadeSH9(float4(i.normal, 1.0));
            float3 ambient = 0.03 * Albedo;
            float3 iblDiffuse = max(half3(0, 0, 0), ambient.rgb + ambient_contrib);

            float mip_roughness =perceptualRoughness * (1.7 - 0.7 * perceptualRoughness);
            float3 reflectVec = reflect(-viewDir, i.normal);

            half mip = mip_roughness * UNITY_SPECCUBE_LOD_STEPS;
            half4 rgbm = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, reflectVec, mip);
            float3 iblSpecular = DecodeHDR(rgbm, unity_SpecCube0_HDR);
            
            half surfaceReduction = 1.0 / (roughness * roughness + 1.0);
            float oneMinusReflectivity = unity_ColorSpaceDielectricSpec.a - unity_ColorSpaceDielectricSpec.a * _Metallic;
            half grazingTerm = saturate(_Smoothness + (1 - oneMinusReflectivity));

            //float2 envBRDF = tex2D(_LUT, float2(lerp(0, 0.99, nv), lerp(0, 0.99, roughness))).rg;

            float3 Flast = fresnelSchlickRoughness(max(nv, 0.0), F0, roughness);
            float3 kdLast = (1 - Flast) * (1 - _Metallic);
            float3 iblDiffuseResult = iblDiffuse * kdLast *Albedo;

        	//float3 iblSpecularResult = iblSpecular * (Flast * envBRDF.r + envBRDF.g);
        	float3 iblSpecularResult = surfaceReduction * iblSpecular * FresnelLerp(F0, grazingTerm, nv);

        	float3 IndirectResult = iblDiffuseResult + iblSpecularResult;

        	float4 result = float4(DirectLightResult + IndirectResult, 1);

        	return result;
        }
      
        ENDCG
    }
    }
}
