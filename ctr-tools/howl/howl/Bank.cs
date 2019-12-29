using CTRFramework.Shared;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace howl
{

    public class Bank
    {
        public Dictionary<int, byte[]> samples = new Dictionary<int, byte[]>();

        public Bank()
        {
        }

        public Bank(string s)
        {
            using (BinaryReaderEx br = new BinaryReaderEx(File.OpenRead(s)))
            {
                Read(br);
            }
        }



        public void Read(BinaryReaderEx br)
        {
            List<short> info = new List<short>();

            int bankoffset = (int)br.BaseStream.Position;

            int sampCnt = br.ReadInt16();

            for (int i = 0; i < sampCnt; i++)
                info.Add(br.ReadInt16());


            br.BaseStream.Position = bankoffset + 0x800;


            int sample_start = 0;
            int sample_end = 0;


            byte[] buf;

            for (int i = 0; i < sampCnt; i++)
            {
                sample_start = (int)br.BaseStream.Position;

                //br.BaseStream.Position += 16;

                //Console.WriteLine(br.BaseStream.Position.ToString("X8"));

                do
                {
                    buf = br.ReadBytes(16);
                }
                while (!frameIsEmpty(buf));

                sample_end = (int)br.BaseStream.Position;

                br.BaseStream.Position = sample_start;

                samples.Add(info[i], br.ReadBytes(sample_end - sample_start));
            }
        }



        public bool Contains(int key)
        {
            return samples.ContainsKey(key);
        }


        public void Export(int id, int freq)
        {
            Directory.CreateDirectory(".\\vag");

            using (BinaryWriterEx bw = new BinaryWriterEx(File.Create("vag\\sample_" + id.ToString("0000") + ".vag")))
            {
                VAGHeader vh = new VAGHeader(samples[id]);
                vh.Write(bw);
                if (freq != -1) vh.frequency = freq;
                bw.Write(samples[id]);
            }
        }

        public void ExportAll()
        {
            foreach (KeyValuePair<int, byte[]> s in samples)
                Export(s.Key, -1);
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(samples.Count + " samples total\r\n");

            int cnt = 0;

            foreach (KeyValuePair<int, byte[]> s in samples)
            {
                sb.Append(s.ToString() + "\r\n");
                cnt++;
            }

            return sb.ToString();
        }



        #region [Private functions]

        private bool frameIsEmpty(byte[] buf)
        {
            foreach (byte b in buf)
                if (b != 0) return false;

            return true;
        }

        #endregion
    }
}
