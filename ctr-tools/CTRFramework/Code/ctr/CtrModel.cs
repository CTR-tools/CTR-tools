using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace CTRFramework
{
    public class CtrModel
    {
        public string path;

        private string name = "defaultname";
        [Browsable(true), DisplayName("Model name"), Description(""), Category("CTR Model")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private CTREvent gameEvent = CTREvent.Nothing;
        [Browsable(true), DisplayName("CTR event"), Description(""), Category("CTR Model")]
        public CTREvent GameEvent
        {
            get { return gameEvent; }
            set { gameEvent = value; }
        }

        //public short numEntries;
        public int ptrHeaders = 0;

        public List<CtrHeader> Entries = new List<CtrHeader>();

        public CtrModel()
        {
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

        public static CtrModel FromFile(string s)
        {
            return new CtrModel(s);
        }

        public void Read(BinaryReaderEx br)
        {
            Console.WriteLine(br.BaseStream.Position.ToString("X8"));

            name = br.ReadStringFixed(16);
            gameEvent = (CTREvent)br.ReadInt16();
            int numEntries = br.ReadInt16();
            ptrHeaders = br.ReadInt32();

            Console.WriteLine("LODModel: " + name);

            for (int i = 0; i < numEntries; i++)
            {
                Entries.Add(new CtrHeader(br));
            }

            //Helpers.WriteToFile("test.obj", sb.ToString());
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(name + ": ");

            foreach (CtrHeader head in Entries)
                sb.Append(head.name + ", ");

            sb.Append("\r\n");

            return sb.ToString();
        }


        public void Export(string dir)
        {
            foreach (CtrHeader h in Entries)
            {
                string fn = Path.Combine(dir, String.Format("{0}\\{1}.obj", name, h.name));
                Helpers.WriteToFile(fn, h.ToObj());
            }
        }

        public void Write(string path)
        {
            Write(path, $"{name}.ctr");
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

            if (name.Length > 16)
                Helpers.Panic(this, $"Name too long: {name}");

            bw.Write(name.ToCharArray());
            bw.BaseStream.Position = 20;

            bw.Write((ushort)gameEvent);
            bw.Write((ushort)Entries.Count);

            ptrs.Add((int)bw.BaseStream.Position);
            bw.Write(ptrHeaders);

            foreach (var ctr in Entries)
            {
                ctr.Write(bw, CtrWriteMode.Header);
            }

            foreach (var ctr in Entries)
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

            curPtr += 64 * Entries.Count;

            if (curPtr % 4 != 0)
                curPtr = ((curPtr / 4) + 1) * 4;

            foreach (var ctr in Entries)
            {
                ctr.ptrCmd = curPtr;
                curPtr += (4 + ctr.drawList.Count * 4 + 4);

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

        public static CtrModel FromObj(OBJ obj)
        {
            return FromObj(new List<OBJ> { obj });
        }

        public static CtrModel FromObj(List<OBJ> objlist)
        {
            if (objlist.Count == 0)
                return null;

            CtrModel ctr = new CtrModel();

            foreach (OBJ obj in objlist)
                ctr.Entries.Add(CtrHeader.FromObj(obj));

            ctr.Name = objlist[0].ObjectName;

            return ctr;
        }
    }

    public enum CtrWriteMode
    {
        Header,
        Data
    }
}