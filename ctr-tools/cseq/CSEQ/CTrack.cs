using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio.Midi;
using System.Diagnostics;
using System.IO;

namespace cseq
{
    public class CTrack
    {
        List<Command> cmd;

        public CTrack()
        {
            cmd = new List<Command>();
        }


        public void Play(CSeqHeader header)
        {
            Form1.midiOut.Send(MidiMessage.ChangeControl(123, 0, 1).RawData);

            //textBox1.AppendText(cx.ToString());
            System.Windows.Forms.Application.DoEvents();

            //if (i > 0)
            foreach (Command cx in cmd)
            {

                if (cx.wait > 1000) cx.wait = 1000;

                NOP(cx.wait * header.tickDuration() / 1000);

                if (cx.evt == CSEQEvent.NoteOn)
                {
                    Form1.midiOut.Send(MidiMessage.StartNote(cx.pitch, cx.velocity, isDrum ? 10 : 1).RawData);
                }

                if (cx.evt == CSEQEvent.NoteOff)
                {
                    Form1.midiOut.Send(MidiMessage.StopNote(cx.pitch, 127, isDrum ? 10 : 1).RawData);
                }


                //System.Threading.Thread.Sleep((int)(cx.wait * mult));

            }
        }


        private static void NOP(double durationSeconds)
        {
            var durationTicks = Math.Round(durationSeconds * Stopwatch.Frequency);
            var sw = Stopwatch.StartNew();

            while (sw.ElapsedTicks < durationTicks)
            {
            }
        }


        public void ExportMIDI(BinaryWriter bw, int tracknum)
        {
            bw.Write(System.Text.Encoding.ASCII.GetBytes("MTrk"));

            foreach (Command c in cmd)
            {
                MidiMessage msg = c.GetMIDIMessage(tracknum);
                if (msg != null) bw.Write(msg.RawData);
            }
        }



        public void Read(BinaryReaderEx br)
        {
            Command cx;

            do
            {
                cx = new Command();
                cx.Read(br);
                cmd.Add(cx);
            }

            while (cx.evt != CSEQEvent.EndOfTrack);

            // PitchShift(-12);

            maybeDrumTrack();

            if (isDrum)
            {
                ConvertToDrumTrack();
            }

        }

        public bool isDrum = false;

        public bool maybeDrumTrack()
        {
            if (!isDrum)
            {
                foreach (Command cx in cmd)
                {
                    if (cx.pitch > 10 && (cx.evt == CSEQEvent.NoteOn || cx.evt == CSEQEvent.NoteOff))
                    {
                        isDrum = false;
                        return isDrum;
                    }
                }

                isDrum = true;
                return isDrum;
            }

            return isDrum;
        }

        public void PitchShift(int i)
        {
            foreach (Command c in cmd)
            {
                c.PitchShift(i);
            }
        }


        public void ConvertToDrumTrack()
        {
            for (int i = 0; i < cmd.Count; i++)
            {
                cmd[i].pitch += 36;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (Command c in cmd)
                sb.Append(c.ToString());

            return sb.ToString();
        }

    }
}
