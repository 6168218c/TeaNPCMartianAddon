sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float3 uSecondaryColor;
float uOpacity : register(C0);
float uSaturation;
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;

// This is a shader. You are on your own with shaders. Compile shaders in an XNB project.

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
	float4 color = tex2D(uImage0, coords);
	if (!any(color))
		return color;
	if (any(float4(uColor,1)))
		color *= float4(uColor,1);
	float4 color1= tex2D( uImage1 , coords.xy);

	float readRed = 1 - uOpacity;

	if(color1.r < readRed){
		color *= 0;//transparent
	}
	return color;
}


technique Technique1 
{
    pass FadeIn
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }

}
