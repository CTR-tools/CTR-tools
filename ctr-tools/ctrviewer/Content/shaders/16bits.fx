#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_5_0

//scanlines effects params
#define SCANLINES 1
#define SCANLINE_WIDTH 2
#define SCANLINE_BRIGHTNESS 0.5
#define BUFFER_HEIGHT 216

Texture2D SpriteTexture;

//==============================================
//SHADER SETTINGS
//==============================================

bool	bMirrorX		= false;

bool	bChopColor		= true;
int		iChopColorBits	= 5;

bool	bMonochrome		= false;

bool	bInvert			= false;

bool	bFadeToBlack	= false;
float	fFader			= 1;

bool	bMultiply		= false;
float4	fMult			= float4(1.0, 1.0, 1.0, 1.0);

float4	cTod			= float4(1.0, 1.0, 1.0, 1.0);

//==============================================

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

float max = 0;

float chopComponent(float input, int bits)
{
    max = pow(2, bits) - 1;
    return (int)(input * max) / max;
}


float2 getUV(Vertex input)
{
	float2 uv = input.TextureCoordinates;
	
	if (bMirrorX)
		uv = float2(1.0 - uv.x, uv.y);

	return uv;
}

float4 chopColor(float4 pixel)
{
	pixel.r = chopComponent(pixel.r, iChopColorBits);
	pixel.g = chopComponent(pixel.g, iChopColorBits);
	pixel.b = chopComponent(pixel.b, iChopColorBits);

	return pixel;
}

float4 main(Vertex input) : COLOR
{
    float4 pixel = tex2D(SpriteTextureSampler, getUV(input));
	if (bChopColor) pixel = chopColor(pixel);

    //scanline
    //if ((int)(input.TextureCoordinates.y * BUFFER_HEIGHT * 2) % SCANLINE_WIDTH >= SCANLINE_WIDTH / 2)
    //if (input.TextureCoordinates.y % 2 == 0.0)
    //    pixel = pixel * SCANLINE_BRIGHTNESS;

    pixel = pixel * cTod;
	

	if (bMonochrome)
    {
        pixel.rgb = 0.2126 * pixel.r + 0.7152 * pixel.g + 0.0722 * pixel.b;
    }
	
	if (bMultiply)
    {
        pixel = pixel * fMult;
    }
	
	if (bInvert)
    {
        pixel = 1 - pixel;
    }
	
	if (bFadeToBlack)
    {
        pixel = pixel * fFader;
    }

    return pixel;
}


/*

shader_type spatial;
render_mode unshaded, shadows_disabled, depth_test_disable, depth_draw_never;

uniform int color_depth : hint_range(1, 8) = 5;
uniform bool dithering = true;
uniform int resolution_scale = 4;

int dithering_pattern(ivec2 fragcoord) {
	const int pattern[] = {
		-4, +0, -3, +1,
		+2, -2, +3, -1,
		-3, +1, -4, +0,
		+3, -1, +2, -2
	};

	int x = fragcoord.x % 4;
	int y = fragcoord.y % 4;

	return pattern[y * 4 + x];
}

void vertex() {
	POSITION = vec4(VERTEX, 1.0);
}

void fragment() {
	ivec2 uv = ivec2(FRAGCOORD.xy / float(resolution_scale));
	vec3 color = texelFetch(SCREEN_TEXTURE, uv * resolution_scale, 0).rgb;

	// Convert from [0.0, 1.0] range to [0, 255] range
	ivec3 c = ivec3(round(color * 255.0));

	// Apply the dithering pattern
	if (dithering) {
		c += ivec3(dithering_pattern(uv));
	}

	// Truncate from 8 bits to color_depth bits
	c >>= (8 - color_depth);

	// Convert back to [0.0, 1.0] range
	ALBEDO = vec3(c) / float(1 << color_depth);
}

*/



technique
{
    pass
    {
        PixelShader = compile PS_SHADERMODEL main();
    }
}