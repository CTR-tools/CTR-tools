using System;
using System.IO;
using NAudio.Midi;

namespace cseq
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
        public CSEQEvent evt;
        public byte pitch;
        public byte velocity;
        public int wait;

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
                        Log.Write(op.ToString("X2") + " found at " + br.HexPos() + "\r\n");
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
                        Log.Write(op.ToString("X2") + " not recognized at " + br.HexPos() + "\r\n");
                        break;
                        //throw new System.NotImplementedException(String.Format("Not implemented opcode: {0} at 0x{1}", op, br.BaseStream.Position.ToString("X8") ));
                    }
            }
        }

        public MidiEvent ToMidiEvent(int absTime, int channel)
        {
            TrackPatch tp = new TrackPatch();

            //we can't go beyond 16 with midi
            channel = (channel <= 16) ? channel : 16;

            switch (evt)
            {
                case CSEQEvent.NoteOn:      return new NoteEvent(absTime + wait, channel, MidiCommandCode.NoteOn, channel != 10 ? pitch : tp.SwapDrum(pitch), velocity);
                case CSEQEvent.NoteOff:     return new NoteEvent(absTime + wait, channel, MidiCommandCode.NoteOff, channel != 10 ? pitch : tp.SwapDrum(pitch), velocity);
                case CSEQEvent.ChangePatch: return new PatchChangeEvent(absTime + wait, channel, channel == 10 ? pitch : tp.SwapPatch(pitch));
                case CSEQEvent.EndTrack2:
                case CSEQEvent.EndTrack:    return new MetaEvent(MetaEventType.EndTrack, 0, absTime);
                case CSEQEvent.BendAssume:  return new PitchWheelChangeEvent(absTime + wait, channel, pitch * 64);
                case CSEQEvent.PanAssume:   return new ControlChangeEvent(absTime + wait, channel, MidiController.Pan, pitch / 2);
                case CSEQEvent.VelAssume:   return new ControlChangeEvent(absTime + wait, channel, MidiController.MainVolume, pitch / 2);
                default: return null;
            }
        }

        public override string ToString()
        {
            return String.Format("{0}t - {1}[p:{2}, v:{3}]\r\n", wait, evt.ToString(), pitch, velocity);
        }


        public void WriteBytes(BinaryWriter bw)
        {
            //bw.Write((byte)0); //time delta here!

            /*
            int time = 0;
            int ttltime = wait;

            do
            {
                time = (byte)(ttltime & 0x7F);
                ttltime = ttltime >> 7;

                if (ttltime > 0)
                {
                    time = time & 0x80;
                }

                bw.Write((byte)time);

            }
            while (ttltime > 0);
            */

            int value = wait;

            int buffer = value & 0x7F;

            while ( value != (value >> 7) )
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
