using System;
using System.Text;

namespace CTRFramework
{
    public enum CtrDrawFlags
    {
        s = 1 << 7,
        l = 1 << 6,
        b1 = 1 << 5,
        b2 = 1 << 4,
        k = 1 << 3,
        v = 1 << 2,
        b3 = 1 << 1,
        b4 = 1 << 0
    }

    public class CtrDraw
    {
        public uint value = 0;

        //public byte flagsval => (byte)(value >> (8 * 3) & 0xFF);
        public CtrDrawFlags flags;// => (Flags)flagsval;
        public byte stackIndex; //=> (byte)(value >> 16 & 0xFF);
        public byte colorIndex; // => (byte)(value >> 9 & 0x7F);
        public byte texIndex; // => (byte)(value & 0x1FF);

        public CtrDraw()
        {
        }

        public CtrDraw(uint x)
        {
            value = x;

            flags = (CtrDrawFlags)(x >> (8 * 3) & 0xFF);
            stackIndex = (byte)(x >> 16 & 0xFF);
            colorIndex = (byte)(x >> 9 & 0x7F);
            texIndex = (byte)(x & 0x1FF);

            Console.WriteLine(value.ToString("X8") + " " + GetValue().ToString("X8"));
        }

        public uint GetValue()
        {
            uint packbackvalue = 0;

            packbackvalue |= (uint)((byte)flags << 24);
            packbackvalue |= (uint)(stackIndex << 16);
            packbackvalue |= (uint)(colorIndex << 9);
            packbackvalue |= (uint)(texIndex);
            
            
            /*
            packbackvalue |= (uint)((byte)flags);
            packbackvalue |= (uint)(stackIndex << 8);
            packbackvalue |= (uint)(colorIndex << 16);
            packbackvalue |= (uint)(texIndex << 23);
            */


            return packbackvalue;
        }

        public override string ToString()
        {
            return $"[{value.ToString("X8")}] f:{((byte)flags).ToString("X2")} s:{stackIndex} t:{texIndex} c:{colorIndex}";
        }
    }
}
