using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CTRFramework.Shared;

namespace CTRFramework
{
    public class SkyBox
    {
        public int cntVertex;
        public uint ptrVertex;
        public short[] sizes = new short[8];
        public uint[] offs = new uint[8];

        List<Vertex> verts = new List<Vertex>();

        public SkyBox(BinaryReader br)
        {
            Read(br);
        }

        public void Read(BinaryReader br)
        {
            cntVertex = br.ReadInt32();
            ptrVertex = br.ReadUInt32();

            for (int i = 0; i < 8; i++)
                sizes[i] = br.ReadInt16();

            for (int i = 0; i < 8; i++)
                offs[i] = br.ReadUInt32();

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

            File.WriteAllText("skytest.obj", sb.ToString());
        }
    }
}
