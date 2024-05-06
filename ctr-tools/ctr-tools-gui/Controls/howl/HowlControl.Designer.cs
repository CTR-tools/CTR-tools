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
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.playButton = new System.Windows.Forms.Button();
            this.editSongButton = new System.Windows.Forms.Button();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.cseqImport = new System.Windows.Forms.Button();
            this.cseqSave = new System.Windows.Forms.Button();
            this.songListBox = new System.Windows.Forms.ListBox();
            this.tabSong = new System.Windows.Forms.TabPage();
            this.cseqControl1 = new CTRTools.Controls.CseqControl();
            this.tabBanks = new System.Windows.Forms.TabPage();
            this.banksTreeView = new System.Windows.Forms.TreeView();
            this.tabSampleTable = new System.Windows.Forms.TabPage();
            this.instrumentControl1 = new CTRTools.Controls.InstrumentControl();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.sampleTableListBox = new System.Windows.Forms.ListBox();
            this.fbd = new System.Windows.Forms.FolderBrowserDialog();
            this.ofd = new System.Windows.Forms.OpenFileDialog();
            this.sfd = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabSongs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.tabSong.SuspendLayout();
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
            this.tabControl1.Controls.Add(this.tabSong);
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
            this.tabSongs.Controls.Add(this.propertyGrid1);
            this.tabSongs.Controls.Add(this.label3);
            this.tabSongs.Controls.Add(this.button1);
            this.tabSongs.Controls.Add(this.playButton);
            this.tabSongs.Controls.Add(this.editSongButton);
            this.tabSongs.Controls.Add(this.numericUpDown1);
            this.tabSongs.Controls.Add(this.cseqImport);
            this.tabSongs.Controls.Add(this.cseqSave);
            this.tabSongs.Controls.Add(this.songListBox);
            this.tabSongs.Location = new System.Drawing.Point(4, 22);
            this.tabSongs.Name = "tabSongs";
            this.tabSongs.Padding = new System.Windows.Forms.Padding(3);
            this.tabSongs.Size = new System.Drawing.Size(614, 373);
            this.tabSongs.TabIndex = 0;
            this.tabSongs.Text = "tabSongs";
            this.tabSongs.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(227, 119);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(253, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "WARNING! Playback is CPU intensive and it sucks!";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(348, 93);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(115, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Stop";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // playButton
            // 
            this.playButton.Location = new System.Drawing.Point(227, 93);
            this.playButton.Name = "playButton";
            this.playButton.Size = new System.Drawing.Size(115, 23);
            this.playButton.TabIndex = 5;
            this.playButton.Text = "Play";
            this.playButton.UseVisualStyleBackColor = true;
            this.playButton.Click += new System.EventHandler(this.playButton_Click);
            // 
            // editSongButton
            // 
            this.editSongButton.Location = new System.Drawing.Point(227, 64);
            this.editSongButton.Name = "editSongButton";
            this.editSongButton.Size = new System.Drawing.Size(242, 23);
            this.editSongButton.TabIndex = 4;
            this.editSongButton.Text = "Edit selected song (or double click)";
            this.editSongButton.UseVisualStyleBackColor = true;
            this.editSongButton.Click += new System.EventHandler(this.editSongButton_Click);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(349, 37);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(120, 20);
            this.numericUpDown1.TabIndex = 3;
            this.numericUpDown1.Value = new decimal(new int[] {
            120,
            0,
            0,
            0});
            // 
            // cseqImport
            // 
            this.cseqImport.Location = new System.Drawing.Point(227, 35);
            this.cseqImport.Name = "cseqImport";
            this.cseqImport.Size = new System.Drawing.Size(115, 23);
            this.cseqImport.TabIndex = 2;
            this.cseqImport.Text = "Import MIDI";
            this.cseqImport.UseVisualStyleBackColor = true;
            this.cseqImport.Click += new System.EventHandler(this.cseqImport_Click);
            // 
            // cseqSave
            // 
            this.cseqSave.Location = new System.Drawing.Point(227, 6);
            this.cseqSave.Name = "cseqSave";
            this.cseqSave.Size = new System.Drawing.Size(115, 23);
            this.cseqSave.TabIndex = 1;
            this.cseqSave.Text = "Export CSEQ";
            this.cseqSave.UseVisualStyleBackColor = true;
            this.cseqSave.Click += new System.EventHandler(this.cseqSave_Click);
            // 
            // songListBox
            // 
            this.songListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.songListBox.FormattingEnabled = true;
            this.songListBox.Location = new System.Drawing.Point(3, 6);
            this.songListBox.Name = "songListBox";
            this.songListBox.Size = new System.Drawing.Size(218, 355);
            this.songListBox.TabIndex = 0;
            this.songListBox.SelectedIndexChanged += new System.EventHandler(this.songListBox_SelectedIndexChanged);
            this.songListBox.DoubleClick += new System.EventHandler(this.songListBox_DoubleClick);
            // 
            // tabSong
            // 
            this.tabSong.Controls.Add(this.cseqControl1);
            this.tabSong.Location = new System.Drawing.Point(4, 22);
            this.tabSong.Name = "tabSong";
            this.tabSong.Size = new System.Drawing.Size(614, 373);
            this.tabSong.TabIndex = 3;
            this.tabSong.Text = "tabSong";
            this.tabSong.UseVisualStyleBackColor = true;
            // 
            // cseqControl1
            // 
            this.cseqControl1.AllowDrop = true;
            this.cseqControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cseqControl1.Location = new System.Drawing.Point(3, 3);
            this.cseqControl1.Name = "cseqControl1";
            this.cseqControl1.Size = new System.Drawing.Size(608, 367);
            this.cseqControl1.TabIndex = 0;
            // 
            // tabBanks
            // 
            this.tabBanks.Controls.Add(this.banksTreeView);
            this.tabBanks.Location = new System.Drawing.Point(4, 22);
            this.tabBanks.Name = "tabBanks";
            this.tabBanks.Padding = new System.Windows.Forms.Padding(3);
            this.tabBanks.Size = new System.Drawing.Size(614, 373);
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
            this.tabSampleTable.Controls.Add(this.instrumentControl1);
            this.tabSampleTable.Controls.Add(this.textBox1);
            this.tabSampleTable.Controls.Add(this.sampleTableListBox);
            this.tabSampleTable.Location = new System.Drawing.Point(4, 22);
            this.tabSampleTable.Name = "tabSampleTable";
            this.tabSampleTable.Size = new System.Drawing.Size(614, 373);
            this.tabSampleTable.TabIndex = 2;
            this.tabSampleTable.Text = "tabSampleTable";
            this.tabSampleTable.UseVisualStyleBackColor = true;
            // 
            // instrumentControl1
            // 
            this.instrumentControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.instrumentControl1.Instrument = null;
            this.instrumentControl1.Location = new System.Drawing.Point(195, 7);
            this.instrumentControl1.Name = "instrumentControl1";
            this.instrumentControl1.Size = new System.Drawing.Size(416, 363);
            this.instrumentControl1.TabIndex = 21;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(8, 7);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(181, 20);
            this.textBox1.TabIndex = 20;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // sampleTableListBox
            // 
            this.sampleTableListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.sampleTableListBox.FormattingEnabled = true;
            this.sampleTableListBox.Location = new System.Drawing.Point(8, 33);
            this.sampleTableListBox.Name = "sampleTableListBox";
            this.sampleTableListBox.Size = new System.Drawing.Size(181, 329);
            this.sampleTableListBox.TabIndex = 14;
            this.sampleTableListBox.SelectedIndexChanged += new System.EventHandler(this.sampleTableListBox_SelectedIndexChanged);
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
            this.saveAsToolStripMenuItem,
            this.exportToolStripMenuItem,
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
            this.openToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.saveAsToolStripMenuItem.Text = "Save as";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(181, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Location = new System.Drawing.Point(227, 135);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(242, 232);
            this.propertyGrid1.TabIndex = 8;
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
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.HowlControl_KeyPress);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabSongs.ResumeLayout(false);
            this.tabSongs.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.tabSong.ResumeLayout(false);
            this.tabBanks.ResumeLayout(false);
            this.tabSampleTable.ResumeLayout(false);
            this.tabSampleTable.PerformLayout();
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
        private ListBox sampleTableListBox;
        private ListBox songListBox;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem exportToolStripMenuItem;
        private Button cseqImport;
        private Button cseqSave;
        private NumericUpDown numericUpDown1;
        private ToolStripMenuItem saveAsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem exitToolStripMenuItem;
        private TabPage tabSong;
        private CseqControl cseqControl1;
        private Button editSongButton;
        private TextBox textBox1;
        private InstrumentControl instrumentControl1;
        private Button playButton;
        private Button button1;
        private Label label3;
        private PropertyGrid propertyGrid1;
    }
}
