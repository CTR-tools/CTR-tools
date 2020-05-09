using CTRFramework.Shared;
using System;

namespace CTRFramework.Sound
{
    public class HowlHeader : IRead, IWrite
    {
        public char[] magic;    //HOWL char[]

        public int u1;          //freezes the game if changed
        public int reserved1;   //no effect
        public int reserved2;   //no effect

        public int cnt4;        //number of entries in an unknown array, messes up all samples if anything is modified
        public int cnt81;       //number of sample declarations, contains all sfx entries (not instruments)
        public int cnt82;       //number of engine sound array entries
        public int cntBank;     //number of banks
        public int cntSeq;      //number of sequences

        public int sampleDataSize;    //whole header to the last seq pointer size

        public HowlHeader(BinaryReaderEx br)
        {
            Read(br);
        }

        public void Read(BinaryReaderEx br)
        {
            magic = br.ReadChars(4); //System.Text.Encoding.ASCII.GetString(br.ReadBytes(4));

            if (new string(magic) != "HOWL")
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

        public void Write(BinaryWriterEx bw)
        {
            if (new string(magic) != "HOWL")
            {
                Console.WriteLine("Not a CTR HOWL file.");
                return;
            }

            bw.Write(magic);
            bw.Write(u1);
            bw.Write(reserved1);
            bw.Write(reserved2);

            bw.Write(cnt4);
            bw.Write(cnt81);
            bw.Write(cnt82);

            bw.Write(cntBank);
            bw.Write(cntSeq);

            bw.Write(sampleDataSize);

        }
    }
}
