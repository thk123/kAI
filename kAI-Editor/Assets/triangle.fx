float4 offset;

float4 VShader(float4 position : POSITION) : SV_POSITION
{
	/*float4 scaledPos = offset.z * position;
	return scaledPos + float4(offset.x, offset.y, 0, 0);*/

	return position;
	
}

float4 PShader(float4 position : SV_POSITION) : SV_Target
{
	return position;
}