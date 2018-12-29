using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace lng2txt
{
    class LNG
    {

        int numStrings;
        int offset;


        StringBuilder sb = new StringBuilder();

        public LNG(string s)
        {
            switch (Path.GetExtension(s).ToLower())
            {
                case ".lng": ConvertLNG(s); break;
                case ".txt": ConvertTXT(s); break;
                default: Console.WriteLine("Not a supported file."); break;
            }
        }


        public void ConvertLNG(string s)
        {
            string path = s;

            byte[] data = File.ReadAllBytes(s);
            MemoryStream ms = new MemoryStream(data);
            BinaryReader br = new BinaryReader(ms);

            numStrings = br.ReadInt32();
            offset = br.ReadInt32();

            for (int i = 0; i < numStrings; i++)
            {
                br.BaseStream.Position = offset + i * 4;
                br.BaseStream.Position = br.ReadUInt32();

                string x = "";
                char c;

                do
                {
                    c = br.ReadChar();
                    if (c != 0) x += c;
                }
                while (c != 0);

                sb.Append(x.Replace((char)0x0D, '|') + "\r\n");
            }

            File.WriteAllText(Path.ChangeExtension(path, ".txt"), sb.ToString());
        }


        public void ConvertTXT(string f)
        {
            string path = f;

            string[] s = File.ReadAllLines(f);

            List<int> offsets = new List<int>();

            using (BinaryWriter writer = new BinaryWriter(File.Open(Path.ChangeExtension(path, ".lng"), FileMode.Create)))
            {
                writer.Write((int)s.Count());
                writer.Write((int)0); //get back here to know the offset

                foreach (string str in s)
                {
                    offsets.Add((int)writer.BaseStream.Position);

                    writer.Write(System.Text.Encoding.ASCII.GetBytes(str.Replace("|", ""+(char)0xD)));
                    writer.Write((byte)0);
                }

                int lastoff = (int)writer.BaseStream.Position;

                foreach (int i in offsets)
                    writer.Write(i);

                writer.Write(System.Text.Encoding.ASCII.GetBytes("MISSING MSG"));
                writer.Write((byte)0);

                writer.BaseStream.Position = 4;
                writer.Write(lastoff);
            }

        }
    }
}
