using System.Windows.Forms;

namespace CTRTools.Controls
{
    partial class BigFileControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BigFileControl));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.fileTree = new System.Windows.Forms.TreeView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exportFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendToTabctrToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bigIcons = new System.Windows.Forms.ImageList(this.components);
            this.fileInfo = new System.Windows.Forms.TextBox();
            this.ofd = new System.Windows.Forms.OpenFileDialog();
            this.fbd = new System.Windows.Forms.FolderBrowserDialog();
            this.bigLoader = new System.ComponentModel.BackgroundWorker();
            this.expandAll = new System.Windows.Forms.Button();
            this.bigVersion = new System.Windows.Forms.Label();
            this.sfd = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsZIPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.zfd = new System.Windows.Forms.SaveFileDialog();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.splitContainer1);
            this.groupBox1.Location = new System.Drawing.Point(3, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(634, 420);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Big File";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(6, 19);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.fileTree);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.fileInfo);
            this.splitContainer1.Size = new System.Drawing.Size(622, 395);
            this.splitContainer1.SplitterDistance = 258;
            this.splitContainer1.TabIndex = 0;
            // 
            // fileTree
            // 
            this.fileTree.ContextMenuStrip = this.contextMenuStrip1;
            this.fileTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileTree.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.fileTree.HideSelection = false;
            this.fileTree.ImageIndex = 0;
            this.fileTree.ImageList = this.bigIcons;
            this.fileTree.Location = new System.Drawing.Point(0, 0);
            this.fileTree.Name = "fileTree";
            this.fileTree.SelectedImageIndex = 0;
            this.fileTree.ShowNodeToolTips = true;
            this.fileTree.Size = new System.Drawing.Size(258, 395);
            this.fileTree.TabIndex = 5;
            this.fileTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.fileTree.DoubleClick += new System.EventHandler(this.fileTree_DoubleClick);
            this.fileTree.MouseClick += new System.Windows.Forms.MouseEventHandler(this.fileTree_MouseClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportFileToolStripMenuItem,
            this.sendToTabctrToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(135, 48);
            // 
            // exportFileToolStripMenuItem
            // 
            this.exportFileToolStripMenuItem.Name = "exportFileToolStripMenuItem";
            this.exportFileToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.exportFileToolStripMenuItem.Text = "Export file";
            // 
            // sendToTabctrToolStripMenuItem
            // 
            this.sendToTabctrToolStripMenuItem.Name = "sendToTabctrToolStripMenuItem";
            this.sendToTabctrToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.sendToTabctrToolStripMenuItem.Text = "Send to tab";
            // 
            // bigIcons
            // 
            this.bigIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("bigIcons.ImageStream")));
            this.bigIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.bigIcons.Images.SetKeyName(0, "folder.png");
            this.bigIcons.Images.SetKeyName(1, "lev.png");
            this.bigIcons.Images.SetKeyName(2, "vrm.png");
            this.bigIcons.Images.SetKeyName(3, "ctr.png");
            this.bigIcons.Images.SetKeyName(4, "bin.png");
            this.bigIcons.Images.SetKeyName(5, "str.png");
            this.bigIcons.Images.SetKeyName(6, "mpk.png");
            this.bigIcons.Images.SetKeyName(7, "ptr.png");
            this.bigIcons.Images.SetKeyName(8, "lng.png");
            // 
            // fileInfo
            // 
            this.fileInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileInfo.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.fileInfo.Location = new System.Drawing.Point(0, 0);
            this.fileInfo.Multiline = true;
            this.fileInfo.Name = "fileInfo";
            this.fileInfo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.fileInfo.Size = new System.Drawing.Size(360, 395);
            this.fileInfo.TabIndex = 7;
            this.fileInfo.WordWrap = false;
            // 
            // ofd
            // 
            this.ofd.Filter = "Crash Team Racing BIG file (*.big)|*.big";
            // 
            // fbd
            // 
            this.fbd.Description = "Select path to extract:";
            // 
            // bigLoader
            // 
            this.bigLoader.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bigLoader_DoWork);
            this.bigLoader.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bigLoader_RunWorkerCompleted);
            // 
            // expandAll
            // 
            this.expandAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.expandAll.Location = new System.Drawing.Point(3, 453);
            this.expandAll.Name = "expandAll";
            this.expandAll.Size = new System.Drawing.Size(96, 24);
            this.expandAll.TabIndex = 10;
            this.expandAll.Text = "Expand";
            this.expandAll.UseVisualStyleBackColor = true;
            this.expandAll.Click += new System.EventHandler(this.expandAll_Click);
            // 
            // bigVersion
            // 
            this.bigVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bigVersion.AutoSize = true;
            this.bigVersion.Location = new System.Drawing.Point(105, 459);
            this.bigVersion.Name = "bigVersion";
            this.bigVersion.Size = new System.Drawing.Size(0, 13);
            this.bigVersion.TabIndex = 11;
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.Transparent;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.menuStrip1.Size = new System.Drawing.Size(640, 24);
            this.menuStrip1.TabIndex = 12;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveAsZIPToolStripMenuItem,
            this.extractToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // extractToolStripMenuItem
            // 
            this.extractToolStripMenuItem.Name = "extractToolStripMenuItem";
            this.extractToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.extractToolStripMenuItem.Text = "Extract all";
            this.extractToolStripMenuItem.Click += new System.EventHandler(this.extractToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // saveAsZIPToolStripMenuItem
            // 
            this.saveAsZIPToolStripMenuItem.Name = "saveAsZIPToolStripMenuItem";
            this.saveAsZIPToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveAsZIPToolStripMenuItem.Text = "Save as ZIP";
            this.saveAsZIPToolStripMenuItem.Click += new System.EventHandler(this.saveAsZIPToolStripMenuItem_Click);
            // 
            // zfd
            // 
            this.zfd.Filter = "ZIP files (*.zip)|*.zip";
            // 
            // BigFileControl
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.bigVersion);
            this.Controls.Add(this.expandAll);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.Name = "BigFileControl";
            this.Size = new System.Drawing.Size(640, 480);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.BigFileControl_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.BigFileControl_DragEnter);
            this.groupBox1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GroupBox groupBox1;
        private TreeView fileTree;
        private TextBox fileInfo;
        private SplitContainer splitContainer1;
        private OpenFileDialog ofd;
        private FolderBrowserDialog fbd;
        private System.ComponentModel.BackgroundWorker bigLoader;
        private Button expandAll;
        private Label bigVersion;
        private ImageList bigIcons;
        private SaveFileDialog sfd;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem extractToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem exportFileToolStripMenuItem;
        private ToolStripMenuItem sendToTabctrToolStripMenuItem;
        private ToolStripMenuItem saveAsZIPToolStripMenuItem;
        private ColorDialog colorDialog1;
        private SaveFileDialog zfd;
    }
}
