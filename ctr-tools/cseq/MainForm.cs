using CTRtools.Helpers;
using CTRFramework.Sound;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

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
            treeView1.Nodes.Clear();

            TreeNode tn = new TreeNode("SampleDef");

            foreach (SampleDef sd in seq.samples)
            {
                TreeNode tn1 = new TreeNode(sd.Tag + "");
                tn.Nodes.Add(tn1);
            }

            TreeNode tn2 = new TreeNode("SampleDefReverb");

            foreach (SampleDefReverb sd in seq.samplesReverb)
            {
                TreeNode tn3 = new TreeNode(sd.Tag + "");
                tn2.Nodes.Add(tn3);
            }

            treeView1.Nodes.Add(tn2);
            treeView1.Nodes.Add(tn);

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
                //seq.ToSFZ(Path.ChangeExtension(sfd.FileName, ".sfz"));
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

                    MessageBox.Show(seq.CheckBankForSamples() ? "samples OK!" : "samples missing");

                    textBox1.Text = seq.ListMissingSamples();

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
                    bnk.ExportAll(Path.GetDirectoryName(ofd.FileName));
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
            VagHeader.DefaultSampleRate = 11025;
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            VagHeader.DefaultSampleRate = 22050;
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            VagHeader.DefaultSampleRate = 33075;
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            VagHeader.DefaultSampleRate = 44100;
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


            MetaInst info = CTRJson.GetMetaInst(track, instr, id);
            textBox1.Text += "" + info.Midi + " " + info.Key + " " + info.Title + " " + info.Pitch;
        }

        private void patchMIDIInstrumentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CSEQ.PatchMidi = patchMIDIInstrumentsToolStripMenuItem.Checked;
        }

        private void exportSamplesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (seq != null)
                seq.ExportSamples();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CSEQ.PatchName = comboBox1.Items[comboBox1.SelectedIndex].ToString();
        }

        private void ignoreOriginalVolumeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CSEQ.IgnoreVolume = ignoreOriginalVolumeToolStripMenuItem.Checked;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView1.SelectedNode.Parent != null)
            {
                if (treeView1.SelectedNode.Parent.Index == 1)
                {
                    propertyGrid1.SelectedObject = seq.samples[treeView1.SelectedNode.Index];
                }

                if (treeView1.SelectedNode.Parent.Index == 0)
                {
                    propertyGrid1.SelectedObject = seq.samplesReverb[treeView1.SelectedNode.Index];
                }
            }
            else
            {
                propertyGrid1.SelectedObject = null;
            }
        }

        private void trackBox_DoubleClick(object sender, EventArgs e)
        {
            int x = sequenceBox.SelectedIndex;
            int y = trackBox.SelectedIndex;

            if (x != -1 && y != -1)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "MIDI File (*.mid)|*.mid";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    seq.sequences[x].tracks[y].Import(ofd.FileName);
                }
            }

            tabControl1.SelectedIndex = 0;
        }
    }
}
