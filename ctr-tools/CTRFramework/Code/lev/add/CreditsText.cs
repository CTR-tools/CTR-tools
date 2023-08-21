using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CTRFramework
{
    public class CreditsText : IRead
    {
        public List<string> Entries = new List<string>();

        public CreditsText()
        {
        }

        public static CreditsText FromReader(BinaryReaderEx br) => new CreditsText(br);

        public CreditsText(BinaryReaderEx br) => Read(br);

        public void Read(BinaryReaderEx br)
        {
            int chunkSize = br.ReadInt32();
            int numEntries = br.ReadInt32();

            Console.WriteLine(numEntries + " ");
            //Console.ReadKey();

            var ptrText = br.ReadListUInt32(numEntries);

            int skip = br.ReadInt32();

            for (int i = 0; i < numEntries; i++)
            {
                Entries.Add(br.ReadStringNT());
            }
        }

        public void Save(string filename)
        {
            var sb = new StringBuilder();

            foreach (var s in Entries)
            {
                sb.AppendLine(s);
            }

            File.WriteAllText(filename, sb.ToString());
        }
    }
}
