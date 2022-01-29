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
        /// <param name="filename">Filename.</param>
        /// <returns></returns>
        public static BigFile FromFile(string filename)
        {
            return new BigFile(filename);
        }

        /// <summary>
        /// Reads BigFile depending on the given file type.
        /// </summary>
        /// <param name="filename">Filename.</param>
        private BigFile(string filename)
        {
            switch (Path.GetExtension(filename).ToLower())
            {
                case ".big": LoadFromBig(filename); break;
                case ".txt": LoadFromTxt(filename); break;

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
                throw new FileNotFoundException($"File doesn't exist: {filename}");

            Helpers.Panic(this, PanicType.Info, $"Loading BIG from: {filename}");

            using (var reader = BigFileReader.FromFile(filename))
            {
                while (reader.NextFile())
                    Entries.Add(reader.ReadEntry());
            }

            Helpers.Panic(this, PanicType.Info, "BIG loaded.");
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
            Helpers.Panic(this, PanicType.Info, $"Exporting BIG to: {path}");
            Helpers.Panic(this, PanicType.Info, $"{Entries.Count} files:");

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

            Helpers.Panic(this, PanicType.Info, "\r\nDone.");
        }

        /// <summary>
        /// Saves BigFile to a given location.
        /// </summary>
        /// <param name="filename">Filename.</param>
        public void Save(string filename = Meta.BigFileName)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Helpers.Panic(this, PanicType.Info, $"we'll need {TotalSize / 1024.0f / 1024.0f}MB for {Entries.Count} files:");

            byte[] final_big = new byte[TotalSize];

            using (var bw = new BinaryWriterEx(new MemoryStream(final_big)))
            {
                bw.Write((int)0);
                bw.Write(Entries.Count);

                bw.Jump(3 * Meta.SectorSize);

                foreach (var entry in Entries)
                {
                    Console.Write(".");

                    int pos = (int)bw.BaseStream.Position;
                    entry.Offset = pos / Meta.SectorSize;

                    bw.Write(entry.Data);

                    bw.Jump(pos + entry.SizePadded);
                }

                Console.WriteLine();

                bw.Jump(8);

                foreach (var entry in Entries)
                {
                    bw.Write(entry.Offset);
                    bw.Write(entry.Size);
                }

                Helpers.Panic(this, PanicType.Info, "Dumping to disk...");

                Helpers.WriteToFile(filename, final_big);
            }

            sw.Stop();

            Helpers.Panic(this, PanicType.Info, $"BIG file created in {sw.Elapsed.TotalSeconds}");
        }
    }
}