using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Collections;
using System.Security.Cryptography;

namespace bigtool
{
    public struct Pair
    {
        public uint size;
        public uint offset;

        public Pair(uint o, uint s)
        {
            offset = o;
            size = s;
        }
    }

    class BIG
    {
        public string path;

        BinaryReader br;
        MemoryStream ms;

        public uint totalFiles = 0;

        public List<Pair> pairs = new List<Pair>();

        Dictionary<int, string> names = new Dictionary<int, string>();

        List<CTRFile> ctrfiles = new List<CTRFile>();


        static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }


        public enum BIGType
        {
            Unknown, 
            USA, 
            EUR, 
            JAP, 
            Review,
            USDemo,
            EuroDemo
        }

        public string NameBig(BIGType big)
        {
            switch (big)
            {
                case BIGType.USA: return "USA NTSC-U detected!";
                case BIGType.EUR: return "EUR PAL detected!";
                case BIGType.JAP: return "JAP NTSC-J detected!";
                case BIGType.Review: return "Review copy detected!";
                case BIGType.USDemo: return "US Demo detected!";
                case BIGType.EuroDemo: return "Euro Demo detected!"; 
                default: return "Unknown BIGFILE.BIG";
            }
        }

        public BIGType DetectBig(string md5)
        {
            switch (md5)
            {
                case "c43eba5a20f0b4fc69a00c8d61a8ec10": return BIGType.USA;
                case "03a005e2abc6022fd1e1e7405300ad77": return BIGType.EUR;
                case "b22c894eff31539de853c83cf52a9025": return BIGType.JAP; ;
                case "e73f0f3cade06dc5fc2719fe186cbe26": return BIGType.Review;
                case "eb0b4551f3a4a374080c085dea1a8609": return BIGType.USDemo;
                case "870780bddb386d04882d57526a03c966": return BIGType.EuroDemo;
                default: return BIGType.Unknown; ;
            }
        }


        public BIG()
        {
        }


        public uint TotalSize()
        {
            //let's hardcode it as you can't add files anyway.
            //ideally you should calculate the amount of sectors used for size/offset array
            uint to_alloc = 2048 * 3;

            foreach (CTRFile c in ctrfiles)
                to_alloc += c.padded_size;

            return to_alloc;
        }

        public void Build(string txt)
        {
            string path = Path.GetFileNameWithoutExtension(txt);
            string[] files = File.ReadAllLines(txt);

            for (int i = 0; i < files.Length; i++)
            {
                files[i] = ".\\" + path + "\\" + files[i];
                ctrfiles.Add(new CTRFile(files[i]));
            }

            Console.WriteLine("we'll need " + TotalSize() + " Bytes for " + ctrfiles.Count + " files:");

            byte[] final_big = new byte[TotalSize()];
            MemoryStream ms = new MemoryStream(final_big);
            BinaryWriter bw = new BinaryWriter(ms);

            bw.BaseStream.Position = 4;
            bw.Write(ctrfiles.Count);

            bw.BaseStream.Position = 3 * 2048;

            foreach (CTRFile c in ctrfiles)
            {
                Console.Write(".");

                uint pos = (uint)bw.BaseStream.Position;
                c.offset = pos / 2048;

                bw.Write(c.data);

                bw.BaseStream.Position = pos + c.padded_size;
            }

            Console.WriteLine();

            bw.BaseStream.Position = 8;

            foreach (CTRFile c in ctrfiles)
            {
                bw.Write(c.offset);
                bw.Write(c.size);
            }

            Console.WriteLine("Dumping to disk...");

            File.WriteAllBytes(path + ".BIG", final_big);

            Console.WriteLine("BIG file created.");

        }

        public BIG(string s)
        {
            Console.WriteLine("Begin: " + s);
            Console.WriteLine("Calculating md5...");

            string md5 = CalculateMD5(s);
            BIGType reg = DetectBig(md5);

            Console.WriteLine("md5 = " + md5);
            Console.WriteLine(NameBig(reg) + "\r\n");


            if (reg == BIGType.Unknown)
                File.WriteAllText("unknown_md5.txt", md5);


            string[] buf = File.ReadAllLines(System.AppDomain.CurrentDomain.BaseDirectory+"filenames.txt");

            foreach (string b in buf)
            {
                string[] bb = b.Split('=');
                names.Add(int.Parse(bb[0]), bb[1]);
            }


            path = s;

            ms = new MemoryStream(File.ReadAllBytes(s));
            br = new BinaryReader(ms);



            br.BaseStream.Position += 4;
            totalFiles = br.ReadUInt32();

            for (int i = 0; i < totalFiles; i++)
                pairs.Add(new Pair(br.ReadUInt32(), br.ReadUInt32()));
        }

        ~BIG()
        {
            if (br != null)
            {
                br.Close();
                br = null;
            }

            if (ms != null)
            {
                ms.Close();
                ms = null;
            }
        }

        StringBuilder filelist = new StringBuilder();

        public void Export()
        {
            int i = 0;

            Directory.CreateDirectory(Path.GetDirectoryName(path) + "\\BIGFILE\\");
            Console.Write(pairs.Count + " files:\r\n");

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
                    case 0x00000020: knownext = ".vram"; break;
                    case 0x80010160: knownext = ".str"; break;
                    //default: knownext = ""; break;
                }

                if (p.size == 0) knownext = ".null";

                //--------------

                br.BaseStream.Position = p.offset * 2048;

                string knownname = "";

                if (names.ContainsKey(i))
                    knownname = names[i];

                string fname = 
                    Path.GetDirectoryName(path) + 
                    "\\BIGFILE\\" + 
                    i.ToString("0000") + 
                    (knownname !="" ? ("_" + knownname) : "") + 
                    knownext;

                filelist.Append(i.ToString("0000") +
                    (knownname != "" ? ("_" + knownname) : "") +
                    knownext + "\r\n");

                File.WriteAllBytes(fname, br.ReadBytes((int)p.size));

                Console.Write(".");

                i++;
                //Console.WriteLine(fname);
            }

            File.WriteAllText("bigfile.txt", filelist.ToString());

            Console.Write("\r\n");

        }
    }
}
