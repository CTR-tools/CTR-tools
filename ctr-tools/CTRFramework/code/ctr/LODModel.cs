using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CTRFramework
{
    public class LODModel
    {
        public string path;

        BinaryReaderEx br;
        MemoryStream ms;

        public string name;
        public ushort evt;
        public short numLods;
        public int ptrLodHeads;

        List<LODHeader> lh = new List<LODHeader>();
        //List<LODVertexDef> vdef = new List<LODVertexDef>();

        //public int numColors;


        public LODModel(string s)
        {
            path = s;

            ms = new MemoryStream(File.ReadAllBytes(s));
            br = new BinaryReaderEx(ms);

            int size = br.ReadInt32();

            ms = new MemoryStream(br.ReadBytes((int)br.BaseStream.Length - 4));
            br = new BinaryReaderEx(ms);

            Read(br);

        }

        public LODModel(BinaryReaderEx br)
        {
            Read(br);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(name + ": ");

            foreach(LODHeader head in lh)
                sb.Append(head.name + ", ");

            sb.Append("\r\n");

            return sb.ToString();
        }

        public void Read(BinaryReaderEx br)
        {
            //Console.WriteLine("lodmodel start: " + br.BaseStream.Position.ToString("X8"));

            name = br.ReadStringFixed(16);
            evt = br.ReadUInt16();
            numLods = br.ReadInt16();
            ptrLodHeads = br.ReadInt32();

            /*
            Console.WriteLine("name: " + name);
            Console.WriteLine("evt: " + (CTREvent)evt);
            Console.WriteLine("lodCount: " + numLods);
            Console.WriteLine("ptrLods: " + (ptrLodHeads + 4).ToString("X8"));
            */
            
            for (int i = 0; i < numLods; i++)
            {
                lh.Add(new LODHeader(br));
            }

            //Console.ReadKey();

            /*
            br.BaseStream.Position = 0x28F;

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < 0x136 / 3; i++)
            {
                sb.AppendFormat("v {0} {1} {2}\r\n",
                    br.ReadByte(),
                    br.ReadByte(),
                    br.ReadByte()
                    );
            }

            File.WriteAllText("model.obj", sb.ToString());

            Console.ReadKey();
            */
            /*
            List<CTRAnim> anims = new List<CTRAnim>();

            Console.WriteLine();
            
            for (int i = 0; i < numLods; i++)
            {
                for (int j = 0; j < lh[i].numAnims; j++)
                {
                    int off = br.ReadInt32();
                    long pos = br.BaseStream.Position;

                    br.BaseStream.Position = off;
                    CTRAnim anim = new CTRAnim(br);
                    anims.Add(anim);

                    br.BaseStream.Position = pos;
                }
            }
            
            */
            /*
            numColors = br.ReadInt32();


            Console.WriteLine("some int: " + numColors);

            int x = 0;

            do
            {
                x = br.ReadInt32();

                if (x != -1)
                {
                    LODVertexDef vd = new LODVertexDef(x);
                    vdef.Add(vd);
                }
            }
            while (x != -1);


            */


            /*
            br.BaseStream.Position = lh[0].offsettooffsets;

            br.BaseStream.Position = br.ReadInt32();


            
            long sizeread = (br.BaseStream.Position - lh[0].offsettooffsets) / 4;

            List<LODVertex> lodv = new List<LODVertex>();

            for (int i = 0; i < sizeread; i++)
            {
                LODVertex lv = new LODVertex(br);
                lodv.Add(lv);

                Console.WriteLine(lv.ToOBJ());
            }

            StringBuilder sb = new StringBuilder();

            foreach (LODVertex lv in lodv)
            {
                sb.Append(lv.ToOBJ()+"\r\n");
            }
            */
            //File.WriteAllText("test.obj", sb.ToString());
        }
    }
}
