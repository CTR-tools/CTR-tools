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

float4 color5to8(float input)
{
    return (float)(((int)(input * 0xFF) * 249 + 1014) >> 11);
}

//5to8
//((color * 527u) + 23u) >> 6
//8to5
//((color * 249u) + 1014u) >> 11

float4 pixelShader(Vertex input) : COLOR
{
    float4 color = tex2D(SpriteTextureSampler, input.TextureCoordinates);
    
    //bw test
    //color.rgb = color.r * 0.2126 + color.g * 0.7152 + color.b * 0.0722;

    color.r = color5to8(color.r);
    color.g = color5to8(color.g);
    color.b = color5to8(color.b);

    return color;
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL pixelShader();
    }
}