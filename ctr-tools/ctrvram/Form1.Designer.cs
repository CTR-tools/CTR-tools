namespace WindowsFormsApplication1
{
    partial class Form1
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
            this.selBitMode = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // selBitMode
            // 
            this.selBitMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.selBitMode.FormattingEnabled = true;
            this.selBitMode.Items.AddRange(new object[] {
            "Indexed4",
            "Indexed8",
            "Colored16"});
            this.selBitMode.Location = new System.Drawing.Point(12, 47);
            this.selBitMode.Name = "selBitMode";
            this.selBitMode.Size = new System.Drawing.Size(156, 21);
            this.selBitMode.TabIndex = 2;
            this.selBitMode.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1024, 512);
            this.Controls.Add(this.selBitMode);
            this.Name = "Form1";
            this.Text = "CTR VRAM (drop .vram file to view, Ctrl+S to save)";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox selBitMode;

    }
}

