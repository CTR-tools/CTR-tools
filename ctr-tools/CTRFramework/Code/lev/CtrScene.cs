using CTRFramework.Shared;
using CTRFramework.Vram;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Numerics;
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

        public List<WaterAnim> waterAnim = new List<WaterAnim>();

        public List<RespawnPoint> respawnPts = new List<RespawnPoint>();

        public CtrVrm vram;
        public Tim ctrvram;

        public TextureLayout enviroMap;

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
                Helpers.Panic(this, PanicType.Info, "VRAM found!");
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
            Stopwatch sw = new Stopwatch();

            sw.Start();

            try
            {
                ReadScene(PatchedContainer.FromReader(br).GetReader());
            }
            catch
            {
                //try to load scene without patch table
                ReadScene(br);
            }

            sw.Stop();

            Helpers.Panic(this, PanicType.Measure, $"CtrScene.Read: {sw.ElapsedMilliseconds}");
        }

        public void ReadScene(BinaryReaderEx br)
        {
            header = Instance<SceneHeader>.FromReader(br);

            if (header == null)
                throw new Exception("Scene header is null. Halt parsing.");


            mesh = new PtrWrap<MeshInfo>(header.ptrMeshInfo).Get(br);

            if (mesh != null)
            {
                quads = mesh.QuadBlocks;
                verts = mesh.Vertices;
                visdata = mesh.VisData;

                //foreach (var quad in quads)
                //quad.GenerateCtrQuads(verts);
            }

            //Console.WriteLine((mesh.numVertices * Vertex.SizeOf + mesh.ptrVertices.ToUInt32()).ToString("X8"));
            //Console.ReadKey();

            respawnPts = new PtrWrap<RespawnPoint>(header.ptrRespawnPts).GetList(br, header.numRespawnPts);

            foreach (var respawn in respawnPts)
            {
                respawn.Prev = respawnPts[respawn.prev];
                respawn.Next = respawnPts[respawn.next];
                respawn.Pose.Rotation = Vector3.Transform(-Vector3.UnitZ, Matrix4x4.CreateLookAt(respawn.Pose.Position, respawn.Next.Pose.Position, Vector3.UnitX)) / 180f * (float)Math.PI;
            }

            vertanims = new PtrWrap<VertexAnim>(header.ptrVcolAnim).GetList(br, header.numVcolAnim);
            skybox = new PtrWrap<SkyBox>(header.ptrSkybox).Get(br);
            nav = new PtrWrap<Nav>(header.ptrAiNav).Get(br);
            iconpack = new PtrWrap<IconPack>(header.ptrIcons).Get(br);
            trial = new PtrWrap<TrialData>(header.ptrTrialData).Get(br);
            enviroMap = new PtrWrap<TextureLayout>(header.ptrEnviroMap).Get(br);

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
                    var ctr = CtrModel.FromReader(br);
                    if (ctr != null)
                        Models.Add(ctr);
                }
                catch (Exception ex)
                {
                    Helpers.Panic(this, PanicType.Error, "Unexpected CtrModel crash." + ex.ToString());
                }
            }

            foreach (var va in vertanims)
            {
                Helpers.Panic(this, PanicType.Info, va.ToString());
            }

            br.Jump(header.ptrWater);

            for (int i = 0; i < header.numWater; i++)
            {
                waterAnim.Add(WaterAnim.FromReader(br));
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
            var sb = new StringBuilder();

            var ptrs = new List<uint>();

            foreach (var w in waterAnim)
            {
                ptrs.Add(w.ptrWaterAnim.Address.ToUInt32());
            }

            ptrs.Sort();

            foreach (var x in ptrs)
            {
                Console.WriteLine(x.ToString("X8"));
            }

            //Console.ReadKey();
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

            Helpers.WriteToFile(Helpers.PathCombine(path, $"{fname}.obj"), sb.ToString());

            sb.Clear();


            Dictionary<string, TextureLayout> tex = GetTexturesList(lod);

            string lodpath = Helpers.PathCombine(path, "tex" + lod.ToString());
            Helpers.CheckFolder(lodpath);

            foreach (var tl in tex.Values)
            {
                if (tl.Position != 0)
                {
                    string texname = $"tex{lod}\\{tl.Tag}.png";

                    sb.AppendLine($"newmtl {tl.Tag}");
                    sb.AppendLine($"map_Kd {texname}\r\n");

                    if (!File.Exists(Helpers.PathCombine(path, texname)))
                    {
                        Helpers.Panic(this, PanicType.Warning, "missing bitmap");

                        Bitmap bmp = new Bitmap(1, 1);
                        bmp.Save(Helpers.PathCombine(path, texname));
                    }
                }
                else
                {
                    Helpers.Panic(this, PanicType.Warning, $"tl position == 0? {tl.Tag}");
                }
            }

            /*
             //generates bunch of labeled textures for each byte value
            Helpers.CheckFolder(Helpers.PathCombine(path, "midunk"));

            for (int i = 0; i < 256; i++)
            {
                sb.AppendLine($"newmtl {i.ToString("X2")}");
                sb.AppendLine($"map_Kd midunk\\{i.ToString("X2")}.png\r\n");

                Bitmap bmp = new Bitmap(64, 64);
                Graphics graphics = Graphics.FromImage(bmp);
                graphics.FillRectangle(Brushes.White, new Rectangle(0, 0, 64, 64));
                graphics.DrawString(i.ToString("X2"), new Font("Consolas", 24, FontStyle.Bold), Brushes.Black, new Point(0, 0));
                bmp.Save(Helpers.PathCombine(path, $"midunk\\{i.ToString("X2")}.png"));
            }
            */

            /*
                importers will warn about missing texture here
                but this way it will apply default editor's placeholder texture
                usually checkerboard pattern or magenta filler
            */

            sb.Append("newmtl default\r\n");
            sb.Append("Map_Kd default.png\r\n");

            Helpers.WriteToFile(Helpers.PathCombine(path, $"{fname}.mtl"), sb.ToString());

            sb.Clear();
        }

        private void ExportModels(string dir)
        {
            dir = Helpers.PathCombine(dir, Meta.ModelsPath);

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
                Helpers.WriteToFile(Helpers.PathCombine(path, $"{name}_sky.obj"), skybox.ToObj());
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
                foreach (var tl in GetTexturesList().Values)
                    ctrvram.GetTexture(tl);
            }
        }

        /// <summary>
        /// Exports textures for all levels of detail.
        /// </summary>
        /// <param name="path"></param>
        public void ExportTextures(string path)
        {
            ExportTextures(Helpers.PathCombine(path, "texMed"), Detail.Med);
            ExportTextures(Helpers.PathCombine(path, "texLow"), Detail.Low);
            ExportTextures(Helpers.PathCombine(path, "texHigh"), Detail.High);
            ExportTextures(Helpers.PathCombine(path, "texModels"), Detail.Models);
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

            if (enviroMap != null)
            {
                string p = Helpers.PathCombine(path, "enviroMap.png");
                ctrvram.GetTexture(enviroMap).Save(p);
            }

            Helpers.Panic(this, PanicType.Debug, ctrvram.ToString());

            Helpers.Panic(this, PanicType.Info, "Exporting textures...");

            path = Helpers.PathCombine(path, $"tex{lod}");
            Helpers.CheckFolder(path);

            foreach (var tl in GetTexturesList(lod).Values)
                ctrvram.GetTexture(tl, path)?.Save(Helpers.PathCombine(path, $"{tl.Tag}.png"), System.Drawing.Imaging.ImageFormat.Png);

            if (lod == Detail.High)
                foreach (var quad in quads)
                    foreach (var tex in quad.tex)
                    {
                        if (tex == null)
                        {
                            Helpers.Panic(this, PanicType.Error, $"tex is null for whatever reason... {quad.id}");
                            continue;
                        }

                        try
                        {
                            string file = Helpers.PathCombine(path, $"{tex.lod2.Tag}.png");
                            if (!File.Exists(file))
                                tex.GetHiBitmap(ctrvram, quad)?.Save(file, System.Drawing.Imaging.ImageFormat.Png);
                        }
                        catch (Exception ex)
                        {
                            Helpers.Panic(this, PanicType.Error, $"oh no: {ex.Message}");
                            //Console.ReadKey();
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
            sb.AppendFormat("{0}: {1}\r\n", "vert anims", vertanims.Count);
            sb.AppendFormat("{0}: {1}\r\n", "water anims", waterAnim.Count);
            sb.AppendFormat("{0}: {1}\r\n", "visdata total", (visdata != null ? visdata.Count : 0));
            sb.AppendFormat("{0}: {1}\r\n", "visdata leaves", numLeaves);
            sb.AppendLine($"minblocks in leaf: {minQuads}");
            sb.AppendLine($"maxblocks in leaf: {maxQuads}");

            foreach (var kvp in midunkstats)
            {
                sb.AppendLine($"{kvp.Key.ToString("X2")}: {kvp.Value}");
            }

            sb.AppendLine($"bgmode: {header.someRenderFlags}");
            sb.AppendLine($"color4: {header.color4.ToString("X8")}");

            sb.AppendLine($"begin: {header.compilationBegins}");
            sb.AppendLine($"end: {header.compilationEnds}");
            sb.AppendLine($"File was compiled in: {Math.Round((header.compilationEnds - header.compilationBegins).TotalMinutes)} minutes");

            int maxindex = 0;

            foreach (var qb in quads)
                if (maxindex < qb.trackPos && qb.trackPos != 0xFF)
                    maxindex = qb.trackPos;

            sb.AppendLine($"restarts: length = {respawnPts.Count} maxindex = {maxindex}");

            foreach (var s in respawnPts)
            {
                // sb.AppendLine(""+s.Rotation);
            }

            return sb.ToString();
        }

        public Dictionary<string, TextureLayout> GetTexturesList(Detail lod)
        {
            var result = new Dictionary<string, TextureLayout>();

            if (lod == Detail.Models)
            {
                foreach (var model in Models)
                    foreach (var entry in model.Entries)
                        foreach (var tl in entry.matIndices)
                            if (tl != null)
                                result[tl.Tag] = tl;

                if (iconpack != null)
                    foreach (var i in iconpack.Icons.Values)
                        if (i.tl != null)
                        {
                            result[i.tl.Tag] = i.tl;
                        }
                        else
                        {
                            //hmm
                            Helpers.Panic(this, PanicType.Error, i.Name);
                        }
            }
            else
            {
                foreach (var qb in quads)
                {
                    switch (lod)
                    {
                        case Detail.Low:
                            if (qb.ptrTexLow != UIntPtr.Zero)
                                result[qb.texlow.Tag] = qb.texlow;
                            break;

                        case Detail.Med:
                            foreach (var tex in qb.tex)
                            {
                                if (tex == null) continue;

                                if (tex.lod2.Position != 0)
                                    result[tex.lod2.Tag] = tex.lod2;
                            }
                            break;

                        case Detail.High:
                            foreach (var tex in qb.tex)
                            {
                                if (tex == null) continue;

                                foreach (var x in tex.hi)
                                    if (x != null)
                                        result[x.Tag] = x;
                            }
                            break;
                    }
                }
            }

            return result;
        }


        public Dictionary<string, TextureLayout> GetTexturesList()
        {
            var result = new Dictionary<string, TextureLayout>();

            foreach (var t in GetTexturesList(Detail.Low))
                result[t.Key] = t.Value;

            foreach (var t in GetTexturesList(Detail.Med))
                result[t.Key] = t.Value;

            foreach (var t in GetTexturesList(Detail.High))
                result[t.Key] = t.Value;

            foreach (var t in GetTexturesList(Detail.Models))
                result[t.Key] = t.Value;

            return result;
        }

        /// <summary>
        /// Returns VisData children
        /// </summary>
        /// <param name="node"></param>
        public List<VisData> GetVisDataChildren(VisData node)
        {
            var childVisData = new List<VisData>();

            if (node.leftChild != 0 && !node.IsLeaf) // in the future: handle leaves different. Draw them?
            {
                ushort uLeftChild = (ushort)(node.leftChild & 0x3fff);
                VisData leftChild = visdata.Find(cc => cc.id == uLeftChild);
                childVisData.Add(leftChild);
            }

            if (node.rightChild != 0 && !node.IsLeaf) // in the future: handle leaves different. Draw them?
            {
                ushort uRightChild = (ushort)(node.rightChild & 0x3fff);
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
            var leafQuadBlocks = new List<QuadBlock>();

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

        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable)
        {
            bw.Jump(SceneHeader.SizeOf);

            //write enviro map
            if (enviroMap != null)
            {
                header.ptrEnviroMap.Address = (UIntPtr)bw.BaseStream.Position;
                enviroMap.Write(bw, patchTable);
            }
            else
            {
                header.ptrEnviroMap.Address = UIntPtr.Zero;
            }

            //write respawn table
            foreach (var respawn in respawnPts)
                respawn.Write(bw, patchTable);

            //write instances
            header.ptrInstances.Address = (UIntPtr)bw.BaseStream.Position;

            foreach (var pickup in pickups)
                pickup.Write(bw, patchTable);

            //reserve space for model pointers
            header.ptrModelsPtr.Address = (UIntPtr)bw.BaseStream.Position;

            foreach (var model in Models)
                bw.Write(-1);

            header.ptrMeshInfo.Address = (UIntPtr)bw.BaseStream.Position;

            bw.Seek(MeshInfo.SizeOf);

            //write quadblocks
            mesh.numQuadBlocks = (uint)quads.Count;
            mesh.ptrQuadBlocks = (UIntPtr)bw.BaseStream.Position;

            foreach (var quad in quads)
                quad.Write(bw, patchTable);

            //write vertices
            mesh.numVertices = (uint)vertanims.Count;
            mesh.ptrVertices = (UIntPtr)bw.BaseStream.Position;

            foreach (var vert in verts)
                vert.Write(bw, patchTable);


            //finally jump back and write header

            bw.Jump(0);
            header.Write(bw, patchTable);

            //bw.Jump(header.ptrMeshInfo);
            //mesh.Write(bw, patchTable);
        }

        public void Save(string filename)
        {
            var cont = new PatchedContainer();

            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriterEx(ms))
            {
                Write(bw, cont.PatchTable);
                cont.Data = ms.ToArray();
            }

            cont.Save(filename);
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
            respawnPts.Clear();
            ctrvram = null;
        }
    }
}