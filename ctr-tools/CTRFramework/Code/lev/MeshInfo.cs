using CTRFramework.Shared;
using System;

namespace CTRFramework
{
    public class MeshInfo : IRead
    {
        public uint cntQuadBlock;
        public uint cntVertex;
        public uint cntUnk; //this is probably some third count

        public uint ptrQuadBlockArray;
        public uint ptrVertexArray;
        public uint ptrUnk;// and this supposed to be third pointer, but it's null?

        public uint ptrVisDataArray; //is it related to collision?
        public uint cntColData;

        public MeshInfo()
        {
        }

        public MeshInfo(BinaryReaderEx br)
        {
            Read(br);
        }

        public void Read(BinaryReaderEx br)
        {
            cntQuadBlock = br.ReadUInt32();
            cntVertex = br.ReadUInt32();
            cntUnk = br.ReadUInt32();

            ptrQuadBlockArray = br.ReadUInt32();
            ptrVertexArray = br.ReadUInt32();
            ptrUnk = br.ReadUInt32();

            ptrVisDataArray = br.ReadUInt32();
            cntColData = br.ReadUInt32();

            if (ptrUnk != 0)
            {
                Console.WriteLine("MeshInfo unk2 != 0 !!!");
                Console.ReadKey();
            }
        }
    }
}