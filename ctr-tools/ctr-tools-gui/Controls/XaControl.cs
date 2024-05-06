using CTRFramework.Shared;
using CTRFramework.Audio;
using NAudio.Wave;
using System;
using System.IO;
using System.Windows.Forms;

namespace CTRTools.Controls
{
    public partial class XaControl : UserControl
    {

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
            if (ofdxnf.ShowDialog() == DialogResult.OK)
                LoadAudio(ofdxnf.FileName);
        }

        private void UpdateUI()
        {
            if (xnf != null)
            {
                comboBox1.Items.Clear();

                for (int i = 0; i < xnf.numGroups; i++)
                    comboBox1.Items.Add(xnf.folders[i]);

                comboBox1.SelectedIndex = 0;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (xnf != null)
            {
                listBox1.Items.Clear();

                for (int i = 0; i < xnf.numEntries[comboBox1.SelectedIndex]; i++)
                {
                    listBox1.Items.Add(xnf.Entries[i + xnf.entryStartIndex[comboBox1.SelectedIndex]].ToString());
                }

                label1.Text = $"Sounds: {xnf.numEntries[comboBox1.SelectedIndex]}";
            }
        }

        AudioFileReader wave;
        WaveOut outputSound;

        private void GoNext(object sender, StoppedEventArgs args)
        {
            if (listBox1.SelectedIndex + 1 < listBox1.Items.Count)
            {
                listBox1.SelectedIndex++;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (xnf != null)
            {
                if (listBox1.SelectedIndex >= 0)
                {
                    string name = Helpers.PathCombine(xnf.RootPath, xnf.folders[comboBox1.SelectedIndex], xnf.Entries[xnf.entryStartIndex[comboBox1.SelectedIndex] + listBox1.SelectedIndex].GetName());

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
            }
        }
    }
}
