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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabAbout = new System.Windows.Forms.TabPage();
            this.warning = new System.Windows.Forms.Label();
            this.signLabel = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.appLogo = new System.Windows.Forms.PictureBox();
            this.discordLogo = new System.Windows.Forms.PictureBox();
            this.githubLogo = new System.Windows.Forms.PictureBox();
            this.tabBig = new System.Windows.Forms.TabPage();
            this.bigFileControl = new CTRTools.Controls.BigFileControl();
            this.tabVram = new System.Windows.Forms.TabPage();
            this.vramControl = new CTRTools.Controls.VramControl();
            this.tabCtr = new System.Windows.Forms.TabPage();
            this.ctrControl = new CTRTools.Controls.CtrControl();
            this.tabLev = new System.Windows.Forms.TabPage();
            this.levControl = new CTRTools.Controls.LevControl();
            this.tabCseq = new System.Windows.Forms.TabPage();
            this.cseqControl = new CTRTools.Controls.CseqControl();
            this.tabLang = new System.Windows.Forms.TabPage();
            this.langControl = new CTRTools.Controls.LangControl();
            this.tabXa = new System.Windows.Forms.TabPage();
            this.xaControl = new CTRTools.Controls.XaControl();
            this.tabControl.SuspendLayout();
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
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabAbout);
            this.tabControl.Controls.Add(this.tabBig);
            this.tabControl.Controls.Add(this.tabVram);
            this.tabControl.Controls.Add(this.tabCtr);
            this.tabControl.Controls.Add(this.tabLev);
            this.tabControl.Controls.Add(this.tabCseq);
            this.tabControl.Controls.Add(this.tabLang);
            this.tabControl.Controls.Add(this.tabXa);
            this.tabControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tabControl.Location = new System.Drawing.Point(7, 7);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(619, 438);
            this.tabControl.TabIndex = 1;
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
            this.tabAbout.Size = new System.Drawing.Size(611, 412);
            this.tabAbout.TabIndex = 7;
            this.tabAbout.Text = "About";
            // 
            // warning
            // 
            this.warning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.warning.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.warning.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.warning.Location = new System.Drawing.Point(3, 22);
            this.warning.Name = "warning";
            this.warning.Size = new System.Drawing.Size(605, 61);
            this.warning.TabIndex = 10;
            this.warning.Text = "Warning! This tool is in the early stage of development.\r\nSome features may be mi" +
    "ssing or won\'t work as intended.";
            this.warning.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // signLabel
            // 
            this.signLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.signLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.signLabel.Location = new System.Drawing.Point(3, 313);
            this.signLabel.Name = "signLabel";
            this.signLabel.Size = new System.Drawing.Size(605, 24);
            this.signLabel.TabIndex = 9;
            this.signLabel.Text = "Signature from DLL";
            this.signLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelVersion
            // 
            this.labelVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.labelVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelVersion.Location = new System.Drawing.Point(3, 182);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(605, 24);
            this.labelVersion.TabIndex = 6;
            this.labelVersion.Text = "Framework version from DLL";
            this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // appLogo
            // 
            this.appLogo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.appLogo.ErrorImage = null;
            this.appLogo.Image = global::CTRTools.Properties.Resources.ctr_tools_logo;
            this.appLogo.Location = new System.Drawing.Point(3, 95);
            this.appLogo.Name = "appLogo";
            this.appLogo.Size = new System.Drawing.Size(605, 84);
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
            this.discordLogo.Location = new System.Drawing.Point(319, 233);
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
            this.githubLogo.Location = new System.Drawing.Point(240, 233);
            this.githubLogo.Name = "githubLogo";
            this.githubLogo.Size = new System.Drawing.Size(64, 64);
            this.githubLogo.TabIndex = 4;
            this.githubLogo.TabStop = false;
            this.githubLogo.Click += new System.EventHandler(this.githubBox_Click);
            // 
            // tabBig
            // 
            this.tabBig.Controls.Add(this.bigFileControl);
            this.tabBig.Location = new System.Drawing.Point(4, 22);
            this.tabBig.Name = "tabBig";
            this.tabBig.Size = new System.Drawing.Size(611, 412);
            this.tabBig.TabIndex = 5;
            this.tabBig.Text = "BIG archive";
            this.tabBig.UseVisualStyleBackColor = true;
            // 
            // bigFileControl
            // 
            this.bigFileControl.AllowDrop = true;
            this.bigFileControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bigFileControl.Location = new System.Drawing.Point(0, 0);
            this.bigFileControl.MinimumSize = new System.Drawing.Size(488, 223);
            this.bigFileControl.Name = "bigFileControl";
            this.bigFileControl.Size = new System.Drawing.Size(611, 412);
            this.bigFileControl.TabIndex = 0;
            // 
            // tabVram
            // 
            this.tabVram.Controls.Add(this.vramControl);
            this.tabVram.Location = new System.Drawing.Point(4, 22);
            this.tabVram.Name = "tabVram";
            this.tabVram.Size = new System.Drawing.Size(611, 412);
            this.tabVram.TabIndex = 8;
            this.tabVram.Text = "VRAM textures";
            this.tabVram.UseVisualStyleBackColor = true;
            // 
            // vramControl
            // 
            this.vramControl.AllowDrop = true;
            this.vramControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vramControl.Location = new System.Drawing.Point(0, 0);
            this.vramControl.MinimumSize = new System.Drawing.Size(461, 218);
            this.vramControl.Name = "vramControl";
            this.vramControl.Size = new System.Drawing.Size(611, 412);
            this.vramControl.TabIndex = 0;
            // 
            // tabCtr
            // 
            this.tabCtr.Controls.Add(this.ctrControl);
            this.tabCtr.Location = new System.Drawing.Point(4, 22);
            this.tabCtr.Name = "tabCtr";
            this.tabCtr.Size = new System.Drawing.Size(611, 412);
            this.tabCtr.TabIndex = 9;
            this.tabCtr.Text = "CTR models";
            this.tabCtr.UseVisualStyleBackColor = true;
            // 
            // ctrControl
            // 
            this.ctrControl.AllowDrop = true;
            this.ctrControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrControl.Location = new System.Drawing.Point(0, 0);
            this.ctrControl.Name = "ctrControl";
            this.ctrControl.Size = new System.Drawing.Size(611, 412);
            this.ctrControl.TabIndex = 0;
            // 
            // tabLev
            // 
            this.tabLev.Controls.Add(this.levControl);
            this.tabLev.Location = new System.Drawing.Point(4, 22);
            this.tabLev.Name = "tabLev";
            this.tabLev.Size = new System.Drawing.Size(611, 412);
            this.tabLev.TabIndex = 0;
            this.tabLev.Text = "LEV scenes";
            this.tabLev.UseVisualStyleBackColor = true;
            // 
            // levControl
            // 
            this.levControl.AllowDrop = true;
            this.levControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.levControl.Location = new System.Drawing.Point(0, 0);
            this.levControl.Name = "levControl";
            this.levControl.Size = new System.Drawing.Size(611, 412);
            this.levControl.TabIndex = 0;
            // 
            // tabCseq
            // 
            this.tabCseq.Controls.Add(this.cseqControl);
            this.tabCseq.Location = new System.Drawing.Point(4, 22);
            this.tabCseq.Name = "tabCseq";
            this.tabCseq.Size = new System.Drawing.Size(611, 412);
            this.tabCseq.TabIndex = 10;
            this.tabCseq.Text = "CSEQ music";
            this.tabCseq.UseVisualStyleBackColor = true;
            // 
            // cseqControl
            // 
            this.cseqControl.AllowDrop = true;
            this.cseqControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cseqControl.Location = new System.Drawing.Point(0, 0);
            this.cseqControl.Name = "cseqControl";
            this.cseqControl.Size = new System.Drawing.Size(611, 412);
            this.cseqControl.TabIndex = 11;
            // 
            // tabLang
            // 
            this.tabLang.Controls.Add(this.langControl);
            this.tabLang.Location = new System.Drawing.Point(4, 22);
            this.tabLang.Name = "tabLang";
            this.tabLang.Size = new System.Drawing.Size(611, 412);
            this.tabLang.TabIndex = 12;
            this.tabLang.Text = "LNG text";
            this.tabLang.UseVisualStyleBackColor = true;
            // 
            // langControl
            // 
            this.langControl.AllowDrop = true;
            this.langControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.langControl.Location = new System.Drawing.Point(0, 0);
            this.langControl.Name = "langControl";
            this.langControl.Size = new System.Drawing.Size(611, 412);
            this.langControl.TabIndex = 0;
            // 
            // tabXa
            // 
            this.tabXa.Controls.Add(this.xaControl);
            this.tabXa.Location = new System.Drawing.Point(4, 22);
            this.tabXa.Name = "tabXa";
            this.tabXa.Size = new System.Drawing.Size(611, 412);
            this.tabXa.TabIndex = 13;
            this.tabXa.Text = "XA audio";
            this.tabXa.UseVisualStyleBackColor = true;
            // 
            // xaControl
            // 
            this.xaControl.AllowDrop = true;
            this.xaControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xaControl.Location = new System.Drawing.Point(0, 0);
            this.xaControl.Name = "xaControl";
            this.xaControl.Size = new System.Drawing.Size(611, 412);
            this.xaControl.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 451);
            this.Controls.Add(this.tabControl);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(640, 480);
            this.Name = "MainForm";
            this.Text = "CTR-tools-gui";
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.tabControl.ResumeLayout(false);
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
        private System.Windows.Forms.Label warning;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Label signLabel;

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabAbout;
        private System.Windows.Forms.TabPage tabLev;
        private System.Windows.Forms.TabPage tabBig;
        private System.Windows.Forms.TabPage tabVram;
        private System.Windows.Forms.TabPage tabCtr;
        private System.Windows.Forms.TabPage tabCseq;
        private System.Windows.Forms.TabPage tabLang;
        private System.Windows.Forms.TabPage tabXa;

        private Controls.VramControl vramControl;
        private Controls.BigFileControl bigFileControl;
        private Controls.CseqControl cseqControl;
        private Controls.CtrControl ctrControl;
        private Controls.LevControl levControl;
        private Controls.LangControl langControl;
        private Controls.XaControl xaControl;
    }
}