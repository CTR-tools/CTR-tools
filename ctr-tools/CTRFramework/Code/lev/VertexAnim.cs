using CTRFramework.Shared;
using System;
using System.Collections.Generic;

namespace CTRFramework
{
    public class VertexAnim : IReadWrite
    {
        public uint ptrVertex;
        public uint unk1;
        public uint unk2;
        public Vector4b color;

        public VertexAnim()
        {
        }

        public VertexAnim(BinaryReaderEx br)
        {
            Read(br);
        }

        public void RandomizeColors(uint u1, uint u2)
        {
            unk1 = u1;
            unk2 = u2;

            color.X = 255;// (byte)Helpers.Random.Next(0, 256);
            color.Y = 255;// (byte)Helpers.Random.Next(0, 256);
            color.Z = 255;// (byte)Helpers.Random.Next(0, 256);
            color.W = 0;
        }

        public void Read(BinaryReaderEx br)
        {
            ptrVertex = br.ReadUInt32();
            unk1 = br.ReadUInt32();
            unk2 = br.ReadUInt32();
            color = new Vector4b(br);
        }

        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            bw.Write(ptrVertex);
            bw.Write(unk1);
            bw.Write(unk2);
            color.Write(bw);
        }

        public override string ToString()
        {
            return ptrVertex.ToString("X8") + " " + color.ToString() + " " + unk1.ToString("X8") + " " + unk2.ToString("X8");
        }
    }
}