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
        /// <param name="filename">Filename.</param>
        private void LoadFromBig(string filename)
        {
            if (!File.Exists(filename))
                throw new Exception($"File doesn't exist: {filename}");

            Console.WriteLine($"Loading BIG from: {filename}");

            using (BigFileReader b = BigFileReader.FromFile(filename))
            {
                while (b.NextFile())
                {
                    Entries.Add(b.ReadEntry());
                }
            }

            Console.WriteLine("BIG loaded.");
        }

        /// <summary>
        /// Populates Big with a list of files.
        /// </summary>
        /// <param name="filename">Text file path.</param>
        private void LoadFromTxt(string filename)
        {
            string path = Path.GetFileNameWithoutExtension(filename);
            string[] files = File.ReadAllLines(filename);

            for (int i = 0; i < files.Length; i++)
            {
                files[i] = Path.Combine(".\\", path, files[i]);
                Entries.Add(new BigEntry(files[i]));
            }
        }

        /// <summary>
        /// Exports all BigFile entries to a given folder.
        /// </summary>
        /// <param name="path">Folder to extract files to.</param>
        public void Extract(string path)
        {
            Console.WriteLine($"Exporting BIG to: {path}");
            Console.WriteLine($"{Entries.Count} files:");

            StringBuilder biglist = new StringBuilder();

            foreach (var entry in Entries)
            {
                string filename = Path.Combine(path, entry.Name);
                //Helpers.CheckFolder(Path.GetDirectoryName(filename));

                biglist.AppendLine(entry.Name);

                //this ensures we don't have dummy files
                if (entry.Size > 0)
                    Helpers.WriteToFile(filename, entry.Data);

                Console.Write(".");
            }

            Helpers.WriteToFile(Path.ChangeExtension(path, "txt"), biglist.ToString());

            Console.WriteLine("\r\nDone.");
        }

        /// <summary>
        /// Saves BigFile to a given location.
        /// </summary>
        /// <param name="filename">Filename.</param>
        public void Save(string filename = "bigfile.big")
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
                    c.Offset = pos / Meta.SectorSize;

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

                Helpers.WriteToFile(filename, final_big);
            }

            sw.Stop();

            Console.WriteLine($"BIG file created in {sw.Elapsed.TotalSeconds}");
        }
    }
}