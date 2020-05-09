using CTRFramework.Shared;
using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.IO;

namespace cseq
{
    public class Sequence
    {
        public CSeqHeader header;
        public List<CTrack> tracks;


        public Sequence()
        {
            header = new CSeqHeader();
            tracks = new List<CTrack>();
        }


        //reads CSEQ from given binaryreader
        public bool Read(BinaryReaderEx br, CSEQ cs)
        {
            //read header
            if (!header.Read(br))
                return false;

            //read offsets
            short[] seqOffsets = br.ReadArrayInt16(header.trackNum);

            //padding i guess?
            if (header.trackNum % 2 == 0)
                br.ReadInt16();

            //save current position to read tracks
            int trackData = (int)br.BaseStream.Position;

            //loop through all tracks
            for (int i = 0; i < header.trackNum; i++)
            {
                //jump to track offset
                br.Jump(trackData + seqOffsets[i]);

                //read track
                CTrack t = new CTrack();
                t.Read(br, i);

                //add track to the list
                tracks.Add(t);
            }

            return true;
        }

        public void ExportMIDI(string fn, CSEQ seq)
        {
            string cr = Path.GetFileNameWithoutExtension(fn) + "\r\n\r\n" + Properties.Resources.midi_copyright;

            MidiEventCollection mc = new MidiEventCollection(1, header.TPQN);

            //this is a lazy fix for guitarpro5 bug, 1st track does not import there
            List<MidiEvent> dummy = new List<MidiEvent>();
            dummy.Add(new TextEvent(Path.GetFileNameWithoutExtension(fn), MetaEventType.SequenceTrackName, 0));
            dummy.Add(new TextEvent(cr, MetaEventType.Copyright, 0));
            dummy.Add(new TempoEvent(header.MPQN, 0));

            mc.AddTrack(dummy);

            int availablechannel = 1;

            for (int i = 0; i < tracks.Count; i++)
            {
                mc.AddTrack(tracks[i].ToMidiEventList(header, tracks[i].isDrumTrack ? 10 : availablechannel, seq));

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


        public void WriteBytes(BinaryWriterEx bw)
        {
            header.WriteBytes(bw);

            long offsetsarraypos = bw.BaseStream.Position;

            foreach (CTrack ct in tracks)
            {
                bw.Write((short)0);
            }

            if (tracks.Count % 2 == 0)
                bw.Write((short)0);

            List<long> offsets = new List<long>();
            long offsetstart = bw.BaseStream.Position;

            foreach (CTrack ct in tracks)
            {
                offsets.Add(bw.BaseStream.Position - offsetstart);
                ct.WriteBytes(bw);
            }

            long comeback = bw.BaseStream.Position;

            bw.BaseStream.Position = offsetsarraypos;

            foreach (long off in offsets)
                bw.Write((short)off);

            bw.BaseStream.Position = comeback;

        }

    }
}