using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CTRtools
{
    class CTRnew
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


        enum Parms
        {
            infoHeader = 0,
            unk1 = 1,
            FaceGroupPtr = 2,
            PickupHeaderCnt = 3,
            PickupHeaderPtr = 4
        }

        public CTRnew(string s, string fmt, System.Windows.Forms.TextBox t)
        {
            StringBuilder sb = new StringBuilder();


            sb.Clear();
            path = s;

            ms = new MemoryStream(File.ReadAllBytes(s));
            br = new BinaryReader(ms);
            
            int size = br.ReadInt32();

            ms = new MemoryStream(br.ReadBytes((int)br.BaseStream.Length-4));
            br = new BinaryReader(ms);

            t.Clear();


            List<int> parms = new List<int>();

            for (int i = 0; i < 27; i++)
                parms.Add(br.ReadInt32());


            foreach (int kek in parms)
            {
                sb.Append(kek + "\t\t" + kek.ToString("x8").ToUpper() + "\r\n");
            }



            /*
            br.BaseStream.Position = parms[(int)Parms.FaceGroupPtr];

            List<FaceGroup> fg = new List<FaceGroup>();

            do
            {
                fg.Add(new FaceGroup(br, parms[(int)Parms.FaceGroupPtr]));
            }
            while (fg[fg.Count-1].valid);

            sb.Append(fg.Count + "\r\n");

            foreach (FaceGroup fgg in fg)
            {
                sb.Append(fgg.ToString() + "\r\n");
            }

            */


            br.BaseStream.Position = parms[(int)Parms.PickupHeaderPtr];

            List<PickupHeader> hdrs = new List<PickupHeader>();

            for (int i = 0; i < parms[(int)Parms.PickupHeaderCnt]; i++)
            {
                hdrs.Add(new PickupHeader(br));
            }

            foreach (PickupHeader hdr in hdrs)
                sb.Append(hdr.ToString() + "\r\n");



            List<PosAng> positions = new List<PosAng>();

            br.BaseStream.Position = 0x6C;

            for (int i = 0; i < 8; i++)
            {
                positions.Add(new PosAng(br));
            }

            foreach (PosAng pa in positions)
            {
                sb.Append(pa.ToString() + "\r\n");
            }

            t.Text = sb.ToString();

            /*
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
             * */
        }

        ~CTRnew()
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


        public string ASCIIFace(string label, short[] inds, int x, int y, int z)
        {
            return label + " " + inds[x] + " " + inds[y] + " " + inds[z] + "\r\n";
        }



        public void ToTreeView(TreeView tv)
        {
            TreeNode main = new TreeNode("main");
            TreeNode header = new TreeNode("header");
            TreeNode array1 = new TreeNode("array1");

            main.Nodes.Add(header);
            tv.Nodes.Add(main);
        }

    }
}
