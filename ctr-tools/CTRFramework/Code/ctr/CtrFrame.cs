using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace CTRFramework.Models
{
    public class CtrFrame
    {
        public Vector3 Offset = new Vector3(0, 0, 0);
        private float OffsetScale => Helpers.GteScaleSmall;

        public int ptrVerts = 0x1C;
        public List<Vector3b> Vertices = new List<Vector3b>();

        public CtrFrame()
        {
        }

        public CtrFrame(BinaryReaderEx br, int numVerts, bool compressed) => Read(br, numVerts, compressed);

        public static CtrFrame FromReader(BinaryReaderEx br, int numVerts, bool compressed) => new CtrFrame(br, numVerts, compressed);

        public void Read(BinaryReaderEx br, int numVerts, bool compressed)
        {
            if (compressed)
                throw new Exception("compressed models are not supported yet.");

            Offset = br.ReadVector3sPadded(OffsetScale); //new Vector4s(br);

            Helpers.Panic(this, PanicType.Debug, Offset.ToString());

            //skipping 16 zero bytes
            //this is used at runtime
            br.Seek(16);

            ptrVerts = br.ReadInt32();

            Helpers.Panic(this, PanicType.Assume, $"vertOffset: {ptrVerts.ToString("X8")}");

            br.Seek(ptrVerts - 0x1C);

            for (int i = 0; i < numVerts; i++)
                Vertices.Add(new Vector3b(br));
        }

        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            //posOffset.Write(bw);
            bw.WriteVector3sPadded(Offset, OffsetScale);
            bw.Write(new byte[16]);
            bw.Write(ptrVerts);

            foreach (var vertex in Vertices)
                vertex.Write(bw);
        }
    }
}