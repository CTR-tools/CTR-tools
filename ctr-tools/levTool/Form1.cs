using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

namespace CTRtools
{
    public partial class Form1 : Form
    {
        string path;
        int numPickupHeaders;
        int ptrPickupHeaders;
        List<PickupHeader> headers = new List<PickupHeader>();

        List<CTRVertex> verts = new List<CTRVertex>();
        CTRPtrInfo info;


        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();


        public Form1()
        {
            //AllocConsole();
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadLEV();
        }


        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (headers.Count > 0)
                propertyGrid1.SelectedObject = headers[trackBar1.Value];
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveLEV();
        }


        private void LoadLEV()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CTR level file (*.lev)|*.lev";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                path = ofd.FileName;

                using (BinaryReader br = new BinaryReader(File.OpenRead(path)))
                {

                    uint size = br.ReadUInt32();
                    uint ptrInfo = br.ReadUInt32() + 4;

                    br.BaseStream.Position += 8;

                    numPickupHeaders = br.ReadInt32();
                    ptrPickupHeaders = br.ReadInt32() + 4;


                    br.BaseStream.Position = ptrInfo;

                    info = new CTRPtrInfo();
                    info.Read(br);

                    br.BaseStream.Position = info.ptrvertarray + 4;

                    for (int i = 0; i < info.vertexnum; i++)
                    {
                        verts.Add(new CTRVertex(br));
                    }


                    br.BaseStream.Position = ptrPickupHeaders;


                    for (int i = 0; i < numPickupHeaders; i++)
                    {
                        PickupHeader h = new PickupHeader(br);
                        Console.WriteLine(h.Name);
                        headers.Add(h);
                    }
                }

                propertyGrid1.SelectedObject = headers[0];
                trackBar1.Maximum = headers.Count - 1;

            }
        }

        private void SaveLEV()
        {
            using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(path)))
            {
                bw.BaseStream.Position = ptrPickupHeaders;

                foreach (PickupHeader ph in headers)
                    ph.Write(bw);


                bw.BaseStream.Position = info.ptrvertarray + 4;

                foreach (CTRVertex v in verts)
                    v.Write(bw);

            }
        }

        Color vertexcolor1 = Color.Gray;
        Color vertexcolor2 = Color.Gray;

        private void button1_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();

            if (cd.ShowDialog() == DialogResult.OK)
            {
                vertexcolor1 = cd.Color;
                button1.BackColor = vertexcolor1;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            foreach (PickupHeader ph in headers)
            {
                ph.Position.Y += (short)numericUpDown1.Value;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            foreach (PickupHeader ph in headers)
            {
                ph.Position.Y -= (short)numericUpDown1.Value;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (verts.Count == 0)
                MessageBox.Show("Load LEV first.");

            foreach (CTRVertex v in verts)
            {
                v.SetColor1(vertexcolor1);
                v.SetColor2(vertexcolor2);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();

            if (cd.ShowDialog() == DialogResult.OK)
            {
                vertexcolor2 = cd.Color;
                button5.BackColor = vertexcolor2;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            foreach (CTRVertex v in verts)
            {
                v.color.X = (byte)(v.color.X * 0.12f);
                v.color.Y = (byte)(v.color.Y * 0.21f);
                v.color.Z = (byte)(v.color.Z * 0.32f);
                v.color2.X = (byte)(v.color2.X * 0.12f);
                v.color2.Y = (byte)(v.color2.Y * 0.21f);
                v.color2.Z = (byte)(v.color2.Z * 0.32f);
            }
        }
    }
}
