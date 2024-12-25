using System.Windows.Forms;

namespace CTRTools.Controls
{
    partial class XaControl
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
            this.xnfEntryEditor = new System.Windows.Forms.PropertyGrid();
            this.label1 = new System.Windows.Forms.Label();
            this.playNext = new System.Windows.Forms.CheckBox();
            this.xnfEntriesList = new System.Windows.Forms.ListBox();
            this.xnfFolderBox = new System.Windows.Forms.ComboBox();
            this.loadButton = new System.Windows.Forms.Button();
            this.ofdxnf = new System.Windows.Forms.OpenFileDialog();
            this.saveButton = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadXNFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveXNFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveXNFAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.xnfEntryEditor);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.playNext);
            this.groupBox1.Controls.Add(this.xnfEntriesList);
            this.groupBox1.Controls.Add(this.xnfFolderBox);
            this.groupBox1.Location = new System.Drawing.Point(3, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(714, 432);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "XNF XaInfo";
            // 
            // xnfEntryEditor
            // 
            this.xnfEntryEditor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.xnfEntryEditor.Location = new System.Drawing.Point(472, 46);
            this.xnfEntryEditor.Name = "xnfEntryEditor";
            this.xnfEntryEditor.Size = new System.Drawing.Size(236, 369);
            this.xnfEntryEditor.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(484, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 3;
            // 
            // playNext
            // 
            this.playNext.AutoSize = true;
            this.playNext.Location = new System.Drawing.Point(256, 23);
            this.playNext.Name = "playNext";
            this.playNext.Size = new System.Drawing.Size(80, 17);
            this.playNext.TabIndex = 2;
            this.playNext.Text = "Play all files";
            this.playNext.UseVisualStyleBackColor = true;
            // 
            // xnfEntriesList
            // 
            this.xnfEntriesList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.xnfEntriesList.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.xnfEntriesList.FormattingEnabled = true;
            this.xnfEntriesList.Location = new System.Drawing.Point(6, 46);
            this.xnfEntriesList.Name = "xnfEntriesList";
            this.xnfEntriesList.Size = new System.Drawing.Size(460, 355);
            this.xnfEntriesList.TabIndex = 1;
            this.xnfEntriesList.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // xnfFolderBox
            // 
            this.xnfFolderBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.xnfFolderBox.FormattingEnabled = true;
            this.xnfFolderBox.Location = new System.Drawing.Point(6, 19);
            this.xnfFolderBox.Name = "xnfFolderBox";
            this.xnfFolderBox.Size = new System.Drawing.Size(244, 21);
            this.xnfFolderBox.TabIndex = 0;
            this.xnfFolderBox.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // loadButton
            // 
            this.loadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.loadButton.Location = new System.Drawing.Point(3, 465);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(96, 24);
            this.loadButton.TabIndex = 10;
            this.loadButton.Text = "Load XNF";
            this.loadButton.UseVisualStyleBackColor = true;
            this.loadButton.Click += new System.EventHandler(this.actionLoadXnf_Click);
            // 
            // ofdxnf
            // 
            this.ofdxnf.Filter = "Crash Team Racing XNF file (*.xnf)|*.xnf";
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.saveButton.Location = new System.Drawing.Point(105, 465);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(96, 24);
            this.saveButton.TabIndex = 11;
            this.saveButton.Text = "Save XNF";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.Transparent;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.menuStrip1.Size = new System.Drawing.Size(720, 24);
            this.menuStrip1.TabIndex = 12;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadXNFToolStripMenuItem,
            this.saveXNFToolStripMenuItem,
            this.saveXNFAsToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadXNFToolStripMenuItem
            // 
            this.loadXNFToolStripMenuItem.Name = "loadXNFToolStripMenuItem";
            this.loadXNFToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.loadXNFToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.loadXNFToolStripMenuItem.Text = "Open XNF";
            this.loadXNFToolStripMenuItem.Click += new System.EventHandler(this.loadXNFToolStripMenuItem_Click);
            // 
            // saveXNFToolStripMenuItem
            // 
            this.saveXNFToolStripMenuItem.Name = "saveXNFToolStripMenuItem";
            this.saveXNFToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveXNFToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.saveXNFToolStripMenuItem.Text = "Save XNF";
            this.saveXNFToolStripMenuItem.Click += new System.EventHandler(this.saveXNFToolStripMenuItem_Click);
            // 
            // saveXNFAsToolStripMenuItem
            // 
            this.saveXNFAsToolStripMenuItem.Name = "saveXNFAsToolStripMenuItem";
            this.saveXNFAsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.saveXNFAsToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.saveXNFAsToolStripMenuItem.Text = "Save XNF as...";
            this.saveXNFAsToolStripMenuItem.Click += new System.EventHandler(this.saveXNFAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(215, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // XaControl
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.loadButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.Name = "XaControl";
            this.Size = new System.Drawing.Size(720, 492);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.XaControl_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.XaControl_DragEnter);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GroupBox groupBox1;
        private Button loadButton;
        private OpenFileDialog ofdxnf;
        private ComboBox xnfFolderBox;
        private ListBox xnfEntriesList;
        private CheckBox playNext;
        private Label label1;
        private PropertyGrid xnfEntryEditor;
        private Button saveButton;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem loadXNFToolStripMenuItem;
        private ToolStripMenuItem saveXNFToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem saveXNFAsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
    }
}
