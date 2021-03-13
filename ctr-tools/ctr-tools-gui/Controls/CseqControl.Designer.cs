using System.Windows.Forms;

namespace CTRTools.Controls
{
    partial class CseqControl
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportSEQToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.patchMIDIInstrumentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ignoreOriginalVolumeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyInstrumentVolumeToTracksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.loadBankToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportSamplesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.defaultSampleRateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.testJsonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alistAlBankSamplesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadMIDIMappingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tutorialToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.trackBox = new System.Windows.Forms.ListBox();
            this.sequenceBox = new System.Windows.Forms.ListBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.ofd = new System.Windows.Forms.OpenFileDialog();
            this.sfd = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(640, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.exportSEQToolStripMenuItem,
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
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // exportSEQToolStripMenuItem
            // 
            this.exportSEQToolStripMenuItem.Enabled = false;
            this.exportSEQToolStripMenuItem.Name = "exportSEQToolStripMenuItem";
            this.exportSEQToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exportSEQToolStripMenuItem.Text = "Export CSEQ";
            this.exportSEQToolStripMenuItem.Click += new System.EventHandler(this.exportSEQToolStripMenuItem_Click);
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
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.patchMIDIInstrumentsToolStripMenuItem,
            this.ignoreOriginalVolumeToolStripMenuItem,
            this.copyInstrumentVolumeToTracksToolStripMenuItem,
            this.toolStripMenuItem6,
            this.loadBankToolStripMenuItem,
            this.exportSamplesToolStripMenuItem,
            this.defaultSampleRateToolStripMenuItem,
            this.testJsonToolStripMenuItem,
            this.alistAlBankSamplesToolStripMenuItem,
            this.reloadMIDIMappingsToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // patchMIDIInstrumentsToolStripMenuItem
            // 
            this.patchMIDIInstrumentsToolStripMenuItem.CheckOnClick = true;
            this.patchMIDIInstrumentsToolStripMenuItem.Name = "patchMIDIInstrumentsToolStripMenuItem";
            this.patchMIDIInstrumentsToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.patchMIDIInstrumentsToolStripMenuItem.Text = "Patch MIDI Instruments";
            this.patchMIDIInstrumentsToolStripMenuItem.ToolTipText = "Only use this option if you\'re not planning to create SF2 out of original samples" +
    ".";
            this.patchMIDIInstrumentsToolStripMenuItem.Click += new System.EventHandler(this.patchMIDIInstrumentsToolStripMenuItem_Click);
            // 
            // ignoreOriginalVolumeToolStripMenuItem
            // 
            this.ignoreOriginalVolumeToolStripMenuItem.CheckOnClick = true;
            this.ignoreOriginalVolumeToolStripMenuItem.Name = "ignoreOriginalVolumeToolStripMenuItem";
            this.ignoreOriginalVolumeToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.ignoreOriginalVolumeToolStripMenuItem.Text = "Force max velocity";
            this.ignoreOriginalVolumeToolStripMenuItem.ToolTipText = "If this option is set, MIDI notes ignore original velocity value and use max velo" +
    "city.";
            this.ignoreOriginalVolumeToolStripMenuItem.Click += new System.EventHandler(this.ignoreOriginalVolumeToolStripMenuItem_Click);
            // 
            // copyInstrumentVolumeToTracksToolStripMenuItem
            // 
            this.copyInstrumentVolumeToTracksToolStripMenuItem.Checked = true;
            this.copyInstrumentVolumeToTracksToolStripMenuItem.CheckOnClick = true;
            this.copyInstrumentVolumeToTracksToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.copyInstrumentVolumeToTracksToolStripMenuItem.Name = "copyInstrumentVolumeToTracksToolStripMenuItem";
            this.copyInstrumentVolumeToTracksToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.copyInstrumentVolumeToTracksToolStripMenuItem.Text = "Copy instrument volume to tracks";
            this.copyInstrumentVolumeToTracksToolStripMenuItem.ToolTipText = "This option will ensure that MIDI track volume setting is filled with sample volu" +
    "me value.";
            this.copyInstrumentVolumeToTracksToolStripMenuItem.Click += new System.EventHandler(this.copyInstrumentVolumeToTracksToolStripMenuItem_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(251, 6);
            // 
            // loadBankToolStripMenuItem
            // 
            this.loadBankToolStripMenuItem.Name = "loadBankToolStripMenuItem";
            this.loadBankToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.loadBankToolStripMenuItem.Text = "Load bank";
            this.loadBankToolStripMenuItem.ToolTipText = "Loads BNK file and checks whether it contains the samples used by CSEQ.";
            this.loadBankToolStripMenuItem.Click += new System.EventHandler(this.loadBankToolStripMenuItem_Click);
            // 
            // exportSamplesToolStripMenuItem
            // 
            this.exportSamplesToolStripMenuItem.Name = "exportSamplesToolStripMenuItem";
            this.exportSamplesToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.exportSamplesToolStripMenuItem.Text = "Export samples";
            this.exportSamplesToolStripMenuItem.ToolTipText = "Exports all the samples used by currently loaded CSEQ";
            this.exportSamplesToolStripMenuItem.Click += new System.EventHandler(this.exportSamplesToolStripMenuItem_Click);
            // 
            // defaultSampleRateToolStripMenuItem
            // 
            this.defaultSampleRateToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripMenuItem4,
            this.toolStripMenuItem5});
            this.defaultSampleRateToolStripMenuItem.Name = "defaultSampleRateToolStripMenuItem";
            this.defaultSampleRateToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.defaultSampleRateToolStripMenuItem.Text = "Default sample rate";
            this.defaultSampleRateToolStripMenuItem.ToolTipText = "Sets sample rate to use for samples not used by any CSEQ instrument.";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(180, 22);
            this.toolStripMenuItem2.Text = "11025";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(180, 22);
            this.toolStripMenuItem3.Text = "22050";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(180, 22);
            this.toolStripMenuItem4.Text = "33075";
            this.toolStripMenuItem4.Click += new System.EventHandler(this.toolStripMenuItem4_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(180, 22);
            this.toolStripMenuItem5.Text = "44100";
            this.toolStripMenuItem5.Click += new System.EventHandler(this.toolStripMenuItem5_Click);
            // 
            // testJsonToolStripMenuItem
            // 
            this.testJsonToolStripMenuItem.Name = "testJsonToolStripMenuItem";
            this.testJsonToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.testJsonToolStripMenuItem.Text = "List all samples";
            this.testJsonToolStripMenuItem.Click += new System.EventHandler(this.testJsonToolStripMenuItem_Click);
            // 
            // alistAlBankSamplesToolStripMenuItem
            // 
            this.alistAlBankSamplesToolStripMenuItem.Name = "alistAlBankSamplesToolStripMenuItem";
            this.alistAlBankSamplesToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.alistAlBankSamplesToolStripMenuItem.Text = "Sample to bank relation list";
            this.alistAlBankSamplesToolStripMenuItem.Click += new System.EventHandler(this.alistAlBankSamplesToolStripMenuItem_Click);
            // 
            // reloadMIDIMappingsToolStripMenuItem
            // 
            this.reloadMIDIMappingsToolStripMenuItem.Name = "reloadMIDIMappingsToolStripMenuItem";
            this.reloadMIDIMappingsToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.reloadMIDIMappingsToolStripMenuItem.Text = "Reload MIDI mappings";
            this.reloadMIDIMappingsToolStripMenuItem.ToolTipText = "Might be useful if you\'re editing JSON file.";
            this.reloadMIDIMappingsToolStripMenuItem.Click += new System.EventHandler(this.reloadMIDIMappingsToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tutorialToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            this.helpToolStripMenuItem.Visible = false;
            // 
            // tutorialToolStripMenuItem
            // 
            this.tutorialToolStripMenuItem.Name = "tutorialToolStripMenuItem";
            this.tutorialToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.tutorialToolStripMenuItem.Text = "Tutorial";
            this.tutorialToolStripMenuItem.Click += new System.EventHandler(this.tutorialToolStripMenuItem_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.comboBox1);
            this.groupBox1.Controls.Add(this.trackBox);
            this.groupBox1.Controls.Add(this.sequenceBox);
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Location = new System.Drawing.Point(3, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(142, 450);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "SEQ Info";
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(5, 422);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(131, 21);
            this.comboBox1.TabIndex = 5;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // trackBox
            // 
            this.trackBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.trackBox.FormattingEnabled = true;
            this.trackBox.Location = new System.Drawing.Point(6, 199);
            this.trackBox.Name = "trackBox";
            this.trackBox.Size = new System.Drawing.Size(130, 212);
            this.trackBox.TabIndex = 4;
            this.trackBox.SelectedIndexChanged += new System.EventHandler(this.trackBox_SelectedIndexChanged);
            this.trackBox.DoubleClick += new System.EventHandler(this.trackBox_DoubleClick);
            // 
            // sequenceBox
            // 
            this.sequenceBox.FormattingEnabled = true;
            this.sequenceBox.Location = new System.Drawing.Point(6, 137);
            this.sequenceBox.Name = "sequenceBox";
            this.sequenceBox.Size = new System.Drawing.Size(130, 56);
            this.sequenceBox.TabIndex = 3;
            this.sequenceBox.SelectedIndexChanged += new System.EventHandler(this.sequenceBox_SelectedIndexChanged);
            this.sequenceBox.DoubleClick += new System.EventHandler(this.sequenceBox_DoubleClick);
            // 
            // textBox2
            // 
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox2.Location = new System.Drawing.Point(6, 19);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(130, 112);
            this.textBox2.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(151, 27);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(486, 450);
            this.tabControl1.TabIndex = 5;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.textBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(478, 424);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Track Info";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox1.Location = new System.Drawing.Point(3, 3);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(472, 418);
            this.textBox1.TabIndex = 5;
            this.textBox1.Text = "  ";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.treeView1);
            this.tabPage2.Controls.Add(this.propertyGrid1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(478, 424);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Instruments / Samples";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.treeView1.Location = new System.Drawing.Point(6, 14);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(302, 395);
            this.treeView1.TabIndex = 3;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeView1.DoubleClick += new System.EventHandler(this.treeView1_DoubleClick);
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid1.Location = new System.Drawing.Point(314, 14);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(149, 395);
            this.propertyGrid1.TabIndex = 2;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.textBox3);
            this.tabPage3.Controls.Add(this.listBox1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(478, 424);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Meta";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // textBox3
            // 
            this.textBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox3.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox3.Location = new System.Drawing.Point(129, 3);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox3.Size = new System.Drawing.Size(156, 409);
            this.textBox3.TabIndex = 1;
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listBox1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 15;
            this.listBox1.Location = new System.Drawing.Point(3, 3);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(120, 394);
            this.listBox1.TabIndex = 0;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.button4);
            this.tabPage4.Controls.Add(this.button3);
            this.tabPage4.Controls.Add(this.button2);
            this.tabPage4.Controls.Add(this.button1);
            this.tabPage4.Controls.Add(this.listBox2);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(478, 424);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Bank";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Enabled = false;
            this.button4.Location = new System.Drawing.Point(132, 93);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 4;
            this.button4.Text = "Save";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Enabled = false;
            this.button3.Location = new System.Drawing.Point(132, 64);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 3;
            this.button3.Text = "Replace";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(132, 35);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Remove";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(132, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Add";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // listBox2
            // 
            this.listBox2.FormattingEnabled = true;
            this.listBox2.Location = new System.Drawing.Point(6, 6);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(120, 407);
            this.listBox2.TabIndex = 0;
            // 
            // ofd
            // 
            this.ofd.Filter = "Crash Team Racing CSEQ (*.cseq)|*.cseq";
            // 
            // sfd
            // 
            this.sfd.Filter = "MIDI File (*.mid)|*.mid";
            // 
            // CseqControl
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.Name = "CseqControl";
            this.Size = new System.Drawing.Size(640, 480);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private GroupBox groupBox1;
        private TextBox textBox2;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ListBox trackBox;
        private ListBox sequenceBox;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TextBox textBox1;
        private TabPage tabPage2;
        private ToolStripMenuItem optionsToolStripMenuItem;
        private ToolStripMenuItem exportSEQToolStripMenuItem;
        private ToolStripMenuItem loadBankToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem tutorialToolStripMenuItem;
        private ToolStripMenuItem defaultSampleRateToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripMenuItem toolStripMenuItem3;
        private ToolStripMenuItem toolStripMenuItem4;
        private ToolStripMenuItem toolStripMenuItem5;
        private ToolStripMenuItem patchMIDIInstrumentsToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem6;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem testJsonToolStripMenuItem;
        private ToolStripMenuItem exportSamplesToolStripMenuItem;
        private ComboBox comboBox1;
        private ToolStripMenuItem ignoreOriginalVolumeToolStripMenuItem;
        private PropertyGrid propertyGrid1;
        private TreeView treeView1;
        private ToolStripMenuItem copyInstrumentVolumeToTracksToolStripMenuItem;
        private ToolStripMenuItem alistAlBankSamplesToolStripMenuItem;
        private TabPage tabPage3;
        private TextBox textBox3;
        private ListBox listBox1;
        private ToolStripMenuItem reloadMIDIMappingsToolStripMenuItem;
        private TabPage tabPage4;
        private Button button4;
        private Button button3;
        private Button button2;
        private Button button1;
        private ListBox listBox2;
        private OpenFileDialog ofd;
        private SaveFileDialog sfd;
    }
}

