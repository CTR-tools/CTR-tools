namespace CTRTools.Controls
{
    partial class AboutControl
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.warning = new System.Windows.Forms.Label();
            this.signLabel = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.appLogo = new System.Windows.Forms.PictureBox();
            this.discordLogo = new System.Windows.Forms.PictureBox();
            this.githubLogo = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.appLogo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.discordLogo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.githubLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.warning);
            this.panel1.Controls.Add(this.signLabel);
            this.panel1.Controls.Add(this.labelVersion);
            this.panel1.Controls.Add(this.appLogo);
            this.panel1.Controls.Add(this.discordLogo);
            this.panel1.Controls.Add(this.githubLogo);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(639, 423);
            this.panel1.TabIndex = 0;
            // 
            // warning
            // 
            this.warning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.warning.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.warning.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.warning.Location = new System.Drawing.Point(3, 319);
            this.warning.Name = "warning";
            this.warning.Size = new System.Drawing.Size(633, 104);
            this.warning.TabIndex = 22;
            this.warning.Text = "Warning! This tool is in the early stage of development.\r\nSome features may be mi" +
    "ssing or won\'t work as intended.";
            this.warning.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // signLabel
            // 
            this.signLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.signLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.signLabel.Location = new System.Drawing.Point(3, 295);
            this.signLabel.Name = "signLabel";
            this.signLabel.Size = new System.Drawing.Size(633, 24);
            this.signLabel.TabIndex = 21;
            this.signLabel.Text = "Signature from DLL";
            this.signLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelVersion
            // 
            this.labelVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.labelVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelVersion.Location = new System.Drawing.Point(3, 181);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(633, 24);
            this.labelVersion.TabIndex = 20;
            this.labelVersion.Text = "Framework version from DLL";
            this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // appLogo
            // 
            this.appLogo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.appLogo.ErrorImage = null;
            this.appLogo.Image = global::CTRTools.Properties.Resources.ctr_tools_logo;
            this.appLogo.Location = new System.Drawing.Point(3, 94);
            this.appLogo.Name = "appLogo";
            this.appLogo.Size = new System.Drawing.Size(633, 84);
            this.appLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.appLogo.TabIndex = 17;
            this.appLogo.TabStop = false;
            // 
            // discordLogo
            // 
            this.discordLogo.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.discordLogo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.discordLogo.Image = global::CTRTools.Properties.Resources.icon_discord;
            this.discordLogo.InitialImage = null;
            this.discordLogo.Location = new System.Drawing.Point(318, 217);
            this.discordLogo.Name = "discordLogo";
            this.discordLogo.Size = new System.Drawing.Size(64, 64);
            this.discordLogo.TabIndex = 19;
            this.discordLogo.TabStop = false;
            this.discordLogo.Click += new System.EventHandler(this.discordLogo_Click);
            // 
            // githubLogo
            // 
            this.githubLogo.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.githubLogo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.githubLogo.ErrorImage = null;
            this.githubLogo.Image = global::CTRTools.Properties.Resources.icon_github;
            this.githubLogo.Location = new System.Drawing.Point(239, 217);
            this.githubLogo.Name = "githubLogo";
            this.githubLogo.Size = new System.Drawing.Size(64, 64);
            this.githubLogo.TabIndex = 18;
            this.githubLogo.TabStop = false;
            this.githubLogo.Click += new System.EventHandler(this.githubLogo_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.Color.Silver;
            this.label1.Location = new System.Drawing.Point(7, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(629, 91);
            this.label1.TabIndex = 23;
            this.label1.Text = "Drag and drop CTR game files, if the file is supported, it should automatically s" +
    "witch to proper tab.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AboutControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.Name = "AboutControl";
            this.Size = new System.Drawing.Size(639, 423);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.appLogo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.discordLogo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.githubLogo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label warning;
        private System.Windows.Forms.Label signLabel;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.PictureBox appLogo;
        private System.Windows.Forms.PictureBox discordLogo;
        private System.Windows.Forms.PictureBox githubLogo;
        private System.Windows.Forms.Label label1;
    }
}
