Shader "Custom/DPBR"
{
    Properties
    {
        _Tint ("Color", Color) = (1,1,1,1)
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
        
        float4 fragmentPass(ps i) : SV_TARGET
        {
            i.normal = normalize(i.normal);
		    float3 lightDir = _WorldSpaceLightPos0.xyz;
		    float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
		    float3 lightColor = _LightColor0.rgb;

		    float3 specularTint;
		    float oneMinusReflectivity;
		    float3 albedo = tex2D(_MainTex, i.uv).rgb * _Tint.rgb;
		    albedo = DiffuseAndSpecularFromMetallic( // 从金属度生成漫反射颜色，镜面反射颜色等
			    albedo, _Metallic, specularTint, oneMinusReflectivity
		    );
				
		    UnityLight light;
		    light.color = lightColor;
		    light.dir = normalize(lightDir);
		    light.ndotl = DotClamped(i.normal, lightDir);
		    UnityIndirect indirectLight;
		    indirectLight.diffuse = 0;
	    	indirectLight.specular = 0;

		    return UNITY_BRDF_PBS( //生成直接光pbr结果
		    	albedo, specularTint,
		    	oneMinusReflectivity, _Smoothness,
				i.normal, viewDir,
		    	light, indirectLight
		    );
        }
      
        ENDCG
    }
    }
}
