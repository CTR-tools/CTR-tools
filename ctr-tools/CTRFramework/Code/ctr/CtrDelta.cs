using CTRFramework.Shared;

namespace CTRFramework.Models
{
    public class CtrDelta
    {
        // base position difference value
        public Vector3b Position = new Vector3b(0, 0, 0);

        // additional bits to take from shared compressed vertex stream
        public Vector3b Bits = new Vector3b(0, 0, 0);

        private uint packedValue => packValue();

        public CtrDelta(uint value) => unpackValue(value);

        public CtrDelta(BinaryReaderEx br) => unpackValue(br.ReadUInt32());

        public static CtrDelta FromReader(BinaryReaderEx br) => new CtrDelta(br);

        /// <summary>
        /// Retrieves delta struct as a packed uint.
        /// </summary>
        /// <returns></returns>
        private uint packValue()
        {
            // !!! note that X is shifted by 1 bit, it drops LSB
            return
                (uint)((Bits.X & 7) << (3 * 2)) |
                (uint)((Bits.Y & 7) << (3 * 1)) |
                (uint)((Bits.Z & 7) << (3 * 0)) |
                (uint)(((Position.X >> 1) & 0x7F) << (9 + 8 * 2)) |
                (uint)((Position.Y & 0xFF) << (9 + 8 * 1)) |
                (uint)((Position.Z & 0xFF) << (9 + 8 * 0));
        }

        /// <summary>
        /// Unpacks uint value to a delta struct.
        /// </summary>
        /// <returns></returns>
        private void unpackValue(uint value)
        {
            // 32 bits = 9 (3*3) bits + 8 (Z) + 8 (Y) + 7 (X)

            Bits.X = (byte)((value >> (3 * 2)) & 7);
            Bits.Y = (byte)((value >> (3 * 1)) & 7);
            Bits.Z = (byte)((value >> (3 * 0)) & 7);

            // !!! note that X comes last, and there is one bit overlap, so it only stores 7 bits of X - it drops LSB
            Position.X = (byte)((value >> (9 + 8 * 2)) << 1 & 0xFF);
            Position.Y = (byte)((value >> (9 + 8 * 1)) & 0xFF);
            Position.Z = (byte)((value >> (9 + 8 * 0)) & 0xFF);

            // validate decompression
            Helpers.PanicIf(value != packedValue, this, PanicType.Error, $"CtrDelta fail, values do not match: {value.ToString("X8")} != {packedValue.ToString("X8")}");
        }

        public override string ToString() => $"CtrDelta (Position: {Position} Bits: {Bits})";
    }
}