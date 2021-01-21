using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;

namespace CTRFramework.Vram
{
    /// <summary>
    /// Please note this is super hacky implementation of PSX Tim format hardcoded for 4 bits used in Crash Team Racing.
    /// You can load any tim files, but overall code will not work properly with anything other than 4 bits.
    /// </summary>
    public class Tim : IRead
    {

        public Dictionary<string, Bitmap> textures = new Dictionary<string, Bitmap>();

        public uint magic;
        public uint flags;

        public uint clutsize;
        public Rectangle clutregion;
        public ushort[] clutdata;

        public uint datasize;

        public Rectangle region;

        public ushort[] data;
        public ushort[] realdata;

        public byte bpp
        {
            get
            {
                switch (flags & 3)
                {
                    case 0: return 4;
                    case 1: return 8;
                    case 2: return 16;
                    case 3: return 24;
                    default: return 0;
                }
            }
        }

        public bool hasClut
        {
            get
            {
                return (((flags >> 3) & 1) == 1);
            }
        }

        public Tim()
        {
        }

        public static Tim FromFile(string fn)
        {
            using (BinaryReaderEx br = new BinaryReaderEx(File.OpenRead(fn)))
            {
                return new Tim(br);
            }
        }

        public Tim(BinaryReaderEx br)
        {
            Read(br);
        }


        public Tim(Rectangle rect)
        {
            magic = 0x10;
            region = rect;
            data = new ushort[rect.Width * rect.Height];
            datasize = (uint)(data.Length * 2 + 4 * 3);
            flags = 2; //(((uint)bpp / 8) << 3) | 8;
        }

        /// <summary>
        /// Reads TIM from file using BinaryReader.
        /// </summary>
        /// <param name="br">BinaryReader.</param>
        public void Read(BinaryReaderEx br)
        {
            magic = br.ReadUInt32();
            flags = br.ReadUInt32();

            if (hasClut)
            {
                clutsize = br.ReadUInt32();
                clutregion.X = br.ReadUInt16();
                clutregion.Y = br.ReadUInt16();
                clutregion.Width = br.ReadUInt16();
                clutregion.Height = br.ReadUInt16();
                clutdata = br.ReadArrayUInt16(clutregion.Width * clutregion.Height);
            }

            datasize = br.ReadUInt32();
            region.X = br.ReadUInt16();
            region.Y = br.ReadUInt16();
            region.Width = br.ReadUInt16();
            region.Height = br.ReadUInt16();
            data = br.ReadArrayUInt16(region.Width * region.Height);
        }

        /// <summary>
        /// Writes current TIM to file.
        /// </summary>
        /// <param name="s">Filename.</param>
        public void Write(string s)
        {
            using (BinaryWriterEx bw = new BinaryWriterEx(File.OpenWrite(s)))
            {
                bw.Write(magic);
                bw.Write(flags);

                if (hasClut)
                {
                    bw.Write(clutsize);
                    bw.Write((short)clutregion.X);
                    bw.Write((short)clutregion.Y);
                    bw.Write((short)clutregion.Width);
                    bw.Write((short)clutregion.Height);
                    foreach (ushort u in clutdata) bw.Write(u);
                }

                bw.Write(datasize);
                bw.Write((short)region.X);
                bw.Write((short)region.Y);
                bw.Write((short)region.Width);
                bw.Write((short)region.Height);
                foreach (ushort u in data)
                    bw.Write(u);
            }
        }

        public override string ToString()
        {
            return region.ToString();
        }

        /// <summary>
        /// Draws one TIM over another.
        /// Not a failproof implementation, ensure that target TIM is larger than original.
        /// In CTR context only used to draw 2 TIM regions in a single TIM.
        /// </summary>
        /// <param name="src">Source TIM to draw.</param>
        public void DrawTim(Tim src)
        {
            if (src.data == null)
            {
                Helpers.Panic(this, "missing tim data.");
                return;
            }

            //int dstptr = (this.region.Width * src.region.Y + src.region.X) * 2;


            int srcptr = 0;
            int dstptr = this.region.Width * src.region.Y * 2 + src.region.X * 2;

            for (int i = 0; i < src.region.Height; i++)

            {
                //Console.WriteLine(srcptr + "\t" + dstptr);
                //Console.ReadKey();

                Buffer.BlockCopy(
                    src.data, srcptr,
                    this.data, dstptr,
                    src.region.Width * 2);

                dstptr += this.region.Width * 2;
                srcptr += src.region.Width * 2;
            }

            if (src.clutdata == null)
            {
                Helpers.Panic(this, "clutdata is missing");
                return;
            }

            Buffer.BlockCopy(
                src.clutdata, 0,
                this.data, (this.region.Width * src.clutregion.Y + src.clutregion.X) * 2,
                src.clutdata.Length * 2); //keep in mind there will be leftover garbage if palette is less than 16 colors.
        }

        /// <summary>
        /// Saves current TIM as 4-bit BMP using BMPHeader.
        /// </summary>
        /// <param name="s">Filename.</param>
        /// <param name="pal">Palette.</param>
        public void SaveBMP(string s, byte[] pal)
        {
            s = Path.ChangeExtension(s, ".bmp");
            File.WriteAllBytes(s, SaveBMPToStream(pal));
        }

        public byte[] SaveBMPToStream(byte[] pal)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriterEx bw = new BinaryWriterEx(ms))
                {
                    BMPHeader bh = new BMPHeader();
                    bh.Update(region.Width * 4, region.Height, 16, 4);
                    bh.UpdateData(pal, FixBitmapData(FixPixelOrder(data), region.Width * 2, region.Height));
                    bh.Write(bw);

                    return ms.ToArray();
                }
            }
        }

        /// <summary>
        /// This function is necessary to fix pixel order in 4-bit BMPs.
        /// </summary>
        /// <param name="data">4-bit pixel array of bytes.</param>
        /// <returns></returns>
        private byte[] FixPixelOrder(byte[] data)
        {
            byte[] x = data;

            for (int i = 0; i < x.Length; i++)
                data[i] = (byte)(((data[i] & 0x0F) << 4) | ((data[i] & 0xF0) >> 4));

            return x;
        }

        /// <summary>
        /// This function is necessary to fix pixel order in 4-bit BMPs.
        /// </summary>
        /// <param name="data">4-bit pixel array of ushorts</param>
        /// <returns></returns>
        private byte[] FixPixelOrder(ushort[] data)
        {
            byte[] y = new byte[data.Length * 2];

            for (int i = 0; i < data.Length; i++)
            {
                y[i * 2] = (byte)(data[i] & 0xFF);
                y[i * 2 + 1] = (byte)(data[i] >> 8);
            }

            for (int i = 0; i < y.Length; i++)
                y[i] = (byte)(((y[i] & 0x0F) << 4) | ((y[i] & 0xF0) >> 4));

            return y;
        }

        /// <summary>
        /// Cuts a Tim subtexture from current Tim, based on TextureLayout data.
        /// </summary>
        /// <param name="tl">TextureLayout object.</param>
        /// <returns>Tim object.</returns>
        public Tim GetTimTexture(TextureLayout tl)
        {
            int bpp = 4;

            if (tl.f1 > 0 && tl.f2 > 0 && tl.f3 > 0)
                bpp = 8;

            //Directory.CreateDirectory(path);

            //int width = (tl.width / 4) * 2;
            int width = (int)(tl.width * (bpp / 8.0f));
            int height = tl.height;

            ushort[] buf = new ushort[(width / 2) * height];

            //Console.WriteLine(width + "x" + height);

            int ptr = tl.Position * 2; // tl.PageY * 1024 * (1024 * 2 / 16) + tl.frame.Y * 1024 + tl.PageX * (1024 * 2 / 16) + tl.frame.X;

            for (int i = 0; i < height; i++)
            {
                Buffer.BlockCopy(
                    this.data, ptr,
                    buf, i * width,
                    width);

                ptr += CtrVrm.Width * 2;
            }


            Tim x = new Tim(tl.frame);

            x.data = buf;

            x.region = new Rectangle(tl.RealX, tl.RealY, tl.width / 4, tl.height);

            x.clutregion = new Rectangle(tl.PalX * 16, tl.PalY, 16, 1);
            x.clutdata = GetCtrClut(tl);
            x.clutsize = (uint)(x.clutregion.Width * 2 + 12);
            x.flags = 8; //4 bit + pal = 8

            /*
            Rectangle r = x.clutregion;

            if (r.Width % 4 != 0)
                r.Width += 16;

            Tim x2 = new Tim(r);
            x2.DrawTim(x);
            */

            //Console.WriteLine(x.clutdata.Length);

            return x;
        }


        public Tim GetTrueColorTexture(Rectangle r)
        {
            return GetTrueColorTexture(r.X, r.Y, r.Width, r.Height);
        }

        public Tim GetTrueColorTexture(int x, int y, int w, int h)
        {
            ushort[] buf = new ushort[w * h];

            //Console.WriteLine(width + "x" + height);

            int ptr = 1024 * y + x; // tl.PageY * 1024 * (1024 * 2 / 16) + tl.frame.Y * 1024 + tl.PageX * (1024 * 2 / 16) + tl.frame.X;

            for (int i = 0; i < h; i++)
            {
                Buffer.BlockCopy(
                    this.data, ptr * 2,
                    buf, i * w * 2,
                    w * 2);

                ptr += 1024;
            }


            Tim tim = new Tim(new Rectangle(x, y, w, h));
            tim.data = buf;
            tim.flags = 2; //4 bit + pal = 8

            return tim;
        }


        /// <summary>
        /// Returns bitmap object.
        /// </summary>
        /// <param name="tl"></param>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Bitmap GetTexture(TextureLayout tl, string path = "", string name = "")
        {
            try
            {
                Tim x = GetTimTexture(tl);

                if (x.region.Width <= 0 || x.region.Height <= 0)
                {
                    Helpers.Panic(this, "negative or null size");
                    return new Bitmap(1, 1);
                }

                using (MemoryStream stream = new MemoryStream(x.SaveBMPToStream(CtrClutToBmpPalette(x.clutdata))))
                {
                    Bitmap oldBmp = (Bitmap)Bitmap.FromStream(stream);
                    Bitmap newBmp = new Bitmap(oldBmp);

                    if (!textures.ContainsKey(tl.Tag()))
                        textures.Add(tl.Tag(), newBmp);

                    return newBmp;
                }
            }
            catch (Exception ex)
            {
                Helpers.Panic(this, "GetTexture fails: " + " " + ex.Message + "\r\n" + ex.ToString() + "\r\n");
                return null;
            }
        }

        /// <summary>
        /// Loads texture data from bitmap file. Make sure you're quantizing your textures to 4 bits beforehand.
        /// </summary>
        /// <param name="f"></param>
        public void LoadDataFromBitmap(string f)
        {
            Bitmap bitmap = (Bitmap)Bitmap.FromFile(f);

            if (bitmap.Width / 4 != region.Width || bitmap.Height != region.Height)
            {
                Helpers.Panic(this, "Bitmap size mismatch.");
                return;
            }

            List<ushort> palette = new List<ushort>();

            byte[] newdata = new byte[bitmap.Width * bitmap.Height / 2];

            BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            //create data buffer 
            byte[] bytes = new byte[data.Height * data.Stride];

            // copy bitmap data into buffer
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            // unlock the bitmap data
            bitmap.UnlockBits(data);

            for (int i = 0; i < bytes.Length / 4; i++)
            {
                byte R = bytes[i * 4 + 0];
                byte G = bytes[i * 4 + 1];
                byte B = bytes[i * 4 + 2];
                byte A = bytes[i * 4 + 3];

                ushort col = ConvertTo16(R, G, B);

                if (!palette.Contains(col))
                {
                    palette.Add(col);
                    if (palette.Count > 16)
                    {
                        Helpers.Panic(this, "Too many colors.");
                        return;
                    }
                }

                if (i % 2 == 0)
                {
                    newdata[i / 2] |= (byte)(palette.IndexOf(col));
                }
                else
                {
                    newdata[i / 2] |= (byte)(palette.IndexOf(col) << 4);
                }

                for (int j = 0; j < i / 4; j++)
                    this.data[j] = (ushort)(newdata[j * 2] | newdata[j * 2 + 1] << 8);

                this.clutdata = palette.ToArray();
            }
        }
        



        public byte[] FixBitmapData(byte[] b, int width, int height)
        {
            byte[] data;

            if (width % 4 != 0)
            {
                int newWidth = width + width % 4;
                data = new byte[newWidth * height];
                for (int i = 0; i < height; i++)
                {
                    Buffer.BlockCopy(
                    b, i * width,
                    data, i * newWidth,
                    width);
                }
                return data;
            }

            return b;
        }


        /// <summary>
        /// Returns PS1 palette (CLUT) for corresponding texture layout.
        /// </summary>
        /// <param name="tl">Texture layout data.</param>
        public ushort[] GetCtrClut(TextureLayout tl)
        {
            ushort[] buf = new ushort[16];

            int ptr = tl.PalPosition * 2;

            Buffer.BlockCopy(
                this.data, ptr,
                buf, 0,
                16 * 2);

            return buf;
        }

        /// <summary>
        /// Converts PS1 palette (CLUT) to 32 bit BMP palette.
        /// </summary>
        /// <param name="clut">Array of 32 bytes.</param>
        public byte[] CtrClutToBmpPalette(ushort[] clut)
        {
            byte[] pal = new byte[16 * 4];

            // pals++;


            for (int i = 0; i < 16; i++)
            {
                Color c = Convert16(clut[i], true);

                // palbmp.SetPixel(i, pals, c);

                pal[i * 4] = c.B;
                pal[i * 4 + 1] = c.G;
                pal[i * 4 + 2] = c.R;
                pal[i * 4 + 3] = c.A;
            }


            return pal;
        }

        /// <summary>
        /// Converts 5-5-5-1 16 bit color to 8-8-8-8 32 bit color.
        /// </summary>
        /// <param name="col">16 bit ushort color value.</param>
        /// <param name="useAlpha">Defines whether alpha value should be preserved.</param>
        /// <returns></returns>
        public static Color Convert16(ushort col, bool useAlpha)
        {
            byte r = (byte)(((col >> 0) & 0x1F) << 3);
            byte g = (byte)(((col >> 5) & 0x1F) << 3);
            byte b = (byte)(((col >> 10) & 0x1F) << 3);
            byte a = (byte)((col >> 15) * 255);

            //um...
            if (a != 255 && r == 0 && g == 0 & b == 0)
            {
                r = 255;
                g = 0;
                b = 255;
            }

            return Color.FromArgb((useAlpha ? a : 255), r, g, b);
        }

        public static ushort ConvertTo16(Color c)
        {
            return ConvertTo16(c.R, c.G, c.B);
        }

        public static ushort ConvertTo16(byte r, byte g, byte b)
        {
            return (ushort)((r >> 3 << 10) | (g >> 3 << 5) | (b >> 3 << 0));
        }

        public static Color Convert16(byte[] b, bool useAlpha)
        {
            ushort val = BitConverter.ToUInt16(b, 0);
            return Convert16(val, useAlpha);
        }
    }

}
