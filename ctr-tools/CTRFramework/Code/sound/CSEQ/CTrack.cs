using CTRFramework.Shared;
using NAudio.Midi;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CTRFramework.Sound.CSeq
{
    public class CTrack
    {
        public string Name => $"Track_{Index.ToString("00")}{(isDrumTrack ? "_drums" : "")}";

        //public string address = "";
        public int Index = 0;

        public int instrument = 0;
        public bool isDrumTrack = false;

        public List<Command> cseqEventCollection = new List<Command>();

        public CTrack()
        {
        }

        public CTrack(BinaryReaderEx br)
        {
            Read(br);
        }

        public static CTrack FromReader(BinaryReaderEx br)
        {
            return new CTrack(br);
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
                Command cmd = Command.FromReader(br);

                if (cmd.cseqEvent == CSEQEvent.ChangePatch)
                    instrument = cmd.pitch;

                cseqEventCollection.Add(cmd);

                if (cmd.cseqEvent == CSEQEvent.EndTrack)
                    break;

            }
            while (true);
        }

        public void FromMidiEventList(List<MidiEvent> events)
        {
            cseqEventCollection.Clear();

            foreach (var evt in events)
                cseqEventCollection.Add(Command.FromMidiEvent(evt));
        }

        public List<MidiEvent> ToMidiEventList(int MPQN, int channel, CSEQ seq)
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
                me.Add(new PatchChangeEvent(absTime, channel, Meta.GetBankIndex(CSEQ.PatchName)));
            }

            if (CSEQ.UseSampleVolumeForTracks && !CSEQ.IgnoreVolume)
                me.Add(new ControlChangeEvent(absTime, channel, MidiController.MainVolume, (byte)(seq.samplesReverb[instrument].Volume * 127)));

            foreach (var c in cseqEventCollection)
            {
                me.AddRange(c.ToMidiEvent(absTime, channel, seq, this));
                absTime += c.wait;
            }

            return me;
        }

        public void Write(BinaryWriterEx bw)
        {
            bw.Write((short)(isDrumTrack ? 1 : 0));

            foreach (var cseqEvent in cseqEventCollection)
                cseqEvent.Write(bw);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            //sb.Append(address + "\r\n");

            foreach (Command c in cseqEventCollection)
                sb.Append(c.ToString());

            return sb.ToString();
        }
    }
}