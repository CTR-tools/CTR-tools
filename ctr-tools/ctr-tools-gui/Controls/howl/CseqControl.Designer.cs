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
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importMIDIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.patchMIDIInstrumentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ignoreOriginalVolumeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyInstrumentVolumeToTracksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.loadBankToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.defaultSampleRateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.alistAlBankSamplesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadMIDIMappingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tutorialToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.patchBox = new System.Windows.Forms.ComboBox();
            this.trackBox = new System.Windows.Forms.ListBox();
            this.sequenceBox = new System.Windows.Forms.ListBox();
            this.seqInfoBox = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabTrackInfo = new System.Windows.Forms.TabPage();
            this.trackInfoBox = new System.Windows.Forms.TextBox();
            this.tabInstruments = new System.Windows.Forms.TabPage();
            this.changeInstrumentButton = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.duplicateInstrumentButton = new System.Windows.Forms.Button();
            this.exportAllButton = new System.Windows.Forms.Button();
            this.instrumentList = new System.Windows.Forms.TreeView();
            this.instrumentControl1 = new CTRTools.Controls.InstrumentControl();
            this.tabMeta = new System.Windows.Forms.TabPage();
            this.metaInstInfo = new System.Windows.Forms.TextBox();
            this.metaInstList = new System.Windows.Forms.ListBox();
            this.ofd = new System.Windows.Forms.OpenFileDialog();
            this.sfd = new System.Windows.Forms.SaveFileDialog();
            this.findBank = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabTrackInfo.SuspendLayout();
            this.tabInstruments.SuspendLayout();
            this.tabMeta.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.Transparent;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.menuStrip1.Size = new System.Drawing.Size(835, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.exportSEQToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.importMIDIToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(294, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // exportSEQToolStripMenuItem
            // 
            this.exportSEQToolStripMenuItem.Name = "exportSEQToolStripMenuItem";
            this.exportSEQToolStripMenuItem.Size = new System.Drawing.Size(294, 22);
            this.exportSEQToolStripMenuItem.Text = "Save";
            this.exportSEQToolStripMenuItem.Click += new System.EventHandler(this.exportSEQToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(294, 22);
            this.saveAsToolStripMenuItem.Text = "Save as...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // importMIDIToolStripMenuItem
            // 
            this.importMIDIToolStripMenuItem.Name = "importMIDIToolStripMenuItem";
            this.importMIDIToolStripMenuItem.Size = new System.Drawing.Size(294, 22);
            this.importMIDIToolStripMenuItem.Text = "Import MIDI (not from here! remove later)";
            this.importMIDIToolStripMenuItem.Click += new System.EventHandler(this.importMIDIToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.patchMIDIInstrumentsToolStripMenuItem,
            this.ignoreOriginalVolumeToolStripMenuItem,
            this.copyInstrumentVolumeToTracksToolStripMenuItem,
            this.toolStripMenuItem6,
            this.loadBankToolStripMenuItem,
            this.defaultSampleRateToolStripMenuItem,
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
            this.toolStripMenuItem2.Size = new System.Drawing.Size(104, 22);
            this.toolStripMenuItem2.Text = "11025";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(104, 22);
            this.toolStripMenuItem3.Text = "22050";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(104, 22);
            this.toolStripMenuItem4.Text = "33075";
            this.toolStripMenuItem4.Click += new System.EventHandler(this.toolStripMenuItem4_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(104, 22);
            this.toolStripMenuItem5.Text = "44100";
            this.toolStripMenuItem5.Click += new System.EventHandler(this.toolStripMenuItem5_Click);
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
            this.tutorialToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.tutorialToolStripMenuItem.Text = "Tutorial";
            this.tutorialToolStripMenuItem.Click += new System.EventHandler(this.tutorialToolStripMenuItem_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.button5);
            this.groupBox1.Controls.Add(this.patchBox);
            this.groupBox1.Controls.Add(this.trackBox);
            this.groupBox1.Controls.Add(this.sequenceBox);
            this.groupBox1.Controls.Add(this.seqInfoBox);
            this.groupBox1.Location = new System.Drawing.Point(3, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(142, 450);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "SEQ Info";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(71, 199);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "Check size";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(71, 170);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 6;
            this.button5.Text = "Remove all";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // patchBox
            // 
            this.patchBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.patchBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.patchBox.FormattingEnabled = true;
            this.patchBox.Location = new System.Drawing.Point(5, 422);
            this.patchBox.Name = "patchBox";
            this.patchBox.Size = new System.Drawing.Size(131, 21);
            this.patchBox.TabIndex = 5;
            this.patchBox.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
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
            // seqInfoBox
            // 
            this.seqInfoBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.seqInfoBox.Location = new System.Drawing.Point(6, 19);
            this.seqInfoBox.Multiline = true;
            this.seqInfoBox.Name = "seqInfoBox";
            this.seqInfoBox.Size = new System.Drawing.Size(130, 112);
            this.seqInfoBox.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabTrackInfo);
            this.tabControl1.Controls.Add(this.tabInstruments);
            this.tabControl1.Controls.Add(this.tabMeta);
            this.tabControl1.Location = new System.Drawing.Point(151, 27);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(681, 450);
            this.tabControl1.TabIndex = 5;
            // 
            // tabTrackInfo
            // 
            this.tabTrackInfo.Controls.Add(this.trackInfoBox);
            this.tabTrackInfo.Location = new System.Drawing.Point(4, 22);
            this.tabTrackInfo.Name = "tabTrackInfo";
            this.tabTrackInfo.Padding = new System.Windows.Forms.Padding(3);
            this.tabTrackInfo.Size = new System.Drawing.Size(673, 424);
            this.tabTrackInfo.TabIndex = 0;
            this.tabTrackInfo.Text = "Track Info";
            this.tabTrackInfo.UseVisualStyleBackColor = true;
            // 
            // trackInfoBox
            // 
            this.trackInfoBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackInfoBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.trackInfoBox.Location = new System.Drawing.Point(3, 3);
            this.trackInfoBox.Multiline = true;
            this.trackInfoBox.Name = "trackInfoBox";
            this.trackInfoBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.trackInfoBox.Size = new System.Drawing.Size(667, 418);
            this.trackInfoBox.TabIndex = 5;
            this.trackInfoBox.Text = "  ";
            this.trackInfoBox.TextChanged += new System.EventHandler(this.trackInfoBox_TextChanged);
            // 
            // tabInstruments
            // 
            this.tabInstruments.Controls.Add(this.findBank);
            this.tabInstruments.Controls.Add(this.changeInstrumentButton);
            this.tabInstruments.Controls.Add(this.button2);
            this.tabInstruments.Controls.Add(this.duplicateInstrumentButton);
            this.tabInstruments.Controls.Add(this.exportAllButton);
            this.tabInstruments.Controls.Add(this.instrumentList);
            this.tabInstruments.Controls.Add(this.instrumentControl1);
            this.tabInstruments.Location = new System.Drawing.Point(4, 22);
            this.tabInstruments.Name = "tabInstruments";
            this.tabInstruments.Padding = new System.Windows.Forms.Padding(3);
            this.tabInstruments.Size = new System.Drawing.Size(673, 424);
            this.tabInstruments.TabIndex = 1;
            this.tabInstruments.Text = "Instruments / Samples";
            this.tabInstruments.UseVisualStyleBackColor = true;
            // 
            // changeInstrumentButton
            // 
            this.changeInstrumentButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.changeInstrumentButton.Location = new System.Drawing.Point(6, 369);
            this.changeInstrumentButton.Name = "changeInstrumentButton";
            this.changeInstrumentButton.Size = new System.Drawing.Size(107, 23);
            this.changeInstrumentButton.TabIndex = 8;
            this.changeInstrumentButton.Text = "Change instrument";
            this.changeInstrumentButton.UseVisualStyleBackColor = true;
            this.changeInstrumentButton.Click += new System.EventHandler(this.changeInstrumentButton_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button2.Location = new System.Drawing.Point(6, 340);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(220, 23);
            this.button2.TabIndex = 7;
            this.button2.Text = "Add percussion";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // duplicateInstrumentButton
            // 
            this.duplicateInstrumentButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.duplicateInstrumentButton.Location = new System.Drawing.Point(6, 311);
            this.duplicateInstrumentButton.Name = "duplicateInstrumentButton";
            this.duplicateInstrumentButton.Size = new System.Drawing.Size(220, 23);
            this.duplicateInstrumentButton.TabIndex = 6;
            this.duplicateInstrumentButton.Text = "Add instrument";
            this.duplicateInstrumentButton.UseVisualStyleBackColor = true;
            this.duplicateInstrumentButton.Click += new System.EventHandler(this.duplicateInstrumentButton_Click);
            // 
            // exportAllButton
            // 
            this.exportAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.exportAllButton.Location = new System.Drawing.Point(6, 395);
            this.exportAllButton.Name = "exportAllButton";
            this.exportAllButton.Size = new System.Drawing.Size(220, 23);
            this.exportAllButton.TabIndex = 5;
            this.exportAllButton.Text = "Export all VAGs";
            this.exportAllButton.UseVisualStyleBackColor = true;
            this.exportAllButton.Click += new System.EventHandler(this.exportAllButton_Click);
            // 
            // instrumentList
            // 
            this.instrumentList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.instrumentList.HideSelection = false;
            this.instrumentList.Location = new System.Drawing.Point(6, 6);
            this.instrumentList.Name = "instrumentList";
            this.instrumentList.Size = new System.Drawing.Size(220, 299);
            this.instrumentList.TabIndex = 3;
            this.instrumentList.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // instrumentControl1
            // 
            this.instrumentControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.instrumentControl1.Instrument = null;
            this.instrumentControl1.Location = new System.Drawing.Point(232, 6);
            this.instrumentControl1.Name = "instrumentControl1";
            this.instrumentControl1.Size = new System.Drawing.Size(438, 415);
            this.instrumentControl1.TabIndex = 4;
            // 
            // tabMeta
            // 
            this.tabMeta.Controls.Add(this.metaInstInfo);
            this.tabMeta.Controls.Add(this.metaInstList);
            this.tabMeta.Location = new System.Drawing.Point(4, 22);
            this.tabMeta.Name = "tabMeta";
            this.tabMeta.Size = new System.Drawing.Size(673, 424);
            this.tabMeta.TabIndex = 2;
            this.tabMeta.Text = "Meta";
            this.tabMeta.UseVisualStyleBackColor = true;
            // 
            // metaInstInfo
            // 
            this.metaInstInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.metaInstInfo.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.metaInstInfo.Location = new System.Drawing.Point(129, 3);
            this.metaInstInfo.Multiline = true;
            this.metaInstInfo.Name = "metaInstInfo";
            this.metaInstInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.metaInstInfo.Size = new System.Drawing.Size(541, 409);
            this.metaInstInfo.TabIndex = 1;
            // 
            // metaInstList
            // 
            this.metaInstList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.metaInstList.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.metaInstList.FormattingEnabled = true;
            this.metaInstList.ItemHeight = 15;
            this.metaInstList.Location = new System.Drawing.Point(3, 3);
            this.metaInstList.Name = "metaInstList";
            this.metaInstList.Size = new System.Drawing.Size(120, 409);
            this.metaInstList.TabIndex = 0;
            this.metaInstList.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // ofd
            // 
            this.ofd.Filter = "Crash Team Racing CSEQ (*.cseq)|*.cseq";
            // 
            // sfd
            // 
            this.sfd.Filter = "MIDI File (*.mid)|*.mid";
            // 
            // findBank
            // 
            this.findBank.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.findBank.Location = new System.Drawing.Point(119, 369);
            this.findBank.Name = "findBank";
            this.findBank.Size = new System.Drawing.Size(107, 23);
            this.findBank.TabIndex = 9;
            this.findBank.Text = "Find bank";
            this.findBank.UseVisualStyleBackColor = true;
            this.findBank.Click += new System.EventHandler(this.findBank_Click);
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
            this.Size = new System.Drawing.Size(835, 480);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabTrackInfo.ResumeLayout(false);
            this.tabTrackInfo.PerformLayout();
            this.tabInstruments.ResumeLayout(false);
            this.tabMeta.ResumeLayout(false);
            this.tabMeta.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private GroupBox groupBox1;
        private TextBox seqInfoBox;
        private ListBox trackBox;
        private ListBox sequenceBox;
        private TabControl tabControl1;
        private TabPage tabTrackInfo;
        private TextBox trackInfoBox;
        private TabPage tabInstruments;
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
        private ComboBox patchBox;
        private ToolStripMenuItem ignoreOriginalVolumeToolStripMenuItem;
        private TreeView instrumentList;
        private ToolStripMenuItem copyInstrumentVolumeToTracksToolStripMenuItem;
        private ToolStripMenuItem alistAlBankSamplesToolStripMenuItem;
        private TabPage tabMeta;
        private TextBox metaInstInfo;
        private ListBox metaInstList;
        private ToolStripMenuItem reloadMIDIMappingsToolStripMenuItem;
        private OpenFileDialog ofd;
        private SaveFileDialog sfd;
        private ToolStripMenuItem importMIDIToolStripMenuItem;
        private ToolStripMenuItem saveAsToolStripMenuItem;
        private Button button5;
        private InstrumentControl instrumentControl1;
        private Button exportAllButton;
        private Button button1;
        private Button duplicateInstrumentButton;
        private Button button2;
        private Button changeInstrumentButton;
        private Button findBank;
    }
}

