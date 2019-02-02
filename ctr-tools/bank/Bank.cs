using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace bank
{
    class Bank
    {
        short sampCnt;
        List<short> id = new List<short>();
        List<int> offs = new List<int>();


        public bool frameIsEmpty(byte[] buf)
        {
            foreach (byte b in buf)
                if (b != 0) return false;
            
            return true;
        }


        public void Read(BinaryReader br)
        {
            sampCnt = br.ReadInt16();

            for (int i = 0; i < sampCnt; i++)
            {
                id.Add(br.ReadInt16());
            }

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
        }
    }
}
