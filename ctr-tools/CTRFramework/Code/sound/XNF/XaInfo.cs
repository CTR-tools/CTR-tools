using CTRFramework.Shared;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CTRFramework.Audio
{
    public class XaInfo : IRead
    {
        public string magic = "XINF";

        //0x64 = aug 5 beta
        //0x65 = aug 14 beta
        //0x66 = release
        public int version = 0x66;

        public int numGroups = 0;
        public int numFilesTotal = 0;

        public int[] numFiles;
        public int[] fileStartIndex;
        public int[] numEntries;
        public int[] entryStartIndex;

        public string[] folders;
        public string RootPath = ".\\";
        public string Lang = "ENG";

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
            using (var br = new BinaryReaderEx(File.OpenRead(filename)))
            {
                var xa = XaInfo.FromReader(br);
                xa.RootPath = Path.GetDirectoryName(filename);
                xa.Lang = Path.GetFileNameWithoutExtension(filename);

                xa.folders = new string[] {
                    "MUSIC",
                    $"{xa.Lang}\\EXTRA",
                    $"{xa.Lang}\\GAME",
                    $"{xa.Lang}\\WRAP"  //this one is used in August beta
                };

                return xa;
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

            string list = "";

            //move to versions xml
            if (Entries.Count == 383) list = "xa_aug5_beta.txt";
            if (Entries.Count == 407) list = "xa_aug14_beta.txt";
            if (Entries.Count == 427) list = "xa_usa_beta_sep.txt";
            if (Entries.Count == 414) list = "xa_usa_release.txt";
            if (Entries.Count == 358) list = "xa_pal_release.txt";
            if (Entries.Count == 364) list = "xa_jpn_release.txt";

            for (int i = 0; i < Entries.Count; i++)
                Entries[i].ListIndex = i;

            if (list == "") return;

            var xanames = Helpers.LoadNumberedList(list);

            for (int i = 0; i < Entries.Count; i++)
                if (xanames.ContainsKey(i))
                    Entries[i].Name = xanames[i];
        }

        /// <summary>
        /// Saves XaInfo to file.
        /// </summary>
        /// <param name="filename">Target file name.</param>
        public void Save(string filename)
        {
            Helpers.BackupFile(filename);

            using (var bw = new BinaryWriterEx(File.Create(filename)))
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
            bw.Write("XINF".ToCharArray());
            bw.Write(version);

            bw.Write(numGroups);
            bw.Write(numFilesTotal);
            bw.Write(Entries.Count);

            foreach (var x in numFiles)
                bw.Write(x);

            foreach (var x in fileStartIndex)
                bw.Write(x);

            foreach (var x in numEntries)
                bw.Write(x);

            foreach (var x in entryStartIndex)
                bw.Write(x);

            bw.Seek(numFilesTotal * 4);

            foreach (var entry in Entries)
                entry.Write(bw);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

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