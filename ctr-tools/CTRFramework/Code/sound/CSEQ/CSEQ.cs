using CTRFramework.Shared;
using NAudio.Midi;
using NAudio.SoundFont;
using System;
using System.Collections.Generic;
using System.IO;

namespace CTRFramework.Audio
{
    /// <summary>
    /// Global CSEQ settings.
    /// </summary>
    public partial class Cseq
    {
        //public bool USdemo = false;
        public static bool PatchMidi = false;
        public static bool IgnoreVolume = false;
        public static bool UseSampleVolumeForTracks = true;

        public static int ActiveInstrument = 0;
        public string PatchName = "";

        //this is hardcoded in the game, cseq doesnt contain this data
        public static byte[] hubTracksMask = new byte[]
        {
            0x1F, 0x17, 0x08, 0x1F,
            0x10, 0x1F, 0x01, 0x08,
            0x01, 0x10, 0x01, 0x1F,
            0x04, 0x04, 0x02, 0x1F,
            0x10, 0x08, 0x10, 0x02
        };
    }

    public partial class Cseq
    {
        public HowlContext Context;

        public string path;
        public string Name;

        public short numPercussions => (short)Percussions.Count;
        public short numInstruments => (short)Instruments.Count;

        public short numSongs => (short)Songs.Count;

        public List<SpuInstrument> Instruments = new List<SpuInstrument>();
        public List<SpuInstrumentShort> Percussions = new List<SpuInstrumentShort>();

        public List<CseqSong> Songs = new List<CseqSong>();

        //public Bank Bank = new Bank();

        #region [Constructors, factories]

        public Cseq()
        {
        }

        public Cseq(BinaryReaderEx br, HowlContext context = null)
        {
            Read(br, context);
        }

        public static Cseq FromReader(BinaryReaderEx br, HowlContext context = null) => new Cseq(br, context);

        public static Cseq FromFile(string filename)
        {
            using (var br = new BinaryReaderEx(File.OpenRead(filename)))
            {
                var seq = FromReader(br);
                seq.path = Path.GetDirectoryName(filename);
                seq.Name = Path.GetFileNameWithoutExtension(filename);

                return seq;
            }
        }

        /*
        public static Cseq FromMidi(string filename)
        {
            var midi = new MidiFile(filename);

            var seq = new Cseq();
            var song = new CseqSong();

            //find a way to read from midi
            song.BeatsPerMinute = 120;
            song.TicksPerQuarterNote = midi.DeltaTicksPerQuarterNote;

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
        */

        #endregion

        public void Read(BinaryReaderEx br, HowlContext context = null)
        {
            Context = context is null ? HowlContext.Create() : context;

            long cseqStart = br.Position;

            int size = br.ReadInt32();
            byte numInstruments = br.ReadByte();
            byte numPercussions = br.ReadByte();
            short numSongs = br.ReadInt16();

            for (int i = 0; i < numInstruments; i++)
            {
                var inst = SpuInstrument.FromReader(br);
                inst = Context.AddInsrumentDeduplicated(Context.InstrumentPool, inst);
                Instruments.Add(inst);
            }

            // Context.InstrumentPool.AddRange(Instruments);

            for (int i = 0; i < numPercussions; i++)
            {
                SpuInstrument inst = SpuInstrumentShort.FromReader(br);
                inst = Context.AddInsrumentDeduplicated(Context.PercussionPool, inst);
                Percussions.Add((SpuInstrumentShort)inst);
            }

            //Context.PercussionPool.AddRange(Percussions);


            //read offsets
            short[] seqPtrs = br.ReadArrayInt16(numSongs);

            // and make sure it's padded, since track data must begin at a valid MIPS offset (divisible by 4)
            br.Pad(4);

            // remember sequence data offset, we will use it to jump to every song
            int seqStart = (int)br.Position;

            // read all songs
            for (int i = 0; i < numSongs; i++)
            {
                br.Jump(seqStart + seqPtrs[i]);
                Songs.Add(CseqSong.FromReader(br, Context));

                // add a generic song name
                Songs[i].Name = $"Song_{i.ToString("00")}";
            }


            int cseqEnd = (int)br.Position;

            Helpers.PanicIf(cseqEnd - cseqStart != size, this, PanicType.Warning, $"CSEQ size mismatch! {size} vs {cseqEnd - cseqStart}");
        }

        public void LoadMetaInstruments()
        {
            for (int i = 0; i < Instruments.Count; i++)
                Instruments[i].metaInst = Meta.GetMetaInst(PatchName, "long", i);

            for (int i = 0; i < Percussions.Count; i++)
                Percussions[i].metaInst = Meta.GetMetaInst(PatchName, "short", i);
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
            if (Instruments.Count > Byte.MaxValue)
                throw new IndexOutOfRangeException("Too many instruments.");

            if (Percussions.Count > Byte.MaxValue)
                throw new IndexOutOfRangeException("Too many percussion instruments.");

            if (Songs.Count > UInt16.MaxValue)
                throw new IndexOutOfRangeException("Too many sequences.");

            int cseqStart = (int)bw.BaseStream.Position;

            bw.Write((int)0); //filesize, write in the end
            bw.Write((byte)Instruments.Count);
            bw.Write((byte)Percussions.Count);
            bw.Write((short)Songs.Count);

            foreach (var instrument in Instruments)
                instrument.Write(bw);

            foreach (var instrument in Percussions)
                instrument.Write(bw);

            long ptrTracks = bw.BaseStream.Position;

            bw.Seek(Songs.Count * 2);
            bw.Pad();


            var offsets = new List<long>();
            long ptrSongs = bw.BaseStream.Position;

            foreach (var song in Songs)
            {
                offsets.Add(bw.BaseStream.Position - ptrSongs);
                song.Write(bw);
            }

            bw.Pad();

            int cseqEnd = (int)bw.BaseStream.Position;

            bw.Jump(ptrTracks);

            foreach (var ptr in offsets)
                bw.Write((short)ptr);

            int size = cseqEnd - cseqStart;

            bw.Jump(cseqStart);
            bw.Write(size);
            bw.Jump(cseqEnd);

            Helpers.PanicIf(size > 0x5800, this, PanicType.Warning, $"Song size exceends hardcoded 11 sectors 0x5800! -> {size.ToString("X8")}");

            if (bw.BaseStream.Length < bw.BaseStream.Position)
                bw.BaseStream.SetLength(bw.BaseStream.Position);
        }

        public void ExportSamples(string path)
        {
            if (Context == null) return;

            foreach (var entry in Percussions)
            {
                var sample = Context.SamplePool[entry.SampleID];
                sample.GetVag(entry.Frequency).Save(Helpers.PathCombine(path, $"{sample.Name}_{entry.Frequency}.vag"));
            }

            foreach (var entry in Instruments)
            {
                var sample = Context.SamplePool[entry.SampleID];
                sample.GetVag(entry.Frequency).Save(Helpers.PathCombine(path, $"{sample.Name}_{entry.Frequency}.vag"));
            }
        }

        public List<int> GetAllIDs()
        {
            List<int> ids = new List<int>();

            foreach (var s in Instruments)
                ids.Add(s.SampleID);

            foreach (var s in Percussions)
                ids.Add(s.SampleID);

            return ids;
        }

        public int GetFrequencyBySampleID(int id)
        {
            foreach (var s in Instruments)
                if (s.SampleID == id)
                    return s.Frequency;

            foreach (var s in Percussions)
                if (s.SampleID == id)
                    return s.Frequency;

            return VagSample.DefaultSampleRate;
        }

        public int GetLongSampleIDByTrack(CSeqTrack ct)
        {
            return Instruments[ct.instrument].SampleID;
        }

        public override string ToString() => $"Songs: {Songs.Count}";
    }
}
