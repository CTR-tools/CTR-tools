using CTRFramework;
using CTRFramework.Big;
using CTRFramework.Lang;
using CTRFramework.Shared;
using CTRFramework.Vram;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CTRTools
{
    public partial class MainForm : Form
    {
        string path;
        Scene scn;
        ColorDialog cd;


        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        TextWriter _writer = null;

        public MainForm()
        {
            this.DoubleBuffered = true;
            InitializeComponent();
            cd = new ColorDialog();

            checkedListBox1.Items.AddRange(Enum.GetNames(typeof(QuadFlags)));
            this.Text += " - " + Meta.GetVersionInfo();
            label3.Text = Meta.GetVersionInfo();

            comboBox1.SelectedIndex = 0xF;

            // _writer = new ConsoleHook(txtConsole);
            // Redirect the out Console stream
            // Console.SetOut(_writer);
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

                scn = Scene.FromFile(path);

                Text = String.Format("levTool - {0}", path);
                propertyGrid1.SelectedObject = scn.pickups[0];
                trackBar1.Maximum = scn.pickups.Count - 1;
            }
        }



        private void SaveLEV()
        {
            using (BinaryWriterEx bw = new BinaryWriterEx(File.OpenWrite(path)))
            {

                bw.BaseStream.Position = 4;

                scn.header.Write(bw);

                bw.BaseStream.Position = scn.header.ptrRestartPts + 4;

                foreach (PosAng pa in scn.restartPts)
                    pa.Write(bw);

                bw.BaseStream.Position = scn.header.ptrInstances + 4;

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
                foreach (VisData vi in scn.visdata)
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


        private void Button8_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CTR VRAM file|*.vrm";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Tim buf = CtrVrm.FromFile(ofd.FileName);

                MessageBox.Show(buf.data.Length / 256 + "");

                if (scn != null)
                {
                    Dictionary<string, TextureLayout> tex = scn.GetTexturesList();
                    MessageBox.Show(tex.Count.ToString());

                    foreach (TextureLayout tl in tex.Values)
                    {
                        //buf.GetTexturePage(tl, "");
                    }
                }

                buf.SaveBMP("test.bmp", BMPHeader.GrayScalePalette(16));
                //buf.palbmp.Save("palletes.png", System.Drawing.Imaging.ImageFormat.Png);

                //Process.Start("palletes.png");
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
                scn.ExportTexturesAll(@"kek");
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

        BigFile big;

        private async void button24_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Crash Team Racing BIG file|*.big";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Task load = new Task(() => big = BigFile.FromFile(ofd.FileName));
                load.Start();
                await load;
                LoadBig(ofd.FileName);
            }
        }

        private void LoadBig(string fn)
        {
            treeView1.Nodes.Clear();

            TreeNode tn = new TreeNode("bigfile");
            tn.Expand();

            foreach (BigEntry cf in big.Entries)
            {
                if (cf.Size > 0)
                {
                    string[] s = cf.Name.Split('\\');

                    TreeNode curnode = tn;

                    for (int i = 0; i < s.Length - 1; i++)
                    {
                        curnode = GetOrCreateNode(curnode, s[i]);
                    }

                    TreeNode final = new TreeNode(s[s.Length - 1]);
                    final.Tag = cf;

                    curnode.Nodes.Add(final);
                }
            }

            treeView1.Nodes.Add(tn);
            //treeView1.ExpandAll();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView1.SelectedNode.Tag != null)
            {
                BigEntry cf = treeView1.SelectedNode.Tag as BigEntry;

                if (Path.GetExtension(cf.Name) == ".lng")
                {
                    try
                    {
                        File.WriteAllBytes("temp.lng", cf.Data);
                        LNG lng = new LNG("temp.lng");
                        textBox4.Text = File.ReadAllText("temp.txt");
                        File.Delete("temp.txt");
                        File.Delete("temp.lng");
                    }
                    catch
                    {
                    }
                }

                if (Path.GetExtension(cf.Name) == ".lev")
                {
                    try
                    {
                        File.WriteAllBytes("temp.lev", cf.Data);
                        Scene s = Scene.FromFile("temp.lev");
                        textBox4.Text = s.Info();
                        File.Delete("temp.lev");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }


        }

        private void button25_Click(object sender, EventArgs e)
        {

            int maxpos = 0;

            if (scn != null)
            {
                foreach (PosAng pa in scn.header.startGrid)
                {
                    pa.Position.X -= 1000;
                    pa.Angle.Y += 2048;
                }

                scn.restartPts.Reverse();

                foreach (PosAng pa in scn.restartPts)
                    pa.Angle.Y += 2048;


                foreach (QuadBlock qb in scn.quads)
                {
                    if (qb.trackPos != 0xFF)
                        if (qb.trackPos > maxpos)
                            maxpos = qb.trackPos;
                }

                MessageBox.Show(maxpos + "");

                foreach (QuadBlock qb in scn.quads)
                {
                    if (qb.trackPos != 0xFF)
                        qb.trackPos = (byte)(maxpos - qb.trackPos);
                }
            }
        }


        Mem m;
        Char c;

        private void button26_Click(object sender, EventArgs e)
        {
            m = new Mem("ePSXe");

            if (m.ready)
            {

                uint baseP1ptr = 0;
                uint fpspatch = 0;
                uint fpspatch2 = 0;

                char[] vers1 = m.ReadCharArray(0x8008CFB8, 4);
                char[] vers2 = m.ReadCharArray(0x8008D338, 4);
                char[] vers3 = m.ReadCharArray(0x800903BC, 4);

                if ("ENG\0" == new string(vers1))
                {
                    baseP1ptr = 0x8009900C;
                    fpspatch = 0x80037930;
                    fpspatch2 = 0x8008d2b4;
                    textBox5.Text = "NTSC-U." + "\r\n" + textBox5.Text;
                }
                else if ("ENG\0" == new string(vers2))
                {
                    baseP1ptr = 0x800993CC;
                    textBox5.Text = "PAL." + "\r\n" + textBox5.Text;
                }
                else if ("ENG\0" == new string(vers3))
                {
                    baseP1ptr = 0x8009C4CC;
                    fpspatch = 0x800395f4;
                    fpspatch2 = 0x800906c0;
                    textBox5.Text = "NTSC-J." + "\r\n" + textBox5.Text;
                }
                else
                {
                    textBox5.Text = "Unsupported game/version." + "\r\n" + textBox5.Text;
                    return;
                }


                m.WritePSXUInt16(fpspatch, (ushort)(checkBox1.Checked ? 1 : 2), textBox5);
                m.WritePSXUInt16(fpspatch2, (ushort)(checkBox1.Checked ? 1 : 2), textBox5);

                textBox5.Text = "ePSXe.exe base address: " + m.process.MainModule.BaseAddress.ToString("X8") + "\r\n" + textBox5.Text;

                uint charPtr = m.ReadPSXUInt32(baseP1ptr);
                textBox5.Text = baseP1ptr.ToString("X8") + "\r\n" + textBox5.Text;
                textBox5.Text = charPtr.ToString("X8") + "\r\n" + textBox5.Text;

                using (BinaryReader br = new BinaryReader(new MemoryStream(m.ReadArray(charPtr, 1024))))
                {
                    c = new Char(br);
                    //c.wheelScale = 0x2000;
                    c.curWeapon = (byte)comboBox1.SelectedIndex;
                    c.numCharges = (byte)numericUpDown2.Value;

                    byte[] b = new byte[14 * 4 + 2];
                    using (BinaryWriter bw = new BinaryWriter(new MemoryStream(b)))
                    {
                        c.Write(bw);
                        m.WriteArray(charPtr, b);
                    }

                }

                propertyGrid2.SelectedObject = c;
            }
            else
            {
                textBox5.Text = "Failed to find ePSXe process." + "\r\n" + textBox5.Text;
            }
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files.Length > 0)
            {
                switch (Path.GetExtension(files[0]).ToLower())
                {
                    case ".big": LoadBig(files[0]); break;
                    default: MessageBox.Show("Unsupported file."); break;
                }
            }
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void button27_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                big.Extract(fbd.SelectedPath);
            }
        }

        private void gitHubBox_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/DCxDemo/CTR-tools");
        }

        private void discordBox_Click(object sender, EventArgs e)
        {
            Process.Start("https://discord.gg/56xm9Aj");
        }

        private void button28_Click(object sender, EventArgs e)
        {
            m = new Mem("ePSXe");

            uint levPtr = 0x80083a48;
            uint lev = m.ReadPSXUInt32(levPtr);
            uint size = m.ReadPSXUInt32(lev - 4);
            uint ptrMeshInfo = m.ReadPSXUInt32(lev - 4);

            byte[] meshinfodata = m.ReadArray(ptrMeshInfo, 8 * 4);

            MeshInfo mi;

            using (BinaryReaderEx br = new BinaryReaderEx(new MemoryStream(meshinfodata)))
            {
                mi = new MeshInfo(br);
            }

            ushort ind = m.ReadPSXUInt16(mi.ptrQuadBlockArray);

            textBox5.Text += ind.ToString("X8");

            m.WritePSXUInt32((uint)(mi.ptrVertexArray + ind * 16 + 8), 0, textBox5);
            m.WritePSXUInt32((uint)(mi.ptrVertexArray + ind * 16 + 12), 0, textBox5);


            //m.WriteArray(lev, b);
        }


        private void button18_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CTR VRAM file|*.vrm";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Tim buf = CtrVrm.FromFile(ofd.FileName);
                Bitmap bmp = new Bitmap(160, 80);
                Graphics g = Graphics.FromImage(bmp);

                using (BinaryReaderEx br2 = new BinaryReaderEx(File.Open("ui_map", FileMode.Open)))
                {
                    int z = br2.ReadInt32();
                    List<TexMap> list = new List<TexMap>();

                    for (int i = 0; i < 50; i++)
                        list.Add(new TexMap(br2));


                    int x = 0;
                    int y = 0;

                    foreach (TexMap map in list)
                    {
                        Bitmap b = buf.GetTexture(map.tl, "tex", map.name);

                        g.DrawImage(b, x * 16, y * 16);

                        x++;
                        if (x >= 10)
                        {
                            x = 0;
                            y++;
                        }
                    }

                    bmp.Save("font.png", System.Drawing.Imaging.ImageFormat.Png);
                }

                /*
                    Dictionary<string, TextureLayout> tex = scn.GetTexturesList();
                    MessageBox.Show(tex.Count.ToString());
                }
                */

                buf.SaveBMP("test.bmp", BMPHeader.GrayScalePalette(16));
                //buf.palbmp.Save("palletes.png", System.Drawing.Imaging.ImageFormat.Png);

                Process.Start("font.png");
            }
        }

        private void button29_Click(object sender, EventArgs e)
        {
            Scene s = Scene.FromFile("test.lev");
            s.ExportTexturesAll("test");

            MessageBox.Show("done");
        }

        private void button30_Click(object sender, EventArgs e)
        {
            Scene s = Scene.FromFile("test.lev");

            s.ctrvram.SaveBMP("before.bmp", BMPHeader.GrayScalePalette(16));

            s.ctrvram = new Tim(new Rectangle(0, 0, 1024, 512));

            foreach (var t in s.GetTexturesList())
            {
                if (File.Exists("test\\" + t.Value.Tag() + ".tim"))
                {
                    try
                    {
                        s.ctrvram.DrawTim(Tim.FromFile("test\\" + t.Value.Tag() + ".tim"));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            s.ctrvram.SaveBMP("after.bmp", BMPHeader.GrayScalePalette(16));

            MessageBox.Show("done");
        }

        private void exportFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BigEntry en = (BigEntry)treeView1.SelectedNode.Tag;

            if (en != null)
            {
                SaveFileDialog fd = new SaveFileDialog();
                fd.FileName = Path.GetFileName(en.Name);

                if (fd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.WriteAllBytes(fd.FileName, en.Data);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message + "\r\n" + ex.ToString());
                    }
                }
            }
        }


        private void button31_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                string[] files = Directory.GetFiles(fbd.SelectedPath, "*.lev");

                StringBuilder sb = new StringBuilder();

                foreach (string str in files)
                {
                    using (Scene s = Scene.FromFile(str))
                    {
                        sb.AppendLine(Path.GetFileNameWithoutExtension(str) + "," + s.Statistics());
                    }

                }

                Helpers.WriteToFile(fbd.SelectedPath + "\\stats.csv", sb.ToString());
            }
        }


        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }
    }
}