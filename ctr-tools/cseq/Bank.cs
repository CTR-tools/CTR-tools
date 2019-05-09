using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace cseq
{
    public class Bank
    {
        short sampCnt;
        List<short> id = new List<short>();
        List<int> offs = new List<int>();

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

        public bool hasID(int myid)
        {
            foreach (int i in id)
            {
                if (i == myid)
                    return true;
            }

            return false;
        }

        public void Read(BinaryReader br)
        {
            sampCnt = br.ReadInt16();

            for (int i = 0; i < sampCnt; i++)
            {
                id.Add(br.ReadInt16());
            }

            /*

            byte[] buf;

            br.BaseStream.Position = 0x800;

            int curr = 0;

            string currFile = id[0].ToString("X4") + ".vag";

            BinaryWriter bw = new BinaryWriter(File.Create(currFile));

            do
            {
                buf = br.ReadBytes(16);

                if (frameIsEmpty(buf))
                {
                    curr++;
                    offs.Add((int)br.BaseStream.Position - 16);

                    currFile = id[curr].ToString("X4") + ".vag";

                    bw.Close();
                    bw = new BinaryWriter(File.Create(currFile));
                }

                bw.Write(buf);
            }
            while (curr < sampCnt-1);

            bw.Close();


            foreach (int i in offs)
            {
                Console.WriteLine(i.ToString("X8"));
            }


            for (int i = 0; i < sampCnt; i++)
            {
                
            }
             * 
           
             * * */
        }



        public bool frameIsEmpty(byte[] buf)
        {
            foreach (byte b in buf)
                if (b != 0) return false;

            return true;
        }


        public string ListIDs()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(id.Count + " samples total\r\n");

            int cnt = 0;

            foreach (int i in id)
            {
                sb.Append(cnt.ToString("0000")+ " -> " + i.ToString("X4") + "\r\n");
                cnt++;
            }

            return sb.ToString();
        }


    }
}
