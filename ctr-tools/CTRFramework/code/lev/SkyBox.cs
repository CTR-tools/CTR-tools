using CTRFramework.Shared;
using System;
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
        public List<List<Vector4s>> faces = new List<List<Vector4s>>();

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

            StringBuilder sb = new StringBuilder();

            //sb.Append(br.BaseStream.Position.ToString("X8"));

            foreach (Vertex v in verts)
            {
                sb.Append(v.ToString(false) + "\r\n");
            }


            for (int i = 0; i < 8; i++)
            {
                List<Vector4s> ff = new List<Vector4s>();

                for (int j = 0; j < sizes[i]; j++)
                {
                    ff.Add(new Vector4s(br));
                }

                faces.Add(ff);
            }

            for (int i = 0; i < 8; i++)
            {
                sb.AppendFormat("g skyobj_{0}\r\n", i);

                foreach (Vector4s tri in faces[i])
                {
                    sb.Append(String.Format("f {0} {1} {2}\r\n",
                        (tri.X / 0xC) + 1,
                        (tri.Z / 0xC) + 1,
                        (tri.Y / 0xC) + 1
                        )
                    );
                }
            }

            CTRFramework.Shared.Helpers.WriteToFile("data.Sky.obj", sb.ToString());
        }
    }
}
