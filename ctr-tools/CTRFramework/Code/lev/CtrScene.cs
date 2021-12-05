using CTRFramework.Shared;
using CTRFramework.Vram;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace CTRFramework
{
    public class CtrScene : IRead, IDisposable
    {
        public static bool ReadHiTex = true;

        public string path;
        public string name;

        public SceneHeader header;
        public MeshInfo mesh;

        public List<Vertex> verts = new List<Vertex>();
        public List<VertexAnim> vertanims = new List<VertexAnim>();
        public List<QuadBlock> quads = new List<QuadBlock>();
        public List<PickupHeader> pickups = new List<PickupHeader>();
        public List<VisData> visdata = new List<VisData>();
        public List<CtrModel> Models = new List<CtrModel>();
        public SkyBox skybox;
        public Nav nav;
        public SpawnGroup spawnGroups;
        public TrialData trial;
        public IconPack iconpack;

        public List<Pose> restartPts = new List<Pose>();

        public CtrVrm vram;
        public Tim ctrvram;

        public CtrScene()
        {
        }

        public CtrScene(string filename, Tim vram = null)
        {
            path = filename;
            name = Path.GetFileNameWithoutExtension(filename);

            using (var br = new BinaryReaderEx(File.OpenRead(filename)))
            {
                Read(br);
            }

            if (vram != null)
            {
                ctrvram = vram;
                return;
            }

            string vrmpath = Path.ChangeExtension(filename, ".vrm");

            if (File.Exists(vrmpath))
            {
                Console.WriteLine("VRAM found!");
                //ctrvram = CtrVrm.FromFile(vrmpath);

                SetVram(CtrVrm.FromFile(vrmpath));
            }
        }

        public void SetVram(CtrVrm c)
        {
            vram = c;
            ctrvram = c.GetVram();
            LoadTextures();
        }

        public static CtrScene FromFile(string filename, bool readHi = true)
        {
            CtrScene.ReadHiTex = readHi;
            return new CtrScene(filename);
        }

        public List<Vector3s> posu2 = new List<Vector3s>();

        public void Read(BinaryReaderEx br)
        {
            try
            {
                ReadScene(PatchedContainer.FromReader(br).GetReader());
            }
            catch
            {
                //try to load scene without patch table
                ReadScene(br);
            }
        }

        public void ReadScene(BinaryReaderEx br)
        {
            header = Instance<SceneHeader>.FromReader(br);

            if (header == null)
                throw new Exception("Scene header is null. Halt parsing.");

            if (header.ptrMeshInfo != PsxPtr.Zero)
            {
                mesh = new PtrWrap<MeshInfo>(header.ptrMeshInfo).Get(br);
                quads = mesh.QuadBlocks;
                verts = mesh.Vertices;
                visdata = mesh.VisData;

                //foreach (var quad in quads)
                //quad.GenerateCtrQuads(verts);
            }

            restartPts = new PtrWrap<Pose>(header.ptrRestartPts).GetList(br, header.numRestartPts);
            vertanims = new PtrWrap<VertexAnim>(header.ptrVcolAnim).GetList(br, header.numVcolAnim);
            skybox = new PtrWrap<SkyBox>(header.ptrSkybox).Get(br);
            nav = new PtrWrap<Nav>(header.ptrAiNav).Get(br);
            iconpack = new PtrWrap<IconPack>(header.ptrIcons).Get(br);
            trial = new PtrWrap<TrialData>(header.ptrTrialData).Get(br);

            if (header.numSpawnGroups > 0)
            {
                br.Jump(header.ptrSpawnGroups);
                spawnGroups = new SpawnGroup(br, (int)header.numSpawnGroups);
            }


            if (header.cntu2 > 0)
            {
                br.Jump(header.ptru2);

                int cnt = br.ReadInt32();
                int ptr = br.ReadInt32();


                br.Jump(ptr);

                for (int i = 0; i < cnt; i++)
                    posu2.Add(new Vector3s(br));
            }


            //find all water quads in visdata
            foreach (var node in visdata)
            {
                if (node.IsLeaf)
                {
                    int z = (int)((node.ptrQuadBlock - mesh.ptrQuadBlocks.ToUInt32()) / 0x5C);

                    for (int i = z; i < z + node.numQuadBlock; i++)
                        quads[i].visDataFlags = node.flag;

                    //if (node.flag.HasFlag(VisDataFlags.Unk4))
                    //   throw new Exception("gotcha!");
                }
            }

            //assign anim color target to vertex
            foreach (var va in vertanims)
            {
                verts[(int)((va.ptrVertex - mesh.ptrVertices.ToUInt32()) / 16)].color_target = va.color;
            }


            /*
             //water texture
            br.BaseStream.Position = header.ptrWater;

            List<uint> vptr = new List<uint>();
            List<uint> wptr = new List<uint>();

            for (int i = 0; i < header.cntWater; i++)
            {
                vptr.Add(br.ReadUInt32());
                wptr.Add(br.ReadUInt32());
            }

            wptr.Sort();

            foreach(uint u in wptr)
            {
                Console.WriteLine(u.ToString("X8"));
            }

            Console.ReadKey();
            */

            //read pickups
            for (int i = 0; i < header.numInstances; i++)
            {
                br.Jump(header.ptrInstancesPtr.Address + 4 * i);
                br.Jump(br.ReadUInt32());

                pickups.Add(PickupHeader.FromReader(br));
            }


            br.Jump(header.ptrModelsPtr);

            List<uint> modelPtr = br.ReadListUInt32(header.numModels);

            foreach (var ptr in modelPtr)
            {
                br.Jump(ptr);

                try
                {
                    CtrModel ctr = CtrModel.FromReader(br);
                    if (ctr != null)
                        Models.Add(ctr);
                }
                catch (Exception ex)
                {
                    Helpers.Panic(this, PanicType.Error, "Unexpected CtrModel crash." + ex.ToString());
                }
            }

            foreach (VertexAnim va in vertanims)
            {
                Helpers.Panic(this, PanicType.Info, va.ToString());
            }


            if (sceneDebug)
                SceneTests();
        }

        bool sceneDebug = false;

        /// <summary>
        /// A debug method for testing purposes, called in ReadScene.
        /// </summary>
        private void SceneTests()
        {

            StringBuilder sb = new StringBuilder();

            int countadd = 0;
            int countmid = 0;

            int waterleaf = 0;
            int waterbranch = 0;

            foreach (var node in visdata)
            {
                if (!node.IsLeaf && node.flag != 0) waterbranch++;
            }

            Console.WriteLine($"waterleaf={waterleaf} waterbranch={waterbranch}");
            Console.ReadKey();


            foreach (QuadBlock qb in quads)
            {
                if (qb.ptrTexMid[0] == PsxPtr.Zero)
                    countmid++;

                if (qb.ptrAddVis != PsxPtr.Zero)
                    countadd++;

                sb.AppendLine(
                    $"ptr3 data: {qb.mosaicPtr4.GetDifference(qb.mosaicPtr3)} " +
                    $"ptr2 data: {qb.mosaicPtr3.GetDifference(qb.mosaicPtr1)} " +
                    $"ptr1 data: {qb.mosaicPtr1.GetDifference(qb.mosaicPtr2)}");

                /*
                sb.AppendLine(
                    $"{qb.id.ToString("X4")}\t" +
                    $"{(qb.mosaicPtr1.ToUInt32() & 0xFFFFFFFC).ToString("X8")} ({Helpers.TestPointer(qb.mosaicPtr1)})\t" +
                    $"{(qb.mosaicPtr2.ToUInt32() & 0xFFFFFFFC).ToString("X8")} ({Helpers.TestPointer(qb.mosaicPtr2)})\t" +
                    $"{(qb.mosaicPtr3.ToUInt32() & 0xFFFFFFFC).ToString("X8")} ({Helpers.TestPointer(qb.mosaicPtr3)})\t" +
                    $"{(qb.mosaicPtr4.ToUInt32() & 0xFFFFFFFC).ToString("X8")} ({Helpers.TestPointer(qb.mosaicPtr4)})"
                    );
                */

                /*
                int pos = (int)br.BaseStream.Position;

                br.Jump(qb.mosaicPtr4);

                uint ptr4val = br.ReadUInt32();

                sb.AppendLine("ptr4val: " + ptr4val.ToString("X8"));
                

                br.Jump(pos);
                */
            }

            Helpers.WriteToFile(".\\mosaic_test.txt", sb.ToString());

            foreach (var quad in quads)
            {
                //quad.ColTest(verts);
                //Console.ReadKey();
            }
        }

        /// <summary>
        /// Generates OBJ file based on lod selection 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="lod">LOD level (Detail enum)</param>
        private void ExportMesh(string path, Detail lod)
        {
            if (quads.Count == 0)
                return;

            Helpers.CheckFolder(path);

            string fname = $"{name}_{lod.ToString()}";

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("#Converted to OBJ using model_reader, CTR-Tools by DCxDemo*.");
            sb.AppendLine($"#{Meta.GetVersion()}");
            sb.AppendLine("#Original models: (C) 1999, Activision, Naughty Dog.\r\n");
            sb.AppendLine($"mtllib {Path.GetFileName(fname + ".mtl")}\r\n");

            int a = 0;
            int b = 0;

            foreach (var quad in quads)
                sb.AppendLine(quad.ToObj(verts, lod, ref a, ref b));

            if (header.ptrAiNav != PsxPtr.Zero)
                sb.AppendLine(nav.ToObj(ref a));

            Helpers.WriteToFile(Path.Combine(path, $"{fname}.obj"), sb.ToString());

            sb.Clear();


            Dictionary<string, TextureLayout> tex = GetTexturesList(lod);

            string lodpath = Path.Combine(path, "tex" + lod.ToString());
            Helpers.CheckFolder(lodpath);

            foreach (var tl in tex.Values)
            {
                if (tl.Position != 0)
                {
                    string texname = $"tex{lod}\\{tl.Tag}.png";

                    sb.AppendLine($"newmtl {tl.Tag}");
                    sb.AppendLine($"map_Kd {texname}\r\n");

                    if (!File.Exists(Path.Combine(path, texname)))
                    {
                        Helpers.Panic(this, PanicType.Warning, "missing bitmap");

                        Bitmap bmp = new Bitmap(1, 1);
                        bmp.Save(Path.Combine(path, texname));
                    }
                }
                else
                {
                    Helpers.Panic(this, PanicType.Warning, $"tl position == 0? {tl.Tag}");
                }
            }

            /*
             //generates bunch of labeled textures for each byte value
            Helpers.CheckFolder(Path.Combine(path, "midunk"));

            for (int i = 0; i < 256; i++)
            {
                sb.AppendLine($"newmtl {i.ToString("X2")}");
                sb.AppendLine($"map_Kd midunk\\{i.ToString("X2")}.png\r\n");

                Bitmap bmp = new Bitmap(64, 64);
                Graphics graphics = Graphics.FromImage(bmp);
                graphics.FillRectangle(Brushes.White, new Rectangle(0, 0, 64, 64));
                graphics.DrawString(i.ToString("X2"), new Font("Consolas", 24, FontStyle.Bold), Brushes.Black, new Point(0, 0));
                bmp.Save(Path.Combine(path, $"midunk\\{i.ToString("X2")}.png"));
            }
            */

            /*
                importers will warn about missing texture here
                but this way it will apply default editor's placeholder texture
                usually checkerboard pattern or magenta filler
            */

            sb.Append("newmtl default\r\n");
            sb.Append("Map_Kd default.png\r\n");

            Helpers.WriteToFile(Path.Combine(path, $"{fname}.mtl"), sb.ToString());

            sb.Clear();
        }

        private void ExportModels(string dir)
        {
            dir = Path.Combine(dir, Meta.ModelsPath);

            Helpers.CheckFolder(dir);

            foreach (var model in Models)
            {
                model.Export(dir, ctrvram);
                model.Save(dir);
            }
        }

        private void ExportSkyBox(string path)
        {
            if (skybox != null)
                Helpers.WriteToFile(Path.Combine(path, $"{name}_sky.obj"), skybox.ToObj());
        }

        public void Export(string path, ExportFlags flags)
        {
            Helpers.Panic(this, PanicType.Info, $"Exporting scene to: {path}");

            if (path.Contains(" "))
                Helpers.Panic(this, PanicType.Info, "Warning, the selected path contains space in its name.\r\nThis may affect material import in certain applications.\r\n");

            if (flags.HasFlag(ExportFlags.MeshLow)) ExportMesh(path, Detail.Low);
            if (flags.HasFlag(ExportFlags.TexLow)) ExportTextures(path, Detail.Low);

            Helpers.Panic(this, PanicType.Info, "Low mesh: done.");

            if (flags.HasFlag(ExportFlags.MeshMed)) ExportMesh(path, Detail.Med);
            if (flags.HasFlag(ExportFlags.TexMed)) ExportTextures(path, Detail.Med);

            Helpers.Panic(this, PanicType.Info, "Mid mesh: done.");

            //not implemented, should return subdivided mesh. not sure if needed.
            //if (flags.HasFlag(ExportFlags.MeshHigh)) ExportMesh(path, Detail.High);
            if (flags.HasFlag(ExportFlags.TexHigh)) ExportTextures(path, Detail.High);

            Helpers.Panic(this, PanicType.Info, "High mesh: done.");

            if (flags.HasFlag(ExportFlags.Models)) ExportModels(path);
            if (flags.HasFlag(ExportFlags.TexModels)) ExportTextures(path, Detail.Models);
            if (flags.HasFlag(ExportFlags.SkyBox)) ExportSkyBox(path);

            Helpers.Panic(this, PanicType.Info, "Additional models: done.");
        }


        public void LoadTextures()
        {
            if (ctrvram != null)
            {
                foreach (TextureLayout tl in GetTexturesList().Values)
                    ctrvram.GetTexture(tl);
            }
        }

        /// <summary>
        /// Exports textures for all levels of detail.
        /// </summary>
        /// <param name="path"></param>
        public void ExportTextures(string path)
        {
            ExportTextures(Path.Combine(path, "texMed"), Detail.Med);
            ExportTextures(Path.Combine(path, "texLow"), Detail.Low);
            ExportTextures(Path.Combine(path, "texHigh"), Detail.High);
            ExportTextures(Path.Combine(path, "texModels"), Detail.Models);
        }

        /// <summary>
        /// Exports textures for a specific level of detail, defined by Detail enum.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="lod"></param>
        public void ExportTextures(string path, Detail lod)
        {
            if (ctrvram == null)
            {
                Helpers.Panic(this, PanicType.Info, "No export textures as no vram found.\r\nMake sure VRM file is in the same folder.");
                return;
            }

            Helpers.Panic(this, PanicType.Debug, ctrvram.ToString());

            Helpers.Panic(this, PanicType.Info, "Exporting textures...");

            path = Path.Combine(path, $"tex{lod}");
            Helpers.CheckFolder(path);

            foreach (var tl in GetTexturesList(lod).Values)
                ctrvram?.GetTexture(tl, path)?.Save(Path.Combine(path, $"{tl.Tag}.png"), System.Drawing.Imaging.ImageFormat.Png);

            if (lod == Detail.High)
                foreach (var quad in quads)
                    foreach (var tex in quad.tex)
                    {
                        try
                        {
                            string file = Path.Combine(path, $"{tex.lod2.Tag}.png");
                            if (!File.Exists(file))
                                tex.GetHiBitmap(ctrvram, quad)?.Save(file, System.Drawing.Imaging.ImageFormat.Png);
                        }
                        catch
                        {
                            Console.WriteLine("oh no");
                        }
                    }


            Helpers.Panic(this, PanicType.Info, "Textures done.");
        }

        public override string ToString()
        {
            return Info();
        }

        public string Info()
        {
            int numLeaves = 0;
            int numBranches = 0;
            uint maxQuads = 0;
            uint minQuads = 9999;

            foreach (var v in visdata)
            {
                if (v.IsLeaf)
                {
                    numLeaves++;
                    if (v.numQuadBlock > maxQuads) maxQuads = v.numQuadBlock;
                    if (v.numQuadBlock < minQuads) minQuads = v.numQuadBlock;
                }
                if (!v.IsLeaf) numBranches++;
            }

            Dictionary<int, int> midunkstats = new Dictionary<int, int>();

            foreach (var quad in quads)
            {
                if (!midunkstats.ContainsKey(quad.midunk))
                    midunkstats.Add(quad.midunk, 0);

                midunkstats[quad.midunk]++;
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("{0}: {1}\r\n", "verts", verts.Count);
            sb.AppendFormat("{0}: {1}\r\n", "primitives", quads.Count);
            sb.AppendFormat("{0}: {1}\r\n", "lo quads", quads.Count);
            sb.AppendFormat("{0}: {1}\r\n", "lo tris", quads.Count * 2);
            sb.AppendFormat("{0}: {1}\r\n", "hi quads", quads.Count * 4);
            sb.AppendFormat("{0}: {1}\r\n", "hi tris", quads.Count * 4 * 2);
            sb.AppendFormat("{0}: {1}\r\n", "skybox verts", (skybox != null ? skybox.Vertices.Count : 0));
            sb.AppendFormat("{0}: {1}\r\n", "visdata total", (visdata != null ? visdata.Count : 0));
            sb.AppendFormat("{0}: {1}\r\n", "visdata leaves", numLeaves);
            sb.AppendLine($"minblocks in leaf: {minQuads}");
            sb.AppendLine($"maxblocks in leaf: {maxQuads}");

            foreach (var kvp in midunkstats)
            {
                sb.AppendLine($"{kvp.Key.ToString("X2")}: {kvp.Value}");
            }

            sb.AppendLine($"bgmode: {header.bgMode}");
            sb.AppendLine($"color4: {header.color4.ToString("X8")}");

            sb.AppendLine($"begin: {header.compilationBegins}");
            sb.AppendLine($"end: {header.compilationEnds}");
            sb.AppendLine($"File was compiled in: {Math.Round((header.compilationEnds - header.compilationBegins).TotalMinutes)} minutes");

            return sb.ToString();
        }

        public Dictionary<string, TextureLayout> GetTexturesList(Detail lod)
        {
            Dictionary<string, TextureLayout> tex = new Dictionary<string, TextureLayout>();

            if (lod == Detail.Models)
            {
                foreach (var model in Models)
                    foreach (var entry in model.Entries)
                        foreach (TextureLayout tl in entry.tl)
                            if (!tex.ContainsKey(tl.Tag))
                                tex.Add(tl.Tag, tl);

                if (iconpack != null)
                    foreach (var i in iconpack.Icons.Values)
                        if (i.tl != null)
                        {
                            if (!tex.ContainsKey(i.tl.Tag))
                                tex.Add(i.tl.Tag, i.tl);
                        }
                        else
                        {
                            //hmm
                            Helpers.Panic(this, PanicType.Error, i.Name);
                        }
            }
            else
            {
                foreach (QuadBlock qb in quads)
                {
                    switch (lod)
                    {
                        case Detail.Low:
                            if (qb.ptrTexLow != UIntPtr.Zero)
                                if (!tex.ContainsKey(qb.texlow.Tag))
                                    tex.Add(qb.texlow.Tag, qb.texlow);
                            break;

                        case Detail.Med:
                            foreach (CtrTex t in qb.tex)
                                if (t != null)
                                    if (t.lod2.Position != 0)
                                        if (!tex.ContainsKey(t.lod2.Tag))
                                            tex.Add(t.lod2.Tag, t.lod2);
                            break;

                        case Detail.High:
                            foreach (CtrTex t in qb.tex)
                                if (t != null)
                                    foreach (var x in t.hi)
                                        if (x != null)
                                            if (!tex.ContainsKey(x.Tag))
                                                tex.Add(x.Tag, x);
                            break;
                    }
                }
            }

            return tex;
        }


        public Dictionary<string, TextureLayout> GetTexturesList()
        {
            Dictionary<string, TextureLayout> tex = new Dictionary<string, TextureLayout>();

            foreach (var t in GetTexturesList(Detail.Low))
                if (!tex.ContainsKey(t.Key))
                    tex.Add(t.Key, t.Value);

            foreach (var t in GetTexturesList(Detail.Med))
                if (!tex.ContainsKey(t.Key))
                    tex.Add(t.Key, t.Value);

            foreach (var t in GetTexturesList(Detail.High))
                if (!tex.ContainsKey(t.Key))
                    tex.Add(t.Key, t.Value);

            foreach (var t in GetTexturesList(Detail.Models))
                if (!tex.ContainsKey(t.Key))
                    tex.Add(t.Key, t.Value);

            return tex;
        }

        /// <summary>
        /// Returns VisData children
        /// </summary>
        /// <param name="visData"></param>
        public List<VisData> GetVisDataChildren(VisData visData)
        {
            List<VisData> childVisData = new List<VisData>();

            if (visData.leftChild != 0 && !visData.IsLeaf) // in the future: handle leaves different. Draw them?
            {
                ushort uLeftChild = (ushort)(visData.leftChild & 0x3fff);
                VisData leftChild = visdata.Find(cc => cc.id == uLeftChild);
                childVisData.Add(leftChild);
            }

            if (visData.rightChild != 0 && !visData.IsLeaf) // in the future: handle leaves different. Draw them?
            {
                ushort uRightChild = (ushort)(visData.rightChild & 0x3fff);
                VisData rightChild = visdata.Find(cc => cc.id == uRightChild);
                childVisData.Add(rightChild);
            }

            return childVisData;
        }

        private int levelShiftOffset = -52; // offset (found in Unity)

        /// <summary>
        /// Return QuadBlocks associated with the leaf, make sure you pass a leaf and not a branch.
        /// </summary>
        /// <param name="leaf"></param>
        public List<QuadBlock> GetListOfLeafQuadBlocks(VisData leaf)
        {
            List<QuadBlock> leafQuadBlocks = new List<QuadBlock>();

            if (!leaf.IsLeaf)
                return leafQuadBlocks;

            uint ptrQuadBlock = (uint)(((leaf.ptrQuadBlock) / QuadBlock.SizeOf) + levelShiftOffset);
            uint numQuadBlock = leaf.numQuadBlock;
            for (int i = 0; i < numQuadBlock; i++)
            {
                long index = ptrQuadBlock + i;
                QuadBlock quad = quads[(int)Math.Min(Math.Max(index, 0), quads.Count - 1)];
                leafQuadBlocks.Add(quad);
            }

            return leafQuadBlocks;
        }

        public void Dispose()
        {
            header = null;
            mesh = null;
            verts.Clear();
            vertanims.Clear();
            quads.Clear();
            pickups.Clear();
            visdata.Clear();
            Models.Clear();
            skybox = null;
            nav = null;
            spawnGroups = null;
            restartPts.Clear();
            ctrvram = null;
        }
    }
}