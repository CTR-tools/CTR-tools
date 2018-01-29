using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

namespace model_reader2
{

    class CTRModel
    {


        struct Header
        {
            public uint ptrInfo;
            //???
            //???
            public int numPickupHeaders;
            public uint ptrPickupHeaders;
            public int numPickupModels;
            public uint ptrPickupModelsPtrArray;
            //???
            //???
            public uint ptrPickupHeadersPtrArray;
            public int null1;
            public int null2;

        }


        public string path;

        BinaryReader br;
        MemoryStream ms;

        List<PickupHeader> pickups = new List<PickupHeader>();
        List<PosAng> startGrid = new List<PosAng>();

        public CTRModel(string s)
        {
            path = s;

            ms = new MemoryStream(File.ReadAllBytes(s));
            br = new BinaryReader(ms);

            int size = br.ReadInt32();

            ms = new MemoryStream(br.ReadBytes(size));
            br = new BinaryReader(ms);

            StringBuilder sb = new StringBuilder();

            Header header = new Header();
            header.ptrInfo = br.ReadUInt32();

            br.BaseStream.Position += 8;

            header.numPickupHeaders = br.ReadInt32();
            header.ptrPickupHeaders = br.ReadUInt32();
            header.numPickupModels = br.ReadInt32();
            header.ptrPickupModelsPtrArray = br.ReadUInt32();

            br.BaseStream.Position += 8;

            header.ptrPickupHeadersPtrArray = br.ReadUInt32();
            br.BaseStream.Position += 4;
            header.null1 = br.ReadInt32();
            header.null2 = br.ReadInt32();

            if (header.null1 != 0 || header.null2 != 0)
            {
                Console.WriteLine("WARNING header.null1 = " + header.null1 + "; header.null2 = " + header.null2);
            }



            for (int i = 0; i < header.numPickupHeaders; i++)
            {
                br.BaseStream.Position = header.ptrPickupHeadersPtrArray + 4 * i;
                br.BaseStream.Position = br.ReadUInt32();

                pickups.Add(new PickupHeader(br.ReadBytes(0x40)));
            }



            br.BaseStream.Position = header.ptrInfo;
            br.ReadInt32();
            int vertexnum = br.ReadInt32();
            br.BaseStream.Position += 8;
            uint ptrvertarray = br.ReadUInt32();
            br.ReadInt32();
            uint ptrfacearray = br.ReadUInt32();
            int facenum = br.ReadInt32();
           // while (can)

            br.BaseStream.Position = ptrvertarray;

            for (int i = 0; i < vertexnum; i++)
            {
                int x = br.ReadInt16();
                int y = br.ReadInt16();
                int z = br.ReadInt16();
                br.BaseStream.Position += 2;

                int r = br.ReadByte();
                int g = br.ReadByte();
                int b = br.ReadByte();
                br.BaseStream.Position += 5;

                sb.Append("v " + x + " " + y + " " + z + " " + r + " " +  g + " " + b + "\r\n");
                sb.Append("v " + x + " " + y + " " + z + " " + r + " " + g + " " + b + "\r\n");
                sb.Append("v " + x + " " + y + " " + z + " " + r + " " + g + " " + b + "\r\n");
               // sb.Append("v " + (x + 10) + " " + y + " " + z + " " + r + " " + g + " " + b + "\r\n");
               // sb.Append("v " + x + " " + (y + 1) + " " + z + " " + r + " " + g + " " + b + "\r\n");


                sb.Append("f " + (i * 3 + 1).ToString() + " " + (i * 3 + 2).ToString() + " " + (i * 3 + 3).ToString() + "\r\n");

                /*
                if (i % 3 == 0 && i > 0) 
                    sb.Append("f " + (i * 2 + 1).ToString() + " " + (i * 2 + 2).ToString() + " " + (i * 2 + 3).ToString() + "\r\n");
                 * */
            }


            br.BaseStream.Position = ptrvertarray;
            /*
            for (int i = 0; i < facenum; i++)
            {
                br.BaseStream.Position += 4;

                br.BaseStream.Position += 12;

                int f1 = br.ReadInt16();
                int f2 = br.ReadInt16();
                int f3 = br.ReadInt16();
                //sb.Append("v " + f1 + " " + f2 + " " + f3 + "\r\n");

                br.BaseStream.Position += 12 * 2;
            }

            */

            //this one read riders' starting positions

            br.BaseStream.Position = 0x6C;

            for (int i = 0; i < 8; i++)
            {
                PosAng pa = new PosAng(new Vector3s(br.ReadBytes(6)), new Vector3s(br.ReadBytes(6)));
                startGrid.Add(pa);
            }

            foreach (PosAng pa in startGrid)
            {
                sb.Append(pa.Position.ToObjVertex() + "\r\n");
            }

            string fname = Path.GetFileNameWithoutExtension(path) + ".obj";
            File.WriteAllText(fname, sb.ToString());
        }

        ~CTRModel()
        {
            br.Close();
            ms.Close();

            ms = null;
            br = null;
        }

        public void Export()
        {
            //StringBuilder sb = new StringBuilder();

            //string fname = Path.GetFileNameWithoutExtension(path) + ".txt";
            //File.WriteAllText(fname, sb.ToString());
        }
    }
}
