using System.Collections.Generic;
using System.Text;
using System.IO;

namespace cseq
{
    public class CSEQ
    {
        public static bool usdemo = false;

        public string name;

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


        public int GetLongSampleIDByTrack(CTrack ct)
        {
            return longSamples[ct.instrument].sampleID;
        }


        public bool Read(string s, System.Windows.Forms.TextBox textBox1)
        {
            name = Path.GetFileNameWithoutExtension(s);
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
                seq.Read(br, textBox1, this);
                sequences.Add(seq);
            }

            return true;
        }

        public void ToSFZ(string fn)
        {
            StringBuilder sb = new StringBuilder();

            foreach (Sample ss in longSamples)
                sb.Append(ss.ToSFZ(name));

            foreach (Sample ss in shortSamples)
                sb.Append(ss.ToSFZ(name));

            File.WriteAllText(fn, sb.ToString());
        }




        public void Export(string s)
        {
            using (BinaryWriter bw = new BinaryWriter(File.Create(s)))
            {
                header.WriteBytes(bw);

                foreach (Sample ls in longSamples)
                    ls.WriteBytes(bw);

                foreach (Sample ss in shortSamples)
                    ss.WriteBytes(bw);


                long offsetsarraypos = bw.BaseStream.Position;

                foreach (Sequence seq in sequences)
                    bw.Write((short)0);

                int p = 3;
                if (usdemo) p = 1;

                for (int i = 0; i < p; i++)
                {
                    bw.Write((byte)0);
                }

                List<long> offsets = new List<long>();
                long offsetstart = bw.BaseStream.Position;

                foreach (Sequence seq in sequences)
                {
                    offsets.Add(bw.BaseStream.Position - offsetstart);
                    seq.WriteBytes(bw);
                }

                bw.BaseStream.Position = offsetsarraypos;

                foreach (long off in offsets)
                    bw.Write((short)off);

                bw.BaseStream.Position = 0;
                bw.Write((int)bw.BaseStream.Length);

                bw.Close();

            }

            System.Windows.Forms.MessageBox.Show("done");
        }

        public bool CheckBankForSamples(Bank bnk)
        {
            foreach (Sample x in longSamples)
            {
                if (!bnk.Contains(x.sampleID)) return false;
            }

            foreach (Sample x in shortSamples)
            {
                if (!bnk.Contains(x.sampleID)) return false;
            }

            return true;
        }

        public List<int> GetAllIDs()
        {
            List<int> ids = new List<int>();

            foreach (Sample s in longSamples)
                ids.Add(s.sampleID);

            foreach (Sample s in shortSamples)
                ids.Add(s.sampleID);

            return ids;
        }

        public int GetFrequencyBySampleID(int id)
        {
            foreach (Sample s in longSamples)
                if (s.sampleID == id) 
                    return s.basepitch * 44100 / 4096;

            foreach (Sample s in shortSamples)
                if (s.sampleID == id)
                    return s.basepitch * 44100 / 4096;

            return 44100;
        }
    }
}
