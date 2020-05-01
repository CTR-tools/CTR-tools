using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CTRFramework
{
    public class LODModel
    {
        public string path;

        public string name;
        public ushort evt;
        public short numLods;
        public int ptrLodHeads;

        List<LODHeader> lh = new List<LODHeader>();

        public LODModel(string s)
        {
            path = s;

            MemoryStream ms = new MemoryStream(File.ReadAllBytes(s));
            BinaryReaderEx br = new BinaryReaderEx(ms);

            int size = br.ReadInt32();

            ms = new MemoryStream(br.ReadBytes((int)size));
            br = new BinaryReaderEx(ms);

            Read(br);
        }

        public LODModel(BinaryReaderEx br)
        {
            Read(br);
        }


        public void Read(BinaryReaderEx br)
        {
            Console.WriteLine(br.BaseStream.Position.ToString("X8"));

            name = br.ReadStringFixed(16);
            evt = br.ReadUInt16();
            numLods = br.ReadInt16();
            ptrLodHeads = br.ReadInt32();

            Console.WriteLine("LODModel: " + name);

            for (int i = 0; i < numLods; i++)
            {
                lh.Add(new LODHeader(br));
            }

            //Helpers.WriteToFile("test.obj", sb.ToString());
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(name + ": ");

            foreach (LODHeader head in lh)
                sb.Append(head.name + ", ");

            sb.Append("\r\n");

            return sb.ToString();
        }
    }
}
