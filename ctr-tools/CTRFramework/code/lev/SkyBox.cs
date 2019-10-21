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

        List<Vertex> verts = new List<Vertex>();

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

            int z = 0;

            br.BaseStream.Position = offs[z];

            for (int i = 0; i < sizes[z]; i++)
            {
                sb.Append(String.Format("f {0} {1} {2}\r\n", br.ReadInt16(), br.ReadInt16(), br.ReadInt16()));
                br.ReadInt16();
            }

            //CTRFramework.Shared.Helpers.WriteToFile("skytest.obj", sb.ToString());
        }
    }
}
