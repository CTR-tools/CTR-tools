using CTRFramework.Shared;
using CTRFramework.Vram;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace CTRFramework.Big
{
    public enum LoadingMode
    {
        FromDisk, ToRam
    }

    public class BigFileReader : BinaryReaderEx
    {
        public string Version = "Unknown.";

        public int FileCursor = -1;


        public int TotalFiles => totalFiles;
        private int totalFiles = 0;

        // retrieves current file size from a pos/offset pair
        public int FileSize
        {
            get
            {
                Jump(12 + 8 * FileCursor);
                return ReadInt32();
            }
        }

        Dictionary<int, string> names = new Dictionary<int, string>();

        // retrieves a proper name based on the loaded list.
        // important, it relies on the current cursor position.
        public string Filename
        {
            get
            {
                if (FileCursor > -1) // if cursor is positive
                    if (names.ContainsKey(FileCursor)) // if name exists
                        return names[FileCursor]; // return name

                // fallback to a generic file_XXXX.bin name
                return $"file_{FileCursor.ToString("0000")}.bin";
            }
        }

        /// <summary>
        /// Holds a pointer to the pos/offset pair for the current file entry
        /// </summary>
        private int fileDefPtr => 8 + FileCursor * 8;

        public BigFileReader(Stream stream) : base(stream)
        {
            Validate();
            KnownFileCheck();
        }


        public static BigFileReader FromFile(string filename, LoadingMode mode = LoadingMode.FromDisk)
        {
            switch (mode)
            {
                // create a reader using a file stream
                case LoadingMode.FromDisk: return new BigFileReader(File.OpenRead(filename));

                // load data to ram in a single pass, then create a reader
                case LoadingMode.ToRam: return new BigFileReader(new MemoryStream(File.ReadAllBytes(filename)));
            }

            Helpers.Panic("BigFileReader", PanicType.Error, "Couldn't create a stream!!!");
            return null;
        }

        /// <summary>
        /// Validates CTR BIG format. Throws an exception if something goes wrong.
        /// </summary>
        private void Validate()
        {
            // make sure we're in the beginning of the stream
            Jump(0);

            // it always starts with a zero
            if (ReadInt32() != 0)
                throw new NotSupportedException($"{this.GetType().Name}: unlikely a CTR BIG file.");

            // amount of files
            totalFiles = ReadInt32();

            // an arbitrary file count, original game only holds about 700 files
            if (totalFiles > 2048)
                throw new NotSupportedException($"{this.GetType().Name}: unlikely a CTR BIG file, more than 2048 files.");

            // scan every entry
            for (int i = 0, ptr = 0, size = 0; i < totalFiles; i++)
            {
                // read pos/offset pair
                ptr = ReadInt32();
                size = ReadInt32();

                // check out of bounds cases
                if (ptr > BaseStream.Length || ptr + size > BaseStream.Length)
                    throw new NotSupportedException($"{this.GetType().Name}: unlikely a CTR BIG file, entry out of bounds.");
            }
        }

        /// <summary>
        /// Reads the external list of file names if it's a known BIG file.
        /// </summary>
        private void KnownFileCheck()
        {
            // ju
            Jump(0);

            var doc = new XmlDocument();
            doc.LoadXml(Helpers.GetTextFromResource(Meta.XmlPath));

            // calculate hash and look for matching entry

            var md5 = Helpers.CalculateMD5(BaseStream);
            string xpath = $"/data/big/entry[@md5='{md5}']";
            var node = doc.SelectSingleNode(xpath);

            if (node != null)
            {
                string name = node["name"]?.InnerText;
                string region = node["region"]?.InnerText;
                string list = node["list"]?.InnerText;

                Version = $"{name} [{region}]";
                Helpers.Panic(this, PanicType.Info, $"{md5}\r\n{Version} detected.\r\nUsing {list}");
                names = Helpers.LoadNumberedList(list);
                return;
            }

            //no matches by hash, let's try number of files
            xpath = $"/data/filenums/entry[@files='{TotalFiles}']";
            node = doc.SelectSingleNode(xpath);

            if (node != null)
            {
                Helpers.Panic(this, PanicType.Info, $"Unknown BIG file. {TotalFiles} files, using {node.Attributes["list"].Value}");
                names = Helpers.LoadNumberedList(node.Attributes["list"].Value);
                return;
            }

            Helpers.Panic(this, PanicType.Info, "Unknown BIG file. No matches found.");
        }

        /// <summary>
        /// Wrapped index jump.
        /// </summary>
        /// <param name="index">File index.</param>
        /// <returns>BigEntry instance.</returns>
        public BigEntry ReadEntry(int index)
        {
            FileCursor = index;
            return ReadEntry();
        }

        /// <summary>
        /// Reads the file entry at the current cursor.
        /// </summary>
        /// <returns>BigEntry instance.</returns>
        public BigEntry ReadEntry()
        {
            if (FileCursor == -1)
                throw new ArgumentOutOfRangeException($"{this.GetType().Name}: Must use NextFile() first!");

            if (fileDefPtr > BaseStream.Length)
                throw new IndexOutOfRangeException($"{this.GetType().Name}: out of bounds.");

            // jump to pos/offset
            Jump(fileDefPtr);

            // read pos/offset
            int _ptr = ReadInt32() * Meta.SectorSize;
            int _size = ReadInt32();

            // validate out of bounds
            if (_ptr + _size > BaseStream.Length)
                throw new IndexOutOfRangeException($"{this.GetType().Name}: out of bounds.");

            // jump to file data
            Jump(_ptr);

            // create a new bigfile entry
            return new BigEntry()
            {
                Index = FileCursor,
                Name = Filename,
                Offset = _ptr,
                Data = ReadBytes(_size)
            };
        }

        /// <summary>
        /// Moves cursor to next file.
        /// </summary>
        /// <returns>True if next file exists, false otherwise.</returns>
        public bool NextFile()
        {
            FileCursor++;
            return FileCursor < TotalFiles;
        }

        /// <summary>
        /// Extracts all entries to target folder.
        /// </summary>
        /// <param name="path">Target path.</param>
        public void ExtractAll(string path)
        {
            Reset();

            while (NextFile())
                ReadEntry().Save(Helpers.PathCombine(path));
        }

        public BigFile GetBigFile() => BigFile.FromBigReader(this);

        /// <summary>
        /// Moves file cursor to the beginning.
        /// </summary>
        public void Reset()
        {
            FileCursor = -1;
        }

        /// <summary>
        /// Loads scene and vram by the absolute index. Please note that vram file comes first, then goes scene file.
        /// Be careful, this function blindly assumes that *you know what you're doing*.
        /// </summary>
        /// <param name="index">File index in the BIG file.</param>
        /// <returns>CtrScene instance.</returns>
        public CtrScene ReadScene(int index)
        {
            if (index < 0 || index >= TotalFiles)
                throw new ArgumentException($"{this.GetType().Name}: Index was out of bounds");

            var vram = ReadEntry(index).ParseAs<CtrVrm>();
            var scene = ReadEntry(index + 1).ParseAs<CtrScene>();
            scene.SetVram(vram);

            return scene;
        }
    }
}