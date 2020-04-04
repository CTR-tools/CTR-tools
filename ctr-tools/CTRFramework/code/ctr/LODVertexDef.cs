namespace CTRFramework
{
    class LODVertexDef
    {
        public uint value;

        public byte flags { get { return (byte)(value >> (8 * 3) & 0xFF); } }
        public byte stackIndex { get { return (byte)(value >> 16 & 0xFF); } }
        public byte colorIndex { get { return (byte)(value >> 9 & 0x7F); } }
        public byte texIndex { get { return (byte)(value & 0x1FF); } }

        public LODVertexDef(uint x)
        {
            value = x;
            //Console.WriteLine(ToString());
        }

        public override string ToString()
        {
            return value.ToString("X8") + "\t" + texIndex + "\t" + colorIndex + "\t" + stackIndex + "\t" + flags;
        }
    }
}
