using CTRFramework.Shared;
using System;
using System.Collections.Generic;

namespace CTRFramework.Sound
{
    public class HowlHeader : IReadWrite
    {
        public char[] magic;    //HOWL char[]

        public int u1;          //freezes the game if changed, game code tests against fixed number for some reason. maybe like version.
        public int reserved1;   //no effect
        public int reserved2;   //no effect

        public int cntUnk;      //number of entries in an unknown array, messes up all samples if anything is modified
        public int cntSfx;      //number of sample declarations, contains all sfx entries (not instruments)
        public int cntEngineSfx;//number of engine sound array entries
        public int cntBank;     //number of banks
        public int cntSeq;      //number of sequences

        public int sampleDataSize;    //whole sample data size to the last seq pointer

        public HowlHeader(BinaryReaderEx br)
        {
            Read(br);
        }

        public static HowlHeader FromReader(BinaryReaderEx br)
        {
            return new HowlHeader(br);
        }

        public void Read(BinaryReaderEx br)
        {
            magic = br.ReadChars(4);

            if (new string(magic) != "HOWL")
                throw new Exception("Not a CTR HOWL file.");

            u1 = br.ReadInt32();
            reserved1 = br.ReadInt32();
            reserved2 = br.ReadInt32();

            if (reserved1 != 0 || reserved2 != 0)
                Helpers.Panic(this, PanicType.Assume, "reserved is not null. Possible error.");

            cntUnk = br.ReadInt32();
            cntSfx = br.ReadInt32();
            cntEngineSfx = br.ReadInt32();

            cntBank = br.ReadInt32();
            cntSeq = br.ReadInt32();

            sampleDataSize = br.ReadInt32();
        }

        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
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

            bw.Write(cntUnk);
            bw.Write(cntSfx);
            bw.Write(cntEngineSfx);

            bw.Write(cntBank);
            bw.Write(cntSeq);

            bw.Write(sampleDataSize);
        }
    }
}
