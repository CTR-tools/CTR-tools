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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.actionExport = new System.Windows.Forms.Button();
            this.actionLoad = new System.Windows.Forms.Button();
            this.fbd = new System.Windows.Forms.FolderBrowserDialog();
            this.ofd = new System.Windows.Forms.OpenFileDialog();
            this.actionSave = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.sfd = new System.Windows.Forms.SaveFileDialog();
            this.treeBanks = new System.Windows.Forms.TreeView();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.treeBanks);
            this.groupBox1.Controls.Add(this.propertyGrid1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.actionExport);
            this.groupBox1.Controls.Add(this.actionLoad);
            this.groupBox1.Controls.Add(this.actionSave);
            this.groupBox1.Controls.Add(this.listBox1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(640, 480);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Howl";
            this.groupBox1.DragDrop += new System.Windows.Forms.DragEventHandler(this.HowlControl_DragDrop);
            this.groupBox1.DragEnter += new System.Windows.Forms.DragEventHandler(this.HowlControl_DragEnter);
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid1.Location = new System.Drawing.Point(193, 24);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(441, 450);
            this.propertyGrid1.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(140, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "label2";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(140, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "label1";
            // 
            // actionExport
            // 
            this.actionExport.Location = new System.Drawing.Point(6, 48);
            this.actionExport.Name = "actionExport";
            this.actionExport.Size = new System.Drawing.Size(128, 23);
            this.actionExport.TabIndex = 9;
            this.actionExport.Text = "Export CSEQ";
            this.actionExport.UseVisualStyleBackColor = true;
            this.actionExport.Click += new System.EventHandler(this.actionExport_Click);
            // 
            // actionLoad
            // 
            this.actionLoad.Location = new System.Drawing.Point(6, 19);
            this.actionLoad.Name = "actionLoad";
            this.actionLoad.Size = new System.Drawing.Size(128, 23);
            this.actionLoad.TabIndex = 8;
            this.actionLoad.Text = "Load HOWL";
            this.actionLoad.UseVisualStyleBackColor = true;
            this.actionLoad.Click += new System.EventHandler(this.actionLoad_Click);
            // 
            // ofd
            // 
            this.ofd.Filter = "Crash Team Racing HWL file (*.hwl)|*.hwl";
            // 
            // actionSave
            // 
            this.actionSave.Location = new System.Drawing.Point(6, 77);
            this.actionSave.Name = "actionSave";
            this.actionSave.Size = new System.Drawing.Size(128, 23);
            this.actionSave.TabIndex = 14;
            this.actionSave.Text = "Write to HOWL";
            this.actionSave.UseVisualStyleBackColor = true;
            this.actionSave.Click += new System.EventHandler(this.actionSave_Click);
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(6, 103);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(181, 355);
            this.listBox1.TabIndex = 13;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // sfd
            // 
            this.sfd.FileName = "sfd";
            this.sfd.Filter = "Crash Team Racing HWL file (*.hwl)|*.hwl";
            // 
            // treeBanks
            // 
            this.treeBanks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeBanks.Location = new System.Drawing.Point(392, 53);
            this.treeBanks.Name = "treeBanks";
            this.treeBanks.Size = new System.Drawing.Size(242, 380);
            this.treeBanks.TabIndex = 15;
            this.treeBanks.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeBanks_AfterSelect);
            // 
            // HowlControl
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "HowlControl";
            this.Size = new System.Drawing.Size(640, 480);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.HowlControl_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.HowlControl_DragEnter);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private GroupBox groupBox1;
        private TreeView treeBanks;
        private PropertyGrid propertyGrid1;
        private Label label2;
        private Label label1;
        private Button actionExport;
        private Button actionLoad;
        private Button actionSave;
        private ListBox listBox1;
        private FolderBrowserDialog fbd;
        private OpenFileDialog ofd;
        private SaveFileDialog sfd;
    }
}
