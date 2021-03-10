using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CTRFramework.Lang
{
    public class LNG : IReadWrite, IDisposable
    {
        public List<string> Entries = new List<string>();

        public LNG()
        {
        }

        public LNG(BinaryReaderEx br)
        {
            Read(br);
        }

        /// <summary>
        /// Reads ctr localization file from stream using binary reader.
        /// </summary>
        /// <param name="br">BinaryReaderEx object.</param>
        public void Read(BinaryReaderEx br)
        {
            int numStrings = br.ReadInt32();
            int offset = br.ReadInt32();

            br.Jump(offset);

            List<uint> offsets = br.ReadListUInt32(numStrings);

            foreach (uint u in offsets)
            {
                br.Jump(u);
                Entries.Add(br.ReadStringNT().Replace((char)0x0D, '|'));
                // | is considered new line in the entry. used for long strings.
            }
        }

        /// <summary>
        /// Loads LNG object from ctr localization file.
        /// </summary>
        /// <param name="filename">Source file name.</param>
        /// <returns>LNG object.</returns>
        public static LNG FromFile(string filename)
        {
            using (BinaryReaderEx br = new BinaryReaderEx(File.OpenRead(filename)))
            {
                return new LNG(br);
            }
        }

        /// <summary>
        /// Loads LNG object from the array of strings.
        /// </summary>
        /// <param name="lines">Array of strings.</param>
        public static LNG FromText(string[] lines)
        {
            LNG lng = new LNG();
            lng.Entries = lines.ToList();
            foreach (var entry in lng.Entries)
                entry.Trim();
            return lng;
        }

        /// <summary>
        /// Exports CTR localization file to a list of strings 
        /// </summary>
        /// <param name="filename">Target file name.</param>
        public void Export(string filename)
        {
            Helpers.WriteToFile(filename, ToString());
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var entry in Entries)
            {
                if (entry != Entries.Last())
                {
                    sb.AppendLine(entry);
                    continue;
                }

                sb.Append(entry);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Saves ctr localization file.
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
        /// Writes all entries to stream using binary writer.
        /// </summary>
        /// <param name="filename">BinaryWriterEx object.</param>
        public void Write(BinaryWriterEx bw)
        {
            List<string> dEntries = new List<string>();

            foreach (var entry in Entries)
                if (!dEntries.Contains(entry))
                    dEntries.Add(entry);

            var list = new Dictionary<string, int>();

            bw.Write((int)Entries.Count());
            bw.Seek(4); //get back here when we know the offset

            foreach (string entry in dEntries)
            {
                list.Add(entry, (int)bw.BaseStream.Position);
                bw.Write(entry.Replace("|", "" + (char)0xD).ToCharArray());
                bw.Write((byte)0);
            }

            bw.Jump(((bw.BaseStream.Position / 4) + 1) * 4);

            int lastoff = (int)bw.BaseStream.Position;

            foreach (var entry in Entries)
                bw.Write(list[entry]);

            bw.Write("MISSING MSG\0".ToCharArray());

            bw.Jump(4);
            bw.Write(lastoff);
        }

        /// <summary>
        /// Implements IDisposable interface.
        /// </summary>
        public void Dispose()
        {
            Entries.Clear();
        }
    }
}