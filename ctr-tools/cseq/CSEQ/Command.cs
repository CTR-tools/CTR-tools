using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NAudio.Midi;

namespace cseq
{
    public enum CSEQEvent
    {
        Unknown0 = 0x00,
        NoteOff = 0x01,
        Unknown2 = 0x02,
        EndOfTrack = 0x03,
        Unknown4 = 0x04,
        NoteOn = 0x05,
        Unknown6 = 0x06,
        Unknown7 = 0x07,
        Unknown8 = 0x08,
        Unknown9 = 0x09,
        UnknownA = 0x0A,
        EndOfKek = 666
    }


    public class Command
    {
        public CSEQEvent evt;

        public byte pitch;
        public byte velocity;
        public int wait;

        private int ReadTimeDelta(BinaryReader br)
        {

            int time = 0;
            byte next = 0;

            byte cnt = 0;

            int ttltime = 0;

            do
            {
                byte x = br.ReadByte();

                time = (byte)(x & 0x7F);
                next = (byte)(x & 0x80);

                ttltime = (ttltime << (cnt * 7) ) | time;

                /*
                if (cnt > 0)
                System.Windows.Forms.MessageBox.Show(String.Format(
                    "initial value: {0}\r\n" +
                    "time this byte: {1}\r\n" + 
                    "next: {2}\r\n" +
                    "ttltime: {3}",
                    x, time, next, ttltime

                    ));
            */
                cnt++;
            }
            while (next != 0);

            return ttltime;

        }

        public void Read(BinaryReaderEx br)
        {
            wait = ReadTimeDelta(br);

            byte op = br.ReadByte();

            evt = (CSEQEvent)op;

            switch (evt)
            {
                case CSEQEvent.Unknown0:
                case CSEQEvent.Unknown2:
                    {
                        break;
                    }
                case CSEQEvent.Unknown4:
                case CSEQEvent.Unknown6:
                case CSEQEvent.Unknown7:
                case CSEQEvent.Unknown8:
                case CSEQEvent.Unknown9:
                case CSEQEvent.UnknownA:
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
                case CSEQEvent.EndOfTrack:
                    {
                        break;
                    }

                default:
                    {
                        evt = CSEQEvent.EndOfKek;
                        Log.Write(op.ToString("X2") + " not recognized at " + br.HexPos() + "\r\n");
                        break;
                        //throw new System.NotImplementedException(String.Format("Not implemented opcode: {0} at 0x{1}", op, br.BaseStream.Position.ToString("X8") ));
                    }
            }
        }


        public MidiMessage GetMIDIMessage(int tracknum)
        {
            switch (evt)
            {
                case CSEQEvent.NoteOn: return MidiMessage.StartNote(pitch, velocity, tracknum);
                case CSEQEvent.NoteOff: return MidiMessage.StopNote(pitch, 127, tracknum);
                default: return null;
            }
        }


        
        public void PitchShift(int i)
        {
            int newPitch = pitch + i;

            if (newPitch >= 0 && newPitch <= 127)
            {
                pitch = (byte)newPitch;
            }
        }


        public override string ToString()
        {
            return String.Format("{0}t - {1}[p:{2}, v:{3}]\r\n", wait, evt.ToString(), pitch, velocity);
        }
    }
}
