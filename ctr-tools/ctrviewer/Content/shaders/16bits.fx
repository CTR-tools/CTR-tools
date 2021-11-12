#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_5_0

Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
};

struct Vertex
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

int NUM_BITS_CHOP = 3;

float4 chopBits(float input)
{
    //get rid of lower bits
    return (float)(((int)(input * 0xFF)) >> NUM_BITS_CHOP << NUM_BITS_CHOP) / 0xFF;
}

float4 pixelShader(Vertex input) : COLOR
{
    float4 color = tex2D(SpriteTextureSampler, input.TextureCoordinates);
    
    //bw test
    //color.rgb = color.r * 0.2126 + color.g * 0.7152 + color.b * 0.0722;

    color.r = chopBits(color.r);
    color.g = chopBits(color.g);
    color.b = chopBits(color.b);

    return color;
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL pixelShader();
    }
}