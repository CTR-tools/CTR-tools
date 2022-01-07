using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Xml;

namespace CTRFramework.Big
{
    public class BigFileReader : BinaryReaderEx
    {
        public string Version = "Unknown.";

        public int FileCursor = -1;

        private int totalFiles = 0;
        public int TotalFiles
        {
            get => totalFiles;
        }

        public int FileSize
        {
            get
            {
                Jump(FileCursor * 8 + 12);
                return ReadInt32();
            }
        }

        Dictionary<int, string> names = new Dictionary<int, string>();

        public string GetFilename()
        {
            if (FileCursor != -1)
                if (names.ContainsKey(FileCursor))
                    return names[FileCursor];

            return $"file_{FileCursor.ToString("0000")}.bin";
        }

        private int fileDefPtr => 8 + FileCursor * 8;

        public BigFileReader(Stream stream) : base(stream)
        {
            SanityCheck();
            KnownFileCheck();
        }

        public static BigFileReader FromFile(string filename)
        {
            return new BigFileReader(File.OpenRead(filename));
        }

        /// <summary>
        /// Sanity check for CTR BIG format.
        /// </summary>
        private void SanityCheck()
        {
            Jump(0);

            if (ReadInt32() != 0)
                throw new NotSupportedException($"{this.GetType().Name}: unlikely a CTR BIG file.");

            totalFiles = ReadInt32();

            if (totalFiles > 2048)
                throw new NotSupportedException($"{this.GetType().Name}: unlikely a CTR BIG file, more than 2048 files.");

            for (int i = 0, ptr, size; i < totalFiles; i++)
            {
                ptr = ReadInt32();
                size = ReadInt32();

                if (ptr > BaseStream.Length || ptr + size > BaseStream.Length)
                    throw new NotSupportedException($"{this.GetType().Name}: unlikely a CTR BIG file.");
            }
        }

        /// <summary>
        /// Reads the external list of file names if it's a known BIG file.
        /// </summary>
        private void KnownFileCheck()
        {
            Jump(0);

            //Console.WriteLine(Meta.GetTextFromResource(Meta.XmlPath));
            //Console.ReadKey();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(Helpers.GetTextFromResource(Meta.XmlPath));

            var hash = MD5.Create().ComputeHash(BaseStream);
            string md5 = BitConverter.ToString(hash).Replace("-", "");

            foreach (XmlElement el in doc.SelectNodes("/data/big/entry"))
            {
                if (md5.ToLower() == el["md5"].InnerText.ToLower())
                {
                    Version = $"{el["name"].InnerText} [{el["region"].InnerText}]";
                    Helpers.Panic(this, PanicType.Info, $"{md5}\r\n{Version} detected.\r\nUsing {el["list"].InnerText}");
                    names = Meta.GetBigList(el["list"].InnerText);
                    return;
                }
            }

            foreach (XmlElement el in doc.SelectNodes("/data/filenums/entry"))
            {
                if (TotalFiles == Int32.Parse(el["num"].InnerText))
                {
                    Helpers.Panic(this, PanicType.Info, $"Using {el["list"].InnerText}");
                    names = Meta.GetBigList(el["list"].InnerText);
                    return;
                }
            }

            Helpers.Panic(this, PanicType.Info, "Unknown BIG file.");
        }

        /// <summary>
        /// Reads file entry.
        /// </summary>
        /// <returns>BigEntry instance.</returns>
        public BigEntry ReadEntry()
        {
            if (FileCursor == -1)
                throw new ArgumentOutOfRangeException($"{this.GetType().Name}: Must use NextFile() first!");

            if (fileDefPtr > BaseStream.Length)
                throw new IndexOutOfRangeException($"{this.GetType().Name}: out of bounds.");

            Jump(fileDefPtr);

            int _ptr = ReadInt32() * Meta.SectorSize;
            int _size = ReadInt32();

            if (_ptr + _size > BaseStream.Length)
                throw new IndexOutOfRangeException($"{this.GetType().Name}: out of bounds.");

            Jump(_ptr);

            return new BigEntry()
            {
                Index = FileCursor,
                Name = GetFilename(),
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
                ReadEntry().Save(Path.Combine(path));
        }

        /// <summary>
        /// Moves file cursor in the beginning.
        /// </summary>
        public void Reset()
        {
            FileCursor = -1;
        }
    }
}