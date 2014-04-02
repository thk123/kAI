
struct VS_INPUT
{
	float4 position : POSITION;
	float4 col : COLOR0;
};

struct VS_OUTPUT
{
	float4 position : SV_POSITION;
	float4 col : COLOR0;
};

VS_OUTPUT VShader(VS_INPUT input)
{
	VS_OUTPUT result;
	result.position = input.position;
	result.col  = input.col;
	return result;
}

float4 PShader(VS_OUTPUT input) : SV_Target
{
	return input.col;
}