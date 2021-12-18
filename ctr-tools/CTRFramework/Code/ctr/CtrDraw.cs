using CTRFramework.Shared;

namespace CTRFramework
{
    public class CtrDraw
    {
        public CtrDrawFlags flags;
        public byte stackIndex;
        public byte colorIndex;
        public ushort texIndex;

        public uint Value =>
            (uint)((byte)flags << 24) |
            (uint)(stackIndex << 16) |
            (uint)(colorIndex << 9) |
            texIndex;

        public CtrDraw()
        {
        }

        public CtrDraw(uint input)
        {
            flags = (CtrDrawFlags)(input >> (8 * 3) & 0xFF);
            stackIndex = (byte)(input >> 16 & 0xFF);
            colorIndex = (byte)(input >> 9 & 0x7F);
            texIndex = (ushort)(input & 0x1FF);

            if (input != Value)
                Helpers.Panic(this, PanicType.Error, $"cmd value pack fails: {input.ToString("X8")} <-!!!-> {Value.ToString("X8")}");

            Helpers.Panic(this, PanicType.Debug, ToString());
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