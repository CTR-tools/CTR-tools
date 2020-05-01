using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CTRFramework
{
    class LODHeader : IRead
    {
        public string name;
        int unk0; //0?
        int unk1;
        Vector4s scale;
        int ptrCmd; //this is null if we have anims
        int ptrVerts; //0?
        public int ptrTex;
        public int ptrClut;
        int unk3; //?
        public int numAnims;
        int ptrAnims;
        int unk4; //?

        public bool IsAnimated
        {
            get
            {
                return numAnims > 0;
            }
        }


        private int maxTex = 0;
        private int maxClut = 0;

        List<CTRAnim> anims = new List<CTRAnim>();

        public List<MshCommand> defs = new List<MshCommand>();


        public Vector3s ReadVertex(BinaryReaderEx br, int i)
        {
            Vector3s v = new Vector3s(0, 0, 0);

            if (!IsAnimated)
            {
                br.Jump(ptrVerts + 0x1C + i * 3);

                // v.X = (short)(((int)br.ReadByte() / 255.0f) * scale.X);
                // v.Y = (short)(((int)br.ReadByte() / 255.0f) * scale.Z);
                // v.Z = (short)(((int)br.ReadByte() / 255.0f) * scale.Y);
            }
            else
            {
                br.Jump(ptrAnims);
                br.Jump(br.ReadInt32() + 0x18 + 0x1C + i * 3);
                //Console.WriteLine(ptrAnims.ToString("X8") + " " + br.BaseStream.Position.ToString("X8") + "");
                //Console.ReadKey();
            }

            v.X = (short)(((int)br.ReadByte() / 255.0f) * scale.X);
            v.Y = (short)(((int)br.ReadByte() / 255.0f) * scale.Z);
            v.Z = (short)(((int)br.ReadByte() / 255.0f) * scale.Y);

            return v;
        }

        public Vector4b ReadColor(BinaryReaderEx br, int i)
        {
            br.Jump(ptrClut + i * 4);
            Vector4b b = new Vector4b(br);
           // Console.WriteLine(i + " " + b.ToString());
            //Console.ReadKey();
            return b;
        }

        public TextureLayout ReadTexture(BinaryReaderEx br, int i)
        {
            br.Jump(ptrTex + i * 4);
            br.Jump(br.ReadUInt32());

            return new TextureLayout(br);
        }

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


        /*
        public void Read(BinaryReaderEx br)
        {

            Console.WriteLine(br.BaseStream.Position.ToString("X8"));
            name = br.ReadStringFixed(16);
            Console.WriteLine(name);

            unk0 = br.ReadInt32(); //0?
            unk1 = br.ReadInt32(); //probably flags
            scale = new Vector4s(br);

            //ptr
            ptrCmd = br.ReadInt32();
            ptrVerts = br.ReadInt32();
            ptrTex = br.ReadInt32();
            ptrClut = br.ReadInt32();
            unk3 = br.ReadInt32(); //?

            if (unk3 != 0)
                Console.WriteLine("unk3 == " + unk3.ToString("X8"));

            numAnims = br.ReadInt32();
            ptrAnims = br.ReadInt32();
            unk4 = br.ReadInt32(); //?

            /*
            long pos = br.BaseStream.Position;

            br.Jump(ptrCmd + 4);

            uint x;

            do
            {
                x = br.ReadUInt32Big();
                if (x != 0xFFFFFFFF)
                    defs.Add(new MshCommand(x));
            }
            while (x != 0xFFFFFFFF);


            foreach (MshCommand c in defs)
            {
                if (c.colorIndex > maxClut) { Console.Write(c.colorIndex + ", " ); maxClut = c.colorIndex; }
                if (c.texIndex - 1 > maxTex) maxTex = c.texIndex;
            }

            Console.WriteLine("maxClut: " + maxClut);
            Console.WriteLine("texClut: " + maxTex);
            //Console.ReadKey();

            br.BaseStream.Position = pos;


            
        }
    */

            public void Read(BinaryReaderEx br)
        {
            name = br.ReadStringFixed(16);

            Console.WriteLine(name);
            //Console.ReadKey();

            unk0 = br.ReadInt32(); //0?
            unk1 = br.ReadInt32(); //probably flags
            scale = new Vector4s(br);

            //ptr
            ptrCmd = br.ReadInt32();
            ptrVerts = br.ReadInt32();
            ptrTex = br.ReadInt32();
            ptrClut = br.ReadInt32();
            unk3 = br.ReadInt32(); //?

            numAnims = br.ReadInt32();
            ptrAnims = br.ReadInt32();
            unk4 = br.ReadInt32(); //?

            long pos = br.BaseStream.Position;

            br.Jump(ptrCmd + 4);

            uint x;

            do
            {
                x = br.ReadUInt32Big();
                if (x != 0xFFFFFFFF)
                    defs.Add(new MshCommand(x));
            }
            while (x != 0xFFFFFFFF);

            /*
            if (numAnims > 0)
            {
                for (int f = 0; f < numAnims; f++)
                {
                    br.Jump(ptrAnims + f * 4);
                    br.Jump(br.ReadInt32());
                    anims.Add(new CTRAnim(br));
                }
            }
            */

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("o {0}\r\n", name);

            Vector4b[] clr = new Vector4b[3];
            Vector3s[] crd = new Vector3s[4];
            Vector3s[] stack = new Vector3s[256];
            Vector3s curvert;

            bool invert = false;

            int i = 0;
            int cur_i = 0;

            int ttl_faces = 0;

            foreach (MshCommand d in defs)
            {
                Console.WriteLine(
                    d.value.ToString("X8") +
                    //" t:" + d.texIndex +
                    " c:" + d.colorIndex +
                    " s:" + d.stackIndex);

                if (d.flags.HasFlag(Flags.s))
                {
                    cur_i = 0;
                }

                if (d.flags.HasFlag(Flags.v))
                {
                    curvert = stack[d.value >> 16 & 0xFF];
                }
                else
                {
                    curvert = ReadVertex(br, i);
                    stack[d.value >> 16 & 0xFF] = curvert;
                    i++;
                }

                crd[0] = crd[1];
                crd[1] = crd[2];
                crd[2] = crd[3];
                crd[3] = curvert;

                if (d.flags.HasFlag(Flags.l))
                {
                    crd[1] = crd[0];
                }

                clr[0] = clr[1];
                clr[1] = clr[2];
                clr[2] = ReadColor(br, d.colorIndex);

                if (cur_i >= 2)
                {
                    for (int z = 1; z < 4; z++)
                        sb.AppendLine("v " + crd[z].ToString(VecFormat.Numbers) + " " + clr[z - 1].ToString(VecFormat.Numbers));

                    if (!invert)
                    {
                        sb.AppendFormat("f {0} {1} {2}\r\n\r\n", ttl_faces + 1, ttl_faces + 2, ttl_faces + 3);
                    }
                    else
                    {
                        sb.AppendFormat("f {0} {1} {2}\r\n\r\n", ttl_faces + 3, ttl_faces + 2, ttl_faces + 1);
                    }

                    invert = !invert;

                    ttl_faces += 3;
                }

                cur_i++;

                // Console.ReadKey();

            }

            Directory.CreateDirectory("mpk");
            Helpers.WriteToFile("mpk\\" + name + ".obj", sb.ToString());


            br.BaseStream.Position = pos;

        }

    


        public override string ToString()
        {
            /*
            //Console.ReadKey(
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
            return "le me";
        }
    }
}
