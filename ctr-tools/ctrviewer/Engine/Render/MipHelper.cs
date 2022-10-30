using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ctrviewer.Engine.Render
{
    public class MipHelper
    {
        public static int MinMipSize = 2;

        /// <summary>
        /// Loads Mipmapped Texture2D object from disk using GDI+ Bitmap.
        /// </summary>
        /// <param name="device">MonoGame GraphicsDevice.</param>
        /// <param name="path">Path to image.</param>
        /// <returns>Texture2D object.</returns>
        public static Texture2D LoadTextureFromFile(GraphicsDevice device, string path, out bool alpha)
        {
            using (var bmp = (Bitmap)Bitmap.FromFile(path))
            {
                return LoadTextureFromBitmap(device, bmp, out alpha);
            }
        }

        /// <summary>
        /// Loads Mipmapped Texture2D object from GDI+ Bitmap object.
        /// </summary>
        /// <param name="device">MonoGame GraphicsDevice.</param>
        /// <param name="bmp">Bitmap object.</param>
        /// <returns>Texture2D object.</returns>
        public static Texture2D LoadTextureFromBitmap(GraphicsDevice device, Bitmap bmp, out bool alpha)
        {
            var mips = new List<Bitmap>();

            mips.Add(bmp);

            int width = bmp.Width >> 1;
            int height = bmp.Height >> 1;

            var old = bmp;

            while (width >= MinMipSize && height >= MinMipSize)
            {
                var mip = new Bitmap(width, height);

                var gr = Graphics.FromImage(mip);
                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;

                var attributes = new ImageAttributes();
                attributes.SetWrapMode(WrapMode.TileFlipXY);

                gr.DrawImage(old, new Rectangle(0, 0, width, height), 0, 0, old.Width, old.Height, GraphicsUnit.Pixel, attributes);
                mips.Add(mip);
                old = mip;

                width >>= 1;
                height >>= 1;
            }

            bool x = false;

            var texture = new Texture2D(device, bmp.Width, bmp.Height, true, SurfaceFormat.Color);

            for (int i = 0; i < mips.Count; i++)
                texture = GetTexture2DFromBitmap(device, mips[i], out x, true, texture, i, fix: false);

            alpha = x;

            return texture;
        }

        /// <summary>
        /// Creates Texture2D object from a given GDI+ Bitmap object.
        /// The idea is to call it first time and get the initial Texture2D object, then call it X times for every MIP level, passing the first Texture2D object.
        /// Initial code sample: https://stackoverflow.com/questions/2869801/is-there-a-fast-alternative-to-creating-a-texture2d-from-a-bitmap-object-in-xna
        /// </summary>
        /// <param name="device">MonoGame GraphicsDevice.</param>
        /// <param name="bitmap">GDI+ Bitmap.</param>
        /// <param name="tex">Optional. Texture to update.</param>
        /// <param name="level">Optional. Mip level to update.</param>
        /// <returns>Texture2D object.</returns>
        public static Texture2D GetTexture2DFromBitmap(GraphicsDevice device, Bitmap bitmap, out bool alpha, bool mipmaps = true, Texture2D tex = null, int level = -1, bool fix = false)
        {
            byte[] bytes = new byte[bitmap.Width * bitmap.Height * 4];

            bitmap = FixBitmap(bitmap, out alpha, out bytes, fix);

            if (tex == null)
            {
                tex = new Texture2D(device, bitmap.Width, bitmap.Height, mipmaps, SurfaceFormat.Color);
                tex.SetData<byte>(bytes, 0, bytes.Length);
            }
            else
            {
                tex.SetData<byte>(level, new Microsoft.Xna.Framework.Rectangle(0, 0, bitmap.Width, bitmap.Height), bytes, 0, bytes.Length);
            }

            return tex;
        }

        public static Bitmap FixBitmap(Bitmap bitmap, out bool alpha, out byte[] bytes, bool swapRB = false)
        {
            BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            //create data buffer 
            bytes = new byte[data.Height * data.Stride];

            // copy bitmap data into buffer
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            alpha = false;

            //BGRA -> RGBA
            for (int i = 0; i < bytes.Length / 4; i++)
            {
                byte B = bytes[i * 4 + 0];
                byte G = bytes[i * 4 + 1];
                byte R = bytes[i * 4 + 2];
                byte A = bytes[i * 4 + 3];

                if (A != 255)
                    alpha = true;

                if (B == 255 && G == 0 && R == 255)
                {
                    B = 0;
                    R = 0;
                    A = 0;

                    alpha = true;
                }

                bytes[i * 4 + 0] = swapRB ? B : R;
                bytes[i * 4 + 1] = G;
                bytes[i * 4 + 2] = swapRB ? R : B;
                bytes[i * 4 + 3] = A;
            }

            // copy data back to bitmap
            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);

            // unlock the bitmap data
            bitmap.UnlockBits(data);

            return bitmap;
        }
        /*
        public static Texture2D Paint(GraphicsDevice graphics, RenderTarget2D rendertarget, SpriteBatch spriteBatch, Texture2D texture)
        {
            if (rendertarget == null) return null;
            if (spriteBatch == null) return null;
            if (texture == null) return null;

            graphics.SetRenderTarget(rendertarget);
            spriteBatch.Draw(
                texture, 
                new Xna.Rectangle(0, 0, rendertarget.Width, rendertarget.Height),
                new Xna.Rectangle(0, 0, texture.Width, texture.Height),
                Xna.Color.White
                );
        }
        */
    }
}