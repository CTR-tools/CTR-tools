using CTRFramework.Shared;

namespace CTRFramework.Models
{
    public class CtrDraw
    {
        public CtrDrawFlags flags;
        public byte stackIndex;
        public byte colorIndex;
        public ushort texIndex;

        public uint RawValue => packValue();

        public CtrDraw()
        {
        }

        public CtrDraw(uint value)
        {
            unpackValue(value);

            var packed = packValue();

            if (value != packed)
                Helpers.Panic(this, PanicType.Error, $"cmd value pack fails: {value.ToString("X8")} <-!!!-> {packed.ToString("X8")}");

            Helpers.Panic(this, PanicType.Debug, ToString());
        }

        private void unpackValue(uint value)
        {
            flags = (CtrDrawFlags)(value >> (8 * 3) & 0xFF);
            stackIndex = (byte)(value >> 16 & 0xFF);
            colorIndex = (byte)(value >> 9 & 0x7F);
            texIndex = (ushort)(value & 0x1FF);
        }

        private uint packValue()
        {
            return
                (uint)((byte)flags << 24) |
                (uint)(stackIndex << 16) |
                (uint)(colorIndex << 9) |
                texIndex;
        }

        public override string ToString()
        {
            return $"[{RawValue.ToString("X8")}] " +
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