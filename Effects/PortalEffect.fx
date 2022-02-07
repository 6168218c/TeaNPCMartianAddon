sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
float opacity;
float2 RotateBy(float2 vec, float r)
{
	return mul(vec, float2x2(cos(r), -sin(r), sin(r), cos(r)));
}

float4 Main(float2 coords:TEXCOORD0):COLOR0
{
	float2 targetCoords = float2(0.5,0.5);
    float2 dist = coords - targetCoords;
	float length = sqrt(dot(dist, dist));
    float4 color = tex2D(uImage0, coords);
	float factor=0;
	if(length>=0.12&&length<=0.48)
	{
		float2 rotation=atan2(dist.y,dist.x);
		factor=abs(length-0.30)/0.1;
		float2 tarCoords=float2(0,0);
		tarCoords.x=(length-0.12)/0.36;
		tarCoords.y=rotation/3.14;
		float4 newColor=tex2D(uImage1,tarCoords);
		color=newColor*smoothstep(0,1,factor)*opacity;
	}
	else if(length<0.12)
	{
		color=lerp(color,tex2D(uImage2,targetCoords+RotateBy(dist,length*2)),0.6);
	}
	
	return color;
}

technique Technique1
{
    pass PortalEffect
    {
        PixelShader = compile ps_2_0 Main();
    }
}