using CTRFramework.Shared;
using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CTRFramework.Audio
{
    public class CseqSong
    {
        HowlContext Context;

        public string Name = "Song";

        public override string ToString() => Name;

        public byte unk0 = 0; //is it part of this struct?

        private int BPM = 120;
        private int TPQN = 0;

        public int BeatsPerMinute
        {
            get
            {
                return BPM;
            }
            set
            {
                BPM = value;
            }
        }

        public int TicksPerQuarterNote
        {
            get
            {
                return TPQN;
            }
            set
            {
                TPQN = value;
            }
        }

        public int MPQN => (int)(60000000.0f / (float)BPM);

        public List<CSeqTrack> Tracks = new List<CSeqTrack>();

        public byte NumTracks => (byte)Tracks.Count;

        #region [Constructors, factories]

        public CseqSong()
        {
        }

        public CseqSong(BinaryReaderEx br, HowlContext context)
        {
            Read(br, context);
        }

        public static CseqSong FromReader(BinaryReaderEx br, HowlContext context = null) => new CseqSong(br, context);

        #endregion

        //reads CSEQ from given binaryreader
        public void Read(BinaryReaderEx br, HowlContext context = null)
        {
            if (context != null)
                Context = context;

            unk0 = br.ReadByte();
            int trackNum = br.ReadByte();
            BPM = br.ReadInt16();
            TPQN = br.ReadInt16();

            //read offsets
            short[] seqOffsets = br.ReadArrayInt16(trackNum);

            //padding i guess?
            //if (trackNum % 2 == 0)
            //    br.ReadInt16();

            br.Pad();

            //save current position to read tracks
            int trackData = (int)br.Position;

            //loop through all tracks
            for (int i = 0; i < trackNum; i++)
            {
                //jump to track offset
                br.Jump(trackData + seqOffsets[i]);

                //add track to the list
                Tracks.Add(CSeqTrack.FromReader(br));

                Tracks[i].Index = i;
            }

            Helpers.PanicIf(NumTracks != trackNum, this, PanicType.Warning, "NumTracks != trackNum, this is weird");
        }

        enum HubNames
        {
            gem_valley = 0,
            nsanity = 1,
            lost_ruins = 2,
            gracier_park = 3,
            citadel_city = 4
        };

        public void ExportMIDI(string fn, Cseq seq, bool hubFilter = false, int hubIndex = 0)
        {
            //add mask theme detection, only export from canyon since it's the 1st cseq
            //if (seq.PatchName != "canyon" && <track is mask>)
            //  do nothing

            // if it's a hub theme, export 5 midis for every hub
            if (seq.PatchName == "adv_gem_valley" && !hubFilter)
            {
                for (int i = 0; i < 5; i++)
                {
                    ExportMIDI(Helpers.PathCombine(Path.GetDirectoryName(fn), $"advhub_{(HubNames)i}.mid"), seq, true, i);

                    Console.WriteLine($"hub_{i}");
                    //Console.ReadKey();
                }

                return;
            }

            string cr = Path.GetFileNameWithoutExtension(fn) + "\r\n\r\n" + Properties.Resources.midi_copyright;

            var mc = new MidiEventCollection(1, TPQN);

            //this is a lazy fix for guitarpro5 bug, 1st track does not import there
            var dummy = new List<MidiEvent>();
            dummy.Add(new TextEvent(Path.GetFileNameWithoutExtension(fn), MetaEventType.SequenceTrackName, 0));
            dummy.Add(new TextEvent(cr, MetaEventType.Copyright, 0));
            dummy.Add(new TempoEvent(MPQN, 0));

            mc.AddTrack(dummy);

            int availablechannel = 1;

            for (int i = 0; i < Tracks.Count; i++)
            {
                if (!hubFilter || ((Cseq.hubTracksMask[i] & (1 << hubIndex)) > 0))
                    mc.AddTrack(Tracks[i].ToMidiEventList(MPQN, Tracks[i].isDrumTrack ? 10 : availablechannel, seq));

                if (!Tracks[i].isDrumTrack)
                {
                    availablechannel++;

                    //skip drum track
                    if (availablechannel == 10) availablechannel++;

                    //limit channel if overflow
                    if (availablechannel > 16) availablechannel = 16;
                }
            }

            // it's a hub midi? load ctr_drumfill! 
            if (hubFilter || seq.PatchName == "character_select")
            {
                var drumfill = new MidiFile(Helpers.GetResourceAsStream("ctr_drumfill.mid"), false);

                foreach (var drum in drumfill.Events)
                {
                    foreach (var midi in drum)
                    {
                        midi.AbsoluteTime /= (drumfill.DeltaTicksPerQuarterNote / TicksPerQuarterNote);
                        Console.WriteLine($"fixing from {midi.AbsoluteTime} to {midi.AbsoluteTime / (drumfill.DeltaTicksPerQuarterNote / TicksPerQuarterNote)}");
                    }
                }

                for (int i = 0; i < drumfill.Tracks; i++)
                    mc.AddTrack(drumfill.Events.GetTrackEvents(i));
            }

            mc.PrepareForExport();


            try
            {
                MidiFile.Export(fn, mc);
            }
            catch (Exception ex)
            {
                Helpers.Panic(this, PanicType.Error, ex.Message);
            }
        }

        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            //sanity check
            if (Tracks.Count > Byte.MaxValue)
                throw new IndexOutOfRangeException("Too many tracks, max 255.");

            long songStart = bw.BaseStream.Position;

            bw.Write((byte)0); //some counter, but always 0?
            bw.Write((byte)Tracks.Count);
            bw.Write((short)BPM);
            bw.Write((short)TPQN);

            long offsetsarraypos = bw.BaseStream.Position;

            bw.Seek(Tracks.Count * 2);
            bw.Pad();

            //if (Tracks.Count % 2 == 0)
            //    bw.Write((short)0);

            var offsets = new List<long>();
            long ptrTracks = bw.BaseStream.Position;

            foreach (var track in Tracks)
            {
                offsets.Add(bw.BaseStream.Position - ptrTracks);
                track.Write(bw);
            }

            bw.Pad();

            long songEnd = bw.BaseStream.Position;

            bw.Jump(offsetsarraypos);

            foreach (long off in offsets)
                bw.Write((short)off);

            bw.Jump(songEnd);
        }
    }
}