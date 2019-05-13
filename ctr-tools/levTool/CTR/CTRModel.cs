using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

namespace CTRtools
{
    class CTRModel
    {
        CTRHeader header;
        List<Array1> array1 = new List<Array1>();
        List<CTRVertex> vertex = new List<CTRVertex>();
        List<CTRNgon> ngon = new List<CTRNgon>();
        List<PickupHeader> pickups = new List<PickupHeader>();
        List<PosAng> startGrid = new List<PosAng>();


        public CTRModel(BinaryReader brr)
        {
            int size = brr.ReadInt32();

            using (BinaryReader br = new BinaryReader(new MemoryStream(brr.ReadBytes((int)brr.BaseStream.Length - 4))))
            {
                //reading header

                header = new CTRHeader(br);


                //reading pickup headers

                for (int i = 0; i < header.numPickupHeaders; i++)
                {
                    br.BaseStream.Position = header.ptrPickupHeadersPtrArray + 4 * i;
                    br.BaseStream.Position = br.ReadUInt32();

                    pickups.Add(new PickupHeader(br));
                }


                //reading pointer info struct

                br.BaseStream.Position = header.ptrInfo;

                CTRPtrInfo pinfo = new CTRPtrInfo();
                pinfo.Read(br);


                //reading vertices

                br.BaseStream.Position = pinfo.ptrvertarray;

                for (int i = 0; i < pinfo.vertexnum; i++)
                {
                    CTRVertex vert = new CTRVertex(br);
                    vertex.Add(vert);
                }

                //read faces
                br.BaseStream.Position = pinfo.ptrNgonArray;

                for (int i = 0; i < pinfo.facesnum; i++)
                {
                    CTRNgon g = new CTRNgon(br);
                    ngon.Add(g);
                }
            }
        }


        public string ToNgonList()
        {
            StringBuilder sb = new StringBuilder();

            foreach (CTRNgon c in ngon)
            {
                sb.Append(c.ToString() + "\r\n");
            }

            return sb.ToString();
        }


        public string ToOBJ()
        {
            StringBuilder sb = new StringBuilder();

            foreach (CTRVertex v in vertex)
            {
                sb.Append("v ");
               // sb.Append(v.coord.ToString(0.02f) + " ");
                sb.Append(v.color2.ToString(1) + "\r\n");
            }

            int facenum = 0;

            foreach (CTRNgon g in ngon)
            {
                short[] ind = g.ind;

                for (int j = 0; j < 9; j++) ind[j]++;

                sb.Append("o piece_" + facenum.ToString("0000") + "\r\n");
                sb.Append("g piece_" + facenum.ToString("0000") + "\r\n");

                sb.Append(ASCIIFace("f", ind, 5, 4, 0));
                sb.Append(ASCIIFace("f", ind, 4, 5, 6));
                sb.Append(ASCIIFace("f", ind, 6, 1, 4));
                sb.Append(ASCIIFace("f", ind, 1, 6, 7));
                sb.Append(ASCIIFace("f", ind, 2, 6, 5));
                sb.Append(ASCIIFace("f", ind, 6, 2, 8));
                sb.Append(ASCIIFace("f", ind, 8, 7, 6));
                sb.Append(ASCIIFace("f", ind, 7, 8, 3));

                sb.Append("\r\n");

                facenum++;
            }

            return sb.ToString();
            //File.WriteAllText(s, sb.ToString());
        }

        /*
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

            */
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

            /*

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
                    sb.Append(ASCIIFace("3", ind, 5, 4, 0));
                    sb.Append(ASCIIFace("3", ind, 4, 5, 6));
                    sb.Append(ASCIIFace("3", ind, 6, 1, 4));
                    sb.Append(ASCIIFace("3", ind, 1, 6, 7));
                    sb.Append(ASCIIFace("3", ind, 2, 6, 5));
                    sb.Append(ASCIIFace("3", ind, 6, 2, 8));
                    sb.Append(ASCIIFace("3", ind, 8, 7, 6));
                    sb.Append(ASCIIFace("3", ind, 7, 8, 3));
                }
                else
                {
                    sb.Append("o piece_" + i.ToString("0000") + "\r\n");
                    sb.Append("g piece_" + i.ToString("0000") + "\r\n");

                    sb.Append(ASCIIFace("f", ind, 5, 4, 0));
                    sb.Append(ASCIIFace("f", ind, 4, 5, 6));
                    sb.Append(ASCIIFace("f", ind, 6, 1, 4));
                    sb.Append(ASCIIFace("f", ind, 1, 6, 7));
                    sb.Append(ASCIIFace("f", ind, 2, 6, 5));
                    sb.Append(ASCIIFace("f", ind, 6, 2, 8));
                    sb.Append(ASCIIFace("f", ind, 8, 7, 6));
                    sb.Append(ASCIIFace("f", ind, 7, 8, 3));
                }

            }

            string fname = Path.ChangeExtension(path, fmt);
            File.WriteAllText(fname, sb.ToString());

            Console.WriteLine(fname);
        }

        */

        string ASCIIFace(string label, short[] inds, int x, int y, int z)
        {
            return label + " " + inds[x] + " " + inds[y] + " " + inds[z] + "\r\n";
        }

    }
}
