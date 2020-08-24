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
                int to_alloc = 2048 * 3;

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
            if (File.Exists(fn))
            {
                Console.WriteLine("Loading BIG from: " + fn);

                using (BinaryReaderEx br = new BinaryReaderEx(File.OpenRead(fn)))
                {
                    if (br.ReadUInt32() != 0)
                        Console.WriteLine("Warning! Possibly modified or not a BIG file.");

                    int totalFiles = br.ReadInt32();

                    var names = Meta.GetProperBigList(fn, totalFiles);

                    for (int i = 0; i < totalFiles; i++)
                        Entries.Add(new BigEntry(br, (names.ContainsKey(i) ? names[i] : i.ToString("000")), i));
                }

                Console.WriteLine("BIG loaded.");
            }
            else
            {
                Console.WriteLine("File not found: " + fn);
            }
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
        /// <param name="fn">Filename.</param>
        public void Build(string fn)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Console.WriteLine("we'll need " + TotalSize / 1024.0f / 1024.0f + "MB for " + Entries.Count + " files:");

            byte[] final_big = new byte[TotalSize];
            using (BinaryWriterEx bw = new BinaryWriterEx(new MemoryStream(final_big)))
            {
                bw.Write((int)0);
                bw.Write(Entries.Count);

                bw.BaseStream.Position = 3 * 2048;

                foreach (BigEntry c in Entries)
                {
                    Console.Write(".");

                    int pos = (int)bw.BaseStream.Position;
                    c.Offset = pos / 2048;

                    bw.Write(c.Data);

                    bw.BaseStream.Position = pos + c.SizePadded;
                }

                Console.WriteLine();

                bw.BaseStream.Position = 8;

                foreach (BigEntry c in Entries)
                {
                    bw.Write(c.Offset);
                    bw.Write(c.Size);
                }

                Console.WriteLine("Dumping to disk...");

                File.WriteAllBytes(fn, final_big);
            }

            sw.Stop();

            Console.WriteLine("BIG file created in " + sw.Elapsed.TotalSeconds);
        }
    }
}
