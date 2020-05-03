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

    class Samplers
    {
        public static bool EnableBilinear = true;

        public static SamplerState Default = new SamplerState();
        public static SamplerState Animated = new SamplerState();

        public static void Refresh()
        {
            Default = new SamplerState();
            Default.Filter = EnableBilinear ? TextureFilter.Anisotropic : TextureFilter.Point;
            Default.MaxAnisotropy = 16;
            Default.MaxMipLevel = 8;
            Default.MipMapLevelOfDetailBias = 0;
            Default.AddressU = TextureAddressMode.Clamp;
            Default.AddressV = TextureAddressMode.Clamp;

            Animated = new SamplerState();
            Animated.Filter = EnableBilinear ? TextureFilter.Anisotropic : TextureFilter.Point;
            Animated.MaxAnisotropy = 16;
            Animated.MaxMipLevel = 8;
            Animated.MipMapLevelOfDetailBias = 0;
            Animated.AddressU = TextureAddressMode.Clamp;
            Animated.AddressV = TextureAddressMode.Wrap;
        }

        public static void SetToDevice(GraphicsDeviceManager graphics, EngineSampler sampler)
        {
            switch (sampler)
            {
                case EngineSampler.Default: graphics.GraphicsDevice.SamplerStates[0] = Default; break;
                case EngineSampler.Animated: graphics.GraphicsDevice.SamplerStates[0] = Animated; break;
                default: throw new Exception("Unexpected sampler value");
            }

            graphics.ApplyChanges();
        }
    }
}
