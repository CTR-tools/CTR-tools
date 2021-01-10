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
        /// Loads Mipmapped Texture2D object from disk using GDI+ Bitmap.
        /// </summary>
        /// <param name="device">MonoGame GraphicsDevice.</param>
        /// <param name="path">Path to image.</param>
        /// <returns>Texture2D object.</returns>
        public static Texture2D LoadTextureFromFile(GraphicsDevice device, string path, out bool alpha)
        {
            using (Bitmap bmp = (Bitmap)Bitmap.FromFile(path))
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
            Texture2D t = GetTexture2DFromBitmap(device, bmp, out alpha);

            bool x = alpha;

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
                    t = GetTexture2DFromBitmap(device, mip, out alpha, true, t, i);
                }

                i++;
            }
            while (width >= 2 && height >= 2);

            alpha = x;

            return t;
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
        public static Texture2D GetTexture2DFromBitmap(GraphicsDevice device, Bitmap bitmap, out bool alpha, bool mipmaps = true, Texture2D tex = null, int level = -1)
        {
            BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            //create data buffer 
            byte[] bytes = new byte[data.Height * data.Stride];

            // copy bitmap data into buffer
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            // unlock the bitmap data
            bitmap.UnlockBits(data);

            alpha = false;

            //BGRA -> RGBA
            for (int i = 0; i < bytes.Length / 4; i++)
            {
                byte B = bytes[i * 4 + 0];
                byte G = bytes[i * 4 + 1];
                byte R = bytes[i * 4 + 2];
                byte A = bytes[i * 4 + 3];

                if (B == 255 && R == 255)
                {
                    B = 0;
                    R = 0;
                    A = 0;

                    alpha = true;
                }

                bytes[i * 4 + 0] = R;
                bytes[i * 4 + 1] = G;
                bytes[i * 4 + 2] = B;
                bytes[i * 4 + 3] = A;
            }

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

    }
}
