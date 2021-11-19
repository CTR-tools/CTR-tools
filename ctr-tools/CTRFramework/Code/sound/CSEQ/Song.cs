using CTRFramework.Shared;
using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.IO;

namespace CTRFramework.Sound.CSeq
{
    public class Song
    {
        public int BPM;
        public int TPQN;
        public int MPQN => (int)(60000000.0f / (float)BPM);

        public List<CTrack> tracks = new List<CTrack>();

        public Song()
        {
        }

        public Song(BinaryReaderEx br)
        {
            Read(br);
        }

        public static Song FromReader(BinaryReaderEx br)
        {
            return new Song(br);
        }

        //reads CSEQ from given binaryreader
        public bool Read(BinaryReaderEx br)
        {
            int trackNum = br.ReadByte();
            BPM = br.ReadInt16();
            TPQN = br.ReadInt16();

            //read offsets
            short[] seqOffsets = br.ReadArrayInt16(trackNum);

            //padding i guess?
            if (trackNum % 2 == 0)
                br.ReadInt16();

            //save current position to read tracks
            int trackData = (int)br.Position;

            //loop through all tracks
            for (int i = 0; i < trackNum; i++)
            {
                //jump to track offset
                br.Jump(trackData + seqOffsets[i]);

                //add track to the list
                tracks.Add(CTrack.FromReader(br));

                tracks[i].Index = i;
            }

            return true;
        }

        public void ExportMIDI(string fn, CSEQ seq)
        {
            string cr = Path.GetFileNameWithoutExtension(fn) + "\r\n\r\n" + Properties.Resources.midi_copyright;

            MidiEventCollection mc = new MidiEventCollection(1, TPQN);

            //this is a lazy fix for guitarpro5 bug, 1st track does not import there
            List<MidiEvent> dummy = new List<MidiEvent>();
            dummy.Add(new TextEvent(Path.GetFileNameWithoutExtension(fn), MetaEventType.SequenceTrackName, 0));
            dummy.Add(new TextEvent(cr, MetaEventType.Copyright, 0));
            dummy.Add(new TempoEvent(MPQN, 0));

            mc.AddTrack(dummy);

            int availablechannel = 1;

            for (int i = 0; i < tracks.Count; i++)
            {
                mc.AddTrack(tracks[i].ToMidiEventList(MPQN, tracks[i].isDrumTrack ? 10 : availablechannel, seq));

                if (!tracks[i].isDrumTrack)
                {
                    availablechannel++;

                    //skip drum track
                    if (availablechannel == 10) availablechannel++;

                    //limit channel if overflow
                    if (availablechannel > 16) availablechannel = 16;

                }
            }

            mc.PrepareForExport();


            try
            {
                MidiFile.Export(fn, mc);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }


        public void Write(BinaryWriterEx bw)
        {
            //sanity check
            if (tracks.Count > Byte.MaxValue)
                throw new IndexOutOfRangeException("Too many tracks, max 255.");

            long songStart = bw.BaseStream.Position;

            bw.Write((byte)tracks.Count);
            bw.Write((short)BPM);
            bw.Write((short)TPQN);

            long offsetsarraypos = bw.BaseStream.Position;

            bw.Seek(tracks.Count * 2);

            if (tracks.Count % 2 == 0)
                bw.Write((short)0);

            List<long> offsets = new List<long>();
            long ptrTracks = bw.BaseStream.Position;

            foreach (var track in tracks)
            {
                offsets.Add(bw.BaseStream.Position - ptrTracks);
                track.Write(bw);
            }

            long songEnd = bw.BaseStream.Position;

            bw.Jump(offsetsarraypos);

            foreach (long off in offsets)
                bw.Write((short)off);

            bw.Jump(songEnd);
        }
    }
}