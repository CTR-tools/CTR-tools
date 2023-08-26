namespace CTRTools.Controls
{
    partial class SampleControl
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
            this.instrumentProperties = new System.Windows.Forms.PropertyGrid();
            this.wipeButton = new System.Windows.Forms.Button();
            this.addToSfxBank = new System.Windows.Forms.Button();
            this.findFreeIndexButton = new System.Windows.Forms.Button();
            this.replaceSampleButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.wipeButton);
            this.groupBox1.Controls.Add(this.addToSfxBank);
            this.groupBox1.Controls.Add(this.findFreeIndexButton);
            this.groupBox1.Controls.Add(this.replaceSampleButton);
            this.groupBox1.Controls.Add(this.instrumentProperties);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(349, 387);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Instrument editor";
            // 
            // instrumentProperties
            // 
            this.instrumentProperties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.instrumentProperties.Location = new System.Drawing.Point(6, 19);
            this.instrumentProperties.Name = "instrumentProperties";
            this.instrumentProperties.Size = new System.Drawing.Size(337, 304);
            this.instrumentProperties.TabIndex = 1;
            // 
            // wipeButton
            // 
            this.wipeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.wipeButton.Location = new System.Drawing.Point(177, 358);
            this.wipeButton.Name = "wipeButton";
            this.wipeButton.Size = new System.Drawing.Size(165, 23);
            this.wipeButton.TabIndex = 23;
            this.wipeButton.Text = "Wipe sample";
            this.wipeButton.UseVisualStyleBackColor = true;
            // 
            // addToSfxBank
            // 
            this.addToSfxBank.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.addToSfxBank.Location = new System.Drawing.Point(6, 358);
            this.addToSfxBank.Name = "addToSfxBank";
            this.addToSfxBank.Size = new System.Drawing.Size(165, 23);
            this.addToSfxBank.TabIndex = 22;
            this.addToSfxBank.Text = "Add to SFX bank";
            this.addToSfxBank.UseVisualStyleBackColor = true;
            // 
            // findFreeIndexButton
            // 
            this.findFreeIndexButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.findFreeIndexButton.Location = new System.Drawing.Point(177, 329);
            this.findFreeIndexButton.Name = "findFreeIndexButton";
            this.findFreeIndexButton.Size = new System.Drawing.Size(165, 23);
            this.findFreeIndexButton.TabIndex = 21;
            this.findFreeIndexButton.Text = "Find next free index";
            this.findFreeIndexButton.UseVisualStyleBackColor = true;
            // 
            // replaceSampleButton
            // 
            this.replaceSampleButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.replaceSampleButton.Location = new System.Drawing.Point(6, 329);
            this.replaceSampleButton.Name = "replaceSampleButton";
            this.replaceSampleButton.Size = new System.Drawing.Size(165, 23);
            this.replaceSampleButton.TabIndex = 20;
            this.replaceSampleButton.Text = "Import VAG";
            this.replaceSampleButton.UseVisualStyleBackColor = true;
            // 
            // SampleControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "SampleControl";
            this.Size = new System.Drawing.Size(355, 393);
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
    }
}
