using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ctrviewer.Engine.Render
{
    public enum EngineSampler
    {
        Default, Animated
    }

    public enum EngineRasterizer
    {
        Default, Wireframe, DoubleSided
    }

    class Samplers
    {
        public static bool EnableWireframe = false;
        public static bool EnableFiltering = true;



        public static SamplerState DefaultSampler = new SamplerState();
        public static SamplerState AnimatedSampler = new SamplerState();

        public static RasterizerState DefaultRasterizer = new RasterizerState();
        public static RasterizerState WireframeRasterizer = new RasterizerState();
        public static RasterizerState DoubleSidedRasterizer = new RasterizerState();

        public static void InitRasterizers()
        {
            DefaultRasterizer = new RasterizerState();
            DefaultRasterizer.FillMode = FillMode.Solid;
            DefaultRasterizer.CullMode = CullMode.CullCounterClockwiseFace;

            WireframeRasterizer = new RasterizerState();
            WireframeRasterizer.FillMode = FillMode.WireFrame;
            WireframeRasterizer.CullMode = CullMode.None;

            DoubleSidedRasterizer = new RasterizerState();
            DoubleSidedRasterizer.FillMode = FillMode.Solid;
            DoubleSidedRasterizer.CullMode = CullMode.None;
        }


        public static void Refresh()
        {
            DefaultSampler = new SamplerState();
            DefaultSampler.Filter = EnableFiltering ? TextureFilter.Anisotropic : TextureFilter.PointMipLinear;
            DefaultSampler.MipMapLevelOfDetailBias = -10;

            DefaultSampler.AddressU = TextureAddressMode.Clamp;
            DefaultSampler.AddressV = TextureAddressMode.Clamp;

            AnimatedSampler = new SamplerState();
            AnimatedSampler.Filter = EnableFiltering ? TextureFilter.Anisotropic : TextureFilter.PointMipLinear;

            AnimatedSampler.AddressU = TextureAddressMode.Clamp;
            AnimatedSampler.AddressV = TextureAddressMode.Wrap;
        }

        public static void SetToDevice(GraphicsDeviceManager graphics, EngineRasterizer rasterizer)
        {
            switch (rasterizer)
            {
                case EngineRasterizer.Default: graphics.GraphicsDevice.RasterizerState = DefaultRasterizer; break;
                case EngineRasterizer.Wireframe: graphics.GraphicsDevice.RasterizerState = WireframeRasterizer; break;
                case EngineRasterizer.DoubleSided: graphics.GraphicsDevice.RasterizerState = DoubleSidedRasterizer; break;
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
