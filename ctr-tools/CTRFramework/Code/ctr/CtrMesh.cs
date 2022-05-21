using CTRFramework.Shared;
using CTRFramework.Vram;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using ThreeDeeBear.Models.Ply;

namespace CTRFramework
{
    public class CtrMesh : IRead
    {
        public string Name { get; set; } = "default_name";
        public int unk0 = 0; //0?
        public short lodDistance = 1000;
        public short billboard = 0; //bit0 forces model to always face the camera, check other bits
        public Vector3 scale = new Vector3(4096);

        public UIntPtr ptrCmd = UIntPtr.Zero; //this is null if we have anims
        public UIntPtr ptrVerts = UIntPtr.Zero;
        public UIntPtr ptrTex = UIntPtr.Zero;
        public UIntPtr ptrClut = UIntPtr.Zero;

        public int unk3 = 0; //?
        public int numAnims = 0;
        public UIntPtr ptrAnims = UIntPtr.Zero;
        public int unk4 = 0; //?

        //public Vector4s posOffset = new Vector4s(0, 0, 0, 0);

        public List<Vertex> verts = new List<Vertex>();
        public List<TextureLayout> matIndices = new List<TextureLayout>();

        public int cmdNum = 0x40;
        //public int vrenderMode = 0x1C;

        public bool IsAnimated
        {
            get
            {
                return numAnims > 0;
            }
        }

        #region [Component model]
        [Browsable(true), DisplayName("LOD distance"), Description(""), Category("CTR Mesh")]
        public short LodDistance
        {
            get => lodDistance;
            set => lodDistance = value;
        }
        [Browsable(true), DisplayName("Scale"), Description(""), Category("CTR Mesh")]
        public Vector3 Scale
        {
            get => scale;
            set => scale = value;
        }
        #endregion


        //private int maxTex = 0;
        //private int maxClut = 0;

        List<CtrAnim> anims = new List<CtrAnim>();

        public List<CtrDraw> drawList = new List<CtrDraw>();
        //public List<Vector3b> vtx = new List<Vector3b>();
        public CtrFrame frame;
        public List<TextureLayout> tl = new List<TextureLayout>();
        public List<Vector4b> cols = new List<Vector4b>();

        public List<int> animPtrMap = new List<int>();

        public byte[] trtire16 = new byte[] { 0x00, 0xD8, 0x00, 0x3F, 0x1F, 0xD8, 0x05, 0x00, 0x00, 0xF7, 0x1F, 0xF7 };


        public bool isTextured
        {
            get => ptrTex == ptrClut;
        }

        public CtrMesh()
        {
        }

        public CtrMesh(BinaryReaderEx br)
        {
            Read(br);
        }

        public static CtrMesh FromReader(BinaryReaderEx br)
        {
            return new CtrMesh(br);
        }

        /// <summary>
        /// Reads CTR model using BinaryReaderEx.
        /// </summary>
        /// <param name="br">BinaryReaderEx object.</param>
        public void Read(BinaryReaderEx br)
        {
            Name = br.ReadStringFixed(16);
            unk0 = br.ReadInt32();
            lodDistance = br.ReadInt16();
            billboard = br.ReadInt16();
            scale = br.ReadVector3sPadded();

            ptrCmd = br.ReadUIntPtr();
            ptrVerts = br.ReadUIntPtr();
            ptrTex = br.ReadUIntPtr();
            ptrClut = br.ReadUIntPtr();
            unk3 = br.ReadInt32();

            numAnims = br.ReadInt32();
            ptrAnims = br.ReadUIntPtr();
            unk4 = br.ReadInt32();

            Helpers.Panic(this, PanicType.Info, $"Mesh: {Name}");

            if (unk0 != 0)
                Helpers.Panic(this, PanicType.Assume, $"check unusual unk0 value = {unk0}");

            if (billboard > 1)
                Helpers.Panic(this, PanicType.Assume, $"check unusual billboard value = {billboard}");


            int returnto = (int)br.Position;

            if (IsAnimated)
            {
                br.Jump(ptrAnims);

                for (int i = 0; i < numAnims; i++)
                    animPtrMap.Add(br.ReadInt32());
            }

            if (unk3 != 0)
                Helpers.Panic(this, PanicType.Assume, $"check unusual unk3 value = {unk3}");

            if (unk4 != 0)
                Helpers.Panic(this, PanicType.Assume, $"check unusual unk4 value = {unk4}");



            //read all drawing commands

            br.Jump(ptrCmd);

            cmdNum = br.ReadInt32();

            uint x;

            do
            {
                x = br.ReadUInt32(); //big endian or little endian?
                if (x != 0xFFFFFFFF)
                    drawList.Add(new CtrDraw(x));
            }
            while (x != 0xFFFFFFFF);


            TextureLayout curtl;

            //define temporary arrays
            Vector4b[] clr = new Vector4b[4];       //color buffer
            Vector3s[] crd = new Vector3s[4];       //face buffer
            TextureLayout[] tlb = new TextureLayout[4];       //face buffer

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
                    if (draw.texIndex > maxt)
                        maxt = draw.texIndex;

                Helpers.Panic(this, PanicType.Debug, draw.ToString());
            }

            Helpers.Panic(this, PanicType.Info, $"maxv: {maxv}\r\nmaxc: {maxc}\r\nmaxt: {maxt}\r\n");

            //int ppos = (int)br.Position;

            br.Jump(ptrClut);
            for (int k = 0; k <= maxc; k++)
                cols.Add(new Vector4b(br));


            //read texture layouts
            br.Jump(ptrTex);
            uint[] texptrs = br.ReadArrayUInt32(maxt);

            Helpers.Panic(this, PanicType.Debug, "texptrs: " + texptrs.Length);
            foreach (var u in texptrs)
            {
                Helpers.Panic(this, PanicType.Debug, "ptr: " + u.ToString("X8"));
            }

            foreach (uint t in texptrs)
            {
                Helpers.Panic(this, PanicType.Debug, t.ToString("X8"));

                br.Jump(t);
                TextureLayout tx = TextureLayout.FromReader(br);
                tl.Add(tx);

                Helpers.Panic(this, PanicType.Debug, tx.ToString());
            }

            Helpers.Panic(this, PanicType.Debug, "tlcnt: " + tl.Count);

            foreach (var t in tl)
            {
                t.uv[3] = t.uv[2];
                t.NormalizeUV();
            }



            //if static model
            if (!IsAnimated)
            {
                br.Jump(ptrVerts);
            }
            else
            {
                for (int i = 0; i < numAnims; i++)
                {
                    br.Jump(animPtrMap[i]);
                    anims.Add(CtrAnim.FromReader(br, maxv));
                }

                //jump to first animation, read header and jump to vertex garbage
                br.Jump(animPtrMap[0] + 0x18);
            }

            frame = CtrFrame.FromReader(br, maxv);

            foreach (var v in frame.Vertices)
                Helpers.Panic(this, PanicType.Debug, v.ToString(VecFormat.Hex));

            List<Vector3s> vfixed = new List<Vector3s>();

            foreach (var v in frame.Vertices)
                vfixed.Add(new Vector3s(v.X, v.Y, v.Z));

            foreach (Vector3s v in vfixed)
            {
                //scale vertices
                v.X = (short)((((float)(v.X + frame.posOffset.X) / 255.0f) * scale.X));
                v.Y = (short)(-(((float)(v.Y + frame.posOffset.Z) / 255.0f) * scale.Z));
                v.Z = (short)((((float)(v.Z + frame.posOffset.Y) / 255.0f) * scale.Y));

                //flip axis
                short zz = v.Z;
                v.Z = (short)-v.Y;
                v.Y = zz;
            }

            int vertexIndex = 0;
            int stripLength = 0;

            //process all commands
            foreach (var d in drawList)
            {
                if (d.texIndex - 1 >= maxt)
                {
                    Helpers.Panic(this, PanicType.Error, $"tex index overflow. texindex = {d.texIndex} maxt = {maxt} tl.count = {tl.Count}");
                    break;
                }

                //add texture to the list, null if no texture
                curtl = d.texIndex == 0 ? null : tl[d.texIndex - 1];


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

                //push new color
                clr[0] = clr[1];
                clr[1] = clr[2];
                clr[2] = clr[3];
                clr[3] = cols[d.colorIndex];


                tlb[0] = tlb[1];
                tlb[1] = tlb[2];
                tlb[2] = tlb[3];
                tlb[3] = (d.texIndex == 0 ? null : tl[d.texIndex - 1]);


                if (d.flags.HasFlag(CtrDrawFlags.l))
                {
                    crd[1] = crd[0];
                    clr[1] = clr[0];
                }


                //if got reset flag, reset tristrip vertex counter
                if (d.flags.HasFlag(CtrDrawFlags.s))
                {
                    stripLength = 0;
                }

                //if we got 3 indices in tristrip (0,1,2)
                if (stripLength >= 2)
                {
                    //read 3 vertices and push to the array
                    for (int z = 3 - 1; z >= 0; z--)
                    {
                        Vertex v = new Vertex();
                        v.Position = new Vector3(crd[1 + z].X, crd[z + 1].Y, crd[z + 1].Z);
                        v.Color = clr[1 + z];
                        v.MorphColor = v.Color;

                        if (d.texIndex != 0)
                            v.uv = tl[d.texIndex - 1].normuv[z];

                        verts.Add(v);
                    }

                    //if got normal flag, change vertex order to flip normals
                    if (d.flags.HasFlag(CtrDrawFlags.n))
                    {
                        Vertex v = verts[verts.Count - 1];
                        verts[verts.Count - 1] = verts[verts.Count - 2];
                        verts[verts.Count - 2] = v;
                    }

                    matIndices.Add(d.texIndex != 0 ? tl[d.texIndex - 1] : null);
                }

                stripLength++;
            }

            for (int i = 0; i < verts.Count / 3; i++)
                verts.Reverse(i * 3, 3);

            br.Jump(returnto);
        }

        public List<string> GetDistinctTags()
        {
            List<string> dist = new List<string>();

            foreach (var t in tl)
            {
                if (!dist.Contains(t.Tag))
                    dist.Add(t.Tag);
            }

            return dist;
        }

        /// <summary>
        /// Exports CTR model data to OBJ format.
        /// </summary>
        /// <returns>OBJ text as string.</returns>
        public string ToObj(string filename)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("#Converted to OBJ using model_reader, CTR-Tools by DCxDemo*.");
            sb.AppendLine($"#{Meta.GetVersion()}");
            sb.AppendLine("#Original models: (C) 1999, Activision, Naughty Dog.\r\n");

            if (tl.Count > 0)
                sb.AppendLine($"mtllib {filename}.mtl\r\n");


            sb.AppendLine("o " + Name);

            foreach (var v in verts)
            {
                //while the lev is scaled down by 100, ctr models are scaled down by 1000?
                sb.AppendLine(v.ToObj(1 / 1000f));
            }

            Helpers.Panic(this, PanicType.Debug, $"{matIndices.Count}");
            Helpers.Panic(this, PanicType.Debug, $"{verts.Count / 3}");

            for (int i = 0; i < verts.Count / 3; i++)
            {
                if (matIndices[i] != null)
                {
                    //sb.AppendLine(matIndices[i].ToObj(3));

                    sb.AppendLine($"usemtl {matIndices[i].Tag}");

                    for (int j = 0; j < 3; j++)
                    {
                        sb.AppendLine($"vt {verts[i * 3 + j].uv.X / 255f} {-verts[i * 3 + j].uv.Y / 255f}");
                    }
                }
                else
                {
                    sb.AppendLine($"usemtl no_texture");
                    sb.AppendLine($"vt 0 0");
                    sb.AppendLine($"vt 0 1");
                    sb.AppendLine($"vt 1 1");
                    // sb.AppendLine($"vt 1 0");
                }

                sb.AppendLine($"f {i * 3 + 1}/{i * 3 + 1} {i * 3 + 3}/{i * 3 + 3} {i * 3 + 2}/{i * 3 + 2}");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Creates material file for OBJ.
        /// </summary>
        /// <returns>MTL file contents.</returns>
        public string ToMtl()
        {
            if (tl.Count == 0)
                return "";

            StringBuilder sb = new StringBuilder();

            foreach (var tex in GetDistinctTags())
            {
                sb.AppendLine($"newmtl {tex}");
                sb.AppendLine($"map_Kd {Name}\\{tex}.png");
                sb.AppendLine();
            }

            sb.AppendLine($"newmtl no_texture");
            sb.AppendLine($"map_Kd {Name}\\default.png");
            sb.AppendLine();

            return sb.ToString();
        }

        /// <summary>
        /// Builds CTR model from raw data arrays.
        /// </summary>
        /// <param name="name">Model name.</param>
        /// <param name="vertices">Vertex array.</param>
        /// <param name="colors">Color array.</param>
        /// <param name="faces">Face indices array.</param>
        /// <returns>CtrMesh object.</returns>
        public static CtrMesh FromRawData(string name, List<Vector3f> vertices, List<Vector4b> colors, List<Vector3i> faces)
        {
            CtrMesh mesh = new CtrMesh();
            mesh.Name = name + "_hi";
            mesh.lodDistance = -1;
            mesh.frame = new CtrFrame();

            //mesh.tl.Add(new TextureLayout());

            List<Vector4b> cc = new List<Vector4b>();

            foreach (var c in colors)
            {
                System.Drawing.Color cl = Tim.Convert16(Tim.ConvertTo16(System.Drawing.Color.FromArgb(c.W, c.Z, c.Y, c.X)));

                if (cl.R == 255 && cl.G == 0 && cl.B == 255)
                    cl = System.Drawing.Color.Black;

                cc.Add(new Vector4b(cl.R, cl.G, cl.B, 0));
            }

            colors = cc;

            //get distinct values from input lists
            List<Vector3f> dVerts = new List<Vector3f>();
            List<Vector4b> dColors = new List<Vector4b>();

            foreach (var v in vertices)
            {
                if (!dVerts.Contains(v))
                    dVerts.Add(v);
            }

            foreach (var c in colors)
            {
                if (!dColors.Contains(c))
                    dColors.Add(c);
            }


            //recalculate indices for distinct arrays
            List<Vector3i> vfaces = new List<Vector3i>();
            List<Vector3i> cfaces = new List<Vector3i>();

            if (dVerts.Count != vertices.Count)
            {
                foreach (var f in faces)
                    vfaces.Add(new Vector3i(
                        dVerts.IndexOf(vertices[f.X]),
                        dVerts.IndexOf(vertices[f.Y]),
                        dVerts.IndexOf(vertices[f.Z])
                        ));
            }

            if (dColors.Count != colors.Count)
            {
                foreach (var f in faces)
                    cfaces.Add(new Vector3i(
                        dColors.IndexOf(colors[f.X]),
                        dColors.IndexOf(colors[f.Y]),
                        dColors.IndexOf(colors[f.Z])
                        ));
            }

            if (vfaces.Count == 0) vfaces = faces;
            if (cfaces.Count == 0) cfaces = faces;

            int clutlimit = 128;

            //check for clut overflow
            if (dColors.Count > clutlimit)
            {
                Helpers.Panic("CtrHeader", PanicType.Warning, "More than 128 distinct colors! Truncating...");
                dColors = dColors.GetRange(0, clutlimit);

                foreach (var x in cfaces)
                {
                    if (x.X >= clutlimit) x.X = 0;
                    if (x.Y >= clutlimit) x.Y = 0;
                    if (x.Z >= clutlimit) x.Z = 0;
                }
            }


            //get bbox
            BoundingBox bb = BoundingBox.GetBB(dVerts);

            //offset the bbox to world origin
            BoundingBox bb2 = bb - bb.minf;

            //offset all vertices to world origin
            for (int i = 0; i < dVerts.Count; i++)
                dVerts[i] -= bb.minf;

            //save converted offset to model
            mesh.frame.posOffset = new Vector4s(
                (short)(bb.minf.X / bb2.maxf.X * 255),
                (short)(bb.minf.Y / bb2.maxf.Y * 255),
                (short)(bb.minf.Z / bb2.maxf.Z * 255),
                0);

            //save scale to model
            mesh.scale = new Vector3(
                bb2.maxf.X * 1000f,
                bb2.maxf.Y * 1000f,
                bb2.maxf.Z * 1000f
                );


            //compress vertices to byte vector 
            mesh.frame.Vertices.Clear();

            foreach (var v in dVerts)
            {
                Vector3b vv = new Vector3b(
                   (byte)(v.X / bb2.maxf.X * 255),
                   (byte)(v.Z / bb2.maxf.Z * 255),
                   (byte)(v.Y / bb2.maxf.Y * 255)
                    );

                mesh.frame.Vertices.Add(vv);
            }


            //save colors
            if (dColors.Count > 0)
            {
                mesh.cols = dColors;
            }
            else
            {
                mesh.cols.Add(new Vector4b(0x40, 0x40, 0x40, 0));
                mesh.cols.Add(new Vector4b(0x80, 0x80, 0x80, 0));
                mesh.cols.Add(new Vector4b(0xC0, 0xC0, 0xC0, 0));
            }


            //create new vertex array and loop through all faces
            List<Vector3b> newlist = new List<Vector3b>();

            for (int i = 0; i < faces.Count; i++)
            {
                CtrDraw[] cmd = new CtrDraw[3];

                cmd[0] = new CtrDraw()
                {
                    texIndex = 0,
                    colorIndex = (byte)cfaces[i].X,
                    stackIndex = 87,
                    flags = CtrDrawFlags.s | CtrDrawFlags.d //| CtrDrawFlags.k
                };

                cmd[1] = new CtrDraw()
                {
                    texIndex = 0,
                    colorIndex = (byte)cfaces[i].Z,
                    stackIndex = 88,
                    flags = CtrDrawFlags.d //| CtrDrawFlags.k
                };

                cmd[2] = new CtrDraw()
                {
                    texIndex = 0,
                    colorIndex = (byte)cfaces[i].Y,
                    stackIndex = 89,
                    flags = CtrDrawFlags.d //| CtrDrawFlags.k
                };

                newlist.Add(mesh.frame.Vertices[vfaces[i].X]);
                newlist.Add(mesh.frame.Vertices[vfaces[i].Z]);
                newlist.Add(mesh.frame.Vertices[vfaces[i].Y]);

                mesh.drawList.AddRange(cmd);
            }

            mesh.frame.Vertices = newlist;

            return mesh;
        }

        /// <summary>
        /// Loads CTR model from PLY result arrays.
        /// </summary>
        /// <param name="name">Model name.</param>
        /// <param name="ply">PlyResult object.</param>
        /// <returns>CtrHeader object.</returns>
        public static CtrMesh FromPly(string name, PlyResult ply)
        {
            List<Vector3i> faces = new List<Vector3i>();

            for (int i = 0; i < ply.Triangles.Count / 3; i++)
                faces.Add(new Vector3i(ply.Triangles[i * 3], ply.Triangles[i * 3 + 1], ply.Triangles[i * 3 + 2]));

            return FromRawData(name, ply.Vertices, ply.Colors, faces);
        }

        /// <summary>
        /// Loads CTR model from OBJ model object.
        /// </summary>
        /// <param name="name">Model name.</param>
        /// <param name="obj">OBJ object.</param>
        /// <returns>CtrHeader object.</returns>
        public static CtrMesh FromObj(string name, OBJ obj)
        {
            return FromPly(name, obj.Result);
        }

        public void ExportPly(string filename)
        {
            PlyResult ply = new PlyResult(new List<Vector3f>(), new List<int>(), new List<Vector4b>());

            foreach (var v in verts)
                ply.Vertices.Add(new Vector3f(v.Position.X, v.Position.Y, v.Position.Z));
        }

        /// <summary>
        /// Writes CTR model in original CTR format.
        /// </summary>
        /// <param name="bw">BinaryWriterEx object.</param>
        /// <param name="mode">Write mode (writes wither header or data).</param>
        public void Write(BinaryWriterEx bw, CtrWriteMode mode, List<UIntPtr> patchTable)
        {
            int pos = 0;

            switch (mode)
            {
                case CtrWriteMode.Header:
                    pos = (int)bw.BaseStream.Position;
                    bw.Write(Name.ToCharArray().Take(16).ToArray());
                    bw.Jump(pos + 16);
                    bw.Write(unk0);
                    bw.Write(lodDistance);
                    bw.Write(billboard);
                    bw.WriteVector3sPadded(scale);
                    bw.Write(ptrCmd, patchTable);
                    bw.Write(ptrVerts, patchTable);
                    bw.Write(ptrTex, patchTable);
                    bw.Write(ptrClut, patchTable);
                    bw.Write(unk3);
                    bw.Write(numAnims);
                    bw.Write(ptrAnims, patchTable);
                    bw.Write(unk4);

                    break;

                case CtrWriteMode.Data:

                    //write commands

                    bw.Jump(ptrCmd + 4);

                    bw.Write(cmdNum);

                    foreach (var c in drawList)
                        bw.Write(c.Value);

                    bw.Write(0xFFFFFFFF);

                    //write texturelayouts
                    if (tl.Count > 0)
                    {
                        bw.Jump(ptrTex + 4);

                        pos = (int)bw.BaseStream.Position - 4;

                        for (int i = 0; i < tl.Count; i++)
                        {
                            //CtrModel.ptrs.Add((int)bw.BaseStream.Position - 4);

                            UIntPtr ptr = (UIntPtr)(pos + 4 * tl.Count + i * 12);
                            bw.Write(ptr, patchTable);
                        }

                        foreach (var t in tl)
                        {
                            //bw.Write(trtire16);
                            t.Write(bw);
                        }
                    }

                    //write vertices

                    bw.Jump(ptrVerts + 4);

                    frame.Write(bw, patchTable);

                    Helpers.Panic(this, PanicType.Debug, "---");

                    //write clut

                    bw.Jump(ptrClut + 4);

                    foreach (var x in cols)
                    {
                        x.W = 0;
                        x.Write(bw);
                        Helpers.Panic(this, PanicType.Debug, x.X.ToString("X2") + x.Y.ToString("X2") + x.Z.ToString("X2") + x.W.ToString("X2"));
                    }


                    break;

                default: Helpers.Panic(this, PanicType.Warning, $"unimplemented mode {mode}"); break;
            }
        }

        /// <summary>
        /// Exports all textures
        /// </summary>
        /// <param name="path"></param>
        /// <param name="vram"></param>
        public void ExportTextures(string path, Tim vram = null)
        {
            if (vram == null)
            {
                Helpers.Panic(this, PanicType.Warning, $"No vram passed for '{Name}'.");
                return;
            }

            if (tl.Count > 0)
            {
                string texturepath = Path.Combine(path, Name);

                Helpers.CheckFolder(texturepath);

                foreach (var tex in tl)
                    vram.GetTexture(tex).Save(Path.Combine(texturepath, $"{tex.Tag}.png"));
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"\tMesh: {Name}");


            sb.AppendLine($"unk0: {unk0}");
            sb.AppendLine($"lodDistance: {lodDistance}");
            sb.AppendLine($"billboard: {billboard}");
            sb.AppendLine($"scale: {scale.ToString()}");

            sb.AppendLine($"ptrCmd: {ptrCmd.ToUInt32().ToString("X8")}");
            sb.AppendLine($"ptrVerts: {ptrVerts.ToUInt32().ToString("X8")}");
            sb.AppendLine($"ptrTex: {ptrTex.ToUInt32().ToString("X8")}");
            sb.AppendLine($"ptrClut: {ptrClut.ToUInt32().ToString("X8")}");
            sb.AppendLine($"unk3: {unk3}");
            sb.AppendLine($"numAnims: {numAnims}");
            sb.AppendLine($"ptrAnims: {ptrAnims.ToUInt32().ToString("X8")}");
            sb.AppendLine($"unk4: {unk4.ToString("X8")}");

            sb.AppendLine($"numVerts: {verts.Count}");

            foreach (var entry in anims)
            {
                sb.AppendLine($"\t\t{entry.Name} ({entry.numFrames} frames)");
                sb.AppendLine($"\t\t\tcompressed? {entry.IsCompressed}");
                sb.AppendLine($"\t\t\tnumDeltas: {entry.deltas.Count}");
                sb.AppendLine($"\t\t\tframeSize: {entry.frameSize}");
            }

            return sb.ToString();
        }
    }
}
