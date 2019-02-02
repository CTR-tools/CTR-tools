using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public static Bitmap bmp;

        CTRVRAM ctr;
        PsxVram pv = new PsxVram();

        Point palpos = new Point(32, 253);

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.KeyPreview = true;

            selMode = BitMode.Indexed4;

            switch (selMode)
            {
                case BitMode.Indexed4: selBitMode.SelectedIndex = 0; break;
                case BitMode.Indexed8: selBitMode.SelectedIndex = 1; break;
                case BitMode.Colored16: selBitMode.SelectedIndex = 2; break;
                default: selBitMode.SelectedIndex = 0; break;
            }
        }


        private void OpenFile(string s)
        {
            pv.Clear();
            pv.fn = s;

            ctr = new CTRVRAM();

            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(s)))
            using (BinaryReader br = new BinaryReader(ms))
            {
                try
                {
                    ctr.Read(br);
                }
                catch
                {
                    try
                    {
                        br.BaseStream.Position = 0;

                        Tim t = new Tim();
                        t.Read(br);

                        MemPage p = new MemPage();
                        p.tim = t;

                        ctr.pages.Add(p);
                        ctr.pagesCount = ctr.pages.Count;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("File not supported.\r\n" + ex.Message + "\r\n" + ex.ToString());
                    }
                }
            }

            foreach (MemPage p in ctr.pages)
                pv.WriteTim(p.tim);

            bmp = pv.ToBitmap();

            Invalidate();

            ctr = null;
        }



        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (bmp != null) 
                e.Graphics.DrawImage(bmp, new Point(0, 0));

            e.Graphics.DrawRectangle(Pens.White, new Rectangle(squareX * stepPix, squareY * stepPix, horzPix, vertPix));
            e.Graphics.DrawRectangle(Pens.Black, new Rectangle(squareX * stepPix-1, squareY * stepPix-1, horzPix+2, vertPix+2));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            palpos.Y++;
            Invalidate();
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files) OpenFile(file);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.S && e.Control)
            {
                pv.ToBitmap().Save(pv.fn + ".png");
            }

            if (e.KeyCode == Keys.Down)
            {
                vertPix += 8;
            }

            if (e.KeyCode == Keys.Up)
            {
                if (vertPix > 0) vertPix -= 8;
            }

            Invalidate();
        }

        string title = "CTR VRAM (drop .vram file to view, Ctrl+S to save)";

        int squareX = 0;
        int squareY = 0;

        int horzPix = 16;
        int vertPix = 64;

        int stepPix = 16;

        BitMode selMode = BitMode.Indexed4;

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            this.Text = String.Format(title + " {0}", e.X / horzPix + ":" + e.Y / vertPix);

            if (
                    pv.tim.region.IntersectsWith(new Rectangle(e.X, 0, 1, 1)) &&
                    pv.tim.region.IntersectsWith(new Rectangle(0, e.Y, 1, 1))
                )
            {
                squareX = e.X / stepPix;
                squareY = e.Y / stepPix;
            }

            Invalidate();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (selBitMode.SelectedText)
            {
                case "Indexed4": selMode = BitMode.Indexed4; break;
                case "Indexed8": selMode = BitMode.Indexed8; break;
                case "Colored16": selMode = BitMode.Colored16; break;
            }
        }

    }
}
