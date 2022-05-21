using System.Windows.Forms;

namespace CTRTools.Controls
{
    partial class HowlControl
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
            this.actionLoad = new System.Windows.Forms.Button();
            this.actionExport = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.fbd = new System.Windows.Forms.FolderBrowserDialog();
            this.ofd = new System.Windows.Forms.OpenFileDialog();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.actionSave = new System.Windows.Forms.Button();
            this.sfd = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // actionLoad
            // 
            this.actionLoad.Location = new System.Drawing.Point(3, 3);
            this.actionLoad.Name = "actionLoad";
            this.actionLoad.Size = new System.Drawing.Size(128, 23);
            this.actionLoad.TabIndex = 0;
            this.actionLoad.Text = "Load HOWL";
            this.actionLoad.UseVisualStyleBackColor = true;
            this.actionLoad.Click += new System.EventHandler(this.actionLoad_Click);
            // 
            // actionExport
            // 
            this.actionExport.Location = new System.Drawing.Point(3, 32);
            this.actionExport.Name = "actionExport";
            this.actionExport.Size = new System.Drawing.Size(128, 23);
            this.actionExport.TabIndex = 1;
            this.actionExport.Text = "Export CSEQ";
            this.actionExport.UseVisualStyleBackColor = true;
            this.actionExport.Click += new System.EventHandler(this.actionExport_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(137, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "label1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(137, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "label2";
            // 
            // ofd
            // 
            this.ofd.Filter = "Crash Team Racing HWL file (*.hwl)|*.hwl";
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid1.Location = new System.Drawing.Point(190, 8);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(447, 469);
            this.propertyGrid1.TabIndex = 4;
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(3, 87);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(181, 381);
            this.listBox1.TabIndex = 5;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // actionSave
            // 
            this.actionSave.Location = new System.Drawing.Point(3, 61);
            this.actionSave.Name = "actionSave";
            this.actionSave.Size = new System.Drawing.Size(128, 23);
            this.actionSave.TabIndex = 6;
            this.actionSave.Text = "Write to HOWL";
            this.actionSave.UseVisualStyleBackColor = true;
            this.actionSave.Click += new System.EventHandler(this.actionSave_Click);
            // 
            // sfd
            // 
            this.sfd.FileName = "sfd";
            this.sfd.Filter = "Crash Team Racing HWL file (*.hwl)|*.hwl";
            // 
            // HowlControl
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.actionSave);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.propertyGrid1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.actionExport);
            this.Controls.Add(this.actionLoad);
            this.Name = "HowlControl";
            this.Size = new System.Drawing.Size(640, 480);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.HowlControl_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.HowlControl_DragEnter);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button actionLoad;
        private Button actionExport;
        private Label label1;
        private Label label2;
        private FolderBrowserDialog fbd;
        private OpenFileDialog ofd;
        private PropertyGrid propertyGrid1;
        private ListBox listBox1;
        private Button actionSave;
        private SaveFileDialog sfd;
    }
}
