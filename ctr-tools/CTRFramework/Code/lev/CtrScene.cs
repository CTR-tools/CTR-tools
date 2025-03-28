﻿using CTRFramework.Models;
using CTRFramework.Shared;
using CTRFramework.Vram;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
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
        public List<CtrModel> Models = new List<CtrModel>();
        public List<CtrInstance> Instances = new List<CtrInstance>();
        public List<VisNode> visdata = new List<VisNode>();
        public List<VisNode> instvisdata = new List<VisNode>();
        public CtrSkyBox skybox;
        public NavData nav;
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

            Helpers.Panic(this, PanicType.Info, $"Loading scene '{name}' from {path}...");

            using (var br = new BinaryReaderEx(File.OpenRead(filename)))
            {
                Read(br);
            }

            Helpers.Panic(this, PanicType.Info, $"Scene loaded.");

            //if vram passed as param
            if (vram != null)
            {
                ctrvram = vram;
                BuildMontageCache();
                return;
            }

            //otherwise try to read from file
            string vrmpath = Path.ChangeExtension(filename, ".vrm");

            if (File.Exists(vrmpath))
            {
                Helpers.Panic(this, PanicType.Info, "VRAM found!");
                //ctrvram = CtrVrm.FromFile(vrmpath);

                SetVram(CtrVrm.FromFile(vrmpath));
            }

            Helpers.Panic(this, PanicType.Warning, "No VRAM found. Can't build montage cache.");
        }

        public void BuildMontageCache()
        {
            foreach (var quad in quads)
                foreach (var tex in quad.tex)
                {
                    if (tex is null)
                    {
                        Helpers.Panic(this, PanicType.Warning, $"tex is null for whatever reason... {quad.id}");
                        continue;
                    }

                    try
                    {
                        //skip if already in cache
                        if (MontageCache.ContainsKey(tex.lod2.Tag)) continue;
                        tex.GetHiBitmap(ctrvram, quad, MontageCache);
                    }
                    catch (Exception ex)
                    {
                        Helpers.Panic(this, PanicType.Error, $"Montage failed for {quad.id}: {ex.Message}.");
                    }
                }
        }

        public void SetVram(CtrVrm c)
        {
            vram = c;
            ctrvram = c.GetVram();
            BuildMontageCache();
            LoadTextures();
        }

        /// <summary>
        /// Factory method, reads CtrScene from LEV file.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="readHi"></param>
        /// <returns></returns>
        public static CtrScene FromFile(string filename, bool readHi = true)
        {
            //get rid of this maybe? not slow anymore.
            CtrScene.ReadHiTex = readHi;

            return new CtrScene(filename);
        }

        public List<Vector3s> posu2 = new List<Vector3s>();

        public void Read(BinaryReaderEx br)
        {
            var sw = new Stopwatch();

            sw.Start();

            try
            {
                //try to load scene with patch container
                ReadScene(PatchedContainer.FromReader(br).GetReader());
            }
            catch (Exception ex)
            {
                Console.WriteLine("failed to read scene with patch container: " + ex.Message + "\r\n" + ex.ToString());

                // but maybe it's a sampler level?
                // extra reset is needed here. it's 0 cause we don't have stacked scenes in a single lev anyways.
                // TODO: unless does it work in bigfile? guess not, gotta test. i believe im creating a substream tho, so it should work
                br.BaseStream.Position = 0;

                // try to load this scene without patch table. just throw if fails too.
                ReadScene(br);
            }

            sw.Stop();

            Helpers.Panic(this, PanicType.Measure, $"CtrScene.Read: {sw.ElapsedMilliseconds}");
        }

        /// <summary>
        /// Main scene parsing function.
        /// </summary>
        /// <param name="br"></param>
        /// <exception cref="Exception"></exception>
        private void ReadScene(BinaryReaderEx br)
        {
            header = SceneHeader.FromReader(br);

            if (header is null)
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


            //if (sceneDebug)
            //    SceneTests();

            //Console.WriteLine((mesh.numVertices * Vertex.SizeOf + mesh.ptrVertices.ToUInt32()).ToString("X8"));
            //Console.ReadKey();


            vertanims = new PtrWrap<VertexAnim>(header.ptrVcolAnim).GetList(br, header.numVcolAnim);
            skybox = new PtrWrap<CtrSkyBox>(header.ptrSkybox).Get(br);
            nav = new PtrWrap<NavData>(header.ptrNavData).Get(br);
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
                        quads[i].visNodeFlags = node.flag;

                    //if (node.flag.HasFlag(VisDataFlags.Unk4))
                    //   throw new Exception("gotcha!");
                }
            }

            //read all instance boxes
            foreach (var node in visdata)
            {
                if (!node.IsLeaf) continue;

                if (node.ptrInstanceNodes == 0) continue;

                Helpers.Panic(this, PanicType.Info, $"hello: {node.ptrInstanceNodes.ToString("X8")}");

                br.Jump(node.ptrInstanceNodes);

                instvisdata.Add(VisNode.FromReader(br));

                int terminator = -1;

                do
                {
                    terminator = br.ReadInt32();

                    if (terminator != 0)
                    {
                        br.Seek(-4);
                        instvisdata.Add(VisNode.FromReader(br));
                    }
                }
                while (terminator != 0);
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

            //read instances
            for (int i = 0; i < header.numInstances; i++)
            {
                br.Jump(header.ptrInstancesPtr.Address + 4 * i);
                br.Jump(br.ReadUInt32());

                Instances.Add(CtrInstance.FromReader(br));
            }

            //read models
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

        }

        bool sceneDebug = true;

        /// <summary>
        /// A debug method for testing purposes, called in ReadScene.
        /// </summary>
        private void SceneTests()
        {
            foreach (var quad in quads)
            {
                foreach (var value in quad.faceNormal)
                {
                    ushort x = (ushort)(value.X * 4096f);
                    ushort y = (ushort)(value.X * 4096f);

                    if (x > 0 && y > 0)

                        Console.WriteLine(
                            x.ToString("X4") + "\t" +
                            y.ToString("X4") + "\t" +
                            quad.midunk.ToString("X2") + "\t" + " shifted: \t" +
                            ((4096 << quad.midunk) / x).ToString("X8") + "\t" +
                            ((4096 << quad.midunk) / y).ToString("X8")
                        );
                }


                Console.WriteLine("---");
                Console.ReadKey();
            }



            return;

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

        #region [Export functions]

        /// <summary>
        /// Generates OBJ file based on lod selection.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="lod">LOD level (Detail enum)</param>
        private void ExportMesh(string path, Detail lod)
        {
            if (quads.Count == 0)
            {
                Helpers.Panic(this, PanicType.Info, "No quads in this scene.");
                return;
            }

            Helpers.CheckFolder(path);

            string fname = $"{name}_{lod.ToString()}";

            var sb = new StringBuilder();

            sb.AppendLine("#Converted to OBJ using model_reader, CTR-Tools by DCxDemo*.");
            sb.AppendLine($"#{Meta.Version}");
            sb.AppendLine("#Original models: (C) 1999, Activision, Naughty Dog.\r\n");
            sb.AppendLine($"mtllib {Path.GetFileName(fname + ".mtl")}\r\n");

            int a = 0;
            int b = 0;

            foreach (var quad in quads)
                sb.AppendLine(quad.ToObj(verts, lod, ref a, ref b));

            if (nav != null)
                sb.AppendLine(nav.ToObj(ref a));

            Helpers.WriteToFile(Helpers.PathCombine(path, $"{fname}.obj"), sb.ToString());

            sb.Clear();


            var tex = GetTexturesList(lod);

            string lodpath = Helpers.PathCombine(path, "tex" + lod.ToString());
            Helpers.CheckFolder(lodpath);

            // in case stuff goes wrong, make an empty 1x1 fallback bitmap
            var dummyBmp = new Bitmap(1, 1);

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

                        dummyBmp.Save(Helpers.PathCombine(path, texname));
                    }
                }
                else
                {
                    Helpers.Panic(this, PanicType.Warning, $"tl position == 0? {tl.Tag}");
                }
            }

            /*
                importers will warn about missing texture here
                but also this way it will apply default editor's placeholder texture
                usually it's a checkerboard pattern (meshlab, unreal) or a magenta filler (blender)

                TODO: actually check if i don't use it for untextured faces already
            */

            sb.Append("newmtl default\r\n");
            sb.Append("Map_Kd default.png\r\n");

            Helpers.WriteToFile(Helpers.PathCombine(path, $"{fname}.mtl"), sb.ToString());

            sb.Clear();
        }

        /// <summary>
        /// Exports all instanced models.
        /// </summary>
        /// <param name="dir"></param>
        private void ExportModels(string dir)
        {
            if (Models.Count == 0) return;

            dir = Helpers.PathCombine(dir, Meta.ModelsPath);

            Helpers.CheckFolder(dir);

            foreach (var model in Models)
            {
                model.Export(dir, ctrvram);
                model.Save(dir);
            }
        }

        /// <summary>
        /// Exports skybox.
        /// </summary>
        /// <param name="path"></param>
        private void ExportSkyBox(string path)
        {
            if (skybox != null)
                Helpers.WriteToFile(Helpers.PathCombine(path, $"{name}_sky.obj"), skybox.ToObj());
        }

        /// <summary>
        /// Main content extraction method. Expects a path and export flags mask. For complete extraction use ExportFlags.All.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="flags"></param>
        public void Export(string path, ExportFlags flags)
        {
            Helpers.Panic(this, PanicType.Info, $"Exporting scene to: {path}");

            Helpers.PanicIf(path.Contains(" "), this, PanicType.Info, "Warning, the selected path contains space in its name.\r\nThis may affect material import in certain applications.\r\n");

            if (flags.HasFlag(ExportFlags.MeshLow)) ExportMesh(path, Detail.Low);
            if (flags.HasFlag(ExportFlags.TexLow)) ExportTextures(path, Detail.Low);

            Helpers.Panic(this, PanicType.Info, "Low mesh: done.");

            if (flags.HasFlag(ExportFlags.MeshMed)) ExportMesh(path, Detail.Med);
            if (flags.HasFlag(ExportFlags.TexMed)) ExportTextures(path, Detail.Med);

            Helpers.Panic(this, PanicType.Info, "Mid mesh: done.");

            //not implemented, should return subdivided mesh. not sure if needed.
            //if (flags.HasFlag(ExportFlags.MeshHigh)) ExportMesh(path, Detail.High);
            if (flags.HasFlag(ExportFlags.TexHigh)) ExportTextures(path, Detail.High);
            if (flags.HasFlag(ExportFlags.TexMontage)) ExportTextures(path, Detail.Montage);

            Helpers.Panic(this, PanicType.Info, "High mesh: done.");

            if (flags.HasFlag(ExportFlags.Models)) ExportModels(path);
            if (flags.HasFlag(ExportFlags.TexModels)) ExportTextures(path, Detail.Models);
            if (flags.HasFlag(ExportFlags.SkyBox)) ExportSkyBox(path);

            if (flags.HasFlag(ExportFlags.AnimTex)) ExportTextures(path, Detail.Anim);


            if (flags.HasFlag(ExportFlags.DumpLayouts)) TextureReplacer.DumpTextureLayoutList(Helpers.PathCombine(path, Meta.LayoutsName), GetTexturesList().Values.ToList());

            Helpers.Panic(this, PanicType.Info, "Additional models: done.");
        }

        public void LoadTextures()
        {
            if (ctrvram is null)
            {
                Helpers.Panic(this, PanicType.Warning, "No VRAM to load textures from.");
                return;
            }

            // here's the trick, Tim class contains a static textures dictionary that is reused later
            // doing this loop we populate that array. gotta come up with a better solution really.

            foreach (var tl in GetTexturesList().Values)
                ctrvram.GetTexture(tl);
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
            ExportTextures(Helpers.PathCombine(path, "texMontage"), Detail.Montage);
        }


        public Dictionary<string, Bitmap> MontageCache = new Dictionary<string, Bitmap>();

        /// <summary>
        /// Exports textures for a specific level of detail, defined by Detail enum.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="lod"></param>
        public void ExportTextures(string path, Detail lod)
        {
            //check if vram is present
            if (ctrvram is null)
            {
                Helpers.Panic(this, PanicType.Info, "Can't export textures as no vram found.\r\nMake sure VRM file is in the same folder.");
                return;
            }

            Helpers.Panic(this, PanicType.Info, $"Exporting textures... lod: {lod}");

            Helpers.Panic(this, PanicType.Debug, $"vram: {ctrvram}");

            //this will be overwritten 5 times for each lod call, should move somewhere else 
            if (enviroMap != null)
            {
                string p = Helpers.PathCombine(path, "enviroMap.png");
                ctrvram.GetTexture(enviroMap).Save(p);
            }

            var textures = GetTexturesList(lod).Values;

            if (textures.Count > 0)
            {
                //check lod path existence
                path = Helpers.PathCombine(path, $"tex{lod}");
                Helpers.CheckFolder(path);

                //export current lod textures
                foreach (var layout in textures)
                    ctrvram.GetTexture(layout, path)?.Save(Helpers.PathCombine(path, $"{layout.Tag}.png"), System.Drawing.Imaging.ImageFormat.Png);
            }

            if (lod == Detail.Anim)
            {
                var animPath = Helpers.PathCombine(path, "animTex");
                //hardcoded animated
                Helpers.CheckFolder(animPath);

                foreach (var quad in quads)
                {
                    if (quad is null) continue;

                    foreach (var tex in quad.tex)
                    {
                        if (tex is null) continue;

                        if (tex.animframes.Count > 0)
                            foreach (var anim in tex.animframes)
                                //must implement cached export here too
                                if (!File.Exists($"{animPath}\\{anim.Tag}.png"))
                                    ctrvram.GetTexture(anim)?.Save($"{animPath}\\{anim.Tag}.png");
                    }
                }
            }


            //special case for tageing textures
            if (lod == Detail.Montage)
            {
                path = Helpers.PathCombine(path, $"tex{lod}");
                Helpers.CheckFolder(path);

                foreach (var bmp in MontageCache)
                    bmp.Value.Save(Helpers.PathCombine(path, $"{bmp.Key}.png"));
            }

            Helpers.Panic(this, PanicType.Info, "...requested LoD export done.");
        }

        #endregion

        /// <summary>
        /// Returns tagged list of TextureLayouts used in the scene for a specific LoD level.
        /// </summary>
        /// <param name="lod"></param>
        /// <returns></returns>
        public Dictionary<string, TextureLayout> GetTexturesList(Detail lod)
        {
            var result = new Dictionary<string, TextureLayout>();

            if (lod == Detail.Models)
            {
                foreach (var model in Models)
                    foreach (var mesh in model)
                        foreach (var tl in mesh.groupedtl)
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
                                if (tex is null) continue;

                                if (tex.lod2.Position != 0)
                                    result[tex.lod2.Tag] = tex.lod2;
                            }
                            break;

                        case Detail.High:
                            foreach (var tex in qb.tex)
                            {
                                if (tex is null) continue;

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

        /// <summary>
        /// Returns tagged list of TextureLayouts used in the scene for all LoD levels.
        /// Every scene texture must be here for texture replacer to work with.
        /// </summary>
        /// <returns></returns>
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
        /// Returns VisNode children (move to VisNode class?)
        /// </summary>
        /// <param name="node"></param>
        public List<VisNode> GetVisNodeChildren(VisNode node)
        {
            var childVisData = new List<VisNode>();

            if (node.leftChild != 0 && !node.IsLeaf) // in the future: handle leaves different. Draw them?
            {
                ushort uLeftChild = (ushort)(node.leftChild & 0x3fff);
                VisNode leftChild = visdata.Find(cc => cc.id == uLeftChild);
                childVisData.Add(leftChild);
            }

            if (node.rightChild != 0 && !node.IsLeaf) // in the future: handle leaves different. Draw them?
            {
                ushort uRightChild = (ushort)(node.rightChild & 0x3fff);
                VisNode rightChild = visdata.Find(cc => cc.id == uRightChild);
                childVisData.Add(rightChild);
            }

            return childVisData;
        }

        private int levelShiftOffset = -52; // offset (found in Unity)

        /// <summary>
        /// Return QuadBlocks associated with the leaf, make sure you pass a leaf and not a branch.
        /// </summary>
        /// <param name="leaf"></param>
        public List<QuadBlock> GetListOfLeafQuadBlocks(VisNode leaf)
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


        /// <summary>
        /// Writes current scene to BinaryReader.
        /// </summary>
        /// <param name="bw"></param>
        /// <param name="patchTable"></param>
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

            foreach (var pickup in Instances)
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

        /// <summary>
        /// Saves current scene as a LEV file.
        /// </summary>
        /// <param name="filename"></param>
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

            var sb = new StringBuilder();

            sb.AppendLine($"sceneflags: {header.sceneFlags}");
            sb.AppendLine($"{header.ptrSkybox}");
            sb.AppendLine($"{header.backColor}");
            sb.AppendLine($"{header.bgColorTop}");
            sb.AppendLine($"{header.bgColorBottom}");
            sb.AppendLine($"{header.gradColorTop}");
            sb.AppendLine($"{header.gradColorBottom}");

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

            sb.AppendLine($"bgmode: {header.sceneFlags}");
            sb.AppendLine($"color4: {header.gradColorBottom}");

            sb.AppendLine($"begin: {header.compilationBegins}");
            sb.AppendLine($"end: {header.compilationEnds}");
            sb.AppendLine($"File was compiled in: {Math.Round((header.compilationEnds - header.compilationBegins).TotalMinutes)} minutes");

            int maxindex = 0;

            foreach (var qb in quads)
                if (maxindex < qb.trackPos && qb.trackPos != 0xFF)
                    maxindex = qb.trackPos;

            sb.AppendLine($"restarts: length = {respawnPts.Count} maxindex = {maxindex}");

            foreach (var model in Models)
            {
                sb.AppendLine("" + model.ToString());
            }

            return sb.ToString();
        }


        public void Dispose()
        {
            header = null;
            mesh = null;
            verts.Clear();
            vertanims.Clear();
            quads.Clear();
            Instances.Clear();
            visdata.Clear();
            Models.Clear();
            skybox = null;
            nav = null;
            spawnGroups = null;
            respawnPts.Clear();
            ctrvram = null;
        }


        public void ApplyLightMap(string filename)
        {
            var bitmap = (Bitmap)Bitmap.FromFile(filename);
            var graph = Graphics.FromImage(bitmap);


            //  var test = new Bitmap(bitmap.Width, bitmap.Height);
            //  var graph2 = Graphics.FromImage(test);


            var min = Vector3.Zero;
            var max = Vector3.Zero;


            foreach (var v in verts)
            {
                if (v.Position.X < min.X) min.X = v.Position.X;
                if (v.Position.X > max.X) max.X = v.Position.X;
                if (v.Position.Y < min.Y) min.Y = v.Position.Y;
                if (v.Position.Y > max.Y) max.Y = v.Position.Y;
                if (v.Position.Z < min.Z) min.Z = v.Position.Z;
                if (v.Position.Z > max.Z) max.Z = v.Position.Z;
            }


            // shift it to world origin
            min = -min;
            max += min;

            //graph2.DrawImage(bitmap, Point.Empty);

            foreach (var vertex in verts)
            {
                //offset by negative, divide by max to 0-1 range, multiple by widthxheight for bitmap coords
                var mapped = (vertex.Position + min) / max * new Vector3(bitmap.Width - 1, 0, bitmap.Height - 1);

                var color = bitmap.GetPixel((int)mapped.X, (int)mapped.Z);

                vertex.Color = new Vector4b(color);
                vertex.MorphColor = vertex.Color;

                // test.SetPixel((int)mapped.X, (int)mapped.Z, Color.Red);
            }
        }

        public void FromObj(OBJ obj)
        {

        }
    }
}