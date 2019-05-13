using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CTRtools;
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

            propertyGrid1.SelectedObject = headers[0];
            trackBar1.Maximum = headers.Count - 1;
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

                    br.BaseStream.Position = 0x10;

                    numPickupHeaders = br.ReadInt32();
                    ptrPickupHeaders = br.ReadInt32() + 4;

                    br.BaseStream.Position = ptrPickupHeaders;

                    for (int i = 0; i < numPickupHeaders; i++)
                    {
                        PickupHeader h = new PickupHeader(br);
                        Console.WriteLine(h.Name);
                        headers.Add(h);
                    }
                }
            }
        }

        private void SaveLEV()
        {
            using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(path)))
            {
                bw.BaseStream.Position = ptrPickupHeaders;

                foreach (PickupHeader ph in headers)
                    ph.Write(bw);
            }
        }

        private void moveAllUp10ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (PickupHeader ph in headers)
            {
                ph.Position.Y -= 100;
            }
        }
    }
}
