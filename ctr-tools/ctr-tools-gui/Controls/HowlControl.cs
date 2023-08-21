using CTRFramework.Sound;
using NAudio.Midi;
using NAudio.Wave;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Windows.Forms;

namespace CTRTools.Controls
{
    public partial class HowlControl : UserControl
    {
        public Howl howl;

        public HowlControl()
        {
            InitializeComponent();
        }

        private void actionLoad_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() == DialogResult.OK)
                LoadHowl(ofd.FileName);
        }

        private void LoadHowl(string filename)
        {
            howl = Howl.FromFile(filename);
            label1.Text = $"Howl loaded from {filename}.";

            PopulateSamplesTab();
            PopulateSongsTab();

            banksTreeView.BeginUpdate();

            banksTreeView.Nodes.Clear();

            foreach (var bank in howl.Banks)
            {
                var bankNode = new TreeNode()
                {
                    Text = bank.Name + $" [{bank.numEntries}]",
                    Tag = bank
                };

                foreach (var sample in bank.Entries.Values)
                {
                    var sampleNode = new TreeNode()
                    {
                        Text = $"{sample.Name} ({sample.ID}) [{sample.HashString}]",
                        Tag = sample
                    };

                    bankNode.Nodes.Add(sampleNode);
                }

                banksTreeView.Nodes.Add(bankNode);
            }

            banksTreeView.EndUpdate();
        }

        private void PopulateSamplesTab()
        {
            sampleTableListBox.BeginUpdate();

            sampleTableListBox.Items.Clear();

            foreach (var entry in Howl.SampleTable)
            {
                var sample = entry.GetSample(entry.SampleID, howl.Context);
                sample.Context = howl.Context; //duh
                sampleTableListBox.Items.Add(entry.GetSample(entry.SampleID, howl.Context).Name);
            }

            sampleTableListBox.EndUpdate();
        }

        private void PopulateSongsTab()
        {
            songListBox.BeginUpdate();

            songListBox.Items.Clear();

            foreach (var entry in howl.Songs)
                songListBox.Items.Add(entry.name);

            songListBox.EndUpdate();
        }

        private void actionExport_Click(object sender, EventArgs e)
        {
            if (howl is null)
                return;

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                howl.ExportCSEQ(fbd.SelectedPath);
                label2.Text = $"Exported to {fbd.SelectedPath}";
            }
        }

        private void HowlControl_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files.Length > 0)
            {
                switch (Path.GetExtension(files[0]).ToUpper())
                {
                    case ".HWL": LoadHowl(files[0]); break;
                    default: MessageBox.Show("Unsupported file."); break;
                }
            }
        }

        private void HowlControl_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        AudioFileReader wave;
        WaveOut output;

        private void PlaySample(VagSample sample, float volume)
        {
            if (output != null)
            {
                output.Stop();
                wave.Close();
                wave = null;
            }

            File.Delete("temp.wav");
            sample.ExportWav("temp.wav");

            wave = new AudioFileReader("temp.wav");
            output = new WaveOut();

            output.Volume = volume;
            output.Init(wave);
            output.Play();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sampleTableListBox.SelectedIndex < 0) return;

            var samp = Howl.SampleTable[sampleTableListBox.SelectedIndex];
            propertyGrid1.SelectedObject = samp;
            PlaySample(samp.GetVagSample(howl.Context), samp.Volume);
        }

        private void actionSave_Click(object sender, EventArgs e)
        {
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                howl.Save(sfd.FileName);
            }
        }

        private void treeBanks_AfterSelect(object sender, TreeViewEventArgs e)
        {
        }

        private void songListBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (howl is null) return;

            if (ofd.ShowDialog() == DialogResult.OK)
                LoadHowl(ofd.FileName);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (howl is null) return;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                howl.Save(sfd.FileName);
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (howl is null) return;

            var fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                //unimplemented
            }
        }

        private void replaceSampleButton_Click(object sender, EventArgs e)
        {
            if (howl is null) return;

            int sampleIndex = Howl.SampleTable[sampleTableListBox.SelectedIndex].SampleID;

            var ofd = new OpenFileDialog();
            ofd.Filter = "PSX VAG sample file (*.vag)|*.vag";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                howl.ReplaceVagSample(sampleIndex, ofd.FileName);
            }
        }

        private void cseqSave_Click(object sender, EventArgs e)
        {
            var seq = howl.Songs[songListBox.SelectedIndex];

            var savedialog = new SaveFileDialog();
            savedialog.Filter = "CTR CSEQ (*.cseq)|*.cseq";
            savedialog.FileName = seq.name;

            if (savedialog.ShowDialog() == DialogResult.OK)
            {
                howl.Songs[songListBox.SelectedIndex].Save(savedialog.FileName);
            }
        }

        private void cseqImport_Click(object sender, EventArgs e)
        {
            var seq = howl.Songs[songListBox.SelectedIndex];

            var ofd = new OpenFileDialog();
            ofd.Filter = "MIDI File (*.mid)|*.mid";

            if (ofd.ShowDialog() == DialogResult.OK)
                ImportMIDI(seq, ofd.FileName);
        }

        private void ImportMIDI(Cseq seq, string filename)
        {
            var midi = new MidiFile(filename);
            var newMidi = new CseqSong();

            //find a way to read from midi
            newMidi.BeatsPerMinute = (int)numericUpDown1.Value;
            newMidi.TicksPerQuarterNote = midi.DeltaTicksPerQuarterNote;

            for (int i = 1; i < midi.Tracks; i++)
            {
                var track = new CSeqTrack();
                //track.name = $"track_{i.ToString("00")}";
                track.FromMidiEventList(midi.Events.GetTrackEvents(i).ToList());

                newMidi.Tracks.Add(track);
            }

            foreach (var x in newMidi.Tracks.ToList())
            {
                if (x.isDrumTrack)
                    newMidi.Tracks.Remove(x);
            }

            //fix track index
            for (int i = 0; i < newMidi.Tracks.Count; i++)
                newMidi.Tracks[i].Index = i;


            foreach (var x in newMidi.Tracks)
            {
                var c = new CseqEvent();
                c.eventType = CseqEventType.ChangePatch;
                c.pitch = (byte)x.Index;
                c.wait = 0;
                x.cseqEventCollection.Insert(0, c);
            }

            seq.Songs[0] = newMidi;
        }
    }
}