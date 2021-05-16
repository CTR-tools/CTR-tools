using CTRFramework;
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

            this.Text = $"{this.Text} - {Meta.GetVersion()}";
            labelVersion.Text = Meta.GetVersion();
            signLabel.Text = Meta.GetSignature();

            OBJ.FixCulture();
        }

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
                switch (Path.GetExtension(files[0]).ToLower())
                {
                    case ".dyn":
                    case ".ctr":
                    case ".obj":
                    case ".ply":
                        tabControl.SelectedTab = tabCtr;
                        break;
                    case ".vrm":
                        tabControl.SelectedTab = tabVram;
                        break;
                    case ".lev":
                        if (tabControl.SelectedTab != tabVram)
                            tabControl.SelectedTab = tabLev;
                        break;
                    case ".big":
                        tabControl.SelectedTab = tabBig;
                        break;
                    case ".xnf":
                        tabControl.SelectedTab = tabXa;
                        break;
                    //case ".hwl":
                    //   tabControl1.SelectedTab = tabHowl;
                    //   break;
                    case ".cseq":
                        tabControl.SelectedTab = tabCseq;
                        break;
                    case ".lng":
                        tabControl.SelectedTab = tabLang;
                        break;
                }
            }
        }

        /// <summary>
        /// Link to CTR-tools Github repository.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void githubBox_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/CTR-tools/CTR-tools");
        }

        /// <summary>
        /// Link to CTR-tools Discord.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void discordBox_Click(object sender, EventArgs e)
        {
            Process.Start("https://discord.gg/56xm9Aj");
        }
    }
}