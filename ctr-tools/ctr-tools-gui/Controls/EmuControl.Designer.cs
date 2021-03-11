using System.Windows.Forms;

namespace CTRTools.Controls
{
    partial class EmuControl
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
            this.button28 = new System.Windows.Forms.Button();
            this.propertyGrid2 = new System.Windows.Forms.PropertyGrid();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.actionAttach = new System.Windows.Forms.Button();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            this.SuspendLayout();
            // 
            // button28
            // 
            this.button28.Location = new System.Drawing.Point(3, 343);
            this.button28.Name = "button28";
            this.button28.Size = new System.Drawing.Size(75, 23);
            this.button28.TabIndex = 12;
            this.button28.Text = "button28";
            this.button28.UseVisualStyleBackColor = true;
            this.button28.Click += new System.EventHandler(this.button28_Click);
            // 
            // propertyGrid2
            // 
            this.propertyGrid2.Location = new System.Drawing.Point(260, 32);
            this.propertyGrid2.Name = "propertyGrid2";
            this.propertyGrid2.Size = new System.Drawing.Size(335, 305);
            this.propertyGrid2.TabIndex = 11;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(207, 7);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(61, 17);
            this.checkBox1.TabIndex = 10;
            this.checkBox1.Text = "60 FPS";
            this.checkBox1.UseVisualStyleBackColor = true;
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
            this.comboBox1.Location = new System.Drawing.Point(274, 3);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(171, 21);
            this.comboBox1.TabIndex = 9;
            // 
            // textBox5
            // 
            this.textBox5.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox5.Location = new System.Drawing.Point(3, 32);
            this.textBox5.Multiline = true;
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(251, 305);
            this.textBox5.TabIndex = 8;
            // 
            // actionAttach
            // 
            this.actionAttach.Location = new System.Drawing.Point(3, 3);
            this.actionAttach.Name = "actionAttach";
            this.actionAttach.Size = new System.Drawing.Size(198, 23);
            this.actionAttach.TabIndex = 7;
            this.actionAttach.Text = "Attach";
            this.actionAttach.UseVisualStyleBackColor = true;
            this.actionAttach.Click += new System.EventHandler(this.actionAttach_Click);
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.Location = new System.Drawing.Point(448, 4);
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(147, 20);
            this.numericUpDown2.TabIndex = 13;
            this.numericUpDown2.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // EmuControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.numericUpDown2);
            this.Controls.Add(this.button28);
            this.Controls.Add(this.propertyGrid2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.actionAttach);
            this.Name = "EmuControl";
            this.Size = new System.Drawing.Size(640, 480);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button button28;
        private PropertyGrid propertyGrid2;
        private CheckBox checkBox1;
        private ComboBox comboBox1;
        private TextBox textBox5;
        private Button actionAttach;
        private NumericUpDown numericUpDown2;
    }
}
