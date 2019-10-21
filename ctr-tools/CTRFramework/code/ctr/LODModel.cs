using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.IO;

namespace CTRFramework
{
    public class LODModel
    {
        public string path;

        BinaryReaderEx br;
        MemoryStream ms;

        public string name;
        public short unk0;
        public short lodCount;
        public int unk1; //always 0x18!

        List<LODHeader> lh = new List<LODHeader>();
        List<LODVertexDef> vdef = new List<LODVertexDef>();

        public int numColors;

        List<CTRAnim> anims = new List<CTRAnim>();


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

        public void Read(BinaryReaderEx br)
        {
            name = System.Text.Encoding.ASCII.GetString(br.ReadBytes(16)).Replace("\0", "");
            unk0 = br.ReadInt16();
            lodCount = br.ReadInt16();
            unk1 = br.ReadInt32();

            if (unk1 != 0x18)
            {
                Console.WriteLine("unk1 == " + unk1);
            }


            Console.WriteLine("Model: " + name);

            //Console.WriteLine(name + "\t" + unk0 + "\t" + lodCount + "\t" + currentoffset);

            for (int i = 0; i < lodCount; i++)
            {
                LODHeader lod = new LODHeader();
                lod.Read(br);
                lh.Add(lod);
            }

            /*
            for (int i = 0; i < lodCount; i++)
            {
                for (int j = 0; j < lh[i].animsCnt; j++)
                {
                    int off = br.ReadInt32();
                    long pos = br.BaseStream.Position;

                    br.BaseStream.Position = off;
                    CTRAnim anim = new CTRAnim(br);
                    anims.Add(anim);

                    br.BaseStream.Position = pos;
                }
            }

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
