using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using CTRFramework.Vram;
using ThreeDeeBear.Models.Ply;

namespace CTRFramework
{
    public class CtrModel : IRead
    {
        List<UIntPtr> PatchTable = new List<UIntPtr>();

        public string path;
        string name = "defaultname";
        CTREvent gameEvent = CTREvent.None;
        public List<CtrMesh> Entries = new List<CtrMesh>();

        #region Component model
        [Browsable(true), DisplayName("Model name"), Description(""), Category("CTR Model")]
        public string Name
        {
            get => name;
            set => name = value;
        }

        [Browsable(true), DisplayName("CTR event"), Description(""), Category("CTR Model")]
        public CTREvent GameEvent
        {
            get => gameEvent;
            set => gameEvent = value;
        }
        #endregion

        public CtrModel()
        {
        }

        public CtrModel(string filename)
        {
            path = filename;

            PatchedContainer cnt = PatchedContainer.FromFile(filename);
            Read(cnt.GetReader());
            PatchTable = cnt.PatchTable;
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
            UIntPtr ptrHeaders = br.ReadUIntPtr();

            for (int i = 0; i < numEntries; i++)
                Entries.Add(CtrMesh.FromReader(br));
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(name + ": ");

            foreach (var entry in Entries)
                sb.Append(entry.name + ", ");

            sb.Append("\r\n");

            return sb.ToString();
        }

        /// <summary>
        /// Exports all model lods to OBJ files.
        /// </summary>
        /// <param name="path">Path to export.</param>
        public void Export(string path, Tim vram = null)
        {
            int i = 0;

            foreach (var entry in Entries)
            {
                string fn = Path.Combine(path, $"{name}.{entry.name}.{i.ToString("00")}{(entry.IsAnimated ? ".Animated" : "")}.obj");
                Helpers.WriteToFile(fn, entry.ToObj());

                entry.ExportTextures(path, vram);

                i++;
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
            using (BinaryWriterEx bw = new BinaryWriterEx(File.Create(Path.Combine(path, filename))))
            {
                Write(bw);
            }
        }

        /// <summary>
        /// Writes Ctr model to BinaryWriter.
        /// </summary>
        /// <param name="bw">BinaryWriter object.</param>
        public void Write(BinaryWriterEx bw)
        {
            PatchTable.Clear();

            bw.Write(FixPointers());

            if (name.Length > 16)
                Helpers.Panic(this, PanicType.Warning, $"Name too long, will be truncated: {name}");

            bw.Write(name.ToCharArray().Take(16).ToArray());
            bw.BaseStream.Position = 20;

            bw.Write((ushort)gameEvent);
            bw.Write((ushort)Entries.Count);

            bw.Write((UIntPtr)bw.BaseStream.Position, PatchTable);

            foreach (var ctr in Entries)
                ctr.Write(bw, CtrWriteMode.Header, PatchTable);

            foreach (var ctr in Entries)
                ctr.Write(bw, CtrWriteMode.Data, PatchTable);

            bw.Write(PatchTable.Count * 4);

            foreach (int x in PatchTable)
                bw.Write(x - 4);
        }



        public int FixPointers()
        {
            int curPtr = 0x18;
            //ptrHeaders = (UIntPtr)curPtr;

            curPtr += 64 * Entries.Count;

            if (curPtr % 4 != 0)
                curPtr = ((curPtr / 4) + 1) * 4;

            foreach (var ctr in Entries)
            {
                ctr.ptrCmd = (UIntPtr)curPtr;
                curPtr += (4 + ctr.drawList.Count * 4 + 4);

                if (curPtr % 4 != 0)
                    curPtr = ((curPtr / 4) + 1) * 4;

                ctr.ptrTex = (UIntPtr)curPtr;

                if (ctr.tl.Count > 0)
                {
                    curPtr += ctr.tl.Count * 4 + ctr.tl.Count * 0x0C;

                    if (curPtr % 4 != 0)
                        curPtr = ((curPtr / 4) + 1) * 4;
                }

                ctr.ptrVerts = (UIntPtr)curPtr;
                curPtr += (8 + 16 + 4 + ctr.vtx.Count * 3);

                if (curPtr % 4 != 0)
                    curPtr = ((curPtr / 4) + 1) * 4;

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


        /// <summary>
        /// Reads CtrModel object using binary reader.
        /// </summary>
        /// <param name="br">BinaryReaderEx object.</param>
        /// <returns>CtrModel object.</returns>
        public static CtrModel FromReader(BinaryReaderEx br)
        {
            return new CtrModel(br);
        }


        /// <summary>
        /// Creates CtrModel object using list of OBJ files.
        /// </summary>
        /// <param name="objlist"></param>
        /// <returns></returns>
        public static CtrModel FromObj(List<OBJ> objlist)
        {
            CtrModel ctr = new CtrModel();

            foreach (OBJ obj in objlist)
                ctr.Entries.Add(CtrMesh.FromObj(obj.ObjectName, obj));

            ctr.Name = objlist[0].ObjectName;

            return ctr;
        }

        /// <summary>
        /// Creates CtrModel object, FromObj overload for a single OBJ file.
        /// </summary>
        /// <param name="obj">OBJ object.</param>
        /// <returns></returns>
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
            ctr.Entries.Add(CtrMesh.FromPly(ctr.Name, ply));

            return ctr;
        }

        public void ExportPly(string path)
        {
            foreach (var entry in Entries)
                entry.ExportPly(Path.Combine(path, $"{Name}.ply"));
        }
    }
}