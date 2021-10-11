using CTRFramework.Shared;
using NAudio.Midi;
using System;
using System.Collections.Generic;


namespace CTRFramework.Sound.CSeq
{
    public enum CSEQEvent
    {
        NoteOff = 0x01,
        EndTrack2 = 0x02,
        EndTrack = 0x03,
        Unknown4 = 0x04,
        NoteOn = 0x05,
        VelAssume = 0x06,
        PanAssume = 0x07,
        Unknown8 = 0x08,
        ChangePatch = 0x09,
        BendAssume = 0x0A,
        Error = 0xFF
    }


    public class Command
    {
        public CSEQEvent evt = CSEQEvent.Error;
        public byte pitch = 0;
        public byte velocity = 0;
        public int wait = 0;

        public int absoluteTime = 0;

        public void Read(BinaryReaderEx br)
        {
            wait = br.ReadTimeDelta();

            byte op = br.ReadByte();

            evt = (CSEQEvent)op;

            switch (evt)
            {
                case CSEQEvent.Unknown4:
                case CSEQEvent.Unknown8:
                    {
                        pitch = br.ReadByte();
                        Console.Write(op.ToString("X2") + " found at " + br.HexPos() + "\r\n");
                        break;
                    }

                case CSEQEvent.EndTrack2:
                case CSEQEvent.ChangePatch:
                case CSEQEvent.BendAssume:
                case CSEQEvent.VelAssume:
                case CSEQEvent.PanAssume:
                case CSEQEvent.NoteOff:
                    {
                        pitch = br.ReadByte();
                        break;
                    }
                case CSEQEvent.NoteOn:
                    {
                        pitch = br.ReadByte();
                        velocity = br.ReadByte();
                        break;
                    }
                case CSEQEvent.EndTrack:
                    {
                        break;
                    }

                default:
                    {
                        evt = CSEQEvent.Error;
                        Console.Write(op.ToString("X2") + " not recognized at " + br.HexPos() + "\r\n");
                        break;
                    }
            }
        }

        public List<MidiEvent> ToMidiEvent(int absTime, int channel, CSEQ seq, CTrack ct)
        {
            List<MidiEvent> events = new List<MidiEvent>();
            //TrackPatch tp = new TrackPatch();

            absTime += wait;

            //we can't go beyond 16 with midi
            channel = (channel <= 16) ? channel : 16;

            if (CSEQ.IgnoreVolume)
                velocity = 127;

            var p = pitch;

            if (CSEQ.PatchMidi)
            {
                if (ct.isDrumTrack)
                {
                    if (evt == CSEQEvent.NoteOn || evt == CSEQEvent.NoteOff)
                    {
                        p = (byte)seq.samples[pitch].MIDI;
                        //p = (byte)seq.shortSamples[pitch].info.Key;
                    }
                }

                else
                {
                    if (evt == CSEQEvent.ChangePatch)
                    {
                        CSEQ.ActiveInstrument = pitch;
                        p = (byte)seq.samplesReverb[pitch].MIDI;
                        //p = (byte)seq.longSamples[CSEQ.ActiveInstrument].info.Midi;
                    }
                    else if (evt == CSEQEvent.NoteOn || evt == CSEQEvent.NoteOff)
                    {
                        try
                        {
                            p += (byte)seq.samplesReverb[pitch].Pitch;
                            //p += (byte)seq.longSamples[CSEQ.ActiveInstrument].info.Pitch;
                        }
                        catch //(Exception ex)
                        {
                        }
                    }
                }
            }

            switch (evt)
            {
                case CSEQEvent.NoteOn: events.Add(new NoteEvent(absTime, channel, MidiCommandCode.NoteOn, p, velocity)); break;
                case CSEQEvent.NoteOff: events.Add(new NoteEvent(absTime, channel, MidiCommandCode.NoteOff, p, velocity)); break;

                case CSEQEvent.ChangePatch:
                    // events.Add(new ControlChangeEvent(absTime, channel, MidiController.MainVolume, seq.longSamples[pitch].velocity / 2));
                    events.Add(new PatchChangeEvent(absTime, channel, p));
                    break;

                case CSEQEvent.BendAssume: events.Add(new PitchWheelChangeEvent(absTime, channel, p * 64)); break;
                case CSEQEvent.PanAssume: events.Add(new ControlChangeEvent(absTime, channel, MidiController.Pan, p / 2)); break;
                case CSEQEvent.VelAssume: events.Add(new ControlChangeEvent(absTime, channel, MidiController.MainVolume, p / 2)); break; //not really used

                //case CSEQEvent.EndTrack2:
                case CSEQEvent.EndTrack: events.Add(new MetaEvent(MetaEventType.EndTrack, 0, absTime)); break;
            }

            return events;
        }

        public static Command FromMidiEvent(MidiEvent midi)
        {
            Command cmd = new Command();
            cmd.absoluteTime = (int)midi.AbsoluteTime;

            switch (midi.CommandCode)
            {
                case MidiCommandCode.NoteOn:
                    {
                        cmd.evt = CSEQEvent.NoteOn;

                        var x = (NoteEvent)midi;

                        if (x.NoteNumber > 255)
                            throw new Exception("note too large!");

                        cmd.pitch = (byte)x.NoteNumber;

                        if (x.Velocity > 127)
                        {
                            cmd.velocity = 127;
                        }
                        else
                        {
                            cmd.velocity = (byte)x.Velocity;
                        }

                        cmd.wait = x.DeltaTime;

                        break;
                    }
                case MidiCommandCode.NoteOff:
                    {
                        cmd.evt = CSEQEvent.NoteOff;

                        var x = (NoteEvent)midi;

                        if (x.NoteNumber > 255)
                            throw new Exception("note too large!");

                        cmd.pitch = (byte)x.NoteNumber;

                        if (x.Velocity * 2 > 255)
                        {
                            cmd.velocity = 255;
                        }
                        else
                        {
                            cmd.velocity = (byte)(x.Velocity * 2);
                        }

                        cmd.wait = 0;

                        break;
                    }
                default:
                    Helpers.Panic("Command", PanicType.Warning, $"Unimplemented MIDI event: {midi.CommandCode}");
                    break;
            }

            return cmd;
        }

        public override string ToString()
        {
            return String.Format("{0}t - {1}[p:{2}, v:{3}]\r\n", wait, evt.ToString(), pitch, velocity);
        }


        public void WriteBytes(BinaryWriterEx bw)
        {

            int value = wait;

            int buffer = value & 0x7F;

            while (value != (value >> 7))
            {
                value = value >> 7;
                buffer <<= 8;
                buffer |= ((value & 0x7F) | 0x80);
            }

            while (true)
            {
                bw.Write((byte)buffer);
                if ((buffer & 0x80) > 0)
                {
                    buffer >>= 8;
                }
                else
                {
                    break;
                }
            }


            bw.Write((byte)evt);

            switch (evt)
            {
                case CSEQEvent.Unknown4:
                case CSEQEvent.Unknown8:
                case CSEQEvent.EndTrack2:
                case CSEQEvent.ChangePatch:
                case CSEQEvent.BendAssume:
                case CSEQEvent.VelAssume:
                case CSEQEvent.PanAssume:
                case CSEQEvent.NoteOff:
                    {
                        bw.Write((byte)pitch);
                        break;
                    }

                case CSEQEvent.NoteOn:
                    {
                        bw.Write((byte)pitch);
                        bw.Write((byte)velocity);
                        break;
                    }

                case CSEQEvent.EndTrack:
                    {
                        break;
                    }
            }
        }
    }
}
