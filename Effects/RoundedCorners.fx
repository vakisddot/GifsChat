sampler2D input : register(s0);
float2 spriteSize;
float4 sourceRect;

float4 RoundedCorners(float2 uv : TEXCOORD) : COLOR
{
    float4 color = tex2D(input, uv);
    float2 spriteUV = (uv * spriteSize - sourceRect.xy) / sourceRect.zw;
    float dist = length((spriteUV - 0.5) * 2);
    float radius = 0.4;
    if (dist > radius)
    {
        color.a = 0;
    }
    return color;
}

technique
{
    pass
    {
        PixelShader = compile ps_2_0 RoundedCorners();
    }
}