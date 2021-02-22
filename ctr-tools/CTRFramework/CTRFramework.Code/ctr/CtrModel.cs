using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CTRFramework
{
    public class CtrModel
    {
        public string path;

        public string Name;
        public ushort GameEvent;
        //public short numEntries;
        public int ptrHeaders;

        public List<CtrHeader> headers = new List<CtrHeader>();

        public static CtrModel FromFile(string s)
        {
            return new CtrModel(s);
        }

        public CtrModel(string s)
        {
            path = s;

            MemoryStream ms = new MemoryStream(File.ReadAllBytes(s));
            BinaryReaderEx br = new BinaryReaderEx(ms);

            int size = br.ReadInt32();

            ms = new MemoryStream(br.ReadBytes((int)size));
            br = new BinaryReaderEx(ms);

            Read(br);
        }

        public CtrModel(BinaryReaderEx br)
        {
            Read(br);
        }


        public void Read(BinaryReaderEx br)
        {
            Console.WriteLine(br.BaseStream.Position.ToString("X8"));

            Name = br.ReadStringFixed(16);
            GameEvent = br.ReadUInt16();
            int numEntries = br.ReadInt16();
            ptrHeaders = br.ReadInt32();

            Console.WriteLine("LODModel: " + Name);

            for (int i = 0; i < numEntries; i++)
            {
                headers.Add(new CtrHeader(br));
            }

            //Helpers.WriteToFile("test.obj", sb.ToString());
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Name + ": ");

            foreach (CtrHeader head in headers)
                sb.Append(head.name + ", ");

            sb.Append("\r\n");

            return sb.ToString();
        }


        public void Export(string dir)
        {
            foreach (CtrHeader h in headers)
            {
                string fn = Path.Combine(dir, String.Format("{0}\\{1}.obj", Name, h.name));
                Helpers.WriteToFile(fn, h.ToObj());
                try
                {
                    Write(dir);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(h.name);
                    Console.WriteLine(ex.Message);
                    Console.ReadKey();
                }
            }
        }

        public void Write(string path)
        {
            Write(path, Name + ".ctr");

            //testing
            //CtrModel ctr = CtrModel.FromFile(Path.Combine(path, Name + ".ctr"));
            //ctr.Export(path);
        }

        public void Write(string path, string filename)
        {
            using (BinaryWriterEx bw = new BinaryWriterEx(File.OpenWrite(Path.Combine(path, filename))))
            {
                Write(bw);
                bw.Close();
            }
        }

        public void Write(BinaryWriterEx bw)
        {
            ptrs.Clear();

            bw.Write(FixPointers());

            if (Name.Length > 15)
                Helpers.Panic(this, $"Name too long: {Name}");

            bw.Write(Name.ToCharArray());
            bw.BaseStream.Position = 20;

            bw.Write(GameEvent);
            bw.Write((ushort)headers.Count);

            ptrs.Add((int)bw.BaseStream.Position);
            bw.Write(ptrHeaders);

            foreach (var ctr in headers)
            {
                ctr.Write(bw, CtrWriteMode.Header);
            }

            foreach (var ctr in headers)
            {
                ctr.Write(bw, CtrWriteMode.Data);
            }

            bw.Write(ptrs.Count * 4);

            foreach (int x in ptrs)
                bw.Write(x - 4);
        }


        public static List<int> ptrs = new List<int>();

        public int FixPointers()
        {
            int curPtr = 0x18;
            ptrHeaders = curPtr;

            curPtr += 64 * headers.Count;

            if (curPtr % 4 != 0)
                curPtr = ((curPtr / 4) + 1 ) * 4;

            foreach (var ctr in headers)
            {
                ctr.ptrCmd = curPtr;
                curPtr += (4 + ctr.defs.Count * 4 + 4);

                if (curPtr % 4 != 0)
                    curPtr = ((curPtr / 4) + 1) * 4;

                ctr.ptrVerts = curPtr;
                curPtr += (8 + 16 + 4 + ctr.vtx.Count * 3);

                if (curPtr % 4 != 0)
                    curPtr = ((curPtr / 4) + 1) * 4;

                ctr.ptrTex = curPtr;

                if (ctr.tl.Count > 0)
                {
                    curPtr += ctr.tl.Count * 4 + ctr.tl.Count * 0x0C;

                    if (curPtr % 4 != 0)
                        curPtr = ((curPtr / 4) + 1) * 4;
                }

                ctr.ptrClut = curPtr;
                curPtr += (ctr.cols.Count * 4);

                if (curPtr % 4 != 0)
                    curPtr = ((curPtr / 4) + 1) * 4;
            }

            return curPtr;
        }
    }

    public enum CtrWriteMode
    {
        Header,
        Data
    }
}