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

        public List<Vertex> vert = new List<Vertex>();
        public List<QuadBlock> quad = new List<QuadBlock>();
        public List<PickupHeader> pickups = new List<PickupHeader>();


        public Scene(string s, string fmt)
        {
            path = s;

            MemoryStream ms = new MemoryStream(File.ReadAllBytes(s));
            BinaryReader br = new BinaryReader(ms);

            int size = br.ReadInt32();

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

            WriteToFile(fname, sb.ToString());

            sb.Clear();

            for (int i = 8; i < 16; i++)
                for (int j = 0; j < 2; j++)
                {
                    sb.Append(String.Format("newmtl texpage_{0}_{1}\r\n", i, j));
                    sb.Append(String.Format("map_Ka texpage_{0}_{1}.png\r\n", i, j));
                    sb.Append(String.Format("map_Kd texpage_{0}_{1}.png\r\n\r\n", i, j));
                }

            WriteToFile(mtllib, sb.ToString());

            Console.WriteLine("Done!");

            return fname;
        }


        public void Read(BinaryReader br)
        {
            //read header
            header = new SceneHeader(br);


            //read pickups
            for (int i = 0; i < header.numPickupHeaders; i++)
            {
                br.BaseStream.Position = header.ptrPickupHeadersPtrArray + 4 * i;
                br.BaseStream.Position = br.ReadUInt32();

                pickups.Add(new PickupHeader(br));
            }


            //read meshinfo
            br.BaseStream.Position = header.ptrMeshInfo;
            meshinfo = new MeshInfo(br);


            //read vertices
            br.BaseStream.Position = meshinfo.ptrvertarray;

            for (int i = 0; i < meshinfo.vertexnum; i++)
            {
                Vertex vt = new Vertex(br);
                vert.Add(vt);
            }


            //read faces
            br.BaseStream.Position = meshinfo.ptrNgonArray;

            List<short> uniflag = new List<short>();

            for (int i = 0; i < meshinfo.facesnum; i++)
            {
                QuadBlock qb = new QuadBlock(br);
                quad.Add(qb);

                //check unique values here
                if (!uniflag.Contains(qb.unk2[3]))
                    uniflag.Add(qb.unk2[3]);

            }

            uniflag.Sort();

            foreach (byte b in uniflag)
                Console.WriteLine(b);
        }


        //avoids excessive fragmentation
        private void WriteToFile(string fname, string content)
        {
            using (FileStream fs = new FileStream(fname, FileMode.OpenOrCreate))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    fs.SetLength(content.Length);
                    sw.Write(content);
                }
            }
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