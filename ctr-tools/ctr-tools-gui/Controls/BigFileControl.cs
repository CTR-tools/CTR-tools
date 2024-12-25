using CTRFramework;
using CTRFramework.Big;
using CTRFramework.Lang;
using CTRFramework.Models;
using CTRFramework.Shared;
using CTRFramework.Vram;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace CTRTools.Controls
{
    public partial class BigFileControl : UserControl
    {
        private string loadedFile = "";

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
            onReaderUpdated += LoadFromReader;
            fileTree.NodeMouseClick += (sender, args) => fileTree.SelectedNode = args.Node;
        }

        private void LoadBigFull(string path)
        {
            if (!File.Exists(path))
            {
                MessageBox.Show($"File doesn't exist!\r\n{path}");
                return;
            }

            expandAll.Text = "Expand";
            bigLoader.RunWorkerAsync(path);

            loadedFile = path;
        }

        TreeNode root;

        private void LoadFromReader(object sender, EventArgs e)
        {
            if (root is null)
                root = new TreeNode("root");

            root.Expand();

            Reader.Reset();

            while (Reader.NextFile())
            {
                if (Reader.FileSize == 0)
                    continue;

                string[] s = Reader.Filename.Split('\\');

                var curnode = root;

                for (int i = 0; i < s.Length - 1; i++)
                    curnode = GetOrCreateNode(curnode, s[i]);

                TreeNode final = new TreeNode(s[s.Length - 1]);
                final.Tag = Reader.FileCursor;

                switch (Path.GetExtension(s[s.Length - 1]).ToUpper())
                {
                    case ".LEV": final.ImageIndex = final.SelectedImageIndex = 1; break;
                    case ".VRM":
                    case ".TIM": final.ImageIndex = final.SelectedImageIndex = 2; break;
                    case ".CTR": final.ImageIndex = final.SelectedImageIndex = 3; break;
                    case ".BIN": final.ImageIndex = final.SelectedImageIndex = 4; break;
                    case ".STR": final.ImageIndex = final.SelectedImageIndex = 5; break;
                    case ".MPK": final.ImageIndex = final.SelectedImageIndex = 6; break;
                    case ".PTR": final.ImageIndex = final.SelectedImageIndex = 7; break;
                    case ".LNG": final.ImageIndex = final.SelectedImageIndex = 8; break;
                    default: final.ImageIndex = final.SelectedImageIndex = 0; break;
                }

                curnode.Nodes.Add(final);
            }

            if (fileTree.InvokeRequired)
            {
                fileTree.BeginInvoke(new MethodInvoker(delegate { UpdateListBox(root); }));
            }
            else
            {
                UpdateListBox(root);
            }
        }

        private void UpdateListBox(TreeNode treeNode)
        {
            fileTree.BeginUpdate();

            fileTree.Nodes.Clear();
            fileTree.Nodes.Add(treeNode);

            fileTree.EndUpdate();
        }

        private TreeNode GetOrCreateNode(TreeNode parent, string name)
        {
            foreach (TreeNode node in parent.Nodes)
                if (node.Text == name) return node;

            var child = new TreeNode(name);
            parent.Nodes.Add(child);

            // TODO: temporary list, should be moved to a text file
            switch (name)
            {
                case "levels":
                    child.ToolTipText = "Contains most levels in the game, including race tracks, battle arenas,\r\n" +
                        "adventure hubs, main menu and adventure selection screens.";
                    break;
                case "packs":
                    child.ToolTipText = "Various game data combined into separate packages,\r\n" +
                        "to be loaded at once in the specific game mode.\r\n" +
                        "Weapons, crates, kart models, fonts, rewards, etc.";
                    break;
                case "overlays":
                    child.ToolTipText = "Additional game code that can be loaded and unloaded dynamically at runtime.";
                    break;
                case "lang":
                    child.ToolTipText = "Localization files to translate the game on the fly.";
                    break;
                case "models":
                    child.ToolTipText = "Various instanced models, including karts models,\r\n" +
                        "podium scenes and boss hub custscenes.";
                    break;
                case "screen":
                    child.ToolTipText = "Copyright loading screens used during the initial game loading.";
                    break;
                case "cutscenes":
                    child.ToolTipText = "Intro and outro cutscenes, including Oxide monologues.";
                    break;
                case "credits":
                    child.ToolTipText = "All the credits scenes, a scene per character.";
                    break;
                case "thumbs":
                    child.ToolTipText = "Level preview movies on the level selection screen.";
                    break;
            }

            return child;
        }

        private void exportFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Reader.FileCursor = (int)fileTree.SelectedNode.Tag;
                var entry = Reader.ReadEntry();

                if (entry != null)
                {
                    var sfd = new SaveFileDialog();
                    sfd.FileName = Path.GetFileName(entry.Name);

                    if (sfd.ShowDialog() == DialogResult.OK)
                        entry.Save(sfd.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.ToString());
            }
        }

        private void BigFileControl_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files.Length > 0)
            {
                switch (Path.GetExtension(files[0]).ToUpper())
                {
                    case ".BIG":
                        LoadBigFull(files[0]);
                        break;

                    default:
                        MessageBox.Show("Unsupported file.");
                        break;
                }
            }
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
                    var entry = Reader.ReadEntry();

                    switch (Path.GetExtension(entry.Name).ToUpper())
                    {
                        case ".LNG":
                            var lng = entry.ParseAs<LNG>();
                            fileInfo.Lines = lng.Entries.ToArray();
                            break;

                        case ".LEV":
                            var lev = entry.ParseAs<CtrScene>();
                            fileInfo.Text = lev.ToString();
                            break;

                        case ".CTR":
                            var pc = entry.ParseAs<PatchedContainer>();
                            var ctr = CtrModel.FromReader(pc.GetReader());
                            fileInfo.Text = ctr.ToString();
                            break;

                        case ".VRM":
                            var vrm = entry.ParseAs<CtrVrm>();
                            fileInfo.Text = vrm.ToString();
                            break;

                        case ".TIM":
                            var tim = entry.ParseAs<Tim>();
                            fileInfo.Text = tim.ToString();
                            break;

                        case ".MPK":
                            var mpk = entry.ParseAs<ModelPack>();
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

        private void fileTree_DoubleClick(object sender, EventArgs e)
        {
            if ((sender as TreeView).SelectedNode.Tag is null)
                return;

            _reader.FileCursor = (int)(sender as TreeView).SelectedNode.Tag;
            sfd.FileName = Path.GetFileName(_reader.Filename);

            if (sfd.ShowDialog() == DialogResult.OK)
                Helpers.WriteToFile(sfd.FileName, _reader.ReadEntry().Data);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                LoadBigFull(ofd.FileName);
                expandAll.Text = "Expand";
            }
        }

        private void extractToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Reader is null) return;

            if (fbd.ShowDialog() == DialogResult.OK)
                Reader.ExtractAll(fbd.SelectedPath);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void fileTree_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void saveAsZIPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Reader is null) return;

            zfd.FileName = Path.ChangeExtension(Path.GetFileName(loadedFile), ".zip");

            try
            {
                if (zfd.ShowDialog() == DialogResult.OK)
                {
                    using (var big = BigFile.FromBigReader(Reader))
                    {
                        big.ToZip(zfd.FileName);
                    }

                    MessageBox.Show("Done.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Can't write ZIP: {ex.Message}");
            }
        }
    }
}