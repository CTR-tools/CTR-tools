using CTRFramework.Shared;
using CTRFramework.Audio;
using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CTRTools.Controls
{
    public partial class CseqControl : UserControl
    {
        public string samplePath = "";

        public Cseq seq;

        //??
        string loadedfile = "";

        public void SetContext(HowlContext howl)
        {
            instrumentControl1.Context = howl;
        }

        public CseqControl()
        {
            InitializeComponent();
            samplePath = Properties.Settings.Default.samplePath;
            LoadMeta();
        }

        public void LoadMeta()
        {
            if (!Meta.LoadMidiMeta())
            {
                MessageBox.Show("Couldn't load Json!");
            }
            else
            {
                metaInstList.Items.Clear();
                patchBox.Items.Clear();

                metaInstList.Items.AddRange(Meta.GetPatchList().ToArray());
                patchBox.Items.AddRange(Meta.GetPatchList().ToArray());
            }

            //howl.Context.samplenames = Helpers.LoadNumberedList(Meta.SmplPath);
        }

        private void LoadCseq(string filename)
        {
            //Log.Clear();
            sequenceBox.Items.Clear();
            trackBox.Items.Clear();

            string name = Path.GetFileNameWithoutExtension(filename);

            for (int i = 0; i < patchBox.Items.Count; i++)
            {
                if (name.Contains(patchBox.Items[i].ToString()))
                    patchBox.SelectedIndex = i;
            }

            try
            {
                seq = Cseq.FromFile(filename);
                FillUI(filename);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to read CTR sequence!\r\n" + ex.ToString());
            }
        }

        public void FillUI(string filename = "empty", int selA = -1, int selB = -1)
        {
            if (seq is null) return;

            instrumentList.Nodes.Clear();
            sequenceBox.Items.Clear();
            trackBox.Items.Clear();

            seqInfoBox.Text = "";

            loadedfile = Path.GetFileNameWithoutExtension(filename);
            this.Text = "CTR CSEQ - " + filename;

            seqInfoBox.Text = seq.ToString();

            foreach (var song in seq.Songs)
                sequenceBox.Items.Add(song);//"Sequence_" + i.ToString("X2"));

            PopulateInstrumentsNode(instrumentList, "Instruments", seq.Instruments);
            PopulateInstrumentsNode(instrumentList, "Percussion", seq.Percussions);

            instrumentList.ExpandAll();

            if (selA != -1 && selB != -1)
            {
                instrumentList.SelectedNode = instrumentList.Nodes[selA].Nodes[selB];
                instrumentList.Focus();
            }
        }

        private void PopulateInstrumentsNode<T>(TreeView root, string name, List<T> samples) where T : SpuInstrument
        {
            var instruments = new TreeNode(name);

            foreach (var sample in samples)
            {
                var vag = sample.GetSample(sample.SampleID, seq.Context);
                vag.Context = seq.Context;

                var node = new TreeNode(vag.Name);
                node.Tag = sample;
                instruments.Nodes.Add(node);
            }

            root.Nodes.Add(instruments);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() == DialogResult.OK)
                LoadCseq(ofd.FileName);
        }

        private void sequenceBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sequenceBox.SelectedItem is null) return;

            trackBox.Items.Clear();

            foreach (var track in ((CseqSong)sequenceBox.SelectedItem).Tracks)
                trackBox.Items.Add(track);

            tabControl1.SelectedIndex = 1;

            //instrumentInfo.SelectedObject = (CseqSong)sequenceBox.SelectedItem;
        }

        private void trackBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (trackBox.SelectedItem is null) return;

            trackInfoBox.Text = ((CSeqTrack)trackBox.SelectedItem).ListCommands();
            tabControl1.SelectedIndex = 0;
        }

        private void sequenceBox_DoubleClick(object sender, EventArgs e)
        {
            sfd.FileName = Path.ChangeExtension(loadedfile, ".mid");

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                seq.Songs[sequenceBox.SelectedIndex].ExportMIDI(sfd.FileName, seq);
                //seq.ToSFZ(Path.ChangeExtension(sfd.FileName, ".sfz"));
            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files.Length == 0) return;

            switch (Path.GetExtension(files[0]))
            {
                case ".cseq": LoadCseq(files[0]); break;
                // case ".mid": ImportMIDI(files[0]); break;
                case ".bnk": LoadBank(files[0]); tabControl1.SelectedIndex = 3; break;
                default: MessageBox.Show("Unsupported file."); break;
            }
        }

        private void exportSEQToolStripMenuItem_Click(object sender, EventArgs e)
        {
            seq?.Save(Helpers.PathCombine(Meta.BasePath, "test.cseq"));
        }


        public void LoadBank(string filename)
        {
        }


        private void loadBankToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CTR Sound Bank (*.bnk)|*.bnk";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                LoadBank(ofd.FileName);
            }
        }



        #region [Form stuff]

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void tutorialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            trackInfoBox.Text = "halp!";//Properties.Resources.help;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            VagSample.DefaultSampleRate = 11025;
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            VagSample.DefaultSampleRate = 22050;
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            VagSample.DefaultSampleRate = 33075;
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            VagSample.DefaultSampleRate = 44100;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #endregion

        private void testJsonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                string[] files = Directory.GetFiles(fbd.SelectedPath);


                List<string> x = new List<string>();

                foreach (string s in files)
                {
                    Cseq c = Cseq.FromFile(s);

                    foreach (var sd in c.Percussions)
                        if (!x.Contains(sd.Tag))
                            x.Add(sd.Tag);

                    foreach (var sd in c.Instruments)
                        if (!x.Contains(sd.Tag))
                            x.Add(sd.Tag);
                }

                x.Sort();
                StringBuilder sb = new StringBuilder();

                foreach (string s in x) sb.AppendLine(s);

                trackInfoBox.Text = sb.ToString();
            }

            /*
            string track = "canyon";
            string instr = "long";
            int id = 0;


            MetaInst info = CTRJson.GetMetaInst(track, instr, id);
            textBox1.Text += "" + info.Midi + " " + info.Key + " " + info.Title + " " + info.Pitch;
            */
        }

        private void patchMIDIInstrumentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cseq.PatchMidi = patchMIDIInstrumentsToolStripMenuItem.Checked;
        }

        private void exportSamplesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (seq.Context == null) return;

            var fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                seq.ExportSamples(fbd.SelectedPath);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (seq != null)
                seq.PatchName = patchBox.Items[patchBox.SelectedIndex].ToString();
        }

        private void ignoreOriginalVolumeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cseq.IgnoreVolume = ignoreOriginalVolumeToolStripMenuItem.Checked;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string filename = "";

            if (instrumentList.SelectedNode.Parent != null)
            {
                if (instrumentList.SelectedNode.Parent.Index == 1)
                {
                    var sd = instrumentList.SelectedNode.Tag as SpuInstrument;
                    instrumentControl1.Instrument = sd;
                    HowlPlayer.Play(sd);
                }

                if (instrumentList.SelectedNode.Parent.Index == 0)
                {
                    var sd = seq.Instruments[instrumentList.SelectedNode.Index];
                    instrumentControl1.Instrument = sd;
                    HowlPlayer.Play(sd);
                }
            }
            else
            {
                instrumentControl1.Clear();
            }
        }


        private void trackBox_DoubleClick(object sender, EventArgs e)
        {
            int x = sequenceBox.SelectedIndex;
            int y = trackBox.SelectedIndex;

            if (x != -1 && y != -1)
            {
                var ofd = new OpenFileDialog();
                ofd.Filter = "MIDI File (*.mid)|*.mid";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    seq.Songs[x].Tracks[y].Import(ofd.FileName);
                }
            }

            tabControl1.SelectedIndex = 0;
        }

        private void copyInstrumentVolumeToTracksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cseq.UseSampleVolumeForTracks = copyInstrumentVolumeToTracksToolStripMenuItem.Checked;
        }

        // TODO: is this used?? maybe wipe it out
        private void alistAlBankSamplesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                string[] files = Directory.GetFiles(fbd.SelectedPath);

                var x = new List<string>();

                foreach (string s in files)
                {
                    var bank = Bank.FromFile(s);

                    foreach (var v in bank.Entries)
                    {
                        x.Add(Path.GetFileNameWithoutExtension(s) + "," + v.ToString("000"));
                    }
                }

                x.Sort();

                var sb = new StringBuilder();

                foreach (string s in x) sb.AppendLine(s);

                trackInfoBox.Text = sb.ToString();
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            metaInstInfo.Text = Meta.GetMetaInstText(metaInstList.SelectedItem.ToString());
        }

        private void reloadMIDIMappingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadMeta();
        }

        private void importMIDIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (seq is null)
                seq = new Cseq();

            var ofd = new OpenFileDialog();
            ofd.Filter = "MIDI File (*.mid)|*.mid";

            if (ofd.ShowDialog() == DialogResult.OK)
                HowlControl.ImportMIDI(seq, ofd.FileName, 120, 0);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (seq is null)
                return;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Crash Team Racing CSEQ (*.cseq)|*.cseq";

            if (sfd.ShowDialog() == DialogResult.OK)
                seq.Save(sfd.FileName);
        }

        private void setSamplePathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            if (seq != null)
            {
                fbd.RootFolder = Environment.SpecialFolder.Desktop;
                fbd.SelectedPath = seq.path;
            }

            if (fbd.ShowDialog() == DialogResult.OK)
                samplePath = fbd.SelectedPath;

            Properties.Settings.Default.samplePath = samplePath;
        }

        private void trackInfoBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (seq is null) return;

            seq.Songs.Clear();
            sequenceBox.Items.Clear();
        }

        private void exportAllButton_Click(object sender, EventArgs e)
        {
            if (seq is null) return;
            if (seq.Context == null) return;

            var fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                seq.ExportSamples(fbd.SelectedPath);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (seq is null) return;

            using (var bw = new BinaryWriterEx(new MemoryStream()))
            {
                seq.Write(bw);

                int size = (int)bw.BaseStream.Length;

                MessageBox.Show($"Size {(size > 0x5800 ? "OVERFLOW" : "OK")}! {bw.BaseStream.Length.ToString("X8")} (0x5800 is the limit)");
            }
        }

        private void duplicateInstrumentButton_Click(object sender, EventArgs e)
        {
            if (seq is null) return;

            seq.Instruments.Add(new SpuInstrument());

            FillUI();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (seq is null) return;

            seq.Percussions.Add(new SpuInstrumentShort());

            FillUI();
        }

        InstrumentSelectionForm instSelectForm;

        private void changeInstrumentButton_Click(object sender, EventArgs e)
        {
            if (seq is null) return;

            if (!(instrumentList.SelectedNode.Tag is SpuInstrument)) return;

            if (instSelectForm == null)
                instSelectForm = new InstrumentSelectionForm(seq.Context.howl);

            if (instSelectForm.ShowDialog() == DialogResult.OK)
            {
                //if it's an instrument
                if (instrumentList.SelectedNode.Parent.Index == 0)
                {
                    seq.Instruments[instrumentList.SelectedNode.Index] = instSelectForm.SelectedInstrument as SpuInstrument;
                    instrumentList.SelectedNode.Text = instSelectForm.SelectedInstrument.Sample.Name;
                    instrumentList.SelectedNode.Tag = instSelectForm.SelectedInstrument;
                }
                // else if it's percussion
                else if (instrumentList.SelectedNode.Parent.Index == 1)
                {
                    seq.Percussions[instrumentList.SelectedNode.Index] = instSelectForm.SelectedInstrument as SpuInstrumentShort;
                    instrumentList.SelectedNode.Text = instSelectForm.SelectedInstrument.Sample.Name;
                    instrumentList.SelectedNode.Tag = instSelectForm.SelectedInstrument;
                }
            }

            //instrumentList.Focus();
        }

        private void findBank_Click(object sender, EventArgs e)
        {
            int i = 0;

            foreach (var bank in seq.Context.howl.Banks)
            {
                if (bank.MatchesCseq(seq))
                {
                   
                    MessageBox.Show("found bank! bank index: " + i);
                    return;
                }
                i++;
            }
        }
    }
}