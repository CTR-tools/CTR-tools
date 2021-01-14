
namespace CTRTools
{
    partial class CtrToolsVramControl
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pathFile = new System.Windows.Forms.TextBox();
            this.pathFolder = new System.Windows.Forms.TextBox();
            this.optionFolder = new System.Windows.Forms.CheckBox();
            this.actionBrowseFile = new System.Windows.Forms.Button();
            this.actionBrowseFolder = new System.Windows.Forms.Button();
            this.actionOpenFolder = new System.Windows.Forms.Button();
            this.optionTextMid = new System.Windows.Forms.CheckBox();
            this.optionTexLow = new System.Windows.Forms.CheckBox();
            this.optionDebugVram = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // actionPack
            // 
            this.actionPack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.actionPack.Location = new System.Drawing.Point(388, 192);
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
            this.actionExtract.Location = new System.Drawing.Point(286, 192);
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
            this.groupBox2.Controls.Add(this.tableLayoutPanel1);
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(481, 183);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Settings";
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
            this.tableLayoutPanel1.Controls.Add(this.label7, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 2);
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
            this.tableLayoutPanel1.Size = new System.Drawing.Size(468, 158);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // pathFile
            // 
            this.pathFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.pathFile.Location = new System.Drawing.Point(119, 5);
            this.pathFile.Name = "pathFile";
            this.pathFile.Size = new System.Drawing.Size(204, 20);
            this.pathFile.TabIndex = 1;
            // 
            // pathFolder
            // 
            this.pathFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.pathFolder.Location = new System.Drawing.Point(119, 34);
            this.pathFolder.Name = "pathFolder";
            this.pathFolder.Size = new System.Drawing.Size(204, 20);
            this.pathFolder.TabIndex = 3;
            // 
            // optionFolder
            // 
            this.optionFolder.AutoSize = true;
            this.optionFolder.Checked = true;
            this.optionFolder.CheckState = System.Windows.Forms.CheckState.Checked;
            this.optionFolder.Location = new System.Drawing.Point(3, 3);
            this.optionFolder.Name = "optionFolder";
            this.optionFolder.Size = new System.Drawing.Size(200, 17);
            this.optionFolder.TabIndex = 5;
            this.optionFolder.Text = "Create \"textures\" folder automatically";
            this.optionFolder.UseVisualStyleBackColor = true;
            // 
            // actionBrowseFile
            // 
            this.actionBrowseFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.actionBrowseFile.Location = new System.Drawing.Point(329, 3);
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
            this.actionBrowseFolder.Location = new System.Drawing.Point(329, 33);
            this.actionBrowseFolder.Name = "actionBrowseFolder";
            this.actionBrowseFolder.Size = new System.Drawing.Size(61, 23);
            this.actionBrowseFolder.TabIndex = 1;
            this.actionBrowseFolder.Text = "Browse...";
            this.actionBrowseFolder.UseVisualStyleBackColor = true;
            this.actionBrowseFolder.Click += new System.EventHandler(this.actionBrowseFolder_Click);
            // 
            // actionOpenFolder
            // 
            this.actionOpenFolder.Location = new System.Drawing.Point(396, 33);
            this.actionOpenFolder.Name = "actionOpenFolder";
            this.actionOpenFolder.Size = new System.Drawing.Size(69, 23);
            this.actionOpenFolder.TabIndex = 6;
            this.actionOpenFolder.Text = "Open folder";
            this.actionOpenFolder.UseVisualStyleBackColor = true;
            this.actionOpenFolder.Click += new System.EventHandler(this.actionOpenFolder_Click);
            // 
            // optionTextMid
            // 
            this.optionTextMid.AutoSize = true;
            this.optionTextMid.Checked = true;
            this.optionTextMid.CheckState = System.Windows.Forms.CheckState.Checked;
            this.optionTextMid.Location = new System.Drawing.Point(3, 26);
            this.optionTextMid.Name = "optionTextMid";
            this.optionTextMid.Size = new System.Drawing.Size(115, 17);
            this.optionTextMid.TabIndex = 7;
            this.optionTextMid.Text = "Export mid textures";
            this.optionTextMid.UseVisualStyleBackColor = true;
            // 
            // optionTexLow
            // 
            this.optionTexLow.AutoSize = true;
            this.optionTexLow.Checked = true;
            this.optionTexLow.CheckState = System.Windows.Forms.CheckState.Checked;
            this.optionTexLow.Location = new System.Drawing.Point(3, 49);
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
            this.optionDebugVram.Size = new System.Drawing.Size(164, 17);
            this.optionDebugVram.TabIndex = 9;
            this.optionDebugVram.Text = "Output debug VRAM bitmaps";
            this.optionDebugVram.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.optionFolder);
            this.panel1.Controls.Add(this.optionTextMid);
            this.panel1.Controls.Add(this.optionTexLow);
            this.panel1.Controls.Add(this.optionDebugVram);
            this.panel1.Location = new System.Drawing.Point(119, 62);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(204, 92);
            this.panel1.TabIndex = 4;
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
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(67, 102);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(46, 13);
            this.label7.TabIndex = 4;
            this.label7.Text = "Options:";
            // 
            // CtrToolsVramControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.actionExtract);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.actionPack);
            this.MinimumSize = new System.Drawing.Size(488, 223);
            this.Name = "CtrToolsVramControl";
            this.Size = new System.Drawing.Size(488, 223);
            this.groupBox2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button actionPack;
        private System.Windows.Forms.Button actionExtract;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox pathFile;
        private System.Windows.Forms.TextBox pathFolder;
        private System.Windows.Forms.CheckBox optionFolder;
        private System.Windows.Forms.Button actionBrowseFile;
        private System.Windows.Forms.Button actionBrowseFolder;
        private System.Windows.Forms.Button actionOpenFolder;
        private System.Windows.Forms.CheckBox optionTextMid;
        private System.Windows.Forms.CheckBox optionTexLow;
        private System.Windows.Forms.CheckBox optionDebugVram;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
    }
}
