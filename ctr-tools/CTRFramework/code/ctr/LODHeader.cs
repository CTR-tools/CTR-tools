using CTRFramework.Shared;
using System;
using System.Collections.Generic;

namespace CTRFramework
{
    class LODHeader : IRead
    {
        public string name;
        int unk0; //0?
        int unk1;
        Vector4s position;
        int ptrFaces; //this is null if we have anims
        int ptrVerts; //0?
        public int ptrTex;
        public int ptrClut;
        int unk3; //?
        public int numAnims;
        int ptrAnims;
        int unk4; //?

        List<CTRAnim> anims = new List<CTRAnim>();

        public bool isTextured
        {
            get { return ptrTex == ptrClut; }
        }

        public LODHeader()
        {

        }

        public LODHeader(BinaryReaderEx br)
        {
            Read(br);
        }

        public void Read(BinaryReaderEx br)
        {
            name = br.ReadStringFixed(16);
            unk0 = br.ReadInt32(); //0?
            unk1 = br.ReadInt32(); //probably flags
            position = new Vector4s(br);

            //ptr
            ptrFaces = br.ReadInt32();
            ptrVerts = br.ReadInt32();
            ptrTex = br.ReadInt32();
            ptrClut = br.ReadInt32();
            unk3 = br.ReadInt32(); //?

            numAnims = br.ReadInt32();
            ptrAnims = br.ReadInt32();
            unk4 = br.ReadInt32(); //?

            /*
            Console.WriteLine(name);
            Console.WriteLine((unk0).ToString("X8"));
            Console.WriteLine((unk1).ToString("X8"));
            Console.WriteLine(position.ToString(VecFormat.CommaSeparated));

            Console.WriteLine((ptrFaces).ToString("X8"));
            Console.WriteLine((ptrVerts).ToString("X8"));
            Console.WriteLine((ptrTex).ToString("X8"));
            Console.WriteLine((ptrClut).ToString("X8"));
            Console.WriteLine((unk3).ToString("X8"));

            Console.WriteLine(numAnims);
            Console.WriteLine((ptrAnims).ToString("X8"));
            Console.WriteLine((unk4).ToString("X8"));
            */
            /*
            if (unk0 != 0)
            {
                Console.WriteLine("!!! unk0 != 0 !!!" + unk0);
                Console.ReadKey();
            }

            if (unk3 != 0)
            {
                Console.WriteLine("!!! unk3 != 0 !!!" + unk3);
                Console.ReadKey();
            }
            if (unk4 != 0)
            {
                Console.WriteLine("!!! unk4 != 0 !!!" + unk4);
                Console.ReadKey();
            }
            */

            /*
            long x = br.BaseStream.Position;

            if (ptrAnims > 0)
            {           
                br.Jump(ptrAnims);

                uint[] ptrs = br.ReadArrayUInt32(numAnims);

                foreach(uint s in ptrs)
                {
                    br.Jump(s);
                    anims.Add(new CTRAnim(br, name));
                }
            }

            br.Jump(x);
            */
        }


    }
}
