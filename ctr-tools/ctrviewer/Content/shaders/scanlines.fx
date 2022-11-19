#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_5_0

#define VERTICAL_WIDTH 1080
#define BRIGHTNESS 0.5


Texture2D SpriteTexture;

sampler2D tex = sampler_state
{
    Texture = <SpriteTexture>;
};

struct Vertex
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float chopComponent(float input, int bits)
{
    float max = pow(2, bits) - 1;
    return (int)(input * max) / max;
}

float4 chopColor(Vertex input) : COLOR
{
    float2 uv = input.TextureCoordinates;

    if ((int)(uv.y * VERTICAL_WIDTH) % 2 > 0)
        return tex2D(tex, uv);
    else
        return tex2D(tex, uv) * BRIGHTNESS;
}

technique
{
    pass
    {
        PixelShader = compile PS_SHADERMODEL chopColor();
    }
}