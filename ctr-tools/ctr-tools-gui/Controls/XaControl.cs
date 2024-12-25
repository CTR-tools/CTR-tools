using CTRFramework.Audio;
using CTRFramework.Shared;
using NAudio.Wave;
using System;
using System.IO;
using System.Windows.Forms;

namespace CTRTools.Controls
{
    public partial class XaControl : UserControl
    {
        public string lastLoaded = String.Empty;

        XaInfo xnf;

        public XaControl()
        {
            InitializeComponent();
        }

        public void LoadAudio(string filename)
        {
            try
            {
                switch (Path.GetExtension(filename).ToUpper())
                {
                    case ".XNF":
                        xnf = XaInfo.FromFile(filename);
                        break;

                    default:
                        MessageBox.Show("Unsupported file.");
                        break;
                }

                UpdateUI();

                lastLoaded = filename;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void XaControl_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files.Length > 0)
                LoadAudio(files[0]);
        }

        private void XaControl_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void actionLoadXnf_Click(object sender, EventArgs e)
        {

        }

        private void UpdateUI()
        {
            if (xnf != null)
            {
                xnfFolderBox.Items.Clear();

                for (int i = 0; i < xnf.numGroups; i++)
                    xnfFolderBox.Items.Add(xnf.folders[i]);

                xnfFolderBox.SelectedIndex = 0;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (xnf != null)
            {
                xnfEntriesList.Items.Clear();

                for (int i = 0; i < xnf.numEntries[xnfFolderBox.SelectedIndex]; i++)
                {
                    xnfEntriesList.Items.Add(xnf.Entries[i + xnf.entryStartIndex[xnfFolderBox.SelectedIndex]].ToString());
                }

                label1.Text = $"Sounds: {xnf.numEntries[xnfFolderBox.SelectedIndex]}";
            }
        }

        AudioFileReader wave;
        WaveOut outputSound;

        private void GoNext(object sender, StoppedEventArgs args)
        {
            if (xnfEntriesList.SelectedIndex + 1 < xnfEntriesList.Items.Count)
            {
                xnfEntriesList.SelectedIndex++;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (xnf == null) return;
            if (xnfEntriesList.SelectedIndex == -1) return;

            xnfEntryEditor.SelectedObject = xnf.Entries[xnf.entryStartIndex[xnfFolderBox.SelectedIndex] + xnfEntriesList.SelectedIndex];
            xnfEntryEditor.Update();

            string name = Helpers.PathCombine(xnf.RootPath, xnf.folders[xnfFolderBox.SelectedIndex], xnf.Entries[xnf.entryStartIndex[xnfFolderBox.SelectedIndex] + xnfEntriesList.SelectedIndex].GetName());

            if (File.Exists(name))
            {
                if (outputSound != null)
                {
                    outputSound.Stop();
                    if (playNext.Checked)
                        outputSound.PlaybackStopped -= GoNext;
                }

                wave = new NAudio.Wave.AudioFileReader(name);
                outputSound = new WaveOut();
                outputSound.Init(wave);
                outputSound.Play();
                if (playNext.Checked)
                    outputSound.PlaybackStopped += GoNext;
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void loadXNFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ofdxnf.ShowDialog() == DialogResult.OK)
                LoadAudio(ofdxnf.FileName);
        }

        private void saveXNFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (xnf == null) return;

            if (String.IsNullOrEmpty(lastLoaded))
                SaveAs();
            else
                xnf.Save(lastLoaded);
        }

        private void saveXNFAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs();
        }

        private void SaveAs()
        {
            if (xnf == null) return;

            var sfd = new SaveFileDialog();
            sfd.Filter = "Crash Team Racing XNF file (*.xnf)|*.xnf";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                lastLoaded = sfd.FileName;
                xnf.Save(lastLoaded);
            }
        }

    }
}
