using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using CTRFramework;
using CTRFramework.Shared;
using CTRFramework.Vram;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using CTRFramework.Lang;
using System.Linq;

namespace CTRTools
{
    public partial class LevControl : UserControl
    {
        Scene scn;

        public LevControl()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

            checkedListBox1.Items.AddRange(Enum.GetNames(typeof(QuadFlags)));
            checkedListBox2.Items.AddRange(Enum.GetNames(typeof(VisDataFlags)));
        }

        private void LevControl_DragDrop(object sender, DragEventArgs e)
        {
            string path = ((string[])e.Data.GetData(DataFormats.FileDrop, false))[0];
        }

        private void LevControl_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (scn != null)
                if (scn.pickups.Count > 0)
                    propertyGrid1.SelectedObject = scn.pickups[trackBar1.Value];
        }

        string path = "";

        private void LoadLEV()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CTR Scene file (*.lev)|*.lev";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                path = ofd.FileName;
                Helpers.BackupFile(path);
                scn = Scene.FromFile(path);

                Text = String.Format("levTool - {0}", path);
                propertyGrid1.SelectedObject = scn.pickups[0];
                trackBar1.Maximum = scn.pickups.Count - 1;
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

        private void button24_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();

            if (cd.ShowDialog() == DialogResult.OK)
            {
                vertexcolor1 = cd.Color;
                button24.BackColor = vertexcolor1;
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
            if (scn != null)
            {
                foreach (Vertex v in scn.verts)
                {
                    v.color.Scale(0.12f, 0.21f, 0.32f, 1f);
                    v.color_morph.Scale(0.12f, 0.21f, 0.32f, 1f);
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
                    qb.mosaicStruct = 0;
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

        private void button12_Click(object sender, EventArgs e)
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

        Random r = new Random();

        private void button13_Click(object sender, EventArgs e)
        {
            if (scn != null)
            {
                foreach (QuadBlock qb in scn.quads)
                {
                    qb.terrainFlag = (TerrainFlags)r.Next(20);
                }
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            if (scn != null)
            {
                foreach (QuadBlock qb in scn.quads)
                {
                    qb.WeatherIntensity = 255;
                }
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (scn != null)
            {
                foreach (QuadBlock qb in scn.quads)
                {
                    qb.WeatherIntensity = 0;
                }
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            if (scn != null)
            {
                foreach (QuadBlock qb in scn.quads)
                {
                    qb.WeatherIntensity = (byte)r.Next(255);
                }
            }
        }

        private void button17_Click(object sender, EventArgs e)
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

        private void button7_Click(object sender, EventArgs e)
        {
            if (scn != null)
            {
                foreach (Vertex v in scn.verts)
                {
                    v.color_morph = v.color;
                }
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

                bw.BaseStream.Position = scn.meshinfo.ptrVisDataArray + 4;

                foreach (VisData v in scn.visdata)
                    v.Write(bw);

                bw.BaseStream.Position = scn.header.ptrAiNav + 4;
                scn.nav.Write(bw);

            }
        }

        private void button8_Click(object sender, EventArgs e)
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

        private void button18_Click(object sender, EventArgs e)
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

        private void button26_Click(object sender, EventArgs e)
        {
            LoadLEV();
        }

        private void button27_Click(object sender, EventArgs e)
        {
            SaveLEV();
        }

        private void button28_Click(object sender, EventArgs e)
        {
            if (scn != null)
            {
                foreach (VisData v in scn.visdata)
                {
                    if (v.IsLeaf)
                    {
                        v.flag = GetVisDataFlags(checkedListBox2);
                    }
                }
            }
        }

        private VisDataFlags GetVisDataFlags(CheckedListBox clb)
        {
            ushort final = 0;

            for (int i = 1; i < 9; i++)
            {
                ushort x = (ushort)((clb.GetItemChecked(i) ? 1 : 0) << (i-1));
                final |= x;
            }

            return (VisDataFlags)final;
        }

        private void button32_Click(object sender, EventArgs e)
        {
            if (path != "")
            {
                Helpers.RestoreFile(path);
                scn = Scene.FromFile(path);
            }
        }

        private void button33_Click(object sender, EventArgs e)
        {
            Random r = new Random();

            /*
            foreach (Vertex v in scn.verts)
            {
                v.coord.X = (short)-v.coord.X;
            }

            foreach (VisData v in scn.visdata)
            {
                v.bbox.Min.X = (short)-v.bbox.Min.X;
                v.bbox.Max.X = (short)-v.bbox.Max.X;

                if (v.bbox.Min.X > v.bbox.Max.X)
                {
                    short x = v.bbox.Min.X;
                    v.bbox.Min.X = v.bbox.Max.X;
                    v.bbox.Max.X = v.bbox.Min.X;
                }
            }

            foreach (PosAng pa in scn.restartPts)
            {
                pa.Position.X = (short)-pa.Position.X;
            }
            */

            foreach (QuadBlock qb in scn.quads)
            {
                if (qb.mosaicStruct != 0)
                    qb.mosaicStruct = 0x3CE48;
            }

            MessageBox.Show("Done.");
        }
    }
}
