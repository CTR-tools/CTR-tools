using System;
using System.IO;

namespace cseq
{
    public struct CSeqHeader
    {
        public int trackNum;
        public int BPM;
        public int TPQN;

        public float tickDuration()
        {
            return 60.0f / BPM / TPQN * 1000.0f;
        }

        public void Read(BinaryReader br)
        {
            trackNum = br.ReadByte();
            BPM = br.ReadInt16();
            TPQN = br.ReadInt16();
        }

        public override string ToString()
        {
            return String.Format("trackNum: {0}\r\nBPM: {1}\r\nTPQN: {2}\r\n", trackNum, BPM, TPQN);
        }
    }
}
