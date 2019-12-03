using CTRFramework.Shared;
using System;

namespace CTRFramework
{
    public class SceneHeader : IRead
    {
        public uint ptrMeshInfo;
        public uint ptrSkybox;  //leads to a small aray of vertices?
        public uint unk2;  //facegroup //leads to a weird array of pointers, every pointer group ends in 2 dwords - 0X0A, 0x00, those pointers lead to some array of 0x30 bytes

        public int numPickupHeaders;
        public uint ptrPickupHeaders;
        public int numPickupModels;
        public uint ptrPickupModelsPtr;

        public uint unk3;
        public uint unk4;
        public uint ptrPickupHeadersPtrArray;
        public uint unk5;

        public int null1;
        public int null2;

        public uint someCount1;
        public uint somePtr1; //leads to another array of offsets, those lead to locations in vrtex array
        public uint somePtr2; //lead to the header for the data below
        public uint somePtr3; //leads to some named data (drop, bubble, map-asphalt01) with an array of 0x0C bytes afterwards
        public uint ptrArray1;

        public SomeData[] someData;
        public PosAng[] startGrid;

        public uint somePtr4;
        public uint somePtr5;
        public uint somePtr6;
        public Vector4b backColor;

        uint ptrBuildStart;
        uint ptrBuildEnd;
        uint ptrBuildType;

        byte[] skip;

        public uint numRestartPts;
        public uint ptrRestartPts;

        byte[] skip2;

        public uint ptrAiNav;

        byte[] skip3;


        public SceneHeader()
        {
        }

        public SceneHeader(BinaryReaderEx br)
        {
            Read(br);
        }

        public void Read(BinaryReaderEx br)
        {
            ptrMeshInfo = br.ReadUInt32();
            ptrSkybox = br.ReadUInt32();
            unk2 = br.ReadUInt32();

            numPickupHeaders = br.ReadInt32();
            ptrPickupHeaders = br.ReadUInt32();
            numPickupModels = br.ReadInt32();
            ptrPickupModelsPtr = br.ReadUInt32();

            unk3 = br.ReadUInt32();
            unk4 = br.ReadUInt32();
            ptrPickupHeadersPtrArray = br.ReadUInt32();
            unk5 = br.ReadUInt32();

            null1 = br.ReadInt32();
            null2 = br.ReadInt32();

            if (null1 != 0 || null2 != 0)
            {
                Console.WriteLine("WARNING header.null1 = " + null1 + "; header.null2 = " + null2);
            }

            someCount1 = br.ReadUInt32();
            somePtr1 = br.ReadUInt32();
            somePtr2 = br.ReadUInt32();
            somePtr3 = br.ReadUInt32();

            ptrArray1 = br.ReadUInt32();

            someData = new SomeData[3];

            for (int i = 0; i < 3; i++)
            {
                SomeData sd = new SomeData();
                sd.Read(br);
                someData[i] = sd;
            }

            startGrid = new PosAng[8];

            for (int i = 0; i < 8; i++)
            {
                PosAng pos = new PosAng(br);
                startGrid[i] = pos;
                Console.WriteLine(startGrid[i].ToString());
            }

            somePtr4 = br.ReadUInt32();
            somePtr5 = br.ReadUInt32();
            somePtr6 = br.ReadUInt32();
            backColor = new Vector4b(br);

            br.ReadUInt32();
            ptrBuildStart = br.ReadUInt32();
            ptrBuildEnd = br.ReadUInt32();
            ptrBuildType = br.ReadUInt32();

            skip = br.ReadBytes(0x6C - 16);

            numRestartPts = br.ReadUInt32();
            ptrRestartPts = br.ReadUInt32();

            skip2 = br.ReadBytes(0x38);

            ptrAiNav = br.ReadUInt32();

            skip3 = br.ReadBytes(0x24);

            long posx = br.BaseStream.Position;

            br.Jump(ptrBuildStart);
            Console.WriteLine(br.ReadStringNT());
            br.Jump(ptrBuildEnd);
            Console.WriteLine(br.ReadStringNT());
            br.Jump(ptrBuildType);
            Console.WriteLine(br.ReadStringNT());

            br.Jump(posx);
            //Console.ReadKey();
        }
    }
}
