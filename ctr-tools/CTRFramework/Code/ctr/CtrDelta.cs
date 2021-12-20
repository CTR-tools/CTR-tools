using CTRFramework.Shared;

namespace CTRFramework
{
    public class CtrDelta
    {
        public Vector3b Position = new Vector3b(0, 0, 0);
        public Vector3b Bits = new Vector3b(0, 0, 0);

        public CtrDelta(BinaryReaderEx br)
        {
            Read(br);
        }

        public void Read(BinaryReaderEx br)
        {
            int value = br.ReadInt32();

            Bits.X = (byte)((value >> (3 * 0)) & 7);
            Bits.Y = (byte)((value >> (3 * 1)) & 7);
            Bits.Z = (byte)((value >> (3 * 2)) & 7);

            Position.X = (byte)((value >> (9 + 8 * 0)) & 0xFF);
            Position.Y = (byte)((value >> (9 + 8 * 1)) & 0xFF);
            Position.Z = (byte)((value >> (9 + 8 * 2)) & 0xFF);
        }

        public static CtrDelta FromReader(BinaryReaderEx br)
        {
            return new CtrDelta(br);
        }
        public override string ToString()
        {
            return $"Position: {Position} Bits: {Bits}";
        }
    }
}
