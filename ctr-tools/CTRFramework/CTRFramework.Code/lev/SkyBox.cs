using CTRFramework.Shared;
using System.Collections.Generic;
using System.Text;

namespace CTRFramework
{
    public class SkyBox : IRead
    {
        public int cntVertex;
        public uint ptrVertex;
        public short[] sizes = new short[8];
        public uint[] offs = new uint[8];

        public List<Vertex> verts = new List<Vertex>();
        public List<Vector4s> faces = new List<Vector4s>();

        public SkyBox()
        {
        }

        public SkyBox(BinaryReaderEx br)
        {
            Read(br);
        }

        public void Read(BinaryReaderEx br)
        {
            cntVertex = br.ReadInt32();
            ptrVertex = br.ReadUInt32();

            sizes = br.ReadArrayInt16(8);
            offs = br.ReadArrayUInt32(8);

            for (int i = 0; i < cntVertex; i++)
            {
                Vertex x = new Vertex();
                x.ReadShort(br);

                verts.Add(x);
            }

            for (int i = 0; i < 8; i++)
                for (int j = 0; j < sizes[i]; j++)
                    faces.Add(new Vector4s(br));

            for (int j = 0; j < faces.Count; j++)
            {
                faces[j].X /= 0xC;
                faces[j].Y /= 0xC;
                faces[j].Z /= 0xC;
            }
        }

        public string ToObj()
        {
            StringBuilder sb = new StringBuilder();

            foreach (Vertex v in verts)
                sb.Append(v.ToString() + "\r\n");

            foreach (Vector4s tri in faces)
                sb.AppendLine($"f {tri.X + 1} {tri.Z + 1} {tri.Y + 1}");

            return sb.ToString();
        }
    }
}
