using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace CTRFramework
{
    public class TextureLayout : IRead
    {
        public uint offset;

        public List<Vector2b> uv = new List<Vector2b>();
        public List<Vector2b> normuv = new List<Vector2b>();

        public ushort PalX;
        public ushort PalY;

        public ushort PageX;
        public ushort PageY;

        public byte check;


        public Point min
        {
            get
            {
                return new Point(
                    Math.Min(Math.Min(uv[0].X, uv[1].X), Math.Min(uv[2].X, uv[3].X)),
                    Math.Min(Math.Min(uv[0].Y, uv[1].Y), Math.Min(uv[2].Y, uv[3].Y))
                    );
            }
        }
        public Point max
        {
            get
            {
                return new Point(
                    Math.Max(Math.Max(uv[0].X, uv[1].X), Math.Max(uv[2].X, uv[3].X)),
                    Math.Max(Math.Max(uv[0].Y, uv[1].Y), Math.Max(uv[2].Y, uv[3].Y))
                    );
            }
        }


        public int width
        {
            get
            {
                return max.X - min.X + 1; //uv[1].X - uv[0].X + 1; }
            }
        }

        public int height
        {
            get
            {
                return max.Y - min.Y + 1;// return uv[3].Y - uv[0].Y + 1; }
            }
        }

        public int Position
        {
            get
            {
                return PageY * (CtrVrm.Height * CtrVrm.Width / 2) + uv[0].Y * CtrVrm.Width + PageX * 64 + uv[0].X / 4;
            }
        }

        public int PalPosition
        {
            get
            {
                return CtrVrm.Width * PalY + PalX * 16;
            }
        }
        

        public int RealX
        {
            get
            {
                return PageX * 64 + min.X;
            }
        }
        public int RealY
        {
            get
            {
                return PageY * 256 + min.Y;
            }
        }


        public Rectangle frame
        {
            get
            {
                return new Rectangle(RealX, RealY, width / 4, height);
            }
        }


        public TextureLayout(BinaryReaderEx br)
        {
            Read(br);
        }

        
        public int Normalize(int min, int max, int val)
        {
            return (val - min) / (max - min);
        }

        public void NormalizeUV()
        {
            foreach(Vector2b v in uv)
            {
                Vector2b n = new Vector2b(0,0);
                n.X = (byte)Normalize(min.X, max.X, v.X);
                n.Y = (byte)Normalize(min.Y, max.Y, v.Y);
                normuv.Add(n);
            }
        }
        

        public void Read(BinaryReaderEx br)
        {
            offset = (uint)br.BaseStream.Position;

            uv.Add(new Vector2b(br));

            ushort buf = br.ReadUInt16();

            PalX = (ushort)(buf & 0x3F);
            PalY = (ushort)(buf >> 6);

            uv.Add(new Vector2b(br));

            buf = br.ReadByte();

            PageX = (ushort)(buf & 0xF);
            PageY = (ushort)((buf >> 4) & 1);

            check = br.ReadByte();
            //checking page byte 2 if it's ever not 0
            if (check != 0)
            {
                Console.WriteLine("TextureLayout says ---WTF--- page 2nd byte != 0");
                //Console.ReadKey();
            }

            uv.Add(new Vector2b(br));
            uv.Add(new Vector2b(br));

            NormalizeUV();

            //Console.WriteLine(Tag());
        }

        //meant to be unique
        public string Tag()
        {
            return offset.ToString("X8");

            /*
            return PageX.ToString("X2") + PageY.ToString("X2") + "_" +
                PalX.ToString("X4") + PalY.ToString("X4") + "_" + uv[0].X.ToString("X2") + uv[0].Y.ToString("X2");
                */
        }

        public override string ToString()
        {
            return
                uv[0].ToString() + "|\t" +
                uv[1].ToString() + "|\t" +
                uv[2].ToString() + "|\t" +
                uv[3].ToString() + "|\t" +
                PalX + ", " + PalY + "|\t" +
                PageX + ", " + PageY;
        }

        public string ToObj()
        {
            StringBuilder sb = new StringBuilder();

            foreach (Vector2b v in normuv)
                sb.AppendFormat(
                    "vt {0} {1}\r\n",

                    Math.Round(v.X * 1.0, 3).ToString(),
                    Math.Round(-v.Y * 1.0, 3).ToString()
                );

            sb.AppendFormat("\r\nusemtl {0}\r\n", Tag());

            return sb.ToString();
        }
    }
}
