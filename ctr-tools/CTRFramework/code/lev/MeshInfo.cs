using System;
using System.IO;

namespace CTRFramework
{
    public class MeshInfo : IRead
    {
        public int cntQuadBlock;
        public int cntVertex;
        public int unk1; //this is probably some third count

        public uint ptrQuadBlockArray;
        public uint ptrVertexArray;
        public uint unk2;// and this supposed to be third pointer, but it's null?

        public uint ptrColDataArray; //is it related to collision?
        public int cntColData;

        public MeshInfo()
        {
        }

        public MeshInfo(BinaryReader br)
        {
            Read(br);
        }

        public void Read(BinaryReader br)
        {
            cntQuadBlock = br.ReadInt32();
            cntVertex = br.ReadInt32();
            unk1 = br.ReadInt32(); 

            ptrQuadBlockArray = br.ReadUInt32();
            ptrVertexArray = br.ReadUInt32();
            unk2 = br.ReadUInt32(); 

            ptrColDataArray = br.ReadUInt32();
            cntColData = br.ReadInt32();

            if (unk2 != 0)
            {
                Console.WriteLine("unk2 != 0 !!!");
                Console.ReadKey();
            }
        }
    }
}