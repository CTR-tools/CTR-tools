using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace cseq
{
    public class VABSample
    {
        public static int defaultrate = 11025;

        public int id;
        public int frequency = defaultrate;
        public byte[] data;

        public VABSample()
        {
        }

        public void Write()
        {
            using (BinaryWriterEx bw = new BinaryWriterEx(File.OpenWrite("sample_"+id.ToString("X4") + ".vag")))
            {
                bw.Write(System.Text.Encoding.ASCII.GetBytes("VAGp"));
                bw.WriteBig((int)3);
                bw.Write((int)0);
                bw.WriteBig((int)data.Length);
                bw.WriteBig((int)frequency);
                bw.Write((int)0);
                bw.Write((int)0);
                bw.Write((int)0);
                bw.Write(System.Text.Encoding.ASCII.GetBytes("testtesttesttest"));
                bw.Write(data);
            }
        }

        public override string ToString()
        {
            return id.ToString("X4");
        }
    }
}
