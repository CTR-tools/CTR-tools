using CTRFramework.Shared;
using System;

namespace CTRtools.CSEQ
{
    public struct CSeqHeader
    {
        public int trackNum;
        public int BPM;
        public int TPQN;
        public int MPQN { get { return (int)(60000000.0f / (float)BPM); } }
        //public float tickDuration { get { return 60.0f / BPM / TPQN * 1000.0f; } }

        public bool Read(BinaryReaderEx br)
        {
            trackNum = br.ReadByte();
            BPM = br.ReadInt16();
            TPQN = br.ReadInt16();

            return true;
        }

        public override string ToString()
        {
            return String.Format("trackNum: {0}\r\nBPM: {1}\r\nTPQN: {2}\r\n", trackNum, BPM, TPQN);
        }

        public void WriteBytes(BinaryWriterEx bw)
        {
            bw.Write((byte)trackNum);
            bw.Write((short)BPM);
            bw.Write((short)TPQN);
        }
    }
}
