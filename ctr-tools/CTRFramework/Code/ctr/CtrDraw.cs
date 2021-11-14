using System;
using CTRFramework.Shared;

namespace CTRFramework
{
    public class CtrDraw
    {
        public CtrDrawFlags flags;
        public byte stackIndex;
        public byte colorIndex;
        public byte texIndex;

        public uint Value => PackValue();

        public CtrDraw()
        {
        }

        public CtrDraw(uint value)
        {
            flags = (CtrDrawFlags)(value >> (8 * 3) & 0xFF);
            stackIndex = (byte)(value >> 16 & 0xFF);
            colorIndex = (byte)(value >> 9 & 0x7F);
            texIndex = (byte)(value & 0x1FF);

            Helpers.Panic(this, PanicType.Debug, value.ToString("X8") + " <-> " + Value.ToString("X8") + " " + ToString());
        }

        private uint PackValue()
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
            return $"[{Value.ToString("X8")}] " +
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
