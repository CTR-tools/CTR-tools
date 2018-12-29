using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

    }
}
