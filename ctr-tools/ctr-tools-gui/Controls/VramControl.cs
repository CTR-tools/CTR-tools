using CTRFramework;
using CTRFramework.Shared;
using CTRFramework.Vram;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CTRTools.Controls
{
    public partial class VramControl : UserControl
    {
        public VramControl()
        {
            InitializeComponent();
        }

        private string pathFileParent => Path.GetDirectoryName(pathFile.Text);

        private void actionPack_Click(object sender, EventArgs e)
        {
            if (!DataExists()) return;

            var textures = new Dictionary<string, TextureLayout>();
            var vrm = new Tim();

            switch (Path.GetExtension(pathFile.Text))
            {
                case ".lev":
                    using (var scene = CtrScene.FromFile(pathFile.Text))
                    {
                        textures = scene.GetTexturesList();
                    }
                    break;
                case ".mpk":
                    using (var mpk = ModelPack.FromFile(pathFile.Text))
                    {
                        textures = mpk.GetTexturesList();
                    }
                    break;
            }

            if (textures.Count == 0)
            {
                MessageBox.Show("no textures to replace!");
                return;
            }

            vrm = CtrVrm.FromFile(pathVram.Text).GetVram();

            ReplaceTextures(textures, vrm);

            //force collect the garbage
            GC.Collect();

            MessageBox.Show("Done.");
        }

        //checks whether all paths provided are ok
        private bool DataExists()
        {
            if (pathFile.Text == "" || !File.Exists(pathFile.Text))
            {
                MessageBox.Show($"File doesn't exist!\r\n{pathFile.Text}");
                return false;
            }

            if (pathVram.Text == "" || !File.Exists(pathVram.Text))
            {
                MessageBox.Show($"Vram file doesn't exist!\r\n{pathVram.Text}");
                return false;
            }

            if (pathFolder.Text == "" || !Directory.Exists(pathFolder.Text))
            {
                MessageBox.Show($"Folder doesn't exist!\r\n{pathFolder.Text}");
                return false;
            }

            return true;
        }

        //
        private void ReplaceTextures(Dictionary<string, TextureLayout> textures, Tim vrm)
        {
            if (!DataExists()) return;

            //dumping vram before the change
            if (optionDebugVram.Checked)
                //only dump if file doesn't exist (to compare to earliest version of vram)
                if (!File.Exists(Path.Combine(pathFileParent, "test_old.bmp")))
                    vrm.SaveBMP(Path.Combine(pathFileParent, "test_old.bmp"), BMPHeader.GrayScalePalette(16));

            foreach (var png in Directory.GetFiles(pathFolder.Text, "*.png"))
            {
                string tag = Path.GetFileNameWithoutExtension(png);

                Helpers.Panic(vrm, PanicType.Info, $"replacing {tag}... ");

                if (!textures.ContainsKey(tag))
                {
                    Helpers.Panic(vrm, PanicType.Warning, "unknown texture entry");
                    continue;
                }

                Tim newtex = vrm.GetTimTexture(textures[tag]);
                newtex.LoadDataFromBitmap(png);

                vrm.DrawTim(newtex);
            }


            if (optionDebugVram.Checked)
                vrm.SaveBMP(Path.Combine(pathFileParent, "test_new.bmp"), BMPHeader.GrayScalePalette(16));

            //now onto saving vram...

            //backup existing file
            Helpers.BackupFile(pathVram.Text);
            File.Delete(pathVram.Text);

            var tims = new List<Tim>() {
                vrm.GetTrueColorTexture(new Rectangle(512, 0, 384, 256)),
                vrm.GetTrueColorTexture(new Rectangle(512, 256, 512, 256))
            };

            //should move this to ctrvram i guess
            using (var bw = new BinaryWriterEx(File.Create(pathVram.Text)))
            {
                bw.Write((int)0x20);

                foreach (var tim in tims)
                {
                    bw.Write(tim.Filesize);
                    tim.Write(bw);
                }

                bw.Write((int)0);
            }

            //ctr.GetTrueColorTexture(512, 0, 384, 256).Write(Path.Combine(Path.GetDirectoryName(pathFolder.Text), "x01.tim"));
            //ctr.GetTrueColorTexture(512, 256, 512, 256).Write(Path.Combine(Path.GetDirectoryName(pathFolder.Text), "x02.tim"));
        }

        private void actionExtract_Click(object sender, EventArgs e)
        {
            if (!DataExists()) return;

            using (var scene = CtrScene.FromFile(pathFile.Text))
            {
                string data = Path.Combine(pathFileParent, "data");

                if (optionTexHigh.Checked) scene.ExportTextures(data, Detail.High);
                if (optionTexMed.Checked) scene.ExportTextures(data, Detail.Med);
                if (optionTexLow.Checked) scene.ExportTextures(data, Detail.Low);
                if (optionTexModels.Checked) scene.ExportTextures(data, Detail.Models);

                //generates colored vram, keep in mind same texture data may use different palettes
                var bmp = new Bitmap(2048, 512);
                var g = Graphics.FromImage(bmp);

                foreach (var x in scene.GetTexturesList())
                {
                    using (var bb = scene.ctrvram.GetTexture(x.Value))
                    {
                        if (bb != null)
                            g.DrawImage(bb, x.Value.RealX * 4 - 2048, x.Value.RealY);
                    }
                }

                bmp.Save(Path.Combine(pathFileParent, "test_color.bmp"));
                scene.ctrvram.SaveBMP(Path.Combine(pathFileParent, "test_bw.bmp"), BMPHeader.GrayScalePalette(16));
            }

            GC.Collect();
            MessageBox.Show("Done!");
        }

        private void actionBrowseFile_Click(object sender, EventArgs e)
        {
            //gotta support ctr here as well i suppose
            ofd.Filter = "CTR file (*.lev, *.mpk)|*.lev;*.mpk|CTR scene file (*.lev)|*.lev|CTR model pack (*.mpk)|*.mpk;";

            if (ofd.ShowDialog() == DialogResult.OK)
                MaybeLoadFile(ofd.FileName);
        }

        private void MaybeLoadFile(string path)
        {
            string ext = Path.GetExtension(path).ToLower();

            if (ext != ".lev" && ext != ".mpk")
            {
                MessageBox.Show("Not a supported CTR file.");
                return;
            }

            pathFile.Text = path;
            pathFolder.Text = Path.Combine(Path.GetDirectoryName(path), "newtex");
            Helpers.CheckFolder(pathFolder.Text);

            string vrampath = Path.ChangeExtension(pathFile.Text, ".vrm");

            if (File.Exists(vrampath))
                pathVram.Text = vrampath;
        }

        private void actionBrowseFolder_Click(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog();

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
            if (pathFolder.Text == "") return;

            if (!Directory.Exists(pathFolder.Text))
            {
                MessageBox.Show($"Folder doesn't exist!\r\n{pathFolder.Text}");
                return;
            }

            Process.Start(pathFolder.Text);
        }

        private void VramControl_DragDrop(object sender, DragEventArgs e)
        {
            string path = ((string[])e.Data.GetData(DataFormats.FileDrop, false))[0];
            MaybeLoadFile(path);
        }

        private void VramControl_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!File.Exists(pathFile.Text))
            {
                MessageBox.Show($"File doesn't exist!\r\n{pathFile.Text}");
                return;
            }

            using (var scn = CtrScene.FromFile(pathFile.Text))
            {
                Tim ctr = scn.ctrvram;

                try
                {
                    using (var br = new BinaryReaderEx(new MemoryStream(StringToByteArrayFastest(textBox1.Text))))
                    {
                        var tl = new TextureLayout(br);
                        pictureBox1.Image = ctr.GetTexture(tl);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Attempt failed.\r\n" + ex.Message);
                }
            }
        }

        public static byte[] StringToByteArrayFastest(string hex)
        {
            hex = hex.Replace(" ", "");

            if (hex.Length % 2 == 1)
                throw new Exception("The binary key cannot have an odd number of digits");

            byte[] arr = new byte[hex.Length >> 1];

            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            }

            return arr;
        }

        public static int GetHexVal(char hex)
        {
            int val = (int)hex;
            //For uppercase A-F letters:
            //return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            //return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }

        private void actionRestore_Click(object sender, EventArgs e) => Helpers.RestoreFile(pathVram.Text);

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Bitmap src = (Bitmap)Bitmap.FromFile(ofd.FileName);
                pictureBox2.Image = src;

                Bitmap dst = new Bitmap(src.Width, src.Height / 2);

                Graphics g = Graphics.FromImage(dst);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.DrawImage(src, new Point(0, 0));

                for (int i = 0; i < dst.Width; i++)
                    for (int j = 0; j < dst.Height; j++)
                    {
                        Color x = src.GetPixel(i, j);
                        Color y = src.GetPixel(i, j + dst.Height);

                        if (x.A == 0)
                            x = Color.FromArgb(0, 0, 0, 0);

                        if (y.A == 0)
                            y = Color.FromArgb(0, 0, 0, 0);

                        Color final = Color.FromArgb(x.R + y.R / 2, x.G + y.G / 2, x.B + y.B / 2);

                        dst.SetPixel(i, j, final);
                    }

                pictureBox3.Image = dst;
            }
        }

        private void actionBrowseVram_Click(object sender, EventArgs e)
        {
            ofd.Filter = "CTR VRAM file (*.vrm)|*.vrm";

            if (ofd.ShowDialog() == DialogResult.OK)
                pathFolder.Text = ofd.FileName;
        }
    }
}
