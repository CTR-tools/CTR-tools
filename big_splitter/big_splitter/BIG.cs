using System.Collections.Generic;
using System.IO;

namespace big_splitter
{
    public struct Pair
    {
        public uint size;
        public uint offset;
    }

    class BIG
    {
        public string path;

        BinaryReader br;
        MemoryStream ms;

        public uint totalFiles = 0;

        public List<Pair> pairs = new List<Pair>();

        public BIG(string s)
        {
            path = s;

            ms = new MemoryStream(File.ReadAllBytes(s));
            br = new BinaryReader(ms);

            br.BaseStream.Position += 4;
            totalFiles = br.ReadUInt32();

            for (int i = 0; i < totalFiles; i++)
            {
                Pair x;
                x.offset = br.ReadUInt32();
                x.size = br.ReadUInt32();
                pairs.Add(x);
            }
        }

        ~BIG()
        {
            br.Close();
            ms.Close();
            ms = null;
            br = null;
        }

        public void Export()
        {
            int i = 0;

            Directory.CreateDirectory(Path.GetDirectoryName(path) + "\\BIGFILE\\");

            foreach (Pair p in pairs)
            {
                //detect known formats

                br.BaseStream.Position = p.offset * 2048;

                uint h = br.ReadUInt32();
                uint h2 = br.ReadUInt32();

                string knownext = "";

                switch (h)
                {
                    case 0x00000010: if (h2 == 2) knownext = ".tim"; break;
                    case 0x00000020: knownext = ".tim2x"; break;
                    case 0x80010160: knownext = ".str"; break;
                    //default: knownext = ""; break;
                }

                if (p.size == 0) knownext = ".null";

                //

                br.BaseStream.Position = p.offset * 2048;

                string fname = Path.GetDirectoryName(path) + "\\BIGFILE\\" + i.ToString("00000000") + knownext;
                File.WriteAllBytes(fname, br.ReadBytes((int)p.size));

                i++;
                //Console.WriteLine(fname);
            }
        }
    }
}
