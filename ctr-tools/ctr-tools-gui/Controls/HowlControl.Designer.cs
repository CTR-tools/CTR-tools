using System.Windows.Forms;

namespace CTRTools.Controls
{
    partial class HowlControl
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabSongs = new System.Windows.Forms.TabPage();
            this.songListBox = new System.Windows.Forms.ListBox();
            this.tabBanks = new System.Windows.Forms.TabPage();
            this.banksTreeView = new System.Windows.Forms.TreeView();
            this.tabSampleTable = new System.Windows.Forms.TabPage();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.sampleTableListBox = new System.Windows.Forms.ListBox();
            this.fbd = new System.Windows.Forms.FolderBrowserDialog();
            this.ofd = new System.Windows.Forms.OpenFileDialog();
            this.sfd = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabSongs.SuspendLayout();
            this.tabBanks.SuspendLayout();
            this.tabSampleTable.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.tabControl1);
            this.groupBox1.Location = new System.Drawing.Point(3, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(634, 450);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Howl";
            this.groupBox1.DragDrop += new System.Windows.Forms.DragEventHandler(this.HowlControl_DragDrop);
            this.groupBox1.DragEnter += new System.Windows.Forms.DragEventHandler(this.HowlControl_DragEnter);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "label2";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "label1";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabSongs);
            this.tabControl1.Controls.Add(this.tabBanks);
            this.tabControl1.Controls.Add(this.tabSampleTable);
            this.tabControl1.Location = new System.Drawing.Point(6, 45);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(622, 399);
            this.tabControl1.TabIndex = 16;
            // 
            // tabSongs
            // 
            this.tabSongs.Controls.Add(this.songListBox);
            this.tabSongs.Location = new System.Drawing.Point(4, 22);
            this.tabSongs.Name = "tabSongs";
            this.tabSongs.Padding = new System.Windows.Forms.Padding(3);
            this.tabSongs.Size = new System.Drawing.Size(614, 373);
            this.tabSongs.TabIndex = 0;
            this.tabSongs.Text = "tabSongs";
            this.tabSongs.UseVisualStyleBackColor = true;
            // 
            // songListBox
            // 
            this.songListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.songListBox.FormattingEnabled = true;
            this.songListBox.Location = new System.Drawing.Point(3, 6);
            this.songListBox.Name = "songListBox";
            this.songListBox.Size = new System.Drawing.Size(605, 355);
            this.songListBox.TabIndex = 0;
            this.songListBox.SelectedIndexChanged += new System.EventHandler(this.songListBox_SelectedIndexChanged);
            // 
            // tabBanks
            // 
            this.tabBanks.Controls.Add(this.banksTreeView);
            this.tabBanks.Location = new System.Drawing.Point(4, 22);
            this.tabBanks.Name = "tabBanks";
            this.tabBanks.Padding = new System.Windows.Forms.Padding(3);
            this.tabBanks.Size = new System.Drawing.Size(620, 342);
            this.tabBanks.TabIndex = 1;
            this.tabBanks.Text = "tabBanks";
            this.tabBanks.UseVisualStyleBackColor = true;
            // 
            // banksTreeView
            // 
            this.banksTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.banksTreeView.Location = new System.Drawing.Point(6, 6);
            this.banksTreeView.Name = "banksTreeView";
            this.banksTreeView.Size = new System.Drawing.Size(608, 330);
            this.banksTreeView.TabIndex = 16;
            // 
            // tabSampleTable
            // 
            this.tabSampleTable.Controls.Add(this.propertyGrid1);
            this.tabSampleTable.Controls.Add(this.sampleTableListBox);
            this.tabSampleTable.Location = new System.Drawing.Point(4, 22);
            this.tabSampleTable.Name = "tabSampleTable";
            this.tabSampleTable.Size = new System.Drawing.Size(620, 342);
            this.tabSampleTable.TabIndex = 2;
            this.tabSampleTable.Text = "tabSampleTable";
            this.tabSampleTable.UseVisualStyleBackColor = true;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid1.Location = new System.Drawing.Point(195, 7);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(422, 320);
            this.propertyGrid1.TabIndex = 15;
            // 
            // sampleTableListBox
            // 
            this.sampleTableListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.sampleTableListBox.FormattingEnabled = true;
            this.sampleTableListBox.Location = new System.Drawing.Point(8, 7);
            this.sampleTableListBox.Name = "sampleTableListBox";
            this.sampleTableListBox.Size = new System.Drawing.Size(181, 329);
            this.sampleTableListBox.TabIndex = 14;
            this.sampleTableListBox.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // ofd
            // 
            this.ofd.Filter = "Crash Team Racing HWL file (*.hwl)|*.hwl";
            // 
            // sfd
            // 
            this.sfd.FileName = "sfd";
            this.sfd.Filter = "Crash Team Racing HWL file (*.hwl)|*.hwl";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(640, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.exportToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // HowlControl
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "HowlControl";
            this.Size = new System.Drawing.Size(640, 480);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.HowlControl_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.HowlControl_DragEnter);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabSongs.ResumeLayout(false);
            this.tabBanks.ResumeLayout(false);
            this.tabSampleTable.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GroupBox groupBox1;
        private Label label2;
        private Label label1;
        private FolderBrowserDialog fbd;
        private OpenFileDialog ofd;
        private SaveFileDialog sfd;
        private TabControl tabControl1;
        private TabPage tabBanks;
        private TabPage tabSongs;
        private TreeView banksTreeView;
        private TabPage tabSampleTable;
        private PropertyGrid propertyGrid1;
        private ListBox sampleTableListBox;
        private ListBox songListBox;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem exportToolStripMenuItem;
    }
}
