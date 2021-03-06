using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Linq;
using ThreeDeeBear.Models.Ply;

namespace CTRFramework
{
    public class CtrModel
    {
        public static List<UIntPtr> PointerMap = new List<UIntPtr>();

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
        public UIntPtr ptrHeaders = (UIntPtr)0;

        public List<CtrHeader> Entries = new List<CtrHeader>();

        public CtrModel()
        {
        }

        public CtrModel(string filename)
        {
            path = filename;

            using (BinaryReaderEx br = new BinaryReaderEx(File.OpenRead(filename)))
            {
                int dataSize = br.ReadInt32();

                using (BinaryReaderEx br2 = new BinaryReaderEx(new MemoryStream(br.ReadBytes(dataSize))))
                {
                    Read(br2);
                }

                int ptrMapSize = br.ReadInt32() / 4;

                for (int i = 0; i < ptrMapSize; i++)
                    PointerMap.Add((UIntPtr)br.ReadUInt32());
            }
        }

        public CtrModel(BinaryReaderEx br)
        {
            Read(br);
        }

        /// <summary>
        /// Reads CTR model from BinaryReader.
        /// </summary>
        /// <param name="br">BinaryReader object.</param>
        public void Read(BinaryReaderEx br)
        {
            name = br.ReadStringFixed(16);
            gameEvent = (CTREvent)br.ReadInt16();
            int numEntries = br.ReadInt16();
            ptrHeaders = br.ReadUIntPtr();

            for (int i = 0; i < numEntries; i++)
                Entries.Add(new CtrHeader(br));
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

        /// <summary>
        /// Exports all model lods to OBJ files.
        /// </summary>
        /// <param name="path">Path to export.</param>
        public void Export(string path)
        {
            foreach (var en in Entries)
            {
                string fn = Path.Combine(path, $"{name}.{en.name}.obj");
                Helpers.WriteToFile(fn, en.ToObj());
            }
        }

        /// <summary>
        /// Saves CTR model to file using internal model name.
        /// </summary>
        /// <param name="path">Path to save.</param>
        public void Save(string path)
        {
            Save(path, $"{name}.ctr");
        }

        /// <summary>
        /// Saves CTR model to specific file.
        /// </summary>
        /// <param name="path">Path to save.</param>
        /// <param name="filename">Target file name.</param>
        public void Save(string path, string filename)
        {
            using (BinaryWriterEx bw = new BinaryWriterEx(File.OpenWrite(Path.Combine(path, filename))))
            {
                Write(bw);
            }
        }

        //public static List<int> ptrs = new List<int>();

        public void Write(BinaryWriterEx bw)
        {
            PointerMap.Clear();
            BinaryWriterEx.PointerMap.Clear();
            //ptrs.Clear();

            bw.Write(FixPointers());

            if (name.Length > 16)
                Helpers.Panic(this, $"Name too long: {name}");

            bw.Write(name.ToCharArray().Take(16).ToArray());
            bw.BaseStream.Position = 20;

            bw.Write((ushort)gameEvent);
            bw.Write((ushort)Entries.Count);

            //ptrs.Add((int)bw.BaseStream.Position);
            bw.Write(ptrHeaders);

            foreach (var ctr in Entries)
            {
                ctr.Write(bw, CtrWriteMode.Header);
            }

            foreach (var ctr in Entries)
            {
                ctr.Write(bw, CtrWriteMode.Data);
            }

            PointerMap = BinaryWriterEx.PointerMap;

            bw.Write(PointerMap.Count * 4);

            foreach (int x in PointerMap)
                bw.Write(x - 4);
        }



        public int FixPointers()
        {
            int curPtr = 0x18;
            ptrHeaders = (UIntPtr)curPtr;

            curPtr += 64 * Entries.Count;

            if (curPtr % 4 != 0)
                curPtr = ((curPtr / 4) + 1) * 4;

            foreach (var ctr in Entries)
            {
                ctr.ptrCmd = (UIntPtr)curPtr;
                curPtr += (4 + ctr.drawList.Count * 4 + 4);

                if (curPtr % 4 != 0)
                    curPtr = ((curPtr / 4) + 1) * 4;

                ctr.ptrVerts = (UIntPtr)curPtr;
                curPtr += (8 + 16 + 4 + ctr.vtx.Count * 3);

                if (curPtr % 4 != 0)
                    curPtr = ((curPtr / 4) + 1) * 4;

                ctr.ptrTex = (UIntPtr)curPtr;

                if (ctr.tl.Count > 0)
                {
                    curPtr += ctr.tl.Count * 4 + ctr.tl.Count * 0x0C;

                    if (curPtr % 4 != 0)
                        curPtr = ((curPtr / 4) + 1) * 4;
                }

                ctr.ptrClut = (UIntPtr)curPtr;
                curPtr += (ctr.cols.Count * 4);

                if (curPtr % 4 != 0)
                    curPtr = ((curPtr / 4) + 1) * 4;
            }

            return curPtr;
        }

        /// <summary>
        /// Returns CtrModel object from file.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>CtrModel object.</returns>
        public static CtrModel FromFile(string filename)
        {
            return new CtrModel(filename);
        }


        public static CtrModel FromObj(List<OBJ> objlist)
        {
            CtrModel ctr = new CtrModel();

            foreach (OBJ obj in objlist)
                ctr.Entries.Add(CtrHeader.FromObj(obj.ObjectName, obj));

            ctr.Name = objlist[0].ObjectName;

            return ctr;
        }

        public static CtrModel FromObj(OBJ obj)
        {
            return FromObj(new List<OBJ> { obj });
        }

        /// <summary>
        /// Creates CtrModel object from PLY model.
        /// </summary>
        /// <param name="filename">PLY filename.</param>
        /// <returns>CtrModel object.</returns>
        public static CtrModel FromPly(string filename)
        {
            PlyResult ply = PlyHandler.FromFile(filename);

            CtrModel ctr = new CtrModel();
            ctr.Name = Path.GetFileNameWithoutExtension(filename);
            ctr.Entries.Add(CtrHeader.FromPly(ctr.Name, ply));

            return ctr;
        }
    }
}