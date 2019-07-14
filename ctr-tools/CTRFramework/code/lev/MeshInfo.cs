using System;
using System.IO;

namespace CTRFramework
{
    public class MeshInfo : IRead
    {
        public int facesnum;
        public int vertexnum;
        public int unk1;
        public int ptrNgonArray;
        public uint ptrvertarray;
        public int unk2; // null?
        public uint ptrfacearray;    //something else
        public int facenum;           //something else

        public MeshInfo(BinaryReader br)
        {
            Read(br);
        }

        public void Read(BinaryReader br)
        {
            facesnum = br.ReadInt32();
            vertexnum = br.ReadInt32();
            unk1 = br.ReadInt32();
            ptrNgonArray = br.ReadInt32();
            ptrvertarray = br.ReadUInt32();
            unk2 = br.ReadInt32();
            ptrfacearray = br.ReadUInt32();
            facenum = br.ReadInt32();

            if (unk2 != 0)
            {
                Console.WriteLine("unk2 != 0 !!!");
                Console.ReadKey();
            }
        }
    }
}