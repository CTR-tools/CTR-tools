
namespace CTRTools
{
    partial class VramControl
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.actionPack = new System.Windows.Forms.Button();
            this.actionExtract = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
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
            this.actionRestore = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // actionPack
            // 
            this.actionPack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.actionPack.Location = new System.Drawing.Point(388, 224);
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
            this.actionExtract.Location = new System.Drawing.Point(286, 224);
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
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Controls.Add(this.tableLayoutPanel1);
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(481, 215);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Settings";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 186);
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
            this.textBox1.Location = new System.Drawing.Point(157, 188);
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
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.pathFile, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.pathFolder, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.actionBrowseFile, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.actionBrowseFolder, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.actionOpenFolder, 4, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 19);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(469, 158);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.optionTexHigh);
            this.panel1.Controls.Add(this.optionTexMed);
            this.panel1.Controls.Add(this.optionTexLow);
            this.panel1.Controls.Add(this.optionDebugVram);
            this.panel1.Location = new System.Drawing.Point(119, 62);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(204, 91);
            this.panel1.TabIndex = 4;
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
            this.optionDebugVram.Location = new System.Drawing.Point(3, 72);
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
            this.label5.Location = new System.Drawing.Point(33, 38);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Textures folder:";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(3, 62);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(110, 91);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            // 
            // pathFile
            // 
            this.pathFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.pathFile.Location = new System.Drawing.Point(119, 5);
            this.pathFile.Name = "pathFile";
            this.pathFile.Size = new System.Drawing.Size(205, 20);
            this.pathFile.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(110, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "CTR scene file (*.lev):";
            // 
            // pathFolder
            // 
            this.pathFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.pathFolder.Location = new System.Drawing.Point(119, 34);
            this.pathFolder.Name = "pathFolder";
            this.pathFolder.Size = new System.Drawing.Size(205, 20);
            this.pathFolder.TabIndex = 3;
            // 
            // actionBrowseFile
            // 
            this.actionBrowseFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.actionBrowseFile.Location = new System.Drawing.Point(330, 3);
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
            this.actionBrowseFolder.Location = new System.Drawing.Point(330, 33);
            this.actionBrowseFolder.Name = "actionBrowseFolder";
            this.actionBrowseFolder.Size = new System.Drawing.Size(61, 23);
            this.actionBrowseFolder.TabIndex = 1;
            this.actionBrowseFolder.Text = "Browse...";
            this.actionBrowseFolder.UseVisualStyleBackColor = true;
            this.actionBrowseFolder.Click += new System.EventHandler(this.actionBrowseFolder_Click);
            // 
            // actionOpenFolder
            // 
            this.actionOpenFolder.Location = new System.Drawing.Point(397, 33);
            this.actionOpenFolder.Name = "actionOpenFolder";
            this.actionOpenFolder.Size = new System.Drawing.Size(69, 23);
            this.actionOpenFolder.TabIndex = 6;
            this.actionOpenFolder.Text = "Open folder";
            this.actionOpenFolder.UseVisualStyleBackColor = true;
            this.actionOpenFolder.Click += new System.EventHandler(this.actionOpenFolder_Click);
            // 
            // actionRestore
            // 
            this.actionRestore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.actionRestore.Location = new System.Drawing.Point(160, 224);
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
            this.MinimumSize = new System.Drawing.Size(488, 223);
            this.Name = "VramControl";
            this.Size = new System.Drawing.Size(488, 251);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.CtrToolsVramControl_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.CtrToolsVramControl_DragEnter);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button actionPack;
        private System.Windows.Forms.Button actionExtract;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox pathFile;
        private System.Windows.Forms.TextBox pathFolder;
        private System.Windows.Forms.Button actionBrowseFile;
        private System.Windows.Forms.Button actionBrowseFolder;
        private System.Windows.Forms.Button actionOpenFolder;
        private System.Windows.Forms.CheckBox optionTexMed;
        private System.Windows.Forms.CheckBox optionTexLow;
        private System.Windows.Forms.CheckBox optionDebugVram;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox optionTexHigh;
        private System.Windows.Forms.Button actionRestore;
    }
}
