using CTRFramework.Shared;
using System;

namespace CTRFramework.Sound.CSeq
{
    public struct CHeader : IRead
    {
        public int size;
        public int longCnt;
        public int shortCnt;
        public int seqCnt;

        public void Read(BinaryReaderEx br)
        {
            size = br.ReadInt32();
            longCnt = br.ReadByte();
            shortCnt = br.ReadByte();
            seqCnt = br.ReadInt16();
        }
        public void Write(BinaryWriterEx bw)
        {
            bw.Write((int)size);
            bw.Write((byte)longCnt);
            bw.Write((byte)shortCnt);
            bw.Write((short)seqCnt);
        }

        public override string ToString()
        {
            return String.Format("size: {0}\r\nlongCnt: {1}\r\nshortCnt: {2}\r\nseqCnt: {3}\r\n", size, longCnt, shortCnt, seqCnt);
        }
    }
}