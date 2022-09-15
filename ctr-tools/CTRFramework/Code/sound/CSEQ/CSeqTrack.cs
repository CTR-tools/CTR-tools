using CTRFramework.Shared;
using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CTRFramework.Sound
{
    public class CSeqTrack : IReadWrite
    {
        public string Name => $"Track_{Index.ToString("00")}{(isDrumTrack ? "_drums" : "")}";

        //public string address = "";
        public int Index = 0;

        public int instrument = 0;
        public bool isDrumTrack = false;

        public List<CseqEvent> cseqEventCollection = new List<CseqEvent>();

        public CSeqTrack()
        {
        }

        public CSeqTrack(BinaryReaderEx br)
        {
            Read(br);
        }

        public static CSeqTrack FromReader(BinaryReaderEx br)
        {
            return new CSeqTrack(br);
        }

        public void Import(string filename)
        {
            MidiFile midi = new MidiFile(filename);
            FromMidiEventList(midi.Events.GetTrackEvents(1).ToList());
        }

        public void Read(BinaryReaderEx br)
        {
            cseqEventCollection.Clear();

            //trackNum = num;
            //address = br.HexPos();

            int trackType = br.ReadInt16();

            if (trackType != 0 && trackType != 1)
                Helpers.Panic(this, PanicType.Warning, "track type value not boolean at " + br.HexPos());

            if (trackType == 1)
                isDrumTrack = true;

            do
            {
                CseqEvent cmd = CseqEvent.FromReader(br);

                if (cmd.eventType == CseqEventType.ChangePatch)
                    instrument = cmd.pitch;

                cseqEventCollection.Add(cmd);

                if (cmd.eventType == CseqEventType.EndTrack)
                    break;

            }
            while (true);
        }

        public void FromMidiEventList(List<MidiEvent> events)
        {
            cseqEventCollection.Clear();

            foreach (var evt in events)
                cseqEventCollection.Add(CseqEvent.FromMidiEvent(evt));
        }

        public List<MidiEvent> ToMidiEventList(int MPQN, int channel, Cseq seq)
        {
            List<MidiEvent> me = new List<MidiEvent>();
            //MidiEvent x;

            int absTime = 0;

            me.Add(new TextEvent(Name, MetaEventType.SequenceTrackName, absTime));
            me.Add(new TempoEvent(MPQN, absTime));

            if (channel == 10)
            {
                me.Add(new ControlChangeEvent(absTime, channel, MidiController.BankSelect, 120));
                me.Add(new ControlChangeEvent(absTime, channel, MidiController.BankSelect, 0));
                me.Add(new PatchChangeEvent(absTime, channel, Meta.GetBankIndex(seq.PatchName)));
            }

            if (Cseq.UseSampleVolumeForTracks && !Cseq.IgnoreVolume)
                me.Add(new ControlChangeEvent(absTime, channel, MidiController.MainVolume, (byte)(seq.samplesReverb[instrument].Volume * 127)));

            foreach (var c in cseqEventCollection)
            {
                me.AddRange(c.ToMidiEvent(absTime, channel, seq, this));
                absTime += c.wait;
            }

            return me;
        }

        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            bw.Write((short)(isDrumTrack ? 1 : 0));

            foreach (var cseqEvent in cseqEventCollection)
                cseqEvent.Write(bw);
        }

        public string ListCommands()
        {
            StringBuilder sb = new StringBuilder();

            //sb.Append(address + "\r\n");

            foreach (CseqEvent c in cseqEventCollection)
                sb.Append(c.ToString());

            return sb.ToString();
        }

        public override string ToString() => Name;
    }
}