namespace CTRTools
{
    partial class MainForm
    {
        /// <summary>
        /// Требуется переменная конструктора.
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

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showConsoleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabAbout = new System.Windows.Forms.TabPage();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.appLogo = new System.Windows.Forms.PictureBox();
            this.discordLogo = new System.Windows.Forms.PictureBox();
            this.githubLogo = new System.Windows.Forms.PictureBox();
            this.tabBig = new System.Windows.Forms.TabPage();
            this.bigFileControl1 = new CTRTools.Controls.BigFileControl();
            this.tabVram = new System.Windows.Forms.TabPage();
            this.ctrToolsVramControl1 = new CTRTools.VramControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.ctrControl1 = new CTRTools.Controls.CtrControl();
            this.tabCtr = new System.Windows.Forms.TabPage();
            this.levControl1 = new CTRTools.LevControl();
            this.tabEmu = new System.Windows.Forms.TabPage();
            this.button28 = new System.Windows.Forms.Button();
            this.propertyGrid2 = new System.Windows.Forms.PropertyGrid();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.button26 = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exportFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replaceFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabAbout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.appLogo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.discordLogo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.githubLogo)).BeginInit();
            this.tabBig.SuspendLayout();
            this.tabVram.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabCtr.SuspendLayout();
            this.tabEmu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.showConsoleToolStripMenuItem,
            this.restoreToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(624, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.loadToolStripMenuItem.Text = "Open...";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.saveToolStripMenuItem.Text = "Save...";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(109, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // showConsoleToolStripMenuItem
            // 
            this.showConsoleToolStripMenuItem.Name = "showConsoleToolStripMenuItem";
            this.showConsoleToolStripMenuItem.Size = new System.Drawing.Size(92, 20);
            this.showConsoleToolStripMenuItem.Text = "Show console";
            this.showConsoleToolStripMenuItem.Visible = false;
            // 
            // restoreToolStripMenuItem
            // 
            this.restoreToolStripMenuItem.Name = "restoreToolStripMenuItem";
            this.restoreToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.restoreToolStripMenuItem.Text = "Restore";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabAbout);
            this.tabControl1.Controls.Add(this.tabBig);
            this.tabControl1.Controls.Add(this.tabVram);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabCtr);
            this.tabControl1.Controls.Add(this.tabEmu);
            this.tabControl1.Location = new System.Drawing.Point(4, 28);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(617, 410);
            this.tabControl1.TabIndex = 2;
            // 
            // tabAbout
            // 
            this.tabAbout.Controls.Add(this.label6);
            this.tabAbout.Controls.Add(this.label3);
            this.tabAbout.Controls.Add(this.appLogo);
            this.tabAbout.Controls.Add(this.discordLogo);
            this.tabAbout.Controls.Add(this.githubLogo);
            this.tabAbout.Location = new System.Drawing.Point(4, 22);
            this.tabAbout.Name = "tabAbout";
            this.tabAbout.Size = new System.Drawing.Size(609, 384);
            this.tabAbout.TabIndex = 7;
            this.tabAbout.Text = "About";
            this.tabAbout.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.Location = new System.Drawing.Point(213, 299);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(200, 24);
            this.label6.TabIndex = 9;
            this.label6.Text = "2016-2021, DCxDemo*";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(131, 168);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(251, 24);
            this.label3.TabIndex = 6;
            this.label3.Text = "Framework version from DLL";
            // 
            // appLogo
            // 
            this.appLogo.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.appLogo.ErrorImage = null;
            this.appLogo.Image = global::CTRTools.Properties.Resources.ctr_tools_logo;
            this.appLogo.Location = new System.Drawing.Point(125, 81);
            this.appLogo.Name = "appLogo";
            this.appLogo.Size = new System.Drawing.Size(365, 84);
            this.appLogo.TabIndex = 3;
            this.appLogo.TabStop = false;
            // 
            // discordLogo
            // 
            this.discordLogo.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.discordLogo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.discordLogo.Image = global::CTRTools.Properties.Resources.icon_discord;
            this.discordLogo.InitialImage = null;
            this.discordLogo.Location = new System.Drawing.Point(318, 219);
            this.discordLogo.Name = "discordLogo";
            this.discordLogo.Size = new System.Drawing.Size(64, 64);
            this.discordLogo.TabIndex = 5;
            this.discordLogo.TabStop = false;
            this.discordLogo.Click += new System.EventHandler(this.discordBox_Click);
            // 
            // githubLogo
            // 
            this.githubLogo.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.githubLogo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.githubLogo.ErrorImage = null;
            this.githubLogo.Image = global::CTRTools.Properties.Resources.icon_github;
            this.githubLogo.Location = new System.Drawing.Point(239, 219);
            this.githubLogo.Name = "githubLogo";
            this.githubLogo.Size = new System.Drawing.Size(64, 64);
            this.githubLogo.TabIndex = 4;
            this.githubLogo.TabStop = false;
            this.githubLogo.Click += new System.EventHandler(this.gitHubBox_Click);
            // 
            // tabBig
            // 
            this.tabBig.Controls.Add(this.bigFileControl1);
            this.tabBig.Location = new System.Drawing.Point(4, 22);
            this.tabBig.Name = "tabBig";
            this.tabBig.Size = new System.Drawing.Size(609, 384);
            this.tabBig.TabIndex = 5;
            this.tabBig.Text = "BIG";
            this.tabBig.UseVisualStyleBackColor = true;
            // 
            // bigFileControl1
            // 
            this.bigFileControl1.AllowDrop = true;
            this.bigFileControl1.Location = new System.Drawing.Point(4, 3);
            this.bigFileControl1.MinimumSize = new System.Drawing.Size(488, 223);
            this.bigFileControl1.Name = "bigFileControl1";
            this.bigFileControl1.Size = new System.Drawing.Size(600, 377);
            this.bigFileControl1.TabIndex = 0;
            // 
            // tabVram
            // 
            this.tabVram.Controls.Add(this.ctrToolsVramControl1);
            this.tabVram.Location = new System.Drawing.Point(4, 22);
            this.tabVram.Name = "tabVram";
            this.tabVram.Size = new System.Drawing.Size(609, 384);
            this.tabVram.TabIndex = 8;
            this.tabVram.Text = "VRAM";
            this.tabVram.UseVisualStyleBackColor = true;
            // 
            // ctrToolsVramControl1
            // 
            this.ctrToolsVramControl1.AllowDrop = true;
            this.ctrToolsVramControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ctrToolsVramControl1.Location = new System.Drawing.Point(4, 3);
            this.ctrToolsVramControl1.MinimumSize = new System.Drawing.Size(461, 218);
            this.ctrToolsVramControl1.Name = "ctrToolsVramControl1";
            this.ctrToolsVramControl1.Size = new System.Drawing.Size(600, 377);
            this.ctrToolsVramControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.ctrControl1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(609, 384);
            this.tabPage1.TabIndex = 9;
            this.tabPage1.Text = "CTR models";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // ctrControl1
            // 
            this.ctrControl1.AllowDrop = true;
            this.ctrControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrControl1.Location = new System.Drawing.Point(3, 3);
            this.ctrControl1.MinimumSize = new System.Drawing.Size(488, 223);
            this.ctrControl1.Name = "ctrControl1";
            this.ctrControl1.Size = new System.Drawing.Size(603, 378);
            this.ctrControl1.TabIndex = 0;
            // 
            // tabCtr
            // 
            this.tabCtr.Controls.Add(this.levControl1);
            this.tabCtr.Location = new System.Drawing.Point(4, 22);
            this.tabCtr.Margin = new System.Windows.Forms.Padding(5);
            this.tabCtr.Name = "tabCtr";
            this.tabCtr.Padding = new System.Windows.Forms.Padding(5);
            this.tabCtr.Size = new System.Drawing.Size(609, 384);
            this.tabCtr.TabIndex = 0;
            this.tabCtr.Text = "LEV";
            this.tabCtr.UseVisualStyleBackColor = true;
            // 
            // levControl1
            // 
            this.levControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.levControl1.Location = new System.Drawing.Point(8, 8);
            this.levControl1.Name = "levControl1";
            this.levControl1.Size = new System.Drawing.Size(593, 368);
            this.levControl1.TabIndex = 0;
            // 
            // tabEmu
            // 
            this.tabEmu.Controls.Add(this.button28);
            this.tabEmu.Controls.Add(this.propertyGrid2);
            this.tabEmu.Controls.Add(this.checkBox1);
            this.tabEmu.Controls.Add(this.numericUpDown2);
            this.tabEmu.Controls.Add(this.comboBox1);
            this.tabEmu.Controls.Add(this.textBox5);
            this.tabEmu.Controls.Add(this.button26);
            this.tabEmu.Location = new System.Drawing.Point(4, 22);
            this.tabEmu.Name = "tabEmu";
            this.tabEmu.Size = new System.Drawing.Size(609, 384);
            this.tabEmu.TabIndex = 6;
            this.tabEmu.Text = "ePSXe";
            this.tabEmu.UseVisualStyleBackColor = true;
            // 
            // button28
            // 
            this.button28.Location = new System.Drawing.Point(8, 370);
            this.button28.Name = "button28";
            this.button28.Size = new System.Drawing.Size(75, 23);
            this.button28.TabIndex = 6;
            this.button28.Text = "button28";
            this.button28.UseVisualStyleBackColor = true;
            // 
            // propertyGrid2
            // 
            this.propertyGrid2.Location = new System.Drawing.Point(334, 59);
            this.propertyGrid2.Name = "propertyGrid2";
            this.propertyGrid2.Size = new System.Drawing.Size(343, 305);
            this.propertyGrid2.TabIndex = 5;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(212, 34);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(61, 17);
            this.checkBox1.TabIndex = 4;
            this.checkBox1.Text = "60 FPS";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.Location = new System.Drawing.Point(612, 30);
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(64, 20);
            this.numericUpDown2.TabIndex = 3;
            this.numericUpDown2.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Turbo",
            "Rolling Bomb",
            "Rocket",
            "TNT Crate",
            "Beaker",
            "Spring (removed)",
            "Shield",
            "Mask",
            "Clock",
            "Orb",
            "Rolling Bomb x3",
            "Rocket x3",
            "Invisibility",
            "Super Turbo",
            "Weapon Slot 0x0E",
            "Empty",
            "Random"});
            this.comboBox1.Location = new System.Drawing.Point(435, 30);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(171, 21);
            this.comboBox1.TabIndex = 2;
            // 
            // textBox5
            // 
            this.textBox5.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox5.Location = new System.Drawing.Point(8, 59);
            this.textBox5.Multiline = true;
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(320, 305);
            this.textBox5.TabIndex = 1;
            // 
            // button26
            // 
            this.button26.Location = new System.Drawing.Point(8, 30);
            this.button26.Name = "button26";
            this.button26.Size = new System.Drawing.Size(198, 23);
            this.button26.TabIndex = 0;
            this.button26.Text = "Attach";
            this.button26.UseVisualStyleBackColor = true;
            this.button26.Click += new System.EventHandler(this.button26_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportFileToolStripMenuItem,
            this.replaceFileToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(135, 48);
            // 
            // exportFileToolStripMenuItem
            // 
            this.exportFileToolStripMenuItem.Name = "exportFileToolStripMenuItem";
            this.exportFileToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.exportFileToolStripMenuItem.Text = "Export file";
            // 
            // replaceFileToolStripMenuItem
            // 
            this.replaceFileToolStripMenuItem.Name = "replaceFileToolStripMenuItem";
            this.replaceFileToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.replaceFileToolStripMenuItem.Text = "Replace file";
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 442);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(640, 480);
            this.Name = "MainForm";
            this.Text = "CTR-tools-gui";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabAbout.ResumeLayout(false);
            this.tabAbout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.appLogo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.discordLogo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.githubLogo)).EndInit();
            this.tabBig.ResumeLayout(false);
            this.tabVram.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabCtr.ResumeLayout(false);
            this.tabEmu.ResumeLayout(false);
            this.tabEmu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabCtr;
        private System.Windows.Forms.ToolStripMenuItem showConsoleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem restoreToolStripMenuItem;
        private System.Windows.Forms.TabPage tabBig;
        private System.Windows.Forms.TabPage tabEmu;
        private System.Windows.Forms.Button button26;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.PropertyGrid propertyGrid2;
        private System.Windows.Forms.TabPage tabAbout;
        private System.Windows.Forms.PictureBox discordLogo;
        private System.Windows.Forms.PictureBox githubLogo;
        private System.Windows.Forms.PictureBox appLogo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem exportFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem replaceFileToolStripMenuItem;
        private System.Windows.Forms.Button button28;
        private System.Windows.Forms.TabPage tabVram;
        private VramControl ctrToolsVramControl1;
        private Controls.BigFileControl bigFileControl1;
        private LevControl levControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private Controls.CtrControl ctrControl1;
    }
}

