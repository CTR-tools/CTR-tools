using CTRFramework.Audio;
using CTRFramework.Shared;
using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

                //pass context whereever it's needed
                HowlPlayer.Context = howl.Context;
                instrumentControl1.Context = howl.Context;
                cseqControl1.SetContext(howl.Context);
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

                foreach (var index in bank.Entries)
                {
                    var sample = howl.Context.SamplePool[index];

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


        // sample cache to show in a listbox
        private List<SpuInstrument> SampleCache = new List<SpuInstrument>();

        private void PopulateSamplesTab(string searchTerm = null)
        {
            sampleTableListBox.BeginUpdate();

            sampleTableListBox.Items.Clear();
            instrumentControl1.Clear();

            SampleCache.Clear();

            int i = 0;

            foreach (var entry in howl.EffectsTable)
            {
                var sample = entry.GetSample(entry.SampleID, howl.Context);

                // TODO figure out whats going on here, missing sampleIDs?
                // only null in demos (spyro, opsm)
                if (sample == null)
                {
                    MessageBox.Show($"missing sample? {entry.SampleID}");
                    continue;
                }

                sample.Context = howl.Context; //duh

                if (searchTerm is null || searchTerm == "")
                {
                    SampleCache.Add(entry);
                    i++;
                }
                else
                {
                    if (sample.Name.ToUpper().Contains(searchTerm.ToUpper()))
                    {
                        SampleCache.Add(entry);
                        i++;
                    }
                }
            }

            foreach (var sample in SampleCache)
            {
                sampleTableListBox.Items.Add(howl.Context.SamplePool[sample.SampleID].Name);
            }

            sampleTableListBox.EndUpdate();
        }

        private void PopulateSongsTab()
        {
            songListBox.BeginUpdate();

            songListBox.Items.Clear();

            foreach (var entry in howl.Songs)
                songListBox.Items.Add($"{entry.Name} [{entry.Percussions.Count + entry.Instruments.Count}]");

            songListBox.EndUpdate();
        }

        /// <summary>
        /// A wrapper howl null check that warns user if howl is not loaded.
        /// </summary>
        /// <returns>bool: HOWL loaded state</returns>
        private bool IsHowlLoaded()
        {
            if (howl == null)
            {
                SystemSounds.Exclamation.Play();
                MessageBox.Show("HOWL is not loaded yet!", "Missing HOWL file");
            }

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
                    case ".MID":
                        if (songListBox.SelectedIndex == -1)
                        {
                            MessageBox.Show("Select a song to replace first!");
                            break;
                        }
                        ImportMIDI(howl.Songs[songListBox.SelectedIndex], files[0]);
                        break;
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
            var inst = SampleCache[sampleTableListBox.SelectedIndex];
            instrumentControl1.Instrument = inst;
            HowlPlayer.Play(inst);
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

            try
            {
                Helpers.BackupFile(loadedFilename);
                howl.Save(loadedFilename);

                label1.Text = "saved to " + loadedFilename;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.ToString(), "Warning!");
            }
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
        }

        private void cseqSave_Click(object sender, EventArgs e)
        {
            if (songListBox.SelectedIndex == -1) return;

            var seq = howl.Songs[songListBox.SelectedIndex];

            var savedialog = new SaveFileDialog();
            savedialog.Filter = "CTR CSEQ (*.cseq)|*.cseq";
            savedialog.FileName = seq.Name;

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
                ImportMIDI(seq, ofd.FileName, (int)numericUpDown1.Value);
        }

        public static void ImportMIDI(Cseq seq, string filename, int bpm = 120, int songIndex = 0)
        {
            if (seq.Songs.Count <= songIndex) return;

            var midi = new MidiFile(filename, false);
            var newMidi = new CseqSong();

            //find a way to read from midi
            newMidi.BeatsPerMinute = bpm;
            newMidi.TicksPerQuarterNote = midi.DeltaTicksPerQuarterNote;

            int absoluteTime = 0;

            //foreach track in midi file
            for (int i = 0; i < midi.Tracks; i++)
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
                        if (evt.eventType == CseqEventType.NoteOn || evt.eventType == CseqEventType.NoteOff)
                            if (!percTable.Contains(evt.pitch))
                                percTable.Add(evt.pitch);
                    }

                    foreach (var evt in x.cseqEventCollection)
                    {
                        if (evt.eventType == CseqEventType.NoteOn || evt.eventType == CseqEventType.NoteOff)
                            evt.pitch = (byte)percTable.IndexOf(evt.pitch);
                    }
                }
            }

            //generate index table for percussion samples for the user
            if (percTable.Count > 0)
            {
                var sb = new StringBuilder();

                sb.Append("");

                for (int i = 0; i < percTable.Count; i++)
                    sb.AppendLine($"{i}={percTable[i]}");

                Clipboard.SetText(sb.ToString());
                MessageBox.Show("copied to clipboard:\r\n" + sb.ToString());
            }

            //fix track index
            for (int i = 0; i < newMidi.Tracks.Count; i++)
                newMidi.Tracks[i].Index = i;

            byte instIndex = 0;

            foreach (var track in newMidi.Tracks)
            {
                //change patch is not needed if it's drum track
                if (track.isDrumTrack)
                {
                    continue;
                }

                var c = new CseqEvent();
                c.eventType = CseqEventType.ChangePatch;
                c.pitch = (byte)instIndex;
                c.wait = 0;
                track.cseqEventCollection.Insert(0, c);

                instIndex++;
            }

            seq.Songs[songIndex] = newMidi;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) => Application.Exit();

        private void editSongButton_Click(object sender, EventArgs e)
        {
            if (songListBox.SelectedIndex == -1) return;

            cseqControl1.seq = howl.Songs[songListBox.SelectedIndex];
            tabControl1.SelectedTab = tabSong;
            cseqControl1.FillUI();
        }

        private void songListBox_DoubleClick(object sender, EventArgs e)
        {
            if (songListBox.SelectedIndex == -1) return;

            cseqControl1.seq = howl.Songs[songListBox.SelectedIndex];
            tabControl1.SelectedTab = tabSong;
            cseqControl1.FillUI();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            instrumentControl1.Clear();
            sampleTableListBox.Items.Clear();
            PopulateSamplesTab(textBox1.Text);
        }

        CancellationTokenSource songStopSource;
        Task songPlayer = null;

        private void playButton_Click(object sender, EventArgs e)
        {

            if (songListBox.SelectedIndex == -1) return;

            int index = songListBox.SelectedIndex;

            if (songPlayer != null)
            {
                songStopSource.Cancel();
                songPlayer = null;
            }

            var song = howl.Songs[index];

            songStopSource = new CancellationTokenSource();

            int i = 0;

            foreach (var track in song.Songs[0].Tracks)
            {
                HowlPlayer.PlayTrack(songStopSource.Token, song, 0, i);
                i++;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (howl is null) return;

            songStopSource.Cancel();
            songPlayer = null;
        }


        private void HowlControl_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void songListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (songListBox.SelectedIndex == -1) return;

            propertyGrid1.SelectedObject = howl.Songs[songListBox.SelectedIndex];

            /*
                        //wont really work as intended

                        int i = 0;

                        var banks = new List<int>();

                        foreach (var bank in howl.Banks)
                        {
                            if (bank.MatchesCseq(howl.Songs[songListBox.SelectedIndex]))
                            {
                                banks.Add(i);
                            }
                            i++;
                        }

                        string result = "found banks: ";

                        foreach (var b in banks) { result += b + ", "; }

                        MessageBox.Show(result);
            */
        }
    }
}