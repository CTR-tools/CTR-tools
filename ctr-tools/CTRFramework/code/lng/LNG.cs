using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CTRFramework
{
    public class LNG
    {
        public List<string> entries = new List<string>();

        public LNG(string s)
        {
            switch (Path.GetExtension(s).ToLower())
            {
                case ".lng": ConvertLNG(s); break;
                case ".txt": ConvertTXT(s); break;
                default: Console.WriteLine("Unsupported file."); break;
            }
        }

        public void ConvertLNG(string s)
        {
            using (BinaryReaderEx br = new BinaryReaderEx(File.OpenRead(s)))
            {
                int numStrings = br.ReadInt32();
                int offset = br.ReadInt32();

                br.Jump(offset);

                List<uint> offsets = br.ReadListUInt32(numStrings);

                foreach (uint u in offsets)
                {
                    br.Jump(u);
                    entries.Add(br.ReadStringNT().Replace((char)0x0D, '|'));
                }
            }

            SaveText(Path.ChangeExtension(s, ".txt"));
        }

        public void SaveText(string s)
        {
            StringBuilder sb = new StringBuilder();

            foreach (string en in entries)
                sb.AppendFormat("{0}\r\n", en);

            File.WriteAllText(s, sb.ToString());
        }

        public void LoadText(string s)
        {
            entries = File.ReadAllLines(s).ToList<string>();
        }

        public void ConvertTXT(string s)
        {
            LoadText(s);

            List<int> offsets = new List<int>();

            using (BinaryWriterEx bw = new BinaryWriterEx(File.Open(Path.ChangeExtension(s, ".lng"), FileMode.Create)))
            {
                bw.Write((int)entries.Count());
                bw.Write((int)0); //get back here to know the offset

                foreach (string en in entries)
                {
                    offsets.Add((int)bw.BaseStream.Position);

                    bw.Write(System.Text.Encoding.ASCII.GetBytes(en.Replace("|", "" + (char)0xD)));
                    bw.Write((byte)0);
                }

                int lastoff = (int)bw.BaseStream.Position;

                foreach (int i in offsets)
                    bw.Write(i);

                bw.Write(System.Text.Encoding.ASCII.GetBytes("MISSING MSG\0"));

                bw.BaseStream.Position = 4;
                bw.Write(lastoff);
            }

        }
    }
}
