using CTRFramework.Shared;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace CTRTools
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            this.Text = $"{this.Text} - {Meta.Version}";
            labelVersion.Text = Meta.Version;
            signLabel.Text = Meta.GetSignature();

            OBJ.FixCulture();

            //pass drag drop event to all child controls to be able to switch tabs

            tabControl.DragEnter += MainForm_DragEnter;

            foreach (TabPage tab in tabControl.TabPages)
            {
                tab.DragEnter += MainForm_DragEnter;

                foreach (Control control in tab.Controls)
                    control.DragEnter += MainForm_DragEnter;
            }
        }

        // Link to CTR-tools Github repository.
        private void githubBox_Click(object sender, EventArgs e) => Process.Start(Meta.LinkGithub);

        // Link to CTR-tools Discord.
        private void discordBox_Click(object sender, EventArgs e) => Process.Start(Meta.LinkDiscord);

        // Save settings on form close
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) => Properties.Settings.Default.Save();

        /// <summary>
        /// Checks file extension and switches to proper tab automatically.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files.Length > 0)
            {
                MaybeSwitchTab(files[0]);
            }
        }

        public void MaybeSwitchTab(string filename)
        {
            switch (Path.GetExtension(filename).ToUpper())
            {
                case ".CTR":
                case ".OBJ":
                case ".PLY": tabControl.SelectedTab = tabCtr; break;
                case ".VRM": tabControl.SelectedTab = tabVram; break;
                case ".MPK": tabControl.SelectedTab = tabVram; break;
                case ".BIG": tabControl.SelectedTab = tabBig; break;
                case ".XNF": tabControl.SelectedTab = tabXa; break;
                case ".HWL": tabControl.SelectedTab = tabHowl; break;
                case ".CSEQ": tabControl.SelectedTab = tabCseq; break;
                case ".LNG":
                case ".TXT": tabControl.SelectedTab = tabLang; break;
                case ".LEV":
                    if (tabControl.SelectedTab != tabVram)
                        tabControl.SelectedTab = tabLev;
                    break;
            }
        }
    }
}