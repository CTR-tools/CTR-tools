using CTRFramework.Sound;
using System;
using System.IO;
using System.Windows.Forms;

namespace CTRTools.Controls
{
    public partial class HowlControl : UserControl
    {
        Howl howl;

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

            foreach (var entry in Howl.samplesSfx)
            {
                listBox1.Items.Add(Howl.GetName(entry.SampleID, Howl.samplenames));
            }
        }

        private void actionExport_Click(object sender, EventArgs e)
        {
            if (howl == null)
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
                switch (Path.GetExtension(files[0]).ToLower())
                {
                    case ".hwl": LoadHowl(files[0]); break;
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
            propertyGrid1.SelectedObject = Howl.samplesSfx[listBox1.SelectedIndex];
        }

        private void actionSave_Click(object sender, EventArgs e)
        {
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                howl.Save(sfd.FileName);
            }
        }
    }
}