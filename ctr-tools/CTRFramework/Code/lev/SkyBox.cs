using CTRFramework.Shared;
using System.Collections.Generic;
using System.Text;

namespace CTRFramework
{
    public class SkyBox : IRead
    {
        public List<Vertex> Vertices = new List<Vertex>();
        public List<Vector4s> Faces = new List<Vector4s>();

        public SkyBox()
        {
        }

        public SkyBox(BinaryReaderEx br)
        {
            Read(br);
        }

        public static SkyBox FromReader(BinaryReaderEx br)
        {
            return new SkyBox(br);
        }

        public void Read(BinaryReaderEx br)
        {
            uint numVertex = br.ReadUInt32();
            uint ptrVertex = br.ReadUInt32();

            short[] sizes = br.ReadArrayInt16(8);
            uint[] offs = br.ReadArrayUInt32(8);

            for (int i = 0; i < numVertex; i++)
                Vertices.Add(new VertexShort(br));

            for (int i = 0; i < 8; i++)
                for (int j = 0; j < sizes[i]; j++)
                    Faces.Add(new Vector4s(br));

            for (int j = 0; j < Faces.Count; j++)
            {
                Faces[j].X /= 0xC;
                Faces[j].Y /= 0xC;
                Faces[j].Z /= 0xC;
            }
        }

        public string ToObj()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var vertex in Vertices)
                sb.AppendLine(vertex.ToObj());

            foreach (var tri in Faces)
                sb.AppendLine($"f {tri.X + 1} {tri.Z + 1} {tri.Y + 1}");

            return sb.ToString();
        }
    }
}