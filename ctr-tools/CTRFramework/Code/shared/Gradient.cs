using System;
using System.Collections.Generic;
using System.Text;

namespace CTRFramework.Shared
{
    public class Gradient : IReadWrite
    {
        public short From = 0;
        public short To = 0;
        public Vector4b ColorFrom = new Vector4b(0, 0, 0, 0);
        public Vector4b ColorTo = new Vector4b(0, 0, 0, 0);

        public Gradient()
        {
        }
        public Gradient(BinaryReaderEx br)
        {
            Read(br);
        }

        public static Gradient FromReader(BinaryReaderEx br)
        {
            return new Gradient(br);
        }

        public void Read(BinaryReaderEx br)
        {
            From = br.ReadInt16();
            To = br.ReadInt16();
            ColorFrom = br.ReadVector4b();
            ColorTo = br.ReadVector4b();
        }

        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            bw.Write(From);
            bw.Write(To);
            ColorFrom.Write(bw);
            ColorTo.Write(bw);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"From: {From} To: {To}");

            return sb.ToString();
        }
    }
}
