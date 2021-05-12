using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CTRFramework.Sound
{
    public class XaInfo : IRead
    {
        public string magic = "XINF";
        private int version = 0x66;

        private int numGroups = 0;
        private int numFilesTotal = 0;

        private int[] numFiles;
        private int[] fileStartIndex;
        private int[] numEntries;
        private int[] entryStartIndex;

        public List<XaInfoEntry> Entries = new List<XaInfoEntry>();

        public XaInfo()
        {
        }

        public XaInfo(BinaryReaderEx br)
        {
            Read(br);
        }

        public static XaInfo FromReader(BinaryReaderEx br)
        {
            return new XaInfo(br);
        }

        /// <summary>
        /// Creates XaInfo instance from file.
        /// </summary>
        /// <param name="filename">Source file name.</param>
        /// <returns></returns>
        public static XaInfo FromFile(string filename)
        {
            using (BinaryReaderEx br = new BinaryReaderEx(File.OpenRead(filename)))
            {
                return XaInfo.FromReader(br);
            }
        }

        /// <summary>
        /// Read XaInfo data from stream using binary reader.
        /// </summary>
        /// <param name="br">BinaryReaderEx instance.</param>
        public void Read(BinaryReaderEx br)
        {
            magic = new string(br.ReadChars(4));

            if (magic != "XINF")
                Helpers.Panic(this, PanicType.Error, $"No XINF found. Not a XINF file: magic = {magic}");

            version = br.ReadInt32();
            numGroups = br.ReadInt32();
            numFilesTotal = br.ReadInt32();
            int numTotalEntries = br.ReadInt32();

            numFiles = new int[numGroups];
            fileStartIndex = new int[numGroups];
            numEntries = new int[numGroups];
            entryStartIndex = new int[numGroups];

            for (int i = 0; i < numGroups; i++)
                numFiles[i] = br.ReadInt32();

            for (int i = 0; i < numGroups; i++)
                fileStartIndex[i] = br.ReadInt32();

            for (int i = 0; i < numGroups; i++)
                numEntries[i] = br.ReadInt32();

            for (int i = 0; i < numGroups; i++)
                entryStartIndex[i] = br.ReadInt32();

            br.Seek(numFilesTotal * 4);

            for (int i = 0; i < numTotalEntries; i++)
                Entries.Add(XaInfoEntry.FromReader(br));
        }

        /// <summary>
        /// Saves XaInfo to file.
        /// </summary>
        /// <param name="filename">Target file name.</param>
        public void Save(string filename)
        {
            using (BinaryWriterEx bw = new BinaryWriterEx(File.Create(filename)))
            {
                Write(bw);
            }
        }

        /// <summary>
        /// Writes XaInfo data to stream using binary writer.
        /// </summary>
        /// <param name="bw">BinaryWriterEx object.</param>
        public void Write(BinaryWriterEx bw)
        {
            throw new NotImplementedException("Unimplemented.");
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(magic);
            sb.AppendLine($"Version: {version}");
            sb.AppendLine($"numGroups: {numGroups}");
            sb.AppendLine("---");

            foreach (var en in Entries)
                sb.AppendLine(en.ToString());

            return sb.ToString();
        }
    }
}