using System;

namespace CTRFramework
{
    public class CtrDraw
    {
        public uint value = 0;

        public CtrDrawFlags flags;
        public byte stackIndex;
        public byte colorIndex;
        public byte texIndex;

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

            Console.WriteLine(value.ToString("X8") + " " + GetValue().ToString("X8") + " " + ToString());
        }

        public uint GetValue()
        {
            uint packbackvalue = 0;

            packbackvalue |= (uint)((byte)flags << 24);
            packbackvalue |= (uint)(stackIndex << 16);
            packbackvalue |= (uint)(colorIndex << 9);
            packbackvalue |= (uint)(texIndex);

            return packbackvalue;
        }

        public override string ToString()
        {
            return $"[{value.ToString("X8")}] " +
                (flags.HasFlag(CtrDrawFlags.b4) ? "4" : "_") +
                (flags.HasFlag(CtrDrawFlags.b3) ? "3" : "_") +
                (flags.HasFlag(CtrDrawFlags.v) ? "v" : "_") +
                (flags.HasFlag(CtrDrawFlags.k) ? "k" : "_") +
                (flags.HasFlag(CtrDrawFlags.d) ? "d" : "_") +
                (flags.HasFlag(CtrDrawFlags.n) ? "n" : "_") +
                (flags.HasFlag(CtrDrawFlags.l) ? "l" : "_") +
                (flags.HasFlag(CtrDrawFlags.s) ? "s" : "_") +
                $" f: { ((byte)flags).ToString("X2")} s: {stackIndex} t: {texIndex} c: {colorIndex}";
        }
    }
}
