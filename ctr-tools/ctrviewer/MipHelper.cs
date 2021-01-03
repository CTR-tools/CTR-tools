using System;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace ctrviewer
{
    public class MipHelper
    {
        /// <summary>
        /// Loads Mipmapped Texture2D object from image file using GDI+ Bitmap.
        /// </summary>
        /// <param name="device">MonoGame GraphicsDevice.</param>
        /// <param name="path">Path to image.</param>
        /// <returns>Texture2D object.</returns>
        public static Texture2D LoadTextureFromFile(GraphicsDevice device, string path)
        {
            using (Bitmap bmp = (Bitmap)Bitmap.FromFile(path))
            {
                Texture2D t = GetTexture2DFromBitmap(device, bmp);

                int i = 1;

                int width = bmp.Width;
                int height = bmp.Height;

                do
                {
                    width /= 2;
                    height /= 2;

                    using (Bitmap mip = new Bitmap(width, height))
                    {
                        Graphics gr = Graphics.FromImage(mip);
                        gr.SmoothingMode = SmoothingMode.AntiAlias;
                        gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        gr.CompositingMode = CompositingMode.SourceOver;

                        var attributes = new ImageAttributes();
                        attributes.SetWrapMode(WrapMode.TileFlipXY);

                        gr.DrawImage(bmp, new System.Drawing.Rectangle(0, 0, mip.Width, mip.Height), 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attributes);
                        t = GetTexture2DFromBitmap(device, mip, t, i);
                    }

                    i++;
                }
                while (width >= 2 && height >= 2);

                return t;
            }
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
        private static Texture2D GetTexture2DFromBitmap(GraphicsDevice device, Bitmap bitmap, Texture2D tex = null, int level = -1)
        {
            BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            //create data buffer 
            byte[] bytes = new byte[data.Height * data.Stride];

            // copy bitmap data into buffer
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            // unlock the bitmap data
            bitmap.UnlockBits(data);

            //BGRA -> RGBA
            for (int i = 0; i < bytes.Length / 4; i++)
            {
                byte x = bytes[i * 4 + 0];
                bytes[i * 4 + 0] = bytes[i * 4 + 2];
                bytes[i * 4 + 2] = x;
            }

            if (tex == null)
            {
                tex = new Texture2D(device, bitmap.Width, bitmap.Height, true, SurfaceFormat.Color);
                tex.SetData<byte>(bytes, 0, bytes.Length);
            }
            else
            {
                tex.SetData<byte>(level, new Microsoft.Xna.Framework.Rectangle(0, 0, bitmap.Width, bitmap.Height), bytes, 0, bytes.Length);
            }

            return tex;
        }

    }
}
