using CTRFramework.Shared;
using System;

namespace howl
{
    class HowlHeader
    {
        public string magic;

        public int u1; //freezes the game if changed
        public int reserved1; //no effect
        public int reserved2; //no effect

        public int cnt4;
        public int cnt81;
        public int cnt82;
        public int cntBank;
        public int cntSeq;

        public int sampleDataSize;

        public HowlHeader(BinaryReaderEx br)
        {
            Read(br);
        }

        public void Read(BinaryReaderEx br)
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
                Console.WriteLine("reserved is not null. Possible error.");
            }

            cnt4 = br.ReadInt32();
            cnt81 = br.ReadInt32();
            cnt82 = br.ReadInt32();

            cntBank = br.ReadInt32();
            cntSeq = br.ReadInt32();

            sampleDataSize = br.ReadInt32();
        }
    }
}
