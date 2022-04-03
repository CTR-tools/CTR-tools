using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace CTRFramework
{
    public class VertexAnim : IReadWrite
    {
        public uint ptrVertex;
        public Vector3 Position;
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
            //unk1 = u1;
            //unk2 = u2;

            color.X = 255;// (byte)Helpers.Random.Next(0, 256);
            color.Y = 255;// (byte)Helpers.Random.Next(0, 256);
            color.Z = 255;// (byte)Helpers.Random.Next(0, 256);
            color.W = 0;
        }

        public void Read(BinaryReaderEx br)
        {
            ptrVertex = br.ReadUInt32();
            Position = br.ReadVector3s(1/100f);
            color = new Vector4b(br);
        }

        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            bw.Write(ptrVertex);
            bw.WriteVector3s(Position, 1 / 100f);
            color.Write(bw);
        }

        public override string ToString()
        {
            return $"{ptrVertex.ToString("X8")} {color} {Position}";
        }
    }
}