
float4 VShader(float4 position : POSITION) : SV_POSITION
{


	return position;
	
}

float4 PShader(float4 position : SV_POSITION) : SV_Target
{
	return float4(0.251f,0.251f,0.251f,1);
}