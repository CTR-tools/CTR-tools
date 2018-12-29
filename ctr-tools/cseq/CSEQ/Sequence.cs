using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            //textBox1.Text += header.ToString();

            List<short> offsets = new List<short>();

            for (int i = 0; i < header.trackNum; i++)
                offsets.Add(br.ReadInt16());

            br.ReadInt16();
            br.ReadInt16();

            int trackData = (int)br.BaseStream.Position;

            //System.Windows.Forms.MessageBox.Show(csh.trackNum + "");

            for (int i = 0; i < header.trackNum; i++)
            {

                // textBox1.AppendText("\r\nTrack " + i + "\r\n");

                br.BaseStream.Position = trackData + offsets[i];

                CTrack t = new CTrack();
                t.Read(br);

                tracks.Add(t);

            }
        }


        public byte[] Reverse(ushort x)
        {
            byte[] data = BitConverter.GetBytes(x).Reverse().ToArray();
            return data;
        }


        public void ExportMIDI(string fn)
        {
            using (BinaryWriter bw = new BinaryWriter(File.Create(fn)))
            {
                if (tracks.Count > 16)
                {
                    System.Windows.Forms.MessageBox.Show(tracks.Count + " tracks!!!");
                    return;
                }

                bw.Write(System.Text.Encoding.ASCII.GetBytes("MThd"));
                bw.Write(Reverse(6));
                bw.Write(Reverse(1));
                bw.Write(Reverse(96));

                for (int i = 0; i < tracks.Count; i++)
                    tracks[i].ExportMIDI(bw, i + 1);
            }
        }


    }
}