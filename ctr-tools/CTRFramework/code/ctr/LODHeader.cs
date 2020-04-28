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
        int ptrFaces; //this is null if we have anims
        int ptrVerts; //0?
        public int ptrTex;
        public int ptrClut;
        int unk3; //?
        public int numAnims;
        int ptrAnims;
        int unk4; //?

        List<CTRAnim> anims = new List<CTRAnim>();

        public List<MshCommand> defs = new List<MshCommand>();

        public Vector3s ReadVertex(BinaryReaderEx br, int i)
        {
            Vector3s v = new Vector3s(0, 0, 0);

            if (ptrVerts != 0)
            {
                br.Jump(ptrVerts + 0x1C + i * 3);

                v.X = (short)(((int)br.ReadByte() / 255.0f) * scale.X);
                v.Y = (short)(((int)br.ReadByte() / 255.0f) * scale.Z);
                v.Z = (short)(((int)br.ReadByte() / 255.0f) * scale.Y);
            }

            return v;
        }

        public Vector3s ReadColor(BinaryReaderEx br, int i)
        {
            Vector3s c = new Vector3s(0, 0, 0);
            br.Jump(ptrClut * 4);

            c.X = br.ReadByte();
            c.Y = br.ReadByte();
            c.Z = br.ReadByte();

            return c;
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

        public void Read(BinaryReaderEx br)
        {
            name = br.ReadStringFixed(16);

            Console.WriteLine(name);
            //Console.ReadKey();

            unk0 = br.ReadInt32(); //0?
            unk1 = br.ReadInt32(); //probably flags
            scale = new Vector4s(br);

            //ptr
            ptrFaces = br.ReadInt32();
            ptrVerts = br.ReadInt32();
            ptrTex = br.ReadInt32();
            ptrClut = br.ReadInt32();
            unk3 = br.ReadInt32(); //?

            numAnims = br.ReadInt32();
            ptrAnims = br.ReadInt32();
            unk4 = br.ReadInt32(); //?

            long pos = br.BaseStream.Position;

            br.Jump(ptrFaces+4);

            uint x;

            do
            {
                x = br.ReadUInt32Big();
                if (x != 0xFFFFFFFF)
                    defs.Add(new MshCommand(x));
            }
            while (x != 0xFFFFFFFF);


            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("o {0}\r\n", name);

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

                    if (cur_i >= 2)
                    {
                        for (int z = 1; z < 4; z++)
                            sb.AppendLine("v " + crd[z].ToString(VecFormat.Numbers));

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

                if (ptrVerts != 0)
                {
                    Directory.CreateDirectory("mpk");
                    Helpers.WriteToFile("mpk\\" + name + ".obj", sb.ToString());
                }
            
                //Console.WriteLine(ReadVertex(br, d.stackIndex).ToString());
                //Console.WriteLine(d.texIndex == 0 ? "no textured" : ReadTexture(br, d.texIndex-1).ToString());
                //Console.WriteLine(d.colorIndex == 0 ? " no color?" : ReadColor(br, d.colorIndex-1).ToString());
            }

            br.BaseStream.Position = pos;


            //Console.ReadKey();

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
