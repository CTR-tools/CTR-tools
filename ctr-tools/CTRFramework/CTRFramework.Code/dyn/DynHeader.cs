using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace CTRFramework
{
    public class DynHeader : IRead
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

        public List<Vertex> verts = new List<Vertex>();

        public bool IsAnimated
        {
            get
            {
                return numAnims > 0;
            }
        }


        private int maxTex = 0;
        private int maxClut = 0;

        List<DynAnim> anims = new List<DynAnim>();

        public List<DynDraw> defs = new List<DynDraw>();
        public List<Vector3s> vtx = new List<Vector3s>();
        public List<TextureLayout> tl = new List<TextureLayout>();
        public List<Vector4b> cols = new List<Vector4b>();


        public bool isTextured
        {
            get { return ptrTex == ptrClut; }
        }

        public DynHeader()
        {

        }

        public DynHeader(BinaryReaderEx br)
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
                    defs.Add(new DynDraw(x));
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



            int maxv = 0;
            int maxc = 0;
            int maxt = 0;

            foreach (DynDraw d in defs)
            {
                if (!d.flags.HasFlag(Flags.v))
                    maxv++;

                if (d.colorIndex > maxc)
                    maxc = d.colorIndex;

                if (d.texIndex > 0)
                    if (d.texIndex - 1 > maxt)
                        maxt = d.texIndex;
            }

            Console.WriteLine("maxv: " + maxv);
            Console.WriteLine("maxc: " + maxc);
            Console.WriteLine("maxt: " + maxt);


            //int ppos = (int)br.BaseStream.Position;

            br.Jump(ptrClut);
            for (int k = 0; k <= maxc; k++)
                cols.Add(new Vector4b(br));

            if (!IsAnimated)
            {
                br.Jump(ptrVerts + 0x1C);
            }
            else
            {
                br.Jump(ptrAnims);
                br.Jump(br.ReadInt32() + 0x1C + 0x18);
            }


            for (int k = 0; k <= maxv; k++)
                vtx.Add(new Vector3s(br.ReadByte(), br.ReadByte(), br.ReadByte()));

            foreach (Vector3s v in vtx)
            {
                v.X = (short)(((int)v.X / 255.0f - 0.5) * (scale.X / 16f));
                v.Y = (short)(((int)v.Y / 255.0f - 0.5) * (scale.Z / 16f));
                v.Z = (short)(((int)v.Z / 255.0f) * (scale.Y / 16f));

                short zz = v.Z;
                v.Z = (short)-v.Y;
                v.Y = zz;
            }

            //br.Jump(ppos);

            foreach (DynDraw d in defs)
            {
                if (d.flags.HasFlag(Flags.s))
                {
                    Console.WriteLine(cur_i);
                    cur_i = 0;
                }

                if (d.flags.HasFlag(Flags.v))
                {
                    curvert = stack[d.value >> 16 & 0xFF];
                }
                else
                {
                    curvert = vtx[i];//ReadVertex(br, i);
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
                clr[2] = cols[d.colorIndex];//ReadColor(br, d.colorIndex);

                if (cur_i >= 2)
                {
                    for (int z = 1; z < 4; z++)
                    {
                        Vertex v = new Vertex();
                        v.coord = new Vector4s(crd[z].X, crd[z].Y, crd[z].Z, 0);
                        v.color = clr[z - 1];
                        v.color_morph = v.color;
                        verts.Add(v);
                    }
                }

                cur_i++;

                // Console.ReadKey();

            }

            // Directory.CreateDirectory("mpk");
            // Helpers.WriteToFile("mpk\\" + name + ".obj", sb.ToString());


            br.Jump(ptrTex);
            uint[] texptrs = br.ReadArrayUInt32(maxt);

            Console.WriteLine("texptrs: " + texptrs.Length);

            foreach (uint t in texptrs)
            {
                Console.WriteLine(t.ToString("X8"));
                br.Jump(t);
                TextureLayout tx = TextureLayout.FromStream(br);
                tl.Add(tx);
                Console.WriteLine(tx.ToString());
            }

            Console.WriteLine("tlcnt: " + tl.Count);

            br.BaseStream.Position = pos;
        }


        public string ToObj()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("o " + name);
            foreach (Vertex v in verts)
                sb.AppendLine("v " + v.coord.ToString(VecFormat.Numbers) + " " + v.color.ToString(VecFormat.Numbers));

            for (int i = 0; i < verts.Count / 3; i++)
                sb.AppendLine("f " + (i * 3 + 1) + " " + (i * 3 + 2) + " " + (i * 3 + 3));

            return sb.ToString();
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
