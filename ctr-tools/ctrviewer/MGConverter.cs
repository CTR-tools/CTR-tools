using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CTRFramework;
using CTRFramework.Shared;

namespace ctrviewer
{
    class MGConverter
    {
        public static VertexPositionColorTexture ToVptc(CTRFramework.Vertex v, CTRFramework.Shared.Vector2b uv)
        {
            VertexPositionColorTexture mono_v = new VertexPositionColorTexture();
            mono_v.Position = new Microsoft.Xna.Framework.Vector3(v.coord.X, v.coord.Y, v.coord.Z);
            mono_v.Color = new Color(v.color.X / 255.0f, v.color.Y / 255.0f, v.color.Z / 255.0f);
            mono_v.TextureCoordinate = new Microsoft.Xna.Framework.Vector2(uv.X, uv.Y);
            return mono_v;
        }

        public static Color Blend(Color c1, Color c2)
        {
            Color x = Color.White;
            x.R = (byte)((c1.R + c2.R) / 2);
            x.G = (byte)((c1.G + c2.G) / 2);
            x.B = (byte)((c1.B + c2.B) / 2);
            return x;
        }

        //magic
        public unsafe static Texture2D GetTexture(GraphicsDevice gd, System.Drawing.Bitmap bmp)
        {
            int[] imgData = new int[bmp.Width * bmp.Height];
            Texture2D texture = new Texture2D(gd, bmp.Width, bmp.Height);

            // lock bitmap
            System.Drawing.Imaging.BitmapData origdata =
                bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);

            uint* byteData = (uint*)origdata.Scan0;

            // Switch bgra -> rgba
            for (int i = 0; i < imgData.Length; i++)
            {
                byteData[i] = (byteData[i] & 0x000000ff) << 16 | (byteData[i] & 0x0000FF00) | (byteData[i] & 0x00FF0000) >> 16 | (byteData[i] & 0xFF000000);

                if (byteData[i] == 0xFFFF00FF)
                    byteData[i] = 0x00000000;
            }

            // copy data
            System.Runtime.InteropServices.Marshal.Copy(origdata.Scan0, imgData, 0, bmp.Width * bmp.Height);

            //byteData = null;

            // unlock bitmap
            bmp.UnlockBits(origdata);

            texture.SetData(imgData);

            return texture;
        }
    }
}