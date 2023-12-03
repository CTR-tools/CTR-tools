using CTRFramework.Shared;
using System;
using System.Text;

namespace CTRFramework.Bash
{
    public class BashTex : IRead
    {
        //it seems like it has some autoallocator that operates in 32x32 slots

        //likely vram data width
        public int width;
        public int height;

        //it's like margin to cut from the slot?
        public byte unk01;
        public byte unk02;
        
        //probably actual pixel data size
        //if both values above are 0, then it's width and height too
        //but data width is * 4 and this is * 2
        //depends on bit depth too
        public byte unk03;
        public byte unk04;

        //seems to be always 0
        public int unk1;

        // unk21 = bit1 = 4 or 8 bits,
        // rest is paletteindex >> 1
        // last image is -2, probably means smth
        public short unk21;

        //3 if english banner (level warp name texture), japanese is 0 though
        //maybe other values in other files?
        public short unk22;

        //looks like some flags to me
        public int unk3; //possible vals = 1, 2, 8, 16

        public byte[] data = new byte[0];


        public BashTex(BinaryReaderEx br) => Read(br);

        public static BashTex FromReader(BinaryReaderEx br) => new BashTex(br);

        public void Read(BinaryReaderEx br)
        {
            width = br.ReadInt16();
            height = br.ReadInt16();

            unk01 = br.ReadByte();
            unk02 = br.ReadByte();
            unk03 = br.ReadByte();
            unk04 = br.ReadByte();

            unk1 = br.ReadInt32();

            if (unk1 != 0) Console.WriteLine("unk0 not 0!");

            unk21 = br.ReadInt16();
            unk22 = br.ReadInt16();

            if (unk22 != 0)
                Console.WriteLine($"unk22 not 0 or 3! {unk22}");

            unk3 = br.ReadInt32();

            data = br.ReadBytes(width * height * 2);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(width + "\t");
            sb.Append(height + "\t");

            sb.Append(unk01 + "\t");
            sb.Append(unk02 + "\t");

            sb.Append(unk03 + "\t");
            sb.Append(unk04 + "\t");

            sb.Append(unk1 + "\t");
            sb.Append(unk21 + "\t");
            sb.Append(unk22 + "\t");
            sb.Append(unk3 + "\t");

            return sb.ToString();
        }
    }
}