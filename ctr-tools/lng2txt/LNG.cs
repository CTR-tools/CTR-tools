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
                default: Console.WriteLine("Not supported file."); break;
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


        public void ConvertTXT(string s)
        {
            Console.WriteLine("ConsoleTXTDummy " + s);



        }
    }
}
