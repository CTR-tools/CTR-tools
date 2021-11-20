using CTRFramework;
using CTRFramework.Big;
using CTRFramework.Lang;
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

            bigLoader.RunWorkerAsync(path);
        }


        private void LoadFromReader(object sender, EventArgs e)
        {
            TreeNode tn = new TreeNode("root");
            tn.Expand();

            Reader.Reset();

            while (Reader.NextFile())
            {
                string[] s = Reader.GetFilename().Split('\\');

                TreeNode curnode = tn;

                for (int i = 0; i < s.Length - 1; i++)
                    curnode = GetOrCreateNode(curnode, s[i]);

                TreeNode final = new TreeNode(s[s.Length - 1]);
                final.Tag = Reader.FileCursor;

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
                LoadBigFull(ofd.FileName);
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
                            LNG lng = en.ParseAs<LNG>();
                            fileInfo.Lines = lng.Entries.ToArray();
                            break;

                        case ".lev":
                            Scene lev = en.ParseAs<Scene>();
                            fileInfo.Text = lev.ToString();
                            break;

                        case ".ctr":
                            try
                            {
                                PatchedContainer pc = en.ParseAs<PatchedContainer>();
                                CtrModel ctr = CtrModel.FromReader(pc.GetReader());
                                fileInfo.Text = ctr.ToString();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"this fails? {ex.Message}\r\n{ex}");
                            }
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
        }

        private void expandAll_Click(object sender, EventArgs e)
        {
            if (expandAll.Text == "Expand")
            {
                fileTree.ExpandAll();
                expandAll.Text = "Collapse";
            }
            else
            {
                fileTree.CollapseAll();
                expandAll.Text = "Expand";
            }
        }

        private void fileTree_MouseClick(object sender, MouseEventArgs e)
        {
        }
    }
}