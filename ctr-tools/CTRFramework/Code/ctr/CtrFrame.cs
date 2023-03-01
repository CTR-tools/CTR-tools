using CTRFramework.Shared;
using System;
using System.Collections.Generic;

namespace CTRFramework.Models
{
    public class CtrFrame
    {
        public Vector4s posOffset = new Vector4s(0, 0, 0, 0);
        public int vertOffset = 0x1C;
        public List<Vector3b> Vertices = new List<Vector3b>();

        public CtrFrame()
        {
        }

        public CtrFrame(BinaryReaderEx br, int numVerts) => Read(br, numVerts);

        public static CtrFrame FromReader(BinaryReaderEx br, int numVerts) => new CtrFrame(br, numVerts);

        public void Read(BinaryReaderEx br, int numVerts)
        {
            posOffset = new Vector4s(br);

            Helpers.Panic(this, PanicType.Debug, posOffset.ToString());

            //skipping 16 zero bytes
            for (int i = 0; i < 16; i++)
            {
                byte x = br.ReadByte();
                if (x != 0)
                    Helpers.Panic(this, PanicType.Assume, "skip value not null");
            }

            vertOffset = br.ReadInt32();

            Helpers.Panic(this, PanicType.Assume, $"vertOffset: {vertOffset.ToString("X8")}");

            br.Seek(vertOffset - 0x1C);

            for (int i = 0; i < numVerts; i++)
                Vertices.Add(new Vector3b(br));
        }

        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            posOffset.Write(bw);
            bw.Write(new byte[16]);
            bw.Write(vertOffset);

            foreach (var vertex in Vertices)
                vertex.Write(bw);
        }
    }
}