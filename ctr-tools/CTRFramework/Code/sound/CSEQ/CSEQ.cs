using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CTRFramework.Sound.CSeq
{
    /// <summary>
    /// Global CSEQ settings.
    /// </summary>
    public partial class CSEQ
    {
        public static bool USdemo = false;
        public static bool PatchMidi = false;
        public static bool IgnoreVolume = false;
        public static bool UseSampleVolumeForTracks = true;

        public static int ActiveInstrument = 0;
        public static string PatchName = "";
    }

    public partial class CSEQ : IReadWrite
    {
        public string path;
        public string name;

        public List<Instrument> samples = new List<Instrument>();
        public List<Instrument> samplesReverb = new List<Instrument>();

        public List<Song> songs = new List<Song>();

        public Bank bank = new Bank();

        #region [Constructors, factories]
        public CSEQ()
        {
        }

        public CSEQ(BinaryReaderEx br)
        {
            Read(br);
        }

        public static CSEQ FromReader(BinaryReaderEx br)
        {
            return new CSEQ(br);
        }

        public static CSEQ FromFile(string filename)
        {
            using (BinaryReaderEx br = new BinaryReaderEx(File.OpenRead(filename)))
            {
                return FromReader(br);
            }
        }
        #endregion

        public void Read(BinaryReaderEx br)
        {
            long pos = br.BaseStream.Position;

            int size = br.ReadInt32();
            byte longCnt = br.ReadByte();
            byte shortCnt = br.ReadByte();
            short seqCnt = br.ReadInt16();

            for (int i = 0; i < longCnt; i++)
                samplesReverb.Add(Instrument.FromReader(br));

            for (int i = 0; i < shortCnt; i++)
                samples.Add(InstrumentShort.FromReader(br));

            //read offsets
            short[] seqPtrs = br.ReadArrayInt16(seqCnt);

            //awesome NTSC demo fix
            int p = (seqCnt == 4) ? 1 : 3;

            //checking whether it's 0 or not
            for (int i = 0; i < p; i++)
                if (br.ReadByte() != 0)
                    Console.WriteLine("unknown 3 bytes block - not null at " + br.HexPos());

            //saving sequence data offset
            int seqStart = (int)br.BaseStream.Position;

            //loop through all sequences
            for (int i = 0; i < seqCnt; i++)
            {
                br.Jump(seqStart + seqPtrs[i]);
                songs.Add(Song.FromReader(br));
            }

            if (br.BaseStream.Position - pos != size)
                Helpers.Panic(this, PanicType.Warning, "CSEQ size mismatch!");

            LoadMetaInstruments(PatchName);
        }

        public void LoadMetaInstruments(string song)
        {
            for (int i = 0; i < samplesReverb.Count; i++)
                samplesReverb[i].metaInst = Meta.GetMetaInst(song, "long", i);

            for (int i = 0; i < samples.Count; i++)
                samples[i].metaInst = Meta.GetMetaInst(song, "short", i);
        }

        public void LoadBank(string filename)
        {
            bank = Bank.FromFile(filename);
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
        public void Save(string filename)
        {
            Helpers.CheckFolder(Path.GetDirectoryName(filename));

            using (BinaryWriterEx bw = new BinaryWriterEx(File.Create(filename)))
            {
                Write(bw);
            }
        }

        /// <summary>
        /// Writes CSEQ file to stream using binary writer.
        /// </summary>
        /// <param name="bw">BinaryWriterEx instance.</param>
        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            //sanity checks
            if (samplesReverb.Count > Byte.MaxValue) 
                throw new IndexOutOfRangeException("Too many instruments.");

            if (samples.Count > Byte.MaxValue) 
                throw new IndexOutOfRangeException("Too many percussion instruments.");

            if (songs.Count > UInt16.MaxValue)
                throw new IndexOutOfRangeException("Too many sequences.");

            int pos = (int)bw.BaseStream.Position;

            bw.Write((int)0); //filesize, write in the end
            bw.Write((byte)samplesReverb.Count);
            bw.Write((byte)samples.Count);
            bw.Write((short)songs.Count);

            foreach (var instrument in samplesReverb)
                instrument.Write(bw);

            foreach (var instrument in samples)
                instrument.Write(bw);

            long offsetsarraypos = bw.BaseStream.Position;

            //it's meant to be like that
            //foreach (var seq in sequences)
            //    bw.Write((short)0);
            bw.Seek(songs.Count * 2);

            bw.Seek(USdemo ? 1 : 3);

            List<long> offsets = new List<long>();
            long offsetstart = bw.BaseStream.Position;

            foreach (var song in songs)
            {
                offsets.Add(bw.BaseStream.Position - offsetstart);
                song.Write(bw);
            }

            int songEnd = (int)bw.BaseStream.Position;

            bw.BaseStream.Position = offsetsarraypos;

            foreach (long off in offsets)
                bw.Write((short)off);

            bw.Jump(pos);

            bw.Write((int)bw.BaseStream.Length);

            bw.Jump(songEnd);
        }

        public void ExportSamples()
        {
            if (bank != null)
            {
                foreach (var s in samples)
                {
                    bank.Export(s.SampleID, s.Frequency, path, name, s.Tag);
                }

                foreach (var s in samplesReverb)
                {
                    bank.Export(s.SampleID, s.Frequency, path, name, s.Tag);
                }
            }
        }

        public bool CheckBankForSamples()
        {
            foreach (var x in samplesReverb)
            {
                if (!bank.Contains(x.SampleID))
                    return false;
            }

            foreach (var x in samples)
            {
                if (!bank.Contains(x.SampleID))
                    return false;
            }

            return true;
        }

        public string ListMissingSamples()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var x in samplesReverb)
            {
                if (!bank.Contains(x.SampleID))
                    sb.Append("long: " + x.SampleID + "\r\n");
            }

            foreach (var x in samples)
            {
                if (!bank.Contains(x.SampleID))
                    sb.Append("short: " + x.SampleID + "\r\n");
            }

            return sb.ToString();
        }

        public List<int> GetAllIDs()
        {
            List<int> ids = new List<int>();

            foreach (var s in samplesReverb)
                ids.Add(s.SampleID);

            foreach (var s in samples)
                ids.Add(s.SampleID);

            return ids;
        }

        public int GetFrequencyBySampleID(int id)
        {
            foreach (var s in samplesReverb)
                if (s.SampleID == id)
                    return s.Frequency;

            foreach (var s in samples)
                if (s.SampleID == id)
                    return s.Frequency;

            return VagSample.DefaultSampleRate;
        }

        public int GetLongSampleIDByTrack(CTrack ct)
        {
            return samplesReverb[ct.instrument].SampleID;
        }
    }
}
