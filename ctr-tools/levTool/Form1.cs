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
        string[] files;

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();


        public Form1()
        {

            AllocConsole();

            InitializeComponent();

            files = Directory.GetFiles(textBox2.Text, "*.lev");

            foreach (string s in files)
            {
                comboBox2.Items.Add(Path.GetFileName(s));
            }

            comboBox2.SelectedIndex = 0;

            using (BinaryReader br = new BinaryReader(File.Open(textBox2.Text + "\\" + comboBox2.Items[comboBox2.SelectedIndex].ToString(), FileMode.Open)))
            {
                CTRModel c = new CTRModel(br);
                textBox1.Text = c.ToNgonList();
                //textBox1.Text = c.ToOBJ();
            }

           //c.ToTreeView(treeView1);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CTR level file (*.lev)|*.lev";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                CTRnew cm = new CTRnew(ofd.FileName, "obj", textBox1);
                cm.ToTreeView(treeView1);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            CTRnew c = new CTRnew(textBox2.Text + "\\" + comboBox2.Items[comboBox2.SelectedIndex].ToString(), "obj", textBox1);
        }
    }
}
