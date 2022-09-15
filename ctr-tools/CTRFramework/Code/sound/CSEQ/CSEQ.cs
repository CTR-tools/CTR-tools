using CTRFramework.Shared;
using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CTRFramework.Sound
{
    /// <summary>
    /// Global CSEQ settings.
    /// </summary>
    public partial class Cseq
    {
        public bool USdemo = false;
        public static bool PatchMidi = false;
        public static bool IgnoreVolume = false;
        public static bool UseSampleVolumeForTracks = true;

        public static int ActiveInstrument = 0;
        public string PatchName = "";

        public static byte[] hubTracksMask = new byte[]
        {
            0x1F, 0x17, 0x08, 0x1F,
            0x10, 0x1F, 0x01, 0x08,
            0x01, 0x10, 0x01, 0x1F,
            0x04, 0x04, 0x02, 0x1F,
            0x10, 0x08, 0x10, 0x02
        };
    }

    public partial class Cseq : IReadWrite
    {
        public string path;
        public string name;

        public short numSamples => (short)samples.Count;
        public short numSamplesReverb => (short)samplesReverb.Count;

        public List<InstrumentShort> samples = new List<InstrumentShort>();
        public List<Instrument> samplesReverb = new List<Instrument>();

        public List<CseqSong> Songs = new List<CseqSong>();

        public Bank Bank = new Bank();

        #region [Constructors, factories]

        public Cseq()
        {
        }

        public Cseq(BinaryReaderEx br)
        {
            Read(br);
        }

        public static Cseq FromReader(BinaryReaderEx br) => new Cseq(br);

        public static Cseq FromFile(string filename)
        {
            using (var br = new BinaryReaderEx(File.OpenRead(filename)))
            {
                var seq = FromReader(br);
                seq.path = Path.GetDirectoryName(filename);
                seq.name = Path.GetFileNameWithoutExtension(filename);

                return seq;
            }
        }

        public static Cseq FromMidi(string filename)
        {
            var midi = new MidiFile(filename);

            var seq = new Cseq();
            var song = new CseqSong();

            song.BPM = 120;
            song.TPQN = 30;

            for (int i = 0; i < midi.Tracks; i++)
            {
                //create a track
                var track = new CSeqTrack();
                track.Index = i;
                //track.name = $"track_{i.ToString("00")}";
                track.FromMidiEventList((List<MidiEvent>)midi.Events.GetTrackEvents(i));

                song.Tracks.Add(track);

                //create an instrument for the track
                var inst = new Instrument();
                inst.Volume = 255;
                inst.Frequency = 11050;
                inst.SampleID = (ushort)i;

                seq.samplesReverb.Add(inst);
            }

            foreach (var x in song.Tracks)
            {
                var c = new CseqEvent();
                c.eventType = CseqEventType.ChangePatch;
                c.pitch = (byte)x.Index;
                c.wait = 0;
                x.cseqEventCollection.Insert(0, c);
            }

            seq.Songs.Add(song);

            return seq;
        }

        #endregion

        public void Read(BinaryReaderEx br)
        {
            long cseqStart = br.Position;

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
                    Helpers.Panic(this, PanicType.Warning, "unknown 3 bytes block - not null at " + br.HexPos());

            //saving sequence data offset
            int seqStart = (int)br.Position;

            //loop through all sequences
            for (int i = 0; i < seqCnt; i++)
            {
                br.Jump(seqStart + seqPtrs[i]);
                Songs.Add(CseqSong.FromReader(br));
            }

            int cseqEnd = (int)br.Position;

            if (cseqEnd - cseqStart != size)
                Helpers.Panic(this, PanicType.Warning, $"CSEQ size mismatch! {size} vs {cseqEnd - cseqStart}");
        }

        public void LoadMetaInstruments()
        {
            for (int i = 0; i < samplesReverb.Count; i++)
                samplesReverb[i].metaInst = Meta.GetMetaInst(PatchName, "long", i);

            for (int i = 0; i < samples.Count; i++)
                samples[i].metaInst = Meta.GetMetaInst(PatchName, "short", i);
        }

        public void LoadBank(string filename)
        {
            Bank = Bank.FromFile(filename);
        }

        public void LoadBank(Bank bank)
        {
            Bank = bank;
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

            using (var bw = new BinaryWriterEx(File.Create(filename)))
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

            if (Songs.Count > UInt16.MaxValue)
                throw new IndexOutOfRangeException("Too many sequences.");

            int cseqStart = (int)bw.BaseStream.Position;

            bw.Write((int)0); //filesize, write in the end
            bw.Write((byte)samplesReverb.Count);
            bw.Write((byte)samples.Count);
            bw.Write((short)Songs.Count);

            foreach (var instrument in samplesReverb)
                instrument.Write(bw);

            foreach (var instrument in samples)
                instrument.Write(bw);

            long ptrTracks = bw.BaseStream.Position;

            //it's meant to be like that
            //foreach (var seq in sequences)
            //    bw.Write((short)0);
            bw.Seek(Songs.Count * 2 + (USdemo ? 1 : 3));

            List<long> pointers = new List<long>();
            long offsetstart = bw.BaseStream.Position;

            foreach (var song in Songs)
            {
                pointers.Add(bw.BaseStream.Position - offsetstart);
                song.Write(bw);
            }

            int cseqEnd = (int)bw.BaseStream.Position;

            bw.Jump(ptrTracks);

            foreach (var ptr in pointers)
                bw.Write((short)ptr);

            bw.Jump(cseqStart);
            bw.Write(cseqEnd - cseqStart);
            bw.Jump(cseqEnd);
        }

        public void ExportSamples()
        {
            if (Bank != null)
            {
                foreach (var s in samples)
                {
                    Bank.Export(s.SampleID, s.Frequency, path, name, s.Tag);
                }

                foreach (var s in samplesReverb)
                {
                    Bank.Export(s.SampleID, s.Frequency, path, name, s.Tag);
                }
            }
        }

        public bool CheckBankForSamples()
        {
            foreach (var x in samplesReverb)
            {
                if (!Bank.Contains(x.SampleID))
                    return false;
            }

            foreach (var x in samples)
            {
                if (!Bank.Contains(x.SampleID))
                    return false;
            }

            return true;
        }

        public string ListMissingSamples()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var x in samplesReverb)
            {
                if (!Bank.Contains(x.SampleID))
                    sb.Append("long: " + x.SampleID + "\r\n");
            }

            foreach (var x in samples)
            {
                if (!Bank.Contains(x.SampleID))
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

        public int GetLongSampleIDByTrack(CSeqTrack ct)
        {
            return samplesReverb[ct.instrument].SampleID;
        }

        public override string ToString() => $"Songs: {Songs.Count}";
    }
}
