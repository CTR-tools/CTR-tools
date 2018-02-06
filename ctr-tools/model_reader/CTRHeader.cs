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
        //???
        //???
        public int numPickupHeaders;
        public uint ptrPickupHeaders;
        public int numPickupModels;
        public uint ptrFacesArray;
        //???
        //???
        public uint ptrPickupHeadersPtrArray;
        public int null1;
        public int null2;

        public CTRHeader(BinaryReader br)
        {
            ptrInfo = br.ReadUInt32();

            br.BaseStream.Position += 8;
            //int offset to some array
            //int offset to animations? or some objects?

            numPickupHeaders = br.ReadInt32();
            ptrPickupHeaders = br.ReadUInt32();
            numPickupModels = br.ReadInt32();
            ptrFacesArray = br.ReadUInt32(); //point to some 9 offsets array in park

            br.BaseStream.Position += 8;

            ptrPickupHeadersPtrArray = br.ReadUInt32();
            br.BaseStream.Position += 4;
            null1 = br.ReadInt32();
            null2 = br.ReadInt32();

            if (null1 != 0 || null2 != 0)
            {
                Console.WriteLine("WARNING header.null1 = " + null1 + "; header.null2 = " + null2);
            }
        }
    }
}
