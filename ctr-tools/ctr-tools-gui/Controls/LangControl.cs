using CTRFramework.Lang;
using System;
using System.IO;
using System.Windows.Forms;

namespace CTRTools.Controls
{
    public partial class LangControl : UserControl
    {
        LNG lng;
        int linesOnLoad = 0;

        public LangControl()
        {
            InitializeComponent();
        }

        private void actionLoad_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                lng = LNG.FromFile(ofd.FileName);
                linesOnLoad = lng.Entries.Count;
                langBox.Lines = lng.Entries.ToArray();
            }
        }

        private void langBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
                langBox.SelectAll();
        }

        string msg1 = "The number of lines has changed.";
        string msg2 = "Are you sure you want to save?";

        private void actionSave_Click(object sender, EventArgs e)
        {
            //if number of lines changed, ask user if he still wants to save
            if (langBox.Lines.Length != linesOnLoad && linesOnLoad != 0)
                if (MessageBox.Show($"{msg1}\r\n{msg2}", "Warning", MessageBoxButtons.YesNo) == DialogResult.No)
                    return;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                lng = LNG.FromText(langBox.Lines);
                lng.Save(sfd.FileName);
                linesOnLoad = lng.Entries.Count;
            }
        }

        private void LangControl_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files.Length > 0)
            {
                switch (Path.GetExtension(files[0]).ToLower())
                {
                    case ".lng": lng = LNG.FromFile(files[0]); break;
                    case ".txt": lng = LNG.FromText(File.ReadAllLines(files[0], System.Text.Encoding.Default)); break;
                    default: MessageBox.Show("Unsupported file."); return;
                }

                langBox.Lines = lng.Entries.ToArray();
            }
        }

        private void LangControl_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }
    }
}