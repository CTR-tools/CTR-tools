using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CTRtools
{
    class CTRNgon
    {
        public  string ToString(List<CTRVertex> v, int objnum)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("o piece_" + objnum.ToString("0000") + "\r\n");
            sb.Append("g piece_" + objnum.ToString("0000") + "\r\n");

            for (int i = 0; i < 9; i++)
            {
                v[ind[i]].ToString();
            }

            sb.Append(ASCIIFace("f", 5, 4, 0));
            sb.Append(ASCIIFace("f", 4, 5, 6));
            sb.Append(ASCIIFace("f", 6, 1, 4));
            sb.Append(ASCIIFace("f", 1, 6, 7));
            sb.Append(ASCIIFace("f", 2, 6, 5));
            sb.Append(ASCIIFace("f", 6, 2, 8));
            sb.Append(ASCIIFace("f", 8, 7, 6));
            sb.Append(ASCIIFace("f", 7, 8, 3));

            Console.WriteLine(sb.ToString());
            Console.ReadKey();

            return sb.ToString();
        }


        public string ASCIIFace(string label, int x, int y, int z)
        {
            return label + " " + ind[x] + " " + ind[y] + " " + ind[z] + "\r\n";
        }


        public short[] ind = new short[9];
        public byte[] unk1 = new byte[10];
        public uint[] tex = new uint[4];
        public uint[] unk2 = new uint[7];
        public ushort[] small = new ushort[10];

        List<CTRTex> ctrtex = new List<CTRTex>();

        public CTRNgon(BinaryReader br)
        {
            for (int i = 0; i < 9; i++)
                ind[i] = br.ReadInt16();

            unk1 = br.ReadBytes(10);

            for (int i = 0; i < 4; i++)
                tex[i] = br.ReadUInt32();

            for (int i = 0; i < 7; i++)
                unk2[i] = br.ReadUInt32();

            for (int i = 0; i < 10; i++)
                small[i] = br.ReadUInt16();

            
            int pos = (int)br.BaseStream.Position;

            for (int i = 0; i < 4; i++)
            {
               // System.Windows.Forms.MessageBox.Show(""+tex[i]);
                br.BaseStream.Position = tex[i];
                ctrtex.Add(new CTRTex(br));
            }

            br.BaseStream.Position = pos;
             
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
    }
}
