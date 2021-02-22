namespace CTRFramework
{
    public enum Flags
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
        public uint value;

        public byte flagsval => (byte)(value >> (8 * 3) & 0xFF);
        public Flags flags => (Flags)flagsval;
        public byte stackIndex => (byte)(value >> 16 & 0xFF);
        public byte colorIndex => (byte)(value >> 9 & 0x7F);
        public byte texIndex => (byte)(value & 0x1FF);

        public CtrDraw(uint x)
        {
            value = x;
            //Console.WriteLine(ToString());
        }

        public override string ToString()
        {
            return
                value.ToString("X8") + "\t" +
                flagsval + "\t" +
                stackIndex + "\t" +
                colorIndex + "\t";
        }
    }
}
