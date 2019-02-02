using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace WindowsFormsApplication1
{
    class Tim
    {
        public int x1;
        public int x2;
        public int datasize;
        public Rectangle region;
        public ushort[] data;


        public Tim()
        {
        }

        public Tim(Rectangle rect)
        {
            region = rect;
            data = new ushort[rect.Width * rect.Height];
        }

        public void Read(BinaryReader br)
        {
            x1 = br.ReadInt32();
            x2 = br.ReadInt32();
            datasize = br.ReadInt32();

            if (x1 != 16 || x2 != 2)
            {
                throw new Exception("invalid TIM header");
            }

            region.X = br.ReadUInt16();
            region.Y = br.ReadUInt16();
            region.Width = br.ReadUInt16();
            region.Height = br.ReadUInt16();

            byte[] buf = br.ReadBytes(datasize - 4 * 3);
            data = new ushort[buf.Length / 2];
            Buffer.BlockCopy(buf, 0, data, 0, buf.Length);
        }


        public Bitmap ToBitmap()
        {
            ushort[,] bidata = new ushort[region.Height, region.Width];

            int pX = 0;
            int pY = 0;

            foreach (ushort u in data)
            {
                bidata[pY, pX] = data[pY * region.Width + pX];

                pX++;

                if (pX >= region.Width)
                {
                    pY++;
                    pX = 0;
                }
            }


            List<byte> bytes = new List<byte>();

            pX = 0;
            pY = 0;

            foreach (ushort u in bidata)
            {
                Color c = PsxVram.Convert16(u, false);

                bytes.Add(c.B);
                bytes.Add(c.G);
                bytes.Add(c.R);
                bytes.Add(c.A);

                pX++;

                if (pX >= region.Width)
                {
                    pY++;
                    pX = 0;
                }
            }

            Bitmap bmp = new Bitmap(region.Width, region.Height);
            FastBitmap.LockBits(bmp, bytes.ToArray());

            return bmp;
        }


    }

}