
struct VS_IN
{
	float2 posL    : POSITION;
	float4 colorC  : COLOR;
};

struct VS_OUT
{
	float4 posH    : SV_POSITION;
	float4 colorC  : COLOR;
};

VS_OUT _VertexShader(VS_IN vIn)
{
	VS_OUT vOut;

	vOut.posH = float4(vIn.posL.x, vIn.posL.y, 0.0f, 1.0f);

	// Output 
	vOut.colorC = vIn.colorC;

	return vOut;
}

