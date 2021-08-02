sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);
float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;

float2 RotateBy(float2 vec, float r)
{
	return mul(vec, float2x2(cos(r), -sin(r), sin(r), cos(r)));
}

float4 Main(float2 coords : TEXCOORD0) : COLOR0
{
    float2 targetCoords = (uTargetPosition - uScreenPosition) / uScreenResolution;
	//float2 targetCoords = float2(0.5,0.5);
    float2 centreCoords = (coords - targetCoords) * (uScreenResolution / uScreenResolution.y);
	float length = sqrt(dot(centreCoords, centreCoords));
    float4 color = tex2D(uImage0, coords);
	
    if (length < uProgress) {
        float2 tarPos = targetCoords+RotateBy(centreCoords, 1.57*uIntensity*(1-length/uProgress))/ (uScreenResolution / uScreenResolution.y);
		color=tex2D(uImage0,tarPos);
    }

    return color;
}

technique Technique1
{
    pass CircularDistort
    {
        PixelShader = compile ps_2_0 Main();
    }

}