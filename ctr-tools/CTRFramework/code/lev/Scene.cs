using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CTRFramework.Shared;

namespace CTRFramework
{
    public class Scene : IRead
    {
        public string path;

        public SceneHeader header;
        public MeshInfo meshinfo;

        public List<Vertex> vert = new List<Vertex>();
        public List<QuadBlock> quad = new List<QuadBlock>();
        public List<PickupHeader> pickups = new List<PickupHeader>();
        public List<ColData> coldata = new List<ColData>();
        public List<LODModel> dynamics = new List<LODModel>();
        public SkyBox skybox;

        public List<PosAng> restartPts = new List<PosAng>();

        public Scene(string s, string fmtm, CtrVrm vrm)
        {
            path = s;

            MemoryStream ms = new MemoryStream(File.ReadAllBytes(s));
            BinaryReader br = new BinaryReader(ms);

            int size = br.ReadInt32();

            //this is for files with external offs file
            if ((size & 0xFF) == 0)
            {
                ms = new MemoryStream(br.ReadBytes(size));
                br = new BinaryReader(ms);
            }
            else
            {
                ms = new MemoryStream(br.ReadBytes((int)br.BaseStream.Length-4));
                br = new BinaryReader(ms);
            }

            Read(br);

            if (vrm != null)
            {
                foreach (QuadBlock qb in quad)
                {
                    foreach (TextureLayout tl in qb.ctrtex)
                        vrm.buffer.GetTexturePage(tl);
                }
            }
        }


        public string Export(string fmt)
        {
            string fname = Path.ChangeExtension(path, fmt);
            string mtllib = Path.ChangeExtension(path, ".mtl");
            Console.WriteLine("Exporting to: " + fname);

            StringBuilder sb = new StringBuilder();

            switch (fmt)
            {
                case "obj":

                    sb.Append("#Converted to OBJ using model_reader, CTR-Tools by DCxDemo*.\r\n");
                    sb.Append("#(C) 1999, Activision, Naughty Dog.\r\n\r\n");

                    sb.Append("mtllib " + mtllib + "\r\n\r\n");

                    foreach (QuadBlock g in quad)
                        sb.AppendLine(g.ToObj(vert, Detail.High));

                    break;

                case "ply":
                    Console.WriteLine("ply format is yet to be reimplemented");
                    break;

                default:
                    Console.WriteLine("Unknown export format.");
                    break;

            }

            CTRFramework.Shared.Helpers.WriteToFile(fname, sb.ToString());

            sb.Clear();


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

            foreach (string s in tags)
            {
                sb.Append(String.Format("newmtl {0}\r\n", s));
                sb.Append(String.Format("map_Ka {0}.bmp\r\n", s));
                sb.Append(String.Format("map_Kd {0}.bmp\r\n\r\n", s));
            }

            /*
            for (int i = 8; i < 16; i++)
                for (int j = 0; j < 2; j++)
                {
                    sb.Append(String.Format("newmtl texpage_{0}_{1}\r\n", i, j));
                    sb.Append(String.Format("map_Ka tex\\page_{0}_{1}.bmp\r\n", i, j));
                    sb.Append(String.Format("map_Kd tex\\page_{0}_{1}.bmp\r\n\r\n", i, j));
                }
                */

            CTRFramework.Shared.Helpers.WriteToFile(mtllib, sb.ToString());

            Console.WriteLine("Done!");

            return fname;
        }


        public void Read(BinaryReader br)
        {
            header = Instance<SceneHeader>.ReadFrom(br, 0);   
            meshinfo = Instance<MeshInfo>.ReadFrom(br, (int)header.ptrMeshInfo);   
            skybox = Instance<SkyBox>.ReadFrom(br, (int)header.ptrSkybox);
            vert = InstanceList<Vertex>.ReadFrom(br, (int)meshinfo.ptrVertexArray, meshinfo.cntVertex);
            restartPts = InstanceList<PosAng>.ReadFrom(br, (int)header.ptrRestartPts, (int)header.numRestartPts);
            coldata = InstanceList<ColData>.ReadFrom(br, (int)meshinfo.ptrColDataArray, meshinfo.cntColData);
            quad = InstanceList<QuadBlock>.ReadFrom(br, (int)meshinfo.ptrQuadBlockArray, meshinfo.cntQuadBlock);

            //read pickups
            for (int i = 0; i < header.numPickupHeaders; i++)
            {
                br.BaseStream.Position = header.ptrPickupHeadersPtrArray + 4 * i;
                br.BaseStream.Position = br.ReadUInt32();

                pickups.Add(new PickupHeader(br));
            }


            //read pickup models
            //starts out right, but anims ruin it
            /*
            int x = (int)br.BaseStream.Position;

            for (int i = 0; i < header.numPickupModels; i++)
            {
                br.BaseStream.Position = x + 4 * i;
                br.BaseStream.Position = br.ReadUInt32();

                dynamics.Add(new LODModel(br));
            }
            */

            //exports restart points

            StringBuilder sb = new StringBuilder();

            foreach (PosAng pa in restartPts)
            {
                sb.AppendFormat("v {0}\r\n", pa.Position.ToString(VecFormat.Numbers));
            }

            for (int i = 1; i <= header.numRestartPts; i++)
            {
                sb.AppendFormat("l {0} {1}\r\n", i, (i == header.numRestartPts ? 1 : i + 1));
            }

            File.WriteAllText("restart_pts.obj", sb.ToString());



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