using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using NAudio;
using NAudio.Midi;
using System.Threading;

namespace cseq
{

    public partial class Form1 : Form
    {
        public static MidiOut midiOut;
        public CSEQ seq;

        public Form1()
        {
            InitializeComponent();
        }

        public void SkipToSong(BinaryReader br, int n)
        {
            for (int i = 0; i < n; i++)
            {
                int x = br.ReadInt32();
                x += x % 0x800 - 4;
            }
        }



        private void ReadCSEQ(string fn)
        {
            Log.Clear();

            sequenceBox.Items.Clear();
            trackBox.Items.Clear();


            seq = new CSEQ();

            if (seq.Read(fn, textBox1))
            {
                textBox2.Text = seq.header.ToString();

                int i = 0;
                sequenceBox.Items.Clear();

                foreach (Sequence s in seq.sequences)
                {
                    sequenceBox.Items.Add("Sequence_" + i.ToString("X2"));
                    i++;
                }

                textBox1.Text = Log.Read();

            }
            else
            {
                MessageBox.Show("Failed to read CTR sequence!");
            }
        }



        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Crash Team Racing CSEQ (*.cseq)|*.cseq";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                ReadCSEQ(ofd.FileName);
            }
        }

        private void sequenceBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            trackBox.Items.Clear();

            int i = 0;

            foreach (CTrack c in seq.sequences[sequenceBox.SelectedIndex].tracks)
            {
                trackBox.Items.Add("Track_" + i.ToString("X2"));
                i++;
            }

            textBox1.Text = seq.ListSamples();

        }

        Thread player;

        private void trackBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int x = sequenceBox.SelectedIndex;
            int y = trackBox.SelectedIndex;

            if (x != -1 && y != -1)
            {

                if (player != null)
                    if (player.IsAlive)
                        player.Abort();

                this.textBox1.Text = seq.sequences[x].tracks[y].ToString();

                player = new Thread(new ThreadStart(() => seq.sequences[x].tracks[y].Play(seq.sequences[x].header)));
                player.Start();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (player != null)
                if (player.IsAlive)
                {
                    player.Interrupt();
                    player.Abort();
                    player = null;
                }

            if (Form1.midiOut != null)
            {
                Form1.midiOut.Send(MidiMessage.ChangeControl(123, 0, 1).RawData);
                Form1.midiOut = null;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void sequenceBox_DoubleClick(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "MIDI File (*.mid)|*.mid";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                seq.sequences[sequenceBox.SelectedIndex].ExportMIDI(sfd.FileName);
            }

        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files.Length > 0)
            {
                ReadCSEQ(files[0]);
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }
    }
}
