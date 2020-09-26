using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CTRFramework
{
    public class DynamicModel
    {
        public string path;

        public string Name;
        public ushort GameEvent;
        public short numEntries;
        public int ptrHeaders;

        public List<DynamicHeader> headers = new List<DynamicHeader>();

        public static DynamicModel FromFile(string s)
        {
            return new DynamicModel(s);
        }

        public DynamicModel(string s)
        {
            path = s;

            MemoryStream ms = new MemoryStream(File.ReadAllBytes(s));
            BinaryReaderEx br = new BinaryReaderEx(ms);

            int size = br.ReadInt32();

            ms = new MemoryStream(br.ReadBytes((int)size));
            br = new BinaryReaderEx(ms);

            Read(br);
        }

        public DynamicModel(BinaryReaderEx br)
        {
            Read(br);
        }


        public void Read(BinaryReaderEx br)
        {
            Console.WriteLine(br.BaseStream.Position.ToString("X8"));

            Name = br.ReadStringFixed(16);
            GameEvent = br.ReadUInt16();
            numEntries = br.ReadInt16();
            ptrHeaders = br.ReadInt32();

            Console.WriteLine("LODModel: " + Name);

            for (int i = 0; i < numEntries; i++)
            {
                headers.Add(new DynamicHeader(br));
            }

            //Helpers.WriteToFile("test.obj", sb.ToString());
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Name + ": ");

            foreach (DynamicHeader head in headers)
                sb.Append(head.name + ", ");

            sb.Append("\r\n");

            return sb.ToString();
        }


        public void Export(string dir)
        {
            foreach (DynamicHeader h in headers)
            {
                string fn = Path.Combine(dir, String.Format("{0}\\{1}.obj", Name, h.name));
                Helpers.WriteToFile(fn, h.ToObj());
            }
        }
    }
}
