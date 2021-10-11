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
            this.DoubleBuffered = true;
        }

        private string pathFileParent
        {
            get
            {
                return Path.GetDirectoryName(pathFile.Text);
            }
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

                //dumping vram before the change
                if (optionDebugVram.Checked)
                    //only dump if file doesn't exist (to compare to earliest version of vram)
                    if (!File.Exists(Path.Combine(pathFileParent, "test_old.bmp")))
                        ctr.SaveBMP(Path.Combine(pathFileParent, "test_old.bmp"), BMPHeader.GrayScalePalette(16));

                Dictionary<string, TextureLayout> list = scn.GetTexturesList();

                foreach (string s in Directory.GetFiles(pathFolder.Text, "*.png"))
                {
                    string tag = Path.GetFileNameWithoutExtension(s);

                    Console.Write($"replacing {tag}... ");

                    if (!list.ContainsKey(tag))
                    {
                        Helpers.Panic(ctr, PanicType.Warning, "unknown texture entry");
                        continue;
                    }

                    Tim newtex = ctr.GetTimTexture(list[tag]);
                    newtex.LoadDataFromBitmap(s);

                    ctr.DrawTim(newtex);

                    Console.WriteLine("done.");
                }

                if (optionDebugVram.Checked)
                    ctr.SaveBMP(Path.Combine(pathFileParent, "test_new.bmp"), BMPHeader.GrayScalePalette(16));


                List<Tim> tims = new List<Tim>();

                throw new Exception("reimplement frames via ctrvram2");
                //foreach (var r in CtrVrm2.frames)
                //{
                //    tims.Add(ctr.GetTrueColorTexture(r));
                //}

                string tempFile = Path.Combine(pathFileParent, "temp.tim");
                string vramFile = Path.ChangeExtension(pathFile.Text, ".vrm");

                Helpers.BackupFile(vramFile);
                File.Delete(vramFile);

                using (BinaryWriterEx bw = new BinaryWriterEx(File.Create(vramFile)))
                {
                    bw.Write((int)0x20);

                    foreach (Tim tim in tims)
                    {
                        tim.Write(tempFile);
                        byte[] x = File.ReadAllBytes(tempFile);

                        bw.Write(x.Length);
                        bw.Write(x);
                    }

                    bw.Write((int)0);

                    bw.Flush();
                    bw.Close();
                }

                File.Delete(tempFile);

                //ctr.GetTrueColorTexture(512, 0, 384, 256).Write(Path.Combine(Path.GetDirectoryName(pathFolder.Text), "x01.tim"));
                //ctr.GetTrueColorTexture(512, 256, 512, 256).Write(Path.Combine(Path.GetDirectoryName(pathFolder.Text), "x02.tim"));
            }

            GC.Collect();
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
                if (optionTexHigh.Checked) s.ExportTextures(Path.Combine(pathFileParent, "texHigh"), Detail.High);
                if (optionTexMed.Checked) s.ExportTextures(Path.Combine(pathFileParent, "texMed"), Detail.Med);
                if (optionTexLow.Checked) s.ExportTextures(Path.Combine(pathFileParent, "texLow"), Detail.Low);
                if (optionTexModels.Checked) s.ExportTextures(Path.Combine(pathFileParent, "texModels"), Detail.Models);

                //generates colored vram, keep in mind same texture data may use different palettes
                Bitmap bmp = new Bitmap(2048, 512);
                Graphics g = Graphics.FromImage(bmp);

                foreach (var x in s.GetTexturesList())
                {
                    using (Bitmap bb = s.ctrvram.GetTexture(x.Value))
                    {
                        if (bb != null)
                        {
                            g.DrawImage(bb, x.Value.RealX * 4 - 2048, x.Value.RealY);
                        }
                    }
                }

                bmp.Save(Path.Combine(pathFileParent, "test_color.bmp"));
                s.ctrvram.SaveBMP(Path.Combine(pathFileParent, "test_bw.bmp"), BMPHeader.GrayScalePalette(16));
            }

            GC.Collect();
            MessageBox.Show("Done!");
        }

        private void actionBrowseFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CTR scene file (*.lev)|*.lev";

            if (ofd.ShowDialog() == DialogResult.OK)
                MaybeLoadFile(ofd.FileName);
        }

        private void MaybeLoadFile(string path)
        {
            if (Path.GetExtension(path).ToLower() != ".lev")
            {
                MessageBox.Show("Not a CTR scene file.");
                return;
            }

            pathFile.Text = path;
            pathFolder.Text = Path.Combine(Path.GetDirectoryName(path), "newtex");
            Helpers.CheckFolder(pathFolder.Text);
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

            using (Scene scn = Scene.FromFile(pathFile.Text))
            {
                Tim ctr = scn.ctrvram;

                try
                {
                    using (BinaryReaderEx br = new BinaryReaderEx(new MemoryStream(StringToByteArrayFastest(textBox1.Text))))
                    {
                        TextureLayout tl = new TextureLayout(br);
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

        private void actionRestore_Click(object sender, EventArgs e)
        {
            Helpers.RestoreFile(Path.ChangeExtension(pathFile.Text, ".vrm"));
        }

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
    }
}
