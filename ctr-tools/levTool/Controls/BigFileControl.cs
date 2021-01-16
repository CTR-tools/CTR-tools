using System;
using System.Windows.Forms;
using System.IO;
using CTRFramework;
using CTRFramework.Big;
using CTRFramework.Lang;
using System.Threading.Tasks;


namespace CTRTools.Controls
{
    public partial class BigFileControl : UserControl
    {
        public BigFileControl()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        private void CtrToolsVramControl_DragDrop(object sender, DragEventArgs e)
        {
            string path = ((string[])e.Data.GetData(DataFormats.FileDrop, false))[0];


        }


        BigFile big;

        private async void LoadBigFull(string path)
        {
            if (!File.Exists(path))
            {
                MessageBox.Show($"File doesn't exist!\r\n{path}");
                return;
            }

            Task load = new Task(() => big = BigFile.FromFile(path));
            load.Start();
            await load;
            LoadBig(path);

            MessageBox.Show("Done.");
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

        private void actionLoadBig_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Crash Team Racing BIG file|*.big";

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
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                big.Extract(fbd.SelectedPath);
            }
        }

        private void BigFileControl_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
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
    }
}
