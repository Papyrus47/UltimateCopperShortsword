sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float uOpacity;
float3 uSecondaryColor;
float uTime;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uImageOffset;
float uIntensity;
float uProgress;
float2 uDirection;
float2 uZoom;
float2 uImageSize0;
float2 uImageSize1;

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    if (!any(color))
    {
        return color;
    }
    color.r += uOpacity * 0.01f;
    color.b += uOpacity * 0.01f;
    color.g += uOpacity * 0.1f;
    color.a += uOpacity * 0.01f;
    return color;
}
float4 PixelShaderTwo(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    if (!any(color))
    {
        return color;
    }
    color.r += uOpacity * 0.01f;
    color.b += uOpacity * 0.01f;
    color.g += uOpacity * 0.3f;
    color.a += uOpacity * 0.01f;
    return color;
}

technique Technique1
{
    pass GreenSword
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
    pass GreenSwordTwo
    {
        PixelShader = compile ps_2_0 PixelShaderTwo();

    }
}