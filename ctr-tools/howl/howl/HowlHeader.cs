using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace howl
{
    class HowlHeader
    {
        public string magic;
        public int u1;
        public int reserved1;
        public int reserved2;
        public int cnt4;
        public int cnt81;
        public int cnt82;
        public int bankCount;
        public int seqCount;
        public int u2;

        public HowlHeader(BinaryReader br)
        {
            Read(br);
        }

        public void Read(BinaryReader br)
        {
            magic = System.Text.Encoding.ASCII.GetString(br.ReadBytes(4));

            if (magic != "HOWL")
            {
                Console.WriteLine("Not a CTR HOWL file.");
                return;
            }

            u1 = br.ReadInt32();
            reserved1 = br.ReadInt32();
            reserved2 = br.ReadInt32();

            if (reserved1 != 0 || reserved2 != 0)
            {
                Console.WriteLine("uz1 or uz2 is not null. Possible error.");
            }

            cnt4 = br.ReadInt32();
            cnt81 = br.ReadInt32();
            cnt82 = br.ReadInt32();

            bankCount = br.ReadInt32();
            seqCount = br.ReadInt32();

            u2 = br.ReadInt32();
        }
    }
}
