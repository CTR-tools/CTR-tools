using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace big_splitter
{

    public struct Header
    {
        public uint offs1;
        public uint somenum;
        public uint offs2;
        public uint offs3;
        public uint numberNamed;
        public uint offs_named;

        public override string ToString()
        {
            return
                offs1 + "\r\n" +
                somenum + "\r\n" +
                offs2 + "\r\n" +
                offs3 + "\r\n" +
                numberNamed + "\r\n" +
                offs_named + "\r\n";
        }
    }

    public struct ModelHeader
    {
        public string name;
        public uint offsModel;

        //maybe scale
        public short px;
        public short py;
        public short pz;
        public short p0;

        public uint null1;

        public uint unk1;

        //most likely position
        public short ax;
        public short ay;
        public short az;

        //most likely angle
        public short bx;
        public short by;
        public short bz;

        //event type?
        public int unk2;

        public override string ToString()
        {
            return 
                name + 
               // "\t0x"+offsModel.ToString("X8") + 
               // "\t(" + px + ", " + py +  ", " + pz + ") " +
               // "\t" + null1 +
                "\t"+ unk1 + 
                "\t(" + ax + ", " + ay +  ", " + az + ") " +
                "\t(" + bx + ", " + by + ", " + bz + ") " +
                "\t" + unk2
                ;
        }
    }

    class CTRModel
    {
        public string path;

        BinaryReader br;
        MemoryStream ms;

        Header h = new Header();

        List<ModelHeader> mh = new List<ModelHeader>();

        public CTRModel(string s)
        {
            path = s;

            ms = new MemoryStream(File.ReadAllBytes(s));
            br = new BinaryReader(ms);

            h.offs1 = br.ReadUInt32();
            h.somenum = br.ReadUInt32();
            h.offs2 = br.ReadUInt32();
            h.offs3 = br.ReadUInt32();
            h.numberNamed = br.ReadUInt32();
            h.offs_named = br.ReadUInt32();

            br.BaseStream.Position = 4 + h.offs_named;

            for (int i = 0; i < h.numberNamed; i++)
            {
                ModelHeader m = new ModelHeader();

                m.name = System.Text.Encoding.ASCII.GetString(br.ReadBytes(16));
                m.offsModel = br.ReadUInt32();
                m.px = br.ReadInt16();
                m.py = br.ReadInt16();
                m.pz = br.ReadInt16();

                m.p0 = br.ReadInt16();
                m.null1 = br.ReadUInt32();

                m.unk1 = br.ReadUInt32();

                br.BaseStream.Position += 4 * 3;

                m.ax = br.ReadInt16();
                m.ay = br.ReadInt16();
                m.az = br.ReadInt16();
                m.bx = br.ReadInt16();
                m.by = br.ReadInt16();
                m.bz = br.ReadInt16();

                m.unk2 = br.ReadInt32();

                //br.BaseStream.Position += 16 + 16;

                mh.Add(m);
            }
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

            Console.WriteLine(h.ToString());

            foreach(ModelHeader m in mh)
            {
                Console.WriteLine(m.ToString());
            }

            //string fname = Path.GetFileNameWithoutExtension(path) + ".txt";
            //File.WriteAllText(fname, sb.ToString());
        }
    }
}
