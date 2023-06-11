#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_5_0

float4x4 World;
float4x4 View;
float4x4 Projection;

float VertexColorEnabled = 0;

float AmbientIntensity = 0.5f;
float4 AmbientColor = float4(0.5f, 0.2f, 0.1f, 1.0f);

float bDiffuseMapEnabled = 1;
texture DiffuseMap;
float DiffuseLightIntensity = 2.0f;

float bNormalMapEnabled = 1;
texture NormalMap;

struct Vertex
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

sampler2D TextureSampler = sampler_state {
    Texture = <DiffuseMap>;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
    MaxAnisotropy = 16;
};

sampler2D NormalSampler = sampler_state {
    Texture = <NormalMap>;
    MagFilter = Anisotropic;
    MinFilter = Anisotropic;
    AddressU = Wrap;
    AddressV = Wrap;
    MaxAnisotropy = 16;
};


Vertex VertexShaderFunction(Vertex input)
{
    Vertex output;

    output.Position = mul(mul(mul(input.Position, World), View), Projection);
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;

    return output;
}

float4 PixelShaderFunction(Vertex input) : COLOR0
{
    float4 pixel;
    
    if (VertexColorEnabled)
    {
        pixel = input.Color;
    }
    else
    {
        pixel = float4(1, 1, 1, 1);
    }

    if (bDiffuseMapEnabled > 0)
        pixel *= tex2D(TextureSampler, input.TextureCoordinates) * DiffuseLightIntensity;

    //if (bNormalMapEnabled > 0)
    //    pixel = pixel * tex2D(NormalSampler, input.TextureCoordinates) * DiffuseLightIntensity;

    return pixel;
}

technique Default
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
        PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
    }
}