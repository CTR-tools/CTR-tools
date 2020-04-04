using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using CTRFramework;
using CTRFramework.Shared;
using CTRFramework.Lang;
using CTRFramework.Vram;

namespace levTool
{
    public partial class MainForm : Form
    {
        string path;
        Scene scn;
        ColorDialog cd;

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        public MainForm()
        {
            InitializeComponent();
            cd = new ColorDialog();

            checkedListBox1.Items.AddRange(Enum.GetNames(typeof(QuadFlags)));
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadLEV();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveLEV();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (scn != null)
                if (scn.pickups.Count > 0)
                    propertyGrid1.SelectedObject = scn.pickups[trackBar1.Value];
        }

        string bak;

        private void LoadLEV()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CTR Scene file (*.lev)|*.lev";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                path = ofd.FileName;
                bak = path + ".bak";

                if (!File.Exists(bak))
                    File.Copy(path, bak);

                scn = new Scene(path, "obj");

                Text = String.Format("levTool - {0}", path);
                propertyGrid1.SelectedObject = scn.pickups[0];
                trackBar1.Maximum = scn.pickups.Count - 1;
            }
        }



        private void SaveLEV()
        {
            using (BinaryWriterEx bw = new BinaryWriterEx(File.OpenWrite(path)))
            {

                bw.BaseStream.Position = scn.header.ptrPickupHeaders + 4;

                foreach (PickupHeader ph in scn.pickups)
                    ph.Write(bw);


                bw.BaseStream.Position = scn.meshinfo.ptrVertexArray + 4;

                foreach (Vertex v in scn.verts)
                    v.Write(bw);


                bw.BaseStream.Position = scn.meshinfo.ptrQuadBlockArray + 4;

                foreach (QuadBlock qb in scn.quads)
                    qb.Write(bw);

                bw.BaseStream.Position = scn.header.ptrVcolAnim + 4;

                foreach (VertexAnim vc in scn.vertanims)
                    vc.Write(bw);

                bw.BaseStream.Position = scn.header.ptrAiNav + 4;
                scn.nav.Write(bw);

            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            if (scn != null)
            {
                foreach (PickupHeader ph in scn.pickups)
                {
                    ph.Position.Y += (short)numericUpDown1.Value;
                }

                //lmao
                propertyGrid1.SelectedObject = propertyGrid1.SelectedObject;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (scn != null)
            {
                foreach (PickupHeader ph in scn.pickups)
                {
                    ph.Position.Y -= (short)numericUpDown1.Value;
                }

                //lmao
                propertyGrid1.SelectedObject = propertyGrid1.SelectedObject;
            }
        }



        Color vertexcolor1 = Color.Gray;
        Color vertexcolor2 = Color.Gray;

        private void button2_Click(object sender, EventArgs e)
        {
            if (scn != null)
            {
                foreach (Vertex v in scn.verts)
                {
                    v.SetColor(Vcolor.Default, new Vector4b(vertexcolor1));
                    v.SetColor(Vcolor.Morph, new Vector4b(vertexcolor2));
                }
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (cd.ShowDialog() == DialogResult.OK)
            {
                vertexcolor1 = cd.Color;
                button1.BackColor = vertexcolor1;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (cd.ShowDialog() == DialogResult.OK)
            {
                vertexcolor2 = cd.Color;
                button5.BackColor = vertexcolor2;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (scn != null)
            {
                foreach (Vertex v in scn.verts)
                {
                    v.color.Scale(0.12f, 0.21f, 0.32f, 1f);
                    v.color_morph.Scale(0.12f, 0.21f, 0.32f, 1f);
                }
            }
        }

        private void showConsoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AllocConsole();
        }


        private void button7_Click_1(object sender, EventArgs e)
        {
            if (scn != null)
            {
                foreach (Vertex v in scn.verts)
                {
                    v.color_morph = v.color;
                }
            }
        }


        private void button9_Click(object sender, EventArgs e)
        {
            if (scn != null)
            {
                foreach (QuadBlock qb in scn.quads)
                {
                    qb.quadFlags = GetFlags(checkedListBox1);
                }
            }
        }

        private QuadFlags GetFlags(CheckedListBox clb)
        {
            ushort final = 0;

            for (int i = 0; i < 16; i++)
            {
                ushort x = (ushort)((clb.GetItemChecked(i) ? 1 : 0) << i);
                final |= x;
            }

            return (QuadFlags)final;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (scn != null)
            {
                foreach (QuadBlock qb in scn.quads)
                {
                    qb.offset2 = 0;
                    qb.ptrTexMid = new uint[] { 0, 0, 0, 0 };
                }
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (scn != null)
            {
                StringBuilder sb = new StringBuilder();
                foreach (VisData vi in scn.coldata)
                {
                    sb.Append(vi.ToString() + "\r\n");
                }

                textBox1.Text = sb.ToString();
            }
        }


        Random r = new Random();
        private void Button12_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            List<byte> uniq1 = new List<byte>();
            List<byte> uniq2 = new List<byte>();
            List<byte> uniq3 = new List<byte>();
            List<byte> uniq4 = new List<byte>();

            if (scn != null)
            {
                foreach (QuadBlock qb in scn.quads)
                {
                    /*
                    if (!uniq1.Contains(qb.unk2[0]))
                        uniq1.Add(qb.unk2[0]);
                    if (!uniq2.Contains(qb.unk2[1]))
                        uniq2.Add(qb.unk2[1]);
                    if (!uniq3.Contains(qb.unk2[2]))
                        uniq3.Add(qb.unk2[2]);
                    if (!uniq4.Contains(qb.unk2[3]))
                        uniq4.Add(qb.unk2[3]);
                        */
                    // qb.f1 = 17;
                    //qb.unk2[0] = 8;
                    //qb.unk2[3] = 0;
                    /*
                    qb.ulead = (byte)r.Next(255);

                for (int i = 0; i < 4; i++)
                    qb.uquad[i] = (byte)r.Next(255);
                    */
                }

                uniq1.Sort();
                uniq2.Sort();
                uniq3.Sort();
                uniq4.Sort();

                foreach (byte b in uniq1) sb.Append(b + " ");
                sb.Append("\r\n");
                foreach (byte b in uniq2) sb.Append(b + " ");
                sb.Append("\r\n");
                foreach (byte b in uniq3) sb.Append(b + " ");
                sb.Append("\r\n");
                foreach (byte b in uniq4) sb.Append(b + " ");
                sb.Append("\r\n");

                textBox1.Text = sb.ToString();
            }
        }

        private void Button13_Click(object sender, EventArgs e)
        {
            if (scn != null)
            {
                foreach (QuadBlock qb in scn.quads)
                {
                    qb.terrainFlag = (TerrainFlags)r.Next(20);
                }
            }
        }

        private void Button14_Click(object sender, EventArgs e)
        {
            if (scn != null)
            {
                foreach (QuadBlock qb in scn.quads)
                {
                    qb.WeatherIntensity = 255;
                }
            }
        }

        private void Button15_Click(object sender, EventArgs e)
        {
            if (scn != null)
            {
                foreach (QuadBlock qb in scn.quads)
                {
                    qb.WeatherIntensity = 0;
                }
            }
        }

        private void Button16_Click(object sender, EventArgs e)
        {
            if (scn != null)
            {
                foreach (QuadBlock qb in scn.quads)
                {
                    qb.WeatherIntensity = (byte)r.Next(255);
                }
            }
        }

        private void Button17_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            if (scn != null)
            {
                foreach (QuadBlock qb in scn.quads)
                {
                    qb.bitvalue = 0;
                }
            }

            //textBox1.Text = sb.ToString();
        }

        private void Button18_Click(object sender, EventArgs e)
        {

        }

        private void Button8_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CTR VRAM file|*.vram";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                using (BinaryReaderEx br = new BinaryReaderEx(File.Open(ofd.FileName, FileMode.Open)))
                {
                    Tim buf = CtrVrm.FromReader(br);

                    MessageBox.Show(buf.data.Length / 256 + "");

                    if (scn != null)
                    {
                        Dictionary<string, TextureLayout> tex = scn.GetTexturesList();
                        MessageBox.Show(tex.Count.ToString());

                        foreach (TextureLayout tl in tex.Values)
                        {
                            buf.GetTexturePage(tl, "");
                        }
                    }

                    buf.SaveBMP("test.bmp", BMPHeader.GrayScalePalette(16));
                    //buf.palbmp.Save("palletes.png", System.Drawing.Imaging.ImageFormat.Png);

                    //Process.Start("palletes.png");
                }
            }
        }

        private void button18_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CTR VRAM file|*.vram";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                using (BinaryReaderEx br = new BinaryReaderEx(File.Open(ofd.FileName, FileMode.Open)))
                {
                    Tim buf = CtrVrm.FromReader(br);

                    using (BinaryReaderEx br2 = new BinaryReaderEx(File.Open("ui_map", FileMode.Open)))
                    {
                        List<TexMap> list = new List<TexMap>();

                        for (int i = 0; i < 0xEC; i++)
                        {
                            list.Add(new TexMap(br2));
                        }

                        foreach (TexMap map in list)
                        {
                            buf.GetTexturePage(map.tl, map.id.ToString("X2") + "." + map.name);
                        }
                    }

                    /*
                        Dictionary<string, TextureLayout> tex = scn.GetTexturesList();
                        MessageBox.Show(tex.Count.ToString());
                    }
                    */

                    buf.SaveBMP("test.bmp", BMPHeader.GrayScalePalette(16));
                    //buf.palbmp.Save("palletes.png", System.Drawing.Imaging.ImageFormat.Png);

                    Process.Start("test.bmp");
                }
            }
        }

        private void button19_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd1 = new OpenFileDialog();
            OpenFileDialog ofd2 = new OpenFileDialog();

            if (ofd1.ShowDialog() == DialogResult.OK)
                if (ofd2.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        LNG lng = new LNG(ofd1.FileName);

                        using (BinaryReaderEx br = new BinaryReaderEx(File.OpenRead(ofd2.FileName)))
                        {
                            br.Jump(0x74280);

                            List<LevelSlot> slots = new List<LevelSlot>();

                            for (int i = 0; i < 64; i++)
                                slots.Add(new LevelSlot(br));

                            foreach (LevelSlot s in slots)
                            {
                                textBox2.Text += s.ToString() + lng.entries[s.title_index] + "\r\n";
                            }
                        }
                    }
                    catch
                    {
                        MessageBox.Show("error");
                    }
                }
        }

        private void button20_Click(object sender, EventArgs e)
        {
            if (scn != null)
            {
                if (scn.nav != null)
                {
                    foreach (AIPath ai in scn.nav.paths)
                    {
                        foreach (NavFrame f in ai.frames)
                        {
                            f.unk11 = 0;
                            f.unk2 = 0;
                            f.unk3 = 0;
                        }
                    }

                    textBox1.Text = scn.nav.ToString();
                }
            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd1 = new OpenFileDialog();

            if (ofd1.ShowDialog() == DialogResult.OK)
            {
                using (BinaryReaderEx br = new BinaryReaderEx(File.Open(ofd1.FileName, FileMode.Open)))
                {
                    Mcs m = new Mcs(br);
                }
            }
        }

        private void button22_Click(object sender, EventArgs e)
        {
            if (scn != null)
            {

                StringBuilder sb = new StringBuilder();

                foreach (VertexAnim vc in scn.vertanims)
                {
                    sb.Append(vc.ToString() + "\r\n");
                    vc.RandomizeColors(scn.vertanims[0].unk1, scn.vertanims[0].unk2);
                }

                textBox3.Text = sb.ToString();
            }
        }

        private void restoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void restoreToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (File.Exists(bak))
            {
                File.Delete(path);
                File.Copy(bak, path);
                File.Delete(bak);
                File.Copy(path, bak);
            }
        }

        private void button23_Click(object sender, EventArgs e)
        {
            if (scn != null)
            {
                scn.ExportTextures(@"kek");
            }
        }

        private TreeNode GetOrCreateNode(TreeNode tn, string name)
        {
            foreach (TreeNode n in tn.Nodes)
                if (n.Text == name) return n;

            TreeNode child = new TreeNode(name);
            tn.Nodes.Add(child);

            return child;
        }

        BIG big;

        private void button24_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Crash Team Racing BIG file|*.big";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                treeView1.Nodes.Clear();
                big = null;
                GC.Collect();

                big = new BIG(ofd.FileName);

                TreeNode tn = new TreeNode("bigfile");
                tn.Expand();

                foreach (CTRFile cf in big.files)
                {
                    string[] s = cf.name.Split('\\');

                    TreeNode curnode = tn;

                    for (int i = 0; i < s.Length - 1; i++)
                    {
                        curnode = GetOrCreateNode(curnode, s[i]);
                    }

                    TreeNode final = new TreeNode(s[s.Length - 1]);
                    final.Tag = cf;

                    curnode.Nodes.Add(final);
                }

                treeView1.Nodes.Add(tn);

                //big.Export("data");
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView1.SelectedNode.Tag != null)
            {
                CTRFile cf = treeView1.SelectedNode.Tag as CTRFile;

                if (Path.GetExtension(cf.name) == ".lng")
                {
                    try
                    {
                        File.WriteAllBytes("temp.lng", cf.data);
                        LNG lng = new LNG("temp.lng");
                        textBox4.Text = File.ReadAllText("temp.txt");
                        File.Delete("temp.txt");
                        File.Delete("temp.lng");
                    }
                    catch
                    {
                    }
                }
            }


        }
    }

}