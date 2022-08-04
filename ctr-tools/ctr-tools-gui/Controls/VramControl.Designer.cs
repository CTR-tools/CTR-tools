using System.Windows.Forms;

namespace CTRTools.Controls
{
    partial class VramControl
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
            this.actionPack = new System.Windows.Forms.Button();
            this.actionExtract = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.optionTexModels = new System.Windows.Forms.CheckBox();
            this.optionTexHigh = new System.Windows.Forms.CheckBox();
            this.optionTexMed = new System.Windows.Forms.CheckBox();
            this.optionTexLow = new System.Windows.Forms.CheckBox();
            this.optionDebugVram = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pathFile = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.pathFolder = new System.Windows.Forms.TextBox();
            this.actionBrowseFile = new System.Windows.Forms.Button();
            this.actionBrowseFolder = new System.Windows.Forms.Button();
            this.actionOpenFolder = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.pathVram = new System.Windows.Forms.TextBox();
            this.actionBrowseVram = new System.Windows.Forms.Button();
            this.actionRestore = new System.Windows.Forms.Button();
            this.ofd = new System.Windows.Forms.OpenFileDialog();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // actionPack
            // 
            this.actionPack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.actionPack.Location = new System.Drawing.Point(540, 453);
            this.actionPack.Name = "actionPack";
            this.actionPack.Size = new System.Drawing.Size(96, 24);
            this.actionPack.TabIndex = 11;
            this.actionPack.Text = "Pack to VRAM";
            this.actionPack.UseVisualStyleBackColor = true;
            this.actionPack.Click += new System.EventHandler(this.actionPack_Click);
            // 
            // actionExtract
            // 
            this.actionExtract.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.actionExtract.Location = new System.Drawing.Point(438, 453);
            this.actionExtract.Name = "actionExtract";
            this.actionExtract.Size = new System.Drawing.Size(96, 24);
            this.actionExtract.TabIndex = 10;
            this.actionExtract.Text = "Extract textures";
            this.actionExtract.UseVisualStyleBackColor = true;
            this.actionExtract.Click += new System.EventHandler(this.actionExtract_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.pictureBox3);
            this.groupBox2.Controls.Add(this.pictureBox2);
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Controls.Add(this.tableLayoutPanel1);
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(633, 444);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Settings";
            // 
            // pictureBox3
            // 
            this.pictureBox3.Location = new System.Drawing.Point(323, 237);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(226, 201);
            this.pictureBox3.TabIndex = 9;
            this.pictureBox3.TabStop = false;
            this.pictureBox3.Visible = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(93, 237);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(224, 201);
            this.pictureBox2.TabIndex = 8;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Visible = false;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 237);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 7;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 208);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(145, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "try load texture layout";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(157, 211);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(193, 20);
            this.textBox1.TabIndex = 5;
            this.textBox1.Text = "<place 12 bytes here>";
            this.textBox1.Visible = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 67F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.pathFile, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.pathFolder, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.actionBrowseFile, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.actionBrowseFolder, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.actionOpenFolder, 4, 3);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.pathVram, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.actionBrowseVram, 3, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 19);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(621, 183);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.optionTexModels);
            this.panel1.Controls.Add(this.optionTexHigh);
            this.panel1.Controls.Add(this.optionTexMed);
            this.panel1.Controls.Add(this.optionTexLow);
            this.panel1.Controls.Add(this.optionDebugVram);
            this.panel1.Location = new System.Drawing.Point(124, 90);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(204, 90);
            this.panel1.TabIndex = 4;
            // 
            // optionTexModels
            // 
            this.optionTexModels.AutoSize = true;
            this.optionTexModels.Checked = true;
            this.optionTexModels.CheckState = System.Windows.Forms.CheckState.Checked;
            this.optionTexModels.Location = new System.Drawing.Point(3, 72);
            this.optionTexModels.Name = "optionTexModels";
            this.optionTexModels.Size = new System.Drawing.Size(127, 17);
            this.optionTexModels.TabIndex = 7;
            this.optionTexModels.Text = "Export model textures";
            this.optionTexModels.UseVisualStyleBackColor = true;
            // 
            // optionTexHigh
            // 
            this.optionTexHigh.AutoSize = true;
            this.optionTexHigh.Checked = true;
            this.optionTexHigh.CheckState = System.Windows.Forms.CheckState.Checked;
            this.optionTexHigh.Location = new System.Drawing.Point(3, 49);
            this.optionTexHigh.Name = "optionTexHigh";
            this.optionTexHigh.Size = new System.Drawing.Size(119, 17);
            this.optionTexHigh.TabIndex = 7;
            this.optionTexHigh.Text = "Export high textures";
            this.optionTexHigh.UseVisualStyleBackColor = true;
            // 
            // optionTexMed
            // 
            this.optionTexMed.AutoSize = true;
            this.optionTexMed.Checked = true;
            this.optionTexMed.CheckState = System.Windows.Forms.CheckState.Checked;
            this.optionTexMed.Location = new System.Drawing.Point(3, 26);
            this.optionTexMed.Name = "optionTexMed";
            this.optionTexMed.Size = new System.Drawing.Size(115, 17);
            this.optionTexMed.TabIndex = 7;
            this.optionTexMed.Text = "Export mid textures";
            this.optionTexMed.UseVisualStyleBackColor = true;
            // 
            // optionTexLow
            // 
            this.optionTexLow.AutoSize = true;
            this.optionTexLow.Checked = true;
            this.optionTexLow.CheckState = System.Windows.Forms.CheckState.Checked;
            this.optionTexLow.Location = new System.Drawing.Point(3, 3);
            this.optionTexLow.Name = "optionTexLow";
            this.optionTexLow.Size = new System.Drawing.Size(115, 17);
            this.optionTexLow.TabIndex = 8;
            this.optionTexLow.Text = "Export low textures";
            this.optionTexLow.UseVisualStyleBackColor = true;
            // 
            // optionDebugVram
            // 
            this.optionDebugVram.AutoSize = true;
            this.optionDebugVram.Location = new System.Drawing.Point(3, 95);
            this.optionDebugVram.Name = "optionDebugVram";
            this.optionDebugVram.Size = new System.Drawing.Size(126, 17);
            this.optionDebugVram.TabIndex = 9;
            this.optionDebugVram.Text = "Debug VRAM dumps";
            this.optionDebugVram.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(38, 66);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Textures folder:";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(3, 90);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(110, 90);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            // 
            // pathFile
            // 
            this.pathFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.pathFile.Location = new System.Drawing.Point(124, 5);
            this.pathFile.Name = "pathFile";
            this.pathFile.Size = new System.Drawing.Size(352, 20);
            this.pathFile.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(111, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "CTR file (*.lev, *.mpk):";
            // 
            // pathFolder
            // 
            this.pathFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.pathFolder.Location = new System.Drawing.Point(124, 62);
            this.pathFolder.Name = "pathFolder";
            this.pathFolder.Size = new System.Drawing.Size(352, 20);
            this.pathFolder.TabIndex = 3;
            // 
            // actionBrowseFile
            // 
            this.actionBrowseFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.actionBrowseFile.Location = new System.Drawing.Point(482, 3);
            this.actionBrowseFile.Name = "actionBrowseFile";
            this.actionBrowseFile.Size = new System.Drawing.Size(61, 23);
            this.actionBrowseFile.TabIndex = 4;
            this.actionBrowseFile.Text = "Browse...";
            this.actionBrowseFile.UseVisualStyleBackColor = true;
            this.actionBrowseFile.Click += new System.EventHandler(this.actionBrowseFile_Click);
            // 
            // actionBrowseFolder
            // 
            this.actionBrowseFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.actionBrowseFolder.Location = new System.Drawing.Point(482, 61);
            this.actionBrowseFolder.Name = "actionBrowseFolder";
            this.actionBrowseFolder.Size = new System.Drawing.Size(61, 23);
            this.actionBrowseFolder.TabIndex = 1;
            this.actionBrowseFolder.Text = "Browse...";
            this.actionBrowseFolder.UseVisualStyleBackColor = true;
            this.actionBrowseFolder.Click += new System.EventHandler(this.actionBrowseFolder_Click);
            // 
            // actionOpenFolder
            // 
            this.actionOpenFolder.Location = new System.Drawing.Point(549, 61);
            this.actionOpenFolder.Name = "actionOpenFolder";
            this.actionOpenFolder.Size = new System.Drawing.Size(69, 23);
            this.actionOpenFolder.TabIndex = 6;
            this.actionOpenFolder.Text = "Open folder";
            this.actionOpenFolder.UseVisualStyleBackColor = true;
            this.actionOpenFolder.Click += new System.EventHandler(this.actionOpenFolder_Click);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "CTR VRAM file (*.vrm):";
            // 
            // pathVram
            // 
            this.pathVram.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.pathVram.Location = new System.Drawing.Point(124, 34);
            this.pathVram.Name = "pathVram";
            this.pathVram.Size = new System.Drawing.Size(352, 20);
            this.pathVram.TabIndex = 8;
            // 
            // actionBrowseVram
            // 
            this.actionBrowseVram.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.actionBrowseVram.Location = new System.Drawing.Point(482, 33);
            this.actionBrowseVram.Name = "actionBrowseVram";
            this.actionBrowseVram.Size = new System.Drawing.Size(61, 22);
            this.actionBrowseVram.TabIndex = 9;
            this.actionBrowseVram.Text = "Browse...";
            this.actionBrowseVram.UseVisualStyleBackColor = true;
            this.actionBrowseVram.Click += new System.EventHandler(this.actionBrowseVram_Click);
            // 
            // actionRestore
            // 
            this.actionRestore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.actionRestore.Location = new System.Drawing.Point(312, 453);
            this.actionRestore.Name = "actionRestore";
            this.actionRestore.Size = new System.Drawing.Size(120, 24);
            this.actionRestore.TabIndex = 7;
            this.actionRestore.Text = "Restore from backup";
            this.actionRestore.UseVisualStyleBackColor = true;
            this.actionRestore.Click += new System.EventHandler(this.actionRestore_Click);
            // 
            // VramControl
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.actionRestore);
            this.Controls.Add(this.actionExtract);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.actionPack);
            this.DoubleBuffered = true;
            this.Name = "VramControl";
            this.Size = new System.Drawing.Size(640, 480);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.VramControl_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.VramControl_DragEnter);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Button actionPack;
        private Button actionExtract;
        private GroupBox groupBox2;
        private TableLayoutPanel tableLayoutPanel1;
        private TextBox pathFile;
        private TextBox pathFolder;
        private Button actionBrowseFile;
        private Button actionBrowseFolder;
        private Button actionOpenFolder;
        private CheckBox optionTexMed;
        private CheckBox optionTexLow;
        private CheckBox optionDebugVram;
        private Panel panel1;
        private Label label5;
        private Label label4;
        private Button button1;
        private TextBox textBox1;
        private PictureBox pictureBox1;
        private CheckBox optionTexHigh;
        private Button actionRestore;
        private CheckBox optionTexModels;
        private PictureBox pictureBox3;
        private PictureBox pictureBox2;
        private Button button2;
        private Button actionBrowseVram;
        private Label label1;
        private TextBox pathVram;
        private OpenFileDialog ofd;
    }
}
