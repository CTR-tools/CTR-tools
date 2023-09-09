using CTRFramework.Lang;
using CTRFramework.Shared;
using System;
using System.IO;
using System.Windows.Forms;

namespace CTRTools.Controls
{
    public partial class LangControl : UserControl
    {
        LNG lng;
        int linesOnLoad = 0;
        string orginalFilePath = "";

        public LangControl()
        {
            InitializeComponent();
        }

        private void LoadLang(string filename)
        {
            switch (Path.GetExtension(filename).ToUpper())
            {
                case ".LNG":
                    lng = LNG.FromFile(filename); break;
                case ".TXT":
                    lng = LNG.FromText(File.ReadAllLines(filename, System.Text.Encoding.UTF8)); break;
                default:
                    MessageBox.Show("Unsupported file."); break;
            }

            linesOnLoad = lng.Entries.Count;
            langBox.Lines = lng.Entries.ToArray();
            orginalFilePath = Path.ChangeExtension(filename, ".lng");
        }
        private void SaveLang(string filename)
        {
            if (lng is null) return;

            Helpers.BackupFile(orginalFilePath);
            lng = LNG.FromText(langBox.Lines);
            lng.Save(filename);
            linesOnLoad = lng.Entries.Count;
        }

        private void langBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
                langBox.SelectAll();
        }

        private void LangControl_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files.Length > 0)
                LoadLang(files[0]);
        }

        private void LangControl_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }


        string msg1 = "The number of lines has changed.";
        string msg2 = "Are you sure you want to save?";

        private bool ProceedToSave()
        {
            //if number of lines changed, ask user if he still wants to save
            if (linesOnLoad != 0 && langBox.Lines.Length != linesOnLoad)
                if (MessageBox.Show($"{msg1}\r\n{msg2}", "Warning", MessageBoxButtons.YesNo) == DialogResult.No)
                    return false;

            return true;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() == DialogResult.OK)
                LoadLang(sfd.FileName);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lng is null) return;

            if (ProceedToSave())
                SaveLang(orginalFilePath);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lng is null) return;

            if (ProceedToSave())
            {
                sfd.FileName = Path.GetFileNameWithoutExtension(orginalFilePath);
                sfd.InitialDirectory = Path.GetDirectoryName(orginalFilePath);

                if (sfd.ShowDialog() == DialogResult.OK)
                    SaveLang(sfd.FileName);
            }
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fontDialog = new FontDialog() { Font = langBox.Font };

            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                langBox.Font = fontDialog.Font;
                langBox.SelectionLength = 0;
            }
        }

        private void exportStringsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lng is null) return;

            var saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Text file (*.txt)|*.txt";
            saveDialog.FileName = Path.ChangeExtension(orginalFilePath, ".txt");

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                lng.Export(saveDialog.FileName);
            }
        }
    }
}