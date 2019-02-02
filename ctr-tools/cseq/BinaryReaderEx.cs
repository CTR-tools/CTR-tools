using System.IO;

namespace cseq
{
    public class BinaryReaderEx : BinaryReader
    {
        public BinaryReaderEx(MemoryStream ms)
            : base(ms)
        {
        }

        public void Skip(long x)
        {
            this.BaseStream.Position += x;
        }

        public void Jump(long x)
        {
            this.BaseStream.Position = x;
        }

        public static BinaryReaderEx FromFile(string s)
        {
            byte[] data = File.ReadAllBytes(s);
            MemoryStream ms = new MemoryStream(data);

            return new BinaryReaderEx(ms);
        }

        public string HexPos()
        {
            return "0x" + this.BaseStream.Position.ToString("x8");
        }

        public int ReadTimeDelta()
        {
            int time = 0;
            byte next = 0;
            int ttltime = 0;

            do
            {
                byte x = ReadByte();

                time = (byte)(x & 0x7F);
                next = (byte)(x & 0x80);

                ttltime = (ttltime << 7) | time;
            }
            while (next != 0);

            return ttltime;
        }

    }
}
