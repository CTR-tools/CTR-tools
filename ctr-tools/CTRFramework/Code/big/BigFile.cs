using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace CTRFramework.Big
{
    public class BigFile : List<BigEntry>, IDisposable
    {
        public int TotalSize
        {
            get
            {
                //let's hardcode it as you can't add files anyway.
                //ideally you should calculate the amount of sectors used for size/offset array
                int size = Meta.SectorSize * 3;

                foreach (var entry in this)
                    size += entry.SizePadded;

                return size;
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
        public static BigFile FromFile(string filename) => new BigFile(filename);

        /// <summary>
        /// Reads BigFile depending on the given file type.
        /// </summary>
        /// <param name="filename">Filename.</param>
        private BigFile(string filename)
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException($"File doesn't exist: {filename}");

            switch (Path.GetExtension(filename).ToUpper())
            {
                case ".BIG": LoadFromBig(filename); break;
                case ".TXT": LoadFromTxt(filename); break;

                default: throw new Exception("Unsupported file, try BIG or TXT.");
            }
        }

        /// <summary>
        /// Loads Big from compiled BIGFILE.BIG.
        /// </summary>
        /// <param name="filename">Filename.</param>
        private void LoadFromBig(string filename)
        {
            Helpers.Panic(this, PanicType.Info, $"Loading BIG from: {filename}");

            using (var reader = BigFileReader.FromFile(filename))
            {
                while (reader.NextFile())
                    Add(reader.ReadEntry());
            }

            Helpers.Panic(this, PanicType.Info, "BIG loaded.");
        }

        public static BigFile FromBigReader(BigFileReader bfr)
        {
            var big = new BigFile();

            bfr.Reset();

            while (bfr.NextFile())
                big.Add(bfr.ReadEntry());

            return big;
        }

        /// <summary>
        /// Populates Big with a list of files.
        /// </summary>
        /// <param name="filename">Text file path.</param>
        private void LoadFromTxt(string filename)
        {
            string path = Path.GetDirectoryName(filename);
            string bigname = Path.GetFileNameWithoutExtension(filename);
            string[] files = File.ReadAllLines(filename);

            for (int i = 0; i < files.Length; i++)
            {
                files[i] = Helpers.PathCombine(path, bigname, files[i]);
                Add(new BigEntry(files[i]));
            }
        }

        /// <summary>
        /// Exports all BigFile entries to a given folder.
        /// </summary>
        /// <param name="path">Folder to extract files to.</param>
        public void Extract(string path)
        {
            Helpers.Panic(this, PanicType.Info, $"Exporting BIG to: {path}");
            Helpers.Panic(this, PanicType.Info, $"{Count} files:");

            var biglist = new StringBuilder();

            foreach (var entry in this)
            {
                string filename = Helpers.PathCombine(path, entry.Name);
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
            var sw = new Stopwatch();
            sw.Start();

            Helpers.Panic(this, PanicType.Info, $"we'll need {TotalSize / 1024.0f / 1024.0f}MB for {Count} files:");

            byte[] final_big = new byte[TotalSize];

            using (var bw = new BinaryWriterEx(new MemoryStream(final_big, 0, TotalSize)))
            {
                bw.Write((int)0);
                bw.Write(Count);

                bw.Jump(3 * Meta.SectorSize);

                foreach (var entry in this)
                {
                    Console.Write(".");

                    int pos = (int)bw.BaseStream.Position;
                    entry.Offset = pos / Meta.SectorSize;

                    bw.Write(entry.Data);

                    bw.Jump(pos + entry.SizePadded);
                }

                Console.WriteLine();

                bw.Jump(8);

                foreach (var entry in this)
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

        public void ToZip(string filename)
        {
            Console.Write("zip compress...");

            using (var zip = ZipFile.Open(filename, ZipArchiveMode.Update))
            {
                foreach (var entry in this)
                {
                    if (entry.Size == 0) continue;

                    var zipentry = zip.CreateEntry(entry.Name, CompressionLevel.Optimal);

                    using (var entryStream = zipentry.Open())
                    {
                        entryStream.Write(entry.Data, 0, entry.Size);
                    }
                }
            }

            Console.Write("done!");
        }

        public void Dispose()
        {
            this.Clear();
        }
    }
}