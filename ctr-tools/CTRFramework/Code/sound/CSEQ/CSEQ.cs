﻿using CTRFramework.Shared;
using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.IO;

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

    public partial class Cseq : IReadWrite
    {
        public HowlContext Context;

        public string path;
        public string name;

        public short numSamples => (short)samples.Count;
        public short numSamplesReverb => (short)samplesReverb.Count;

        public List<InstrumentShort> samples = new List<InstrumentShort>();
        public List<Instrument> samplesReverb = new List<Instrument>();

        public List<CseqSong> Songs = new List<CseqSong>();

        //public Bank Bank = new Bank();

        #region [Constructors, factories]

        public Cseq()
        {
        }

        public Cseq(BinaryReaderEx br)
        {
            Read(br);
        }

        public static Cseq FromReader(BinaryReaderEx br, HowlContext context = null) => new Cseq(br) { Context = context };

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
            //int p = (seqCnt == 4) ? 1 : 3;

            br.Pad();

            //checking whether it's 0 or not
            //for (int i = 0; i < p; i++)
            //    if (br.ReadByte() != 0)
            //       Helpers.Panic(this, PanicType.Warning, "unknown 3 bytes block - not null at " + br.HexPos());

            //saving sequence data offset
            int seqStart = (int)br.Position;

            //loop through all sequences
            for (int i = 0; i < seqCnt; i++)
            {
                br.Jump(seqStart + seqPtrs[i]);
                Songs.Add(CseqSong.FromReader(br));
                Songs[i].Name = $"Song_{i.ToString("00")}";
            }

            int cseqEnd = (int)br.Position;

            Helpers.PanicIf(cseqEnd - cseqStart != size, this, PanicType.Warning, $"CSEQ size mismatch! {size} vs {cseqEnd - cseqStart}");
        }

        public void LoadMetaInstruments()
        {
            for (int i = 0; i < samplesReverb.Count; i++)
                samplesReverb[i].metaInst = Meta.GetMetaInst(PatchName, "long", i);

            for (int i = 0; i < samples.Count; i++)
                samples[i].metaInst = Meta.GetMetaInst(PatchName, "short", i);
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

            bw.Seek(Songs.Count * 2);
            bw.Pad();

            //it's meant to be like that
            //foreach (var seq in sequences)
            //    bw.Write((short)0);
            //bw.Seek(Songs.Count * 2 + (USdemo ? 1 : 3));

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

            bw.Jump(cseqStart);
            bw.Write(cseqEnd - cseqStart);
            bw.Jump(cseqEnd);

            if (bw.BaseStream.Length < bw.BaseStream.Position)
                bw.BaseStream.SetLength(bw.BaseStream.Position);
        }

        public void ExportSamples(string path)
        {
            if (Context == null) return;

            foreach (var entry in samples)
            {
                var sample = Context.Samples[entry.SampleID];
                sample.GetVag(entry.Frequency).Save(Helpers.PathCombine(path, $"{sample.Name}_{entry.Frequency}.vag"));
            }

            foreach (var entry in samplesReverb)
            {
                var sample = Context.Samples[entry.SampleID];
                sample.GetVag(entry.Frequency).Save(Helpers.PathCombine(path, $"{sample.Name}_{entry.Frequency}.vag"));
            }
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
