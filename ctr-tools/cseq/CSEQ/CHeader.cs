using System;
using System.IO;

namespace cseq
{
    public struct CHeader
    {
        public int size;
        public int longCnt;
        public int shortCnt;
        public int seqCnt;

        public bool Read(BinaryReader br)
        {
            size = br.ReadInt32();

            if (size != br.BaseStream.Length)
                return false;

            longCnt = br.ReadByte();
            shortCnt = br.ReadByte();
            seqCnt = br.ReadInt16();

            return true;
        }

        public override string ToString()
        {
            return String.Format("size: {0}\r\nlongCnt: {1}\r\nshortCnt: {2}\r\nseqCnt: {3}\r\n", size, longCnt, shortCnt, seqCnt);
        }
    }
}