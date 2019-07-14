using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CTRFramework
{
    class LODHeader
    {
        string name;
        int unk0; //0?
        int unk1;
        short s0;
        short s1;
        short s2;
        short s3;
        int vdecloffset;
        int unk2; //0?
        public int offsettooffsets;
        public int palPtr;
        int unk3; //?
        public int animsCnt;
        int offsettooffsets2;
        int unk4; //?

        public void Read(BinaryReader br)
        {
            name = System.Text.Encoding.ASCII.GetString(br.ReadBytes(16)).Replace("\0", "");
            unk0 = br.ReadInt32(); //0?
            unk1 = br.ReadInt32();
            s0 = br.ReadInt16();
            s1 = br.ReadInt16();
            s2 = br.ReadInt16();
            s3 = br.ReadInt16();
            vdecloffset = br.ReadInt32();
            unk2 = br.ReadInt32(); //0?
            offsettooffsets = br.ReadInt32();
            palPtr = br.ReadInt32();
            unk3 = br.ReadInt32(); //?
            animsCnt = br.ReadInt32();
            offsettooffsets2 = br.ReadInt32();
            unk4 = br.ReadInt32(); //?

            Console.WriteLine(name);
        }

    }
}
