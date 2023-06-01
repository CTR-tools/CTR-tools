using System;
using System.Collections.Generic;

namespace CTRFramework.Shared
{
    public class Gradient : IReadWrite
    {
        public short From = 0;
        public Vector4b FromColor = new Vector4b(0, 0, 0, 0);

        public short To = 0;
        public Vector4b ToColor = new Vector4b(0, 0, 0, 0);

        public Gradient()
        {
        }

        public Gradient(BinaryReaderEx br) => Read(br);
 
        public static Gradient FromReader(BinaryReaderEx br) => new Gradient(br);

        public void Read(BinaryReaderEx br)
        {
            From = br.ReadInt16();
            To = br.ReadInt16();
            FromColor = br.ReadVector4b();
            ToColor = br.ReadVector4b();
        }

        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            bw.Write(From);
            bw.Write(To);
            FromColor.Write(bw);
            ToColor.Write(bw);
        }

        public override string ToString() => $"Gradient: from [{From}], to [{To}]";
    }
}