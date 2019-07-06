using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace model_reader
{
    class CTRNgon
    {
        public short[] ind = new short[9]; //9 indices in vertex array, that form 4 quads.
        public byte[] unk1 = new byte[10];
        public uint[] tex = new uint[4];

        public BoundingBox bb;
        public byte[] unk2 = new byte[4];

        public short id;
        public byte[] midflags = new byte[2];

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

            bb = new BoundingBox(br);

            unk2 = br.ReadBytes(4);

            id = br.ReadInt16();

            midflags = br.ReadBytes(2);


            //Console.WriteLine(midflags[0].ToString("X2") + " " + midflags[1].ToString("X2"));
           // Console.ReadKey();

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


        public enum Detail
        {
            High, Low
        }

        public string ToObj(List<CTRVertex> v, Detail detail)
        {
            StringBuilder sb = new StringBuilder();

            bool x = (unk1[3] & 2) > 0;
            x = false;

            //if (x) Console.WriteLine("!!!");

            for (int i = 0; i < ind.Length; i++)
            {
                sb.Append(String.Format("v {0}\r\n", v[ind[i]].ToString(x)));
            }

            sb.Append("g piece_" + id.ToString("0000") + "\r\n");
            sb.Append("o piece_" + id.ToString("0000") + "\r\n");

            switch (detail)
            {
                case Detail.Low:
                    sb.Append(ASCIIFace("f", -9 + 2, -9 + 1, -9 + 0));
                    sb.Append(ASCIIFace("f", -9 + 1, -9 + 2, -9 + 3));
                    break;

                case Detail.High:
                    sb.Append(ASCIIFace("f", -9 + 5, -9 + 4, -9 + 0));
                    sb.Append(ASCIIFace("f", -9 + 4, -9 + 5, -9 + 6));
                    sb.Append(ASCIIFace("f", -9 + 6, -9 + 1, -9 + 4));
                    sb.Append(ASCIIFace("f", -9 + 1, -9 + 6, -9 + 7));
                    sb.Append(ASCIIFace("f", -9 + 2, -9 + 6, -9 + 5));
                    sb.Append(ASCIIFace("f", -9 + 6, -9 + 2, -9 + 8));
                    sb.Append(ASCIIFace("f", -9 + 8, -9 + 7, -9 + 6));
                    sb.Append(ASCIIFace("f", -9 + 7, -9 + 8, -9 + 3));
                    break;
            }
            //Console.WriteLine(sb.ToString());
            //Console.ReadKey();


            /*
            if (fmt == "ply")
             * 
            {
                sb.Append(ASCIIFace("3", ind, 5, 4, 0));
                sb.Append(ASCIIFace("3", ind, 4, 5, 6));
                sb.Append(ASCIIFace("3", ind, 6, 1, 4));
                sb.Append(ASCIIFace("3", ind, 1, 6, 7));
                sb.Append(ASCIIFace("3", ind, 2, 6, 5));
                sb.Append(ASCIIFace("3", ind, 6, 2, 8));
                sb.Append(ASCIIFace("3", ind, 8, 7, 6));
                sb.Append(ASCIIFace("3", ind, 7, 8, 3));
            }
            else
            {
                sb.Append("o piece_" + i.ToString("0000") + "\r\n");
                sb.Append("g piece_" + i.ToString("0000") + "\r\n");

                sb.Append(ASCIIFace("f", ind, 5, 4, 0));
                sb.Append(ASCIIFace("f", ind, 4, 5, 6));
                sb.Append(ASCIIFace("f", ind, 6, 1, 4));
                sb.Append(ASCIIFace("f", ind, 1, 6, 7));
                sb.Append(ASCIIFace("f", ind, 2, 6, 5));
                sb.Append(ASCIIFace("f", ind, 6, 2, 8));
                sb.Append(ASCIIFace("f", ind, 8, 7, 6));
                sb.Append(ASCIIFace("f", ind, 7, 8, 3));
            }
            */

            return sb.ToString();
        }


        public string ASCIIFace(string label, int x, int y, int z)
        {
            return label + " " + x + " " + y + " " + z + "\r\n";
        }


    }
}
