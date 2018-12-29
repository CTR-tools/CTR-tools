using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NAudio.Midi;
using NAudio;
using System.Diagnostics;
using System.Threading;

namespace cseq
{
    public class CSEQ
    {
        public CHeader header;
        public List<Sample> longSamples;
        public List<Sample> shortSamples;
        public List<Sequence> sequences;

        public CSEQ()
        {
            header = new CHeader();
            longSamples = new List<Sample>();
            shortSamples = new List<Sample>();
            sequences = new List<Sequence>();
        }


        public bool Read(string s, System.Windows.Forms.TextBox textBox1)
        {
            
            if (Form1.midiOut == null)
                Form1.midiOut = new MidiOut(0);

            Form1.midiOut.Send(MidiMessage.ChangePatch(34, 1).RawData);



            BinaryReaderEx br = BinaryReaderEx.FromFile(s);

            if (!header.Read(br)) return false;

            for (int i = 0; i < header.chunk12cnt; i++)
            {
                Sample ls = new Sample();
                ls.Read(br, InstType.Long);
                longSamples.Add(ls);
            }

            for (int i = 0; i < header.chunk8cnt; i++)
            {
                Sample ss = new Sample();
                ss.Read(br, InstType.Short);
                longSamples.Add(ss);
            }

            // MessageBox.Show("extrainfo at 0x" + br.BaseStream.Position.ToString("X8") );

            //probably this is either multiple tracks, or multiple containers 

            List<short> extraoffsets = new List<short>();

            for (int i = 0; i < header.extrainfo; i++)
                extraoffsets.Add(br.ReadInt16());

            /*
            for (int i = 0; i < header.extrainfo; i++)
                textBox1.AppendText(String.Format("extra for trackchunk {0}: {1}\r\n", i, br.ReadByte()));
            */

            br.ReadInt16();
            br.ReadByte();

            //for (int i = 0; i < ch.extrainfo; i++)br.ReadByte();

            int sequencedata = (int)br.BaseStream.Position;

            for (int i = 0; i < header.extrainfo; i++)
            {
                br.BaseStream.Position = sequencedata + extraoffsets[i];

                Sequence seq = new Sequence();

                seq.Read(br, textBox1);
                sequences.Add(seq);

                //Thread th = new Thread(new ThreadStart(() => seq.Read(br, textBox1)));
                //th.Start();
            }

            return true;
        }

        public string ListSamples()
        {
            StringBuilder sb = new StringBuilder();

            foreach (Sample ls in longSamples)
                sb.Append(ls.ToString() + "\r\n");

            foreach (Sample ss in shortSamples)
                sb.Append(ss.ToString() + "\r\n");

            return sb.ToString();
        }
    }
}
