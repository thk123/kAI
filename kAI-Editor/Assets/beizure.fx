float4 DeCasteljauBezierLine(int n,float t)
{
	matrix BernsteinCurveBasis={{1.0,0.0,0.0,0.0},
	{0.0,0.0,0.0,0.0},
	{0.0,0.0,0.0,0.0},
	{0.0,0.0,0.0,0.0}};
	float u = 1-t;
	for(int j=1;j<=n;j++)
	{
		for(int i=0;i<=j;i++)
		{
			if(i==0)
			{
				BernsteinCurveBasis[j][i] = BernsteinCurveBasis[j-1][i]*u;
			}
			else
			{
				BernsteinCurveBasis[j][i] = BernsteinCurveBasis[j-1][i]*u + BernsteinCurveBasis[j-1][i-1]*t;
			}
		}
	}
	return float4(BernsteinCurveBasis[n][0],BernsteinCurveBasis[n][1],BernsteinCurveBasis[n][2],BernsteinCurveBasis[n][3]);
}

struct GS_OUT
{
	float4 pos : GS_OUTPOS;
};	

[maxvertexcount(100)]
void BezierGS(lineadj float4 input [4] : VS_OUTPUT, inout LineStream < GS_OUT > MyLineStream)
{
	for(int i=0;i<4;i++)
	{
		GS_OUT me =(GS_OUT)0;
		me.pos = input[i];
		MyLineStream.Append(me);
	}
	MyLineStream.RestartStrip();

	/*float n = 90;
	for(float t = 0.0; t<1.0; t = t+ 1.0/n)
	{
		GS_OUT value = (GS_OUT)0;
		float4 b = DeCasteljauBezierLine(1,t);

		value.pos = input[0]*b.x + input[1] * b.y + input[2]*b.z +input[3]*b.w ;
		MyLineStream.Append(value);
	}

	MyLineStream.RestartStrip();

	for(int i=0;i<4;i++)
	{
		//input[i].Color = float4(1.0,0.5,0.5,1.0);
		GS_OUT me =(GS_OUT)0;
		me.pos = input[i];
		MyLineStream.Append(me);
	}
	MyLineStream.RestartStrip();*/
}
