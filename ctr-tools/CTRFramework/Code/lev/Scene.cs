using CTRFramework.Shared;
using CTRFramework.Vram;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace CTRFramework
{
    public class Scene : IRead, IDisposable
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
        public UnkAdv unkadv;
        public TrialData trial;
        public IconPack iconpack;

        public List<Pose> restartPts = new List<Pose>();

        public Tim ctrvram;

        public Scene()
        {
        }

        public Scene(string filename, Tim vram = null)
        {
            path = filename;
            name = Path.GetFileNameWithoutExtension(filename);

            byte[] data = File.ReadAllBytes(filename);

            using (MemoryStream ms = new MemoryStream(data, 4, data.Length - 4))
            {
                using (BinaryReaderEx br = new BinaryReaderEx(ms))
                {
                    Read(br);
                }
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
                ctrvram = CtrVrm.FromFile(vrmpath);
                LoadTextures();
            }
        }

        public static Scene FromFile(string fn, bool readHi = true)
        {
            Scene.ReadHiTex = readHi;
            return new Scene(fn);
        }

        public List<Vector3s> posu2 = new List<Vector3s>();
        public List<Pose> posu1 = new List<Pose>();

        public void Read(BinaryReaderEx br)
        {
            header = Instance<SceneHeader>.FromReader(br, 0);

            if (header == null)
                throw new Exception("Scene header is null. Halt parsing.");

            if (header.ptrMeshInfo != UIntPtr.Zero)
            {
                mesh = new PtrWrap<MeshInfo>(header.ptrMeshInfo).Get(br);
                quads = mesh.QuadBlocks;
                verts = mesh.Vertices;
                visdata = mesh.VisData;
            }

            restartPts = new PtrWrap<Pose>(header.ptrRestartPts).GetList(br, header.numRestartPts);
            vertanims = new PtrWrap<VertexAnim>(header.ptrVcolAnim).GetList(br, header.numVcolAnim);
            skybox = new PtrWrap<SkyBox>(header.ptrSkybox).Get(br);
            nav = new PtrWrap<Nav>(header.ptrAiNav).Get(br);
            iconpack = new PtrWrap<IconPack>(header.ptrIcons).Get(br);
            trial = new PtrWrap<TrialData>(header.ptrTrialData).Get(br);

            if (header.numSpawnPts != 0)
            {
                br.Jump(header.ptrSpawnPts);
                unkadv = new UnkAdv(br, (int)header.numSpawnPts);
            }

            if (header.cntTrialData != 0)
            {
                br.Jump(header.ptrTrialData);

                int cnt = br.ReadInt32();
                int ptr = br.ReadInt32();

                br.Jump(ptr);

                for (int i = 0; i < cnt; i++)
                    posu1.Add(new Pose(br));
            }


            if (header.cntu2 != 0)
            {
                br.Jump(header.ptru2);

                int cnt = br.ReadInt32();
                int ptr = br.ReadInt32();


                br.Jump(ptr);

                for (int i = 0; i < cnt; i++)
                    posu2.Add(new Vector3s(br));
            }


            //find all water quads in visdata
            foreach (VisData v in visdata)
            {
                if (v.IsLeaf)
                {
                    if (v.flag.HasFlag(VisDataFlags.Water))
                    {
                        int z = (int)((v.ptrQuadBlock - mesh.ptrQuadBlocks.ToUInt32()) / 0x5C);

                        for (int i = z; i < z + v.numQuadBlock; i++)
                            quads[i].isWater = true;
                    }
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
                br.Jump(header.ptrInstancesPtr + 4 * i);
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
                catch
                {
                    Helpers.Panic(this, PanicType.Error, "Unexpected CtrModel crash.");
                }
            }

            foreach (VertexAnim va in vertanims)
            {
                Helpers.Panic(this, PanicType.Info, va.ToString());
            }

            /*
            quads = quads.OrderBy(o => o.mosaicPtr1).ToList();

            StringBuilder sb = new StringBuilder();

            foreach (QuadBlock qb in quads)
            {
                sb.AppendLine(
                    $"{qb.id.ToString("X4")}\t" +
                    $"{(qb.mosaicPtr1 & 0xFFFFFFFC).ToString("X8")} ({Helpers.TestPointer(qb.mosaicPtr1)})\t" +
                    $"{(qb.mosaicPtr2 & 0xFFFFFFFC).ToString("X8")} ({Helpers.TestPointer(qb.mosaicPtr2)})\t" +
                    $"{(qb.mosaicPtr3 & 0xFFFFFFFC).ToString("X8")} ({Helpers.TestPointer(qb.mosaicPtr3)})\t" +
                    $"{(qb.mosaicPtr4 & 0xFFFFFFFC).ToString("X8")} ({Helpers.TestPointer(qb.mosaicPtr4)})"
                    );
            }

            Helpers.WriteToFile(".\\mosaic_test.txt", sb.ToString());

            */
        }

        /// <summary>
        /// Generates OBJ file based on lod selection 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="lod">LOD level (Detail enum)</param>
        private void ExportMesh(string path, Detail lod)
        {
            Helpers.CheckFolder(path);

            if (quads.Count == 0)
                return;


            string fname = $"{name}_{lod.ToString()}";

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("#Converted to OBJ using model_reader, CTR-Tools by DCxDemo*.");
            sb.AppendLine($"#{Meta.GetVersion()}");
            sb.AppendLine("#Original models: (C) 1999, Activision, Naughty Dog.\r\n");
            sb.AppendLine($"mtllib {Path.GetFileName(fname + ".mtl")}\r\n");

            int a = 0;
            int b = 0;

            foreach (QuadBlock g in quads)
                sb.AppendLine(g.ToObj(verts, lod, ref a, ref b));

            Helpers.WriteToFile(Path.Combine(path, $"{fname}.obj"), sb.ToString());

            sb.Clear();


            Dictionary<string, TextureLayout> tex = GetTexturesList(lod);

            string lodpath = Path.Combine(path, "tex" + lod.ToString());
            Helpers.CheckFolder(lodpath);

            foreach (var tl in tex.Values)
            {
                if (tl.Position != 0)
                {
                    string texname = $"tex{lod.ToString()}\\{ tl.Tag()}.png";

                    sb.AppendLine($"newmtl {tl.Tag()}");
                    sb.AppendLine("Kd 2.0 2.0 2.0"); //not sure if it actually works in obj, but it's what psx does
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
                    Helpers.Panic(this, PanicType.Warning, $"tl position == 0? {tl.Tag()}");
                }
            }

            sb.Append("newmtl default\r\n");

            Helpers.WriteToFile(Path.Combine(path, $"{fname}.mtl"), sb.ToString());

            sb.Clear();
        }

        private void ExportModels(string dir)
        {
            dir = Path.Combine(dir, Meta.ModelsPath);

            Helpers.CheckFolder(dir);

            foreach (var model in Models)
            {
                model.Export(dir);
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
            Console.WriteLine($"Exporting to: {path}");

            if (path.Contains(" "))
                Console.WriteLine("Warning, the selected path contains space in its name.\r\nThis may affect material import in certain applications.\r\n");

            if (flags.HasFlag(ExportFlags.MeshLow)) ExportMesh(path, Detail.Low);
            if (flags.HasFlag(ExportFlags.TexLow)) ExportTextures(path, Detail.Low);

            Console.WriteLine("Low mesh: done.");

            if (flags.HasFlag(ExportFlags.MeshMed)) ExportMesh(path, Detail.Med);
            if (flags.HasFlag(ExportFlags.TexMed)) ExportTextures(path, Detail.Med);

            Console.WriteLine("Mid mesh: done.");

            //not implemented, should return subdivided mesh. not sure if needed.
            //if (flags.HasFlag(ExportFlags.MeshHigh)) ExportMesh(path, Detail.High);
            if (flags.HasFlag(ExportFlags.TexHigh)) ExportTextures(path, Detail.High);

            Console.WriteLine("High mesh: done.");

            if (flags.HasFlag(ExportFlags.Models)) ExportModels(path);
            if (flags.HasFlag(ExportFlags.TexModels)) ExportTextures(path, Detail.Models);
            if (flags.HasFlag(ExportFlags.SkyBox)) ExportSkyBox(path);

            Console.WriteLine("Additional models: done.");

            /*
            foreach (QuadBlock qb in quads)
            {
                string x = ".\\textures\\" + qb.id.ToString("X8") + "\\";

                Helpers.CheckFolder(x);

                ctrvram.GetTexture(qb.texlow).Save(x + "low_" + qb.texlow.Tag() + ".png");

                foreach (CtrTex ct in qb.tex)
                {
                    foreach (TextureLayout tl in ct.midlods)
                    {
                        try
                        {
                            ctrvram.GetTexture(tl).Save(x + "med_" + tl.Tag() + ".png");
                        }
                        catch
                        {
                            File.WriteAllText(x + "error.txt", $"error med: quad - {qb.pos.ToString("X8")}, texlayout: {tl.Position.ToString("X8")}\r\n");
                        }
                    }

                    int i = 0;

                    foreach (TextureLayout tl in ct.hi)
                    {
                        try
                        {
                            ctrvram.GetTexture(tl).Save(x + "hi_" + i + "_" + tl.Tag() + ".png");
                        }
                        catch
                        {
                            File.WriteAllText(x + "error.txt", $"error med: quad - {qb.pos.ToString("X8")}, texlayout: {tl.Position.ToString("X8")}\r\n");
                        }

                        i++;
                    }
                }  
            }
            */
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
                Console.WriteLine("No vram found. Make sure to copy vram file along with lev.");
                return;
            }

            // ctrvram.SaveBMP("lol.bmp", BMPHeader.GrayScalePalette(16));

            Console.WriteLine(ctrvram.ToString());
            Console.WriteLine("Exporting textures...");

            path = Path.Combine(path, $"tex{lod.ToString()}");
            Helpers.CheckFolder(path);

            foreach (TextureLayout tl in GetTexturesList(lod).Values)
            {
                Bitmap bmp = ctrvram.GetTexture(tl, path);
                if (bmp == null)
                {
                    Console.WriteLine("Missing texture.");
                    continue;
                }

                Bitmap bb = new Bitmap(bmp.Width, bmp.Height);
                Graphics g = Graphics.FromImage(bb);
                g.DrawImage(bmp, new Point(0, 0));
                bb.Save(Path.Combine(path, $"{tl.Tag()}.png"), System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        public string Info()
        {
            int numLeaves = 0;
            int numBranches = 0;

            foreach (var v in visdata)
            {
                if (v.IsLeaf) numLeaves++;
                if (!v.IsLeaf) numBranches++;
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("{0}: {1}\r\n", "verts", verts.Count);
            sb.AppendFormat("{0}: {1}\r\n", "primitives", quads.Count);
            sb.AppendFormat("{0}: {1}\r\n", "lo quads", quads.Count);
            sb.AppendFormat("{0}: {1}\r\n", "lo tris", quads.Count * 2);
            sb.AppendFormat("{0}: {1}\r\n", "hi quads", quads.Count * 4);
            sb.AppendFormat("{0}: {1}\r\n", "hi tris", quads.Count * 4 * 2);
            sb.AppendFormat("{0}: {1}\r\n", "skybox verts", (skybox != null ? skybox.verts.Count : 0));
            sb.AppendFormat("{0}: {1}\r\n", "visdata total", (visdata != null ? visdata.Count : 0));
            sb.AppendFormat("{0}: {1}\r\n", "visdata leaves", numLeaves);
            sb.AppendFormat("{0}: {1}\r\n", "visdata branches", numBranches);
            sb.AppendLine($"begin: {header.compilationBegins}");
            sb.AppendLine($"end: {header.compilationEnds}");
            sb.AppendLine($"File was compiled in: {Math.Round((header.compilationEnds - header.compilationBegins).TotalMinutes)} minutes");

            return sb.ToString();
        }

        public string Statistics()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("{0},", verts.Count);
            sb.AppendFormat("{0},", quads.Count);
            sb.AppendFormat("{0},", quads.Count);
            sb.AppendFormat("{0},", quads.Count * 2);
            sb.AppendFormat("{0},", quads.Count * 4);
            sb.AppendFormat("{0},", quads.Count * 4 * 2);
            sb.AppendFormat("{0},", (skybox != null ? skybox.verts.Count : 0));
            sb.AppendFormat("{0},", (skybox != null ? skybox.faces.Count : 0));
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
                            if (!tex.ContainsKey(tl.Tag()))
                                tex.Add(tl.Tag(), tl);

                if (iconpack != null)
                    foreach (var i in iconpack.Icons.Values)
                        if (i.tl != null)
                        {
                            if (!tex.ContainsKey(i.tl.Tag()))
                                tex.Add(i.tl.Tag(), i.tl);
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
                                if (!tex.ContainsKey(qb.texlow.Tag()))
                                    tex.Add(qb.texlow.Tag(), qb.texlow);
                            break;

                        case Detail.Med:
                            foreach (CtrTex t in qb.tex)
                                if (t.midlods[2].Position != 0)
                                    if (!tex.ContainsKey(t.midlods[2].Tag()))
                                        tex.Add(t.midlods[2].Tag(), t.midlods[2]);
                            break;

                        case Detail.High:
                            foreach (CtrTex t in qb.tex)
                                foreach (var x in t.hi)
                                    if (x != null)
                                        if (!tex.ContainsKey(x.Tag()))
                                            tex.Add(x.Tag(), x);
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
        private int levelShiftDivide = 92; // one step width

        /// <summary>
        /// Return QuadBlocks associated with the leaf, make sure you pass a leaf and not a branch.
        /// </summary>
        /// <param name="leaf"></param>
        public List<QuadBlock> GetListOfLeafQuadBlocks(VisData leaf)
        {
            List<QuadBlock> leafQuadBlocks = new List<QuadBlock>();

            if (!leaf.IsLeaf)
                return leafQuadBlocks;

            uint ptrQuadBlock = (uint)(((leaf.ptrQuadBlock) / levelShiftDivide) + levelShiftOffset);
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
            unkadv = null;
            restartPts.Clear();
            ctrvram = null;
        }
    }
}