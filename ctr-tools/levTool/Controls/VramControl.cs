using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using CTRFramework;
using CTRFramework.Shared;
using CTRFramework.Vram;
using System.Diagnostics;

namespace CTRTools
{
    public partial class CtrToolsVramControl : UserControl
    {
        public CtrToolsVramControl()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        private void actionPack_Click(object sender, EventArgs e)
        {
            if (!File.Exists(pathFile.Text))
            {
                MessageBox.Show($"File doesn't exist!\r\n{pathFile.Text}");
                return;
            }

            using (Scene scn = Scene.FromFile(pathFile.Text))
            {
                Tim ctr = scn.ctrvram;

                if (optionDebugVram.Checked)
                    ctr.SaveBMP(Path.Combine(Path.GetDirectoryName(pathFolder.Text), "test_old.bmp"), BMPHeader.GrayScalePalette(16));

                Dictionary<string, TextureLayout> list = scn.GetTexturesList(Detail.Med);

                foreach (string s in Directory.GetFiles(pathFolder.Text, "*.png"))
                {
                    string tag = Path.GetFileNameWithoutExtension(s);

                    Console.Write($"replacing {tag}... ");

                    if (!list.ContainsKey(tag))
                    {
                        Helpers.Panic(ctr, "missing texture entry");
                        continue;
                    }

                    Tim newtex = ctr.GetTimTexture(list[tag]);
                    newtex.LoadDataFromBitmap(s);

                    ctr.DrawTim(newtex);

                    Console.WriteLine("done.");
                }

                if (optionDebugVram.Checked)
                    ctr.SaveBMP(Path.Combine(Path.GetDirectoryName(pathFolder.Text), "test_new.bmp"), BMPHeader.GrayScalePalette(16));

                ctr.GetTrueColorTexture(512, 0, 384, 256).Write(Path.Combine(Path.GetDirectoryName(pathFolder.Text), "x01.tim"));
                ctr.GetTrueColorTexture(512, 256, 512, 256).Write(Path.Combine(Path.GetDirectoryName(pathFolder.Text), "x02.tim"));
            }

            MessageBox.Show("Done.");
        }

        private void actionExtract_Click(object sender, EventArgs e)
        {
            if (!File.Exists(pathFile.Text))
            {
                MessageBox.Show($"File doesn't exist!\r\n{pathFile.Text}");
                return;
            }

            using (Scene s = Scene.FromFile(pathFile.Text))
            {
                Helpers.CheckFolder(pathFolder.Text);

                if (optionTextMid.Checked)
                    s.ExportTextures(pathFolder.Text, Detail.Med);

                if (optionTexLow.Checked)
                    s.ExportTextures(pathFolder.Text, Detail.Low);
            }

            MessageBox.Show("Done!");
        }

        private void actionBrowseFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CTR scene file (*.lev)|*.lev";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                pathFile.Text = ofd.FileName;
                pathFolder.Text = Path.Combine(Path.GetDirectoryName(ofd.FileName), "textures");

                if (optionFolder.Checked)
                    Helpers.CheckFolder(pathFolder.Text);
            }
        }

        private void actionBrowseFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            if (pathFolder.Text != "")
            {
                fbd.SelectedPath = Path.GetDirectoryName(pathFolder.Text);
                fbd.RootFolder = Environment.SpecialFolder.MyComputer;
            }

            if (fbd.ShowDialog() == DialogResult.OK)
                pathFolder.Text = fbd.SelectedPath;
        }


        private void actionOpenFolder_Click(object sender, EventArgs e)
        {
            if (pathFolder.Text != "")
                if (Directory.Exists(pathFolder.Text))
                {
                    Process.Start(pathFolder.Text);
                }
                else
                {
                    MessageBox.Show($"Folder doesn't exist!\r\n{pathFolder.Text}");
                }
        }

    }
}
