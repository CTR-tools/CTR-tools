using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

namespace model_reader
{
    class CTRModel
    {
        public string path;
        StringBuilder sb = new StringBuilder();

        BinaryReader br;
        MemoryStream ms;

        CTRHeader header;
        List<CTRVertex> vertex = new List<CTRVertex>();
        List<CTRNgon> ngon = new List<CTRNgon>();
        List<PickupHeader> pickups = new List<PickupHeader>();
        List<PosAng> startGrid = new List<PosAng>();


        public CTRModel(string s, string fmt)
        {
            sb.Clear();
            path = s;

            ms = new MemoryStream(File.ReadAllBytes(s));
            br = new BinaryReader(ms);
            
            int size = br.ReadInt32();

            ms = new MemoryStream(br.ReadBytes((int)br.BaseStream.Length-4));
            br = new BinaryReader(ms);
            

            header = new CTRHeader(br);

            for (int i = 0; i < header.numPickupHeaders; i++)
            {
                br.BaseStream.Position = header.ptrPickupHeadersPtrArray + 4 * i;
                br.BaseStream.Position = br.ReadUInt32();

                pickups.Add(new PickupHeader(br));
            }



            br.BaseStream.Position = header.ptrInfo;

            int facesnum = br.ReadInt32();
            int vertexnum = br.ReadInt32();
            br.ReadInt32();  //???
            int ptrNgonArray = br.ReadInt32();
            uint ptrvertarray = br.ReadUInt32();
            br.ReadInt32();  //null?
            uint ptrfacearray = br.ReadUInt32();    //something else
            int facenum = br.ReadInt32();           //something else


            if (fmt == "ply")
            {
                sb.Append("ply\r\n");
                sb.Append("format ascii 1.0\r\n");
                sb.Append("comment CTR-Tools by DCxDemo*\r\n");
                sb.Append("comment source file: " + path +"\r\n");
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


            //read vertices
            br.BaseStream.Position = ptrvertarray;

            for (int i = 0; i < vertexnum; i++)
            {
                CTRVertex vert = new CTRVertex(br);
                vertex.Add(vert);

                if (fmt == "obj")  sb.Append("v ");

                sb.Append(vert.coord.ToString() + " ");
                sb.Append(vert.color2.ToString() + "\r\n");
            }


            /*
            //starting positions
            br.BaseStream.Position = 0x6C;

            for (int i = 0; i < 8; i++)
            {
                 PosAng pa = new PosAng(new Vector3s(br.ReadBytes(6)), new Vector3s(br.ReadBytes(6)));
                    startGrid.Add(pa);
            }

            foreach (PosAng pa in startGrid)
            sb.Append(pa.Position.ToObjVertex() + "\r\n");
            */


            //read faces
            br.BaseStream.Position = ptrNgonArray;

            for (int i = 0; i < facesnum; i++)
            {

                CTRNgon g = new CTRNgon(br);
                ngon.Add(g);

                short[] ind = g.ind;

                if (fmt == "obj")
                    for (int j = 0; j < 9; j++) 
                        ind[j]++; 

                if (fmt == "ply")
                {
                    sb.Append("3 " + ind[6] + " " + ind[4] + " " + ind[0] + "\r\n");
                    sb.Append("3 " + ind[5] + " " + ind[6] + " " + ind[0] + "\r\n");

                    sb.Append("3 " + ind[7] + " " + ind[1] + " " + ind[4] + "\r\n");
                    sb.Append("3 " + ind[6] + " " + ind[7] + " " + ind[4] + "\r\n");

                    sb.Append("3 " + ind[8] + " " + ind[6] + " " + ind[5] + "\r\n");
                    sb.Append("3 " + ind[2] + " " + ind[8] + " " + ind[5] + "\r\n");

                    sb.Append("3 " + ind[3] + " " + ind[7] + " " + ind[6] + "\r\n");
                    sb.Append("3 " + ind[8] + " " + ind[3] + " " + ind[6] + "\r\n");
                }
                else
                {
                    sb.Append("o piece_" + i.ToString("0000") + "\r\n");
                    sb.Append("g piece_" + i.ToString("0000") + "\r\n");

                    sb.Append("f " + ind[6] + " " + ind[4] + " " + ind[0] + "\r\n");
                    sb.Append("f " + ind[5] + " " + ind[6] + " " + ind[0] + "\r\n");

                    sb.Append("f " + ind[7] + " " + ind[1] + " " + ind[4] + "\r\n");
                    sb.Append("f " + ind[6] + " " + ind[7] + " " + ind[4] + "\r\n");

                    sb.Append("f " + ind[8] + " " + ind[6] + " " + ind[5] + "\r\n");
                    sb.Append("f " + ind[2] + " " + ind[8] + " " + ind[5] + "\r\n");

                    sb.Append("f " + ind[3] + " " + ind[7] + " " + ind[6] + "\r\n");
                    sb.Append("f " + ind[8] + " " + ind[3] + " " + ind[6] + "\r\n\r\n");
                }



            }

            string fname = Path.GetFileNameWithoutExtension(path) + "." + fmt;
            File.WriteAllText(fname, sb.ToString());
        }

        ~CTRModel()
        {
            vertex = null;
            ngon = null;

            br.Close();
            ms.Close();

            ms = null;
            br = null;

            GC.Collect();
        }

        public void Export()
        {
            //StringBuilder sb = new StringBuilder();

            //string fname = Path.GetFileNameWithoutExtension(path) + ".txt";
            //File.WriteAllText(fname, sb.ToString());
        }
    }
}
