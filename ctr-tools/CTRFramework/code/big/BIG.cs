using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CTRFramework.Shared;

namespace CTRFramework
{
    public class BIG
    {
        public List<CTRFile> files = new List<CTRFile>();

        public BIG()
        {
        }


        public BIG(string fn)
        {
            Console.WriteLine("Begin: " + fn);

            //string mode = Meta.DetectBig(fn);

            using (BinaryReaderEx br = new BinaryReaderEx(File.OpenRead(fn)))
            {
                br.Jump(4);
                uint totalFiles = br.ReadUInt32();

                for (int i = 0; i < totalFiles; i++)
                {
                    CTRFile cf = new CTRFile();
                    cf.name = i.ToString("0000");
                    cf.offset = br.ReadUInt32() * 2048;
                    cf.size = br.ReadUInt32();

                    files.Add(cf);
                }

                foreach (CTRFile cf in files)
                {
                    br.Jump(cf.offset);
                    cf.data = br.ReadBytes((int)cf.size);
                }

                LoadNames("filenames.txt");
            }
        }

        public void Export(string path)
        {
            StringBuilder biglist = new StringBuilder();

            Console.Write(files.Count + " files:\r\n");

            foreach (CTRFile cf in files)
            {

                string filename = Path.Combine(path, cf.name);
                string dirname = Path.GetDirectoryName(filename);

                try
                {
                    Directory.CreateDirectory(dirname);
                }
                catch
                {

                }

                biglist.AppendLine(cf.name);
                if (cf.size > 0) File.WriteAllBytes(filename, cf.data);
                
                //Console.Write(".");
            }

            File.WriteAllText("bigfile.txt", biglist.ToString());

            Console.Write("\r\n");
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

        public uint TotalSize()
        {
            //let's hardcode it as you can't add files anyway.
            //ideally you should calculate the amount of sectors used for size/offset array
            uint to_alloc = 2048 * 3;

            foreach (CTRFile c in files)
                to_alloc += c.padded_size;

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

                    files[x].name = bb[1];
                    //names.Add(x, bb[1]);
                }
            }
        }
    }
}
