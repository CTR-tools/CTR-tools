using CTRFramework;
using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using p = CTRFramework.Properties.Resources;

namespace bigtool
{
    /*
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
    */

    public class BIG
    {
        BinaryReaderEx br;
        MemoryStream ms;

        public int totalFiles = 0;

        public List<BigEntry> pairs = new List<BigEntry>();

        public Dictionary<int, string> names = new Dictionary<int, string>();

        public BIG()
        {
        }


        public string path;
        string basepath;
        string filelist;
        string bigname;
        string bigpath;
        string extpath;
        string verpath;

        public void InitPaths(string fn)
        {
            path = fn;
            basepath = System.AppDomain.CurrentDomain.BaseDirectory;
            filelist = Path.Combine(basepath, "filenames_usa.txt");
            bigname = Path.GetFileNameWithoutExtension(fn);
            bigpath = Path.GetDirectoryName(fn);
            if (bigpath == null || bigpath == "") bigpath = basepath;
            extpath = Path.Combine(bigpath, bigname) + "\\";
            verpath = Path.Combine(basepath, "versions.json");

            //uncomment if something goes wrong
            /*
            Console.WriteLine(path);
            Console.WriteLine(basepath);
            Console.WriteLine(filelist);
            Console.WriteLine(bigname);
            Console.WriteLine(bigpath);
            Console.WriteLine(extpath);
            */
        }


        public BIG(string fn)
        {
            Console.WriteLine("Begin: " + fn);
            InitPaths(fn);

            ms = new MemoryStream(File.ReadAllBytes(fn));
            br = new BinaryReaderEx(ms);

            br.BaseStream.Position += 4;
            totalFiles = br.ReadInt32();

            var namelist = Meta.LoadNumberedList(Meta.Detect(fn, "bigs", totalFiles));
            Console.WriteLine("File list ({0}) - {1}\r\n", namelist, (namelist != null ? "OK" : "ERROR"));

            for (int i = 0; i < totalFiles; i++)
                pairs.Add(new BigEntry(br, (names.ContainsKey(i) ? names[i] : i.ToString("000")), i));
        }


        StringBuilder biglist = new StringBuilder();

        public void Export()
        {
            int i = 0;

            Directory.CreateDirectory(extpath);
            Console.Write(pairs.Count + " files:\r\n");

            foreach (BigEntry p in pairs)
            {
                //detect known formats

                br.BaseStream.Position = p.Offset * 2048;

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

                if (p.Size == 0) knownext = ".null";

                //--------------

                br.BaseStream.Position = p.Offset * 2048;

                string knownname = "";

                if (names.ContainsKey(i))
                    knownname = names[i];


                string fname =
                    extpath +
                    i.ToString("0000") +
                    (knownname != "" ? ("_" + knownname) : "") +
                    knownext;

                /*
                biglist.Append(i.ToString("0000") +
                    (knownname != "" ? ("_" + knownname) : "") +
                    knownext + "\r\n");
                */


                if (knownname == "") knownname = i.ToString("0000") + knownext;

                if (!knownname.Contains("\\")) knownname = i.ToString("0000") + "_" + knownname;

                byte[] data = br.ReadBytes((int)p.Size);

                //string filemd5 = CalculateMD5(data);

                //if (Path.GetExtension(knownname) == ".lev")
                // biglist.Append(filemd5 + "\t" + "NTSC-J\t" + knownname + "\r\n");

                biglist.Append(knownname + "\r\n");

                fname = Path.Combine(extpath, knownname);


                if (p.Size > 0)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fname));
                    File.WriteAllBytes(fname, data);
                }

                Console.Write(".");

                i++;

                //Console.WriteLine(fname);
            }

            File.WriteAllText(bigname + ".txt", biglist.ToString());

            Console.Write("\r\n");

        }


        public void Build(string txt)
        {
            string path = Path.GetFileNameWithoutExtension(txt);
            string[] files = File.ReadAllLines(txt);


            Console.WriteLine(path);

            for (int i = 0; i < files.Length; i++)
            {
                files[i] = ".\\" + path + "\\" + files[i];
                pairs.Add(new BigEntry(files[i]));
            }

            Console.WriteLine("we'll need " + TotalSize() + " Bytes for " + pairs.Count + " files:");

            byte[] final_big = new byte[TotalSize()];
            MemoryStream ms = new MemoryStream(final_big);
            BinaryWriterEx bw = new BinaryWriterEx(ms);

            bw.BaseStream.Position = 4;
            bw.Write(pairs.Count);

            bw.BaseStream.Position = 3 * 2048;

            foreach (BigEntry c in pairs)
            {
                Console.Write(".");

                int pos = (int)bw.BaseStream.Position;
                c.Offset = pos / 2048;


                bw.Write(c.Data);

                bw.BaseStream.Position = pos + c.SizePadded;
            }

            Console.WriteLine();

            bw.BaseStream.Position = 8;

            foreach (BigEntry c in pairs)
            {
                bw.Write(c.Offset);
                bw.Write(c.Size);
            }

            Console.WriteLine("Dumping to disk:");

            File.WriteAllBytes(path + ".BIG", final_big);

            Console.WriteLine("BIG file successfully created.");

        }



        public int TotalSize()
        {
            //let's hardcode it as you can't add files anyway.
            //ideally you should calculate the amount of sectors used for size/offset array
            int to_alloc = 2048 * 3;

            foreach (BigEntry c in pairs)
                to_alloc += c.SizePadded;

            return to_alloc;
        }


    }
}
