using CTRFramework;
using CTRFramework.Big;
using CTRFramework.Lang;
using CTRFramework.Vram;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace CTRTools.Controls
{
    public partial class BigFileControl : UserControl
    {
        private BigFileReader Reader
        {
            get => _reader;
            set
            {
                _reader = value;
                onReaderUpdated?.Invoke(this, null);
            }
        }

        private BigFileReader _reader;
        private EventHandler onReaderUpdated;


        public BigFileControl()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            onReaderUpdated += LoadFromReader;
        }

        private async void LoadBigFull(string path)
        {
            if (!File.Exists(path))
            {
                MessageBox.Show($"File doesn't exist!\r\n{path}");
                return;
            }

            expandAll.Text = "Expand";
            bigLoader.RunWorkerAsync(path);
        }


        private void LoadFromReader(object sender, EventArgs e)
        {
            TreeNode tn = new TreeNode("root");
            tn.Expand();

            Reader.Reset();

            while (Reader.NextFile())
            {
                if (Reader.FileSize == 0)
                    continue;

                string[] s = Reader.GetFilename().Split('\\');

                TreeNode curnode = tn;

                for (int i = 0; i < s.Length - 1; i++)
                    curnode = GetOrCreateNode(curnode, s[i]);

                TreeNode final = new TreeNode(s[s.Length - 1]);
                final.Tag = Reader.FileCursor;
                
                switch (Path.GetExtension(s[s.Length - 1]))
                {
                    case ".lev": final.ImageIndex = final.SelectedImageIndex = 1; break;
                    case ".vrm":
                    case ".tim": final.ImageIndex = final.SelectedImageIndex = 2; break;
                    case ".ctr": final.ImageIndex = final.SelectedImageIndex = 3; break;
                    case ".bin": final.ImageIndex = final.SelectedImageIndex = 4; break;
                    case ".str": final.ImageIndex = final.SelectedImageIndex = 5; break;
                    case ".mpk": final.ImageIndex = final.SelectedImageIndex = 6; break;
                    case ".ptr": final.ImageIndex = final.SelectedImageIndex = 7; break;
                    case ".lng": final.ImageIndex = final.SelectedImageIndex = 8; break;
                    default: final.ImageIndex = 0; break;
                }

                curnode.Nodes.Add(final);
            }

            if (fileTree.InvokeRequired)
            {
                fileTree.BeginInvoke(new MethodInvoker(delegate { UpdateListBox(tn); }));
            }
            else
            {
                UpdateListBox(tn);
            }
        }

        private void UpdateListBox(TreeNode tn)
        {
            fileTree.BeginUpdate();
            fileTree.Nodes.Clear();
            fileTree.Nodes.Add(tn);
            fileTree.EndUpdate();
        }

        private TreeNode GetOrCreateNode(TreeNode tn, string name)
        {
            foreach (TreeNode n in tn.Nodes)
                if (n.Text == name) return n;

            TreeNode child = new TreeNode(name);
            tn.Nodes.Add(child);

            return child;
        }

        private void exportFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Reader.FileCursor = (int)fileTree.SelectedNode.Tag;
                BigEntry en = Reader.ReadEntry();

                if (en != null)
                {
                    SaveFileDialog fd = new SaveFileDialog();
                    fd.FileName = Path.GetFileName(en.Name);

                    if (fd.ShowDialog() == DialogResult.OK)
                        en.Save(fd.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.ToString());
            }
        }

        private void actionLoadBig_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                LoadBigFull(ofd.FileName);
                expandAll.Text = "Expand";
            }
        }

        private void BigFileControl_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files.Length > 0)
            {
                switch (Path.GetExtension(files[0]).ToLower())
                {
                    case ".big":
                        LoadBigFull(files[0]);
                        break;

                    default:
                        MessageBox.Show("Unsupported file.");
                        break;
                }
            }
        }

        private void actionExportAll_Click(object sender, EventArgs e)
        {
            if (Reader != null)
                if (fbd.ShowDialog() == DialogResult.OK)
                    Reader.ExtractAll(fbd.SelectedPath);
        }

        private void BigFileControl_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                if (fileTree.SelectedNode.Tag != null)
                {
                    Reader.FileCursor = (int)fileTree.SelectedNode.Tag;
                    BigEntry en = Reader.ReadEntry();

                    switch (Path.GetExtension(en.Name).ToLower())
                    {
                        case ".lng":
                            var lng = en.ParseAs<LNG>();
                            fileInfo.Lines = lng.Entries.ToArray();
                            break;

                        case ".lev":
                            var lev = en.ParseAs<CtrScene>();
                            fileInfo.Text = lev.ToString();
                            break;

                        case ".ctr":
                            var pc = en.ParseAs<PatchedContainer>();
                            var ctr = CtrModel.FromReader(pc.GetReader());
                            fileInfo.Text = ctr.ToString();
                            break;

                        case ".vrm":
                            var vrm = en.ParseAs<CtrVrm>();
                            fileInfo.Text = vrm.ToString();
                            break;

                        case ".tim":
                            var tim = en.ParseAs<Tim>();
                            fileInfo.Text = tim.ToString();
                            break;

                        case ".mpk":
                            var mpk = en.ParseAs<ModelPack>();
                            fileInfo.Text = mpk.ToString();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.ToString());
            }
        }

        private void bigLoader_DoWork(object sender, DoWorkEventArgs e)
        {
            Reader = BigFileReader.FromFile(e.Argument as string);
        }

        private void bigLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bigVersion.Text = Reader.Version;
        }

        private void expandAll_Click(object sender, EventArgs e)
        {
            fileTree.BeginUpdate();
            if (expandAll.Text == "Expand")
            {
                fileTree.ExpandAll();
                expandAll.Text = "Collapse";
            }
            else
            {
                fileTree.CollapseAll();
                if (fileTree.Nodes.Count > 0)
                    fileTree.Nodes[0].Expand();
                expandAll.Text = "Expand";
            }
            fileTree.EndUpdate();
        }

        private void fileTree_MouseClick(object sender, MouseEventArgs e)
        {
        }
    }
}