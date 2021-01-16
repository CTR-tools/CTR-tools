
namespace CTRTools
{
    partial class VramForm
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
            this.vramControl1 = new CTRTools.VramControl();
            this.SuspendLayout();
            // 
            // vramControl1
            // 
            this.vramControl1.AllowDrop = true;
            this.vramControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vramControl1.Location = new System.Drawing.Point(0, 0);
            this.vramControl1.MinimumSize = new System.Drawing.Size(488, 223);
            this.vramControl1.Name = "vramControl1";
            this.vramControl1.Size = new System.Drawing.Size(488, 236);
            this.vramControl1.TabIndex = 0;
            // 
            // VramForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(488, 236);
            this.Controls.Add(this.vramControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(504, 270);
            this.Name = "VramForm";
            this.Text = "VramForm";
            this.ResumeLayout(false);

        }

        #endregion

        private VramControl vramControl1;
    }
}