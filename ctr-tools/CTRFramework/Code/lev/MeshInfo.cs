using CTRFramework.Shared;
using System.Collections.Generic;
using System;

namespace CTRFramework
{
    public class MeshInfo : IRead
    {
        public uint numQuadBlocks;
        public uint numVertices;
        public uint numUnk; //this is probably some third count

        public uint ptrQuadBlocks;
        public uint ptrVertices;
        public uint ptrUnk;// and this supposed to be third pointer, but it's null?

        public uint ptrVisData; //visibility bsp tree
        public uint numVisData;

        public List<QuadBlock> QuadBlocks = new List<QuadBlock>();
        public List<Vertex> Vertices = new List<Vertex>();
        public List<VisData> VisData = new List<VisData>();

        public MeshInfo()
        {
        }

        public MeshInfo(BinaryReaderEx br)
        {
            Read(br);
        }

        public void Read(BinaryReaderEx br)
        {
            numQuadBlocks = br.ReadUInt32();
            numVertices = br.ReadUInt32();
            numUnk = br.ReadUInt32();

            PtrWrap<QuadBlock> ptrQuadBlocks2 = new PtrWrap<QuadBlock>(br);
            PtrWrap<Vertex> ptrVertices2 = new PtrWrap<Vertex>(br);
            ptrUnk = br.ReadUInt32();

            PtrWrap<VisData> ptrVisData2 = new PtrWrap<VisData>(br);
            numVisData = br.ReadUInt32();

            ptrQuadBlocks = ptrQuadBlocks2.Pointer.ToUInt32();
            ptrVertices = ptrVertices2.Pointer.ToUInt32();
            ptrVisData = ptrVisData2.Pointer.ToUInt32();

            if (ptrUnk != 0)
                Helpers.Panic(this, PanicType.Assume, $"ptrUnk != 0 !!! {ptrUnk}");

            QuadBlocks = ptrQuadBlocks2.GetList(br, numQuadBlocks);
            Vertices = ptrVertices2.GetList(br, numVertices);
            VisData = ptrVisData2.GetList(br, numVisData);
        }
    }
}