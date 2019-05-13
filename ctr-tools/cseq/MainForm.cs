using System;
using System.Windows.Forms;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace cseq
{
    public partial class MainForm : Form
    {
        public static bool finalLap = false;
        public CSEQ seq;

        string loadedfile = "";

        public MainForm()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        private void ReadCSEQ(string fn)
        {
            Log.Clear();
            sequenceBox.Items.Clear();
            trackBox.Items.Clear();

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

            foreach (Sample s in seq.longSamples)
                s.ToDataRow(samples);

            foreach (Sample s in seq.shortSamples)
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


        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void sequenceBox_DoubleClick(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "MIDI File (*.mid)|*.mid";
            sfd.FileName = Path.ChangeExtension(loadedfile, ".mid");

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                seq.sequences[sequenceBox.SelectedIndex].ExportMIDI(sfd.FileName);
            }

        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files.Length > 0)
                ReadCSEQ(files[0]);
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void skipBytesForUSDemoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CSEQ.usdemo = skipBytesForUSDemoToolStripMenuItem.Checked;
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
                bnk = new Bank(ofd.FileName);
                textBox1.Text = bnk.ToString();

                if (seq != null)
                {
                    MessageBox.Show(seq.CheckBankForSamples(bnk) ? "samples OK!" : "samples missing.");

                    foreach (VABSample v in bnk.vs)
                    {
                        v.frequency = seq.GetFrequencyBySampleID(v.id);
                    }

                    foreach (int i in seq.GetAllIDs())
                    {
                        bnk.Export(i);
                    }
                }
                else
                {
                    bnk.ExportAll();
                }
            }
        }

        private void tutorialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Text =
                "CSEQ Tool\r\n2018-2019, DCxDemo*.\r\n\r\n" +
                "This tool reads CSEQ files - custom Crash Team Racing music files.\r\n" +
                "CSEQ files are contained in KART.HWL file, use howl tool to extract bank/sequence files.\r\n" +
                "Use File -> Open to locate your CSEQ file.\r\n" +
                "For NTSC-U Demo make sure to tick Options -> Skip bytes for US demo.\r\n" +
                "Double click sequence on the list to export it to MIDI.\r\n" +
                "Use Instruments / Samples tab to check instrument values (mostly for research).\r\n" +
                "Click track on the list to output all commands on that track.\r\n" +
                "Use Options -> Load bank to find the correct bank file for the sequence loaded. Correct file will end up in Samples OK message (canyon starts with 01).\r\n\r\n" +
                "Special thanks:\r\nlnge - initial CSEQ specification\r\nCREEEE - testing and suggestions";
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            VABSample.defaultrate = 11025;
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            VABSample.defaultrate = 22050;
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            VABSample.defaultrate = 33075;
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            VABSample.defaultrate = 44100;
        }
    }
}
