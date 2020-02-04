using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CTRFramework
{
    public class Scene : IRead
    {
        public string path;

        public SceneHeader header;
        public MeshInfo meshinfo;

        public List<Vertex> verts = new List<Vertex>();
        public List<VertexAnim> vertanims = new List<VertexAnim>();
        public List<QuadBlock> quads = new List<QuadBlock>();
        public List<PickupHeader> pickups = new List<PickupHeader>();
        public List<VisData> coldata = new List<VisData>();
        public List<LODModel> dynamics = new List<LODModel>(); 
        public SkyBox skybox;
        public Nav nav;

        public List<PosAng> restartPts = new List<PosAng>();

        public Tim ctrvram;

        public Scene(string s, string fmtm)
        {
            path = s;

            MemoryStream ms = new MemoryStream(File.ReadAllBytes(s));
            BinaryReaderEx br = new BinaryReaderEx(ms);

            int size = br.ReadInt32();

            //this is for files with external offs file
            if ((size & 0xFF) == 0)
            {
                ms = new MemoryStream(br.ReadBytes(size));
                br = new BinaryReaderEx(ms);
            }
            else
            {
                ms = new MemoryStream(br.ReadBytes((int)br.BaseStream.Length - 4));
                br = new BinaryReaderEx(ms);
            }

            Read(br);

            string vrmpath = Path.ChangeExtension(s, ".vram");

            if (File.Exists(vrmpath))
            {
                Console.WriteLine("VRAM found!");

                using (BinaryReaderEx brr = new BinaryReaderEx(File.OpenRead(vrmpath)))
                {
                    ctrvram = CtrVrm.FromReader(brr);
                }
            }


            /*
            List<uint> offs = new List<uint>();

            foreach (QuadBlock qb in quad)
            {
                foreach (uint u in qb.tex)
                {
                    if (u > 0 && !offs.Contains(u)) offs.Add(u);
                }
            }

            offs.Sort();

            foreach (uint u in offs)
            {
                Console.WriteLine(u.ToString("X8"));
                Console.Read();
            }

            */

        }


        public void ExportTextures(string path)
        {
            if (ctrvram != null)
            {
                ctrvram.SaveBMP("lol.bmp", BMPHeader.GrayScalePalette(16));

                /*
                foreach (QuadBlock qb in quads)
                    foreach(CtrTex ct in qb.tex)
                    {
                        foreach(TextureLayout tl in ct.hi)
                        {
                            ctrvram.GetTexture(tl, "tex_hi", ct.ptrHi.ToString("X8") + "_"+ tl.Tag());
                        }
                    }
                 */

                Console.WriteLine(ctrvram.ToString());
                Console.WriteLine("Exporting textures...");

                Directory.CreateDirectory(path);

                Dictionary<string, TextureLayout> tex = GetTexturesList();

                foreach (TextureLayout tl in tex.Values)
                {
                    try
                    {
                        ctrvram.GetTexture(tl, path);
                        //ctrvram.GetTexturePage(tl, path);
                    }
                    catch (Exception ex)
                    {
                        Helpers.Panic(this, "error " + tl.Tag() + "\r\n" + ex.ToString() + "\r\n\r\n" + tl.ToString());
                        Console.ReadKey();
                    }
                }
            }
            else
            {
                Console.WriteLine("null vram");
            }
        }



        public string Export(string fmt, Detail d, bool exportTextures)
        {
            StringBuilder sb = new StringBuilder();

            string fname = Path.ChangeExtension(path, d.ToString() + "." + fmt);
            string skyname = Path.ChangeExtension(path, ".sky." + fmt);
            string mtllib = Path.ChangeExtension(path, ".mtl");

            if (path.Contains(" "))
                Console.WriteLine("warning, there are spaces in the path. this may affect material import.");

            Console.WriteLine("Exporting to: " + fname);

            switch (fmt)
            {
                case "obj":

                    sb.Append("#Converted to OBJ using model_reader, CTR-Tools by DCxDemo*.\r\n");
                    sb.Append("#(C) 1999, Activision, Naughty Dog.\r\n\r\n");

                    sb.Append("mtllib " + Path.GetFileName(mtllib) + "\r\n\r\n");

                    int a = 0;
                    int b = 0;

                    foreach (QuadBlock g in quads)
                    {
                        sb.AppendLine(g.ToObj(verts, d, ref a, ref b));
                        //a += 9;
                        //b += (d == Detail.Low ? 4 : 16); 
                    }

                    break;

                case "ply":
                    Console.WriteLine("ply format is yet to be reimplemented");
                    break;

                default:
                    Console.WriteLine("Unknown export format.");
                    break;

            }

            Helpers.WriteToFile(fname, sb.ToString());
            Helpers.WriteToFile(skyname, skybox.ToObj());

            sb.Clear();


            /*
            List<string> tags = new List<string>();

            foreach(QuadBlock qb in quad)
            {
                foreach(TextureLayout tl in qb.ctrtex)
                {
                    if (!tags.Contains(tl.Tag()))
                    {
                        tags.Add(tl.Tag());
                    }
                }
            }
            */

            Dictionary<string, TextureLayout> tex = GetTexturesList();

            foreach (var s in tex.Values)
            {
                sb.Append(String.Format("newmtl {0}\r\n", s.Tag()));
                sb.Append(String.Format("map_Ka tex\\{0}.png\r\n", s.Tag()));
                sb.Append(String.Format("map_Kd tex\\{0}.png\r\n\r\n", s.Tag()));
            }

            sb.Append("newmtl default\r\n");

            Helpers.WriteToFile(mtllib, sb.ToString());

            if (exportTextures)
            {
                ExportTextures(Path.GetDirectoryName(path) + "\\tex\\");
                Console.WriteLine("Exported!");
            }

            return fname;
        }

        public void Read(BinaryReaderEx br)
        {
            header = Instance<SceneHeader>.ReadFrom(br, 0);
            meshinfo = Instance<MeshInfo>.ReadFrom(br, header.ptrMeshInfo);
            verts = InstanceList<Vertex>.ReadFrom(br, meshinfo.ptrVertexArray, meshinfo.cntVertex);       
            restartPts = InstanceList<PosAng>.ReadFrom(br, header.ptrRestartPts, header.numRestartPts);
            coldata = InstanceList<VisData>.ReadFrom(br, meshinfo.ptrColDataArray, meshinfo.cntColData);
            quads = InstanceList<QuadBlock>.ReadFrom(br, meshinfo.ptrQuadBlockArray, meshinfo.cntQuadBlock);

            //optional stuff
            if (header.ptrSkybox != 0) skybox = Instance<SkyBox>.ReadFrom(br, header.ptrSkybox);
            if (header.ptrVcolAnim != 0) vertanims = InstanceList<VertexAnim>.ReadFrom(br, header.ptrVcolAnim, header.cntVcolAnim);
            if (header.ptrAiNav != 0) nav = Instance<Nav>.ReadFrom(br, header.ptrAiNav);



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

            /*
            StringBuilder sb = new StringBuilder();


            sb.Append("g lol1");

            foreach (ColData cd in coldata)
            {
                sb.Append(String.Format("v {0} {1} {2}\r\n", cd.v1.X, cd.v1.Y, cd.v1.Z));
            }

            sb.Append("g lol2");

            foreach (ColData cd in coldata)
            {
                sb.Append(String.Format("v {0} {1} {2}\r\n", cd.v2.X, cd.v2.Y, cd.v2.Z));
            }

            File.WriteAllText("coldata.obj", sb.ToString());
            */



            //read pickups
            for (int i = 0; i < header.numPickupHeaders; i++)
            {
                br.Jump(header.ptrPickupHeadersPtrArray + 4 * i);
                br.Jump(br.ReadUInt32());

                pickups.Add(new PickupHeader(br));
            }


            //read pickup models
            //starts out right, but anims ruin it

            br.Jump(header.ptrPickupModelsPtr);
            int x = (int)br.BaseStream.Position;

            for (int i = 0; i < header.numPickupModels; i++)
            {
                br.BaseStream.Position = x + 4 * i;
                br.BaseStream.Position = br.ReadUInt32();

                dynamics.Add(new LODModel(br));
            }

            //Console.ReadKey();

            //exports restart points
            /*
            StringBuilder sb = new StringBuilder();

            foreach (PosAng pa in restartPts)
            {
                sb.AppendFormat("v {0}\r\n", pa.Position.ToString(VecFormat.Numbers));
            }

            for (int i = 1; i <= header.numRestartPts; i++)
            {
                sb.AppendFormat("l {0} {1}\r\n", i, (i == header.numRestartPts ? 1 : i + 1));
            }

            //File.WriteAllText("restart_pts.obj", sb.ToString());
            */

            /*
            List<short> uniflag = new List<short>();

            foreach (QuadBlock qb in quad)
            {
                //check unique values here
                if (!uniflag.Contains(qb.midflags[1]))
                    uniflag.Add(qb.midflags[1]);
            }

            uniflag.Sort();

            foreach (byte b in uniflag)
                Console.WriteLine(b);

            */
            /*
            //ai path test
            int xz = 0;
            foreach (AIPath p in nav.paths)
            {
                File.WriteAllText("path"+xz+".obj", p.ToObj());
                xz++;
            }
            */
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
                
                foreach(CtrTex t in qb.tex)
                    foreach (TextureLayout tl in t.midlods)
                    {
                        if (!tex.ContainsKey(tl.Tag()))
                        {
                            tex.Add(tl.Tag(), tl);
                        }
                    }
            }
            
            List<string> tt = new List<string>();
            StringBuilder sb = new StringBuilder();

            foreach (string s in tex.Keys) tt.Add(s);
            tt.Sort();
            foreach (string s in tt) sb.AppendLine(s);

            File.WriteAllText("texture.txt", sb.ToString());

            return tex;
        }

    }
}




/*                   //get back ply later
                   if (fmt == "ply")
                   {
                       sb.Append("ply\r\n");
                       sb.Append("format ascii 1.0\r\n");
                       sb.Append("comment CTR-Tools by DCxDemo*\r\n");
                       sb.Append("comment source file: " + path + "\r\n");
                       sb.Append("element vertex " + vertexnum + "\r\n");
                       sb.Append("property float x\r\n");
                       sb.Append("property float y\r\n");
                       sb.Append("property float z\r\n");
                       sb.Append("property uchar red\r\n");
                       sb.Append("property uchar green\r\n");
                       sb.Append("property uchar blue\r\n");
                       sb.Append("element face " + facesnum * 8 + "\r\n");
                       sb.Append("property list uchar int vertex_indices\r\n");
                       sb.Append("end_header\r\n");
                   }
*/
