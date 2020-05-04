using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ctrviewer
{
    public enum EngineSampler
    {
        Default, Animated
    }

    public enum EngineRasterizer
    {
        Default, Wireframe
    }

    class Samplers
    {
        public static bool EnableWireframe = false;
        public static bool EnableBilinear = true;



        public static SamplerState DefaultSampler = new SamplerState();
        public static SamplerState AnimatedSampler = new SamplerState();

        public static RasterizerState DefaultRasterizer = new RasterizerState();
        public static RasterizerState WireframeRasterizer = new RasterizerState();


        public static void InitRasterizers()
        {
            DefaultRasterizer = new RasterizerState();
            DefaultRasterizer.FillMode = FillMode.Solid;
            DefaultRasterizer.CullMode = CullMode.CullCounterClockwiseFace;

            WireframeRasterizer = new RasterizerState();
            WireframeRasterizer.FillMode = FillMode.WireFrame;
            WireframeRasterizer.CullMode = CullMode.None;
        }


        public static void Refresh()
        {
            DefaultSampler = new SamplerState();
            DefaultSampler.Filter = EnableBilinear ? TextureFilter.Anisotropic : TextureFilter.Point;
            DefaultSampler.MaxAnisotropy = 16;
            DefaultSampler.MaxMipLevel = 8;
            DefaultSampler.MipMapLevelOfDetailBias = 0;
            DefaultSampler.AddressU = TextureAddressMode.Clamp;
            DefaultSampler.AddressV = TextureAddressMode.Clamp;

            AnimatedSampler = new SamplerState();
            AnimatedSampler.Filter = EnableBilinear ? TextureFilter.Anisotropic : TextureFilter.Point;
            AnimatedSampler.MaxAnisotropy = 16;
            AnimatedSampler.MaxMipLevel = 8;
            AnimatedSampler.MipMapLevelOfDetailBias = 0;
            AnimatedSampler.AddressU = TextureAddressMode.Clamp;
            AnimatedSampler.AddressV = TextureAddressMode.Wrap;
        }

        public static void SetToDevice(GraphicsDeviceManager graphics, EngineRasterizer rasterizer)
        {
            switch (rasterizer)
            {
                case EngineRasterizer.Default: graphics.GraphicsDevice.RasterizerState = DefaultRasterizer; break;
                case EngineRasterizer.Wireframe: graphics.GraphicsDevice.RasterizerState = WireframeRasterizer; break;
                default: throw new Exception("Unexpected rasterizer value");
            }

            //graphics.ApplyChanges();
        }

        public static void SetToDevice(GraphicsDeviceManager graphics, EngineSampler sampler)
        {
            switch (sampler)
            {
                case EngineSampler.Default: graphics.GraphicsDevice.SamplerStates[0] = DefaultSampler; break;
                case EngineSampler.Animated: graphics.GraphicsDevice.SamplerStates[0] = AnimatedSampler; break;
                default: throw new Exception("Unexpected sampler value");
            }

            graphics.ApplyChanges();
        }
    }
}
