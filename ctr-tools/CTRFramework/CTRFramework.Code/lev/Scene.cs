using CTRFramework.Shared;
using CTRFramework.Vram;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Linq;

namespace CTRFramework
{
    public class Scene : IRead, IDisposable
    {
        public string path;
        public string name;

        public SceneHeader header;
        public MeshInfo meshinfo;

        public List<Vertex> verts = new List<Vertex>();
        public List<VertexAnim> vertanims = new List<VertexAnim>();
        public List<QuadBlock> quads = new List<QuadBlock>();
        public List<PickupHeader> pickups = new List<PickupHeader>();
        public List<VisData> visdata = new List<VisData>();
        public List<CtrModel> dynamics = new List<CtrModel>();
        public SkyBox skybox;
        public Nav nav;

        public List<TexMap> texmaps = new List<TexMap>();

        public UnkAdv unkadv;
        public TrialData trial;

        public List<PosAng> restartPts = new List<PosAng>();

        public Tim ctrvram;

        public static Scene FromFile(string fn)
        {
            return new Scene(fn);
        }


        public Scene(string s)
        {
            path = s;
            name = Path.GetFileNameWithoutExtension(s);

            byte[] data = File.ReadAllBytes(s);

            using (MemoryStream ms = new MemoryStream(data, 4, data.Length - 4))
            {
                using (BinaryReaderEx br = new BinaryReaderEx(ms))
                {
                    Read(br);

                    string vrmpath = Path.ChangeExtension(s, ".vrm");

                    if (File.Exists(vrmpath))
                    {
                        Console.WriteLine("VRAM found!");
                        ctrvram = CtrVrm.FromFile(vrmpath);
                        LoadTextures();
                        /*
                        if (ctrvram != null)
                            foreach (var m in texmaps)
                            {
                                ctrvram.GetTexture(m.tl, ".\\textures\\", m.name);
                                Console.WriteLine(m.name);
                                Console.ReadKey();
                            }
                        */
                    }
                }
            }
        }


        public List<Vector3s> posu2 = new List<Vector3s>();
        public List<PosAng> posu1 = new List<PosAng>();

        public void Read(BinaryReaderEx br)
        {
            //data that seems to be present in every level
            header = Instance<SceneHeader>.FromReader(br, 0);

            meshinfo = Instance<MeshInfo>.FromReader(br, header.ptrMeshInfo);
            verts = InstanceList<Vertex>.FromReader(br, meshinfo.ptrVertexArray, meshinfo.cntVertex);
            restartPts = InstanceList<PosAng>.FromReader(br, header.ptrRestartPts, header.cntRestartPts);
            visdata = InstanceList<VisData>.FromReader(br, meshinfo.ptrVisDataArray, meshinfo.cntColData);
            quads = InstanceList<QuadBlock>.FromReader(br, meshinfo.ptrQuadBlockArray, meshinfo.cntQuadBlock);

            //optional stuff, can be missing
            if (header.ptrSkybox != 0) skybox = Instance<SkyBox>.FromReader(br, header.ptrSkybox);
            if (header.ptrVcolAnim != 0) vertanims = InstanceList<VertexAnim>.FromReader(br, header.ptrVcolAnim, header.cntVcolAnim);
            if (header.ptrAiNav != 0) nav = Instance<Nav>.FromReader(br, header.ptrAiNav);
            if (header.ptrTrialData != 0) trial = Instance<TrialData>.FromReader(br, header.ptrTrialData);

            if (header.cntSpawnPts != 0)
            {
                br.Jump(header.ptrSpawnPts);
                unkadv = new UnkAdv(br, (int)header.cntSpawnPts);
            }


            if (header.cntTrialData != 0)
            {
                br.Jump(header.ptrTrialData);

                int cnt = br.ReadInt32();
                int ptr = br.ReadInt32();

                br.Jump(ptr);

                for (int i = 0; i < cnt; i++)
                    posu1.Add(new PosAng(br));
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


            foreach (VisData v in visdata)
            {
                if (v.IsLeaf)
                {
                    if (v.flag.HasFlag(VisDataFlags.Water))
                    {
                        int z = (int)((v.ptrQuadBlock - meshinfo.ptrQuadBlockArray) / 0x5C);

                        for (int i = z; i < z + v.numQuadBlock; i++)
                            quads[i].isWater = true;
                    }
                }
            }


            /*
            //texture defs
            br.Jump(header.ptrTexArray);
            br.Skip(8);
            int ptrTexList = br.ReadInt32();
            br.Jump(ptrTexList);

            Console.WriteLine(ptrTexList.ToString("X8"));
            Console.ReadKey();

            texmaps = new List<TexMap>();

            TexMap mp;

            do
            {
                mp = new TexMap(br, "none");

                Console.WriteLine(mp.name);
                Console.ReadKey();

                if (mp.name != "")
                {
                    texmaps.Add(mp);
                }

            }
            while (mp.name != "");
            */


            /*
            for (int i = 0; i < visdata.Count; i++)
            {
                if (File.Exists($"visdata_{i}.obj"))
                    File.Delete($"visdata_{i}.obj");

                File.AppendAllText($"visdata_{i}.obj", visdata[i].ToObj());
            }
            */

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
                br.Jump(header.ptrPickupHeadersPtrArray + 4 * i);
                br.Jump(br.ReadUInt32());

                pickups.Add(new PickupHeader(br));
            }

            //read pickup models
            //starts out right, but anims ruin it

            br.Jump(header.ptrModelsPtr);
            int x = (int)br.BaseStream.Position;

            for (int i = 0; i < header.numModels; i++)
            {
                br.BaseStream.Position = x + 4 * i;
                br.BaseStream.Position = br.ReadUInt32();

                dynamics.Add(new CtrModel(br));
            }

            /*
            //check why itdoesn't work in export function
            foreach (var d in dynamics)
            {
                d.Export(".\\models\\");
            }
            */

            /*
            foreach (QuadBlock qb in quads)
            {
                for (int i = 0; i < 4; i++)
                {
                    List<CTRFramework.Vertex> list = qb.GetVertexListq(verts, i);

                    foreach (CTRFramework.Vertex v in list)
                        Console.WriteLine(v.ToString());

                    Console.WriteLine(qb.unk3[i].ToString());

                    /*
                    //requires 4.6
                    System.Numerics.Vector3 a = new Vector3(list[3].coord.X - list[0].coord.X, list[3].coord.Y - list[0].coord.Y, list[3].coord.Z - list[0].coord.Z);
                    System.Numerics.Vector3 b = new Vector3(list[2].coord.X - list[0].coord.X, list[2].coord.Y - list[0].coord.Y, list[2].coord.Z - list[0].coord.Z);

                    Vector3 cross = Vector3.Cross(a, b);

                    Console.WriteLine(cross.Length()); 
                    */

            /*
                }

                Console.WriteLine();
                Console.ReadKey();
            }
            */


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
        }



        public void ExportMesh(string dir, Detail lod)
        {
            Helpers.CheckFolder(dir);

            if (quads.Count > 0)
            {
                string fname = name + "_" + lod.ToString();

                StringBuilder sb = new StringBuilder();

                sb.AppendLine("#Converted to OBJ using model_reader, CTR-Tools by DCxDemo*.");
                sb.AppendLine($"#{Meta.GetVersionInfo()}");
                sb.AppendLine("#Original models: (C) 1999, Activision, Naughty Dog.\r\n");
                sb.AppendLine($"mtllib {Path.GetFileName(fname + ".mtl")}\r\n");

                int a = 0;
                int b = 0;

                foreach (QuadBlock g in quads)
                    sb.AppendLine(g.ToObj(verts, lod, ref a, ref b));
                
                /*
                foreach (VisData v in visdata)
                    sb.AppendLine(v.ToObj(verts, lod, ref a, ref b, quads));
                */

                Helpers.WriteToFile(Path.Combine(dir, fname + ".obj"), sb.ToString());

                sb.Clear();

                Dictionary<string, TextureLayout> tex = GetTexturesList(lod);

                foreach (var s in tex.Values)
                {
                    Helpers.CheckFolder(Path.Combine(dir, ".\\tex" + lod.ToString()));

                    if (s.Position != 0)
                    {
                        sb.Append(String.Format("newmtl {0}\r\n", s.Tag()));
                        sb.Append(String.Format($"map_Kd tex{lod.ToString()}\\{s.Tag()}.png\r\n\r\n"));

                        if (!File.Exists(Path.Combine(dir, "tex" + lod.ToString() + "\\" + s.Tag() + ".png")))
                        {
                            Helpers.Panic(this, "missing bitmap");

                            Bitmap bmp = new Bitmap(1, 1);
                            bmp.Save(Path.Combine(dir, "tex" + lod.ToString() + "\\" + s.Tag() + ".png"));
                        }
                    }
                }

                sb.Append("newmtl default\r\n");
                //sb.Append("map_kd tex\\default.png\r\n");

                Helpers.WriteToFile(Path.Combine(dir, fname + ".mtl"), sb.ToString());

                sb.Clear();
            }
        }

        public void ExportModels(string dir)
        {
            Helpers.CheckFolder(Path.Combine(dir, "\\models"));

            foreach (CtrModel d in dynamics)
            {
                d.Export(Path.Combine(dir, "\\models"));
                d.Write(Path.Combine(dir, "\\models"));
            }
        }

        public void ExportSkyBox(string path)
        {
            if (skybox != null)
                Helpers.WriteToFile(Path.Combine(path, $"{name}_sky.obj"), skybox.ToObj());
        }

        public void Export(string path, ExportFlags flags)
        {
            Console.WriteLine($"Exporting to: {path}");

            if (path.Contains(" "))
                Console.WriteLine("Warning, there are spaces in the path. This may affect material import.");

            if (flags.HasFlag(ExportFlags.MeshLow)) ExportMesh(path, Detail.Low);
            if (flags.HasFlag(ExportFlags.TexLow)) ExportTextures(Path.Combine(path, "texLow"), Detail.Low);

            Console.WriteLine("Low mesh: done.");

            if (flags.HasFlag(ExportFlags.MeshMed)) ExportMesh(path, Detail.Med);
            if (flags.HasFlag(ExportFlags.TexMed)) ExportTextures(Path.Combine(path, "texMed"), Detail.Med);

            Console.WriteLine("Mid mesh: done.");

            //not implemented, should return subdivided mesh
            //if (flags.HasFlag(ExportFlags.MeshMed)) ExportMesh(dir, Detail.High);
            if (flags.HasFlag(ExportFlags.TexHigh)) ExportTextures(Path.Combine(path, "texHigh"), Detail.High);

            Console.WriteLine("High mesh: done.");

            if (flags.HasFlag(ExportFlags.Models)) ExportModels(path);
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
        }

        /// <summary>
        /// Exports textures for a specific level of detail, defined by Detail enum.
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="lod"></param>
        public void ExportTextures(string dir, Detail lod)
        {
            if (ctrvram == null)
            {
                Console.WriteLine("No vram found. Make sure to copy vram file along with lev.");
                return;
            }

            // ctrvram.SaveBMP("lol.bmp", BMPHeader.GrayScalePalette(16));

            Console.WriteLine(ctrvram.ToString());
            Console.WriteLine("Exporting textures...");

            Helpers.CheckFolder(dir);

            foreach (TextureLayout tl in GetTexturesList(lod).Values)
            {
                Bitmap bmp = ctrvram.GetTexture(tl, dir);
                if (bmp == null)
                {
                    Console.WriteLine("Missing texture.");
                    continue;
                }

                Bitmap bb = new Bitmap(bmp.Width, bmp.Height);
                Graphics g = Graphics.FromImage(bb);
                g.DrawImage(bmp, new Point(0, 0));
                bb.Save(Path.Combine(dir, $"{tl.Tag()}.png"), System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        public string Info()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("{0}: {1}\r\n", "verts", verts.Count);
            sb.AppendFormat("{0}: {1}\r\n", "primitives", quads.Count);
            sb.AppendFormat("{0}: {1}\r\n", "lo quads", quads.Count);
            sb.AppendFormat("{0}: {1}\r\n", "lo tris", quads.Count * 2);
            sb.AppendFormat("{0}: {1}\r\n", "hi quads", quads.Count * 4);
            sb.AppendFormat("{0}: {1}\r\n", "hi tris", quads.Count * 4 * 2);
            sb.AppendFormat("{0}: {1}\r\n", "skybox verts", (skybox != null ? skybox.verts.Count : 0));
            sb.AppendFormat("{0}: {1}\r\n", "skybox tris", (skybox != null ? skybox.faces.Count : 0));
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

            foreach (QuadBlock qb in quads)
            {
                switch (lod)
                {
                    case Detail.Low:
                        if (qb.ptrTexLow != 0)
                            if (!tex.ContainsKey(qb.texlow.Tag()))
                                tex.Add(qb.texlow.Tag(), qb.texlow);
                        break;
                    case Detail.Med:
                        foreach (CtrTex t in qb.tex)
                            if (t.midlods[2].Position != 0)
                                if (!tex.ContainsKey(t.midlods[2].Tag()))
                                    tex.Add(t.midlods[2].Tag(), t.midlods[2]);

                        foreach (CtrModel dyn in dynamics)
                            foreach (CtrHeader hdr in dyn.headers)
                                foreach (TextureLayout tl in hdr.tl)
                                    if (!tex.ContainsKey(tl.Tag()))
                                        tex.Add(tl.Tag(), tl);
                        break;

                    case Detail.High:
                        foreach (CtrTex t in qb.tex)
                        {
                            foreach (var x in t.hi)
                            {
                                if (x != null)
                                    if (!tex.ContainsKey(x.Tag()))
                                        tex.Add(x.Tag(), x);
                            }
                        }

                        break;
                }
            }

            return tex;
        }


        public Dictionary<string, TextureLayout> GetTexturesList()
        {
            Dictionary<string, TextureLayout> tex = new Dictionary<string, TextureLayout>();

            foreach (QuadBlock qb in quads)
            {
                if (qb.ptrTexLow != 0)
                {
                    if (!tex.ContainsKey(qb.texlow.Tag()))
                    {
                        tex.Add(qb.texlow.Tag(), qb.texlow);
                    }
                }

                foreach (CtrTex t in qb.tex)
                    foreach (TextureLayout tl in t.midlods)
                    {
                        if (!tex.ContainsKey(tl.Tag()))
                        {
                            tex.Add(tl.Tag(), tl);
                        }
                    }
                
                foreach (CtrTex t in qb.tex)
                {
                    foreach (TextureLayout tl in t.hi)
                    {
                        if (tl != null)
                            if (!tex.ContainsKey(tl.Tag()))
                            {
                                tex.Add(tl.Tag(), tl);
                            }
                    }
                }
                
                foreach (CtrModel dyn in dynamics)
                {
                    foreach(CtrHeader hdr in dyn.headers)
                    {
                        foreach (TextureLayout tl in hdr.tl)
                        {
                            if (!tex.ContainsKey(tl.Tag()))
                            {
                                tex.Add(tl.Tag(), tl);
                            }
                        }
                    }
                }
            }

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
                ushort uLeftChild = (ushort) (visData.leftChild & 0x3fff);
                VisData leftChild = visdata.Find(cc => cc.id == uLeftChild);
                childVisData.Add(leftChild);
            }
            if (visData.rightChild != 0 && !visData.IsLeaf) // in the future: handle leaves different. Draw them?
            {
                ushort uRightChild = (ushort) (visData.rightChild & 0x3fff);
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
            uint ptrQuadBlock = (uint) (((leaf.ptrQuadBlock) / levelShiftDivide) + levelShiftOffset);
            uint numQuadBlock = leaf.numQuadBlock;
            for (int i = 0; i < numQuadBlock; i++)
            {
                long index = ptrQuadBlock + i;
                QuadBlock quad = quads[(int) Math.Min(Math.Max(index, 0), quads.Count - 1)];
                leafQuadBlocks.Add(quad);
            }

            return leafQuadBlocks;
        }

        public void Dispose()
        {
            header = null;
            meshinfo = null;
            verts.Clear();
            vertanims.Clear();
            quads.Clear();
            pickups.Clear();
            visdata.Clear();
            dynamics.Clear();
            skybox = null;
            nav = null;
            unkadv = null;
            restartPts.Clear();
            ctrvram = null;
        }
    }
}