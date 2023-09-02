using CTRFramework.Shared;
using NAudio.Midi;
using System;
using System.Collections.Generic;

namespace CTRFramework.Sound
{
    public enum CseqEventType
    {
        Terminator = 0,
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
        //this isnt an opcode.
        Error = 0xFF
    }


    public class CseqEvent : IRead
    {
        public CseqEventType eventType = CseqEventType.Error;
        public byte pitch = 0;
        public byte velocity = 0;
        public int wait = 0;

        public int absoluteTime = 0;

        public CseqEvent()
        {
        }

        public CseqEvent(BinaryReaderEx br)
        {
            Read(br);
        }

        public static CseqEvent FromReader(BinaryReaderEx br)
        {
            return new CseqEvent(br);
        }

        public void Read(BinaryReaderEx br)
        {
            wait = br.ReadTimeDelta();
            eventType = (CseqEventType)br.ReadByte();

            switch (eventType)
            {
                case CseqEventType.Unknown4:
                case CseqEventType.Unknown8:
                    {
                        pitch = br.ReadByte();
                        Helpers.Panic(this, PanicType.Assume, $"{eventType} found at {br.HexPos()}");
                        break;
                    }

                case CseqEventType.EndTrack2:
                case CseqEventType.ChangePatch:
                case CseqEventType.BendAssume:
                case CseqEventType.VelAssume:
                case CseqEventType.PanAssume:
                case CseqEventType.NoteOff:
                    {
                        pitch = br.ReadByte();
                        break;
                    }
                case CseqEventType.NoteOn:
                    {
                        pitch = br.ReadByte();
                        velocity = br.ReadByte();
                        break;
                    }
                case CseqEventType.EndTrack:
                case CseqEventType.Terminator:
                    {
                        break;
                    }

                default:
                    {
                        eventType = CseqEventType.Error;
                        Helpers.Panic(this, PanicType.Warning, $"{eventType} not recognized at  {br.HexPos()}");
                        break;
                    }
            }
        }

        public List<MidiEvent> ToMidiEvent(int absTime, int channel, Cseq seq, CSeqTrack ct)
        {
            var events = new List<MidiEvent>();
            //TrackPatch tp = new TrackPatch();

            absTime += wait;

            //we can't go beyond 16 with midi
            channel = (channel <= 16) ? channel : 16;

            if (Cseq.IgnoreVolume)
                velocity = 127;

            var p = pitch;

            if (Cseq.PatchMidi)
            {
                if (ct.isDrumTrack)
                {
                    if (eventType == CseqEventType.NoteOn || eventType == CseqEventType.NoteOff)
                    {
                        p = (byte)seq.samples[pitch].metaInst.Key;
                    }
                }
                else
                {
                    if (eventType == CseqEventType.ChangePatch)
                    {
                        Cseq.ActiveInstrument = pitch;
                        p = (byte)seq.samplesReverb[pitch].metaInst.Midi;
                    }
                    else if (eventType == CseqEventType.NoteOn || eventType == CseqEventType.NoteOff)
                    {
                        try
                        {
                            p += (byte)seq.samplesReverb[Cseq.ActiveInstrument].metaInst.Pitch;
                        }
                        catch (Exception ex)
                        {
                            Helpers.Panic(this, PanicType.Error, ex.Message);
                            //System.Windows.Forms.MessageBox.Show("" + seq.samplesReverb.Count + " " + p);
                        }
                    }
                }
            }

            switch (eventType)
            {
                case CseqEventType.NoteOn: events.Add(new NoteEvent(absTime, channel, MidiCommandCode.NoteOn, p, velocity)); break;
                case CseqEventType.NoteOff: events.Add(new NoteEvent(absTime, channel, MidiCommandCode.NoteOff, p, velocity)); break;

                case CseqEventType.ChangePatch:
                    // events.Add(new ControlChangeEvent(absTime, channel, MidiController.MainVolume, seq.longSamples[pitch].velocity / 2));
                    events.Add(new PatchChangeEvent(absTime, channel, p));
                    break;

                case CseqEventType.BendAssume: events.Add(new PitchWheelChangeEvent(absTime, channel, p * 64)); break;
                case CseqEventType.PanAssume: events.Add(new ControlChangeEvent(absTime, channel, MidiController.Pan, p / 2)); break;
                case CseqEventType.VelAssume: events.Add(new ControlChangeEvent(absTime, channel, MidiController.MainVolume, p / 2)); break; //not really used

                //case CSEQEvent.EndTrack2:
                case CseqEventType.EndTrack: events.Add(new MetaEvent(MetaEventType.EndTrack, 0, absTime)); break;
            }

            return events;
        }

        public static CseqEvent FromMidiEvent(MidiEvent midi)
        {
            var cmd = new CseqEvent();
            cmd.absoluteTime = (int)midi.AbsoluteTime;

            switch (midi.CommandCode)
            {
                case MidiCommandCode.NoteOn:
                    {
                        cmd.eventType = CseqEventType.NoteOn;

                        var x = (NoteEvent)midi;

                        if (x.NoteNumber > 255)
                            throw new Exception("note too large!");

                        cmd.pitch = (byte)x.NoteNumber;
                        cmd.velocity = x.Velocity > 127 ? (byte)127 : (byte)x.Velocity;
                        cmd.wait = x.DeltaTime;

                        break;
                    }
                case MidiCommandCode.NoteOff:
                    {
                        cmd.eventType = CseqEventType.NoteOff;

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

                        cmd.wait = x.DeltaTime;

                        break;
                    }
                case MidiCommandCode.ControlChange:
                    {
                        var control = midi as ControlChangeEvent;

                        switch (control.Controller)
                        {
                            //balance
                            case MidiController.Pan:
                                {
                                    cmd.eventType = CseqEventType.PanAssume;
                                    cmd.pitch = (byte)(control.ControllerValue / 127f * 255f);

                                    break;
                                }

                            //volume
                            case MidiController.MainVolume:
                                {
                                    cmd.eventType = CseqEventType.VelAssume;
                                    cmd.pitch = (byte)(control.ControllerValue / 127f * 255f);

                                    break;
                                }
                            default:
                                Helpers.Panic("Command", PanicType.Warning, $"Unimplemented MIDI controller: {control.Controller}");
                                return null;
                        }

                        break;
                    }

                default:
                    Helpers.Panic("Command", PanicType.Warning, $"Unimplemented MIDI event: {midi.CommandCode}");
                    return null;
            }

            return cmd;
        }

        public override string ToString() => $"[{absoluteTime}]\t{wait}t - {eventType}[p:{pitch}, v:{velocity}]";

        public void Write(BinaryWriterEx bw)
        {
            bw.WriteTimeDelta((uint)wait);
            bw.Write((byte)eventType);

            switch (eventType)
            {
                case CseqEventType.Unknown4:
                case CseqEventType.Unknown8:
                case CseqEventType.EndTrack2:
                case CseqEventType.ChangePatch:
                case CseqEventType.BendAssume:
                case CseqEventType.VelAssume:
                case CseqEventType.PanAssume:
                case CseqEventType.NoteOff:
                    {
                        bw.Write((byte)pitch);
                        break;
                    }

                case CseqEventType.NoteOn:
                    {
                        bw.Write((byte)pitch);
                        bw.Write((byte)velocity);
                        break;
                    }

                case CseqEventType.EndTrack:
                    {
                        break;
                    }
            }
        }
    }
}
