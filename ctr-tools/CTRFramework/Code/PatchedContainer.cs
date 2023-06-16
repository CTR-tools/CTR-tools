using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.IO;

namespace CTRFramework
{
    public class PatchedContainer : IReadWrite
    {
        public byte[] Data;
        public List<UIntPtr> PatchTable = new List<UIntPtr>();

        public PatchedContainer()
        {
        }

        public PatchedContainer(BinaryReaderEx br) => Read(br);

        /// <summary>
        /// Wraps existing data in a new MemoryStream and returns a new reader for it.
        /// </summary>
        /// <returns>BinaryReaderEx instance.</returns>
        public BinaryReaderEx GetReader() => new BinaryReaderEx(new MemoryStream(Data));

        public void Read(BinaryReaderEx br)
        {
            int dataSize = br.ReadInt32();
            bool hasTable = (dataSize >> 31) == 0;
            dataSize &= ~(1 << 31);
            Data = br.ReadBytes(dataSize);

            if (hasTable)
            {
                PatchTable = new List<UIntPtr>();

                int numEntries = br.ReadInt32() / 4;
                for (int i = 0; i < numEntries; i++)
                    PatchTable.Add(br.ReadUIntPtr());
            }
        }

        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            bw.Write(Data.Length);
            bw.Write(Data);
            bw.Write(PatchTable.Count * 4);

            foreach (var ptr in PatchTable)
                bw.Write(ptr.ToUInt32());
        }

        public void Save(string filename)
        {
            using (var bw = new BinaryWriterEx(File.OpenWrite(filename)))
            {
                Write(bw);
            }
        }

        public static PatchedContainer FromFile(string filename)
        {
            using (var br = new BinaryReaderEx(File.OpenRead(filename)))
            {
                return FromReader(br);
            }
        }

        public static PatchedContainer FromReader(BinaryReaderEx br) => new PatchedContainer(br);
    }
}