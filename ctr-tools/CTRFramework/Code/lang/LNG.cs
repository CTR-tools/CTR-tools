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
        private readonly string missing = "MISSING MSG";
        public bool ForceKatakana = false;

        public List<string> Entries = new List<string>();

        public LNG()
        {
        }

        public LNG(BinaryReaderEx br, bool forceKatakana = false)
        {
            ForceKatakana = forceKatakana;
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

            var offsets = br.ReadListUInt32(numStrings);

            foreach (uint u in offsets)
            {
                br.Jump(u);

                string entry = br.ReadStringNT(ForceKatakana);
                entry = entry.Replace((char)0x0D, '|');
                //you probably can use it with other chars as well?
                //probably better use U+0303, but it's only Ñ in PAL
                entry = entry.Replace((char)0x03 + "n", "ñ");
                entry = entry.Replace((char)0x03 + "N", "Ñ");
                entry = entry.Replace('#', '¡');
                entry = entry.Replace('$', '¿');

                Entries.Add(entry);

                // PAL: # is ¡
                // PAL: $ is ¿
                // ctr-tools: | is considered new line in the entry. used for long strings.
            }

            for (int i = 0; i < Entries.Count; i++)
            {
                if (Entries[i] == missing)
                    Entries[i] = null;
            }
        }

        /// <summary>
        /// Loads LNG object from ctr localization file.
        /// </summary>
        /// <param name="filename">Source file name.</param>
        /// <returns>LNG object.</returns>
        public static LNG FromFile(string filename, bool forceKatana = false)
        {
            using (var br = new BinaryReaderEx(File.OpenRead(filename)))
            {
                return new LNG(br, forceKatana);
            }
        }

        /// <summary>
        /// Loads LNG object from the array of strings.
        /// </summary>
        /// <param name="lines">Array of strings.</param>
        public static LNG FromText(string[] lines, bool trySwap = false)
        {
            LNG lng = new LNG();
            lng.Entries = lines.ToList();

            //trim every string to avoid extra spaces.
            for (int i = 0; i < lng.Entries.Count; i++)
            {
                // use ; for comments, if needed
                lng.Entries[i] = lng.Entries[i].Split(';')[0].Trim();
                if (lng.Entries[i] == "null")
                    lng.Entries[i] = null;
            }

            if (trySwap)
            {
                string swapfile = Helpers.PathCombine(Meta.BasePath, "charswap.txt");

                if (File.Exists(swapfile))
                {
                    CharSwap swap = new CharSwap(swapfile);

                    for (int i = 0; i < lng.Entries.Count; i++)
                        lng.Entries[i] = swap.Parse(lng.Entries[i], SwapMode.ToEnglish);
                }
            }

            return lng;
        }

        /// <summary>
        /// Exports CTR localization file to a list of strings 
        /// </summary>
        /// <param name="filename">Target file name.</param>
        public void Export(string filename, bool trySwap = false)
        {
            if (trySwap)
            {
                string swapfile = Helpers.PathCombine(Meta.BasePath, "charswap.txt");

                if (File.Exists(swapfile))
                {
                    LNG swaplng = new LNG();
                    CharSwap swap = new CharSwap(swapfile);

                    foreach (string en in Entries)
                        swaplng.Entries.Add(swap.Parse(en, SwapMode.ToLocal));

                    swaplng.Export(filename, false);

                    filename = filename + "_original.txt";
                }
            }

            Helpers.WriteToFile(filename, ToString(), Encoding.UTF8);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < Entries.Count - 1; i++)
                sb.AppendLine(Entries[i] == null ? "null" : Entries[i]);

            sb.Append(Entries[Entries.Count - 1] == null ? "null" : Entries[Entries.Count - 1]);

            return sb.ToString();
        }

        /// <summary>
        /// Saves ctr localization file.
        /// </summary>
        /// <param name="filename">Target file name.</param>
        public void Save(string filename)
        {
            using (var bw = new BinaryWriterEx(File.Create(filename)))
            {
                Write(bw);
            }
        }

        /// <summary>
        /// Writes all entries to stream using binary writer.
        /// </summary>
        /// <param name="filename">BinaryWriterEx object.</param>
        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            var dEntries = new List<string>();

            foreach (var entry in Entries)
                if (entry != null)
                    if (!dEntries.Contains(entry))
                        dEntries.Add(entry);

            var list = new Dictionary<string, int>();

            bw.Write((int)Entries.Count());
            bw.Seek(4); //get back here when we know the offset

            foreach (string entry in dEntries)
            {
                Helpers.Panic(this, PanicType.Debug, entry);
                list.Add(entry, (int)bw.BaseStream.Position);

                string result = entry;
                result = result.Replace('|', (char)0xD);
                result = result.Replace('¡', '#');
                result = result.Replace('¿', '$');
                result = result.Replace("ñ", (char)0x03 + "n");
                result = result.Replace("Ñ", (char)0x03 + "N");

                bw.Write(Encoding.Default.GetBytes(result));
                bw.Write((byte)0);
            }

            bw.Jump(((bw.BaseStream.Position / 4) + 1) * 4);

            int lastoff = (int)bw.BaseStream.Position;
            int ptrMissing = lastoff + Entries.Count * 4;

            foreach (var entry in Entries)
                bw.Write(entry == null ? ptrMissing : list[entry]);

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