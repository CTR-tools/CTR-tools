using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CTRtools
{
    class CTRNgon
    {
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
    }
}
