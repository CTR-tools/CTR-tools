using CTRFramework.Shared;
using System;
using System.Collections.Generic;

namespace CTRFramework.Audio
{
    public class XaInfoEntry : IReadWrite
    {
        public int ListIndex { get; set; } = -1;
        public byte Index { get; set; }
        public byte FileIndex { get; set; }
        public short Length { get; set; }

        public string Name { get; set; } = "";

        public XaInfoEntry()
        {
        }

        public XaInfoEntry(BinaryReaderEx br)
        {
            Read(br);
        }

        public static XaInfoEntry FromReader(BinaryReaderEx br)
        {
            return new XaInfoEntry(br);
        }

        /// <summary>
        /// Read XaInfoEntry data from stream using binary reader.
        /// </summary>
        /// <param name="br">BinaryReaderEx instance.</param>
        public void Read(BinaryReaderEx br)
        {
            Index = br.ReadByte();
            FileIndex = br.ReadByte();
            Length = br.ReadInt16();
        }

        /// <summary>
        /// Writes XaInfoEntry data to stream using binary writer.
        /// </summary>
        /// <param name="bw">BinaryWriterEx object.</param>
        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            bw.Write(Index);
            bw.Write(FileIndex);
            bw.Write(Length);
        }

        public string GetName()
        {
            return $"S{FileIndex.ToString("00")}.XA[{Index}].wav";
        }

        public override string ToString()
        {
            return $"{ListIndex.ToString("000")}: {(Name == "" ? GetName() : Name)}\t{GetName()}\t[{FileIndex}, {Index}], Len: {Length}";
        }
    }
}