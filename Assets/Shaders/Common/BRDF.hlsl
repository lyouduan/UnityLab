#ifndef CUSTOM_BRDF_INCLUDED
#define CUSTOM_BRDF_INCLUDED


float NDF(float r2, float nh)
{
    float2 nh2 = nh * nh;
    float demon = PI * (nh2 * (r2 - 1) + 1) * (nh2 * (r2 - 1) + 1);
    
    return r2 / demon;
}

float Pow5(float a)
{
    return a * a * a * a * a;
}
float3 SchlickFresnel(float hv, float3 F0)
{
    return F0 + (1 - F0) * Pow5(1 - hv);
}

float G1(float nv, float r2)
{
    float k = (r2 + 1) * (r2 + 1) / 8.0;
    
    float demon = nv * (1 - k) + k;

    return nv / demon;
}

float GGX(float nv, float nl, float r2)
{
    return G1(nv, r2) * G1(nl, r2);
}

float3 fresnelSchlickRoughness(float cosTheta, float3 F0, float roughness)
{
    return F0 + (max(float3(1.0 - roughness, 1.0 - roughness, 1.0 - roughness), F0) - F0) * Pow5(1.0 - cosTheta);
}
#endif

