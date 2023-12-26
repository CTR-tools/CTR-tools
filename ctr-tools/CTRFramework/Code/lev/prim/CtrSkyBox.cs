using CTRFramework.Shared;
using System.Collections.Generic;
using System.Text;

namespace CTRFramework
{
    public class CtrSkyBox : IRead
    {
        public List<Vertex> Vertices = new List<Vertex>();
        public List<Vector4s>[] Faces = new List<Vector4s>[8];

        public CtrSkyBox()
        {
        }

        public CtrSkyBox(BinaryReaderEx br) => Read(br);

        public static CtrSkyBox FromReader(BinaryReaderEx br) => new CtrSkyBox(br);

        public void Read(BinaryReaderEx br)
        {
            uint numVertex = br.ReadUInt32();
            uint ptrVertex = br.ReadUInt32();

            short[] sizes = br.ReadArrayInt16(8);
            uint[] offs = br.ReadArrayUInt32(8);

            for (int i = 0; i < numVertex; i++)
                Vertices.Add(new VertexShort(br));

            foreach (var vert in Vertices)
            {
                Helpers.PanicAssume(this, vert.Position.ToString());
            }

            for (int i = 0; i < 8; i++)
            {
                Faces[i] = new List<Vector4s>();

                for (int j = 0; j < sizes[i]; j++)
                    Faces[i].Add(new Vector4s(br));
            }

            for (int i = 0; i < 8; i++)
                for (int j = 0; j < Faces[i].Count; j++)
                {
                    Faces[i][j].X /= 0xC;
                    Faces[i][j].Y /= 0xC;
                    Faces[i][j].Z /= 0xC;
                }
        }

        public string ToObj()
        {
            var sb = new StringBuilder();

            sb.AppendLine("# ctr skybox");

            int verts = 1;

            int i = 0;

            foreach (var face in Faces)
            {
                sb.AppendLine($"o segment_{i.ToString("00")}");

                foreach (var tri in face)
                {
                    //atm blender's obj importer treats shared vertices wrong, hence we have to duplicate them for each triangle
                    //https://blenderartists.org/t/blender-3-6-2s-obj-importer-vertex-color-bug/1478553

                    sb.AppendLine(Vertices[tri.X].ToObj());
                    sb.AppendLine(Vertices[tri.Y].ToObj());
                    sb.AppendLine(Vertices[tri.Z].ToObj());

                    sb.AppendLine($"f\t{verts} {verts + 1} {verts + 2}\r\n");

                    verts += 3;
                }

                i++;
            }

            return sb.ToString();
        }
    }
}