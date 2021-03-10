using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using CTRFramework.Sound;

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
    }
}