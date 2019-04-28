using System.Collections.Generic;
using System.Text;

namespace cseq
{
    public class CSEQ
    {
        public static bool usdemo = false;

        public CHeader header;
        public List<Sample> longSamples;
        public List<Sample> shortSamples;
        public List<Sequence> sequences;

        public CSEQ()
        {
            header = new CHeader();
            longSamples = new List<Sample>();
            shortSamples = new List<Sample>();
            sequences = new List<Sequence>();
        }

        public bool Read(string s, System.Windows.Forms.TextBox textBox1)
        {
            BinaryReaderEx br = BinaryReaderEx.FromFile(s);

            if (!header.Read(br)) return false;

            for (int i = 0; i < header.longCnt; i++)
                longSamples.Add(Sample.GetLong(br));

            for (int i = 0; i < header.shortCnt; i++)
                shortSamples.Add(Sample.GetShort(br));


            List<short> seqPtrs = new List<short>();

            for (int i = 0; i < header.seqCnt; i++)
                seqPtrs.Add(br.ReadInt16());

            int p = 3;
            if (usdemo) p = 1;

            for (int i = 0; i < p; i++)
                if (br.ReadByte() != 0) 
                    Log.WriteLine("unknown 3 bytes block - not null at " + br.HexPos());

            int seqStart = (int)br.BaseStream.Position;

            for (int i = 0; i < header.seqCnt; i++)
            {
                br.BaseStream.Position = seqStart + seqPtrs[i];

                Sequence seq = new Sequence();
                seq.Read(br, textBox1);
                sequences.Add(seq);
            }

            return true;
        }
    }
}
