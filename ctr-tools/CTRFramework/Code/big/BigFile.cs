using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace CTRFramework.Big
{
    public class BigFile
    {
        public List<BigEntry> Entries = new List<BigEntry>();

        public int TotalSize
        {
            get
            {
                //let's hardcode it as you can't add files anyway.
                //ideally you should calculate the amount of sectors used for size/offset array
                int to_alloc = Meta.SectorSize * 3;

                foreach (BigEntry c in Entries)
                    to_alloc += c.SizePadded;

                return to_alloc;
            }
        }

        public BigFile()
        {
        }

        /// <summary>
        /// Reads BigFile and returns the object.
        /// </summary>
        /// <param name="fn">Filename.</param>
        /// <returns></returns>
        public static BigFile FromFile(string fn)
        {
            return new BigFile(fn);
        }

        /// <summary>
        /// Reads BigFile depending on the given file type.
        /// </summary>
        /// <param name="fn">Filename.</param>
        private BigFile(string fn)
        {
            switch (Path.GetExtension(fn).ToLower())
            {
                case ".big": LoadFromBig(fn); break;
                case ".txt": LoadFromTxt(fn); break;

                default: throw new Exception("Unsupported file.");
            }
        }

        /// <summary>
        /// Loads Big from compiled BIGFILE.BIG.
        /// </summary>
        /// <param name="fn">Filename.</param>
        private void LoadFromBig(string fn)
        {
            if (!File.Exists(fn))
                throw new Exception($"File doesn't exist: {fn}");

            Console.WriteLine($"Loading BIG from: {fn}");

            using (BigFileReader b = new BigFileReader(File.OpenRead(fn)))
            {
                while (b.NextFile())
                {
                    Entries.Add(new BigEntry(b.GetFilename(), b.ReadFile()));
                }
            }

            Console.WriteLine("BIG loaded.");
        }

        /// <summary>
        /// Populates Big with a list of files.
        /// </summary>
        /// <param name="fn">Filename.</param>
        private void LoadFromTxt(string fn)
        {
            string path = Path.GetFileNameWithoutExtension(fn);
            string[] files = File.ReadAllLines(fn);

            //Console.WriteLine(fn);

            for (int i = 0; i < files.Length; i++)
            {
                files[i] = Path.Combine(".\\", path, files[i]);
                Entries.Add(new BigEntry(files[i]));
            }
        }

        /// <summary>
        /// Exports all BigFile entries to a given folder.
        /// </summary>
        /// <param name="path">Folder.</param>
        public void Extract(string path)
        {
            Console.WriteLine("Exporting BIG to: " + path);

            StringBuilder biglist = new StringBuilder();

            Console.Write(Entries.Count + " files:\r\n");

            foreach (BigEntry cf in Entries)
            {
                string filename = Path.Combine(path, cf.Name);

                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filename));
                }
                catch
                {
                    Console.WriteLine("Can't create directory.");
                }

                biglist.AppendLine(cf.Name);

                //this ensures we don't have dummy files
                if (cf.Size > 0)
                    File.WriteAllBytes(filename, cf.Data);

                Console.Write(".");
            }

            File.WriteAllText(Path.GetFileNameWithoutExtension(path) + ".txt", biglist.ToString());

            Console.WriteLine("\r\nDone.");
        }

        /// <summary>
        /// Saves BigFile to a given location.
        /// </summary>
        /// <param name="filename">Filename.</param>
        public void Save(string filename)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Console.WriteLine($"we'll need {TotalSize / 1024.0f / 1024.0f}MB for {Entries.Count} files:");

            byte[] final_big = new byte[TotalSize];
            using (BinaryWriterEx bw = new BinaryWriterEx(new MemoryStream(final_big)))
            {
                bw.Write((int)0);
                bw.Write(Entries.Count);

                bw.Jump(3 * 2048);

                foreach (BigEntry c in Entries)
                {
                    Console.Write(".");

                    int pos = (int)bw.BaseStream.Position;
                    c.Offset = pos / 2048;

                    bw.Write(c.Data);

                    bw.Jump(pos + c.SizePadded);
                }

                Console.WriteLine();

                bw.Jump(8);

                foreach (BigEntry c in Entries)
                {
                    bw.Write(c.Offset);
                    bw.Write(c.Size);
                }

                Console.WriteLine("Dumping to disk...");

                File.WriteAllBytes(filename, final_big);
            }

            sw.Stop();

            Console.WriteLine($"BIG file created in {sw.Elapsed.TotalSeconds}");
        }
    }
}
