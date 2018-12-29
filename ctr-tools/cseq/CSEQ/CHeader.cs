using System;
using System.IO;

namespace cseq
{
    public struct CHeader
    {
        public int size;
        public int chunk12cnt;
        public int chunk8cnt;
        public int extrainfo;

        public bool Read(BinaryReader br)
        {
            size = br.ReadInt32();

            if (size != br.BaseStream.Length)
            {
                return false;
            }

            chunk12cnt = br.ReadByte();
            chunk8cnt = br.ReadByte();

            extrainfo = br.ReadInt16();

            return true;
        }

        public override string ToString()
        {
            return String.Format("size: {0}\r\nchunk8cnt: {1}\r\nchunk12cnt: {2}\r\nextrainfo: {3}\r\n", size, chunk8cnt, chunk12cnt, extrainfo);
        }
    }
}