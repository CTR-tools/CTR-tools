using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace model_reader
{
    class CTRNgon
    {
        public short[] ind = new short[9];
        public byte[] unk1 = new byte[10];
        public uint[] tex = new uint[4];
        public uint[] unk2 = new uint[4];
        public uint midflags;
        public uint offset1;
        public uint offset2;
        public ushort[] unk3 = new ushort[10];

        //List<CTRTex> ctrtex = new List<CTRTex>();

        public CTRNgon(BinaryReader br)
        {
            for (int i = 0; i < 9; i++)
                ind[i] = br.ReadInt16();

            unk1 = br.ReadBytes(10);

            for (int i = 0; i < 4; i++)
                tex[i] = br.ReadUInt32();

            for (int i = 0; i < 4; i++)
                unk2[i] = br.ReadUInt32();

            midflags = br.ReadUInt32();
            offset1 = br.ReadUInt32();
            offset2 = br.ReadUInt32();

            for (int i = 0; i < 10; i++)
                unk3[i] = br.ReadUInt16();
            /*

            int pos = (int)br.BaseStream.Position;

            for (int i = 0; i < 4; i++)
            {
                // System.Windows.Forms.MessageBox.Show(""+tex[i]);
                br.BaseStream.Position = tex[i];
                ctrtex.Add(new CTRTex(br));
            }

            br.BaseStream.Position = pos;
            */
        }
        /*
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (uint t in tex)
            {
                sb.Append(t.ToString() + "\r\n");
            }

            foreach (CTRTex c in ctrtex)
            {
                sb.Append(c.ToString() + "\r\n");
            }


            return sb.ToString();
        }
         * */

        public string ToObj(List<CTRVertex> v, int objnum)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("o piece_" + objnum.ToString("0000") + "\r\n");
            sb.Append("g piece_" + objnum.ToString("0000") + "\r\n");

            bool x = (unk1[5] & 2) > 0;
            x = unk1[2] > 0;

            //if (x) Console.WriteLine("!!!");

            for (int i = 0; i < ind.Length; i++)
            {
                sb.Append("v " + v[ind[i]].ToString(x) + "\r\n");
            }

            sb.Append(ASCIIFace("f", -9 + 5, -9 + 4, -9 + 0));
            sb.Append(ASCIIFace("f", -9 + 4, -9 + 5, -9 + 6));
            sb.Append(ASCIIFace("f", -9 + 6, -9 + 1, -9 + 4));
            sb.Append(ASCIIFace("f", -9 + 1, -9 + 6, -9 + 7));
            sb.Append(ASCIIFace("f", -9 + 2, -9 + 6, -9 + 5));
            sb.Append(ASCIIFace("f", -9 + 6, -9 + 2, -9 + 8));
            sb.Append(ASCIIFace("f", -9 + 8, -9 + 7, -9 + 6));
            sb.Append(ASCIIFace("f", -9 + 7, -9 + 8, -9 + 3));

            //Console.WriteLine(sb.ToString());
            //Console.ReadKey();

            return sb.ToString();
        }


        public string ASCIIFace(string label, int x, int y, int z)
        {
            return label + " " + x + " " + y + " " + z + "\r\n";
        }


    }
}
