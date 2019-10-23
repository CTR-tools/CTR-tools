using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace CTRFramework
{
    public class TextureLayout : IRead
    {
        public List<Vector2b> uv = new List<Vector2b>();

        public ushort PalX;
        public ushort PalY;

        public ushort PageX;
        public ushort PageY;

        public byte check;


        public TextureLayout(BinaryReaderEx br)
        {
            Read(br);
        }

        public void Read(BinaryReaderEx br)
        {
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
                Console.WriteLine("---WTF---page 2nd byte != 0");
                //Console.ReadKey();
            }

            uv.Add(new Vector2b(br));
            uv.Add(new Vector2b(br));

            //Console.WriteLine(Tag());
        }

        //meant to be unique
        public string Tag()
        {
            return PageX.ToString("X2") + PageY.ToString("X2") + "_" +
                PalX.ToString("X4") + PalY.ToString("X4") + "_" + uv[0].X.ToString("X2") + uv[0].Y.ToString("X2");
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

            foreach (Vector2b v in uv)
                sb.AppendFormat(
                    "vt {0} {1}\r\n",
                    Math.Round(v.X / 255f, 3).ToString(),
                    Math.Round((255 - v.Y) / 255f, 3).ToString()
                );

            sb.AppendFormat("\r\nusemtl {0}\r\n", Tag());

            return sb.ToString();
        }
    }
}
