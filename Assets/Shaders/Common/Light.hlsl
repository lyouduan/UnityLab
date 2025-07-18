﻿#ifndef CUSTOM_LIGHT_INCLUDED
#define CUSTOM_LIGHT_INCLUDED

float3 _DirectionalLightColor;
float3 _DirectionalLightDirection;

struct Light
{
    float3 color;
    float3 direction;
};

Light GetDirectionalLight()
{
    Light light;
    light.color = _DirectionalLightColor;
    light.direction = _DirectionalLightDirection;
    return light;
}

#endif
