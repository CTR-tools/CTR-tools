using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CTRFramework
{
    public class CtrHeader : IRead
    {
        public string name = "defaultname";
        public int unk0 = 0; //0?
        public short lodDistance = 1000;
        public short billboard = 0; //bit0 forces model to always face the camera, check other bits
        public Vector4s scale = new Vector4s(1024, 1024, 1024, 0);

        public int ptrCmd = 0; //this is null if we have anims
        public int ptrVerts = 0; //0?
        public int ptrTex = 0;
        public int ptrClut = 0;

        public int unk3 = 0; //?
        public int numAnims = 0;
        public int ptrAnims = 0;
        public int unk4 = 0; //?

        public Vector4s posOffset = new Vector4s(0,0,0,0);

        public List<Vertex> verts = new List<Vertex>();

        public int cmdNum = 0x40;
        public int vrenderMode = 0x1C;

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

        public List<CtrDraw> drawList = new List<CtrDraw>();
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
            unk0 = br.ReadInt32();
            lodDistance = br.ReadInt16();
            billboard = br.ReadInt16();

            Console.WriteLine($"CtrHeader: {name}");

            if (unk0 != 0)
                Helpers.Panic(this, $"check unusual unk0 value = {unk0}");

            if (billboard > 1)
                Helpers.Panic(this, $"check unusual billboard value = {billboard}");

            scale = new Vector4s(br);

            ptrCmd = br.ReadInt32();
            ptrVerts = br.ReadInt32();
            ptrTex = br.ReadInt32();
            ptrClut = br.ReadInt32();
            unk3 = br.ReadInt32();
            numAnims = br.ReadInt32();
            ptrAnims = br.ReadInt32();
            unk4 = br.ReadInt32();

            if (unk3 != 0)
                Helpers.Panic(this, $"check unusual unk3 value = {unk3}");

            if (unk4 != 0)
                Helpers.Panic(this, $"check unusual unk4 value = {unk4}");

            long pos = br.BaseStream.Position;


            //read all drawing commands

            br.Jump(ptrCmd);

            cmdNum = br.ReadInt32();

            uint x;

            do
            {
                x = br.ReadUInt32Big();
                if (x != 0xFFFFFFFF)
                    drawList.Add(new CtrDraw(x));
            }
            while (x != 0xFFFFFFFF);

            //should read anims here

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

            //define temporary arrays
            Vector4b[] clr = new Vector4b[3];       //color buffer
            Vector3s[] crd = new Vector3s[4];       //face buffer
            Vector3s[] stack = new Vector3s[256];   //vertex buffer

            int maxv = 0;
            int maxc = 0;
            int maxt = 0;

            //one pass through all draw commands to get the array lengths
            foreach (var draw in drawList)
            {
                //only increase vertex count for commands that don't take vertex from stack
                if (!draw.flags.HasFlag(CtrDrawFlags.v))
                    maxv++;

                //simply find max color index
                if (draw.colorIndex > maxc)
                    maxc = draw.colorIndex;

                //find max index, but 0 means no texture.
                if (draw.texIndex > 0)
                    if (draw.texIndex - 1 > maxt)
                        maxt = draw.texIndex;

                Console.WriteLine(draw.ToString());
            }

            Console.WriteLine("maxv: " + maxv);
            Console.WriteLine("maxc: " + maxc);
            Console.WriteLine("maxt: " + maxt);
           
            //int ppos = (int)br.BaseStream.Position;

            br.Jump(ptrClut);
            for (int k = 0; k <= maxc; k++)
                cols.Add(new Vector4b(br));

            //if static model
            if (!IsAnimated)
            {
                br.Jump(ptrVerts);

                posOffset = new Vector4s(br);

                Console.WriteLine(posOffset);

                br.Skip(16);

                vrenderMode = br.ReadInt32();

                if (!(new List<int> { 0x1C, 0x22 } ).Contains(vrenderMode))
                {
                    Helpers.Panic(this, $"check vrender {vrenderMode.ToString("X8")}");
                }
            }
            else
            {
                //jump to first animation, read header and jump to vertex garbage
                br.Jump(ptrAnims);
                int ptr = br.ReadInt32();
                br.Jump(ptr);

                CtrAnim anim = CtrAnim.FromReader(br);

                Console.WriteLine(anim.name + " " + anim.numFrames);

                br.Skip(0x1C);

                Console.WriteLine(br.HexPos());

                posOffset = new Vector4s(0,0,0,0);
            }

            //read vertices
            for (int k = 0; k < maxv; k++)
                vtx.Add(new Vector3b(br));

            foreach (var v in vtx)
                Console.WriteLine(v.ToString(VecFormat.Hex));


            List<Vector3s> vfixed = new List<Vector3s>();

            foreach (var v in vtx)
                vfixed.Add(new Vector3s(v.X, v.Y, v.Z));

            foreach (Vector3s v in vfixed)
            {
                //scale vertices
                v.X = (short)((((float)v.X / 255.0f - 0.5) * scale.X));
                v.Y = (short)((((float)v.Y / 255.0f - 0.5) * scale.Z));
                v.Z = (short)((((float)v.Z / 255.0f) * scale.Y));

                //flip axis
                short zz = v.Z;
                v.Z = (short)-v.Y;
                v.Y = zz;
            }

            int vertexIndex = 0;
            int stripLength = 0;

            //process all commands
            foreach (CtrDraw d in drawList)
            {
                //if we got no stack vertex flag
                if (!d.flags.HasFlag(CtrDrawFlags.v))
                {
                    //push vertex from the array to the buffer
                    stack[d.stackIndex] = vfixed[vertexIndex];
                    vertexIndex++;
                }

                //push new vertex from stack
                crd[0] = crd[1];
                crd[1] = crd[2];
                crd[2] = crd[3];
                crd[3] = stack[d.stackIndex];

                if (d.flags.HasFlag(CtrDrawFlags.l))
                {
                    crd[1] = crd[0];
                } 

                //push new color
                clr[0] = clr[1];
                clr[1] = clr[2];
                clr[2] = cols[d.colorIndex];


                //if got reset flag, reset tristrip vertex counter
                if (d.flags.HasFlag(CtrDrawFlags.s))
                {
                    stripLength = 0;
                }

                //if we got 3 indices in tristrip (0,1,2)
                if (stripLength >= 2)
                {
                    //read 3 vertices and push to the array
                    for (int z = 1; z < 4; z++)
                    {
                        Vertex v = new Vertex();
                        v.coord = new Vector4s(crd[z].X, crd[z].Y, crd[z].Z, 0);
                        v.color = clr[z - 1];
                        v.color_morph = v.color;
                        verts.Add(v);
                    }

                    //ig got normal flag, change vertex order to flip normals
                    if (!d.flags.HasFlag(CtrDrawFlags.n))
                    {
                        Vertex v = verts[verts.Count - 1];
                        verts[verts.Count - 1] = verts[verts.Count - 3];
                        verts[verts.Count - 3] = v;
                    }
                }

                stripLength++;
            }

            //read texture layouts
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

            sb.AppendLine("#Converted to OBJ using model_reader, CTR-Tools by DCxDemo*.");
            sb.AppendLine($"#{Meta.GetVersionInfo()}");
            sb.AppendLine("#Original models: (C) 1999, Activision, Naughty Dog.\r\n");

            sb.AppendLine("# textures used:");

            List<string> uniquetags = new List<string>();

            foreach (TextureLayout t in tl)
            {
                if (!uniquetags.Contains(t.Tag()))
                    uniquetags.Add(t.Tag());
            }

            foreach (string t in uniquetags)
            {
                sb.AppendLine($"# {t}.png");
            }


            sb.AppendLine("o " + name);
            foreach (Vertex v in verts)
            {
                //while the lev is scaled down by 100, ctr models are scaled down by 1000?
                sb.AppendLine("v " + v.coord.X * 0.001f + " " + v.coord.Y * 0.001f  + " " + v.coord.Z * 0.001f + " " +  v.color.ToString(VecFormat.Numbers));
            }

            for (int i = 0; i < verts.Count / 3; i++)
                sb.AppendLine("f " + (i * 3 + 1) + " " + (i * 3 + 3) + " " + (i * 3 + 2));

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
                   
                    foreach (var c in drawList)
                    {
                        bw.Write(c.GetValue());
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
