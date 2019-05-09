using System;
using System.Collections.Generic;
using NAudio.Midi;
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

        public void Read(BinaryReaderEx br, System.Windows.Forms.TextBox textBox1)
        {
            header.Read(br);

            List<short> seqOffsets = new List<short>();

            for (int i = 0; i < header.trackNum; i++)
                seqOffsets.Add(br.ReadInt16());

            //i dont have any intelligent explanation for this
            if (header.trackNum % 2 == 0) br.ReadInt16();

            int trackData = (int)br.BaseStream.Position;

            for (int i = 0; i < header.trackNum; i++)
            {
                br.BaseStream.Position = trackData + seqOffsets[i];

                CTrack t = new CTrack();
                t.Read(br);
                t.name = "Track_" + i.ToString("00") + (t.isDrumTrack ? "_drum" : "");

                tracks.Add(t);
            }
        }

        public void ExportMIDI(string fn)
        {
            string cr = "(C) 1999, Mutato Muzika: Mark Mothersbaugh, Josh Mancell.\r\n\r\nConverted to MIDI using CTR-Tools by DCxDemo*.";

            MidiEventCollection mc = new MidiEventCollection(1, header.TPQN);

            //this is a lazy fix for guitarpro5 bug, 1st track does not import there
            List<MidiEvent> dummy = new List<MidiEvent>();
            dummy.Add(new TextEvent(Path.GetFileNameWithoutExtension(fn), MetaEventType.SequenceTrackName, 0));
            dummy.Add(new TextEvent(cr, MetaEventType.Copyright, 0));
            dummy.Add(new TempoEvent(header.MPQN, 0));

            mc.AddTrack(dummy);

            for (int i = 0; i < tracks.Count; i++)
                mc.AddTrack(tracks[i].ToMidiEventList(header, tracks[i].isDrumTrack ? 10 : i + 1));

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


        public void WriteBytes(BinaryWriter bw)
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