
namespace CTRTools.Controls
{
    partial class VertexArrayControl
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.rainbowButton = new System.Windows.Forms.Button();
            this.darkenButton = new System.Windows.Forms.Button();
            this.blueSlider = new System.Windows.Forms.TrackBar();
            this.greenSlider = new System.Windows.Forms.TrackBar();
            this.redSlider = new System.Windows.Forms.TrackBar();
            this.setMainColorButton = new System.Windows.Forms.Button();
            this.mainToMorphButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.setMorphColorButton = new System.Windows.Forms.Button();
            this.applyColorsButton = new System.Windows.Forms.Button();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.cd = new System.Windows.Forms.ColorDialog();
            this.label4 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.blueSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.greenSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.redSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.rainbowButton);
            this.groupBox1.Controls.Add(this.darkenButton);
            this.groupBox1.Controls.Add(this.blueSlider);
            this.groupBox1.Controls.Add(this.greenSlider);
            this.groupBox1.Controls.Add(this.redSlider);
            this.groupBox1.Controls.Add(this.setMainColorButton);
            this.groupBox1.Controls.Add(this.mainToMorphButton);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.setMorphColorButton);
            this.groupBox1.Controls.Add(this.applyColorsButton);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(256, 355);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Vertex colors";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 263);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(123, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "Randomize vertex colors";
            // 
            // rainbowButton
            // 
            this.rainbowButton.Location = new System.Drawing.Point(9, 279);
            this.rainbowButton.Name = "rainbowButton";
            this.rainbowButton.Size = new System.Drawing.Size(75, 23);
            this.rainbowButton.TabIndex = 17;
            this.rainbowButton.Text = "Rainbow";
            this.rainbowButton.UseVisualStyleBackColor = true;
            this.rainbowButton.Click += new System.EventHandler(this.rainbowButton_Click);
            // 
            // darkenButton
            // 
            this.darkenButton.Location = new System.Drawing.Point(9, 202);
            this.darkenButton.Name = "darkenButton";
            this.darkenButton.Size = new System.Drawing.Size(75, 23);
            this.darkenButton.TabIndex = 9;
            this.darkenButton.Text = "Darken";
            this.darkenButton.UseVisualStyleBackColor = true;
            this.darkenButton.Click += new System.EventHandler(this.darkenButton_Click);
            // 
            // blueSlider
            // 
            this.blueSlider.Location = new System.Drawing.Point(9, 164);
            this.blueSlider.Maximum = 255;
            this.blueSlider.Name = "blueSlider";
            this.blueSlider.Size = new System.Drawing.Size(234, 45);
            this.blueSlider.TabIndex = 16;
            this.blueSlider.TickFrequency = 32;
            this.blueSlider.Value = 255;
            // 
            // greenSlider
            // 
            this.greenSlider.Location = new System.Drawing.Point(9, 128);
            this.greenSlider.Maximum = 255;
            this.greenSlider.Name = "greenSlider";
            this.greenSlider.Size = new System.Drawing.Size(234, 45);
            this.greenSlider.TabIndex = 15;
            this.greenSlider.TickFrequency = 32;
            this.greenSlider.Value = 255;
            // 
            // redSlider
            // 
            this.redSlider.Location = new System.Drawing.Point(9, 93);
            this.redSlider.Maximum = 255;
            this.redSlider.Name = "redSlider";
            this.redSlider.Size = new System.Drawing.Size(234, 45);
            this.redSlider.TabIndex = 14;
            this.redSlider.TickFrequency = 32;
            this.redSlider.Value = 255;
            // 
            // setMainColorButton
            // 
            this.setMainColorButton.Location = new System.Drawing.Point(9, 43);
            this.setMainColorButton.Name = "setMainColorButton";
            this.setMainColorButton.Size = new System.Drawing.Size(75, 23);
            this.setMainColorButton.TabIndex = 12;
            this.setMainColorButton.Text = "Main color";
            this.setMainColorButton.UseVisualStyleBackColor = true;
            this.setMainColorButton.Click += new System.EventHandler(this.setMainColorButton_Click);
            // 
            // mainToMorphButton
            // 
            this.mainToMorphButton.Location = new System.Drawing.Point(9, 231);
            this.mainToMorphButton.Name = "mainToMorphButton";
            this.mainToMorphButton.Size = new System.Drawing.Size(156, 23);
            this.mainToMorphButton.TabIndex = 12;
            this.mainToMorphButton.Text = "Copy main color to morph";
            this.mainToMorphButton.UseVisualStyleBackColor = true;
            this.mainToMorphButton.Click += new System.EventHandler(this.mainToMorphButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(184, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Recolor all vertices with a single color";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 77);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(221, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Darkens vertex colors based on slider values:";
            // 
            // setMorphColorButton
            // 
            this.setMorphColorButton.Location = new System.Drawing.Point(87, 43);
            this.setMorphColorButton.Name = "setMorphColorButton";
            this.setMorphColorButton.Size = new System.Drawing.Size(75, 23);
            this.setMorphColorButton.TabIndex = 8;
            this.setMorphColorButton.Text = "Morph color";
            this.setMorphColorButton.UseVisualStyleBackColor = true;
            this.setMorphColorButton.Click += new System.EventHandler(this.setMorphColorButton_Click);
            // 
            // applyColorsButton
            // 
            this.applyColorsButton.Location = new System.Drawing.Point(168, 43);
            this.applyColorsButton.Name = "applyColorsButton";
            this.applyColorsButton.Size = new System.Drawing.Size(75, 23);
            this.applyColorsButton.TabIndex = 7;
            this.applyColorsButton.Text = "Apply colors";
            this.applyColorsButton.UseVisualStyleBackColor = true;
            this.applyColorsButton.Click += new System.EventHandler(this.applyColorsButton_Click);
            // 
            // textBox3
            // 
            this.textBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox3.Location = new System.Drawing.Point(265, 3);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(480, 355);
            this.textBox3.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(151, 263);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 13);
            this.label4.TabIndex = 19;
            this.label4.Text = "Load light map";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(154, 279);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 20;
            this.button1.Text = "Load image";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(265, 80);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(274, 278);
            this.pictureBox1.TabIndex = 13;
            this.pictureBox1.TabStop = false;
            // 
            // VertexArrayControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.groupBox1);
            this.Name = "VertexArrayControl";
            this.Size = new System.Drawing.Size(748, 361);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.blueSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.greenSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.redSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button setMainColorButton;
        private System.Windows.Forms.Button mainToMorphButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button darkenButton;
        private System.Windows.Forms.Button setMorphColorButton;
        private System.Windows.Forms.Button applyColorsButton;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.ColorDialog cd;
        private System.Windows.Forms.TrackBar blueSlider;
        private System.Windows.Forms.TrackBar greenSlider;
        private System.Windows.Forms.TrackBar redSlider;
        private System.Windows.Forms.Button rainbowButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}
