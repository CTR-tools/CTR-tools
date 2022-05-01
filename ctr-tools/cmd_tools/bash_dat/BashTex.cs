using CTRFramework;
using CTRFramework.Shared;
using System;

namespace bash_dat
{
    public class BashTex : IRead
    {
        public int width;
        public int height;
        public int unk0;
        public int unk1;
        public int unk2;
        public int unk3; //possible vals = 1, 2, 8, 16
        public byte[] data;

        public BashTex(BinaryReaderEx br)
        {
            Read(br);
        }

        public static BashTex FromReader(BinaryReaderEx br)
        {
            return new BashTex(br);
        }

        public void Read(BinaryReaderEx br)
        {
            width = br.ReadInt16();
            height = br.ReadInt16();
            unk0 = br.ReadInt32();

            if (unk1 != 0)
                Console.WriteLine("unk1 not 0!");

            unk1 = br.ReadInt32();
            unk2 = br.ReadInt32();
            unk3 = br.ReadInt32();

            if (unk3 != 1 && unk3 != 2 && unk3 != 8 && unk3 != 16)
                Console.WriteLine("unk3 not 1!" + unk3);

            data = br.ReadBytes(width * height * 2);
        }
    }
}
