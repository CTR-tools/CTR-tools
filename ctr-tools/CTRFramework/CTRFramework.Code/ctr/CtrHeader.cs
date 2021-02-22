using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CTRFramework
{
    public class CtrHeader : IRead
    {
        public string name;
        int unk0; //0?
        short lodDistance;
        short billboard; //bit0 forces model to always face the camera, check other bits
        Vector4s scale;

        public int ptrCmd; //this is null if we have anims
        public int ptrVerts; //0?
        public int ptrTex;
        public int ptrClut;

        public int unk3; //?
        public int numAnims;
        public int ptrAnims;
        public int unk4; //?

        Vector4s posOffset;

        public List<Vertex> verts = new List<Vertex>();

        int cmdNum;
        int vrenderMode;

        public bool IsAnimated
        {
            get
            {
                return numAnims > 0;
            }
        }


        //private int maxTex = 0;
        //private int maxClut = 0;

        List<CtrAnim> anims = new List<CtrAnim>();

        public List<CtrDraw> defs = new List<CtrDraw>();
        public List<Vector3b> vtx = new List<Vector3b>();
        public List<TextureLayout> tl = new List<TextureLayout>();
        public List<Vector4b> cols = new List<Vector4b>();


        public bool isTextured
        {
            get { return ptrTex == ptrClut; }
        }

        public CtrHeader()
        {

        }

        public CtrHeader(BinaryReaderEx br)
        {
            Read(br);
        }

        public void Read(BinaryReaderEx br)
        {
            name = br.ReadStringFixed(16);

            Console.WriteLine(name);
            //Console.ReadKey();

            unk0 = br.ReadInt32(); //0?
            lodDistance = br.ReadInt16();
            billboard = br.ReadInt16(); //probably flags
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

            br.Jump(ptrCmd);

            cmdNum = br.ReadInt32();

            uint x;

            do
            {
                x = br.ReadUInt32Big();
                if (x != 0xFFFFFFFF)
                    defs.Add(new CtrDraw(x));
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

            int i = 0;
            int cur_i = 0;

            int maxv = 0;
            int maxc = 0;
            int maxt = 0;

            foreach (CtrDraw d in defs)
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
                br.Jump(ptrVerts);

                posOffset = new Vector4s(br);

                Console.WriteLine(posOffset);

                br.Skip(16);

                vrenderMode = br.ReadInt32();

                if (vrenderMode != 0x1C)
                {
                    Helpers.Panic(this, $"{vrenderMode.ToString("X8")}");
                   // Console.ReadKey();
                }
            }
            else
            {
                br.Jump(ptrAnims);
                br.Jump(br.ReadInt32() + 0x1C + 0x18);

                posOffset = new Vector4s(0,0,0,0);
            }


            for (int k = 0; k < maxv; k++)
                vtx.Add(new Vector3b(br));

            List<Vector3s> vfixed = new List<Vector3s>();

            foreach (var v in vtx)
                vfixed.Add(new Vector3s(v.X, v.Y, v.Z));

            foreach (Vector3s v in vfixed)
            {
                v.X = (short)(((int)v.X / 255.0f - 0.5) * (scale.X / 16f));
                v.Y = (short)(((int)v.Y / 255.0f - 0.5) * (scale.Z / 16f));
                v.Z = (short)(((int)v.Z / 255.0f) * (scale.Y / 16f));

                short zz = v.Z;
                v.Z = (short)-v.Y;
                v.Y = zz;
            }

            //br.Jump(ppos);

            foreach (CtrDraw d in defs)
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
                    curvert = vfixed[i];//ReadVertex(br, i);
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
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"name: {name}");
            sb.AppendLine($"unk0: {unk0}");
            sb.AppendLine($"lodDistance: {lodDistance}");
            sb.AppendLine($"billboard: {billboard}");
            sb.AppendLine($"scale: {scale.ToString(VecFormat.CommaSeparated)}");
            sb.AppendLine($"ptrCmd: {ptrCmd.ToString("X8")}");
            sb.AppendLine($"ptrVerts: {ptrVerts.ToString("X8")}");
            sb.AppendLine($"ptrTex: {ptrTex.ToString("X8")}");
            sb.AppendLine($"ptrClut: {ptrClut.ToString("X8")}");
            sb.AppendLine($"unk3: {unk3}");
            sb.AppendLine($"numAnims: {numAnims}");
            sb.AppendLine($"ptrAnims: {ptrAnims.ToString("X8")}");
            sb.AppendLine($"unk4: {unk4.ToString("X8")}");

            return sb.ToString();
        }

        public void Write(BinaryWriterEx bw, CtrWriteMode mode)
        {
            int pos = 0;

            switch (mode)
            {
                case CtrWriteMode.Header:

                    pos = (int)bw.BaseStream.Position;

                    bw.Write(name.ToCharArray());
                    bw.BaseStream.Position = pos + 16;

                    bw.Write(unk0);
                    bw.Write(lodDistance);
                    bw.Write(billboard);
                    scale.Write(bw);

                    if (ptrCmd != 0) CtrModel.ptrs.Add((int)bw.BaseStream.Position);
                    bw.Write(ptrCmd);

                    if (ptrVerts != 0) CtrModel.ptrs.Add((int)bw.BaseStream.Position);
                    bw.Write(ptrVerts);

                    if (ptrTex != 0) CtrModel.ptrs.Add((int)bw.BaseStream.Position);
                    bw.Write(ptrTex);

                    if (ptrClut != 0) CtrModel.ptrs.Add((int)bw.BaseStream.Position);
                    bw.Write(ptrClut);

                    if (unk3 != 0) CtrModel.ptrs.Add((int)bw.BaseStream.Position);
                    bw.Write(unk3);

                    bw.Write(numAnims);
                    
                    if (ptrAnims != 0) CtrModel.ptrs.Add((int)bw.BaseStream.Position);
                    bw.Write(ptrAnims);

                    if (unk4 != 0) CtrModel.ptrs.Add((int)bw.BaseStream.Position);
                    bw.Write(unk4);

                    break;
                case CtrWriteMode.Data:

                    //write commands

                    bw.BaseStream.Position = ptrCmd + 4;

                    bw.Write(cmdNum);
                   
                    foreach (var c in defs)
                    {
                        bw.Write(c.value);
                    }

                    bw.Write(0xFFFFFFFF);

                    //write vertices

                    bw.BaseStream.Position = ptrVerts + 4;

                    posOffset.Write(bw);

                    bw.BaseStream.Position += 16;

                    bw.Write(vrenderMode);

                    Console.WriteLine(name);

                    foreach (var x in vtx)
                    {
                        x.Write(bw);
                        Console.WriteLine(x.X.ToString("X2") + x.Y.ToString("X2") + x.Z.ToString("X2"));
                    }

                    Console.WriteLine("---");

                    //write texturelayouts

                    if (tl.Count > 0)
                    {
                        bw.BaseStream.Position = ptrTex + 4;

                        pos = (int)bw.BaseStream.Position;

                        for (int i = 0; i < tl.Count; i++)
                        {
                            CtrModel.ptrs.Add((int)bw.BaseStream.Position - 4);
                            bw.Write(pos + 4 * tl.Count + i * 12);
                        }

                        foreach (var t in tl)
                            t.Write(bw);
                    }

                    //write clut

                    bw.BaseStream.Position = ptrClut + 4;

                    foreach (var x in cols)
                    {
                        x.Write(bw);
                        Console.WriteLine(x.X.ToString("X2") + x.Y.ToString("X2") + x.Z.ToString("X2") + x.W.ToString("X2"));
                    }

                    //Console.ReadKey();


                    break;

                default: Helpers.Panic(this, "unknown mode"); break;
            }
        }
    }
}
