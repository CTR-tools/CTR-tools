using CTRFramework.Sound;
using NAudio.Wave;
using System;
using System.IO;
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


            sampleTableListBox.BeginUpdate();

            sampleTableListBox.Items.Clear();

            foreach (var entry in Howl.SampleTable)
                sampleTableListBox.Items.Add(Howl.GetName(entry.SampleID, Howl.samplenames));

            sampleTableListBox.EndUpdate();


            songListBox.BeginUpdate();
            songListBox.Items.Clear();

            foreach (var entry in howl.Songs)
                songListBox.Items.Add(entry.name);

            songListBox.EndUpdate();


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

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sampleTableListBox.SelectedIndex < 0) return;

            propertyGrid1.SelectedObject = Howl.SampleTable[sampleTableListBox.SelectedIndex];
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
            var seq = howl.Songs[songListBox.SelectedIndex];

            var savedialog = new SaveFileDialog();
            savedialog.Filter = "CTR CSEQ (*.cseq)|*.cseq";
            savedialog.FileName = seq.name;

            if (savedialog.ShowDialog() == DialogResult.OK)
            {


                howl.Songs[songListBox.SelectedIndex].Save(savedialog.FileName);
            }
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
    }
}