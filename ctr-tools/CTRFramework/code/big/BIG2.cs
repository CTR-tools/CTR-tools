using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CTRFramework.Shared;

namespace CTRFramework
{
    public class BIG2
    {
        public int progress = 0;
        public List<BigEntry> Entries = new List<BigEntry>();

        public BIG2()
        {
        }


        public BIG2(string fn)
        {
            switch (Path.GetExtension(fn).ToLower())
            {
                case ".big": LoadFromBig(fn); break;
                case ".txt": LoadFromTxt(fn); break;

                default: throw new Exception("Unsupported file.");
            }
        }

        public void LoadFromBig(string fn)
        {
            progress = 0;

            Console.WriteLine("Loading BIG from: " + fn);

            using (BinaryReaderEx br = new BinaryReaderEx(File.OpenRead(fn)))
            {
                if (br.ReadUInt32() != 0)
                    Console.WriteLine("Warning! Possibly modified or not a BIG file.");

                int totalFiles = br.ReadInt32();

                var names = Meta.GetProperBigList(fn, totalFiles);

                for (int i = 0; i < totalFiles; i++)
                {
                    Entries.Add(new BigEntry(br, (names.ContainsKey(i) ? names[i] : i.ToString("000")), i));
                    progress = (int)(i * 100.0f / totalFiles);
                }
            }

            Console.WriteLine("BIG loaded.");
        }

        public void LoadFromTxt(string fn)
        {

        }


        public void Export(string path)
        {
            Console.WriteLine("Exporting BIG to: " + path);

            StringBuilder biglist = new StringBuilder();

            Console.Write(Entries.Count + " files:\r\n");

            string filename;
            string dirname;

            foreach (BigEntry cf in Entries)
            {
                filename = Path.Combine(path, cf.Name);
                dirname = Path.GetDirectoryName(filename);

                try
                {
                    Directory.CreateDirectory(dirname);
                }
                catch
                {
                    Console.WriteLine("Can't create directory.");
                }

                biglist.AppendLine(cf.Name);

                if (cf.Size > 0)
                    File.WriteAllBytes(filename, cf.Data);
                
                Console.Write(".");
            }

            File.WriteAllText("bigfile.txt", biglist.ToString());

            Console.WriteLine("\r\nDone.");
        }


        /*
        public void Build(string txt)
        {
            string path = Path.GetFileNameWithoutExtension(txt);
            string[] files = File.ReadAllLines(txt);


            Console.WriteLine(path);

            for (int i = 0; i < files.Length; i++)
            {
                files[i] = ".\\" + path + "\\" + files[i];
                ctrfiles.Add(new CTRFile(files[i]));
            }

            Console.WriteLine("we'll need " + TotalSize() + " Bytes for " + ctrfiles.Count + " files:");

            byte[] final_big = new byte[TotalSize()];
            MemoryStream ms = new MemoryStream(final_big);
            BinaryWriterEx bw = new BinaryWriterEx(ms);

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

            Console.WriteLine(p.disk_dump);

            bw.Close();
            ms.Close();

            File.WriteAllBytes(path + ".BIG", final_big);

            Console.WriteLine(p.big_created);

        }

    */

        public int TotalSize()
        {
            //let's hardcode it as you can't add files anyway.
            //ideally you should calculate the amount of sectors used for size/offset array
            int to_alloc = 2048 * 3;

            foreach (BigEntry c in Entries)
                to_alloc += c.SizePadded;

            return to_alloc;
        }

        private void LoadNames(string path)
        {
            string[] buf = File.ReadAllLines(path);

            foreach (string b in buf)
            {
                if (b.Trim() != "")
                {
                    string[] bb = b.Replace(" ", "").Split('=');

                    int x = -1;
                    Int32.TryParse(bb[0], out x);

                    if (x == -1)
                    {
                        Console.WriteLine("List parsing error at: {0}", bb);
                        continue;
                    }

                    Entries[x].Name = bb[1];
                    //names.Add(x, bb[1]);
                }
            }
        }
    }
}
