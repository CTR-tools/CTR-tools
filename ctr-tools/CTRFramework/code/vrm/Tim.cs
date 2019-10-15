using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic;

namespace CTRFramework
{
    public class Tim
    {
        public uint magic;
        public uint flags;
        public uint datasize;
        public Rectangle region;
        public byte[] data;
        public byte[] realdata;

        public int dataWidth
        {
            get {
                return region.Width * bpp / 8;
            }
        }


        public byte bpp
        {
            get {
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

        public Tim(Rectangle rect)
        {
            region = rect;
            //data = new ushort[rect.Width * rect.Height];
            data = new byte[rect.Width * rect.Height * 2];
            realdata = new byte[rect.Width * rect.Height * 2];
            magic = 16;
            flags = 2;
            datasize = (uint)(data.Length + 4 * 3);
        }


        public void Read(BinaryReader br)
        {
            magic = br.ReadUInt32();
            flags = br.ReadUInt32();
            datasize = br.ReadUInt32();

            if (magic != 16 || flags != 2)
            {
                throw new Exception("invalid TIM header");
            }

            region.X = br.ReadUInt16();
            region.Y = br.ReadUInt16();
            region.Width = br.ReadUInt16();
            region.Height = br.ReadUInt16();

            long pos = br.BaseStream.Position;

            data = br.ReadBytes((int)datasize - 4 * 3);

            br.BaseStream.Position = pos;

            realdata = br.ReadBytes((int)datasize - 4 * 3);
            //Array.Copy(data, 0, realdata, 0, data.Length);

            for (int i = 0; i < data.Length; i++)
                data[i] = (byte)(((data[i] & 0x0F) << 4) | ((data[i] & 0xF0) >> 4));


            //byte[] buf = br.ReadBytes((int)datasize - 4 * 3);
            //data = new ushort[buf.Length / 2];
            //Buffer.BlockCopy(buf, 0, data, 0, buf.Length);
        }

        public void Write(string s)
        {
            using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(s)))
            {
                bw.Write(magic);
                bw.Write(flags);
                bw.Write(datasize);
                bw.Write((short)region.X);
                bw.Write((short)region.Y);
                bw.Write((short)region.Width);
                bw.Write((short)region.Height);
                //pal here
                bw.Write(data);
            }
        }

        public override string ToString()
        {
            return region.ToString();
        }

        public void DrawTim(Tim src)
        {
           // byte[] buf = new byte[] { };
           // Array.Resize(ref buf, src.dataWidth);

            for (int i = 0; i < src.region.Height; i++)
            {
                Buffer.BlockCopy(
                    src.data, i * src.dataWidth,
                    this.data, this.dataWidth * (src.region.Y + i) + src.region.X * 2,
                    src.dataWidth);

                Buffer.BlockCopy(
src.realdata, i * src.dataWidth,
this.realdata, this.dataWidth * (src.region.Y + i) + src.region.X * 2,
src.dataWidth);
            }
        }

        public void SaveBMP(string s, byte[] pal)
        {
            BMPHeader bh = new BMPHeader();
            bh.Update(region.Width * 4, region.Height, 16, 4);

            bh.UpdateData(pal, data);

            using (BinaryWriter bw = new BinaryWriter(File.Create(s)))
            {
                bh.Write(bw);
            }
        }



        public void GetTexturePage(TextureLayout tl)
        {

            byte[] buf = new byte[128 * 256];

            for (int i = 0; i < 256; i++)
            {
                Buffer.BlockCopy(
                    this.data, 2048 * i + 128 * tl.PageX + tl.PageY * 2048 * 256,//this.dataWidth * (tl.PageY + i) + tl.PageX * 256 * 2,
                    buf, i * 128, 
                    128);
            }

            Tim x = new Tim(new Rectangle(0, 0, 256 / 4, 256));
            x.data = buf;

            x.SaveBMP("tex\\" + tl.Tag() + ".bmp", CtrClutToBmpPalette(GetCtrClut(tl)));


            using (Bitmap oldBmp = new Bitmap("tex\\" + tl.Tag() + ".bmp"))
            using (Bitmap newBmp = new Bitmap(oldBmp))
            using (Bitmap targetBmp = newBmp.Clone(new Rectangle(0, 0, newBmp.Width, newBmp.Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                Graphics g = Graphics.FromImage(targetBmp);

                Point[] poly = new Point[]
                {
                    new Point(tl.uv[0].X, tl.uv[0].Y),
                    new Point(tl.uv[1].X, tl.uv[1].Y),
                    new Point(tl.uv[3].X, tl.uv[3].Y),
                    new Point(tl.uv[2].X, tl.uv[2].Y)
                };


                g.DrawImage(targetBmp, new Point(0, 0));

                /*
                g.DrawPolygon(Pens.White, poly);

                g.DrawEllipse(Pens.Red, new Rectangle(poly[0].X, poly[0].Y, 3, 3));
                g.DrawEllipse(Pens.Green, new Rectangle(poly[2].X, poly[2].Y, 3, 3));
                g.DrawEllipse(Pens.Blue, new Rectangle(poly[1].X, poly[1].Y, 3, 3));
                g.DrawEllipse(Pens.Purple, new Rectangle(poly[3].X, poly[3].Y, 3, 3));
                */
                targetBmp.Save("tex\\" + tl.Tag() + ".png", System.Drawing.Imaging.ImageFormat.Png);

            }


        }




        public byte[] GetCtrClut(TextureLayout tl)
        {
            byte[] buf = new byte[32];

           // Console.WriteLine(tl.PalX + " " + tl.PalY);

            Buffer.BlockCopy(
                this.realdata, 2048 * tl.PalY + tl.PalX * 16 * 2,
                buf, 0,
                32);

            /* now im sure the palette is correct
            for (int i = 0; i < 32; i++)
            {
                this.data[2048 * tl.PalY + tl.PalX * 16 * 2 + i] = 255;
            }*/

            /*
            Console.Write(x + " " + y);

            foreach (byte b in buf)
                Console.Write(b.ToString("X2"));

            Console.WriteLine();
            */

            return buf;
        }

       // int pals = 0;

       // public Bitmap palbmp = new Bitmap(16, 1024);

        public byte[] CtrClutToBmpPalette(byte[] clut)
        {
            byte[] pal = new byte[16 * 4];

           // pals++;

            using (BinaryReader br = new BinaryReader(new MemoryStream(clut)))
            {
                for (int i = 0; i < 16; i++)
                {
                    Color c = Convert16(br.ReadUInt16(), false);

                   // palbmp.SetPixel(i, pals, c);

                    pal[i * 4] = c.B;
                    pal[i * 4 + 1] = c.G;
                    pal[i * 4 + 2] = c.R;
                    pal[i * 4 + 3] = c.A;
                }
            }
            

            return pal;
        }


        /*
        public Bitmap ToTexturePages()
        {
            Directory.CreateDirectory(@".\tex\");
            SaveBMP("test.bmp", BMPHeader.GrayScalePalette(256));
            Bitmap bmp = (Bitmap)Bitmap.FromFile("test.bmp");

            int tpw = 256;
            int tph = 256;

            for (int i = 0; i < 16; i++)
                for (int j = 0; j < 2; j++)
                {
                    Bitmap x = bmp.Clone(new Rectangle(new Point(i * tpw, j * tph), new Size(tpw, tph)), System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                    x = new Bitmap(x, new Size(256, 256));

                    if (i >= 8)
                        x.Save(String.Format(@".\tex\page_{0}_{1}.bmp", i, j), System.Drawing.Imaging.ImageFormat.Bmp);
                }

            return bmp;
        }

    */

        public static Color Convert16(byte[] b, bool useAlpha)
        {
            ushort val = BitConverter.ToUInt16(b, 0);
            return Convert16(val, useAlpha);
        }

        public static Color Convert16(ushort col, bool useAlpha)
        {
            byte r = (byte)(((col >> 0) & 0x1F) << 3);
            byte g = (byte)(((col >> 5) & 0x1F) << 3);
            byte b = (byte)(((col >> 10) & 0x1F) << 3);
            byte a = (byte)((col >> 15) * 255);

            return Color.FromArgb((useAlpha ? a : 255), r, g, b);
        }


    }

}