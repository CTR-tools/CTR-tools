namespace CTRTools
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabAbout = new System.Windows.Forms.TabPage();
            this.warning = new System.Windows.Forms.Label();
            this.signLabel = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.appLogo = new System.Windows.Forms.PictureBox();
            this.discordLogo = new System.Windows.Forms.PictureBox();
            this.githubLogo = new System.Windows.Forms.PictureBox();
            this.tabBig = new System.Windows.Forms.TabPage();
            this.bigFileControl1 = new CTRTools.Controls.BigFileControl();
            this.tabVram = new System.Windows.Forms.TabPage();
            this.vramControl1 = new CTRTools.Controls.VramControl();
            this.tabCtr = new System.Windows.Forms.TabPage();
            this.ctrControl1 = new CTRTools.Controls.CtrControl();
            this.tabLev = new System.Windows.Forms.TabPage();
            this.levControl1 = new CTRTools.Controls.LevControl();
            this.tabCseq = new System.Windows.Forms.TabPage();
            this.cseqControl1 = new CTRTools.Controls.CseqControl();
            this.tabLang = new System.Windows.Forms.TabPage();
            this.langControl1 = new CTRTools.Controls.LangControl();
            this.tabXa = new System.Windows.Forms.TabPage();
            this.xaControl1 = new CTRTools.Controls.XaControl();
            this.tabControl1.SuspendLayout();
            this.tabAbout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.appLogo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.discordLogo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.githubLogo)).BeginInit();
            this.tabBig.SuspendLayout();
            this.tabVram.SuspendLayout();
            this.tabCtr.SuspendLayout();
            this.tabLev.SuspendLayout();
            this.tabCseq.SuspendLayout();
            this.tabLang.SuspendLayout();
            this.tabXa.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabAbout);
            this.tabControl1.Controls.Add(this.tabBig);
            this.tabControl1.Controls.Add(this.tabVram);
            this.tabControl1.Controls.Add(this.tabCtr);
            this.tabControl1.Controls.Add(this.tabLev);
            this.tabControl1.Controls.Add(this.tabCseq);
            this.tabControl1.Controls.Add(this.tabLang);
            this.tabControl1.Controls.Add(this.tabXa);
            this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tabControl1.Location = new System.Drawing.Point(7, 7);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(611, 429);
            this.tabControl1.TabIndex = 1;
            // 
            // tabAbout
            // 
            this.tabAbout.BackColor = System.Drawing.Color.White;
            this.tabAbout.Controls.Add(this.warning);
            this.tabAbout.Controls.Add(this.signLabel);
            this.tabAbout.Controls.Add(this.labelVersion);
            this.tabAbout.Controls.Add(this.appLogo);
            this.tabAbout.Controls.Add(this.discordLogo);
            this.tabAbout.Controls.Add(this.githubLogo);
            this.tabAbout.Location = new System.Drawing.Point(4, 22);
            this.tabAbout.Name = "tabAbout";
            this.tabAbout.Size = new System.Drawing.Size(603, 403);
            this.tabAbout.TabIndex = 7;
            this.tabAbout.Text = "About";
            // 
            // warning
            // 
            this.warning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.warning.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.warning.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.warning.Location = new System.Drawing.Point(3, 17);
            this.warning.Name = "warning";
            this.warning.Size = new System.Drawing.Size(597, 61);
            this.warning.TabIndex = 10;
            this.warning.Text = "Warning! This tool is in the early stage of development.\r\nSome features may be mi" +
    "ssing or won\'t work as intended.";
            this.warning.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // signLabel
            // 
            this.signLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.signLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.signLabel.Location = new System.Drawing.Point(3, 308);
            this.signLabel.Name = "signLabel";
            this.signLabel.Size = new System.Drawing.Size(597, 24);
            this.signLabel.TabIndex = 9;
            this.signLabel.Text = "Signature from DLL";
            this.signLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelVersion
            // 
            this.labelVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.labelVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelVersion.Location = new System.Drawing.Point(3, 177);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(597, 24);
            this.labelVersion.TabIndex = 6;
            this.labelVersion.Text = "Framework version from DLL";
            this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // appLogo
            // 
            this.appLogo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.appLogo.ErrorImage = null;
            this.appLogo.Image = global::CTRTools.Properties.Resources.ctr_tools_logo;
            this.appLogo.Location = new System.Drawing.Point(3, 90);
            this.appLogo.Name = "appLogo";
            this.appLogo.Size = new System.Drawing.Size(597, 84);
            this.appLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.appLogo.TabIndex = 3;
            this.appLogo.TabStop = false;
            // 
            // discordLogo
            // 
            this.discordLogo.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.discordLogo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.discordLogo.Image = global::CTRTools.Properties.Resources.icon_discord;
            this.discordLogo.InitialImage = null;
            this.discordLogo.Location = new System.Drawing.Point(315, 228);
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
            this.githubLogo.Location = new System.Drawing.Point(236, 228);
            this.githubLogo.Name = "githubLogo";
            this.githubLogo.Size = new System.Drawing.Size(64, 64);
            this.githubLogo.TabIndex = 4;
            this.githubLogo.TabStop = false;
            this.githubLogo.Click += new System.EventHandler(this.githubBox_Click);
            // 
            // tabBig
            // 
            this.tabBig.Controls.Add(this.bigFileControl1);
            this.tabBig.Location = new System.Drawing.Point(4, 22);
            this.tabBig.Name = "tabBig";
            this.tabBig.Size = new System.Drawing.Size(603, 403);
            this.tabBig.TabIndex = 5;
            this.tabBig.Text = "BIG archive";
            this.tabBig.UseVisualStyleBackColor = true;
            // 
            // bigFileControl1
            // 
            this.bigFileControl1.AllowDrop = true;
            this.bigFileControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bigFileControl1.Location = new System.Drawing.Point(0, 0);
            this.bigFileControl1.MinimumSize = new System.Drawing.Size(488, 223);
            this.bigFileControl1.Name = "bigFileControl1";
            this.bigFileControl1.Size = new System.Drawing.Size(603, 403);
            this.bigFileControl1.TabIndex = 0;
            // 
            // tabVram
            // 
            this.tabVram.Controls.Add(this.vramControl1);
            this.tabVram.Location = new System.Drawing.Point(4, 22);
            this.tabVram.Name = "tabVram";
            this.tabVram.Size = new System.Drawing.Size(603, 403);
            this.tabVram.TabIndex = 8;
            this.tabVram.Text = "VRAM textures";
            this.tabVram.UseVisualStyleBackColor = true;
            // 
            // vramControl1
            // 
            this.vramControl1.AllowDrop = true;
            this.vramControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vramControl1.Location = new System.Drawing.Point(0, 0);
            this.vramControl1.MinimumSize = new System.Drawing.Size(461, 218);
            this.vramControl1.Name = "vramControl1";
            this.vramControl1.Size = new System.Drawing.Size(603, 403);
            this.vramControl1.TabIndex = 0;
            // 
            // tabCtr
            // 
            this.tabCtr.Controls.Add(this.ctrControl1);
            this.tabCtr.Location = new System.Drawing.Point(4, 22);
            this.tabCtr.Name = "tabCtr";
            this.tabCtr.Size = new System.Drawing.Size(603, 403);
            this.tabCtr.TabIndex = 9;
            this.tabCtr.Text = "CTR models";
            this.tabCtr.UseVisualStyleBackColor = true;
            // 
            // ctrControl1
            // 
            this.ctrControl1.AllowDrop = true;
            this.ctrControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrControl1.Location = new System.Drawing.Point(0, 0);
            this.ctrControl1.Name = "ctrControl1";
            this.ctrControl1.Size = new System.Drawing.Size(603, 403);
            this.ctrControl1.TabIndex = 0;
            // 
            // tabLev
            // 
            this.tabLev.Controls.Add(this.levControl1);
            this.tabLev.Location = new System.Drawing.Point(4, 22);
            this.tabLev.Name = "tabLev";
            this.tabLev.Size = new System.Drawing.Size(603, 403);
            this.tabLev.TabIndex = 0;
            this.tabLev.Text = "LEV scenes";
            this.tabLev.UseVisualStyleBackColor = true;
            // 
            // levControl1
            // 
            this.levControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.levControl1.Location = new System.Drawing.Point(0, 0);
            this.levControl1.Name = "levControl1";
            this.levControl1.Size = new System.Drawing.Size(603, 403);
            this.levControl1.TabIndex = 0;
            // 
            // tabCseq
            // 
            this.tabCseq.Controls.Add(this.cseqControl1);
            this.tabCseq.Location = new System.Drawing.Point(4, 22);
            this.tabCseq.Name = "tabCseq";
            this.tabCseq.Size = new System.Drawing.Size(603, 403);
            this.tabCseq.TabIndex = 10;
            this.tabCseq.Text = "CSEQ music";
            this.tabCseq.UseVisualStyleBackColor = true;
            // 
            // cseqControl1
            // 
            this.cseqControl1.AllowDrop = true;
            this.cseqControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cseqControl1.Location = new System.Drawing.Point(0, 0);
            this.cseqControl1.Name = "cseqControl1";
            this.cseqControl1.Size = new System.Drawing.Size(603, 403);
            this.cseqControl1.TabIndex = 11;
            // 
            // tabLang
            // 
            this.tabLang.Controls.Add(this.langControl1);
            this.tabLang.Location = new System.Drawing.Point(4, 22);
            this.tabLang.Name = "tabLang";
            this.tabLang.Size = new System.Drawing.Size(603, 403);
            this.tabLang.TabIndex = 12;
            this.tabLang.Text = "LNG text";
            this.tabLang.UseVisualStyleBackColor = true;
            // 
            // langControl1
            // 
            this.langControl1.AllowDrop = true;
            this.langControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.langControl1.Location = new System.Drawing.Point(0, 0);
            this.langControl1.Name = "langControl1";
            this.langControl1.Size = new System.Drawing.Size(603, 403);
            this.langControl1.TabIndex = 0;
            // 
            // tabXa
            // 
            this.tabXa.Controls.Add(this.xaControl1);
            this.tabXa.Location = new System.Drawing.Point(4, 22);
            this.tabXa.Name = "tabXa";
            this.tabXa.Size = new System.Drawing.Size(603, 403);
            this.tabXa.TabIndex = 13;
            this.tabXa.Text = "XA audio";
            this.tabXa.UseVisualStyleBackColor = true;
            // 
            // xaControl1
            // 
            this.xaControl1.AllowDrop = true;
            this.xaControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xaControl1.Location = new System.Drawing.Point(0, 0);
            this.xaControl1.Name = "xaControl1";
            this.xaControl1.Size = new System.Drawing.Size(603, 403);
            this.xaControl1.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 442);
            this.Controls.Add(this.tabControl1);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(640, 480);
            this.Name = "MainForm";
            this.Text = "CTR-tools-gui";
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.tabControl1.ResumeLayout(false);
            this.tabAbout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.appLogo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.discordLogo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.githubLogo)).EndInit();
            this.tabBig.ResumeLayout(false);
            this.tabVram.ResumeLayout(false);
            this.tabCtr.ResumeLayout(false);
            this.tabLev.ResumeLayout(false);
            this.tabCseq.ResumeLayout(false);
            this.tabLang.ResumeLayout(false);
            this.tabXa.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.PictureBox appLogo;
        private System.Windows.Forms.PictureBox discordLogo;
        private System.Windows.Forms.PictureBox githubLogo;

        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Label signLabel;

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabLev;
        private System.Windows.Forms.TabPage tabBig;
        private System.Windows.Forms.TabPage tabAbout;
        private System.Windows.Forms.TabPage tabVram;
        private System.Windows.Forms.TabPage tabCtr;
        private System.Windows.Forms.TabPage tabCseq;
        private System.Windows.Forms.TabPage tabLang;

        private Controls.VramControl vramControl1;
        private Controls.BigFileControl bigFileControl1;
        private Controls.CseqControl cseqControl1;
        private Controls.CtrControl ctrControl1;
        private Controls.LevControl levControl1;
        private Controls.LangControl langControl1;
        private System.Windows.Forms.Label warning;
        private System.Windows.Forms.TabPage tabXa;
        private Controls.XaControl xaControl1;
    }
}