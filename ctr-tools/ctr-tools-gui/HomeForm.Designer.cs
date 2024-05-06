namespace CTRTools
{
    partial class HomeForm
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
            this.tabArea = new System.Windows.Forms.Panel();
            this.aboutButton = new System.Windows.Forms.Button();
            this.howlButton = new System.Windows.Forms.Button();
            this.xaButton = new System.Windows.Forms.Button();
            this.langButton = new System.Windows.Forms.Button();
            this.levButton = new System.Windows.Forms.Button();
            this.ctrButton = new System.Windows.Forms.Button();
            this.vramButton = new System.Windows.Forms.Button();
            this.fileButton = new System.Windows.Forms.Button();
            this.workArea = new System.Windows.Forms.Panel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tabArea.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabArea
            // 
            this.tabArea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabArea.Controls.Add(this.aboutButton);
            this.tabArea.Controls.Add(this.howlButton);
            this.tabArea.Controls.Add(this.xaButton);
            this.tabArea.Controls.Add(this.langButton);
            this.tabArea.Controls.Add(this.levButton);
            this.tabArea.Controls.Add(this.ctrButton);
            this.tabArea.Controls.Add(this.vramButton);
            this.tabArea.Controls.Add(this.fileButton);
            this.tabArea.Location = new System.Drawing.Point(626, 12);
            this.tabArea.Name = "tabArea";
            this.tabArea.Size = new System.Drawing.Size(124, 486);
            this.tabArea.TabIndex = 1;
            // 
            // aboutButton
            // 
            this.aboutButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.aboutButton.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.aboutButton.Location = new System.Drawing.Point(0, 453);
            this.aboutButton.Name = "aboutButton";
            this.aboutButton.Padding = new System.Windows.Forms.Padding(5);
            this.aboutButton.Size = new System.Drawing.Size(124, 33);
            this.aboutButton.TabIndex = 8;
            this.aboutButton.Text = "About";
            this.aboutButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.aboutButton.Click += new System.EventHandler(this.aboutButton_Click);
            // 
            // howlButton
            // 
            this.howlButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.howlButton.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.howlButton.Location = new System.Drawing.Point(0, 198);
            this.howlButton.Name = "howlButton";
            this.howlButton.Padding = new System.Windows.Forms.Padding(5);
            this.howlButton.Size = new System.Drawing.Size(124, 33);
            this.howlButton.TabIndex = 7;
            this.howlButton.Text = "HOWL music/sfx";
            this.howlButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.howlButton.Click += new System.EventHandler(this.howlButton_Click);
            // 
            // xaButton
            // 
            this.xaButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.xaButton.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.xaButton.Location = new System.Drawing.Point(0, 165);
            this.xaButton.Name = "xaButton";
            this.xaButton.Padding = new System.Windows.Forms.Padding(5);
            this.xaButton.Size = new System.Drawing.Size(124, 33);
            this.xaButton.TabIndex = 6;
            this.xaButton.Text = "XA audio";
            this.xaButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.xaButton.Click += new System.EventHandler(this.xaButton_Click);
            // 
            // langButton
            // 
            this.langButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.langButton.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.langButton.Location = new System.Drawing.Point(0, 132);
            this.langButton.Name = "langButton";
            this.langButton.Padding = new System.Windows.Forms.Padding(5);
            this.langButton.Size = new System.Drawing.Size(124, 33);
            this.langButton.TabIndex = 5;
            this.langButton.Text = "LNG texts";
            this.langButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.langButton.Click += new System.EventHandler(this.langButton_Click);
            // 
            // levButton
            // 
            this.levButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.levButton.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.levButton.Location = new System.Drawing.Point(0, 99);
            this.levButton.Name = "levButton";
            this.levButton.Padding = new System.Windows.Forms.Padding(5);
            this.levButton.Size = new System.Drawing.Size(124, 33);
            this.levButton.TabIndex = 4;
            this.levButton.Text = "LEV scenes";
            this.levButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.levButton.Click += new System.EventHandler(this.levButton_Click);
            // 
            // ctrButton
            // 
            this.ctrButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.ctrButton.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ctrButton.Location = new System.Drawing.Point(0, 66);
            this.ctrButton.Name = "ctrButton";
            this.ctrButton.Padding = new System.Windows.Forms.Padding(5);
            this.ctrButton.Size = new System.Drawing.Size(124, 33);
            this.ctrButton.TabIndex = 3;
            this.ctrButton.Text = "CTR models";
            this.ctrButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ctrButton.Click += new System.EventHandler(this.ctrButton_Click);
            // 
            // vramButton
            // 
            this.vramButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.vramButton.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.vramButton.Location = new System.Drawing.Point(0, 33);
            this.vramButton.Name = "vramButton";
            this.vramButton.Padding = new System.Windows.Forms.Padding(5);
            this.vramButton.Size = new System.Drawing.Size(124, 33);
            this.vramButton.TabIndex = 2;
            this.vramButton.Text = "VRAM textures";
            this.vramButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.vramButton.Click += new System.EventHandler(this.vramButton_Click);
            // 
            // fileButton
            // 
            this.fileButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.fileButton.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.fileButton.Location = new System.Drawing.Point(0, 0);
            this.fileButton.Name = "fileButton";
            this.fileButton.Padding = new System.Windows.Forms.Padding(5);
            this.fileButton.Size = new System.Drawing.Size(124, 33);
            this.fileButton.TabIndex = 1;
            this.fileButton.Text = "BIG archive";
            this.fileButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.fileButton.UseVisualStyleBackColor = true;
            this.fileButton.Click += new System.EventHandler(this.fileButton_Click);
            // 
            // workArea
            // 
            this.workArea.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.workArea.Location = new System.Drawing.Point(12, 12);
            this.workArea.Name = "workArea";
            this.workArea.Size = new System.Drawing.Size(608, 486);
            this.workArea.TabIndex = 2;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 501);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(762, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // HomeForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(762, 523);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.workArea);
            this.Controls.Add(this.tabArea);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(640, 480);
            this.Name = "HomeForm";
            this.Text = "HomeForm";
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.HomeForm_DragEnter);
            this.tabArea.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel tabArea;
        private System.Windows.Forms.Button xaButton;
        private System.Windows.Forms.Button langButton;
        private System.Windows.Forms.Button levButton;
        private System.Windows.Forms.Button ctrButton;
        private System.Windows.Forms.Button vramButton;
        private System.Windows.Forms.Button fileButton;
        private System.Windows.Forms.Button howlButton;
        private System.Windows.Forms.Panel workArea;
        private System.Windows.Forms.Button aboutButton;
        private System.Windows.Forms.StatusStrip statusStrip1;
    }
}