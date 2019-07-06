using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace model_reader
{
    class CTRHeader
    {
        public uint ptrInfo;
        public uint unk1;  //leads to a small aray of vertices?
        public uint unk2;  //leads to a weird array of pointers, every pointer group ends in 2 dwords - 0X0A, 0x00, those pointers lead to some array of 0x30 bytes

        public int numPickupHeaders;
        public uint ptrPickupHeaders;
        public int numPickupModels;
        public uint ptrFacesArray;

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
        public PosAng[] startPos;

        public CTRHeader(BinaryReader br)
        {
            ptrInfo = br.ReadUInt32();
            unk1 = br.ReadUInt32();
            unk2 = br.ReadUInt32();

            numPickupHeaders = br.ReadInt32();
            ptrPickupHeaders = br.ReadUInt32();
            numPickupModels = br.ReadInt32();
            ptrFacesArray = br.ReadUInt32(); //point to some 9 offsets array in park

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

            startPos = new PosAng[8];

            for (int i = 0; i < 8; i++)
            {
                PosAng pos = new PosAng(br);
                startPos[i] = pos;
            }
        }
    }
}
