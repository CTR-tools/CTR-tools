using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace cseq
{
    public class Bank
    {
        public List<VABSample> vs = new List<VABSample>();

        public Bank()
        {
        }

        public Bank(string s)
        {
            byte[] data = File.ReadAllBytes(s);
            MemoryStream ms = new MemoryStream(data);
            BinaryReader br = new BinaryReader(ms);

            Read(br);

            br = null;
            ms = null;
            data = null;
        }


        public void Read(BinaryReader br)
        {
            List<int> offs = new List<int>();
            List<int> sizes = new List<int>();

            int sampCnt = br.ReadInt16();

            for (int i = 0; i < sampCnt; i++)
            {
                VABSample vab = new VABSample();
                vab.id = br.ReadInt16();

                vs.Add(vab);
            }

            br.BaseStream.Position = 0x800;

            byte[] buf;
            int curr = 0;

            do
            {
                buf = br.ReadBytes(16);

                if (frameIsEmpty(buf))
                {
                    offs.Add((int)br.BaseStream.Position-16);
                    curr++;
                }
            }
            while (curr <= sampCnt);

            for (int i = 0; i < sampCnt; i++)
            {
                sizes.Add(offs[i + 1] - offs[i]);
            }

            for (int i = 0; i < sampCnt; i++)
            {
                br.BaseStream.Position = offs[i];
                vs[i].data = br.ReadBytes(sizes[i]);
            }

        }


        private bool frameIsEmpty(byte[] buf)
        {
            foreach (byte b in buf)
                if (b != 0) return false;

            return true;
        }


        public bool Contains(int value)
        {
            foreach (VABSample v in vs)
            {
                if (v.id == value)
                    return true;
            }

            return false;
        }


        public void Export(int id)
        {
            foreach (VABSample v in vs)
                if (v.id == id)
                    v.Write();
        }

        public void ExportAll()
        {
            foreach (VABSample v in vs)
                v.Write();
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(vs.Count + " samples total\r\n");

            int cnt = 0;

            foreach (VABSample v in vs)
            {
                sb.Append(v.ToString()+"\r\n");
                cnt++;
            }

            return sb.ToString();
        }
    }
}
