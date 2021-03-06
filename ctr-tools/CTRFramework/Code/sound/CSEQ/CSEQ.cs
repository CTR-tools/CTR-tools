﻿using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CTRFramework.Sound.CSeq
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
        public static bool UseSampleVolumeForTracks = true;

        #endregion

        #region [Properties]

        public string path;
        public string name;
        public CHeader header = new CHeader();
        public List<Instrument> longSamples = new List<Instrument>();
        public List<Instrument> shortSamples = new List<Instrument>();
        public List<Sequence> sequences = new List<Sequence>();

        public Bank bank = new Bank();

        #endregion

        #region [Constructors]

        /// <summary>Empty CSEQ constructor.</summary>
        public CSEQ()
        {
        }

        /// <summary>CSEQ constructor.</summary>
        /// <param name="fileName">CSEQ file name.</param>
        public CSEQ(string fileName)
        {
            Read(fileName);
        }

        public CSEQ(BinaryReaderEx br)
        {
            Read(br);
        }

        #endregion


        /// <summary>Reads CSEQ from the file path given.</summary>
        /// <param name="fileName">CSEQ file name.</param>
        public bool Read(string fileName)
        {
            path = Path.GetDirectoryName(fileName);
            name = Path.GetFileNameWithoutExtension(fileName);

            using (BinaryReaderEx br = new BinaryReaderEx(File.OpenRead(fileName)))
            {
                return Read(br);
            }
        }



        public bool Read(BinaryReaderEx br)
        {
            header.Read(br);

            //if we read from howl, it never equals
            //if (header.size != br.BaseStream.Length)
            //{
            //    return false;
            //}

            long pos = br.BaseStream.Position;

            for (int i = 0; i < header.longCnt; i++)
            {
                samplesReverb.Add(new SampleDefReverb(br));
            }

            for (int i = 0; i < header.shortCnt; i++)
            {
                samples.Add(new SampleDef(br));
            }

            br.BaseStream.Position = pos;

            //read instruments
            for (int i = 0; i < header.longCnt; i++)
                longSamples.Add(Instrument.GetLong(br));

            for (int i = 0; i < header.shortCnt; i++)
                shortSamples.Add(Instrument.GetShort(br));

            //read offsets
            short[] seqPtrs = br.ReadArrayInt16(header.seqCnt);

            //awesome NTSC demo fix
            int p = (header.seqCnt == 4) ? 1 : 3;

            //checking whether it's 0 or not
            for (int i = 0; i < p; i++)
                if (br.ReadByte() != 0)
                    Console.WriteLine("unknown 3 bytes block - not null at " + br.HexPos());

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


        public static CSEQ FromFile(string filename)
        {
            return new CSEQ(filename);
        }

        public static CSEQ FromReader(BinaryReaderEx br)
        {
            return new CSEQ(br);
        }


        public void LoadMetaInstruments(string song)
        {
            for (int i = 0; i < longSamples.Count; i++)
            {
                longSamples[i].info = Meta.GetMetaInst(song, "long", i);
            }

            for (int i = 0; i < shortSamples.Count; i++)
            {
                shortSamples[i].info = Meta.GetMetaInst(song, "short", i);
            }
        }

        public void LoadBank(string s)
        {
            bank = new Bank(s);
        }

        /*
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

    */

        /// <summary>Saves CSEQ file.</summary>
        /// <param name="fileName">Target file name.</param>
        public void Export(string fileName)
        {
            Helpers.CheckFolder(Path.GetDirectoryName(fileName));

            using (BinaryWriterEx bw = new BinaryWriterEx(File.Create(fileName)))
            {
                header.Write(bw);

                //System.Windows.Forms.MessageBox.Show(longSamples.Count + " " + shortSamples.Count);

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
            }
        }


        public void ExportSamples()
        {
            if (bank != null)
            {
                foreach (SampleDef s in samples)
                {
                    bank.Export(s.SampleID, s.Frequency, path, name, s.Tag);
                }

                foreach (SampleDefReverb s in samplesReverb)
                {
                    bank.Export(s.SampleID, s.Frequency, path, name, s.Tag);
                }
            }
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

        public string ListMissingSamples()
        {
            StringBuilder sb = new StringBuilder();

            foreach (Instrument x in longSamples)
            {
                if (!bank.Contains(x.sampleID))
                    sb.Append("long: " + x.sampleID + "\r\n");
            }

            foreach (Instrument x in shortSamples)
            {
                if (!bank.Contains(x.sampleID))
                    sb.Append("short: " + x.sampleID + "\r\n");
            }

            return sb.ToString();
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

            return VagSample.DefaultSampleRate;
        }

        public int GetLongSampleIDByTrack(CTrack ct)
        {
            return longSamples[ct.instrument].sampleID;
        }

    }
}
