


struct TKGS_OUT
{
	float4 pos : SV_POSITION;
};

float4 VShader(float4 position : POSITION) : SV_POSITION
{


	return position;
	
}

cbuffer ConstBuffer : register(c0)
{
	float lineThickness;
}


[maxvertexcount(6)]
void mainGS(line float4 input[2] : SV_POSITION, inout TriangleStream<TKGS_OUT> OutputStream )
{
	TKGS_OUT a;
	float xDiff = abs(input[1].x - input[0].x);
	float yDiff = abs(input[1].y - input[0].y);
	float4 norm = normalize(float4(yDiff, xDiff, 0.0f, 0.0f));

	
	a.pos = input[0] + lineThickness * norm;
	OutputStream.Append(a);

	a.pos = input[1] + lineThickness * norm;
	OutputStream.Append(a);

	a.pos = input[0] - lineThickness * norm;
	OutputStream.Append(a);

	a.pos = input[1] - lineThickness * norm;
	OutputStream.Append(a);

	OutputStream.RestartStrip();
	
}

float4 PShader(TKGS_OUT position) : SV_Target
{
	return position.pos;
}