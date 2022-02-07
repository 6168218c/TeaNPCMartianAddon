sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
float4x4 uTransform;
float uTime;
float uTimeVert;
float alpha;
bool complexTexture;

struct VSInput
{
	float2 Pos : POSITION0;
    float4 Color : COLOR0;
	float3 Texcoord : TEXCOORD0;
};

struct PSInput
{
	float4 Pos : SV_POSITION;
    float4 Color : COLOR0;
	float3 Texcoord : TEXCOORD0;
};

PSInput VSMain(VSInput input)
{
	PSInput output;
	output.Texcoord = input.Texcoord;
    output.Color = input.Color;
	output.Pos = mul(float4(input.Pos, 0, 1), uTransform);
	return output;
}
//originally from @yiyang233
float4 PSMain(PSInput input) : COLOR0
{
	float3 coord = input.Texcoord;
	float4 c = tex2D(uImage0, float2(coord.x + uTime , coord.y+uTimeVert));
    //c += tex2D(uImage2, float2(1 - coord.x * 5, coord.y));
	//c *= coord.z;
	//c *= tex2D(uImage1, float2(c.r,0.5));//颜色图
	if (complexTexture)
		c += tex2D(uImage2, float2(1 - coord.x * 5, coord.y));
	c *= input.Color;
	c *= alpha;
	return c * 1.2;
}

technique Technique1 {
	pass Trail {
		VertexShader = compile vs_2_0 VSMain();
		PixelShader = compile ps_2_0 PSMain();
	}
}