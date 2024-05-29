using CTRFramework.Shared;
using NAudio.Midi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CTRFramework.Audio
{
    public class CSeqTrack : IReadWrite
    {
        public string Name => $"Track_{Index.ToString("00")}{(isDrumTrack ? "_drums" : "")}";

        //public string address = "";
        public int Index = 0;

        public int instrument = 0;
        public bool isDrumTrack = false;

        public List<CseqEvent> cseqEventCollection = new List<CseqEvent>();

        #region [Constructors, factories]
        public CSeqTrack()
        {
        }

        public CSeqTrack(BinaryReaderEx br) => Read(br);

        public static CSeqTrack FromReader(BinaryReaderEx br) => new CSeqTrack(br);
        #endregion

        public void Import(string filename, int trackNum = 1)
        {
            var midi = new MidiFile(filename);
            FromMidiEventList(midi.Events.GetTrackEvents(trackNum).ToList());
        }

        public void Read(BinaryReaderEx br)
        {
            cseqEventCollection.Clear();

            //trackNum = num;
            //address = br.HexPos();

            ushort trackType = br.ReadUInt16();

            Helpers.PanicIf(trackType > 0, this, PanicType.Warning, "track type value not boolean at " + br.HexPos());

            if (trackType == 1)
                isDrumTrack = true;

            do
            {
                var cmd = CseqEvent.FromReader(br);

                if (cmd.eventType == CseqEventType.ChangePatch)
                    instrument = cmd.pitch;

                cseqEventCollection.Add(cmd);

                if (cmd.eventType == CseqEventType.EndTrack)
                    break;

            }
            while (true);
        }


        public int GetAverageNote()
        {
            int numEntries = 0;
            float value = 0;

            foreach (var evt in cseqEventCollection)
            {
                if (evt.eventType == CseqEventType.NoteOn)
                {
                    if (numEntries == 0)
                    {
                        value += evt.pitch;
                    }
                    else if (numEntries == 1)
                    {
                        value = (value + evt.pitch) / 2.0f;
                    }
                    else
                    {
                        value = value * ((numEntries - 1.0f) / numEntries) + evt.pitch * (1.0f / numEntries);
                    }

                    numEntries++;
                }
            }

            return (int)value;
        }

        public string GetNoteStats()
        {
            var pitches = new Dictionary<int, int>();

            int numEntries = 0;
            float value = 0;

            foreach (var evt in cseqEventCollection)
            {
                if (evt.eventType == CseqEventType.NoteOn)
                    if (pitches.ContainsKey(evt.pitch))
                        pitches[evt.pitch]++;
                    else 
                        pitches.Add(evt.pitch, 1);
            }

            var ordered = pitches.OrderBy(x => x.Key).ToDictionary(pair => pair.Key, pair => pair.Value);

            string result = "";

            foreach (var p in ordered)
                result += p.Key + " " + p.Value + "\r\n";

            return result;
        }

        public void FromMidiEventList(List<MidiEvent> events)
        {
            cseqEventCollection.Clear();

            foreach (var evt in events)
            {
                var newevent = CseqEvent.FromMidiEvent(evt);

                if (newevent != null && newevent.eventType != CseqEventType.Error)
                    cseqEventCollection.Add(newevent);
            }

            for (int i = 1; i < cseqEventCollection.Count; i++)
            {
                cseqEventCollection[i].wait = cseqEventCollection[i].absoluteTime - cseqEventCollection[i - 1].absoluteTime;
            }

            foreach (var evt in events)
            {
                if (evt.Channel == 10)
                {
                    isDrumTrack = true;
                    break;
                }
            }

            //cseqEventCollection.Add(new CseqEvent() { wait = 1, eventType = CseqEventType.EndTrack, });
        }

        public List<MidiEvent> ToMidiEventList(int MPQN, int channel, Cseq seq)
        {
            var me = new List<MidiEvent>();

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
                me.Add(new ControlChangeEvent(absTime, channel, MidiController.MainVolume, (byte)(seq.Instruments[instrument].Volume * 127)));

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

            bw.Pad();
        }

        public string ListCommands()
        {
            var sb = new StringBuilder();

            //sb.Append(address + "\r\n");

            if (!isDrumTrack)
            {
                int avg = GetAverageNote();

                sb.AppendLine($"average track note: {avg}");
                sb.AppendLine($"recommended C sample: C{avg / 12}");

                sb.AppendLine(GetNoteStats());
            }

            foreach (var c in cseqEventCollection)
                sb.AppendLine(c.ToString());

            return sb.ToString();
        }

        public override string ToString() => Name;
    }
}