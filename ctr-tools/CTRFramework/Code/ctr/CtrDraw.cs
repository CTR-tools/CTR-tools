using CTRFramework.Shared;

namespace CTRFramework.Models
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
                (flags.HasFlag(CtrDrawFlags.Unused2) ? "4" : "_") +
                (flags.HasFlag(CtrDrawFlags.Unused1) ? "3" : "_") +
                (flags.HasFlag(CtrDrawFlags.StackVertex) ? "v" : "_") +
                (flags.HasFlag(CtrDrawFlags.StackColor) ? "k" : "_") +
                (flags.HasFlag(CtrDrawFlags.CulledFace) ? "d" : "_") +
                (flags.HasFlag(CtrDrawFlags.FlipNormal) ? "n" : "_") +
                (flags.HasFlag(CtrDrawFlags.SwapVertex) ? "l" : "_") +
                (flags.HasFlag(CtrDrawFlags.NewTriStrip) ? "s" : "_") +
                $" f: {((byte)flags).ToString("X2")} s: {stackIndex} t: {texIndex} c: {colorIndex}";
        }
    }
}