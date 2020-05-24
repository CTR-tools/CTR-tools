using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CTRFramework
{
    public class Dyn
    {
        public string path;

        public string Name;
        public ushort GameEvent;
        public short numEntries;
        public int ptrHeaders;

        public List<DynHeader> headers = new List<DynHeader>();

        public static Dyn FromFile(string s)
        {
            return new Dyn(s);
        }

        public Dyn(string s)
        {
            path = s;

            MemoryStream ms = new MemoryStream(File.ReadAllBytes(s));
            BinaryReaderEx br = new BinaryReaderEx(ms);

            int size = br.ReadInt32();

            ms = new MemoryStream(br.ReadBytes((int)size));
            br = new BinaryReaderEx(ms);

            Read(br);
        }

        public Dyn(BinaryReaderEx br)
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
                headers.Add(new DynHeader(br));
            }

            //Helpers.WriteToFile("test.obj", sb.ToString());
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Name + ": ");

            foreach (DynHeader head in headers)
                sb.Append(head.name + ", ");

            sb.Append("\r\n");

            return sb.ToString();
        }


        public void Export(string dir)
        {
            foreach (DynHeader h in headers)
            {
                string fn = Path.Combine(dir, String.Format("{0}\\{1}.obj", Name, h.name));
                Helpers.WriteToFile(fn, h.ToObj());
            }
        }
    }
}
