using CTRFramework.Shared;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CTRtools.SFX
{
    public class SampleInfo
    {
        public short id;
        public int size;
        public int offset;
    }

    public class Bank
    {
        public Dictionary<int, VAG> samples = new Dictionary<int, VAG>();

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
            List<SampleInfo> info = new List<SampleInfo>();

            int sampCnt = br.ReadInt16();

            for (int i = 0; i < sampCnt; i++)
            {
                info.Add(new SampleInfo());
                info[i].id = br.ReadInt16();
            }


            br.Jump(0x800);

            byte[] buf;
            int curr = 0;
            List<int> offs = new List<int>();
            List<int> sizes = new List<int>();

            do
            {
                buf = br.ReadBytes(16);

                if (frameIsEmpty(buf))
                {
                    offs.Add((int)br.BaseStream.Position - 16);
                    curr++;
                }
            }
            while (curr <= sampCnt);

            for (int i = 0; i < sampCnt; i++)
            {
                info[i].offset = offs[i];
                info[i].size = offs[i + 1] - offs[i];
            }


            foreach (SampleInfo si in info)
            {
                VAG vag = new VAG();

                br.Jump(si.offset);
                vag.WaveData = br.ReadBytes(si.size);

                samples.Add(si.id, vag);
            }

        }



        public bool Contains(int key)
        {
            return samples.ContainsKey(key);
        }


        public void Export(int id)
        {
            Directory.CreateDirectory(".\\vag");
            samples[id].Save("vag\\sample_" + id.ToString("X4") + ".vag");
        }

        public void ExportAll()
        {
            foreach (KeyValuePair<int, VAG> s in samples)
                Export(s.Key);
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(samples.Count + " samples total\r\n");

            int cnt = 0;

            foreach (KeyValuePair<int, VAG> s in samples)
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
