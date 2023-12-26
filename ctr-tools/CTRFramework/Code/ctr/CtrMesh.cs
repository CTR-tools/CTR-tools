using CTRFramework.Shared;
using CTRFramework.Vram;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using ThreeDeeBear.Models.Ply;

namespace CTRFramework.Models
{
    public class CtrMesh : IRead
    {
        public bool HasUntextured = false;

        public string Name { get; set; } = "default_name";
        public int unk0 = 0; //0?
        public short lodDistance = 1000;
        public short billboard = 0; //bit0 forces model to always face the camera, check other bits. probably used by orb
        public Vector3 scale = Vector3.One;

        public UIntPtr ptrCmd = UIntPtr.Zero;
        public UIntPtr ptrFrame = UIntPtr.Zero; //this is null if we have anims
        public UIntPtr ptrTex = UIntPtr.Zero;
        public UIntPtr ptrClut = UIntPtr.Zero;

        public int unk3 = 0; //?
        public int numAnims = 0;
        public UIntPtr ptrAnims = UIntPtr.Zero;
        public int unk4 = 0; //?
        public int unk5 = 0; //?

        public List<Vertex> verts = new List<Vertex>();
        public List<TextureLayout> matIndices = new List<TextureLayout>();
        public List<TextureLayout> goupedTextures = new List<TextureLayout>();

        public int unkNum = 0x40;

        //array lengths to read
        int maxv = 0;
        int maxc = 0;
        int maxt = 0;



        public bool IsAnimated => numAnims > 0;

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

        public List<CtrAnim> anims = new List<CtrAnim>();

        public List<CtrDraw> drawList = new List<CtrDraw>();
        //public List<Vector3b> vtx = new List<Vector3b>();
        public CtrFrame frame;
        public List<TextureLayout> tl = new List<TextureLayout>();
        public List<Vector4b> cols = new List<Vector4b>();

        public List<int> animPtrMap = new List<int>();

        public byte[] trtire16 = new byte[] { 0x00, 0xD8, 0x00, 0x3F, 0x1F, 0xD8, 0x05, 0x00, 0x00, 0xF7, 0x1F, 0xF7 };

        public bool isTextured => ptrTex == ptrClut;

        public List<TextureLayout> groupedtl = new List<TextureLayout>();

        public void GroupByPalette()
        {
            //create separate texture lists, grouped by PalXY

            var dict = new Dictionary<string, List<TextureLayout>>();

            foreach (var ttl in tl)
            {
                string key = $"{ttl.PalX}_{ttl.PalY}";

                if (!dict.ContainsKey(key))
                    dict[key] = new List<TextureLayout>();

                dict[key].Add(ttl);
            }

            Helpers.Panic(this, PanicType.Info, $"total entries: {dict.Keys.Count}");

            //combine every list to a single texture

            //var result = new List<TextureLayout>();

            foreach (var ttl in dict.Values)
                groupedtl.Add(Combine(ttl));

            // groupedtl.AddRange(result);
        }

        private TextureLayout Combine(List<TextureLayout> tl)
        {
            //corner cases, dont do anything

            if (tl.Count == 0) return null;
            if (tl.Count == 1) return tl[0];

            var first = tl[0];

            float maxX = 0;
            float maxY = 0;
            float minX = 20000;
            float minY = 20000;

            //find min/max UV

            foreach (var t in tl)
            {
                if (t.min.X < minX) minX = t.min.X;
                if (t.min.Y < minY) minY = t.min.Y;
                if (t.max.X > maxX) maxX = t.max.X;
                if (t.max.Y > maxY) maxY = t.max.Y;
            }

            //return new TL using first entry data and min/max values.
            //not particularly correct psx TL, but still works for export, as it guesses min/max anyways.

            var parent = new TextureLayout()
            {
                Page = first.Page,
                blendingMode = first.blendingMode,
                bpp = first.bpp,
                Palette = first.Palette,
                uv = new Vector2[]
                {
                    new Vector2(minX, minY),
                    new Vector2(minX, maxY),
                    new Vector2(maxX, minY),
                    new Vector2(maxX, maxY)
                }
            };

            parent.NormalizeUV();

            foreach (var t in tl)
            {
                t.ParentLayout = parent;
            }

            return parent;
        }

        #region [Constuctors, factories]
        public CtrMesh()
        {
        }

        public CtrMesh(BinaryReaderEx br) => Read(br);

        public static CtrMesh FromReader(BinaryReaderEx br) => new CtrMesh(br);

        #endregion

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
            scale = br.ReadVector3sPadded(Helpers.GteScaleSmall);

            ptrCmd = br.ReadUIntPtr();
            ptrFrame = br.ReadUIntPtr();
            ptrTex = br.ReadUIntPtr();
            ptrClut = br.ReadUIntPtr();
            unk3 = br.ReadInt32();

            numAnims = br.ReadInt32();
            ptrAnims = br.ReadUIntPtr();
            unk4 = br.ReadInt32();
            //unk5 = br.ReadInt32(); //?? research animated textures here, known examples - main menu crash wink, intro box 0101 scrolling texture

            Helpers.Panic(this, PanicType.Info, $"Mesh: {Name}");

            Helpers.PanicIf(unk0 != 0, this, PanicType.Assume, $"check unusual unk0 value = {unk0}");
            Helpers.PanicIf(billboard > 1, this, PanicType.Assume, $"check unusual billboard value = {billboard}");


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

            if (unk5 != 0)
                Helpers.Panic(this, PanicType.Assume, $"check unusual unk5 value = {unk5}");


            //read all drawing commands

            br.Jump(ptrCmd);

            //check what this actually is
            //usually some low value between 16 and 64
            unkNum = br.ReadInt32();

            uint x;

            do
            {
                x = br.ReadUInt32();

                if (x != 0xFFFFFFFF)
                    drawList.Add(new CtrDraw(x));
            }
            while (x != 0xFFFFFFFF);


            //one pass through all draw commands to get the array lengths
            foreach (var draw in drawList)
            {
                //only increase vertex count for commands that don't take vertex from stack
                if (!draw.flags.HasFlag(CtrDrawFlags.StackVertex))
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


            //read clut
            br.Jump(ptrClut);

            for (int k = 0; k <= maxc; k++)
                cols.Add(new Vector4b(br));


            //read texture layouts
            br.Jump(ptrTex);
            uint[] texptrs = br.ReadArrayUInt32(maxt);

            Helpers.PanicDebug(this, "texptrs: " + texptrs.Length);

            foreach (var u in texptrs)
            {
                Helpers.PanicDebug(this, "ptr: " + u.ToString("X8"));
            }

            foreach (uint t in texptrs)
            {
                Helpers.PanicDebug(this, t.ToString("X8"));

                br.Jump(t);
                var tx = TextureLayout.FromReader(br);
                tl.Add(tx);

                Helpers.PanicDebug(this, tx.ToString());
            }

            Helpers.PanicDebug(this, "tlcnt: " + tl.Count);

            foreach (var t in tl)
            {
                t.uv[3] = t.uv[2];
                t.NormalizeUV();
            }

            GroupByPalette();



            //if static model
            if (!IsAnimated)
            {
                br.Jump(ptrFrame);
                frame = CtrFrame.FromReader(br, maxv, null);
            }
            else // if animated
            {
                for (int i = 0; i < numAnims; i++)
                {
                    br.Jump(animPtrMap[i]);
                    anims.Add(CtrAnim.FromReader(br, maxv));
                }

                //take first frame of the first anim for now
                frame = anims[0].Frames[0];
            }

            GetVertexBuffer();

            br.Jump(returnto);
        }

        /// <summary>
        /// Extracts raw float vertex buffer from raw psx frame data
        /// </summary>
        /// <returns></returns>
        public List<Vertex> GetVertexBuffer()
        {
            verts.Clear();

            TextureLayout curtl;

            //define temporary arrays
            var tempColor = new Vector4b[4];    //color buffer
            var tempCoords = new Vector3[4];    //face buffer
            var tempTex = new TextureLayout[4]; //face buffer
            var stack = new Vector3[256];       //vertex buffer


            var vfixed = new List<Vector3>();

            foreach (var v in frame.Vertices)
            {
                Helpers.Panic(this, PanicType.Debug, v.ToString(VecFormat.Hex));
                vfixed.Add(CalculateFinalVertex(v, frame.Offset, scale));
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
                if (!d.flags.HasFlag(CtrDrawFlags.StackVertex))
                {
                    //push vertex from the array to the buffer
                    stack[d.stackIndex] = vfixed[vertexIndex];
                    vertexIndex++;
                }

                //push new vertex from stack
                tempCoords[0] = tempCoords[1];
                tempCoords[1] = tempCoords[2];
                tempCoords[2] = tempCoords[3];
                tempCoords[3] = stack[d.stackIndex];

                //push new color
                tempColor[0] = tempColor[1];
                tempColor[1] = tempColor[2];
                tempColor[2] = tempColor[3];
                tempColor[3] = cols[d.colorIndex];

                tempTex[0] = tempTex[1];
                tempTex[1] = tempTex[2];
                tempTex[2] = tempTex[3];
                tempTex[3] = (d.texIndex == 0 ? null : tl[d.texIndex - 1]);


                if (d.flags.HasFlag(CtrDrawFlags.SwapVertex))
                {
                    tempCoords[1] = tempCoords[0];
                    tempColor[1] = tempColor[0];

                    tempTex[1] = tempTex[0];
                }


                //if got reset flag, reset tristrip vertex counter
                if (d.flags.HasFlag(CtrDrawFlags.NewTriStrip))
                {
                    stripLength = 0;
                }

                //if we got 3 indices in tristrip (0,1,2)
                if (stripLength >= 2)
                {
                    //read 3 vertices and push to the array
                    for (int z = 3 - 1; z >= 0; z--)
                    {
                        var v = new Vertex()
                        {
                            Position = tempCoords[z + 1],
                            Color = tempColor[1 + z],
                            MorphColor = tempColor[1 + z]
                        };

                        if (d.texIndex != 0)
                        {
                            var tex = tl[d.texIndex - 1];

                            tex.NormalizeUV();

                            v.uv = tex.normuv[z];
                        }

                        verts.Add(v);
                    }

                    //if got normal flag, change vertex order to flip normals
                    if (d.flags.HasFlag(CtrDrawFlags.FlipNormal))
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

            return verts;
        }


        public List<string> GetDistinctTags()
        {
            var dist = new List<string>();

            foreach (var t in tl)
            {
                if (!dist.Contains(t.Tag))
                    dist.Add(t.Tag);
            }

            return dist;
        }

        public List<string> GetDistinctGroupedTags()
        {
            var dist = new List<string>();

            foreach (var t in groupedtl)
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
            var sb = new StringBuilder();

            sb.AppendLine("# Converted to OBJ using model_reader, CTR-Tools by DCxDemo*.");
            sb.AppendLine($"# {Meta.Version}");
            sb.AppendLine("# Original models: (C) 1999, Activision, Naughty Dog.\r\n");

            if (groupedtl.Count > 0)
                sb.AppendLine($"mtllib\t{filename}.mtl\r\n");

            sb.AppendLine($"o\t{Name}\r\n");

            foreach (var v in verts)
            {
                sb.AppendLine(v.ToObj(1));
            }

            Helpers.Panic(this, PanicType.Debug, $"{matIndices.Count}");
            Helpers.Panic(this, PanicType.Debug, $"{verts.Count / 3}");

            for (int i = 0; i < verts.Count / 3; i++)
            {
                if (matIndices[i] != null)
                {
                    for (int j = 0; j < 3; j++)
                        sb.AppendLine($"vt\t{(verts[i * 3 + j].uv.X / 255f).ToString("0.###")} {(1.0f - (verts[i * 3 + j].uv.Y / 255f)).ToString("0.###")}");
                }
                else
                {
                    sb.AppendLine($"vt 0 0");
                    sb.AppendLine($"vt 0 1");
                    sb.AppendLine($"vt 1 1");
                }
            }

            sb.AppendLine();

            //avoid repeating usemtl
            string prevtex = "$-----------nullmat------------$";

            for (int i = 0; i < verts.Count / 3; i++)
            {
                if (matIndices[i] != null)
                {
                    //sb.AppendLine(matIndices[i].ToObj(3));

                    var tex = matIndices[i];

                    if (matIndices[i].ParentLayout != null)
                        tex = tex.ParentLayout;

                    if (prevtex != tex.Tag)
                    {
                        sb.AppendLine();
                        sb.AppendLine($"usemtl\t{tex.Tag}");
                        prevtex = tex.Tag;
                    }
                }
                else
                {
                    if (prevtex != "")
                    {
                        sb.AppendLine();
                        sb.AppendLine("usemtl\tno_texture");
                        prevtex = "";
                        HasUntextured = true;
                    }
                }

                sb.AppendLine($"f\t{i * 3 + 1}/{i * 3 + 1} {i * 3 + 3}/{i * 3 + 3} {i * 3 + 2}/{i * 3 + 2}");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Creates material file for OBJ.
        /// </summary>
        /// <returns>MTL file contents.</returns>
        public string ToMtl()
        {
            if (tl.Count == 0) return "";

            var sb = new StringBuilder();

            /*
            foreach (var tex in GetDistinctTags())
            {
                sb.AppendLine($"newmtl {tex}");
                sb.AppendLine($"map_Kd {Name}\\{tex}.png");
                sb.AppendLine();
            }

            sb.AppendLine("# now grouped!");
            */

            foreach (var tex in GetDistinctGroupedTags())
            {
                sb.AppendLine($"newmtl {tex}");
                sb.AppendLine($"map_Kd {Name}\\{tex}.png");
                sb.AppendLine();
            }

            bool needDefault = false;

            foreach (var tex in matIndices)
                if (tex == null)
                {
                    needDefault = true;
                    break;
                }

            if (needDefault)
            {
                sb.AppendLine($"newmtl no_texture");
                sb.AppendLine($"map_Kd {Name}\\default.png");
                sb.AppendLine();
            }

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
        public static CtrMesh FromRawData(string name, List<Vector3> vertices, List<Vector4b> colors, List<Vector3i> faces)
        {
            var mesh = new CtrMesh() {
                Name = name + "_hi",
                lodDistance = -1,
                frame = new CtrFrame()
            };

            //mesh.tl.Add(new TextureLayout());
            /*
            //this generates black colors in OBJ, but not PLY, wtf.

            var cc = new List<Vector4b>();

            foreach (var c in colors)
            {
                System.Drawing.Color cl = Tim.Convert16(Tim.ConvertTo16(System.Drawing.Color.FromArgb(c.W, c.Z, c.Y, c.X)));

                if (cl.R == 255 && cl.G == 0 && cl.B == 255)
                    cl = System.Drawing.Color.Black;

                cc.Add(new Vector4b(cl.R, cl.G, cl.B, 0));
            }

            colors = cc;
            */


            //get distinct values from input lists
            var dVerts = new List<Vector3>();
            var dColors = new List<Vector4b>();

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
            var vfaces = new List<Vector3i>();
            var cfaces = new List<Vector3i>();

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
            var bb = CtrBoundingBox.GetBB(dVerts);

            //offset the bbox to world origin
            var bb2 = bb - bb.minf;


            //offset all vertices to world origin
            for (int i = 0; i < dVerts.Count; i++)
                dVerts[i] -= bb.minf;

            //save converted offset to model
            mesh.frame.Offset = new Vector3(
                bb.minf.X / bb2.maxf.X,
                bb.minf.Y / bb2.maxf.Y,
                bb.minf.Z / bb2.maxf.Z
            );

            //save scale to model
            mesh.scale = new Vector3(
                bb2.maxf.X,
                bb2.maxf.Y,
                bb2.maxf.Z
                ) * 256f;


            //compress vertices to byte vector 
            mesh.frame.Vertices.Clear();

            foreach (var v in dVerts)
            {
                var vv = new Vector3b(
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
            var newlist = new List<Vector3b>();

            for (int i = 0; i < faces.Count; i++)
            {
                CtrDraw[] cmd = new CtrDraw[3];

                cmd[0] = new CtrDraw()
                {
                    texIndex = 0,
                    colorIndex = (byte)cfaces[i].X,
                    stackIndex = 80, //explain this index
                    flags = CtrDrawFlags.NewTriStrip | CtrDrawFlags.CulledFace //| CtrDrawFlags.k
                };

                cmd[1] = new CtrDraw()
                {
                    texIndex = 0,
                    colorIndex = (byte)cfaces[i].Z,
                    stackIndex = 81,
                    flags = CtrDrawFlags.CulledFace //| CtrDrawFlags.k
                };

                cmd[2] = new CtrDraw()
                {
                    texIndex = 0,
                    colorIndex = (byte)cfaces[i].Y,
                    stackIndex = 82,
                    flags = CtrDrawFlags.CulledFace //| CtrDrawFlags.k
                };

                newlist.Add(mesh.frame.Vertices[vfaces[i].X]);
                newlist.Add(mesh.frame.Vertices[vfaces[i].Z]);
                newlist.Add(mesh.frame.Vertices[vfaces[i].Y]);

                mesh.drawList.AddRange(cmd);
            }


            foreach (var x in mesh.drawList)
            {
                Helpers.PanicAssume("kek", x.ToString());
            }

            Console.ReadKey();

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
            var faces = new List<Vector3i>();

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
        public static CtrMesh FromObj(string name, OBJ obj) => FromPly(name, obj.Result);

        public void ExportPly(string filename)
        {
            var ply = new PlyResult(new List<Vector3>(), new List<int>(), new List<Vector4b>());

            foreach (var v in verts)
                ply.Vertices.Add(Helpers.CloneVector(v.Position));
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
                    bw.Write(ptrFrame, patchTable);
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

                    bw.Write(unkNum);

                    foreach (var c in drawList)
                        bw.Write(c.RawValue);

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

                    bw.Jump(ptrFrame + 4);

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
            //var list = Helpers.LoadTagList("texturenames.txt");

            if (vram is null)
            {
                Helpers.Panic(this, PanicType.Warning, $"No vram passed for '{Name}'.");
                return;
            }

            /*
            if (tl.Count > 0)
            {
                string texturepath = Helpers.PathCombine(path, Name);

                Helpers.CheckFolder(texturepath);

                foreach (var tex in tl)
                    vram.GetTexture(tex).Save(Helpers.PathCombine(texturepath, $"{tex.Tag}.png"));
            */
            string texturepath = Helpers.PathCombine(path, Name);
            Helpers.CheckFolder(texturepath);

            if (groupedtl.Count > 0)
            {
                foreach (var tex in groupedtl)
                    vram.GetTexture(tex).Save(Helpers.PathCombine(texturepath, $"{tex.Tag}.png"));
            }

            if (HasUntextured)
            {
                var def = new System.Drawing.Bitmap(Properties.Resources._default);
                def.Save(Helpers.PathCombine(texturepath, "default.png"));
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"\tMesh: {Name}");


            sb.AppendLine($"unk0: {unk0}");
            sb.AppendLine($"lodDistance: {lodDistance}");
            sb.AppendLine($"billboard: {billboard}");
            sb.AppendLine($"scale: {scale.ToString()}");

            sb.AppendLine($"ptrCmd: {ptrCmd.ToUInt32().ToString("X8")}");
            sb.AppendLine($"ptrVerts: {ptrFrame.ToUInt32().ToString("X8")}");
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

        public static Vector3 CalculateFinalVertex(Vector3b compressed, Vector3 offset, Vector3 scale)
        {
            //initial compressed value is in a 0-255 grid
            //first convert to a 0.0-1.0 float ( / 255)
            //next apply negative offset (to properly center the model)
            //next multiply by the model scale, each axis separately
            //notice: compressed Y and Z are swapped

            float x = (compressed.X / 255.0f + offset.X) * scale.X;
            float y = (compressed.Z / 255.0f + offset.Y) * scale.Y;
            float z = (compressed.Y / 255.0f + offset.Z) * scale.Z;

            return new Vector3(x, y, z);
        }

        //intended to do the inverse of CalculateFinalVertex for export puposes, not tested yet
        public static Vector3b CalculateSourceVertex(Vector3 final, Vector3 offset, Vector3 scale)
        {
            byte x = (byte)((final.X / scale.X - offset.X) * 255);
            byte y = (byte)((final.Y / scale.Y - offset.Y) * 255);
            byte z = (byte)((final.Z / scale.Z - offset.Z) * 255);

            //notice the axis swap, not a typo
            return new Vector3b(x, z, y);
        }
    }
}