﻿using CTRFramework;
using CTRFramework.Lang;
using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Text;
using System.Windows.Forms;

namespace CTRTools.Controls
{
    public partial class LevControl : UserControl
    {
        CtrScene scn;

        public LevControl()
        {
            InitializeComponent();

            checkedListBox1.Items.AddRange(Enum.GetNames(typeof(QuadFlags)));
            checkedListBox2.Items.AddRange(Enum.GetNames(typeof(VisNodeFlags)));
        }

        private void LevControl_DragDrop(object sender, DragEventArgs e)
        {
            string path = ((string[])e.Data.GetData(DataFormats.FileDrop, false))[0];
            LoadLEV(path);
        }

        private void LevControl_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (scn != null)
                if (scn.Instances.Count > 0)
                    propertyGrid1.SelectedObject = scn.Instances[trackBar1.Value];
        }

        string path = "";

        private void LoadLEV(string filename)
        {
            try
            {
                path = filename;
                Helpers.BackupFile(path);
                scn = CtrScene.FromFile(path);

                propertyGrid1.SelectedObject = null;

                if (scn.Instances.Count > 0)
                {
                    propertyGrid1.SelectedObject = scn.Instances[0];
                    trackBar1.Maximum = scn.Instances.Count - 1;
                }

                vertexArrayControl1.Scene = scn;

                propertyGrid2.SelectedObject = scn.header;

                LoadGradient();

                colorTop.BackColor = VectorToColor(scn.header.bgColorTop);
                colorBottom.BackColor = VectorToColor(scn.header.bgColorBottom);
                colorBack.BackColor = VectorToColor(scn.header.backColor);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}\r\n{ex}");
            }
        }

        private void moveAllButton_Click(object sender, EventArgs e)
        {
            if (scn != null)
            {
                foreach (CtrInstance ph in scn.Instances)
                {
                    ph.Pose.Move(new Vector3(0, (float)numericUpDown1.Value / 100f, 0));
                }

                //lmao
                propertyGrid1.SelectedObject = propertyGrid1.SelectedObject;
            }
        }



        private void button9_Click(object sender, EventArgs e)
        {
            if (scn is null) return;

            foreach (QuadBlock qb in scn.quads)
                qb.quadFlags = GetFlags(checkedListBox1);
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
            if (scn is null) return;

            foreach (QuadBlock qb in scn.quads)
            {
                qb.ptrPvsStruct = PsxPtr.Zero;
                qb.ptrTexMid = new PsxPtr[] { PsxPtr.Zero, PsxPtr.Zero, PsxPtr.Zero, PsxPtr.Zero };
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (scn is null) return;

            StringBuilder sb = new StringBuilder();

            foreach (var node in scn.visdata)
                sb.Append(node.ToString() + "\r\n");

            textBox2.Text = sb.ToString();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (scn is null) return;

            StringBuilder sb = new StringBuilder();

            List<byte> uniq1 = new List<byte>();
            List<byte> uniq2 = new List<byte>();
            List<byte> uniq3 = new List<byte>();
            List<byte> uniq4 = new List<byte>();

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

            textBox2.Text = sb.ToString();
        }


        private void button13_Click(object sender, EventArgs e)
        {
            if (scn is null) return;

            foreach (var qb in scn.quads)
                qb.terrainFlag = (TerrainFlags)Helpers.Random.Next(20);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            if (scn is null) return;

            foreach (var qb in scn.quads)
                qb.WeatherIntensity = 255;
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (scn is null) return;

            foreach (var qb in scn.quads)
                qb.WeatherIntensity = 0;
        }

        private void button16_Click(object sender, EventArgs e)
        {
            if (scn != null)
            {
                foreach (var qb in scn.quads)
                {
                    qb.WeatherIntensity = (byte)Helpers.Random.Next(255);
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
                    qb.midunk = 0;
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
                    foreach (NavPath ai in scn.nav.paths)
                    {
                        foreach (NavFrame f in ai.Frames)
                        {
                            f.unk11 = 0;
                            f.unk2 = 0;
                            f.unk3 = 0;
                        }
                    }

                    textBox2.Text = scn.nav.ToString();
                }
            }
        }


        private void SaveLEV()
        {
            using (var bw = new BinaryWriterEx(File.OpenWrite(path)))
            {
                //update scene data with controls data
                //scn.verts = vertexArrayControl1.VertexArray;

                bw.Jump(scn.header.ptrRespawnPts.Address + 4);

                foreach (var pose in scn.respawnPts)
                    pose.Write(bw);


                bw.Jump(scn.header.ptrInstances.Address + 4);

                foreach (var ph in scn.Instances)
                    ph.Write(bw);


                bw.Jump(scn.mesh.ptrVertices + 4);

                foreach (var vert in scn.verts)
                    vert.Write(bw);


                bw.Jump(scn.mesh.ptrQuadBlocks + 4);

                foreach (var qb in scn.quads)
                    qb.Write(bw);

                /*
                bw.Jump(scn.header.ptrVcolAnim.Address + 4);

                foreach (VertexAnim vc in scn.vertanims)
                    vc.Write(bw);
                */

                bw.Jump(scn.mesh.ptrVisData + 4);

                foreach (var vis in scn.visdata)
                    vis.Write(bw);

                if (scn.nav != null)
                {
                    //bw.Jump(scn.header.ptrNavData.Address + 4);
                    //scn.nav.Write(bw);
                }

                // header should be written in the end after we have updated all pointers accordingly

                bw.Jump(4);

                scn.header.Write(bw);
            }
        }

        private void button25_Click(object sender, EventArgs e)
        {
            int maxpos = 0;

            if (scn != null)
            {
                foreach (Pose pa in scn.header.startGrid)
                {
                    pa.Move(new Vector3(1000, 0, 0));
                    pa.Rotate(new Vector3(0.5f, 0, 0));
                }

                scn.respawnPts.Reverse();

                // foreach (var restart in scn.restartPts)
                //     pa.Rotate(new Vector3(0.5f, 0, 0));


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

        private void actionLoadLev(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CTR Scene file (*.lev)|*.lev";

            if (ofd.ShowDialog() == DialogResult.OK)
                LoadLEV(ofd.FileName);
        }

        private void actionSaveLev(object sender, EventArgs e)
        {
            SaveLEV();
        }

        private void button28_Click(object sender, EventArgs e)
        {
            if (scn != null)
            {
                foreach (var v in scn.visdata)
                {
                    if (v.IsLeaf)
                    {
                        v.flag = GetVisDataFlags(checkedListBox2);
                    }
                }
            }
        }

        private VisNodeFlags GetVisDataFlags(CheckedListBox clb)
        {
            ushort final = 0;

            for (int i = 1; i < 9; i++)
            {
                ushort x = (ushort)((clb.GetItemChecked(i) ? 1 : 0) << (i - 1));
                final |= x;
            }

            return (VisNodeFlags)final;
        }

        private void actionRestoreLev(object sender, EventArgs e)
        {
            if (path != "")
            {
                Helpers.RestoreFile(path);
                scn = CtrScene.FromFile(path);
            }
        }

        private void actionExportObj(object sender, EventArgs e)
        {
            if (scn is null) return;

            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = Path.GetDirectoryName(path);

            if (fbd.ShowDialog() == DialogResult.OK)
                scn.Export(fbd.SelectedPath, ExportFlags.All);
        }

        private void LoadGradient()
        {
            if (scn is null) return;

            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            Graphics g = Graphics.FromImage(pictureBox1.Image);

            for (int i = 0; i < 3; i++)
                g.FillRectangle(GetGradient(scn.header.glowGradients[i], 32), new Rectangle(0, 32 * i, 128, 32));

            pictureBox1.Invalidate();

            propertyGrid2.SelectedObject = scn.header;
        }


        private void button4_Click(object sender, EventArgs e)
        {
            LoadGradient();
        }

        private System.Drawing.Drawing2D.LinearGradientBrush GetGradient(Gradient grad, int size)
        {
            var from = grad.FromColor;
            var to = grad.ToColor;

            Color cfrom = Color.FromArgb(255, from.X, from.Y, from.Z);
            Color cto = Color.FromArgb(255, to.X, to.Y, to.Z);

            return new System.Drawing.Drawing2D.LinearGradientBrush(new Point(0, 0), new Point(0, size), cfrom, cto);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog();

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private void button18_Click_1(object sender, EventArgs e)
        {
            if (scn is null)
                return;

            foreach (VisNode v in scn.visdata)
            {
                v.flag = v.flag | VisNodeFlags.Unk5 | VisNodeFlags.Unk2 | VisNodeFlags.Subdiv4x1 | VisNodeFlags.Subdiv4x2;
            }
        }

        private void scaleButton_Click(object sender, EventArgs e)
        {
            float scale = trackBar2.Value / 100f;

            foreach (var vertex in scn.verts)
            {
                vertex.Position *= scale;
            }

            foreach (var pickup in scn.Instances)
            {
                pickup.Pose.Position *= scale;
                pickup.Scale *= scale;
            }

            foreach (var quad in scn.quads)
            {
                quad.bbox.Max.Scale(scale);
                quad.bbox.Min.Scale(scale);

                for (int i = 0; i < quad.faceNormal.Count; i++)
                {
                    //no idea why this works
                    quad.faceNormal[i] /= (scale * scale);
                }
            }


            foreach (var vis in scn.visdata)
            {
                vis.bbox.Min.Scale(scale);
                vis.bbox.Max.Scale(scale);
            }


            foreach (var pos in scn.header.startGrid)
            {
                pos.Position *= scale;
            }

            foreach (var pos in scn.respawnPts)
            {
                pos.Pose.Position *= scale;
            }

            foreach (var nav in scn.nav.paths)
            {
                nav.start.position *= scale;

                foreach (var frame in nav.Frames)
                    frame.position *= scale;
            }
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            scaleButton.Text = $"Scale by {trackBar2.Value}%";
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (scn is null)
                return;

            foreach (var pt in scn.respawnPts)
            {
                //pt.AngleY = 0;

                pt.left = 0;
                pt.right = 0;
            }

            foreach (var quad in scn.quads)
            {
                //if (quad.visDataFlags.HasFlag(VisDataFlags.Subdiv4x2))
                //    quad.SetFaceColor(scn.verts, new Vector4b(255, 0, 0, 0));
            }
        }

        private void colorTop_Click(object sender, EventArgs e)
        {
            if (scn is null) return;

            if (cd.ShowDialog() == DialogResult.OK)
            {
                colorTop.BackColor = cd.Color;
                scn.header.bgColorTop = ColorToVector(cd.Color);
            }
        }

        private void colorBottom_Click(object sender, EventArgs e)
        {
            if (scn is null) return;

            if (cd.ShowDialog() == DialogResult.OK)
            {
                colorBottom.BackColor = cd.Color;
                scn.header.bgColorBottom = ColorToVector(cd.Color);
            }
        }

        private void colorBack_Click(object sender, EventArgs e)
        {
            if (scn is null) return;

            if (cd.ShowDialog() == DialogResult.OK)
            {
                colorBack.BackColor = cd.Color;
                scn.header.backColor = ColorToVector(cd.Color);
            }
        }

        private Vector4b ColorToVector(Color color)
        {
            return new Vector4b(color.R, color.G, color.B, 0);
        }

        private Color VectorToColor(Vector4b vec)
        {
            return Color.FromArgb(255, vec.X, vec.Y, vec.Z);
        }
    }
}