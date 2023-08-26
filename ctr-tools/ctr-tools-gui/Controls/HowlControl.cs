using CTRFramework.Shared;
using CTRFramework.Sound;
using NAudio.Midi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Windows.Forms;

namespace CTRTools.Controls
{
    public partial class HowlControl : UserControl
    {
        string loadedFilename = "";
        Howl howl;

        public HowlControl()
        {
            InitializeComponent();
        }

        private void LoadHowl(string filename)
        {
            try
            {
                howl = Howl.FromFile(filename);
                label1.Text = $"Howl loaded from {filename}.";

                loadedFilename = filename;

                PopulateSamplesTab();
                PopulateSongsTab();
                PopulateBanksTab();

                HowlPlayer.Context = howl.Context;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}\r\n\r\n{ex}", "Error!");
            }
        }

        private void PopulateBanksTab()
        {
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

        /// <summary>
        /// A wrapper howl null check that warns user if howl is not loaded.
        /// </summary>
        /// <returns>bool: HOWL loaded state</returns>
        private bool IsHowlLoaded()
        {
            if (howl == null)
                MessageBox.Show("HOWL is not loaded yet!", "Missing HOWL file");
  
            return howl != null;
        }

        #region [drag/drop handling]
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
        #endregion

        #region [audio hadling]

        #endregion

        private void sampleTableListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsHowlLoaded()) return;

            if (sampleTableListBox.SelectedIndex < 0) return;

            var samp = Howl.SampleTable[sampleTableListBox.SelectedIndex];
            propertyGrid1.SelectedObject = samp;
            HowlPlayer.Play(samp);
        }


        #region [File menu event handlers]

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() == DialogResult.OK)
                LoadHowl(ofd.FileName);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!IsHowlLoaded()) return;

            Helpers.BackupFile(loadedFilename);
            howl.Save(loadedFilename);

            label1.Text = "saved to " + loadedFilename;
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!IsHowlLoaded()) return;
            sfd.FileName = loadedFilename;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                loadedFilename = sfd.FileName;

                Helpers.BackupFile(loadedFilename);
                howl.Save(loadedFilename);
 
                label1.Text = "saved to " + loadedFilename;
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!IsHowlLoaded()) return;

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                howl.ExportCSEQ(fbd.SelectedPath);
                label2.Text = $"Exported to {fbd.SelectedPath}";
            }
        }
        #endregion



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
            if (songListBox.SelectedIndex == -1) return;

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
            if (songListBox.SelectedIndex == -1) return;

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

            int absoluteTime = 0;

            //foreach track in midi file, why 1?
            for (int i = 1; i < midi.Tracks; i++)
            {
                var track = new CSeqTrack();
                //track.name = $"track_{i.ToString("00")}";
                track.FromMidiEventList(midi.Events.GetTrackEvents(i).ToList());

                newMidi.Tracks.Add(track);
            }

            //calculate end track
            foreach (var track in newMidi.Tracks)
            {
                var last = track.cseqEventCollection.Count;
                if (last == 0) continue;

                var test = track.cseqEventCollection[last - 1].absoluteTime;

                if (test > absoluteTime)
                    absoluteTime = test;
            }

            //put proper end tracks
            foreach (var track in newMidi.Tracks.ToList())
            {
                var last = track.cseqEventCollection.Count;

                if (last != 0)
                {

                    var test = absoluteTime - track.cseqEventCollection[last - 1].absoluteTime;

                    track.cseqEventCollection.Add(new CseqEvent()
                    {
                        absoluteTime = absoluteTime,
                        wait = test,
                        eventType = CseqEventType.EndTrack
                    });
                }
                else
                {
                    newMidi.Tracks.Remove(track);
                }
            }

            var percTable = new List<byte>();

            foreach (var x in newMidi.Tracks)
            {
                if (x.isDrumTrack)
                {
                    foreach (var evt in x.cseqEventCollection)
                    {
                        if (!percTable.Contains(evt.pitch))
                            percTable.Add(evt.pitch);
                    }

                    foreach (var evt in x.cseqEventCollection)
                    {
                        evt.pitch = (byte)percTable.IndexOf(evt.pitch);
                    }
                }
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

        private void findFreeIndexButton_Click(object sender, EventArgs e)
        {
            if (!IsHowlLoaded()) return;

            int index = 1;

            do
            {
                if (howl.Context.Samples.ContainsKey(index))
                {
                    index++;
                }
                else
                {
                    MessageBox.Show("found free index: " + index);

                    howl.Context.Samples.Add(index, new Sample() { ID = index } );
                    Howl.SampleTable[sampleTableListBox.SelectedIndex].SampleID = (ushort)index;

                    propertyGrid1.SelectedObject = Howl.SampleTable[sampleTableListBox.SelectedIndex];

                    return;
                }
            }
            while (index < 2048);

            if (index >= 2048)
            { 
                MessageBox.Show("Index search overflow!");
                return;
            }
        }

        private void addToSfxBank_Click(object sender, EventArgs e)
        {
            if (!IsHowlLoaded()) return;

            var index = Howl.SampleTable[sampleTableListBox.SelectedIndex].SampleID;

            if (!howl.Banks[0].Entries.ContainsKey(index))
            {
                howl.Banks[0].Entries.Add(index, howl.Context.Samples[index]);
            }
            else
            {
                MessageBox.Show("Already in the SFX bank!");
            }
        }

        private void wipeButton_Click(object sender, EventArgs e)
        {
            if (!IsHowlLoaded()) return;

            var index = Howl.SampleTable[sampleTableListBox.SelectedIndex].SampleID;
            howl.Context.Samples[index].Data = new byte[0];
            howl.Context.SpuPtrTable[index] = new SpuAddr() { Size = 0 };
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void editSongButton_Click(object sender, EventArgs e)
        {
            if (songListBox.SelectedIndex == -1) return;

            cseqControl1.seq = howl.Songs[songListBox.SelectedIndex];
            tabControl1.SelectedTab = tabSong;
            cseqControl1.FillUI();
        }

        private void songListBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void songListBox_DoubleClick(object sender, EventArgs e)
        {
            if (songListBox.SelectedIndex == -1) return;

            cseqControl1.seq = howl.Songs[songListBox.SelectedIndex];
            tabControl1.SelectedTab = tabSong;
            cseqControl1.FillUI();
        }
    }
}