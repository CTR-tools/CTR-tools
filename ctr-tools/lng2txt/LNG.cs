using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace lng2txt
{
    class LNG
    {

        BinaryReader br;
        MemoryStream ms;

        int numStrings;
        int offset;

        string path;


        StringBuilder sb = new StringBuilder();

        public LNG(string s)
        {
            path = s;

            ms = new MemoryStream(File.ReadAllBytes(s));
            br = new BinaryReader(ms);


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
                    x += c;
                }
                while (c != 0);

                sb.Append(x + "\r\n");
            }

        }

        public void Export()
        {
            File.WriteAllText(Path.ChangeExtension(path, ".txt"), sb.ToString());
        }


    }
}
