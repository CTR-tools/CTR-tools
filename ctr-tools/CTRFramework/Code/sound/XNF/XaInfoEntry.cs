using CTRFramework.Shared;

namespace CTRFramework.Sound
{
    public class XaInfoEntry : IRead
    {
        public byte Index;
        public byte FileIndex;
        public short Length;

        public string Name = "";

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
        public void Write(BinaryWriterEx bw)
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
            return $"{(Name == "" ? GetName() : Name)}\t{GetName()}\t[{FileIndex}, {Index}], Len: {Length}";
        }
    }
}