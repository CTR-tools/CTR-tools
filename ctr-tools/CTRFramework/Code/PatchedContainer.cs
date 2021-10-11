using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.IO;

namespace CTRFramework
{
    public class PatchedContainer
    {
        public byte[] Data;
        public List<UIntPtr> PatchTable = new List<UIntPtr>();

        public PatchedContainer(BinaryReaderEx br)
        {
            Read(br);
        }

        public BinaryReaderEx GetReader()
        {
            return new BinaryReaderEx(new MemoryStream(Data));
        }

        public void Read(BinaryReaderEx br)
        {
            int dataSize = br.ReadInt32();
            bool hasTable = (dataSize >> 31) == 0;
            dataSize &= ~(1 << 31);
            Data = br.ReadBytes(dataSize);

            if (hasTable)
            {
                int numEntries = br.ReadInt32() / 4;
                for (int i = 0; i < numEntries; i++)
                    PatchTable.Add(br.ReadUIntPtr());
            }
        }

        public void Write(BinaryWriterEx bw)
        {
            bw.Write(Data.Length);
            bw.Write(Data);
            bw.Write(PatchTable.Count * 4);
            foreach (var ptr in PatchTable)
                bw.Write(ptr.ToUInt32());
        }

        public void Save(string filename)
        {
            using (BinaryWriterEx bw = new BinaryWriterEx(File.OpenWrite(filename)))
            {
                Write(bw);
            }
        }

        public static PatchedContainer FromFile(string filename)
        {
            using (BinaryReaderEx br = new BinaryReaderEx(File.OpenRead(filename)))
            {
                return FromReader(br);
            }
        }

        public static PatchedContainer FromReader(BinaryReaderEx br)
        {
            return new PatchedContainer(br);
        }
    }
}
