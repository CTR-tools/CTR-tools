using System;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Collections.Generic;
using CTRtools.SFX;
using CTRtools.Helpers;
using Newtonsoft.Json.Linq;

namespace CTRtools.CSEQ
{
    public partial class MainForm : Form
    {
        public CSEQ seq;

        //??
        string loadedfile = "";


        public MainForm()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

            if (!CTRJson.Load())
            {
                MessageBox.Show("Couldn't load Json!");
            }
            else
            {
                foreach (KeyValuePair<string, JToken> j in CTRJson.midi)
                {
                    comboBox1.Items.Add(j.Key);
                }
            }
        }

        private void ReadCSEQ(string fn)
        {
            Log.Clear();
            sequenceBox.Items.Clear();
            trackBox.Items.Clear();

            string name = Path.GetFileNameWithoutExtension(fn);

            for (int i = 0; i < comboBox1.Items.Count; i++)
            {
                if (name.Contains(comboBox1.Items[i].ToString()))
                  comboBox1.SelectedIndex = i;
            }

            seq = new CSEQ();

            if (seq.Read(fn, textBox1))
            {
                FillUI(fn);
            }
            else
            {
                MessageBox.Show("Failed to read CTR sequence!");
            }
        }

        private void FillUI(string fn)
        {
            loadedfile = Path.GetFileName(fn);
            this.Text = "CTR CSEQ - " + loadedfile;

            textBox2.Text = seq.header.ToString();

            int i = 0;
            sequenceBox.Items.Clear();

            foreach (Sequence s in seq.sequences)
            {
                sequenceBox.Items.Add("Sequence_" + i.ToString("X2"));
                i++;
            }

            textBox1.Text = Log.Read();

            DataSet ds = new DataSet();
            DataTable samples = new DataTable("samples");

            samples.Columns.Add("insttype", typeof(string));
            samples.Columns.Add("magic1", typeof(byte));
            samples.Columns.Add("velocity", typeof(byte));
            samples.Columns.Add("always0", typeof(short));
            samples.Columns.Add("basepitch", typeof(short));
            samples.Columns.Add("sampleID", typeof(string));
            samples.Columns.Add("unknown_80FF", typeof(string));
            samples.Columns.Add("reverb", typeof(byte));
            samples.Columns.Add("reverb2", typeof(byte));

            foreach (Instrument s in seq.longSamples)
                s.ToDataRow(samples);

            foreach (Instrument s in seq.shortSamples)
                s.ToDataRow(samples);

            ds.Tables.Add(samples);

            dataGridView1.DataSource = ds.Tables["samples"];

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Crash Team Racing CSEQ (*.cseq)|*.cseq";

            if (ofd.ShowDialog() == DialogResult.OK)
                ReadCSEQ(ofd.FileName);
        }

        private void sequenceBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            trackBox.Items.Clear();

            int i = 0;

            foreach (CTrack c in seq.sequences[sequenceBox.SelectedIndex].tracks)
            {
                trackBox.Items.Add(c.name);
                i++;
            }

            tabControl1.SelectedIndex = 1;
        }

        private void trackBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int x = sequenceBox.SelectedIndex;
            int y = trackBox.SelectedIndex;

            if (x != -1 && y != -1)
                this.textBox1.Text = seq.sequences[x].tracks[y].ToString();

            tabControl1.SelectedIndex = 0;
        }



        private void sequenceBox_DoubleClick(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "MIDI File (*.mid)|*.mid";
            sfd.FileName = Path.ChangeExtension(loadedfile, ".mid");

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                seq.sequences[sequenceBox.SelectedIndex].ExportMIDI(sfd.FileName, seq);
                seq.ToSFZ(Path.ChangeExtension(sfd.FileName, ".sfz"));
            }

        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files.Length > 0)
                ReadCSEQ(files[0]);
        }


        private void exportSEQToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (seq != null)
                seq.Export("test.cseq");
        }


        Bank bnk = new Bank();

        private void loadBankToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CTR Sound Bank (*.bnk)|*.bnk";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (seq != null)
                {
                    seq.LoadBank(ofd.FileName);

                    MessageBox.Show(seq.CheckBankForSamples() ? "samples OK!" : "samples missing.");

                    /*
                    foreach (VABSample v in bnk.vs)
                    {
                        v.frequency = seq.GetFrequencyBySampleID(v.id);
                    }
                   
                    foreach (int i in seq.GetAllIDs())
                    {
                        bnk.Export(i);
                    }
                     */
                }
                else
                {
                    bnk.ExportAll();
                }
            }
        }



        #region [Form stuff]

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void skipBytesForUSDemoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CSEQ.USdemo = skipBytesForUSDemoToolStripMenuItem.Checked;
        }

        private void tutorialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Text = Properties.Resources.help;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            VAG.DefaultSampleRate = 11025;
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            VAG.DefaultSampleRate = 22050;
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            VAG.DefaultSampleRate = 33075;
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            VAG.DefaultSampleRate = 44100;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #endregion

        private void testJsonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string track = "canyon";
            string instr = "long";
            int id = 0;


            MetaInst info  = CTRJson.GetMetaInst(track, instr, id);
            textBox1.Text += "" + info.Midi + " " + info.Key + " " + info.Title + " " + info.Pitch;
        }

        private void patchMIDIInstrumentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CSEQ.PatchMidi = patchMIDIInstrumentsToolStripMenuItem.Checked;
        }

        private void exportSamplesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (seq != null)
                if (seq.bank != null)
                    seq.bank.ExportAll();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CSEQ.PatchName = comboBox1.Items[comboBox1.SelectedIndex].ToString();
        }

        private void ignoreOriginalVolumeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CSEQ.IgnoreVolume = ignoreOriginalVolumeToolStripMenuItem.Checked;
        }
    }
}
