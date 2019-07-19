using System.Collections.Generic;
using System.Text;
using System.IO;
using CTRtools.Helpers;
using CTRtools.SFX;

namespace CTRtools.CSEQ
{
    public class CSEQ
    {

        public List<SampleDef> samples = new List<SampleDef>();
        public List<SampleDefReverb> samplesReverb = new List<SampleDefReverb>();

        #region [CSEQ global settings]

        public static bool USdemo = false;
        public static bool PatchMidi = false;
        public static bool IgnoreVolume = false;

        public static int ActiveInstrument = 0;
        public static string PatchName = "";

        #endregion

        #region [Properties]

        public string name;
        public CHeader header;
        public List<Instrument> longSamples;
        public List<Instrument> shortSamples;
        public List<Sequence> sequences;

        public Bank bank;

        #endregion

        #region [Constructors]

        /// <summary>Empty CSEQ constructor.</summary>
        public CSEQ()
        {
            Init();
        }

        /// <summary>CSEQ constructor.</summary>
        /// <param name="fileName">CSEQ file name.</param>
        public CSEQ(string fileName, System.Windows.Forms.TextBox textBox1)
        {
            Init();
            Read(fileName, textBox1);
        }

        #endregion

        #region [Private methods]

        /// <summary>Initializes CSEQ properties.</summary>
        private void Init()
        {
            header = new CHeader();
            longSamples = new List<Instrument>();
            shortSamples = new List<Instrument>();
            sequences = new List<Sequence>();
            bank = new Bank();
        }

        #endregion

        /// <summary>Reads CSEQ from the file path given.</summary>
        /// <param name="fileName">CSEQ file name.</param>
        /// <param name="textBox1">to be removed</param>
        public bool Read(string fileName, System.Windows.Forms.TextBox textBox1)
        {
            name = Path.GetFileNameWithoutExtension(fileName);
            BinaryReaderEx br = BinaryReaderEx.FromFile(fileName);

            //read CSEQ header
            if (!header.Read(br))
                return false;

            long pos = br.BaseStream.Position;

            for (int i = 0; i < header.longCnt; i++)
            {
                SampleDefReverb sd = new SampleDefReverb();
                sd.Read(br);
                samplesReverb.Add(sd);
            }

            for (int i = 0; i < header.shortCnt; i++)
            {
                SampleDef sd = new SampleDef();
                sd.Read(br);
                samples.Add(sd);
            }

            br.BaseStream.Position = pos;

            //read instruments
            for (int i = 0; i < header.longCnt; i++)
                longSamples.Add(Instrument.GetLong(br));

            for (int i = 0; i < header.shortCnt; i++)
                shortSamples.Add(Instrument.GetShort(br));

            //read offsets
            short[] seqPtrs = br.ReadInt16Array(header.seqCnt);

            //awesome NTSC demo fix
            int p = USdemo ? 1 : 3;

            //checking whether it's 0 or not
            for (int i = 0; i < p; i++)
                if (br.ReadByte() != 0)
                    Log.WriteLine("unknown 3 bytes block - not null at " + br.HexPos());

            //saving sequence data offset
            int seqStart = (int)br.BaseStream.Position;

            //loop through all sequences
            for (int i = 0; i < header.seqCnt; i++)
            {
                br.Jump(seqStart + seqPtrs[i]);

                Sequence seq = new Sequence();
                seq.Read(br, this);

                sequences.Add(seq);
            }

            LoadMetaInstruments(CSEQ.PatchName);

            return true;
        }

        public void LoadMetaInstruments(string song)
        {
            for (int i = 0; i < longSamples.Count; i++)
            {
                longSamples[i].info = CTRJson.GetMetaInst(song, "long", i);
            }

            for (int i = 0; i < shortSamples.Count; i++)
            {
                shortSamples[i].info = CTRJson.GetMetaInst(song, "short", i);
            }
        }

        public void LoadBank(string s)
        {
            bank = new Bank(s);
        }

        /// <summary>Exports every instrument to a SFZ text file.</summary>
        /// <param name="fileName">Target file name.</param>
        public void ToSFZ(string fileName)
        {
            StringBuilder sb = new StringBuilder();

            foreach (Instrument ss in longSamples)
                sb.Append(ss.ToSFZ(name));

            foreach (Instrument ss in shortSamples)
                sb.Append(ss.ToSFZ(name));

            File.WriteAllText(fileName, sb.ToString());
        }

        /// <summary>Saves CSEQ file.</summary>
        /// <param name="fileName">Target file name.</param>
        public void Export(string fileName)
        {
            using (BinaryWriterEx bw = new BinaryWriterEx(File.Create(fileName)))
            {
                header.WriteBytes(bw);

                foreach (Instrument ls in longSamples)
                    ls.Write(bw);

                foreach (Instrument ss in shortSamples)
                    ss.Write(bw);


                long offsetsarraypos = bw.BaseStream.Position;

                foreach (Sequence seq in sequences)
                    bw.Write((short)0);

                int p = 3;
                if (USdemo) p = 1;

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


        public bool CheckBankForSamples()
        {
            foreach (Instrument x in longSamples)
            {
                if (!bank.Contains(x.sampleID)) 
                    return false;
            }

            foreach (Instrument x in shortSamples)
            {
                if (!bank.Contains(x.sampleID)) 
                    return false;
            }

            return true;
        }

        public List<int> GetAllIDs()
        {
            List<int> ids = new List<int>();

            foreach (Instrument s in longSamples)
                ids.Add(s.sampleID);

            foreach (Instrument s in shortSamples)
                ids.Add(s.sampleID);

            return ids;
        }

        public int GetFrequencyBySampleID(int id)
        {
            foreach (Instrument s in longSamples)
                if (s.sampleID == id)
                    return s.frequency;

            foreach (Instrument s in shortSamples)
                if (s.sampleID == id)
                    return s.frequency;

            return VAG.DefaultSampleRate;
        }

        public int GetLongSampleIDByTrack(CTrack ct)
        {
            return longSamples[ct.instrument].sampleID;
        }

    }
}
