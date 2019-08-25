using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;

namespace CTRFramework
{
    public class BMPHeader
    {
        char[] magic = new char[2] { 'B', 'M' };
        uint filesize = 0;
        uint reserved = 0;
        uint ptrBitmap = 0; //total size - header size - palette size

        uint headerSize = 0x28;
        int width = 0;
        int height = 0;
        ushort numPlanes = 1;
        ushort bpp = 4;
        uint compression = 0;

        uint bitmapSize = 0;
        uint ppix = 0xEC4;
        uint ppiy = 0xEC4;
        uint numColors = 16;
        uint numColorsImportant = 16;

        byte[] palette = new byte[0];
        byte[] data = new byte[0];

        uint datasize;

        public void Update(int w, int h, uint numCols, ushort bits)
        {
            width = w;
            height = h;

            numColors = numCols;
            numColorsImportant = numCols;
            palette = new byte[numCols * 4];

            bpp = bits;
            datasize = (uint)(width * height * bpp / 8);
            data = new byte[datasize];

            int totalsize = 0x36 + palette.Length + data.Length;
            ptrBitmap = (uint)(0x36 + palette.Length);
        }

        public byte[] GrayScalePalette()
        {
            byte[] x = new byte[numColors * 4];

            for (int i = 0; i < numColors; i++)
            {
                x[i * 4] = (byte)(255 / numColors * i);
                x[i * 4 + 1] = (byte)(255 / numColors * i);
                x[i * 4 + 2] = (byte)(255 / numColors * i);
                x[i * 4 + 3] = 255;
            }

            return x;
        }

        public byte[] RandomBitmap()
        {
            byte[] x = new byte[datasize];

            Random r = new Random();

            for (int i = 0; i < datasize; i++)
            {
                x[i] = (byte)r.Next(255);
            }

            return x;
        }

        public void UpdateData(byte[] pal, byte[] d)
        {
            palette = pal;
            data = d;
        }

        public Color? GetColor(int x)
        {
            if (x < palette.Length / 4)
                return Color.FromArgb(palette[x + 0], palette[x + 1], palette[x + 2]);

            return null;
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(magic);
            bw.Write(filesize);
            bw.Write(reserved);
            bw.Write(ptrBitmap);
            bw.Write(headerSize);
            bw.Write(width);
            bw.Write(-height);
            bw.Write(numPlanes);
            bw.Write(bpp);
            bw.Write(compression);
            bw.Write(bitmapSize);
            bw.Write(ppix);
            bw.Write(ppiy);
            bw.Write(numColors);
            bw.Write(numColorsImportant);
            bw.Write(palette);
            bw.Write(data);
        }


    }

    public class FastBitmap
    {
        public  static Bitmap LockBits(Bitmap bmp, byte[] data)
        {
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            Marshal.Copy(data, 0, ptr, data.Length);
            bmp.UnlockBits(bmpData);
            return bmp;
        }
    }
}