using CTRFramework.Shared;
using System.Collections.Generic;

namespace CTRFramework
{
    public class CtrFrame
    {
        public Vector4s posOffset = new Vector4s(0, 0, 0, 0);
        public int vrenderMode = 0;
        public List<Vector3b> Vertices = new List<Vector3b>();

        public CtrFrame(BinaryReaderEx br, int numVerts)
        {
            Read(br, numVerts);
        }

        public static CtrFrame FromReader(BinaryReaderEx br, int numVerts)
        {
            return new CtrFrame(br, numVerts);
        }

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

            vrenderMode = br.ReadInt32();

            if (!(new List<int> { 0x1C, 0x22 }).Contains(vrenderMode))
            {
                Helpers.Panic(this, PanicType.Assume, $"check vrender {vrenderMode.ToString("X8")}");
            }

            for (int i = 0; i < numVerts; i++)
                Vertices.Add(new Vector3b(br));
        }
    }
}