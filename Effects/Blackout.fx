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

float4 Rect(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
	float xMax=uTargetPosition.x+uImageOffset.x/2;
	float xMin=uTargetPosition.x-uImageOffset.x/2;
	float yMax=uTargetPosition.y+uImageOffset.y/2;
	float yMin=uTargetPosition.y-uImageOffset.y/2;
	float2 worldCoords=coords*uScreenResolution+uScreenPosition;
	float xDist=0;float yDist=0;//Manhattan distance
	if(worldCoords.x<xMin)
		xDist=xMin-worldCoords.x;
	if(worldCoords.x>xMax)
		xDist=worldCoords.x-xMax;
	if(worldCoords.y<yMin)
		yDist=yMin-worldCoords.y;
	if(worldCoords.y>yMax)
		yDist=worldCoords.y-yMax;
	
	float factor=saturate((xDist+yDist)/100);
	color=lerp(color,float4(0,0,0,1),factor)*uOpacity;

    return color;
}
float4 Circular(float2 coords:TEXCOORD0):COLOR0
{
	float2 targetCoords = (uTargetPosition - uScreenPosition) / uScreenResolution;
	//float2 targetCoords = float2(0.5,0.5);
    float2 centreCoords = (coords - targetCoords) * (uScreenResolution / uScreenResolution.y);
	float length = sqrt(dot(centreCoords, centreCoords));
    float4 color = tex2D(uImage0, coords);
	float factor=saturate((length*uScreenResolution.y-uIntensity)/100);
	
    color=lerp(color,float4(0,0,0,1),factor)*uOpacity;
	
	return color;
}

technique Technique1
{
    pass RectBlackout
    {
        PixelShader = compile ps_2_0 Rect();
    }
	pass CircularBlackout
	{
        PixelShader = compile ps_2_0 Circular();
    }

}