namespace CTRTools.Controls
{
    partial class InstrumentControl
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
            this.wipeButton = new System.Windows.Forms.Button();
            this.exportWavButton = new System.Windows.Forms.Button();
            this.exportVagButton = new System.Windows.Forms.Button();
            this.replaceSampleButton = new System.Windows.Forms.Button();
            this.addToSfxBank = new System.Windows.Forms.Button();
            this.findFreeIndexButton = new System.Windows.Forms.Button();
            this.octaveUpButton = new System.Windows.Forms.Button();
            this.octaveDownButton = new System.Windows.Forms.Button();
            this.instrumentProperties = new System.Windows.Forms.PropertyGrid();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.wipeButton);
            this.groupBox1.Controls.Add(this.exportWavButton);
            this.groupBox1.Controls.Add(this.exportVagButton);
            this.groupBox1.Controls.Add(this.replaceSampleButton);
            this.groupBox1.Controls.Add(this.addToSfxBank);
            this.groupBox1.Controls.Add(this.findFreeIndexButton);
            this.groupBox1.Controls.Add(this.octaveUpButton);
            this.groupBox1.Controls.Add(this.octaveDownButton);
            this.groupBox1.Controls.Add(this.instrumentProperties);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(468, 346);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Instrument editor";
            // 
            // wipeButton
            // 
            this.wipeButton.Location = new System.Drawing.Point(6, 222);
            this.wipeButton.Name = "wipeButton";
            this.wipeButton.Size = new System.Drawing.Size(165, 23);
            this.wipeButton.TabIndex = 37;
            this.wipeButton.Text = "Wipe sample";
            this.wipeButton.UseVisualStyleBackColor = true;
            this.wipeButton.Click += new System.EventHandler(this.wipeButton_Click);
            // 
            // exportWavButton
            // 
            this.exportWavButton.Location = new System.Drawing.Point(6, 193);
            this.exportWavButton.Name = "exportWavButton";
            this.exportWavButton.Size = new System.Drawing.Size(165, 23);
            this.exportWavButton.TabIndex = 36;
            this.exportWavButton.Text = "Export WAV";
            this.exportWavButton.UseVisualStyleBackColor = true;
            this.exportWavButton.Click += new System.EventHandler(this.exportWavButton_Click);
            // 
            // exportVagButton
            // 
            this.exportVagButton.Location = new System.Drawing.Point(6, 164);
            this.exportVagButton.Name = "exportVagButton";
            this.exportVagButton.Size = new System.Drawing.Size(165, 23);
            this.exportVagButton.TabIndex = 35;
            this.exportVagButton.Text = "Export VAG";
            this.exportVagButton.UseVisualStyleBackColor = true;
            this.exportVagButton.Click += new System.EventHandler(this.exportVagButton_Click);
            // 
            // replaceSampleButton
            // 
            this.replaceSampleButton.Location = new System.Drawing.Point(6, 135);
            this.replaceSampleButton.Name = "replaceSampleButton";
            this.replaceSampleButton.Size = new System.Drawing.Size(165, 23);
            this.replaceSampleButton.TabIndex = 34;
            this.replaceSampleButton.Text = "Import VAG";
            this.replaceSampleButton.UseVisualStyleBackColor = true;
            this.replaceSampleButton.Click += new System.EventHandler(this.replaceSampleButton_Click);
            // 
            // addToSfxBank
            // 
            this.addToSfxBank.Location = new System.Drawing.Point(6, 106);
            this.addToSfxBank.Name = "addToSfxBank";
            this.addToSfxBank.Size = new System.Drawing.Size(165, 23);
            this.addToSfxBank.TabIndex = 33;
            this.addToSfxBank.Text = "Add to SFX bank";
            this.addToSfxBank.UseVisualStyleBackColor = true;
            this.addToSfxBank.Click += new System.EventHandler(this.addToSfxBank_Click);
            // 
            // findFreeIndexButton
            // 
            this.findFreeIndexButton.Location = new System.Drawing.Point(6, 77);
            this.findFreeIndexButton.Name = "findFreeIndexButton";
            this.findFreeIndexButton.Size = new System.Drawing.Size(165, 23);
            this.findFreeIndexButton.TabIndex = 32;
            this.findFreeIndexButton.Text = "Find next free index";
            this.findFreeIndexButton.UseVisualStyleBackColor = true;
            this.findFreeIndexButton.Click += new System.EventHandler(this.findFreeIndexButton_Click);
            // 
            // octaveUpButton
            // 
            this.octaveUpButton.Location = new System.Drawing.Point(6, 19);
            this.octaveUpButton.Name = "octaveUpButton";
            this.octaveUpButton.Size = new System.Drawing.Size(165, 23);
            this.octaveUpButton.TabIndex = 31;
            this.octaveUpButton.Text = "Octave up";
            this.octaveUpButton.UseVisualStyleBackColor = true;
            this.octaveUpButton.Click += new System.EventHandler(this.button2_Click);
            // 
            // octaveDownButton
            // 
            this.octaveDownButton.Location = new System.Drawing.Point(6, 48);
            this.octaveDownButton.Name = "octaveDownButton";
            this.octaveDownButton.Size = new System.Drawing.Size(165, 23);
            this.octaveDownButton.TabIndex = 30;
            this.octaveDownButton.Text = "Octave down";
            this.octaveDownButton.UseVisualStyleBackColor = true;
            this.octaveDownButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // instrumentProperties
            // 
            this.instrumentProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.instrumentProperties.Location = new System.Drawing.Point(177, 19);
            this.instrumentProperties.Name = "instrumentProperties";
            this.instrumentProperties.Size = new System.Drawing.Size(285, 321);
            this.instrumentProperties.TabIndex = 1;
            // 
            // InstrumentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "InstrumentControl";
            this.Size = new System.Drawing.Size(475, 352);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PropertyGrid instrumentProperties;
        private System.Windows.Forms.Button wipeButton;
        private System.Windows.Forms.Button addToSfxBank;
        private System.Windows.Forms.Button findFreeIndexButton;
        private System.Windows.Forms.Button replaceSampleButton;
        private System.Windows.Forms.Button exportVagButton;
        private System.Windows.Forms.Button exportWavButton;
        private System.Windows.Forms.Button octaveUpButton;
        private System.Windows.Forms.Button octaveDownButton;
    }
}
