using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace CTRFramework.Vram
{
    /// <summary>
    /// Please note this is super hacky implementation of PSX Tim format hardcoded for 4 bits used in Crash Team Racing.
    /// You can load any tim files, but overall code will not work properly with anything other than 4 bits.
    /// </summary>
    public class Tim : IRead
    {
        public Dictionary<string, Bitmap> textures = new Dictionary<string, Bitmap>();

        const uint magic = 0x10;

        public uint clutsize => (uint)clutdata.Length * 2 + 12;
        public Rectangle clutregion;
        public ushort[] clutdata;

        public uint datasize => (uint)data.Length * 2 + 12;

        public Rectangle region;

        public ushort[] data;

        public BitDepth bpp = BitDepth.Bit16;

        public bool hasClut {
            get
            {
                switch (bpp)
                {
                    case BitDepth.Bit4:
                    case BitDepth.Bit8: return true;
                    case BitDepth.Bit16:
                    case BitDepth.Bit24:
                    default: return false;
                }
            }
        }

        public Tim()
        {
        }

        public static Tim FromFile(string fn)
        {
            using (BinaryReaderEx br = new BinaryReaderEx(File.OpenRead(fn)))
            {
                return FromReader(br);
            }
        }
        public static Tim FromReader(BinaryReaderEx br)
        {
            return new Tim(br);
        }

        public Tim(BinaryReaderEx br)
        {
            Read(br);
        }


        public Tim(Rectangle rect, BitDepth bitDepth)
        {
            region = rect;
            bpp = bitDepth;
            data = new ushort[rect.Width * rect.Height];

            Helpers.Panic(this, PanicType.Debug, "lemao what? " + rect.Width * rect.Height +  data.Length);
        }

        /// <summary>
        /// Reads TIM from file using BinaryReader.
        /// </summary>
        /// <param name="br">BinaryReader.</param>
        public void Read(BinaryReaderEx br)
        {
            if (br.ReadUInt32() != magic)
                Console.WriteLine("Houston! magic mismatch");

            uint flags = br.ReadUInt32();

            switch (flags & 3)
            {
                case 0: bpp = BitDepth.Bit4; break;
                case 1: bpp = BitDepth.Bit8; break;
                case 2: bpp = BitDepth.Bit16; break;
                case 3: bpp = BitDepth.Bit24; break;
            }

            Helpers.Panic(this, PanicType.Info, bpp + " " + hasClut);

            if ((((flags >> 3) & 1) == 1) != hasClut)
                Console.WriteLine("Houston! bpp and clut mismatch.");

            if (hasClut)
            {
                uint _clutsize = br.ReadUInt32();
                clutregion.X = br.ReadUInt16();
                clutregion.Y = br.ReadUInt16();
                clutregion.Width = br.ReadUInt16();
                clutregion.Height = br.ReadUInt16();
                clutdata = br.ReadArrayUInt16(clutregion.Width * clutregion.Height);

                if (_clutsize != clutsize)
                    Console.WriteLine("Houston! clutsize mismatch.");
            }

            uint _datasize = br.ReadUInt32();
            region.X = br.ReadUInt16();
            region.Y = br.ReadUInt16();
            region.Width = br.ReadUInt16();
            region.Height = br.ReadUInt16();
            data = br.ReadArrayUInt16(region.Width * region.Height);
            if (_datasize != datasize)
                Console.WriteLine($"Houston! datasize mismatch. {_datasize} {datasize}");
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

                bw.Write((int)bpp & ((hasClut ? 1 : 0) << 3));

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
            if (src == null)
            {
                Helpers.Panic(this, PanicType.Warning, "Passed TIM is null.");
                return;
            }

            if (src.data == null)
            {
                Helpers.Panic(this, PanicType.Warning, "Nothing to draw.");
                return;
            }

            int srcptr = 0;
            int dstptr = this.region.Width * src.region.Y * 2 + src.region.X * 2;

            for (int i = 0; i < src.region.Height; i++)
            {
                Helpers.Panic(this, PanicType.Debug, "writing tim line " + i);
                Helpers.Panic(this, PanicType.Debug, src.data.Length + " " + this.data.Length);

                Buffer.BlockCopy(
                    src.data, srcptr,
                    this.data, dstptr,
                    src.region.Width * 2);

                dstptr += this.region.Width * 2;
                srcptr += src.region.Width * 2;
            }

            if (src.hasClut)
            {
                if (src.clutdata == null)
                {
                    Helpers.Panic(this, PanicType.Warning, "clutdata is missing");
                    return;
                }

                Buffer.BlockCopy(
                    src.clutdata, 0,
                    this.data, (this.region.Width * src.clutregion.Y + src.clutregion.X) * 2,
                    src.clutdata.Length * 2); //keep in mind there will be leftover garbage if palette is less than 16 colors.
            }
        }

        /// <summary>
        /// Saves current TIM as 4-bit BMP using BMPHeader.
        /// </summary>
        /// <param name="filename">Filename.</param>
        /// <param name="pal">Palette.</param>
        public void SaveBMP(string filename, byte[] pal)
        {
            filename = Path.ChangeExtension(filename, ".bmp");
            Helpers.WriteToFile(filename, SaveBMPToStream(pal));
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
        public static byte[] FixPixelOrder(byte[] data)
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

            switch (tl.bpp)
            {
                case BitDepth.Bit4: bpp = 4; break;
                case BitDepth.Bit8: bpp = 8; break;
                case BitDepth.Bit16: bpp = 16; break;
                case BitDepth.Bit24: bpp = 24; break;
            }

            int palsize = bpp == 8 ? 256 : (bpp == 4 ? 16 : 0);

            //if (tl.f1 > 0 && tl.f2 > 0 && tl.f3 > 0)
            //    bpp = 8;

            //Directory.CreateDirectory(path);

            //int width = (tl.width / 4) * 2;
            int width = (int)(tl.width * (bpp / 8.0f));
            int height = tl.height;

            // Helpers.Panic(this, PanicType.Assume, tl.width + " vs " + width + "|" + bpp + " " + (bpp / 8.0f) + " " + (tl.width * (bpp / 8.0f)));

            //ahem, so this happens because 1 byte = 2 pixel and if it's uneven width, we have to move half-byte.
            bool dirtyhack = false;

            if (width % 2 == 1)
            {
                width++;
                dirtyhack = true;
            }


            Console.WriteLine(width + "x" + height);

            ushort[] buf = new ushort[(width / 2) * height];

            //Console.WriteLine(width + "x" + height);

            int ptr = tl.Position * 2; // tl.PageY * 1024 * (1024 * 2 / 16) + tl.frame.Y * 1024 + tl.PageX * (1024 * 2 / 16) + tl.frame.X;

            for (int i = 0; i < height; i++)
            {
                //Helpers.Panic(this, PanicType.Assume, $"ptr: {ptr}, i: {i}, i * width: {i * width}, width: {width}");

                Buffer.BlockCopy(
                    this.data, ptr,
                    buf, i * width,
                    width);

                ptr += CtrVrm.region.Width * 2;
            }


            Tim x = new Tim(tl.frame, tl.bpp);

            x.data = buf;

            x.region = new Rectangle(tl.RealX, tl.RealY, tl.width / 4, tl.height);

            if (dirtyhack)
                x.region.Width++;

            x.clutregion = new Rectangle(tl.PalX * palsize, tl.PalY, palsize, 1);
            x.clutdata = GetCtrClut(tl);
            //x.clutsize = (uint)(x.clutregion.Width * 2 + 12);
            //x.flags = 8; //4 bit + pal = 8

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


            Tim tim = new Tim(new Rectangle(x, y, w, h), BitDepth.Bit16);
            tim.data = buf;
            //tim.flags = 2; //4 bit + pal = 8

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
            //Helpers.Panic(this, PanicType.Assume, tl.ToString() + "" + "\r\n" + tl.frame.ToString());

            try
            {
                Tim x = GetTimTexture(tl);

                if (x.region.Width <= 0 || x.region.Height <= 0)
                {
                    Helpers.Panic(this, PanicType.Error, "negative or null size");
                    return new Bitmap(1, 1);
                }

                using (MemoryStream stream = new MemoryStream(x.SaveBMPToStream(CtrClutToBmpPalette(x.clutdata))))
                {
                    Bitmap oldBmp = (Bitmap)Bitmap.FromStream(stream);
                    Bitmap newBmp = new Bitmap(oldBmp);

                    if (!textures.ContainsKey(tl.Tag))
                        textures.Add(tl.Tag, newBmp);

                    return newBmp;
                }
            }
            catch (Exception ex)
            {
                Helpers.Panic(this, PanicType.Error, tl.frame + "\r\n" + "GetTexture fails: " + " " + ex.Message + "\r\n" + ex.ToString() + "\r\n");
                return null;
            }
        }

        /// <summary>
        /// Loads texture data from bitmap file. Make sure you're quantizing your textures to 4 bits beforehand.
        /// </summary>
        /// <param name="filename"></param>
        public void LoadDataFromBitmap(string filename)
        {
            Bitmap bitmap = (Bitmap)Bitmap.FromFile(filename);

            if (bitmap.Width / 4 != region.Width || bitmap.Height != region.Height)
            {
                Helpers.Panic(this, PanicType.Error, $"Bitmap size mismatch {filename}.");
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
                        Helpers.Panic(this, PanicType.Error, $"Too many colors. Halt loading texture {filename}.");
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

            }

            if (palette.Count < 16)
            {
                int x = palette.Count;

                for (int g = 0; g < 16 - x; g++)
                    palette.Add((ushort)0);
            }

            this.clutdata = palette.ToArray();
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
            byte a = 0;

            byte stp = (byte)((col >> 15));

            /*
            //https://www.psxdev.net/forum/viewtopic.php?t=953
                Full-black without STP bit = Transparent (alpha = 0)
                Non full-black without STP bit = Solid color (alpha = 255)

                Full-black with STP bit = Semi-transparent black (alpha = 127)
                Non full-black with STP bit = Semi-transparent color (alpha = 127)
            */


            if (stp == 0)
            {
                if (r == 0 && g == 0 && b == 0)
                {
                    a = 0;
                    r = 255;
                    g = 0;
                    b = 255;
                }
                else
                {
                    a = 255;
                }
            }
            else
            {
                a = 127;
                /*
                r = 0;
                g = 255;
                b = 255;
                */
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